using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class StoryTeller : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private GameObject rabbitPrefab;
    [SerializeField]
    private GameObject pigPrefab;
    
    private float doRaidChance = 0.75f; //75% chance
    private bool firstRaid = true;
    private bool firstSpawn = true;
    private int numEnemies = 5;
    private int numEnemiesRange = 2;
    [SerializeField]
    private World world;
    private GameObject player;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
        StartCoroutine(doRaid());
        StartCoroutine(spawnAnimal());
    }
    
    private Vector3 getSpawnPos()
    {
        bool isNegative = (UnityEngine.Random.value <= 0.5f);
        float dir = UnityEngine.Random.Range(1, 360);
        float dist = UnityEngine.Random.Range(20, 30);
        float a = dist * (float)Math.Sin((Math.PI / 180) * dir) + UnityEngine.Random.Range(3, 5);
        float b = (float)Math.Sqrt(Math.Abs(Math.Pow(dist, 2) - Math.Pow(a, 2))) + UnityEngine.Random.Range(3, 5);
        
        //For some reason this will only do 180 degrees around the player. So the isNegative is to make it a full circle.
        return player.transform.position + new Vector3(a, b, 0) * (isNegative ? -1 : 1);
    }
    
    IEnumerator doRaid()
    {
        while (true)
        {
            if (!firstRaid && UnityEngine.Random.value <= doRaidChance)
            {
                bool isNegative = (UnityEngine.Random.value <= 0.5f);
                
                numEnemies += (int)UnityEngine.Random.Range(0, numEnemiesRange) * (isNegative ? -1 : 1);
                
                for (int i = 0; i < numEnemies; ++i)
                {
                    Instantiate(enemyPrefab, getSpawnPos(), Quaternion.identity);
                }
            }
            else
            {
                firstRaid = false;
            }
            
            //TODO(Skyler): Have the time and numEnemies based on the dificulty.
            yield return new WaitForSeconds(UnityEngine.Random.Range(4 * 60, 6 * 60)); //Wait 4 to 6 minutes
        }
    }
    
    IEnumerator spawnAnimal() 
    {
        while (true)
        {
            if (!firstSpawn && world.getAnimalCount() < 20)
            {
                doSpawnAnimal();
            }
            
            if (firstSpawn)
            {
                firstSpawn = false;
                for (int i = 0; i < 5; ++i)
                {
                    doSpawnAnimal();
                }
            }
        
            yield return new WaitForSeconds(UnityEngine.Random.Range(60, 2 * 60)); //Wait 1 to 2 minutes
        }
    }
    
    private void doSpawnAnimal()
    {
        int val = UnityEngine.Random.Range(0, 10);
                
        if (val <= 5)
        {
            GameObject r = Instantiate(rabbitPrefab, getSpawnPos(), Quaternion.identity) as GameObject;
            r.transform.SetParent(WorldController.Instance.transform, true);
        }
        else 
        {
            GameObject p = Instantiate(pigPrefab, getSpawnPos(), Quaternion.identity) as GameObject;
            p.transform.SetParent(WorldController.Instance.transform, true);
        }
        
        world.addAnimal();
    }
}
