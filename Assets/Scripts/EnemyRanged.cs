using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : Enemy
{
    private EnemyState enemyState;
    [SerializeField] private float idealRange;
    [SerializeField] private float rangeTolerance;
    protected override void EnemyAI()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance > lookRadius || isDead)
            enemyState = EnemyState.Idle;

        else if(distance > idealRange + rangeTolerance)
            enemyState = EnemyState.MoveTowards;
        
        else if(distance < idealRange + rangeTolerance)
            enemyState = EnemyState.MoveAway;
        
        else
            enemyState = EnemyState.Attack;
        
        switch(enemyState)
        {
            case EnemyState.MoveTowards:
                agent.SetDestination(target.position);
                break;

            case EnemyState.MoveAway:
                agent.SetDestination(transform.position - target.position);
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
}
