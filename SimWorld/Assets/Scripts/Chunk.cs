using UnityEngine;

public class Chunk
{
    private const int width = 100;
    private const int height = 100;
    private Tile[,] tileData;
    private int x;
    private int y;
    private bool isActive;
    
    public Chunk(int sx, int sy)
    {
        x = sx;
        y = sy;
        tileData = new Tile[width, height];
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
    
    public int w()
    {
        return width;
    }
    
    public int h()
    {
        return height;
    }
}