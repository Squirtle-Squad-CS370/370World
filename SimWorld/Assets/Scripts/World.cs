using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Collections.Generic;
using static Noise;

//[System.Serializable]
public class World : MonoBehaviour// maintain monobehaviour inheritance for use of Start()
{
    // may consider switching to Tilemap in the future if environment gets detailed enough
    private List<Chunk> chunks;
    Chunk currentChunk;
    
    private GameObject rockPrefab;
    private GameObject treePrefab;
    private GameObject bushPrefab;
    private GameObject grassPrefab;
    private float scale = 2.5F;
    private int chunkCount = 0;
    private int seed = 0;
    private int animalCount = 0;

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
        
        chunks = new List<Chunk>();
        currentChunk = null;
        
        rockPrefab = WorldController.Instance.rockPrefab;
        treePrefab = WorldController.Instance.treePrefab;
        bushPrefab = WorldController.Instance.bushPrefab;
        grassPrefab = WorldController.Instance.grassPrefab;
    }

    private void populateChunk(Chunk chunk)
    {
        for (int x = 0; x < chunk.w(); x++)
        {
            for (int y = 0; y < chunk.h(); y++)
            {
                Tile t = new Tile(x, y);
                
                t.obj.name = "cid_" + chunk.id() + "_tile_" + x + "_" + y;
                t.obj.transform.position = new Vector3(t.X + (chunk.x() * 100), t.Y + (chunk.y() * 100), 0);
                // Clean up our heirarchy by making these tiles children
                t.obj.transform.SetParent(WorldController.Instance.transform, true);

                // Give them each a sprite renderer
                t.obj.AddComponent<SpriteRenderer>();
                
                SpriteRenderer renderer = t.obj.GetComponent<SpriteRenderer>();
                renderer.sortingOrder = -1000;
                
                chunk.addTile(x, y, t);
            }
        }
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

        for (int i = 0; i < width * height; ++i) 
        {
            Chunk c = chunks[i];
            if ((x >= c.x() && x <= c.x() + c.w()) && (y >= c.y() && y <= c.y() + c.h()))
            {
                return c.getTile(x % 100, y % 100);
            }
        }
        
        return null;
    }
    
    public int getAnimalCount()
    {
        return animalCount;
    }
    
    public void addAnimal()
    {
        ++animalCount;
    }
    
    public void removeAnimal()
    {
        if (animalCount > 0)
        {
            --animalCount;
        }
    }
    
    public void Generate() 
    {
        string path = Application.persistentDataPath + "/world.370";
        
        if (false && File.Exists(path)) 
        {
            load();
            return;
        } 
        else 
        {
            seed = System.DateTime.Now.Millisecond;
        }
        
        UnityEngine.Random.InitState(seed);

        //Create 4 stater chunks
        for (int x = 0; x < 2; ++x) 
        {
            for (int y = 0; y < 2; ++y)
            {
                Chunk c = new Chunk(x, y, chunkCount);
                chunks.Add(c);
                ++chunkCount;

                populateChunk(c);
                CreateGround(c, seed);
                PlaceObjects(c, seed);
            }
        }

        save();
    }

    public void save()
    {
        //TODO(Skyler): Make it so it can overwrite the file if it is there. So a new game can be started.
        string path = Application.persistentDataPath + "/world.370";
        Debug.Log(path);
        
        StreamWriter sw = new StreamWriter(path);
        sw.WriteLine(string.Format("{0}", seed));
        sw.WriteLine(string.Format("{0}", chunkCount));
        
        for (int i = 0; i < chunkCount; ++i)
        {
            Chunk c = chunks[i];
            int w = c.w();
            int h = c.h();
            
            sw.WriteLine(string.Format("{0},{1},{2}", c.id(), c.x(), c.y()));
            
            string chunkData = "";
            
            for (int y = 0; y < h; ++y)
            {
                for (int x = 0; x < w; ++x)
                {
                    chunkData += c.getTile(x, y).getZ() + ",";
                }
            }
            
            sw.WriteLine(chunkData);
        }
        
        sw.Close();
    }
    
    private void load()
    {
        //TODO(Skyler): Only load the needed chunks.
        
        string path = Application.persistentDataPath + "/world.370";
        
        if (File.Exists(path)) 
        {
            TextReader reader = File.OpenText(path);
            seed = int.Parse(reader.ReadLine());
            chunkCount = int.Parse(reader.ReadLine());
            
            for (int i = 0; i < chunkCount; ++i)
            {
                string cInfo = reader.ReadLine();
                int start = 0;
                int len = 1;
                
                while (cInfo[start + len] != ',')
                {
                    ++len;
                }
                
                int cId = int.Parse(cInfo.Substring(start, len));
                start = start + len + 1;
                len = 1;
                
                while (cInfo[start + len] != ',')
                {
                    ++len;
                }
                
                int cX = int.Parse(cInfo.Substring(start, len));
                start = start + len + 1;
                len = 1;
                
                while (start + len < cInfo.Length && cInfo[start + len] != ',')
                {
                    ++len;
                }
                
                int cY = int.Parse(cInfo.Substring(start, len));
                
                Chunk c = new Chunk(cX, cY, cId);
                chunks.Add(c);
                populateChunk(c);
                
                string cData = reader.ReadLine();
                string number = "";
                int index = 0;
                int x = 0;
                int y = 0;
                
                for (int j = 0; j < c.w() * c.h(); ++j)
                {
                    while (index < cData.Length && cData[index] != ',')
                    {
                        number += cData[index++];
                    }
                    
                    float z = float.Parse(number);
                    number = "";
                    ++index;
                    
                    setTileForVal(c, x, y, z);
                    
                    if (++x == 100)
                    {
                        x = 0;
                        ++y;
                    }
                }
            }
        } 
    }

    private void CreateGround(Chunk chunk, int seed)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xcoord = (((float)(x + (chunk.x() * 100)) / width) * scale) + seed;
                float ycoord = (((float)(y + (chunk.y() * 100)) / height) * scale) + seed;
                float val = Mathf.Clamp(Mathf.PerlinNoise(xcoord, ycoord) * 10, 0, 10);
                //float val = Noise.fbm(xcoord, ycoord) * 10;
                int cx = x % 100;
                int cy = y % 100;
                
                chunk.setZ(cx, cy, val);
                setTileForVal(chunk, cx, cy, val);
            }
        }
        
        //Debug.Log("Tiles Randomized");
    }
    
    private void PlaceObjects(Chunk chunk, int seed) 
    {
        float r = 5.0F;//UnityEngine.Random.Range(0, 10);
        int adjustedX = chunk.x() * 100;
        int adjustedY = chunk.y() * 100;
        
        for (int x = adjustedX; x < adjustedX + width; ++x) 
        {
            for (int y = adjustedY; y < adjustedY + height; ++y)
            {
                float xcoord = (((float)x / width) * scale) + seed;
                float ycoord = (((float)y / height) * scale) + seed;
                float pval = Mathf.PerlinNoise(xcoord, ycoord) * 10;
                float fval = Noise.fbm(xcoord, ycoord) * 10;
                
                if (isDirt(pval))
                {
                    if (UnityEngine.Random.Range(1, 20) == 4) 
                    {
                        GameObject rock = Instantiate(rockPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                        rock.transform.SetParent(WorldController.Instance.transform, true);
                        chunk.addObject(rock);
                    }
                } 
                else if (placeTree(pval, fval, r))
                {
                    GameObject t = Instantiate(treePrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                    t.transform.SetParent(WorldController.Instance.transform, true);
                    SpriteRenderer renderer = t.GetComponent<SpriteRenderer>();
                    renderer.sortingOrder = y + 1;
                    chunk.addObject(t);
                }
                else if (placeBush(pval, fval, r))
                {
                    GameObject bush = Instantiate(bushPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                    chunk.addObject(bush);
                    bush.transform.SetParent(WorldController.Instance.transform, true);
                }
                else if (placeGrass(pval, fval, r))
                {
                    GameObject grass = Instantiate(grassPrefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                    grass.transform.SetParent(WorldController.Instance.transform, true);
                    chunk.addObject(grass);
                }
            }
        }
    }
    
    private void setTileForVal(Chunk chunk, int x, int y, float val)
    {
        if (isDirt(val)) 
        {
            chunk.setType(x, y, Tile.TileType.Dirt);
        }
        else if (isGrass(val))
        {
            chunk.setType(x, y, Tile.TileType.Grass);
        }
        else if (isWater(val))
        {
            chunk.setType(x, y, Tile.TileType.Water);
        }
        else if (isSand(val))
        {
            chunk.setType(x, y, Tile.TileType.Sand);
        }
        else
        {
            chunk.setType(x, y, Tile.TileType.Floor);
            Debug.Log("val: " + val);
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
    
    //pval - perlin noise
    //fval - fractal brownian motion
    //r - range
    private bool placeTree(float pval, float fval, float r)
    {
        //return (((pval >= 5.7 && pval <= 6) || (pval >= 4 && pval <= 4.2) || (pval >= 2.5 && pval <= 3)) && (UnityEngine.Random.Range(1, 4) == 2));
        return ((fval >= r && fval <= (r + 0.5)) && isGrass(pval) && (UnityEngine.Random.Range(1, 4) == 2));
    }
    
    private bool placeBush(float pval, float fval, float r)
    {
        return ((pval >= r && pval <= (r + 0.5)) && isGrass(pval) && (UnityEngine.Random.Range(1, 14) == 3));
    }
    
    private bool placeGrass(float pval, float fval, float r)
    {
        return (!placeTree(pval, fval, r) && !placeBush(pval, fval, r) && isGrass(pval) && (UnityEngine.Random.Range(1, 15) == 12));
    }

    public bool TileHasWalkableNeighbor(Tile tile)
    {
        // For each surrounding tile
        for (int tileX = (tile.X - 1); tileX <= (tile.X + 1); tileX++)
        {
            for (int tileY = (tile.Y - 1); tileY <= (tile.Y + 1); tileY++)
            {
                // If we are not checking ourself and find a walkable tile
                return (GetTileAt(tileX, tileY) != tile && GetTileAt(tileX, tileY).isWalkable);
            }
        }
        
        // If none of them are walkable
        return false;
    }
}
