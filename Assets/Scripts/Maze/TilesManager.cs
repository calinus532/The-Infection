using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilesManager : MonoBehaviour
{
    static public void fillTiles(Tilemap gridMap, Tile tile, Vector2 corner1, Vector2 corner2)
    {
        for (int i = (int)corner1.x; i <= corner2.x; i++)
        {
            for (int j = (int)corner1.y; j <= corner2.y; j++)
            {
                gridMap.SetTile(new Vector3Int(i, j), tile);
            }
        }
    }

    static public bool detectTileAt(int x, int y, Tile tile, Tilemap grid)
    {
        if (grid.GetTile(new Vector3Int(x, y)) == tile) return true;
        return false;
    }

    static public bool detectTilesAt(int x, int y, Tile[] tile, Tilemap grid)
    {
        foreach (Tile tile2 in tile) 
        {
            if (grid.GetTile(new Vector3Int(x, y)) == tile2) return true;
        }
        return false;
    }

    static public int TileIdx(int x, int y, int type, List<Vector3> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            if(list[i].z == type)
                if (list[i].x == x && list[i].y == y) 
                    return i;
        }
        return -1;
    }

    static public bool SetMapTile(int x, int y, Tile tile, Tilemap gridMap) 
    {
        if (!detectTileAt(x, y, tile, gridMap))
        {
            gridMap.SetTile(new Vector3Int(x, y), tile);
            return true;
        }
        return false;
    }
}
