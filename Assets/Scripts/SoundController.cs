using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ************* This script manages the microphone input ******************* //
// tutorial I learned this from: https://www.youtube.com/watch?v=GHc9RF258VA //

public class SoundController : MonoBehaviour
{
    public float sensitivity;
    public Slider volumeSlider;
    public Slider sensitivitySlider;
    private AudioSource _audioSource;
    public Rigidbody rb; // used to apply force to player, use something else if you want

    //microphone input
    public bool useMicrophone;
    public string selectedDevice;

    public float RmsValue;
    public float DbValue;
    public float sum;

    private const int QSamples = 256;
    private const float RefValue = 0.1f;

    public float[] _samples;
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _samples = new float[QSamples];
        sensitivitySlider.value = sensitivity;

        //microphone set up
        if (useMicrophone)
        {
            if (Microphone.devices.Length > 0)
            {

                Debug.Log(Microphone.devices[0].ToString());
                selectedDevice = Microphone.devices[0].ToString();
                _audioSource.clip = Microphone.Start(selectedDevice, true, 1, 22050);
                _audioSource.loop = true;

                if (Microphone.IsRecording(selectedDevice))
                {
                    while (!(Microphone.GetPosition(selectedDevice) > 0)) { }
                    Debug.Log("start playing... position is " + Microphone.GetPosition(selectedDevice));
                }

            }
            else useMicrophone = false;
        }

        _audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        //Debug.Log(DbValue);
        volumeSlider.value = DbValue;

        if (DbValue > sensitivity || Input.GetKey("space"))
        {
            rb.AddForce(Vector3.forward * 20 * Time.deltaTime, ForceMode.VelocityChange);
        }
    }

    public void AdjustSensitivity(float input)
    {
        sensitivity = input;
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetOutputData(_samples, 0); // fill array with samples
        int i;
        sum = 0;
        for (i = 0; i < QSamples; i++)
        {
            sum += _samples[i] * _samples[i]; // sum squared samples
        }
        RmsValue = Mathf.Sqrt(sum / QSamples); // rms = square root of average
        DbValue = 20 * Mathf.Log10(RmsValue / RefValue); // calculate dB
        if (DbValue < -160) DbValue = -160; // clamp it to -160dB min
                                            // get sound spectrum
    }
}
