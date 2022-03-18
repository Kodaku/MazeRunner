using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BinaryTree
{
    public void On(ref MyGrid grid)
    {
        for(int i = 0; i < grid.rows; i++)
        {
            for(int j = 0; j < grid.columns; j++)
            {
                Cell currentCell = grid.CellAt(i, j);
                Dictionary<CellLocation, Cell> validNeighbours = new Dictionary<CellLocation, Cell>();
                Dictionary<CellLocation, Cell> currentCellsLinked = currentCell.GetUnlinks();
                if(currentCellsLinked.ContainsKey(CellLocation.NORTH))
                {
                    validNeighbours.Add(CellLocation.NORTH, currentCellsLinked[CellLocation.NORTH]);
                }
                if(currentCellsLinked.ContainsKey(CellLocation.EAST))
                {
                    validNeighbours.Add(CellLocation.EAST, currentCellsLinked[CellLocation.EAST]);
                }
                if(validNeighbours.Count > 0)
                {
                    int index = Random.Range(0, validNeighbours.Keys.Count);
                    currentCell.Link(validNeighbours.Keys.ToArray<CellLocation>()[index]);
                }
                grid.SetCellAt(i, j, currentCell);
            }
        }
        // foreach (var cell in grid.GetAllCells())
        // {
        //     var neighbors = new List<Cell>();
        //     if (cell.North != null) neighbors.Add(cell.North);
        //     if (cell.East != null) neighbors.Add(cell.East);

        //     int index = Random.Range(0, neighbors.Count);
        //     if (neighbors.Count > 0)
        //     {
        //         Cell neighbor = neighbors[index];
        //         Debug.Log("Linking (" + neighbor.row + "," + neighbor.column + ") with (" + cell.row + "," + cell.column + ")");
        //         cell.Link(neighbor);
        //     }
        // }
    }
}
