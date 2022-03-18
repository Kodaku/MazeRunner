using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecursiveBacktracker
{
    public void On(ref MyGrid grid)
    {
        Cell start = grid.GetRandomCell;
        Stack<Cell> stack = new Stack<Cell>();
        stack.Push(start);

        while(stack.Any())
        {
            Cell current = stack.Peek();
            Dictionary<CellLocation, Cell> neighbors = grid.GetCellNeighbors(current.row, current.column);
            Dictionary<CellLocation, Cell> validNeighbors = new Dictionary<CellLocation, Cell>();
            foreach(CellLocation cellLocation in neighbors.Keys)
            {
                Cell neighbor = neighbors[cellLocation];
                if(neighbor.GetLinks().Count == 0)
                {
                    validNeighbors.Add(cellLocation, neighbor);
                }
            }

            // neighbors = [neighbor for neighbor in current.neighbors() if len(neighbor.links) == 0]

            if(validNeighbors.Count == 0)
            {
                stack.Pop();
            }
            else
            {
                int index = Random.Range(0, validNeighbors.Keys.Count);
                Cell neighbor = validNeighbors[validNeighbors.Keys.ToArray()[index]];
                current.Link(validNeighbors.Keys.ToArray()[index]);
                stack.Push(neighbor);
                grid.SetCellAt(current.row, current.column, current);
            }
        }
    }
}
