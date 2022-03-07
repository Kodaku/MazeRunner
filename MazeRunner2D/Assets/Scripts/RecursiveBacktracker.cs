using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursiveBacktracker
{
    private MyGrid m_grid;

    public RecursiveBacktracker(MyGrid grid)
    {
        m_grid = grid;
    }
    
    public MyGrid On()
    {
        Node start = m_grid.GetRandomCell();
        Stack<Node> stack = new Stack<Node>();
        stack.Push(start);

        while(stack.Count > 0)
        {
            Node current = stack.Pop();
            List<Node> validNeighbours = new List<Node>();
            Node[] currentNeighbours = current.GetNeighbours();
            foreach(Node neighbour in currentNeighbours)
            {
                if(neighbour.GetLinks().Count == 0)
                {
                    validNeighbours.Add(neighbour);
                }
            }

            if(validNeighbours.Count > 0)
            {
                int randomIndex = Random.Range(0, validNeighbours.Count);
                Node neighbour = validNeighbours[randomIndex];
                current.Link(neighbour);
                stack.Push(neighbour);
                // stack.Push(current);
                m_grid.SetNodeFromWorldPoint(current.worldPosition, current);
            }
        }
        return m_grid;
    }
}
