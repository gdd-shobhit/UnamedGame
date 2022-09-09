using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public List<Enemy> enemies;
    // Basic Singleton
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
