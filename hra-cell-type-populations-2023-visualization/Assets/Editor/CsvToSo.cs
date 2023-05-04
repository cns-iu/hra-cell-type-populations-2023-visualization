using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CsvToSo : MonoBehaviour
{
    private static string root = "Assets/Resources/Cells";

    [MenuItem("Utilities/IngestCellPositions")]

    public static void GetCorrelationMatrices()
    {

        foreach (string file in Directory.GetFiles(root, "*.csv"))
        {
            string[] allLines = File.ReadAllLines(file);

            //note that we are only showing 10% of all cells in CSV (which is 1% of cells in CT summary, so 0.1% of all cells)
            for (int i = 0; i < allLines.Length; i += 10)
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

                AssetDatabase.CreateAsset(count, $"Assets/Resources/ScriptableObjects/{count.anatomicalStructure}_{i}.asset");
            }
        }
        AssetDatabase.SaveAssets();
    }
}
