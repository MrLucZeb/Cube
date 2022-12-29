using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathFinderCopy : MonoBehaviour
{
    class Node
    {
        public Vector2Int cord;
        public int adjacentNode = -1;
        public bool locked = false;
        public float gCost = 0;
        public float hCost = 0;
        public float fCost = 0;
    }

    int[,] grid = new int[25, 25];
    List<Node> _nodes = new List<Node>();
    Vector2Int gridSize = new Vector2Int(25, 25);

    int[,] _map2 = new int[25, 25];     // Temporary

    // Start is called before the first frame update
    void Start()
    {
        grid[5, 10] = 1;
        grid[6, 10] = 1;
        grid[7, 10] = 1;
        grid[8, 10] = 1;
        grid[9, 10] = 1;
        grid[10, 10] = 1;
        grid[11, 10] = 1;
        grid[12, 10] = 1;
        grid[13, 10] = 1;
        grid[13, 9] = 1;
        grid[13, 8] = 1;
        grid[13, 7] = 1;
        grid[13, 12] = 1;
        grid[13, 13] = 1;

        FindPath(new Vector2Int(2, 2), new Vector2Int(15, 15));
    }

    void Update()
    {
 
        if (Input.GetKey(KeyCode.E))
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (grid[x, y] == 2 || grid[x, y] == 3 || grid[x, y] == 4)
                        grid[x, y] = 0;
                }

            }

            bool valid = false;
            Vector2Int start = new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
            Vector2Int target = new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
            while (!valid)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    for (int y = 0; y < gridSize.y; y++)
                    {
                        if (grid[(int)start.x, (int)start.y] == 1)
                        {
                            start = new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
                        }
                        else if (grid[(int)target.x, (int)target.y] == 1)
                        {
                            target = new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
                        }
                        else
                            valid = true;
                    }

                }
            }
            Debug.Log("start: =======================================================================" + start);
            Debug.Log("target:" + target);
            FindPath(start, target);


            _map2[(int)start.x, (int)start.y] = 5;
            _map2[(int)target.x, (int)target.y] = 3;
        }
    }

    public void SetGridSize(Vector2Int size) // Note: This also clears the grid
    {
        gridSize = size;

        ClearGrid();
    }

    public void ClearGrid()
    {
        grid = new int[gridSize.x, gridSize.y];
    }

    public void SetGridValueAtPosition(Vector2Int position, int value)
    {
        grid[position.x, position.y] = value;
    }

    void FindPath(Vector2Int start, Vector2Int target) {
        int count = 10000;
        Node startNode = new Node();
        bool pathFound = false;

        startNode.hCost = Vector2Int.Distance(start, target);
        startNode.fCost = startNode.gCost + startNode.hCost;
        startNode.cord = start;

        _nodes.Clear();
        _nodes.Add(startNode);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (grid[x, y] == -1)
                    grid[x, y] = 0;
            }

        }

        while (!pathFound)
        {
            if (count < 0)
                throw new Exception("PathFinder.FindPath: 'Could not find path (finding path took too long).'");

            Node node = GetCheapestNode();
            Debug.Log(node);
            node.locked = true;
            grid[(int)node.cord.x, (int)node.cord.y] = -1;
            Debug.Log("node: " + node.cord);

            _nodes.Add(node);
           
            if (node.hCost <= 1)
                pathFound = true;
            else
                CalculateSurroundingNodes(node, start, target);

            count--;
        }

       TracePath(_nodes[_nodes.Count - 1]);
    }

    Node GetCheapestNode()
    {
        Node cheapestNode = new Node();
        bool nodeSet = false;

        for (int i  = 0; i < _nodes.Count; i++)
        {
            Node node = _nodes[i];
            if (!node.locked)
            {
                if (nodeSet == false)
                {
                    cheapestNode = node;
                    nodeSet = true;
                } else if (_nodes[i].fCost < cheapestNode.fCost || (_nodes[i].fCost == cheapestNode.fCost && _nodes[i].hCost < cheapestNode.hCost))
                {
                    cheapestNode = node;
                }
            }
            
        }

        if (nodeSet == false)
            throw new Exception("PathFinder.GetCheapestNode: 'No path available (No available nodes left)'");

        return cheapestNode;
    }

    void CalculateSurroundingNodes(Node node, Vector2Int start, Vector2Int target)
    {
        int adjacentNodeIndex = _nodes.Count - 1;
        Vector2Int cord = node.cord;

        for (int x = (int)cord.x - 1; x < cord.x + 2; x++)
        {
            for (int y = (int)cord.y - 1; y < cord.y + 2; y++)
            {
                if ((x > -1 && y > -1 && x < gridSize.x && y < gridSize.y) && (x != cord.x || y != cord.y))
                {
                    if (x == cord.x || y == cord.y) {
                        if (grid[x, y] != 1 && grid[x, y] != -1) {
                            bool dontCalculate = false;
                            for (int i = 0; i < _nodes.Count; i++) {
                                if (_nodes[i].cord.x == x && _nodes[i].cord.y == y)
                                    dontCalculate = true;
                            }
                            if (!dontCalculate) {
                                Node _node = new Node();
                                _node.adjacentNode = adjacentNodeIndex;
                                _node.gCost = _nodes[_node.adjacentNode].gCost + Vector2.Distance(new Vector2(x, y), _nodes[_node.adjacentNode].cord);
                                _node.hCost = Vector2.Distance(new Vector2(x, y), target);
                                _node.fCost = _node.gCost + _node.hCost;
                                _node.cord = new Vector2Int(x, y);
                                _map2[x, y] = 4;
                                Debug.Log("at:" + _node.cord + " gCost:" + _node.gCost + " hCost: " + Vector2.Distance(new Vector2(x, y), target) + " fCost: " + _node.fCost);
                                _nodes.Add(_node);
                            }
                        }
                    }
                }
            }
        }
    }

    void TracePath(Node node)
    {
        int count = 10000;
        grid[(int)node.cord.x, (int)node.cord.y] = 2;

        while (true)
        {
            if (count < 0)
                throw new Exception("PathFinder.TracePath: 'Could not trace path (path tracing took too long).'");

            if (node.adjacentNode <= 0) { return; }

            node = _nodes[node.adjacentNode];
            grid[(int)node.cord.x, (int)node.cord.y] = 2;

            count--;
        }
    }

    private void OnDrawGizmos()
    {

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                Gizmos.color = (grid[x, y] == 2) ? Color.red : (grid[x, y] == 1) ? Color.black : Color.white;
                Gizmos.color = (_map2[x, y] == 5) ? Color.yellow : (_map2[x, y] == 3) ? Color.green : Gizmos.color;
                Gizmos.DrawSphere(new Vector3(x * 5, 0, y * 5), 1);
            }
        }
    }
}
