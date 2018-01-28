using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : BaseObject
{
    [Range(1, 5)]
    public int count;
    int height;
    int  width;
    public Grid grid;
    int Size = 2;


    private bool process,isCreated;
    MeshGenerator meshGen;
    Node[,] map;

    // Use this for initialization
    void Start()
    {
        width = 20 * count;
        height = width;
        map = new Node[width, height];
        meshGen = GetComponent<MeshGenerator>();
        process = false;
        isCreated = false;
        CreateMap();
        Debug.DrawLine(new Vector3(0, 0), new Vector3(0, height), Color.white, 10000);
        Debug.DrawLine(new Vector3(0, height), new Vector3(width, height), Color.white, 10000);
        Debug.DrawLine(new Vector3(width, height), new Vector3(width, 0), Color.white, 10000);
        Debug.DrawLine(new Vector3(width, 0), new Vector3(0, 0), Color.white, 10000);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
            CreateMap();
    }

    public void CreateMap()
    {
        if(!process)
            StartCoroutine(CallGenerateMap());
    }

    private IEnumerator CallGenerateMap()
    {
        process = true;
        isCreated = false;
        StartCoroutine(GenerateMap());
        yield return new WaitForSeconds(1f);
        process = false;
    }

    private IEnumerator GenerateMap()
    {
        while (process)
        {
            Debug.Log("Generate Map");

            InitMap();
            grid.CreateGrid();
            List<Room> room = SeparateMap(count, 0, 0, width, height);
            if (room != null)
            {
                meshGen.GenerateMesh(map, Size);
                process = false;
                isCreated = true;
            }
            else
            {
                process = true;
            }
            yield return new WaitForSeconds(0.01f);

        }
    }

    public void SetMap(int i,int j,TileType tileType)
    {
        map[i, j].tileType = tileType;
    }

    public TileType GetTileType(int i,int j)
    {
        return map[i, j].tileType;
    }

    public Node[,] GetMap()
    {
        return map;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public int GetSize()
    {
        return Size;
    }

    public void InitMap()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                map[i, j] = new Node(TileType.NONE, Vector3.zero, i, j);
            }
        }
    }

    public List<Room> SeparateMap(int count, int x1, int y1, int x2, int y2)
    {
        List<Room> resultRoom = new List<Room>();

        if (count < 1)
        {
            resultRoom.Add(CreateMap(x1, y1, x2, y2));
            return resultRoom;
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

        List<Room> roomA, roomB;
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
        if (roomA == null || roomB == null)
            return null;

        int roomAPoint = 0;
        int roomBPoint = 0;
        if (roomA.Count == 1)
        {
            roomAPoint = 0;
            roomBPoint = 0;
        }
        else
        {
            float minDst = roomA[0].center.Distance(roomB[0].center);
            for (int i = 0; i < roomA.Count; i++)
            {               
                for(int j = 0; j < roomB.Count; j++)
                {
                    //Debug.DrawLine(new Vector3(roomA[i].center.tileX, roomA[i].center.tileY, 0), new Vector3(roomB[j].center.tileX, roomB[j].center.tileY, 0), Color.black, 1000);
                    if (minDst >= roomA[i].center.Distance(roomB[j].center))
                    {
                        minDst = roomA[i].center.Distance(roomB[j].center);
                        roomAPoint = i;
                        roomBPoint = j;
                    }
                }
            }
        }
        //Debug.DrawLine(new Vector3(roomA[roomAPoint].center.tileX, roomA[roomAPoint].center.tileY, 0), new Vector3(roomB[roomBPoint].center.tileX, roomB[roomBPoint].center.tileY, 0), Color.white, 1000);

        Room.ConnectRooms(roomA[roomAPoint], roomB[roomBPoint]);
        passage = new Passage(roomA[roomAPoint], roomB[roomBPoint]);
        bool success = passage.MakePassage();
        if (!success)
            return null;
        resultRoom.Add(passage);

        for(int i = 0; i < roomA.Count; i++)
        {
            resultRoom.Add(roomA[i]);
        }
        for (int i = 0; i < roomB.Count; i++)
        {
            resultRoom.Add(roomB[i]);
        }
        return resultRoom;
    }
    
    private Room CreateMap(int x1, int y1, int x2, int y2) {
        List<Coord> tiles = new List<Coord>();
        List<Coord> edgeTiles = new List<Coord>();
        int mWidth = x2 - x1;
        int mHeight = y2 - y1;
        int top = 0 ,right = 0, down = 0, left = 0;

        if (mWidth > mHeight)//가로가 더 넓음
        {
            right = (int)(mWidth * (float)Random.Range(17, 35) / 100);
            left = (int)(mWidth * (float)Random.Range(17, 35) / 100);
            top = (int)(mHeight * (float)Random.Range(11, 17) / 100);
            down = (int)(mHeight * (float)Random.Range(11, 17) / 100);
        }
        else
        {
            right = (int)(mWidth * (float)Random.Range(11, 17) / 100);
            left = (int)(mWidth * (float)Random.Range(11, 17) / 100);
            top = (int)(mHeight * (float)Random.Range(17, 35) / 100);
            down = (int)(mHeight * (float)Random.Range(17, 35) / 100);
        }
        left = x1 + left;
        right = x2 - right;
        down = y1 + down;
        top = y2 - top;
        for (int i = left; i < right; i++)
        {
            for (int j = down; j < top; j++)
            { 
                if (i == left || i == right - 1 || j == down || j == top - 1) // 경계 테두리를 채움
                {
                    if ((i == left || i == right - 1) && (j == down || j == top - 1)) // 구석은 채우지 않음.
                        continue;
                    map[i, j].tileType = TileType.EDGE;
                    edgeTiles.Add(new Coord(i, j));
                }
                else
                {
                    map[i, j].tileType = TileType.FLOOR;
                    tiles.Add(new Coord(i, j));
                }
            }
        }
        Room room = new Room(tiles, edgeTiles, width * height);
        return room;
    }
}
