using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum TileType { NONE, FLOOR, EDGE, DOOR }

public struct Coord
{
    public int tileX;
    public int tileY;

    public Coord(int x, int y)
    {
        tileX = x;
        tileY = y;
    }

    public float Distance(Coord coord)
    {
        return Mathf.Pow(tileX - coord.tileX, 2) + Mathf.Pow(tileY - coord.tileY, 2);
    }
}

public class Room
{

    public List<Coord> tiles;
    public List<Coord> edgeTiles;
    public List<Coord> doorTiles;
    public List<Room> connectedRooms;

    public int roomSize;
    public Coord center;
    protected bool isRoom;

    public Room() { }

    public Room(List<Coord> tiles, List<Coord> edgeTiles, int roomSize)
    {
        connectedRooms = new List<Room>();
        this.tiles = tiles;
        this.edgeTiles = edgeTiles;
        this.roomSize = roomSize;
        isRoom = true;
        center = new Coord((edgeTiles[0].tileX + edgeTiles[edgeTiles.Count - 1].tileX) / 2, (edgeTiles[0].tileY + edgeTiles[edgeTiles.Count - 1].tileY) / 2);
    }

    public static void ConnectRooms(Room roomA, Room roomB)
    {
        roomA.connectedRooms.Add(roomB);
        roomB.connectedRooms.Add(roomA);
    }
    public bool IsConnected(Room otherRoom)
    {
        return connectedRooms.Contains(otherRoom);
    }
    public bool GetIsRoom()
    {
        return isRoom;
    }
    public void CreateDoor(Coord coord)
    {

    }
}

public class Passage : Room
{
    Room roomA, roomB;

    public Passage(Room roomA, Room roomB)
    {
        this.roomA = roomA;
        this.roomB = roomB;
        connectedRooms = new List<Room>();
        tiles = new List<Coord>();
        edgeTiles = new List<Coord>();
        isRoom = false;
    }
    public bool MakePassage() {
        int roomAtileCount = roomA.edgeTiles.Count;
        int roomBtileCount = roomB.edgeTiles.Count;

        int roomAPoint = 0;
        int roomBPoint = 0;

        float minA = roomA.center.Distance(roomB.center);
        float minB = minA;

        for (int i = 0; i < roomAtileCount; i++)
        {
            if (minA >= roomA.edgeTiles[i].Distance(roomB.center))
            {
                minA = roomA.edgeTiles[i].Distance(roomB.center);
                roomAPoint = i;
            }

        }
        for (int j = 0; j < roomBtileCount; j++)
        {
            if (minB >= roomB.edgeTiles[j].Distance(roomA.center))
            {
                minB = roomB.edgeTiles[j].Distance(roomA.center);
                roomBPoint = j;
            }
        }
        if (roomA.edgeTiles.Count == 0 || roomB.edgeTiles.Count == 0)
            return false;
        int aTileX = roomA.edgeTiles[roomAPoint].tileX; // A 룸 시작 타일 X
        int aTileY = roomA.edgeTiles[roomAPoint].tileY; // A 룸 시작 타일 Y
        int bTileX = roomB.edgeTiles[roomBPoint].tileX; // B 룸 시작 타일 X
        int bTileY = roomB.edgeTiles[roomBPoint].tileY; // B 룸 시작 타일 Y
        Coord aCoord = new Coord(aTileX, aTileY);
        Coord bCoord = new Coord(bTileX, bTileY);
        return CreateRoute(aCoord, bCoord);
    }

    public bool CreateRoute(Coord aCoord,Coord bCoord)
    {
        Map map = Object.FindObjectOfType(typeof(Map)) as Map;
        Pathfinding pathfinding = Object.FindObjectOfType(typeof(Pathfinding)) as Pathfinding;
        map.SetMap(aCoord.tileX, aCoord.tileY, TileType.NONE);
        map.SetMap(bCoord.tileX, bCoord.tileY, TileType.NONE);
        center = new Coord((aCoord.tileX + bCoord.tileX) / 2, (aCoord.tileY + bCoord.tileY) / 2);
        Vector3[] passage = pathfinding.FindPassage(aCoord, bCoord);
        if(passage == null)
        {
            return false;
        }
        int length = passage.Length;
        for (int i = 0; i < length; i++)
        {
            tiles.Add(new Coord((int)passage[i].x / map.GetSize(), (int)passage[i].y / map.GetSize()));
            map.SetMap((int)passage[i].x / map.GetSize(), (int)passage[i].y / map.GetSize(), TileType.FLOOR);
        }
        for (int i = 0; i < length; i++)
        {
            CreateEdge(map, tiles[i].tileX, tiles[i].tileY);
        }
        return true;
    }

    public void CreateEdge(Map map,int i,int j)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = i + x;
                int checkY = j + y;

                if (checkX >= 0 && checkX < map.GetWidth() && checkY >= 0 && checkY < map.GetHeight())
                {
                    if ((x == 1 || x == -1) && (y == -1 || y == 1))
                        continue;

                    if (map.GetTileType(checkX, checkY) == TileType.NONE)
                    {
                        map.SetMap(checkX, checkY, TileType.EDGE);
                        edgeTiles.Add(new Coord(checkX, checkY));
                    }
                }
                else
                {
                }
            }
        }       
    }
       
}