using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class LookAtTrigger : MonoBehaviour
{
    [Range(-1f, 1f)] 
    public float angleDeg = 0.8f;

    [Range(0f, 100f)]
    public float rotationSmothing = 0.5f; // Add rotation speed parameter


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

            float t = Time.time / rotationSmothing;



            //Ease in and out
            t = t * t; 
            if (t > 1f)
            {
                t = 1f;
            }

            EasingFunction.Function function = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseInBounce);

            t = function(0, 1, t);

            transform.forward = Vector3.Lerp(transform.forward, v, t * Time.deltaTime);
        }
        else
        {


            float t = Time.time / rotationSmothing;


            t = t * t * t * t * t; 
            if (t > 1f)
            {
                t = 1f;
            }


            //EasingFunction.Function function = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseInBounce);

            //t = function(0, 1, t);

            Quaternion quat = Quaternion.LookRotation(LookAtObject.transform.position);

            gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, quat, t * Time.deltaTime);
        }


        Quaternion leftRotation = Quaternion.Euler(0, angleDegrees, 0); // Rotate set degrees around the axis
        Quaternion rightRotation = Quaternion.Euler(0, -angleDegrees, 0); // Rotate set -degrees around the axis

        Vector3 leftThreshold = leftRotation * l_hat;
        Vector3 rightThreshold = rightRotation * l_hat;

        
    }
}