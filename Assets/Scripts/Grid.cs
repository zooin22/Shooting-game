﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : BaseObject
{
    public Map map;
    float nodeRadius;
    Node[,] grid;
    
    //float nodeDiameter;
    int gridSizeX, gridSizeY;

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    public void CreateGrid()
    {
        nodeRadius = map.GetSize();
        //nodeDiameter = nodeRadius * 2;
        gridSizeX = map.GetWidth();
        gridSizeY = map.GetHeight();
        grid = map.GetMap();

        //Vector3 worldBottomLeft = transform.position;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = Vector3.right * (x * nodeRadius) + Vector3.up * (y * nodeRadius);

                grid[x, y].worldPosition = worldPoint;
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
    
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    if (x == 1 || x == -1)
                    {
                        if (grid[checkX, node.gridY].tileType == TileType.EDGE)
                        {
                            continue;
                        }
                    }
                    if (y == 1 || y == -1)
                    {
                        if (grid[node.gridX, checkY].tileType == TileType.EDGE)
                        {
                            continue;
                        }
                    }
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public List<Node> GetPassageNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    if ((x == 1 || x == -1) && (y == -1 || y == 1))
                    {
                        continue;

                    }
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node GetNode(int x,int y)
    {
        return grid[x, y];
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        //float percentX = (worldPosition.x + gridSizeX / 2) / gridSizeX;
        //float percentY = (worldPosition.y + gridSizeY / 2) / gridSizeY;
        int X = (int)(worldPosition.x) / map.GetSize();
        int Y= (int)(worldPosition.y) / map.GetSize();
        //percentX = Mathf.Clamp01(percentX);
        //percentY = Mathf.Clamp01(percentY);
        //int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        //int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[X,Y];
    }
}