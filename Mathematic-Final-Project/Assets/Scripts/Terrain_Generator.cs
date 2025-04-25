using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TerrainGenerator : MonoBehaviour
{


    public List<NoiseSettings> NoiseSettings = new List<NoiseSettings>();
    
    [SerializeField] bool useTeshhold = false;
    
    [SerializeField][Range(-.5f,.5f)]float treshold = 0f;
    
    [SerializeField] private Material debugTerrainMaterial;
    
    [Range(0f, 10000f)][SerializeField]private float scale = 0f; // Scale of the Terrain Mesh.
    
    [Range(2, 255)][SerializeField]private float segments = 0f; // Number of cells the Terrain Mesh shall have. 
    
    [Range(0f, 5)][SerializeField]private float Height  = 1f; // The Height Range that shall be allowed.
    
    [Range(0, 100f)][SerializeField]private float Roughness = 10f;
    
    private Mesh _mesh; // Reference to the Mesh Component in our GameObject.

    private void OnDrawGizmos()
    {
        float delta = scale / (segments - 1); // Calculating the size of each individual Cell/Square in the mesh.

        for (int i = 0; i < segments; i++)
        {
            for (int j = 0; j < segments; j++) // Nested loop to create a plane with cells for X and Y axis
            {
                float
                    x = i * delta; // X is the current increment multiplied by the size shifting the position of the vertices every iteration.
                float y = j * delta; // The Same is done with Y to create plane that has coordinates 

                //Gizmos.color = Color.green;
                //Gizmos.DrawSphere(new Vector3(x, 0, y), 1f); // Drawing the points for reference
            }
        }
}
    public void GenerateTerrain()
    {
        if (!_mesh) // In case the mesh reference does not exist.
        {
            _mesh = new Mesh(); // Create a new Mesh
            _mesh.name = "Terrain"; // Set the name of the mesh to Terrain
            _mesh.MarkDynamic(); // Optimize it for any Updates
        }
        else
        {
            _mesh.Clear(); // Clear the mesh data to reset any the Mesh between runs
        }
        
        List<Vector3> vertices = new List<Vector3>(); //List of all the vertices in our Mesh
        List<int> triangles = new List<int>(); // List of the triangle sub-meshes that shall compose our mesh
        List<Vector2> uvs = new List<Vector2>();
        
        
        float delta = scale / (segments - 1); // Calculating the size of each individual Cell/Square in the mesh.

        for (int i = 0; i < segments; i++)
        {
            for (int j = 0; j < segments; j++) // Nested loop to create a plane with cells for X and Y axis
            {
                float
                    x = i * delta; // X is the current increment multiplied by the size shifting the position of the vertices every iteration.
                float y = j * delta; // The Same is done with Y to create plane that has coordinates 

                float noiseY = 0f; //Using the perlin noise to generate a value for the height.


                for (int k = 0; k < NoiseSettings.Count; k++)
                {
                    noiseY += NoiseSettings[k].Amplitude * (Mathf.PerlinNoise(NoiseSettings[k].Frequency * x / scale * Roughness , NoiseSettings[k].Frequency * y / scale * Roughness) - .5f);
                }
                //noiseY = noiseY / scale * 10f;
                
                //if (noiseY / scale < treshold && useTeshhold) noiseY = 0;
                
                noiseY = Mathf.Lerp(0.1f, 1000f, noiseY) * Height; // Scaling and smoothing the height Value.

              
                vertices.Add(new Vector3(x, noiseY, y)); // Add the point to the vertices list

                uvs.Add(new Vector2(x / scale, y / scale));

            }
        }
        
        for (int i = 0; i < segments - 1; i++) // Generating the triangles using the vertices.
        {
            for (int j = 0; j < segments - 1; j++)
            {
                int ul = j * (int)segments + i; // Upper Left
                int ur = ul + 1; // Upper Right
                int ll = ul + (int)segments; // Lower Left
                int lr = ll + 1; // Lower Right

                // Triangles
                triangles.Add(ll);
                triangles.Add(ul);
                triangles.Add(ur);

                triangles.Add(ll);
                triangles.Add(ur);
                triangles.Add(lr);
            }
            
        }
        
        _mesh.SetVertices(vertices); // Applying the vertices to the mesh
        _mesh.SetTriangles(triangles, 0); // Applying the triangles to the mesh
        _mesh.RecalculateNormals();
        _mesh.SetUVs(0, uvs);

        
        if(gameObject.TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
            GetComponent<MeshFilter>().sharedMesh = _mesh;
        else
        {
            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();
            GetComponent<MeshFilter>().sharedMesh = _mesh;
            if (debugTerrainMaterial)
            {
                GetComponent<MeshRenderer>().sharedMaterial = debugTerrainMaterial;
            }
        }
        


    }
    
}
