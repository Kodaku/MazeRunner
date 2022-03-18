using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

[System.Serializable]
public class Cell : IEquatable<Cell>
{
    private Dictionary<CellLocation, Cell> links;
    private Dictionary<CellLocation, Cell> unlinks;
    private int m_row;
    private int m_column;

    public Cell(int row, int column)
    {
        m_row = row;
        m_column = column;
        links = new Dictionary<CellLocation, Cell>();
        unlinks = new Dictionary<CellLocation, Cell>();
    }

    public int row
    {
        get { return m_row; }
    }

    public int column
    {
        get { return m_column; }
    }

    public virtual Distances Distances
    {
        get
        {
            Distances distances = new Distances(this);
            List<Cell> frontier = new List<Cell>();
            frontier.Add(this);
            
            while(frontier.Any())
            {
                List<Cell> newFrontier = new List<Cell>();

                foreach(Cell cell in frontier)
                {
                    Dictionary<CellLocation, Cell> links = cell.GetLinks();
                    // Debug.Log("Current: " + cell);
                    foreach(CellLocation cellLocation in links.Keys)
                    {
                        Cell linkedCell = links[cellLocation];
                        if(!distances.ContainsKey(linkedCell))
                        {
                            // Debug.Log("Linked: " + linkedCell);
                            distances[linkedCell] = distances[cell] + 1;
                            newFrontier.Add(linkedCell);
                        }
                    }
                }
                frontier = newFrontier;
            }
            return distances;
        }
    }

    public void Link(CellLocation linkLocation, bool bidi = true)
    {
        if(unlinks.ContainsKey(linkLocation))
        {
            Cell cellToLink = unlinks[linkLocation];
            unlinks.Remove(linkLocation);
            links.Add(linkLocation, cellToLink);
            // Debug.Log("Linking (" + m_column + "," + m_row + ") with (" + cellToLink.column + "," + cellToLink.row + ") at " + linkLocation);
            if(bidi)
            {
                if(linkLocation == CellLocation.EAST)
                {
                    cellToLink.Link(CellLocation.WEST, bidi:false);
                }
                else if(linkLocation == CellLocation.NORTH)
                {
                    cellToLink.Link(CellLocation.SOUTH, bidi:false);
                }
                else if(linkLocation == CellLocation.WEST)
                {
                    cellToLink.Link(CellLocation.EAST, bidi:false);
                }
                else if(linkLocation == CellLocation.SOUTH)
                {
                    cellToLink.Link(CellLocation.NORTH, bidi:false);
                }
            }
        }
    }

    public void Unlink(CellLocation cellLocation, Cell cell)
    {
        if(!unlinks.ContainsKey(cellLocation))
        {
            // Debug.Log("Unlinking (" + m_column + "," + m_row + ") with (" + cell.column + "," + cell.row + ") at " + cellLocation);
            unlinks.Add(cellLocation, cell);
        }
    }

    public List<Cell> GetUnlinkedCells()
    {
        List<Cell> unlikedCells = new List<Cell>();
        foreach(CellLocation cellLocation in unlinks.Keys)
        {
            unlikedCells.Add(unlinks[cellLocation]);
        }
        return unlikedCells;
    }

    public Dictionary<CellLocation, Cell> GetUnlinks()
    {
        return unlinks;
    }

    public Dictionary<CellLocation, Cell> GetLinks()
    {
        return links;
    }

    public override string ToString()
    {
        return $"({this.column},{this.row})";
    }

    public string ToStringDistance()
    {
        return $"({this.row},{this.column})";
    }

    public bool Equals(Cell other)
    {
        if (other == null) return false;
        return (m_row == other.row && m_column == other.column);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Cell);
    }

    public override int GetHashCode()
    {
        return $"{this.column}_{this.row}".GetHashCode();
    }
}
