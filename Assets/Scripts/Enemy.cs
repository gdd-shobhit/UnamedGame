using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    public int level;
    public int attackDamage;
    const int damage = 10;

    // Start is called before the first frame update
    void Start()
    {
        attackDamage = level * damage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
