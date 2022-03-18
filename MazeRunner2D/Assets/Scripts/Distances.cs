using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class Distances
{
    private Cell m_root;
    private Dictionary<Cell, int> m_cells;

    public Distances(Cell root)
    {
        m_root = root;
        m_cells = new Dictionary<Cell, int>();
        m_cells.Add(m_root, 0);
    }

    public IEnumerable<Cell> Cells()
    {
        return m_cells.Keys;
    }

    public bool ContainsKey(Cell cell)
    {
        return m_cells.ContainsKey(cell);
    }

    public int this[Cell cell]
    {
        get
        {
            return m_cells.ContainsKey(cell) ? (int)m_cells[cell] : -1;
        }
        set
        {
            m_cells[cell] = (int)value;
        }
    }

    public Distances PathToGoal(Cell goal)
    {
        Cell currentCell = goal;

        Distances breadcrumbs = new Distances(m_root);
        breadcrumbs[currentCell] = m_cells[currentCell];
        do
        {
            Dictionary<CellLocation, Cell> links = currentCell.GetLinks();
            foreach(CellLocation cellLocation in links.Keys)
            {
                Cell neighbor = links[cellLocation];
                if(m_cells[neighbor] < m_cells[currentCell])
                {
                    breadcrumbs[neighbor] = m_cells[neighbor];
                    currentCell = neighbor;
                    break;
                }
            }
        } while(currentCell != m_root);

        return breadcrumbs;
    }

    public MaxResult Max
    {
        get
        {
            int maxDistance = 0;
            Cell maxCell = m_root;

            foreach(var key in m_cells.Keys)
            {
                int distance = m_cells[key];
                if(distance > maxDistance)
                {
                    maxCell = key;
                    maxDistance = distance;
                }
            }
            
            return new MaxResult(maxCell, maxDistance);
        }
    }
}
