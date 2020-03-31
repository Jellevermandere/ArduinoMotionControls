using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports; // this enables the IO port namespace

// ************* This script manages the Arduino Communication ******************* //
// tutorial I learned this from: https://playground.arduino.cc/Main/MPU-6050/#measurements //
// arduino script at the bottom //

public class ArduinoConnector : MonoBehaviour
{
    [Header("ConnectionSettings")]
    public bool useArduino;
    public string IOPort = "/dev/cu.HC05-SPPDev"; // Change this to whatever port your Arduino is connected to, this is the port for the specefic bluetooth adaptor used (HC-05 Wireless Bluetooth RF Transceiver)
    public int baudeRate = 9600; //this must match the bauderate of the Arduino script
    public float accelerometerNormFactor = 0.00025f;
    public float gyroNormFactor = 1.0f / 32768.0f;


    [HideInInspector]
	public SerialPort sp;

    private string recievedValue;

    [HideInInspector]
    public Vector3 downDirection;
    [HideInInspector]
    public float xAngle = 0;
    [HideInInspector]
    public float yAngle = 0;
    [HideInInspector]
    public float zAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (useArduino) ActivateSP();
    }

    // Update is called once per frame
    void Update()
    {

        if (useArduino && sp.IsOpen)
        {
            try
            {
                recievedValue = sp.ReadLine(); //reads the serial input
                Debug.Log(recievedValue);
                SetDirection(recievedValue); //translates the string into a Vector
            }
            catch (System.Exception)
            {

            }
        }

    }

    void ActivateSP()
    {
        sp = new SerialPort(IOPort, baudeRate, Parity.None, 8, StopBits.One);

        sp.Open();
        sp.ReadTimeout = 25;
    }


    void SetDirection(string message)
    {
        // InputStringFormat:  "AcX,AcY,AcZ,GyX,GyY,GyZ"
        char seperator = ',';
        string[] values = message.Split(seperator);


     //Acceleromter values
        // normalized accelerometer values
        float ax = int.Parse(values[0]) * accelerometerNormFactor;
        float ay = int.Parse(values[1]) * accelerometerNormFactor;
        float az = int.Parse(values[2]) * accelerometerNormFactor;

        // prevent
        //if (Mathf.Abs(ax) - 1 < 0) ax = 0;
        //if (Mathf.Abs(ay) - 1 < 0) ay = 0;
        //if (Mathf.Abs(az) - 1 < 0) az = 1;

        downDirection = Vector3.Normalize(new Vector3(-ax, ay, -az)); // this vector is used to determin the rotation of the accelerometer


     //Gyroscope Values
        // normalized gyrocope values
        float gx = int.Parse(values[3]) * gyroNormFactor;
        float gy = int.Parse(values[4]) * gyroNormFactor;
        float gz = int.Parse(values[5]) * gyroNormFactor;

        // Eliminate Noise
        if (Mathf.Abs(gx) < 0.025f) gx = 0f;
        if (Mathf.Abs(gy) < 0.025f) gy = 0f;
        if (Mathf.Abs(gz) < 0.025f) gz = 0f;

        xAngle += gx; //these values display the angles of the accelerometer, These will drift over time
        yAngle += gy;
        zAngle += gz;


        

    }

}

/* Put this code on your arduino 

    // MPU-6050 Gyroscope & accelerometer read-out to Unity
    // By Arduino User JohnChi // modified by Jelle Vermandere
    // March 21, 2020
    // Public Domain
    #include<Wire.h>
    const int MPU=0x68; 
    int16_t AcX,AcY,AcZ,Tmp,GyX,GyY,GyZ;

    void setup(){
      Wire.begin();
      Wire.beginTransmission(MPU);
      Wire.write(0x6B); 
      Wire.write(0);    
      Wire.endTransmission(true);
      Serial.begin(9600);
    }
    void loop(){
      Wire.beginTransmission(MPU);
      Wire.write(0x3B);  
      Wire.endTransmission(false);
      Wire.requestFrom(MPU,12,true);  
      AcX=Wire.read()<<8|Wire.read();    
      AcY=Wire.read()<<8|Wire.read();  
      AcZ=Wire.read()<<8|Wire.read();  
      GyX=Wire.read()<<8|Wire.read();  
      GyY=Wire.read()<<8|Wire.read();  
      GyZ=Wire.read()<<8|Wire.read();  
  
      Serial.print(AcX); Serial.print(",");
      Serial.print(AcY); Serial.print(",");
      Serial.print(AcZ); Serial.print(",");

      Serial.print(GyX); Serial.print(",");
      Serial.print(GyY); Serial.print(",");
      Serial.print(GyZ); 
      Serial.println("");
      Serial.flush();
      delay(25);
    }
*/
