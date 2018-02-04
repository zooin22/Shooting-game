using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGrid : MonoBehaviour
{
    public MapGenerator map;
    float nodeRadius;
    Node[,] mapGrid;

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
        gridSizeX = map.GetWidth();
        gridSizeY = map.GetHeight();
        mapGrid = map.GetMap();

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = Vector3.right * (x * nodeRadius) + Vector3.up * (y * nodeRadius);

                mapGrid[x, y].worldPosition = worldPoint;
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
                        if (mapGrid[checkX, node.gridY].tileType == TileType.EDGE)
                        {
                            continue;
                        }
                    }
                    if (y == 1 || y == -1)
                    {
                        if (mapGrid[node.gridX, checkY].tileType == TileType.EDGE)
                        {
                            continue;
                        }
                    }
                    neighbours.Add(mapGrid[checkX, checkY]);
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
                    neighbours.Add(mapGrid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node GetNode(int x,int y)
    {
        return mapGrid[x, y];
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        int X = (int)(worldPosition.x) / map.GetSize();
        int Y = (int)(worldPosition.y) / map.GetSize();
        return mapGrid[X,Y];
    }
}