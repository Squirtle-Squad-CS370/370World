using UnityEngine;
using System.Collections.Generic;

public class Chunk
{
    private const int width = 100;
    private const int height = 100;
    private Tile[,] tileData;
    private List<GameObject> objectData;
    private int objectCount;
    private int _x;
    private int _y;
    private bool isActive;
    private int _id;
    
    public Chunk(int x, int y, int id)
    {
        _x = x;
        _y = y;
        _id = id;
        tileData = new Tile[width, height];
        objectData = new List<GameObject>();
        objectCount = 0;
    }
    
    //Must be normalized to the chunk
    public void addTile(int x, int y, Tile tile)
    {
        tileData[x, y] = tile;
    }
    
    public void setTile(int x, int y, Tile tile)
    {
        tileData[x % 100, y % 100] = tile;
    }
    
    public Tile getTile(int x, int y)
    {
        return tileData[x, y];
    }
    
    public void setType(int x, int y, Tile.TileType type)
    {
        Tile t = tileData[x, y];
        t.Type = type;
        
        switch (type)
        {
            case Tile.TileType.Floor:
                t.obj.GetComponent<SpriteRenderer>().sprite = WorldController.Instance.floorSprite;
                t.isWalkable = true;
                break;
            case Tile.TileType.Grass:
                t.obj.GetComponent<SpriteRenderer>().sprite = WorldController.Instance.grassSprite;
                t.isWalkable = true;
                break;
            case Tile.TileType.Water:
                t.obj.GetComponent<SpriteRenderer>().sprite = WorldController.Instance.waterSprite;
                t.isWalkable = false;
                break;
            case Tile.TileType.Dirt:
                t.obj.GetComponent<SpriteRenderer>().sprite = WorldController.Instance.dirtSprite;
                t.isWalkable = true;
                break;
            case Tile.TileType.Sand:
                t.obj.GetComponent<SpriteRenderer>().sprite = WorldController.Instance.sandSprite;
                t.isWalkable = true;
                break;
            default:
                t.obj.GetComponent<SpriteRenderer>().sprite = null;
                t.isWalkable = true;
                break;
        }
        
        tileData[x, y] = t;
    }
    
    public void setZ(int x, int y, float z) 
    {
        tileData[x, y].setZ(z);
    }

    public void addObject(GameObject obj)
    {
        if (objectCount < 1000)
        {
            objectData.Add(obj);
            ++objectCount;
        }
    }

    public int x()
    {
        return _x;
    }

    public int y()
    {
        return _y;
    }
    
    public int w()
    {
        return width;
    }
    
    public int h()
    {
        return height;
    }

    public int id()
    {
        return _id;
    }
}