using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BezierPoint : MonoBehaviour
{
    public GameObject Control1;
    public GameObject Control2;

    public bool DrawLines = true;
    public bool DrawPoints = true;

    public Vector3 getAnchor()
    {
        return transform.position;
    }

    public Vector3 getControl1()
    {
        return Control1.transform.position;
    }

    public Vector3 getControl2()
    {
        return Control2.transform.position;
    }

    private void OnDrawGizmos()
    {

        if (DrawLines)
        {
            Handles.color = Color.white;
            Handles.DrawLine(transform.position, Control1.transform.position, 2f);
            Handles.DrawLine(transform.position, Control2.transform.position, 2f);
        }

        if (DrawPoints)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(Control1.transform.position, 0.3f);
            Gizmos.DrawSphere(Control2.transform.position, 0.3f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.3f);

        }

    }

}
