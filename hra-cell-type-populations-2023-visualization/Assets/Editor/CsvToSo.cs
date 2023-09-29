using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEditor;
using UnityEngine;

public class CsvToSo : MonoBehaviour
{
    
    private static string _rootCellPositions = "Assets/Resources";
    private static string _rootColorMapping = "Assets/Resources";

    //uncomment for usage; commented out to avoid accidental trigger
    [MenuItem("Utilities/IngestCellPositions")]

    public static void GetCells()
    {

        foreach (string file in Directory.GetFiles(_rootCellPositions, "*.csv"))
        {
            string[] allLines = File.ReadAllLines(file);

            //add note on how many cells are are actually showing
            for (int i = 0; i < allLines.Length; i += 1000)
            {
                try
                {
                    if (allLines[i].Split(',')[0] == "organ") continue;
                    //if (line.Split(',')[1] == "VH_F_kidney_capsule_R") continue;

                    SO_CellTypeCount count = ScriptableObject.CreateInstance<SO_CellTypeCount>();
                    count.organ = allLines[i].Split(',')[0];
                    count.anatomicalStructure = allLines[i].Split(",")[1];
                    count.cellType = allLines[i].Split(",")[2];
                    count.position = new Vector3(
                        float.Parse(allLines[i].Split(',')[3]),
                        float.Parse(allLines[i].Split(',')[4]),
                        float.Parse(allLines[i].Split(',')[5])
                        );

                    AssetDatabase.CreateAsset(count, $"Assets/Resources/ScriptableObjects/09_2023/{count.anatomicalStructure}_{i}.asset");
                }
                catch (Exception ex)
                {

                    Debug.Log($"Exception at iterator {i}");
                }
               
            }
        }
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Utilities/IngestColorMapping")]
    public static void GetColorMapping() {

        SO_ColorMapping mapping = ScriptableObject.CreateInstance<SO_ColorMapping>();

        foreach (string file in Directory.GetFiles(_rootColorMapping, "*.csv"))
        {
            string[] allLines = File.ReadAllLines(file);

            //note that we are only showing 10% of all cells in CSV (which is 1% of cells in CT summary, so 0.1% of all cells)
            for (int i = 0; i < allLines.Length; i ++)
            {
                if (allLines[i].Split(',')[0] == "color_array") continue;
                //if (line.Split(',')[1] == "VH_F_kidney_capsule_R") continue;


                mapping.cellTypeToColorMapping.Add(new CellTypeColorPair(allLines[i].Split(',')[1], allLines[i].Split(',')[0]));
               //mapping.

                
            }
        }
        AssetDatabase.CreateAsset(mapping, $"Assets/Resources/ColorMapping.asset");
        AssetDatabase.SaveAssets();
    }
}
