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
    private string _root = "Assets\\Resources\\IngestedCellsPositions/12_2023";
    [SerializeField] int _readIterator = 1;

    [Header("Visual Encoding")]
    [SerializeField] private SO_ColorMapping _mapping;
    [SerializeField] private int _maxNumberCellTypes = 10; //note that most qualitative color palettes do not yield nore than 12 colors and that showing more cell types than that is not adviseable
    [SerializeField] private bool _onlyShowTopCells = false;

    [Header("3D Scene")]
    [SerializeField] private GameObject _pre_cell;
    [SerializeField] private List<Transform> _parents = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        var sos = Resources.LoadAll<SO_CellTypeCount>("IngestedCellPositions/12_2023");
        HashSet<string> unique = new HashSet<string>();

        for (int i = 0; i < sos.Length; i += _readIterator)
        {
            _allCellTypeCounts.Add(sos[i]);

            if (!_dictCountsPerCT.ContainsKey(sos[i].cellLabel))
                _dictCountsPerCT.Add(sos[i].cellLabel, 1);
            else
                _dictCountsPerCT[sos[i].cellLabel]++;
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
            if (!_topCellTypes.Contains(sos[i].cellLabel) && _onlyShowTopCells) continue;

            GameObject cellObj = MakeCell(sos[i]);
            CellData data = cellObj.AddComponent<CellData>();
            data.type = sos[i].cellLabel;
            data.anatomicalStructure = sos[i].asLabel;
            _cells.Add(cellObj);
            unique.Add(sos[i].cellLabel);
        }

        _uniqueCellTypes = unique.ToList();

        //clear color mapping
        foreach (var kvp in _mapping.cellTypeToColorMapping) kvp.cellType = "";

        //to parent to correct AS
        AnatomicalStructureData[] anatomicalStructures = FindObjectsOfType<AnatomicalStructureData>();

        //foreach (var cell in _cells)
        foreach (var cell in _cells)
        {
            CellData data = cell.GetComponent<CellData>();
            cell.GetComponent<SpriteRenderer>().material.color = GetColor(cell.GetComponent<CellData>().type);

            Transform parent = anatomicalStructures.Where(s => s.GetComponent<AnatomicalStructureData>().As3DId == cell.GetComponent<CellData>().anatomicalStructure).First().gameObject.transform;
            cell.transform.SetParent(parent);
        }




        //foreach (var item in _dictCountsPerCT)
        //{
        //    Debug.Log($"{item.Key} occurs {item.Value} times.");
        //}

    }

    private Color GetColor(string cellType)
    {
        //first, we check if we already assigned a color to this cell type
        foreach (var kvp in _mapping.cellTypeToColorMapping)
        {
            if (kvp.cellType == cellType) return kvp.cellColor;
        }

        CellTypeColorPair pair = _mapping.cellTypeToColorMapping.Where(i => i.cellType == "").First();
        pair.cellType = cellType;
        return pair.cellColor;
    }

    private GameObject MakeCell(SO_CellTypeCount count)
    {
        GameObject cell = Instantiate(_pre_cell, count.position, Quaternion.identity);
        cell.transform.position = AdjustPosition(cell);

        foreach (var p in _parents)
        {
            if (count.asLabel.Equals(p.name))
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
