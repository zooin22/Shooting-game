using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapGenerator : MonoBehaviour {

    private static TileMapGenerator instance;
    public Tilemap wallTilemap,floorTilemap;
    public TilemapCollider2D wallColl, floorColl;
    TileBase[] floorTile;
    TileBase[] wallTile;

    public static TileMapGenerator GetInstance()
    {
        if (!instance)
        {
            instance = GameObject.FindObjectOfType(typeof(TileMapGenerator)) as TileMapGenerator;
        }

        return instance;
    }

    public void CreateTileMap(Node[,] map)
    {
        floorTile = new TileBase[30];
        for (int i = 0; i < 30; i++) {
            floorTile[i] = Resources.Load("Tiles/Floor/tiles_" + i) as TileBase;
        }
        wallTile = new TileBase[12];
        for (int i = 0; i < 12; i++)
        {
            wallTile[i] = Resources.Load("Tiles/Wall/wallTiles_" + i) as TileBase;
        }
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        for (int i = width - 1; i > 0; i--)
        {
            for (int j = height -1; j > 0; j--)
            {
                if (map[i,j].tileType == TileType.FLOOR)
                {
                    int random = Random.Range(0, floorTile.Length);
                    floorTilemap.SetTile(new Vector3Int(map[i, j].gridX, map[i, j].gridY, 0),floorTile[random]);
                }
                else if(map[i,j].tileType == TileType.EDGE)
                {
                    int random = Random.Range(0, wallTile.Length);
                    wallTilemap.SetTile(new Vector3Int(map[i, j].gridX, map[i, j].gridY, 0), wallTile[random]);
                }
            }
        }
        floorTilemap.gameObject.SetActive(false);
        floorTilemap.gameObject.SetActive(true);
        wallColl.gameObject.SetActive(false);
        wallColl.gameObject.SetActive(true);
    }
}
