using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using Color = UnityEngine.Color;

public class BezierRoad : MonoBehaviour
{
    List<Vector3> Vertices;

    public GameObject[] Points;

    public Mesh2D CrossSection;


    [Range(0f, 1f)]
    public float T = 0f;

    [Range(10, 10000)]
    public int RoadSegments = 100;

    [Range(0.1f, 10f)]
    public float RoadScaler = 1f;

    public bool DrawCrossSections, DrawVertices, DrawConnectors, ClosedLoop = true;

    private int BezSegment => ClosedLoop ? Points.Length : Points.Length - 1;


    private Mesh mesh;

    private int GetBezSegment(float t)
    {


        float segment = t * BezSegment;

        return Mathf.FloorToInt(segment);
    }


    private float AdjustValue(float t, int bezSegment)
    {
        return (t -
            ((float)bezSegment) / (float)BezSegment)
            / (1f / (float)BezSegment);
    }


    private Vector3 getBezierPoint(int bezierSegment, float t)
    {
        // Get control Points for this segment
        Vector3 A = Points[bezierSegment].GetComponent<BezierPoint>().getAnchor();
        Vector3 B = Points[bezierSegment].GetComponent<BezierPoint>().getControl2();

        int nextSegment = bezierSegment + 1;

        Vector3 C = Points[nextSegment].GetComponent<BezierPoint>().getControl1();
        Vector3 D = Points[nextSegment].GetComponent<BezierPoint>().getAnchor();

        // Interpolation, 1st stage
        Vector3 x = (1 - t) * A + t * B;
        Vector3 Y = (1 - t) * B + t * C;
        Vector3 Z = (1 - t) * C + t * D;

        // Interpolation, 2nd stage
        Vector3 P = (1 - t) * x + t * Y;
        Vector3 Q = (1 - t) * Y + t * Z;

        return (1 - t) * P + t * Q;
    }

    private Vector3 getBezierPoint(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float t)
    {
        // Interpolation, 1st stage
        Vector3 x = (1 - t) * A + t * B;
        Vector3 Y = (1 - t) * B + t * C;
        Vector3 Z = (1 - t) * C + t * D;

        // Interpolation, 2nd stage
        Vector3 P = (1 - t) * x + t * Y;
        Vector3 Q = (1 - t) * Y + t * Z;

        return (1 - t) * P + t * Q;

    }


    Vector3 getForwardVector(int bezierSegment, float t)
    {
        Vector3 A = Points[bezierSegment].GetComponent<BezierPoint>().getAnchor();
        Vector3 B = Points[bezierSegment].GetComponent<BezierPoint>().getControl2();

        // Wrap around for closed loops
        int nextSegment = (bezierSegment + 1) % Points.Length;

        Vector3 C = Points[nextSegment].GetComponent<BezierPoint>().getControl1();
        Vector3 D = Points[nextSegment].GetComponent<BezierPoint>().getAnchor();

        // Interpolation, 1st stage
        Vector3 x = (1 - t) * A + t * B;
        Vector3 Y = (1 - t) * B + t * C;
        Vector3 Z = (1 - t) * C + t * D;

        // Interpolation, 2nd stage
        Vector3 P = (1 - t) * x + t * Y;
        Vector3 Q = (1 - t) * Y + t * Z;

        return (Q - P).normalized;
    }

    Vector3 getForwardVector(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float t)
    {
        // Interpolation, 1st stage
        Vector3 x = (1 - t) * A + t * B;
        Vector3 Y = (1 - t) * B + t * C;
        Vector3 Z = (1 - t) * C + t * D;

        // Interpolation, 2nd stage
        Vector3 P = (1 - t) * x + t * Y;
        Vector3 Q = (1 - t) * Y + t * Z;

        return (Q - P).normalized;
    }

    void generateMesh()
    {
        CrossSection = new Mesh2D(new List<Vector3>()
        {
            new Vector3(-7, -1 , 0),
            new Vector3(7, -1 , 0),
            new Vector3(7, 1 , 0),
            new Vector3(5, 1 , 0),
            new Vector3(3, 0 , 0),
            new Vector3(-3, 0 , 0),
            new Vector3(-5, 1 , 0),
            new Vector3(-7, 1 , 0),
        });

        Vertices = new List<Vector3>();

      

        //CrossSection = new Mesh2D(vetices2D);

        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "Road Mesh";

        }
        else
        {
            mesh.Clear();
        }


        List<int> triangles = new List<int>();

        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; RoadSegments > i; i++) 
        {

            float currentSegmentT = i / RoadSegments;

            int segment = GetBezSegment(currentSegmentT);

            if (ClosedLoop && segment == Points.Length)
            {
                segment = Points.Length - 1;
            }

            float TValue = AdjustValue(currentSegmentT, segment);

            Vector3 bezPoint, bezForward;


            if (ClosedLoop && segment == Points.Length - 1)
            {
                // Get control Points for this segment
                Vector3 A = Points[Points.Length - 1].GetComponent<BezierPoint>().getAnchor();
                Vector3 B = Points[Points.Length - 1].GetComponent<BezierPoint>().getControl2();

                Vector3 C = Points[0].GetComponent<BezierPoint>().getControl1();
                Vector3 D = Points[0].GetComponent<BezierPoint>().getAnchor();

                bezPoint = getBezierPoint(A, B, C, D, TValue);
                bezForward = getForwardVector(A, B, C, D, TValue);
            }
            else
            {
                bezPoint = getBezierPoint(segment, TValue);
                bezForward = getForwardVector(segment, TValue);
            }

            Vector3 bezRight = Vector3.Cross(Vector3.up, bezForward);

            Vector3 bezUp = Vector3.Cross(bezForward, bezRight);

            for (int j = 0; CrossSection.vertices.Length > j; j++)
            {
                Vector3 point = CrossSection.vertices[j].x
                                * bezRight
                                + CrossSection.vertices[j].y
                                * bezUp;


                point *= RoadScaler;

                point += bezPoint;

                Vertices.Add(point);

                float u = j / (CrossSection.vertices.Length - 1);
                float v = i / RoadSegments - 1;
                uvs.Add(new Vector2(v, u));
            }

            int baseIndex = i * CrossSection.vertices.Length;
            int lowerLeft, lowerRight, upperLeft, upperRight;

            for (int j = 1; CrossSection.vertices.Length > j; j++)
            {
                lowerLeft = j + baseIndex;
                lowerRight = lowerLeft + 1;

                upperLeft = lowerLeft + CrossSection.vertices.Length;
                upperRight = upperLeft + 1;

                triangles.Add(upperRight);
                triangles.Add(upperLeft);
                triangles.Add(lowerLeft);

                triangles.Add(lowerRight);
                triangles.Add(upperRight);
                triangles.Add(lowerLeft);

            }

            if (ClosedLoop)
            {
                int baseLastIndex = (RoadSegments - 1) * CrossSection.vertices.Length;

                for (int j = 0; CrossSection.vertices.Length - 1 > j; j++)
                {
                    lowerLeft = baseLastIndex + j;
                    lowerRight = baseLastIndex + i + 1;
                    upperLeft = j;
                    upperRight = i + 1;


                    triangles.Add(upperRight);
                    triangles.Add(upperLeft);
                    triangles.Add(lowerLeft);

                    triangles.Add(lowerRight);
                    triangles.Add(upperRight);
                    triangles.Add(lowerLeft);

                }
            }


        }


        mesh.SetVertices(Vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();

    }

    private void OnDrawGizmos()
    {

        CrossSection = new Mesh2D(new List<Vector3>()
        {
            new Vector3(-7, -1 , 0),
            new Vector3(7, -1 , 0),
            new Vector3(7, 1 , 0),
            new Vector3(5, 1 , 0),
            new Vector3(3, 0 , 0),
            new Vector3(-3, 0 , 0),
            new Vector3(-5, 1 , 0),
            new Vector3(-7, 1 , 0),
        });

        Vector3 prevBezPoint = Points[0].GetComponent<BezierPoint>().getAnchor();
        Vector3 prevBezForward = Points[0].transform.right;
        Vector3 prevBezRight = Vector3.Cross(Vector3.up, prevBezForward);
        Vector3 prevBezUp = Vector3.Cross(prevBezRight, prevBezForward);



        for (int i = 0; i < RoadSegments; i++)
        {

            float currentSegmentT = i / (float)RoadSegments;
            float nextSegementT = (i + 1) / (float)RoadSegments;

            int segment = GetBezSegment(currentSegmentT);
            int nextSegment = GetBezSegment(nextSegementT);

            float tValue = AdjustValue(currentSegmentT, segment);
            float nextTValue = AdjustValue(nextSegementT, nextSegment);

            Vector3 bezPoint, bezForward;
            ;
            Vector3 A, B, C, D;

            if (ClosedLoop && segment == BezSegment - 1)
            {
                A = Points[Points.Length - 1].GetComponent<BezierPoint>().getAnchor();
                B = Points[Points.Length - 1].GetComponent<BezierPoint>().getControl2();
                C = Points[0].GetComponent<BezierPoint>().getControl1();
                D = Points[0].GetComponent<BezierPoint>().getAnchor();

                bezPoint = getBezierPoint(A, B, C, D, tValue);
                bezForward = getForwardVector(A, B, C, D, tValue);

            }
            else
            {
                bezPoint = getBezierPoint(segment, tValue);
                bezForward = getForwardVector(segment, tValue);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(bezPoint, 0.1f);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(prevBezPoint, bezPoint);

            Vector3 bezRight, bezUp;

            bezRight = Vector3.Cross(Vector3.up, bezForward);

            bezUp = Vector3.Cross(bezForward, bezRight);

            for (int j = 0; j < CrossSection.vertices.Length; j++)
            {
                Vector3 point = CrossSection.vertices[j].x
                                * bezRight
                                + CrossSection.vertices[j].y
                                * bezUp;

                point *= RoadScaler;

                point += bezPoint;

                if (DrawConnectors)
                {
                    Vector3 prevPoint = CrossSection.vertices[j].x
                                * prevBezRight
                                + CrossSection.vertices[j].y
                                * prevBezUp;

                    prevPoint *= RoadScaler;

                    prevPoint += prevBezPoint;

                    Gizmos.color = Color.black;


                    Gizmos.DrawLine(point, prevPoint);
                }

                Vector3 nextCrossSecPoint = Vector3.zero;

                if (j < CrossSection.vertices.Length - 2)
                {
                    nextCrossSecPoint = CrossSection.vertices[j + 1].x
                                            * bezRight
                                            + CrossSection.vertices[j + 1].y
                                            * bezUp;
                }

                nextCrossSecPoint *= RoadScaler;

                nextCrossSecPoint += bezPoint;

                if (DrawCrossSections)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(point, nextCrossSecPoint);
                }

                if (DrawVertices)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(point, 0.1f);

                }


            }

            prevBezPoint = bezPoint;
            prevBezForward = bezForward;
            prevBezRight = bezRight;
            prevBezUp = bezUp;



        }
        Vector3 FinalPoint;
        Vector3 FinalBezForward;
        Vector3 FinalBezRight;
        Vector3 FinalbezUp;
        if (ClosedLoop)
        {
            FinalPoint = Points[0].GetComponent<BezierPoint>().getAnchor();
            FinalBezForward = Points[0].transform.right;
            FinalBezRight = Vector3.Cross(Vector3.up, FinalBezForward);
            FinalbezUp = Vector3.Cross(FinalBezRight, prevBezForward);
        }

        else
        {
            FinalPoint = Points[BezSegment].GetComponent<BezierPoint>().getAnchor();
            FinalBezForward = Points[BezSegment].transform.right;
            FinalBezRight = Vector3.Cross(Vector3.up, FinalBezForward);
            FinalbezUp = Vector3.Cross(FinalBezRight, prevBezForward);
        }


        Gizmos.color = Color.green;
        Gizmos.DrawLine(prevBezPoint, FinalPoint);


        Gizmos.color = Color.green;
        Gizmos.DrawLine(prevBezPoint, FinalPoint);

        for (int j = 0; j < CrossSection.vertices.Length; j++)
        {
            Vector3 point = CrossSection.vertices[j].x
                            * FinalBezRight
                            + CrossSection.vertices[j].y
                            * FinalBezRight;
            point *= RoadScaler;

            point += FinalPoint;

            if (DrawConnectors)
            {
                Vector3 prevPoint = CrossSection.vertices[j].x
                            * prevBezRight
                            + CrossSection.vertices[j].y
                            * prevBezUp;

                prevPoint *= RoadScaler;

                prevPoint += prevBezPoint;

                Gizmos.color = Color.black;


                Gizmos.DrawLine(point, prevPoint);
            }

            Vector3 nextCrossSecPoint = Vector3.zero;

            if (j < CrossSection.vertices.Length - 2)
            {
                nextCrossSecPoint = CrossSection.vertices[j + 1].x
                                        * FinalBezRight
                                        + CrossSection.vertices[j + 1].y
                                        * FinalbezUp;
            }

            nextCrossSecPoint *= RoadScaler;

            nextCrossSecPoint += FinalPoint;

        }


    }

    private void Start()
    {
        generateMesh();
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}