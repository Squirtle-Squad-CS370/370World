using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    // "Instance" is a property used to ensure there is only one WC
    public static WorldController Instance { get; protected set; }
    public World world { get; protected set; }
    public Sprite floorSprite;
    public Sprite grassSprite;
    public Sprite waterSprite;
    public Sprite dirtSprite;
    
    public GameObject rockPrefab;
    public GameObject treePrefab;
    //public GameObject grassPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("WorldController - Another WorldController already exists.");
        }
        
        Instance = this;

        world = new World();

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

                // This fancy little complicated-looking thing is called a lambda.
                // Basically we are making up a function to link our callback parameters
                tile_data.RegisterTileTypeChangedCallback( (tile) => { OnTileTypeChanged(tile, tile_go); } );
            }
        }
        
        world.setPrefabs(rockPrefab, treePrefab);
        world.Generate();
    }

void OnTileTypeChanged(Tile tile_data, GameObject tile_go)
    {
        switch (tile_data.Type)
        {
            case Tile.TileType.Floor:
                tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
                break;
            case Tile.TileType.Grass:
                tile_go.GetComponent<SpriteRenderer>().sprite = grassSprite;
                break;
            case Tile.TileType.Water:
                tile_go.GetComponent<SpriteRenderer>().sprite = waterSprite;
                break;
            case Tile.TileType.Dirt:
                tile_go.GetComponent<SpriteRenderer>().sprite = dirtSprite;
                break;
            default:
                tile_go.GetComponent<SpriteRenderer>().sprite = null;
                break;
        }
    }
}
