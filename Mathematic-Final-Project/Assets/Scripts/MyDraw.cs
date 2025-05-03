using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class MyDraw
{
    public static void DrawVectorAt(Vector3 pos, Vector3 vec, Color c, float thickness = 1.0f)
    {
        Color orig = Handles.color;  // save the current color
        Handles.color = c;
        Handles.DrawLine(pos, pos + vec, thickness);

        Handles.ConeHandleCap(0, pos + vec - 0.2f * vec.normalized, Quaternion.LookRotation(vec), 0.286f, EventType.Repaint);
        Handles.color = orig;  // restore the original color
    }
}


