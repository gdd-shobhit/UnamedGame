using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    // Player Tracking
    public float lookRadius = 10f;
    Transform target;
    NavMeshAgent agent;

    public LayerMask whatIsGround, whatIsPlayer;
    public Vector3 walkPoint;
    bool walkPointSet;
    float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // States
    float sightRange, attackRange;
    bool playerInSightRange, playerInAttackRange;

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
        walkPointRange = 10;
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

    void EnemyAI()
    {

        if(health <= 0 && !isDead)
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

        playerInSightRange = Physics.CheckSphere(transform.position, lookRadius, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        //Debug.Log(playerInSightRange);
        float distance = Vector3.Distance(target.position, transform.position);
        if (distance > lookRadius && !isDead) Patrolling();
        if (distance <= lookRadius && !isDead) ChasePlayer();
        //if (playerInAttackRange && playerInSightRange) AttackPlayer();

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
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        //Debug.Log(distanceToWalkPoint.magnitude);
        // Walkpoint reached
        if (distanceToWalkPoint.magnitude < 3f)
        {
            //Debug.Log("made it to point");
            walkPointSet = false;
        }

        //Debug.Log("Patroling");

    }

    private void SearchWalkPoint()
    {
        //Debug.Log("making walkpoint");
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        //Debug.Log("z = " + randomZ);
        //Debug.Log("x = " + randomX);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
            //Debug.Log("walk point set");
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(target.position);

        //Debug.Log("chasing");
    }

    private void AttackPlayer()
    {
        Debug.Log("attacking");
        agent.SetDestination(transform.position);

        transform.LookAt(target);

        if (!alreadyAttacked)
        {

            // Attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}
