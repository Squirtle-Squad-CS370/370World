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
    public bool isWalkable = true;

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
    // Unused currently but just in case ¯\_(ツ)_/¯
    public void UnregisterTiletypeChangedCallback(Action<Tile> callback)
    {
        TileTypeChanged -= callback;
    }

    // This function returns true if two tiles are adjacent.
    // The bool parameter indicates whether to check diagonals
    // as well as cardinal directions.
    public bool IsNeighbor(Tile tile, bool allowDiagonals = false)
    {
        // If directly above or below:
        if( this.X == tile.X && (this.Y == tile.Y-1 || this.Y == tile.Y+1) )
        {
            return true;
        }
        // If to the left or right:
        if( this.Y == tile.Y && (this.X == tile.X-1 || this.X == tile.X+1) )
        {
            return true;
        }
        
        if( allowDiagonals )
        {
            if (this.X == tile.X+1 && (this.Y == tile.Y-1 || this.Y == tile.Y+1))
            {
                return true;
            }
            if (this.X == tile.X-1 && (this.Y == tile.Y-1 || this.Y == tile.Y+1))
            {
                return true;
            }
        }

        return false;
    }

    // This function returns true if any of the surrounding tiles are walkable.
    public bool HasWalkableNeighbor()
    {
        // For each surrounding tile
        for( int tileX = (this.X-1); tileX <= (this.X+1); tileX++ )
        {
            for( int tileY = (this.Y-1); tileY <= (this.Y+1); tileY++ )
            {
                // If we are not checking ourself and find a walkable tile
                if( world.GetTileAt(tileX, tileY) != this && world.GetTileAt(tileX, tileY).isWalkable )
                {
                    return true;
                }
            }
        }
        // If none of them are walkable
        return false;
    }
}
