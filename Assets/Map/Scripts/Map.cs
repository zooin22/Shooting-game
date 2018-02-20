using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour {
    private static Map instance;

    public GameObject m_startRoom;
    public GameObject m_endRoom;
    public GameObject[] m_monsterRoom;
    public GameObject[] m_itemRoom;
    public GameObject[] m_eventRoom;

    GameObject passageObj;
    Tilemap passageTileMap;

    Queue<Point> passagePositions;
    Rooms[,] roomArr;
    List<GameObject> roomList;

    int mapSize = 20;
    int width = 3;
    int height = 3;

    public static Map GetInstance()
    {
        if (!instance)
        {
            instance = GameObject.FindObjectOfType(typeof(Map)) as Map;
        }

        return instance;
    }

    private void Start()
    {
        InitMap();
        CreateRoom();
    }

    void InitMap()
    {
        roomArr = new Rooms[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                roomArr[i, j] = new Rooms(null, new Vector3(i * mapSize, j * mapSize), false);
            }
        }
        roomList = new List<GameObject>();
        passagePositions = new Queue<Point>();
        passageObj = new GameObject();
        passageObj.name = "Passage";
        passageObj.transform.parent = transform;
        passageObj.AddComponent<TilemapRenderer>();
        passageObj.AddComponent<TilemapCollider2D>().usedByComposite = true;
        passageObj.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        passageObj.AddComponent<CompositeCollider2D>();
        passageTileMap = passageObj.GetComponent<Tilemap>();
    }

    void CreateRoom()
    {
        ResetRoom();
        int roomNum = Random.Range(7, 10);
        SetRoom(roomNum);
    }

    void ResetRoom()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                roomArr[i, j].isRoom = false;
            }
        }
    }

    void SetRoom(int _roomNum)
    {
        
        int x;
        int y;
        _roomNum -= 2;
        roomList.Add(m_startRoom);
        roomList.Add(m_endRoom);

        for(int i = 0; i < _roomNum; i++)
        {
            roomList.Add(m_monsterRoom[0]);
        }
        for(int i=roomList.Count; i<width*height;i++)
        {
            roomList.Add(null);
        }
        ShuffleList(roomList);

        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                GameObject roomObj = roomList[i * height + j];
                if (roomObj == null)
                {
                    continue;
                }
                roomArr[i, j].isRoom = true;
                GameObject obj = LoadRoom(roomObj);
                roomArr[i, j].SetObject(obj);
                obj.transform.parent = transform;
                obj.transform.position = roomArr[i,j].m_position;
            }
        }
        ConnectRoom();
    }

    void ConnectRoom()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (!roomArr[i, j].isRoom)
                    continue;
                if (i <= width - 2)
                {
                    if (roomArr[i + 1, j].isRoom)
                    {
                        MakePassage(roomArr[i, j], roomArr[i + 1, j] , true);
                    }
                }
                if (j <= height - 2)
                {
                    if (roomArr[i, j + 1].isRoom)
                    {
                        MakePassage(roomArr[i, j], roomArr[i, j + 1] , false);
                    }
                }
            }
        }
        MakePassageWall();
    }

    void MakePassage(Rooms A, Rooms B, bool isHorizon)
    {
        if (isHorizon)
        {
            Debug.DrawLine(A.m_doorTiles[1], B.m_doorTiles[0],Color.blue , 1000);
            WorkGird(A.m_doorTiles[1], B.m_doorTiles[0]);
            A.m_doorList.Add(A.m_doorTiles[1]);
            B.m_doorList.Add(B.m_doorTiles[0]);
            A.SetDoor(A.m_doorTiles[1]);
            B.SetDoor(B.m_doorTiles[0]);
        }
        else
        {
            Debug.DrawLine(A.m_doorTiles[2], B.m_doorTiles[3], Color.blue, 1000);
            WorkGird(A.m_doorTiles[2], B.m_doorTiles[3]);
            A.m_doorList.Add(A.m_doorTiles[2]);
            B.m_doorList.Add(B.m_doorTiles[3]);
            A.SetDoor(A.m_doorTiles[2]);
            B.SetDoor(B.m_doorTiles[3]);
        }
    }

    void WorkGird(Vector3Int p1, Vector3Int p2)
    {
        int dx = p1.x - p2.x, dy = p1.y - p2.y;
        int nx = Mathf.Abs(dx), ny = Mathf.Abs(dy);
        int sign_x = dx < 0 ? 1 : -1, sign_y = dy < 0 ? 1 : -1;
        Point p = new Point(p1.x, p1.y);
        TileBase floorTile = TileManager.GetInstance().floorTile;
        passageTileMap.SetTile(new Vector3Int(p.x, p.y, 0), floorTile);
        int ix, iy;
        for (ix = 0,iy = 0;ix < nx-1 || iy < ny-1;)
        {
            if ((0.5 + ix) / nx < (0.5 + iy) / ny)
            {
                p.x += sign_x;
                ix++;
            }
            else
            {
                p.y += sign_y;
                iy++;
            }
            passageTileMap.SetTile(new Vector3Int(p.x, p.y, 0), floorTile);
            passagePositions.Enqueue(new Point(p.x, p.y));
        }
        if ((0.5 + ix) / nx < (0.5 + iy) / ny)
        {
            p.x += sign_x;
            ix++;
        }
        else
        {
            p.y += sign_y;
            iy++;
        }
        passageTileMap.SetTile(new Vector3Int(p.x, p.y, 0), floorTile);
    }

    void MakePassageWall()
    {
        TileBase wallTile = TileManager.GetInstance().wallTile;

        while(passagePositions.Count>0)
        {
            Point p = passagePositions.Dequeue();
            if(passageTileMap.GetTile(new Vector3Int(p.x - 1, p.y, 0)) == null)
            {
                passageTileMap.SetTile(new Vector3Int(p.x - 1, p.y, 0),wallTile);
            }
            if(passageTileMap.GetTile(new Vector3Int(p.x + 1, p.y, 0)) == null)
            {
                passageTileMap.SetTile(new Vector3Int(p.x + 1, p.y, 0), wallTile);
            }
            if (passageTileMap.GetTile(new Vector3Int(p.x, p.y + 1, 0)) == null)
            {
                passageTileMap.SetTile(new Vector3Int(p.x, p.y + 1, 0), wallTile);
            }
            if (passageTileMap.GetTile(new Vector3Int(p.x, p.y - 1, 0)) == null)
            {
                passageTileMap.SetTile(new Vector3Int(p.x, p.y - 1, 0), wallTile);
            }
        }
    }

    void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        System.Random rnd = new System.Random();
        while (n > 1)
        {
            int k = (rnd.Next(0, n) % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    GameObject LoadRoom(GameObject _obj)
    {
        return Object.Instantiate(_obj);
    }

    class Point
    {
        public int x;
        public int y;
        public Point(int _x,int _y)
        {
            x = _x;
            y = _y;
        }
    }
}

public class Rooms
{
    GameObject m_roomObj;
    public Vector3 m_position;
    public bool isRoom;
    public Vector3Int[] m_doorTiles; // LEFT, RIGHT, UP, DOWN 
    public List<Vector3Int> m_doorList;

    public Rooms(GameObject _roomObj, Vector3 _position, bool _isRoom)
    {
        m_roomObj = _roomObj;
        m_position = _position;
        isRoom = _isRoom;
        m_doorTiles = new Vector3Int[4];
        m_doorList = new List<Vector3Int>();
    }
    public GameObject GetObject()
    {
        return m_roomObj;
    }
    public void SetObject(GameObject _roomObj)
    {
        m_roomObj = _roomObj;
        SetDoorTile();
    }
    public void SetDoor(Vector3Int _position)
    {
        Tilemap tilemap = m_roomObj.GetComponent<Tilemap>();
        TileBase doorTile = TileManager.GetInstance().doorTile;
        tilemap.SetTile(new Vector3Int(_position.x - (int)m_position.x, _position.y - (int)m_position.y, 0), doorTile);
    }
    void SetDoorTile()
    {
        Tilemap tilemap = m_roomObj.GetComponent<Tilemap>();
        BoundsInt size = tilemap.cellBounds;
        TileBase wallTile = TileManager.GetInstance().wallTile;
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(position);
           
            if (null != tile)
            {
                if (tile.name == "Door")
                {
                    if (tilemap.GetTile(new Vector3Int(position.x - 1, position.y, 0)) == null)
                    {
                        m_doorTiles[0] = new Vector3Int(position.x + (int)m_position.x, position.y + (int)m_position.y, 0);
                    }
                    else if (tilemap.GetTile(new Vector3Int(position.x + 1, position.y, 0)) == null)
                    {
                        m_doorTiles[1] = new Vector3Int(position.x + (int)m_position.x, position.y + (int)m_position.y, 0);
                    }
                    else if (tilemap.GetTile(new Vector3Int(position.x, position.y + 1, 0)) == null)
                    {
                        m_doorTiles[2] = new Vector3Int(position.x + (int)m_position.x, position.y + (int)m_position.y, 0);
                    }
                    else
                    {
                        m_doorTiles[3] = new Vector3Int(position.x + (int)m_position.x, position.y + (int)m_position.y, 0);
                    }
                    tilemap.SetTile(new Vector3Int(position.x, position.y, 0),wallTile);
                }
            }
        }
    }
}
