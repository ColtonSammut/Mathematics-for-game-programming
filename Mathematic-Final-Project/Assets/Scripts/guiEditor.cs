using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(TerrainGenerator))]
public class guiEditor : Editor
{
   public override void OnInspectorGUI()
   {
      DrawDefaultInspector();

      TerrainGenerator tg = (TerrainGenerator)target;
      if (GUILayout.Button("Generate Terrain"))
      {
         tg.GenerateTerrain();
      }
   }
}
