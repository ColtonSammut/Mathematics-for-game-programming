using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class Mesh2D
{
    public Mesh mesh;
    public Vector3[] vertices;
    public int[] triangles;

    public Mesh2D(List<Vector3> points)
    {
        vertices = points.ToArray();
    }
}
