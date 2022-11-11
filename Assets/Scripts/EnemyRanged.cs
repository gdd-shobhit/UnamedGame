using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : Enemy
{
    private EnemyState enemyState;
    [SerializeField] private float idealRange;
    [SerializeField] private float rangeTolerance;
    [SerializeField] GameObject projectile;
    public float attackInterval = 2.0f;
    public float projectileSpeed = 700f;
    float attackCooldown = 0f;

    protected override void EnemyAI()
    {

        anim.SetInteger("MovementState", (int)enemyState);


        anim.SetFloat("Speed", agent.speed);

        float distance = Vector3.Distance(target.position, transform.position);

        if (distance > lookRadius || isDead)
        {
            enemyState = EnemyState.Idle;
        }  

        else if(distance > idealRange + rangeTolerance)
        {
            enemyState = EnemyState.MoveTowards;
        }   
        
        else if(distance < idealRange - rangeTolerance)
        {
            enemyState = EnemyState.MoveAway;
        }   
        
        else
        {
            enemyState = EnemyState.Attack;
        }
            
        

        switch(enemyState)
        {
            case EnemyState.MoveTowards:
                agent.SetDestination(target.position);
                break;

            case EnemyState.MoveAway:
                agent.SetDestination(transform.position + (transform.position - target.position));
                
                break;

            case EnemyState.Attack:
                agent.SetDestination(transform.position);
                if (attackCooldown <= 0.0f)
                {
                    Attack();
                    attackCooldown = attackInterval;
                }
                else
                {
                    attackCooldown -= Time.deltaTime;
                }
                break;
        }


        if (enemyState != EnemyState.Idle)
            transform.LookAt(player.transform.position);

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
    }

    void Attack()
    {
        GameObject spiderAttack = Instantiate(projectile, 
            transform.position
            + new Vector3(0.0f, 1.1f, 0.0f) //move up so it doesn't originate in the floor
            + (target.transform.position - transform.position).normalized, // move towards target so it doesn't collide with enemy
            transform.rotation);
        anim.SetTrigger("Attack");

        spiderAttack.GetComponent<Rigidbody>().AddForce((target.transform.position - transform.position).normalized * 500.0f);
    }

    public override void GetHit(int attackDamage)
    {
        anim.SetTrigger("Hit");
        if (Time.time - lastGotHit == 0f)
        {
            health -= attackDamage;
            anim.SetInteger("Health", health);
            onHitVFX.Play();
        }
    }
}
