using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private GameObject bulletPrefab;
    
    private float raidTimer = 0f;
    private float doRaidChance = 2f;
    private bool canRaid = false;
    private int numEnemies = 5;
    private int numEnemiesRange = 2;
    
    void Start()
    {
        //The first one will take longer then the others.
        //TODO(Skyler): Have the time and numEnemies based on the dificulty.
        raidTimer = 5;//UnityEngine.Random.Range(420, 450); //just blaze
    }

    void Update()
    {
        
    }
    
    void FixedUpdate()
    {
        raidTimer -= Time.deltaTime;
        
        if (raidTimer <= 0)
        {
            raidTimer = 5;//UnityEngine.Random.Range(350, 420); //blaze it
            if ((int)UnityEngine.Random.Range(1, 3) == 2)
            {
                doRaid();
            }
        }
    }
    
    private void doRaid()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        float dir = UnityEngine.Random.Range(1, 360);
        float dist = UnityEngine.Random.Range(20, 30);
        int neg = 1;
        if ((int)UnityEngine.Random.Range(1, 3) == 2) 
        {
            neg = -1;
        }
        
        numEnemies += (int)UnityEngine.Random.Range(0, numEnemiesRange) * neg;
        
        for (int i = 0; i < numEnemies; ++i)
        {
            float a = dist * (float)Math.Sin((Math.PI / 180) * dir) + UnityEngine.Random.Range(3, 5);
            float b = (float)Math.Sqrt(Math.Pow(dist, 2) - Math.Pow(a, 2)) + UnityEngine.Random.Range(3, 5);
            Vector3 spawnPos = player.transform.position + new Vector3(a, b, 0) * neg;
            
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }
}
