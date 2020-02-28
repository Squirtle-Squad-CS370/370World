using System;
using UnityEngine;


public class World // maintain monobehaviour inheritance for use of Start()
{
    // may consider switching to Tilemap in the future if environment gets detailed enough
    Tile[,] tileGrid;  // 2D array to hold our tiles

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



    /*
     * // This could be used to only create tiles when they come in sight of the player
     * // for now, we are creating them all at once at the start of the game
    public Tile GetTileAt(int x, int y)
    {
        if (tileGrid[x, y] == null)
            tileGrid[x, y] = new Tile(this, x, y);
        return tileGrid[x, y];
    }
    */

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

    public void RandomizeTiles()
    {
        for( int x = 0; x < width; x++ )
        {
            for( int y = 0; y < height; y++ )
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                    tileGrid[x, y].Type = Tile.TileType.Empty;
                else
                    tileGrid[x, y].Type = Tile.TileType.Floor;
            }
        }
        Debug.Log("Tiles Randomized");
    }

}

