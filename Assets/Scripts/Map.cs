using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : BaseObject
{
    [Range(1, 10)]
    public int count;
    public int height;
    public int width;

    int[,] map;

    // Use this for initialization
    void Start()
    {
        GenerateMap();
    }
    
    public int[,] GetMap()
    {
        return map;
    }

    public void GenerateMap()
    {
        map = new int[width, height];
        SeparateMap(count, -width / 2, -height / 2, width / 2, height / 2);
        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(map, 1);
    }    

    public Room SeparateMap(int count, int x1, int y1, int x2, int y2)
    {
        if (count < 1)
        {
            return CreateMap(x1, y1, x2, y2);
        }
        --count;
        bool isHorizon = (Random.value > 0.5f);
        float cutRange = Random.Range(40, 60);
        int height = y2 - y1;
        int width = x2 - x1;
        float ratioHeight = height * (cutRange / 100);
        float ratioWidth = width * (cutRange / 100);
        float ratio = (float)height / width;

        if (ratio < 0.8f) // 가로 길이 1 세로 길이 1
        {
            isHorizon = false;
        }
        else if(ratio > 1.2f) // 가로 길이 1 세로 길이 1
        {
            isHorizon = true;
        }

        Room roomA, roomB;
        Passage passage;

        if (isHorizon) // 가로로 자를 경우
        {
            roomA = SeparateMap(count, x1, y1, x2, y1 + (int)ratioHeight);
            roomB = SeparateMap(count, x1, y1 + (int)ratioHeight, x2, y2);
        }
        else // 세로로 자를 경우
        {
            roomA = SeparateMap(count, x1, y1, x1 + (int)ratioWidth, y2);
            roomB = SeparateMap(count, x1 + (int)ratioWidth, y1, x2, y2);
        }

        DrawEdgeMesh(roomA);
        DrawEdgeMesh(roomB);

        if (roomA.GetIsRoom() == true)
        {
            Room.ConnectRooms(roomA, roomB);
        }
        passage = new Passage(roomA, roomB);

        return passage;
    }
    public void DrawEdgeMesh(Room room)
    {
        if (room.GetIsRoom())
        {
            for (int i = 0; i < room.edgeTiles.Count; i++)
            {
                map[room.edgeTiles[i].tileX + width / 2, room.edgeTiles[i].tileY + height / 2] = 1;
            }
        }
    }

    private Room CreateMap(int x1, int y1, int x2, int y2) {
        List<Coord> tiles = new List<Coord>();
        List<Coord> edgeTiles = new List<Coord>();
        int width = x2 - x1;
        int height = y2 - y1;
        int top = 0 ,right = 0, down = 0, left = 0;

        if (width > height)//가로가 더 넓음
        {
            right = (int)(width * (float)Random.Range(17, 35) / 100);
            left = (int)(width * (float)Random.Range(17, 35) / 100);
            top = (int)(height * (float)Random.Range(8, 17) / 100);
            down = (int)(height * (float)Random.Range(8, 17) / 100);
        }
        else
        {
            right = (int)(width * (float)Random.Range(8, 17) / 100);
            left = (int)(width * (float)Random.Range(8, 17) / 100);
            top = (int)(height * (float)Random.Range(17, 35) / 100);
            down = (int)(height * (float)Random.Range(17, 35) / 100);
        }
        left = x1 + left;
        right = x2 - right;
        down = y1 + down;
        top = y2 - top;
        for (int i = left; i < right; i++)
        {
            for (int j = down; j < top; j++)
            {
                if (i == left || i == right - 1 || j == down || j == top - 1)
                {
                    edgeTiles.Add(new Coord(i, j));
                }
                else
                {
                    tiles.Add(new Coord(i, j));
                }
            }
        }
        Room room = new Room(tiles, edgeTiles, width * height);
        return room;
    }
}
