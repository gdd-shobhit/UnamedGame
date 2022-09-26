using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public int health;
    public int level;
    public int attackDamage;
    const int damage = 10;
    public Animator anim;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        if (GetComponent<Animator>() != null)
            anim = GetComponent<Animator>();
        attackDamage = level * damage;
        player = GameManager.instance.myFrog.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(anim.GetBool("GetHit") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
        {
         
        }

        if(health > 0) 
            EnemyAI();
    }

    void EnemyAI()
    {
        transform.LookAt(player.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {

            anim.SetBool("GetHit", false);
            anim.SetBool("GetHit", true);
            health -= 20;
            anim.SetInteger("Health", health);
    }
}
