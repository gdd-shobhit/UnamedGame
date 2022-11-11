using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [SerializeField]
    float findingRadius = 10f;
    // Start is called before the first frame update
    void Start()
    {
        health = 500;
        healthSlider.maxValue = 500;
        healthSlider.value = 0;
        anim = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        FindPlayer();
    }

    void FindPlayer()
    {
        if(Vector3.Distance(transform.position,GameManager.instance.myFrog.transform.position) < findingRadius)
            agent.SetDestination(GameManager.instance.myFrog.transform.position);

        if(Vector3.Distance(transform.position, GameManager.instance.myFrog.transform.position) < 1)
        {
            // Play Animation and attack
        }
    }
}
