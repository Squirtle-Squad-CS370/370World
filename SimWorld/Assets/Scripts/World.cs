using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using static Noise;

public class World : MonoBehaviour// maintain monobehaviour inheritance for use of Start()
{
    // may consider switching to Tilemap in the future if environment gets detailed enough
    Tile[,] tileGrid;  // 2D array to hold our tiles
    //[SerializeField]
    //private Sprite rockSprite;
    private GameObject rock;
    private GameObject tree;
    private float scale = 2.5F;

    // The height and width variables are made properties with accessors
    private int width;  // width of the map, measured in tiles
    public int Width
    {
        get
        {
            return width;
        }
    }
    
    private int height; // height of the map, measured in tiles
    public int Height
    {
        get
        {
            return height;
        }
    }

    // Constructor
    public World(int width = 100, int height = 100) // default size rn is 100x100
    {
        this.width = width;
        this.height = height;

        // instantiate 2D array
        tileGrid = new Tile[width, height];

        // fill with Tile objects
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tileGrid[x, y] = new Tile(this, x, y);
            }
        }
        
        Debug.Log("World created with " + (width * height) + " tiles.");
    }
    
    public void setPrefabs(GameObject r, GameObject t) 
    {
        rock = r;
        tree = t;
    }

    public Tile GetTileAt(int x, int y)
    {
        // I'd like to maybe change this to have the map loop around east/west instead
        if (x < 0 || x > width)
        {
            Debug.LogError("Tile (" + x + ", " + y + ") is out of range.");
            return null;
        }
        
        return tileGrid[x, y];
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
                float val = Mathf.PerlinNoise(xcoord, ycoord) * 10;

                if (isDirt(val)) 
                {
                    tileGrid[x, y].Type = Tile.TileType.Dirt;
                }
                else if (isGrass(val))
                {
                    tileGrid[x, y].Type = Tile.TileType.Grass;
                }
                else if (isWater(val))
                {
                    tileGrid[x, y].Type = Tile.TileType.Water;
                }
                else
                {
                    tileGrid[x, y].Type = Tile.TileType.Floor;
                }
            }
        }
        Debug.Log("Tiles Randomized");
    }
    
    private void PlaceObjects(int seed) 
    {
        float r = 5.0F;//UnityEngine.Random.Range(0, 10);
        //Debug.Log(r);
        
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
                    Instantiate(tree, new Vector3(x, y, 0), Quaternion.identity);
                }
            }
        }
    }
    
    private bool isDirt(float val) 
    {
        return (val >= 0 && val <= 2);
    }
    
    private bool isGrass(float val) 
    {
        return (val > 2 && val <= 8);
    }
    
    private bool isWater(float val) 
    {
        return (val > 8);
    }
    
    private bool placeTree(float pval, float fval, float r)
    {
        //return (((pval >= 5.7 && pval <= 6) || (pval >= 4 && pval <= 4.2) || (pval >= 2.5 && pval <= 3)) && (UnityEngine.Random.Range(1, 4) == 2));
        return ((fval >= r && fval <= (r + 0.5)) && isGrass(pval) && (UnityEngine.Random.Range(1, 4) == 2));
    }
}
