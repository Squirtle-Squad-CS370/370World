using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile
{
    int x;
    int y;
    private float z = 0.0F;
    //TODO: fix public
    public GameObject obj;

    public Installable installedObject = null;

    public enum TileType {Empty, Floor, Grass, Water, Dirt, Sand};
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
    public Tile(int x, int y)
    {
        this.x = x;
        this.y = y;
        obj = new GameObject();
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
        if (this.X == tile.X && (this.Y == tile.Y - 1 || this.Y == tile.Y + 1))
        {
            return true;
        }
        // If to the left or right:
        if (this.Y == tile.Y && (this.X == tile.X - 1 || this.X == tile.X + 1))
        {
            return true;
        }

        if (allowDiagonals)
        {
            if (this.X == tile.X + 1 && (this.Y == tile.Y - 1 || this.Y == tile.Y + 1))
            {
                return true;
            }
            if (this.X == tile.X - 1 && (this.Y == tile.Y - 1 || this.Y == tile.Y + 1))
            {
                return true;
            }
        }

        return false;
    }
    
    public float getZ()
    {
        return z;
    }
    
    public void setZ(float n)
    {
        z = n;
    }

    public void Install(Installable installable)
    {
        if( installedObject != null )
        {
            Debug.Log("Could not install" + installable.name + " because Tile (" + X + ", " + Y + ") already has " + installedObject.name + " installed.");
            return;
        }

        installable.OnInstall();
        installable.transform.position = obj.transform.position;
        installedObject = installable;
    }
    public void Uninstall()
    {
        if( installedObject == null )
        {
            Debug.Log("Could not Uninstall() from Tile (" + X + ", " + Y + ") because it has no object installed.");
            return;
        }

        installedObject.OnUninstall();
        installedObject = null;
    }
}
