using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathFinder3 : MonoBehaviour
{
    class Node
    {
        public Vector2 cord;
        public int adjacentNode = -1;
        public bool locked = false;
        public float gCost = 0;
        public float hCost = 0;
        public float fCost = 0;
    }

    int[,] _map = new int[25, 25];
    int[,] _map2 = new int[25, 25];
    List<Node> _nodes = new List<Node>();


    // Start is called before the first frame update
    void Start()
    {
        _map[5, 10] = 1;
        _map[6, 10] = 1;
        _map[7, 10] = 1;
        _map[8, 10] = 1;
        _map[9, 10] = 1;
        _map[10, 10] = 1;
        _map[11, 10] = 1;
        _map[12, 10] = 1;
        _map[13, 10] = 1;
        _map[13, 9] = 1;
        _map[13, 8] = 1;
        _map[13, 7] = 1;
        _map[13, 12] = 1;
        _map[13, 13] = 1;

        FindPath(new Vector2(2, 2), new Vector2(15, 15));
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            for (int x = 0; x < 25; x++)
            {
                for (int y = 0; y < 25; y++)
                {
                    if (_map[x, y] == 2 || _map[x, y] == 3 || _map[x, y] == 4)
                        _map[x, y] = 0;
                }

            }

            bool valid = false;
            Vector2 start = new Vector2(Random.Range(0, 25), Random.Range(0, 25));
            Vector2 target = new Vector2(Random.Range(0, 25), Random.Range(0, 25));
            while (!valid)
            {
                for (int x = 0; x < 25; x++)
                {
                    for (int y = 0; y < 25; y++)
                    {
                        if (_map[(int)start.x, (int)start.y] == 1)
                        {
                            start = new Vector2(Random.Range(0, 25), Random.Range(0, 25));
                        }
                        else if (_map[(int)target.x, (int)target.y] == 1)
                        {
                            target = new Vector2(Random.Range(0, 25), Random.Range(0, 25));
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

    void FindPath(Vector2 start, Vector2 target) {
        int count = 10000;
        Node startNode = new Node();
        bool pathFound = false;

        startNode.hCost = Vector2.Distance(start, target);
        startNode.fCost = startNode.gCost + startNode.hCost;
        startNode.cord = start;

        _nodes.Clear();
        _nodes.Add(startNode);

        for (int x = 0; x < 25; x++)
        {
            for (int y = 0; y < 25; y++)
            {
                if (_map[x, y] == -1)
                    _map[x, y] = 0;
            }

        }

        while (!pathFound)
        {
            if (count < 0)
                throw new Exception("PathFinder.FindPath: 'Could not find path (finding path took too long).'");

            Node node = GetCheapestNode();
            node.locked = true;
            _map[(int)node.cord.x, (int)node.cord.y] = -1;
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

    void CalculateSurroundingNodes(Node node, Vector2 start, Vector2 target)
    {
        int adjacentNodeIndex = _nodes.Count - 1;
        Vector2 cord = node.cord;

        for (int x = (int)cord.x - 1; x < cord.x + 2; x++)
        {
            for (int y = (int)cord.y - 1; y < cord.y + 2; y++)
            {
                if ((x > -1 && y > -1 && x < 25 && y < 25) && (x != cord.x || y != cord.y))
                {
                    if (x == cord.x || y == cord.y) {
                        if (_map[x, y] != 1 && _map[x, y] != -1) {
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
                                _node.cord = new Vector2(x, y);
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
        _map[(int)node.cord.x, (int)node.cord.y] = 2;

        while (true)
        {
            if (count < 0)
                throw new Exception("PathFinder.TracePath: 'Could not trace path (path tracing took too long).'");

            if (node.adjacentNode <= 0) { return; }

            node = _nodes[node.adjacentNode];
            _map[(int)node.cord.x, (int)node.cord.y] = 2;

            count--;
        }
    }
    float GetHCost(Vector2 a, Vector2 b)
    {
        float x = a.x - b.x;
        float y = a.y - b.y;
        x = (x < 0) ? -x : x;
        y = (y < 0) ? -y : y;

        return x + y;
    }

    private void OnDrawGizmos()
    {
        for (int x = 0; x < _map.GetLength(0); x++)
        {
            for (int y = 0; y < _map.GetLength(1); y++)
            {
                Gizmos.color = (_map[x, y] == 2) ? Color.red : (_map[x, y] == 1) ? Color.black : Color.white;
                Gizmos.color = (_map2[x, y] == 5) ? Color.yellow : (_map2[x, y] == 3) ? Color.green : Gizmos.color;
                Gizmos.DrawSphere(new Vector3(x * 5, 0, y * 5), 1);
            }
        }
    }
}
