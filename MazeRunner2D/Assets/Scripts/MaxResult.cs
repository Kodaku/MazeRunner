using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MaxResult
{
    public Cell Cell;
    public int Distance;

    public MaxResult(Cell _cell, int _distance)
    {
        this.Cell = _cell;
        this.Distance = _distance;
    }
}