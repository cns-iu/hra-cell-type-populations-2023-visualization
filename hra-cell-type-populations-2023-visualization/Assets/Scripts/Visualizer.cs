using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private List<SO_CellTypeCount> _allCellTypeCounts = new List<SO_CellTypeCount>();
    [SerializeField] private List<string> _uniqueCellTypes = new List<string>();
    private Dictionary<string, int> _dictCountsPerCT = new Dictionary<string, int>();
    [SerializeField] private List<string> _topCellTypes = new List<string>();
    private string _root = "Assets\\Resources\\ScriptableObjects";
    [SerializeField] int _readIterator = 10;

    [Header("Visual Encoding")]
    [SerializeField] private SO_ColorMapping _mapping;
    [SerializeField] private int _maxNumberCellTypes = 10;
    [SerializeField] private bool _onlyShowTopCells = false;

    [Header("3D Scene")]
    [SerializeField] private GameObject _pre_cell;
    [SerializeField] private List<Transform> _parents = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        var sos = Resources.LoadAll<SO_CellTypeCount>("ScriptableObjects/");
        HashSet<string> unique = new HashSet<string>();

        for (int i = 0; i < sos.Length; i += _readIterator)
        {
            _allCellTypeCounts.Add(sos[i]);

            if (!_dictCountsPerCT.ContainsKey(sos[i].cellType))
                _dictCountsPerCT.Add(sos[i].cellType, 1);
            else
                _dictCountsPerCT[sos[i].cellType]++;
        }

        //get 10 CTs with most occurrences
        var maxOccurences = _dictCountsPerCT.OrderByDescending(kvp => kvp.Value).ToList().Take(_maxNumberCellTypes);


        foreach (var item in maxOccurences)
        {
            _topCellTypes.Add(item.Key);
        }

        for (int i = 0; i < sos.Length; i += _readIterator)
        {
            //uncomment if wanting to display top cell types only
            if (!_topCellTypes.Contains(sos[i].cellType) && _onlyShowTopCells) continue;

            GameObject cellObj = MakeCell(sos[i]);
            cellObj.AddComponent<CellData>().type = sos[i].cellType;
            _cells.Add(cellObj);
            unique.Add(sos[i].cellType);
        }

        _uniqueCellTypes = unique.ToList();



        //foreach (var cell in _cells)
        foreach (var cell in _cells)
        {
            CellData data = cell.GetComponent<CellData>();
            cell.GetComponent<SpriteRenderer>().material.color = GetColor(cell.GetComponent<CellData>().type);
        }

        //foreach (var item in _dictCountsPerCT)
        //{
        //    Debug.Log($"{item.Key} occurs {item.Value} times.");
        //}

    }

    private Color GetColor(string cellType)
    {
        return _mapping.cellTypeToColorMapping
            .Where(i => i.cellType.Trim().Equals(cellType.Trim()))
            .First().cellColor;
    }

    private GameObject MakeCell(SO_CellTypeCount count)
    {
        GameObject cell = Instantiate(_pre_cell, count.position, Quaternion.identity);
        cell.transform.position = AdjustPosition(cell);

        foreach (var p in _parents)
        {
            if (count.anatomicalStructure.Equals(p.name))
            {
                cell.transform.parent = p;
            }
        }

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

    [Serializable]
    private class ColorMapping
    {
        public string cellType;
        public Color color;

    }
}
