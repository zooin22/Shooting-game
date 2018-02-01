﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour
{

    Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    public Vector3[] FindPath(Coord startCoord , Coord targetCoord)
    {

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;
        Node startNode = grid.GetNode(startCoord.tileX,startCoord.tileY);
        Node targetNode = grid.GetNode(targetCoord.tileX, targetCoord.tileY);
        startNode.parent = startNode;

        if (startNode.tileType == TileType.NONE && targetNode.tileType == TileType.NONE)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);
                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }
                foreach (Node neighbour in grid.GetPassageNeighbours(currentNode))
                {

                    if (neighbour.tileType != TileType.NONE || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode,true);
            return waypoints;
        }
        return null;
    }

    public void FindPath(PathRequest request, Action<PathResult> callback)
    {

        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(request.pathStart);
        Node targetNode = grid.NodeFromWorldPoint(request.pathEnd);
        startNode.parent = startNode;

        if (startNode.tileType == TileType.FLOOR && targetNode.tileType == TileType.FLOOR)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    pathSuccess = true;
                    break;
                }
                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (neighbour.tileType != TileType.FLOOR || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode,false);
            pathSuccess = waypoints.Length > 0;
        }
        callback(new PathResult(waypoints, pathSuccess, request.callback));

    }


    Vector3[] RetracePath(Node startNode, Node endNode,bool isMap)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        if(isMap)
            path.Add(startNode);
        Vector3[] waypoints = SimplifyPath(path);

        Array.Reverse(waypoints);
        return waypoints;

    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 0; i < path.Count; i++)
        {
            //Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            //if (directionNew != directionOld || CheckNearColl(path[i]))
            //{
                waypoints.Add(path[i].worldPosition);
            //}
            //directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }


}