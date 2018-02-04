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
    public enum DoorDir { LEFT, RIGHT, UP, DOWN };

    public List<Coord> tiles;
    public List<Coord> edgeTiles;
    public List<Coord> doorTiles;
    public List<Room> connectedRooms;
    public List<DoorDir> doorDir;

    public int roomSize;
    public Coord center;
    protected bool isRoom;
    protected int size;

    public Room()
    {
        connectedRooms = new List<Room>();
        doorTiles = new List<Coord>();
        doorDir = new List<DoorDir>();
    }
    public Room(List<Coord> tiles, List<Coord> edgeTiles, int roomSize,int size)
    {
        connectedRooms = new List<Room>();
        doorTiles = new List<Coord>();
        doorDir = new List<DoorDir>();

        this.tiles = tiles;
        this.edgeTiles = edgeTiles;
        this.roomSize = roomSize;
        this.size = size;
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
    public GameObject AttachRoomObject(Transform parent)
    {
        GameObject obj = Object.Instantiate(Resources.Load("Prefabs/Room")) as GameObject;
        obj.transform.parent = parent.transform;
        obj.GetComponent<RoomWrapper>().Init(this, new Vector2(center.tileX * size, center.tileY * size), roomSize);
        return obj;
    }
    public List<Door> CreateDoorObject(Transform parent)
    {
        List<Door> doorList = new List<Door>();
        if (doorTiles.Count == 0)
        {
            return null;
        }
        for (int i = 0; i < doorTiles.Count; i++)
        {
            GameObject obj = Object.Instantiate(Resources.Load("Prefabs/Door")) as GameObject;
            obj.transform.parent = parent.transform;
            Coord doorCoord = doorTiles[i];
            obj.GetComponent<Door>().Init(doorDir[i], new Vector2(doorCoord.tileX * size, doorCoord.tileY * size));
            doorList.Add(obj.GetComponent<Door>());
        }
        return doorList;
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
        doorTiles = new List<Coord>();
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

        CheckDoorDir(aCoord, bCoord);
        CreateDoor(roomAPoint, roomBPoint);

        return CreateRoute(aCoord, bCoord);
    }

    private void CheckDoorDir(Coord a,Coord b)
    {
        if(MapGenerator.GetInstance().GetTileType(a.tileX + 1,a.tileY) == TileType.NONE)
        {
            roomA.doorDir.Add(DoorDir.RIGHT);
        }
        else if (MapGenerator.GetInstance().GetTileType(a.tileX - 1, a.tileY) == TileType.NONE)
        {
            roomA.doorDir.Add(DoorDir.LEFT);
        }
        else if (MapGenerator.GetInstance().GetTileType(a.tileX, a.tileY + 1) == TileType.NONE)
        {
            roomA.doorDir.Add(DoorDir.UP);
        }
        else if (MapGenerator.GetInstance().GetTileType(a.tileX, a.tileY - 1) == TileType.NONE)
        {
            roomA.doorDir.Add(DoorDir.DOWN);
        }

        if (MapGenerator.GetInstance().GetTileType(b.tileX + 1, b.tileY) == TileType.NONE)
        {
            roomB.doorDir.Add(DoorDir.RIGHT);
        }
        else if (MapGenerator.GetInstance().GetTileType(b.tileX - 1, b.tileY) == TileType.NONE)
        {
            roomB.doorDir.Add(DoorDir.LEFT);
        }
        else if (MapGenerator.GetInstance().GetTileType(b.tileX, b.tileY + 1) == TileType.NONE)
        {
            roomB.doorDir.Add(DoorDir.UP);
        }
        else if (MapGenerator.GetInstance().GetTileType(b.tileX, b.tileY - 1) == TileType.NONE)
        {
            roomB.doorDir.Add(DoorDir.DOWN);
        }
    }

    private void CreateDoor(int roomAPoint, int roomBPoint)
    {
        roomA.doorTiles.Add(roomA.edgeTiles[roomAPoint]); // 도어 타일 추가
        roomB.doorTiles.Add(roomB.edgeTiles[roomBPoint]); // 도어 타일 추가
        roomA.edgeTiles.RemoveAt(roomAPoint); // 도어가 될 엣지타일 삭제
        roomB.edgeTiles.RemoveAt(roomBPoint); // 도어가 될 엣지타일 삭제
    }

    private bool CreateRoute(Coord aCoord,Coord bCoord)
    {
        Pathfinding pathfinding = Object.FindObjectOfType(typeof(Pathfinding)) as Pathfinding;
        MapGenerator.GetInstance().SetMap(aCoord.tileX, aCoord.tileY, TileType.NONE);
        MapGenerator.GetInstance().SetMap(bCoord.tileX, bCoord.tileY, TileType.NONE);
        center = new Coord((aCoord.tileX + bCoord.tileX) / 2, (aCoord.tileY + bCoord.tileY) / 2);
        Vector3[] passage = pathfinding.FindPath(aCoord, bCoord);
        if(passage == null)
        {
            return false;
        }
        int length = passage.Length;
        for (int i = 0; i < length; i++)
        {
            tiles.Add(new Coord((int)passage[i].x / MapGenerator.GetInstance().GetSize(), (int)passage[i].y / MapGenerator.GetInstance().GetSize()));
            MapGenerator.GetInstance().SetMap((int)passage[i].x / MapGenerator.GetInstance().GetSize(), (int)passage[i].y / MapGenerator.GetInstance().GetSize(), TileType.FLOOR);
        }
        for (int i = 0; i < length; i++)
        {
            bool success = CreateEdge(tiles[i].tileX, tiles[i].tileY);
            if (!success)
                return false;
        }
        return true;
    }

    private bool CreateEdge(int i,int j)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = i + x;
                int checkY = j + y;

                if (checkX >= 0 && checkX < MapGenerator.GetInstance().GetWidth() && checkY >= 0 && checkY < MapGenerator.GetInstance().GetHeight())
                {
                    if ((x == 1 || x == -1) && (y == -1 || y == 1))
                        continue;

                    if (MapGenerator.GetInstance().GetTileType(checkX, checkY) == TileType.NONE)
                    {
                        MapGenerator.GetInstance().SetMap(checkX, checkY, TileType.EDGE);
                        edgeTiles.Add(new Coord(checkX, checkY));
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }

}