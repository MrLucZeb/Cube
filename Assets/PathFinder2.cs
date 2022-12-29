using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder2 : MonoBehaviour
{
    struct Node
    {
        public Vector2Int adjacentNode;
        public float fCost;
        public float hCost;
        public float gCost;
        public bool isLocked;
        public bool isPath;
        public Vector2Int location;
    }
    
    Node[,] grid;
    public Vector2Int a;
    public Vector2Int b;

    // Start is called before the first frame update
    void Start()
    {
        CreateGrid(25, 25);
        FindPath();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public List<Vector2Int> FindPath() {
        // CHANGE THIS TRASH STUFF
        grid[a.x, a.y].isLocked = true;
        grid[a.x, a.y].hCost = Vector2.Distance(a, b);
        Node node = grid[a.x, a.y];
        int count = 0;
        Debug.Log("THE COST " + grid[a.x, a.y].hCost + (node.hCost >= 1));
        while (node.hCost >= 1) {
            count++;
            if (count > 1000)
                throw new Exception("Couldn't find path");

            CalculateSurroundingNodes(new Vector2Int(a.x, a.y));
            node = FindCheapestNode();
            node.isLocked = true;
            grid[node.location.x, node.location.y].isLocked = true;
            
        }
        Debug.Log("CHEAPEST NODE: " + node.location + " HCOST: " + node.hCost + "IS LOCKED " + grid[node.location.x, node.location.y].isLocked);
        return TracePath();
    }

    void CreateGrid(int width, int height) {
        grid = new Node[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                grid[x, y].fCost = -1;
                grid[x, y].gCost = -1;
                grid[x, y].hCost = -1;
                grid[x, y].isLocked = false;
                grid[x, y].location = new Vector2Int(x, y);
            }
        }
    }

    void CalculateSurroundingNodes(Vector2Int node) {
        for (int x = node.x - 1; x < node.x + 2; x++) {
            for (int y = node.y - 1; y < node.y + 2; y++) {
                if (x > -1 && y > -1 && x < grid.GetLength(0) && y < grid.GetLength(1)) { // Boundary check

                    if (grid[x, y].fCost == -1 && grid[x, y].isLocked == false) { // Check if node has already been calculated
                        grid[x, y].gCost = Vector2.Distance(new Vector2(x, y), node) + grid[node.x, node.y].gCost;
                        grid[x, y].hCost = Vector2.Distance(new Vector2(x, y), b);
                        Debug.Log("HCOST OF" + grid[x, y].location + " IS " + grid[x, y].hCost);
                        grid[x, y].fCost = grid[x, y].gCost + grid[x, y].hCost;
                        grid[x, y].adjacentNode = node;

                    } else
                        Debug.Log("WRONG " + x + ", " + y);
                }
            }
        }
    }

    Node FindCheapestNode() {
        Node cheapest = new Node();
        bool firstTime = true;

        foreach (Node node in grid) {
            Debug.Log("loc " + node.location + " locked: " + node.isLocked);
            if (node.fCost != -1 && node.isLocked == false) {
                if (firstTime || node.fCost < cheapest.fCost || (node.fCost == cheapest.fCost && node.hCost < cheapest.hCost)) {
                    cheapest = node;
                    firstTime = false;
                    Debug.Log("SET " + node.location);
                }
            }
        }

        return cheapest;
    }

    List<Vector2Int> TracePath() {
        List<Vector2Int> path = new List<Vector2Int>();
        Node node = grid[b.x, b.y];
        grid[b.x, b.y].isPath = true; // DEBUG
        int count = 0; 

        while (node.adjacentNode != b) {
            count++; 
            if (count > 1000)
                throw new Exception("Couldn't trace path");
            Debug.Log("COORD: " + node.adjacentNode.x + ", " + node.adjacentNode.y);

            grid[node.adjacentNode.x, node.adjacentNode.y].isPath = true; //DEBUG
            node = grid[node.adjacentNode.x, node.adjacentNode.y]; 
        }

        return path;
    }
    private void OnDrawGizmos() {
        if (grid == null) { return; }
        for (int x = 0; x < grid.GetLength(0); x++) {
            for (int y = 0; y < grid.GetLength(1); y++) {
                Gizmos.color = (x == a.x && y == a.y) ? Color.green : (x == b.x && y == b.y) ? Color.red : (grid[x, y].isPath) ? Color.yellow : Color.black;
                Gizmos.DrawSphere(new Vector3(x * 5, 5, y * 5), 1);
            }
        }
    }
}
