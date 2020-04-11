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

    public Sprite floorSprite;
    public Sprite grassSprite;
    public Sprite waterSprite;
    public Sprite dirtSprite;
    public Sprite sandSprite;
    
    public GameObject rockPrefab;
    public GameObject treePrefab;
    //public GameObject grassPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // Create a new world with empty tiles
        if (Instance != null)
        {
            Debug.LogError("WorldController - Another WorldController already exists.");
        }

        Instance = this;

        initWorld();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
    }

    void initWorld()
    {
        world = new World();
        //world = AddComponent<World>() as World;
        world.setPrefabs(rockPrefab, treePrefab);
        world.Generate();
    }

    public void saveWorld()
    {
        world.save();
    }
    
    // This function returns the tile at a given world coordinate
    public Tile GetTileAtWorldCoord(Vector3 coord)
    {
        int x = Mathf.RoundToInt(coord.x);
        int y = Mathf.RoundToInt(coord.y);

        return WorldController.Instance.World.GetTileAt(x, y);
    }
}
