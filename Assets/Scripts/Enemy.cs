using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField]
    int health;
    public int level;
    public int attackDamage;
    const int damage = 10;
    public Animator anim;
    public GameObject player;
    public float lastGotHit = 0;
    public float getHitCooldown = 0.55f;
    public Slider healthSlider;

    // Player Tracking
    public float lookRadius = 10f;
    Transform target;
    NavMeshAgent agent;

    public LayerMask whatIsGround, whatIsPlayer;
    public Vector3 walkPoint;
    bool walkPointSet;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // Test
    float reviveCooldown = 2f;
    public float deathTime = 0;
    bool isDead = false;
    bool inAir = false;
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
        timeBetweenAttacks = 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        HUDUpdate();

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

    private void HUDUpdate()
    {
        healthSlider.value = 100 - health;
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

    void EnemyAI()
    {
        anim.SetFloat("Speed", agent.speed);

        if (health <= 0 && !isDead)
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
        // Depending on the distance of the player and the enemy view distance
        // The enemy will enter a different state
        float distance = Vector3.Distance(target.position, transform.position);
        if (distance > lookRadius && !isDead) Patrolling();
        if (distance <= lookRadius && !isDead) ChasePlayer();
        if (distance <= agent.stoppingDistance + 1 && !isDead) AttackPlayer();

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

    private void Patrolling()
    {
        // Goes to the walkpoint set
        // Makes it look as though the enemy is aimlessly walking around
        agent.speed = 2;
        if (!walkPointSet) SearchWalkPoint();
        agent.SetDestination(walkPoint);



        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        //Debug.Log(distanceToWalkPoint.magnitude);
        // Walkpoint reached
        if (distanceToWalkPoint.magnitude < 3f)
        {
            //gameObject.GetComponent<NavMeshAgent>().isStopped = true;

            walkPointSet = false;
        }

    }

    private void SearchWalkPoint()
    {
        //Makes a random walk pont for the enemy to go to
        float randomZ = Random.Range(-1, 28);
        float randomX = Random.Range(-11, 20);

        walkPoint = new Vector3(randomX, transform.position.y, randomZ);
        
        // Sets it so it is known that the enemy has a walkpoint
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        // Sets the enemy to move towards the player
        agent.speed = 3;
        gameObject.GetComponent<NavMeshAgent>().isStopped = false;
        agent.SetDestination(target.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        if(!isDead)
        transform.LookAt(target);

        agent.speed = 0;
        gameObject.GetComponent<NavMeshAgent>().isStopped = true;

        if (!alreadyAttacked)
        {
            // Attack code
            anim.SetBool("Attack", true);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        anim.SetBool("Attack", false);
    }
}
