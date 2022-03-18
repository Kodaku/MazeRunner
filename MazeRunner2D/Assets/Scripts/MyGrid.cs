using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MyGrid
{
    protected Cell[,] grid;

    private int m_rows;
    private int m_columns;
    [System.NonSerialized]
    private Distances m_distances;
    public Distances distances { get { return m_distances; } set { m_distances = value; } }

    public MyGrid(int rows, int columns)
    {
        m_rows = rows;
        m_columns = columns;
        PrepareGrid();
        ConfigureCells();
    }

    protected virtual void PrepareGrid()
    {
        grid = new Cell[m_rows, m_columns];
        for (var i = 0; i < m_rows; i++)
        {
            for (var j = 0; j < m_columns; j++)
            {
                grid[i, j] = new Cell(i, j);
            }
        }
    }

    public int rows
    {
        get { return m_rows; }
    }

    public int columns
    {
        get { return m_columns; }
    }

    private void ConfigureCells()
    {
        for(int i = 0; i < m_rows; i++)
        {
            for(int j = 0; j < m_columns; j++)
            {
                Cell cell = grid[i, j];
                if(i - 1 >= 0)
                {
                    cell.Unlink(CellLocation.SOUTH, grid[i - 1, j]);
                }
                if(j - 1 >= 0)
                {
                    cell.Unlink(CellLocation.WEST, grid[i, j - 1]);
                }
                if(i + 1 < m_rows)
                {
                    cell.Unlink(CellLocation.NORTH, grid[i + 1, j]);
                }
                if(j + 1 < m_columns)
                {
                    cell.Unlink(CellLocation.EAST, grid[i, j + 1]);
                }
                grid[i, j] = cell;
            }
        }
    }

    public Cell CellAt(int row, int column)
    {
        if(row >= 0 && row < rows && column >= 0 && column < columns)
        {
            return grid[row, column];
        }
        return null;
    }

    public Cell WorldPointToCell(Vector3 position)
    {
        int row = Mathf.RoundToInt(position.y);
        int column = Mathf.RoundToInt(position.x);
        return grid[row, column];
    }

    public void SetCellAt(int row, int column, Cell cell)
    {
        if(row >= 0 && row < rows && column >= 0 && column < columns)
        {
            grid[row, column] = cell;
        }
    }

    public Dictionary<CellLocation, Cell> GetCellNeighbors(int row, int column)
    {
        Cell current = grid[row, column];
        Dictionary<CellLocation, Cell> neighbors = new Dictionary<CellLocation, Cell>();

        if(row - 1 >= 0)
        {
            neighbors.Add(CellLocation.SOUTH, grid[row - 1, column]);
        }
        if(column - 1 >= 0)
        {
            neighbors.Add(CellLocation.WEST, grid[row, column - 1]);
        }
        if(row + 1 < m_rows)
        {
            neighbors.Add(CellLocation.NORTH, grid[row + 1, column]);
        }
        if(column + 1 < m_columns)
        {
            neighbors.Add(CellLocation.EAST, grid[row, column + 1]);
        }

        return neighbors;
    }

    // protected IEnumerable<Cell> GetRow(int row)
    // {
    //     for (var i = 0; i < Rows; i++)
    //     {
    //         for (var j = 0; j < Rows; j++)
    //         {
    //             if (i == row)
    //                 yield return this[i, j];
    //         }
    //     }
    // }

    // public List<List<Cell>> GetAllRows()
    // {
    //     var results = new List<List<Cell>>();
    //     for (var i = 0; i < Rows; i++)
    //     {
    //         var innerList = new List<Cell>();
    //         for (var j = 0; j < Rows; j++)
    //         {
    //             innerList.Add(this[i, j]);
    //         }
    //         results.Add(innerList);
    //     }
    //     return results;
    // }

    // public IEnumerable<Cell> GetAllCells()
    // {
    //     for (int i = 0; i < Rows; i++)
    //     {
    //         for (var j = 0; j < Columns; j++)
    //         {
    //             yield return this[i, j];
    //         }
    //     }
    // }

    public virtual Cell GetRandomCell
    {
        get
        {
            var i = Random.Range(0, m_rows - 1);
            var j = Random.Range(0, m_columns - 1);
            return grid[i, j];
        }
    }

    #region To Display / Debug Grid
    public virtual string ContentsOf(Cell cell)
    {
        return "   ";
    }

    public virtual string ToString(bool displayGridCoordinates)
    {
        return string.Empty;
    }

    public override string ToString()
    {
        return ToString(false);
    }

    public string ToDebug()
    {
        var output = string.Empty;

        return output;
    }

    #endregion

    #region Helper Methods  

    /// <summary>IEnumerator for 2-D cell array.</summary>
    // public IEnumerator<Cell> GetEnumerator()
    // {
    //     for (var i = 0; i < Rows; i++)
    //     {
    //         for (var j = 0; j < Rows; j++)
    //         {
    //             yield return grid[i, j];
    //         }
    //     }
    // }

    // /// <summary>2-D Array Accessor method.</summary>
    // public Cell this[int row, int column]
    // {
    //     get
    //     {
    //         if (row < 0 || row > Rows - 1) return null;
    //         if (column < 0 || column > Columns - 1) return null;
    //         return grid[row, column];
    //     }
    //     set
    //     {
    //         grid[row, column] = value;
    //     }
    // }

    #endregion
}
