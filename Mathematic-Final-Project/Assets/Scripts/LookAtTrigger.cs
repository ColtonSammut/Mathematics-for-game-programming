using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class LookAtTrigger : MonoBehaviour
{
    [Range(-1f, 1f)] 
    public float angleDeg = 0.8f;
    public float rotationSpeed = 2f; // Add rotation speed parameter


    //private Quaternion Rotation; // Track target rotation
    public GameObject TargetObject;
    public GameObject LookAtObject;


    private bool lookAt = false;



    private void Start()
    {
        
    }
    private void Update()
    {
        Vector3 vecTriggerPos = transform.position; // Enemy position
        Vector3 vecTargetPos = TargetObject.transform.position; // Player position
        Vector3 v = vecTargetPos - vecTriggerPos; // Direction to player
        Vector3 l = LookAtObject.transform.position - vecTriggerPos; // LookAt direction

        // Normalize and scale by radius
        Vector3 l_hat = l.normalized;
        Vector3 v_hat = v.normalized;

        

        // Calculate the angle between v_hat and l_hat
        float dotProduct = Vector3.Dot(v_hat, l_hat);
        float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg; // Convert to degrees
        float angleDegrees = Mathf.Acos(angleDeg) * Mathf.Rad2Deg;
        
        // Change color if within 15 degrees
        if (angle <= angleDegrees)
        {

            transform.forward = Vector3.Lerp(transform.forward, v, rotationSpeed * Time.deltaTime);
        }
        else
        {
            Quaternion quat = Quaternion.LookRotation(LookAtObject.transform.position);

            gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, quat, rotationSpeed * Time.deltaTime);
        }


        Quaternion leftRotation = Quaternion.Euler(0, angleDegrees, 0); // Rotate set degrees around the axis
        Quaternion rightRotation = Quaternion.Euler(0, -angleDegrees, 0); // Rotate set -degrees around the axis

        Vector3 leftThreshold = leftRotation * l_hat;
        Vector3 rightThreshold = rightRotation * l_hat;

        
    }
}