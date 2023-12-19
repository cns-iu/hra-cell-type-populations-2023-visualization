using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEditor;
using UnityEngine;

public class CsvToSo : MonoBehaviour
{
    private static string _rootCellPositions = "Assets/Resources/RawCellPositions";
    //private static string folder = "12_2023"; //DateTime.Today.ToString();
    private static string folder = "12_2023";
    private static int _readIterator =25; //**decrease** to read in **more** rows from the cell position CSV file

    //uncomment for usage; comment out to avoid accidental trigger
    [MenuItem("Utilities/IngestCellPositions")]

    public static void GetCells()
    {
        ////set up new folder for ingested cells
        //AssetDatabase.CreateFolder("Assets/Resources/IngestedCellPositions", folder);

        foreach (string file in Directory.GetFiles(_rootCellPositions, "*.csv"))
        {

            string[] allLines = File.ReadAllLines(file);

            //add note on how many cells are are actually showing
            for (int i = 0; i < allLines.Length; i += _readIterator)
            {
                try
                {
                    if (allLines[i].Split(',')[0] == "as_label") continue;

                    SO_CellTypeCount count = ScriptableObject.CreateInstance<SO_CellTypeCount>();
                    count.asLabel = allLines[i].Split(",")[0];
                    count.uberonId = allLines[i].Split(",")[1];
                    count.cellLabel = allLines[i].Split(",")[2];
                    count.cellId = allLines[i].Split(",")[3];
                    count.position = new Vector3(
                        float.Parse(allLines[i].Split(',')[4]),
                        float.Parse(allLines[i].Split(',')[5]),
                        float.Parse(allLines[i].Split(',')[6])
                        );

                    AssetDatabase.CreateAsset(count, $"Assets/Resources/IngestedCellPositions/{folder}/{count.asLabel}_{i}.asset");
                    AssetDatabase.SaveAssets();
                }
                catch (Exception ex)
                {
                    Debug.Log($"Exception at iterator {i}: {ex}");
                }

            }
        }
        AssetDatabase.SaveAssets();
    }


}

