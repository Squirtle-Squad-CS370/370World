using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    #region Singleton

    // "Instance" is a property used to ensure there is only one WC
    public static WorldController Instance { get; protected set; }

    // Awake is called even before Start(). It can be used for initializing self-contained
    // variables whereas Start is better for linking with other objects (as they may not yet
    // be initialized when Awake is called from this object).
    // For now we use it to enforce our singleton pattern.
    void Awake()
    {
        // Make sure this is the only instance of WorldController
        if (Instance != null)
        {
            Debug.LogError("WorldController - Another WorldController already exists.");
            return;
        }

        Instance = this;
    }

    #endregion

    private World world;
    public World World 
    { 
        get 
        {
            return world;
        }
        
        protected set {}
    }

    // This will keep track of tile data and GameObject pairs
    Dictionary<Tile, GameObject> tileGameObjectMap;

    public Sprite floorSprite;
    public Sprite grassSprite;
    public Sprite waterSprite;
    public Sprite dirtSprite;

    // Start is called before the first frame update
    void Start()
    {
        // Create a new world with empty tiles
        world = new World();

        // Instantiate our tile/GameObject dictionary
        tileGameObjectMap = new Dictionary<Tile, GameObject>();

        // Create a game object for each tile
        for (int x = 0; x < world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {
                Tile tile_data = world.GetTileAt(x, y);

                // create game object, name it according to position,
                // then move the object to correct position
                GameObject tile_go = new GameObject();
                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3( tile_data.X, tile_data.Y, 0 );
                // Clean up our heirarchy by making these tiles children
                tile_go.transform.SetParent(this.transform, true);

                // Give them each a sprite renderer
                tile_go.AddComponent<SpriteRenderer>();

                // Add the pair to our dictionary
                tileGameObjectMap.Add(tile_data, tile_go);
                
                // Register our callback so that the tile gets updated when its type changes
                tile_data.RegisterTileTypeChangedCallback( OnTileTypeChanged );

            }
        }
        
        world.Generate();
    }

    // This function returns the tile at a given world coordinate
    public Tile GetTileAtWorldCoord(Vector3 coord)
    {
        int x = Mathf.RoundToInt(coord.x);
        int y = Mathf.RoundToInt(coord.y);

        return WorldController.Instance.World.GetTileAt(x, y);
    }

    // This function will be automatically called whenever a tile's type gets changed
    void OnTileTypeChanged(Tile tile_data)
    {
        if( ! tileGameObjectMap.ContainsKey(tile_data) )
        {
            Debug.LogError("OnTileTypeChanged -  tileGameObjectMap does not contain tile_data");
            return;
        }

        GameObject tile_go = tileGameObjectMap[tile_data];

        if( tile_go == null )
        {
            Debug.LogError("OnTileTypeChanged - tileGameObjectMap's returned GameObject is null");
        }

        switch (tile_data.Type)
        {
            case Tile.TileType.Floor:
                tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
                tile_data.isWalkable = true;
                break;
            case Tile.TileType.Grass:
                tile_go.GetComponent<SpriteRenderer>().sprite = grassSprite;
                tile_data.isWalkable = true;
                break;
            case Tile.TileType.Water:
                tile_go.GetComponent<SpriteRenderer>().sprite = waterSprite;
                tile_data.isWalkable = false;
                break;
            case Tile.TileType.Dirt:
                tile_go.GetComponent<SpriteRenderer>().sprite = dirtSprite;
                tile_data.isWalkable = true;
                break;
            default:
                tile_go.GetComponent<SpriteRenderer>().sprite = null;
                tile_data.isWalkable = true;
                break;
        }
    }
}
