using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 worldPosition;
    private CellInfo m_cellInfo;
    private List<Node> links = new List<Node>();
    private Node m_north  = null;
    private Node m_south = null;
    private Node m_east = null;
    private Node m_west = null;

    public Node(Vector3 _worldPosition)
    {
        worldPosition = _worldPosition;
    }

    public CellInfo cellInfo
    {
        get { return m_cellInfo; }
        set { m_cellInfo = value; }
    }

    public void Link(Node node, bool bidi = false)
    {
        links.Add(node);
        if(bidi)
        {
            node.Link(this);
        }
    }

    public bool IsLinked(Node node)
    {
        return links.Contains(node);
    }

    public List<Node> GetLinks()
    {
        return links;
    }

    public Node[] GetNeighbours()
    {
        List<Node> neighbours = new List<Node>();
        if(m_north != null)
            neighbours.Add(m_north);
        if(m_south != null)
            neighbours.Add(m_south);
        if(m_east != null)
            neighbours.Add(m_east);
        if(m_west != null)
            neighbours.Add(m_west);
        return neighbours.ToArray();
    }

    public Node north
    {
        get { return m_north; }
        set { m_north = value; }
    }

    public Node south
    {
        get { return m_south; }
        set { m_south = value; }
    }

    public Node east
    {
        get { return m_east; }
        set { m_east = value; }
    }

    public Node west
    {
        get { return m_west; }
        set { m_west = value; }
    }
}
