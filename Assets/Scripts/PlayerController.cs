using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ************* This script manages the Player moement ******************* //

[RequireComponent (typeof (ArduinoConnector), typeof (Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float speed = 400;
    public float rotSpeed = 150;
    public float gravity = 20;


    private Rigidbody rb;
    private ArduinoConnector connector;
    private Vector3 direction;


    // Start is called before the first frame update
    void Start()
    {
        connector = GetComponent<ArduinoConnector>();
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        direction = connector.downDirection; //fetches the current accelerometer direction to calculate the movement
    }

    private void FixedUpdate()
    {
        ApplyGravity(); //applies a certain gravity force
        RotateDown();   // applies a torque to rotate the player straight up
        MovePlayer();   // moves the player according to the vector
        
    }
    private void ApplyGravity() 
    {
        rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }

    private void MovePlayer()
    {
        rb.AddForce(transform.forward * direction.z * speed * Time.deltaTime, ForceMode.Impulse);
        rb.rotation *= Quaternion.Euler(0, direction.x * rotSpeed * Time.deltaTime, 0);
    }

    private void RotateDown()
    {
        float angle = Vector3.Angle(transform.up, Vector3.up);
        if (angle > 0.001)
        {
            Vector3 axis = Vector3.Cross(transform.up, Vector3.up);
            rb.AddTorque(axis * angle * 100);
        }
    }
}
