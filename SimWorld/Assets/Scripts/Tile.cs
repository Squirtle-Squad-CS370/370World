using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    World world;
    int x;
    int y;

    public enum TileType {Empty, Floor, Grass, Water, Dirt};
    private TileType type = TileType.Empty;

    // Callback function for when tile changes type to inform other code
    public event Action<Tile> TileTypeChanged;

    // Accessors & mutators for Tile properties
    public TileType Type
    {
        get
        {
            return type;
        }
        set
        {
            type = value;
            // Since tileType has changed, update its sprite & other things
            // Use the "callback" function to tell stuff we changed
            if (TileTypeChanged != null) 
            {
                TileTypeChanged(this);
            }
        }
    }
    public int X
    {
        get
        {
            return x;
        }
    }
    public int Y
    {
        get
        {
            return y;
        }
    }


    // Constructor
    public Tile(World w, int x, int y)
    {
        world = w;
        this.x = x;
        this.y = y;
    }

    // this function will be called with a function as its parameter
    // which will then be called whenever our TileType changes! (:
    public void RegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        TileTypeChanged += callback;
        // the use of += means we can call a whole list of functions! very cool
    }
}
