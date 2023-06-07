using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_ColorMapping : ScriptableObject
{
    public List<CellTypeColorPair> cellTypeToColorMapping = new List<CellTypeColorPair>();
}

[Serializable]
public class CellTypeColorPair
{
    public string cellType;
    public Color cellColor;


    public CellTypeColorPair(string cell, string color)
    {
        cellType = cell;
        ColorUtility.TryParseHtmlString(color, out cellColor);
    }
}
