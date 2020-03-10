using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using static Noise;

public class World : MonoBehaviour// maintain monobehaviour inheritance for use of Start()
{
    // may consider switching to Tilemap in the future if environment gets detailed enough
    Chunk[,] chunks;
    Chunk currentChunk;
    
    private GameObject rock;
    private GameObject tree;
    private float scale = 2.5F;

    // The height and width variables are made properties with accessors
    private int width;  // width of the map, measured in chunks
    public int Width
    {
        get
        {
            return width;
        }
    }
    
    private int height; // height of the map, measured in chunks
    public int Height
    {
        get
        {
            return height;
        }
    }

    // Constructor
    public World(int w = 100, int h = 100) // default size rn is 100x100
    {
        width = w;
        height = h;
        
        chunks = new Chunk[width, height];
        currentChunk = new Chunk(0, 0);
        chunks[0, 0] = currentChunk;

        // fill with Tile objects
        for (int x = 0; x < currentChunk.w(); x++)
        {
            for (int y = 0; y < currentChunk.h(); y++)
            {
                Tile t = new Tile(this, x, y);
                
                t.obj.name = "Tile_" + x + "_" + y;
                t.obj.transform.position = new Vector3(t.X, t.Y, 0);
                // Clean up our heirarchy by making these tiles children
                t.obj.transform.SetParent(WorldController.Instance.transform, true);

                // Give them each a sprite renderer
                t.obj.AddComponent<SpriteRenderer>();
                
                SpriteRenderer renderer = t.obj.GetComponent<SpriteRenderer>();
                renderer.sortingOrder = -1000;
                
                currentChunk.addTile(x, y, t);
            }
        }
        
        Debug.Log("Start chunk created with " + (currentChunk.w() * currentChunk.h()) + " tiles.");
    }
    
    public void setPrefabs(GameObject r, GameObject t) 
    {
        rock = r;
        tree = t;
    }
    
    public void setTile(int x, int y, Tile tile) 
    {
        currentChunk.setTile(x, y, tile);
    }
    
    public Chunk getCurrentChunk()
    {
        return currentChunk;
    }

    public Tile GetTileAt(int x, int y)
    {
        //Figure out what chunk the tile is in.
        int chunkX = (x / 100) - 1;
        int chunkY = (y / 100) - 1;
        
        chunkX = Mathf.Clamp(chunkX, 0, 100);
        chunkY = Mathf.Clamp(chunkY, 0, 100);
        
        Chunk c = chunks[chunkX, chunkY];
        
        return c.getTile(x % 100, y % 100);
    }
    
    public void Generate() 
    {
        int seed = System.DateTime.Now.Millisecond;
        
        UnityEngine.Random.InitState(seed);
        CreateGround(seed);
        PlaceObjects(seed);
    }

    private void CreateGround(int seed)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xcoord = (((float)x / width) * scale) + seed;
                float ycoord = (((float)y / height) * scale) + seed;
                float val = Mathf.Clamp(Mathf.PerlinNoise(xcoord, ycoord) * 10, 0, 10);
                //float val = Noise.fbm(xcoord, ycoord) * 10;
                int cx = x % 100;
                int cy = y % 100;
                
                currentChunk.setZ(cx, cy, val);

                if (isDirt(val)) 
                {
                    currentChunk.setType(cx, cy, Tile.TileType.Dirt);
                }
                else if (isGrass(val))
                {
                    currentChunk.setType(cx, cy, Tile.TileType.Grass);
                }
                else if (isWater(val))
                {
                    currentChunk.setType(cx, cy, Tile.TileType.Water);
                }
                else if (isSand(val))
                {
                    currentChunk.setType(cx, cy, Tile.TileType.Sand);
                }
                else if (isSand(val))
                {
                    tileGrid[x, y].Type = Tile.TileType.Sand;
                }
                else
                {
                    currentChunk.setType(cx, cy, Tile.TileType.Floor);
                    Debug.Log("val: " + val);
                }
            }
        }
        Debug.Log("Tiles Randomized");
    }
    
    private void PlaceObjects(int seed) 
    {
        float r = 5.0F;//UnityEngine.Random.Range(0, 10);
        
        for (int x = 0; x < width; ++x) 
        {
            for (int y = 0; y < height; ++y)
            {
                float xcoord = (((float)x / width) * scale) + seed;
                float ycoord = (((float)y / height) * scale) + seed;
                float pval = Mathf.PerlinNoise(xcoord, ycoord) * 10;
                float fval = Noise.fbm(xcoord, ycoord) * 10;
                
                if (isDirt(pval))
                {
                    if (UnityEngine.Random.Range(1, 20) == 4) 
                    {
                        Instantiate(rock, new Vector3(x, y, 0), Quaternion.identity);
                    }
                } 
                else if (placeTree(pval, fval, r))
                {
                    GameObject t = Instantiate(tree, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                    SpriteRenderer renderer = t.GetComponent<SpriteRenderer>();
                    renderer.sortingOrder = y - 1;
                }
            }
        }
    }
    
    private bool isDirt(float val) 
    {
        return (val >= 8);
    }
    
    private bool isGrass(float val) 
    {
        return (val > 2.25 && val < 8);
    }
    
    private bool isWater(float val) 
    {
        return (val >= 0 && val <= 2);
    }
    
    private bool isSand(float val)
    {
        return (val > 2 && val <= 2.25);
    }
    
    private bool placeTree(float pval, float fval, float r)
    {
        //return (((pval >= 5.7 && pval <= 6) || (pval >= 4 && pval <= 4.2) || (pval >= 2.5 && pval <= 3)) && (UnityEngine.Random.Range(1, 4) == 2));
        return ((fval >= r && fval <= (r + 0.5)) && isGrass(pval) && (UnityEngine.Random.Range(1, 4) == 2));
    }
}
