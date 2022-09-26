using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class ComboSystem : MonoBehaviour
{
    // Combat
    public Animator anim;
    public float cooldownTime = 1f;
    public float nextAttackTime = 0f;
    public int noOfAttacks = 0;
    public float lastAttackTime = 0;
    public float deltaTimeBetweenCombos = 1f;
    float maxComboDelay = 2f;
    public bool isPAttacking1 = false;
    public bool isPAttacking2 = false;
    public bool isPAttacking3 = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastAttackTime > maxComboDelay)
        {
            noOfAttacks = 0;
            anim.SetBool("PAttack2", false);
            anim.SetBool("PAttack3", false);
            anim.SetBool("PAttack1", false);
        }

        //if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime == 1f)
        //{
        //    anim.SetBool("PAttack2", false);
        //    anim.SetBool("PAttack3", false);
        //    anim.SetBool("PAttack1", false);
        //    noOfAttacks = 0;
        //}

        if (GetComponent<StarterAssetsInputs>().pAttack)
        {
            
            noOfAttacks++;
            noOfAttacks = Mathf.Clamp(noOfAttacks, 0, 3);
            StartCoroutine(PAttackCoroutine(3.5f));
            GetComponent<StarterAssetsInputs>().pAttack = false;

        }
    }

    IEnumerator PAttackCoroutine(float time)
    {
        anim.SetBool("PAttack1", true);
        yield return new WaitForSeconds(time);
        if(noOfAttacks >= 2)
        {
            anim.SetBool("PAttack2", true);
            StartCoroutine(PAttack2Coroutine(1f));
        }
    }


    IEnumerator PAttack2Coroutine(float time)
    {
        anim.SetBool("PAttack2", true);
        yield return new WaitForSeconds(time);
        if (noOfAttacks >= 3)
        {
            StartCoroutine(PAttack3Coroutine(1f));
        }
    }


    IEnumerator PAttack3Coroutine(float time)
    {
        anim.SetBool("PAttack3", true);
        yield return new WaitForSeconds(time);
        anim.SetBool("PAttack2", false);
        anim.SetBool("PAttack3", false);
        anim.SetBool("PAttack1", false);
        noOfAttacks = 0;
    }

    void TurnOffAnimBool(List<string> animBools)
    {
        foreach(string animBool in animBools)
            anim.SetBool(animBool,  false);
    }

  
}
