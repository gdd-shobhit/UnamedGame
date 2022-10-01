using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    MoveTowards,
    MoveAway,
    Attack,
    Idle
}

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField]
    protected int health;
    public int level;
    public int attackDamage;
    const int damage = 10;
    public Animator anim;
    public GameObject player;
    public float lastGotHit = 0;
    public float getHitCooldown = 0.55f;

    // Player Tracking
    public float lookRadius = 10f;
    protected Transform target;
    protected NavMeshAgent agent;

    // Test
    protected float reviveCooldown = 2f;
    public float deathTime = 0;
    protected bool isDead = false;
    protected bool inAir = false;
    public ParticleSystem onHitVFX;
    
    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        if (GetComponent<Animator>() != null)
            anim = GetComponent<Animator>();
        attackDamage = level * damage;
        player = GameManager.instance.myFrog.gameObject;
        agent = GetComponent<NavMeshAgent>();
        target = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
            EnemyAI();
        if(anim.GetBool("Hit"))
            ResetHit();

        if (player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("AttackJump") && !inAir)
        {
            Debug.Log("here");
            inAir = true;
            GetComponent<Rigidbody>().AddForce(Vector3.up * 500);
        }
    }

    public void GetHit()
    {
        health -= player.GetComponent<FrogCharacter>().attackDamage;
    }

    public void GetHit(int attackDamage)
    {
        if(anim.GetBool("Hit") && Time.time - lastGotHit == 0f)
        {
            health -= attackDamage;
            anim.SetInteger("Health", health);
            onHitVFX.Play();       
        }
  
    }

    void ResetHit()
    {
        if(Time.time - lastGotHit > getHitCooldown)
            anim.SetBool("Hit", false);
    }

    protected virtual void EnemyAI()
    {
        float distance = Vector3.Distance(target.position, transform.position);
        if(distance <= lookRadius && !isDead)
        {
            agent.SetDestination(target.position);
        }

        if(health > 0)
            transform.LookAt(player.transform.position);
        else if(health <= 0 && !isDead)
        {
            deathTime = Time.time;
            isDead = true;
        }
            
        if (Time.time - deathTime > reviveCooldown && isDead)
        {
            health = 100;
            anim.SetInteger("Health", 100);
            isDead = false;

        }

    }

    private void OnTriggerExit(Collider other)
    {
        anim.SetBool("Hit", false);
    }

    // Display's Enemy's View Distance
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
