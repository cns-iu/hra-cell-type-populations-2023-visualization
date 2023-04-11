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
            int counter = 0;

            foreach (var line in allLines)
            {
                if (line.Split(',')[0] == "organ") continue;
                if (line.Split(',')[1] != "VH_F_kidney_capsule_R") continue;

                SO_CellTypeCount count = ScriptableObject.CreateInstance<SO_CellTypeCount>();
                count.organ = line.Split(',')[0];
                count.anatomicalStructure = line.Split(",")[1];
                count.cellType = line.Split(",")[2];
                count.position = new Vector3(
                    float.Parse(line.Split(',')[3]),
                    float.Parse(line.Split(',')[4]),
                    float.Parse(line.Split(',')[4])
                    );

                AssetDatabase.CreateAsset(count, $"Assets/ScriptableObjects/{count.anatomicalStructure}_{counter}.asset");
                counter++;
            }
        }
        AssetDatabase.SaveAssets();
    }
}
