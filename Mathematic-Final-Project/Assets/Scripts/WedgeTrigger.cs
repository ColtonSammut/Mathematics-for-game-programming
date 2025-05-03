using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class WedgeTrigger : MonoBehaviour
{
    [Range(0.1f, 20f)]
    public float Radius = 5.0f;
    
    [Range(-1f, 1f)] 
    public float ThresholdAngle = 0.8f;

    public GameObject TargetObject;
    public GameObject LookAtObject;

    private void OnDrawGizmos()
    {
        Vector3 vecTriggerPos = transform.position; // Enemy position
        Vector3 vecTargetPos = TargetObject.transform.position; // Player position
        Vector3 v = vecTargetPos - vecTriggerPos; // Direction to player
        Vector3 l = LookAtObject.transform.position - vecTriggerPos; // LookAt direction

        // Normalize and scale by radius
        Vector3 l_hat = l.normalized;
        Vector3 v_hat = v.normalized;

        //Vector3 l_hat = l / l.magnitude; //unit vector
        //Vector3 v_hat = v / v.magnitude; //unit vector

        MyDraw.DrawVectorAt(vecTriggerPos, v, Color.magenta, 4.0f);
        MyDraw.DrawVectorAt(vecTriggerPos, l, new Color32(0xF0, 0xA5, 0xA0, 0xFF), 4.0f);

        MyDraw.DrawVectorAt(vecTriggerPos, v_hat, Color.magenta, 4.0f);
        MyDraw.DrawVectorAt(vecTriggerPos, l_hat, new Color32(0xF0, 0xA5, 0xA0, 0xFF), 4.0f);

        // Calculate the angle between v_hat and l_hat
        float dotProduct = Vector3.Dot(v_hat, l_hat);
        float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg; // Convert to degrees
        float angleDegrees = Mathf.Acos(ThresholdAngle) * Mathf.Rad2Deg;
        
        Debug.Log("dotProduct between v_hat and l_hat: " + dotProduct);
        Debug.Log("angle between v_hat and l_hat: " + angle);
        
        
        // Draw threshold lines using Quaternion.Euler
        Quaternion leftRotation = Quaternion.Euler(0, angleDegrees, 0); // Rotate set degrees around the axis
        Quaternion rightRotation = Quaternion.Euler(0, -angleDegrees, 0); // Rotate set -degrees around the axis

        Vector3 leftThreshold = leftRotation * l_hat;
        Vector3 rightThreshold = rightRotation * l_hat;

        MyDraw.DrawVectorAt(vecTriggerPos, leftThreshold, Color.yellow, 4.0f);
        MyDraw.DrawVectorAt(vecTriggerPos, rightThreshold, Color.yellow, 4.0f);
        
        
        
        // Change color if within  Threshold degrees
        if (angle <= ThresholdAngle)
        {
            Handles.color = Color.red;
        }
        else
        {
            Handles.color = Color.magenta;
        }

        
        //Handles.DrawWireDisc(vecTriggerPos, Vector3.up, Mathf.Abs(1f), 3.0f);
        Handles.DrawWireArc(vecTriggerPos, Vector3.up, rightThreshold, angleDegrees * 2, Radius, 3.0f);
        
    }
    
    
    
}

//