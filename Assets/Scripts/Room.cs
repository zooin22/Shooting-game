using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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
        isRoom = false;
        CreateRoute();
    }

    public void CreateRoute()
    {
        if (roomA.GetIsRoom())
        {
            int roomAtileCount = roomA.edgeTiles.Count;
            int roomBtileCount = roomB.edgeTiles.Count;

            int roomAPoint = 0;
            int roomBPoint = 0;

            float minA = roomA.center.Distance(roomB.center);
            float minB = minA;

            for(int i=0;i< roomAtileCount; i++)
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
            Debug.DrawLine(new Vector3(roomA.edgeTiles[roomAPoint].tileX, roomA.edgeTiles[roomAPoint].tileY, 0), new Vector3(roomB.edgeTiles[roomBPoint].tileX, roomB.edgeTiles[roomBPoint].tileY, 0), Color.green, 100);

        }
    }

}