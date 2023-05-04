using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using UnityEngine.UIElements;

public class Visualizer : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<GameObject> _cells = new List<GameObject>();
    [SerializeField] private List<SO_CellTypeCount> _counts = new List<SO_CellTypeCount>();
    [SerializeField] private GameObject _pre_cell;
    [SerializeField] SO_CellTypeCount _cc;
    private string _root = "Assets\\Resources\\ScriptableObjects";

    [Header("Visual encoding")]
    [SerializeField] private List<Color> _colors = new List<Color>();
    private Dictionary<string, Color> _dictColor = new Dictionary<string, Color>();
    [SerializeField] private List<string> _uniqueCellTypes = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        var sos = Resources.LoadAll<SO_CellTypeCount>("ScriptableObjects/");
        HashSet<string> unique = new HashSet<string>();

        foreach (var s in sos)
        {
            _counts.Add(s);
            GameObject cellObl = MakeCell(s);
            cellObl.AddComponent<CellData>().type = s.cellType;
            _cells.Add(cellObl);

            unique.Add(s.cellType);
        }
        _uniqueCellTypes = unique.ToList();
        _colors = CreateColorSet();

        foreach (var cell in _cells)
        {
            cell.GetComponent<SpriteRenderer>().material.color = GetColor(cell.GetComponent<CellData>().type);
        }

    }

    private Color GetColor(string cellType)
    {
        return _dictColor[cellType];
    }

    private List<Color> CreateColorSet()
    {


        int customSeed = 1234;
        UnityEngine.Random.InitState(customSeed);

        List<Color> result = new List<Color>();
        for (int i = 0; i < _uniqueCellTypes.Count; i++)
        {
            Color c = new Color(
                    UnityEngine.Random.Range(.3f, 1f),
                    UnityEngine.Random.Range(.3f, 1f),
                    UnityEngine.Random.Range(.3f, 1f)
                );

            result.Add(c);
            _dictColor.Add(_uniqueCellTypes[i], c);
        }
        return result;
    }

    private GameObject MakeCell(SO_CellTypeCount count)
    {
        GameObject cell = Instantiate(_pre_cell, count.position, Quaternion.identity);
        cell.transform.position = AdjustPosition(cell);
        return cell;
    }

    private Vector3 AdjustPosition(GameObject go)
    {
        Matrix4x4 reflected = Utils.ReflectX() * Matrix4x4.TRS(
            go.transform.position,
            go.transform.rotation,
            go.transform.localScale
            );

        return reflected.GetPosition();
    }
}
