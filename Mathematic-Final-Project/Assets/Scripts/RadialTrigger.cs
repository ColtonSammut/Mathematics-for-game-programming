using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class RadialTrigger : MonoBehaviour
{
    //Modify this in the end
    [Range(0.1f, 20f)]
    public float radius = 5.0f;
    
    
    public GameObject TargetObject;


    private void OnDrawGizmos()
    {
        Vector3 vecTriggerPos = transform.position; // getting position of enemy 
        Vector3 vecTargetPos = TargetObject.transform.position; // getting position of player
        Vector3 v = vecTargetPos - vecTriggerPos; //getting position of -> point from Trigger to Target

        MyDraw.DrawVectorAt(Vector3.zero, vecTriggerPos, Color.white, 3.0f);
        MyDraw.DrawVectorAt(Vector3.zero, vecTargetPos, Color.white, 3.0f);

        MyDraw.DrawVectorAt(vecTriggerPos, v, Color.magenta, 4.0f);

        
        // Draw the disc with Radius
        if (v.magnitude < radius)
        {
            Handles.color = Color.blue;
            Handles.DrawWireDisc(vecTriggerPos, new Vector3(0, 1, 0), radius, 3.0f);
        }
        else
        {
            Handles.color = Color.magenta;
            Handles.DrawWireDisc(vecTriggerPos, new Vector3(0, 1, 0), radius, 3.0f); 

        }
    }
}



