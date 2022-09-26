using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class FrogCharacter : MonoBehaviour, IDamageable
{
    public int currentHealth;
    public int attackDamage;
    public float currentEnergy;               // FROGMINA IN GAME LOL
    public float speed;
    public int maxhealth;
    public int maxEnergy;

    // stretch goals
    public int skillPoints;
    public int level;

    // Combat
    public Animator anim;
    public float cooldownTime = 1f;
    public float nextAttackTime = 0f;
    public int noOfAttacks = 0;
    public float lastAttackTime = 0;
    public float deltaTimeBetweenCombos = 1f;
    float maxComboDelay = 1;
    // probably switch to the frog son
    public FrogSon Son;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = 100;
        level = 1;
        currentEnergy = 100;
        maxhealth = 100;
        maxEnergy = 100;
        speed = gameObject.GetComponent<ThirdPersonController>().MoveSpeed;  
    }

    private void FixedUpdate()
    {
        RegenerateEnergy();
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("PAttack1"))
            anim.SetBool("PAttack1", false);
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("PAttack2"))
            anim.SetBool("PAttack2", false);
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("PAttack3"))
        {
            anim.SetBool("PAttack3", false);
            noOfAttacks = 0;
        }

        if (Time.time - lastAttackTime > maxComboDelay)
        {
            noOfAttacks = 0;
        }

        if (Time.time > nextAttackTime)
        {
            if (GetComponent<StarterAssetsInputs>().pAttack)
            {
                if(Time.time - lastAttackTime > deltaTimeBetweenCombos)
                {
                    noOfAttacks++;
                    PrimaryAttack();                 
                }
                GetComponent<StarterAssetsInputs>().pAttack = false;
            }
               
        }
    }

    public void PrimaryAttack()
    { 
        lastAttackTime = Time.time;
        if (noOfAttacks == 1)
        {
            // play PAttack1
            Debug.Log("PAttack");
            anim.SetBool("PAttack1", true);
        }
        // mechanic for saving Pattacks for combos down below

        noOfAttacks = Mathf.Clamp(noOfAttacks, 0, 3);

        if (noOfAttacks >= 2 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("PAttack1"))
        {
            anim.SetBool("PAttack1", false);
            anim.SetBool("PAttack2", true);
        }

        if (noOfAttacks >= 3 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("PAttack2"))
        {
            anim.SetBool("PAttack2", false);
            anim.SetBool("PAttack3", true);
        }
        if (noOfAttacks > 3)
            noOfAttacks = 0;
        //if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("PAttack3"))
        //{
        //    anim.SetBool("PAttack3", false);
        //    noOfAttacks = 0;
        //}
    }

    public void OnLevelUp()
    {
        level++;
        skillPoints++;
    }

    public void RegenerateEnergy()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += Time.deltaTime;
            GameManager.instance.hudUpdate = true;
        }
         
    }

    public bool Dash()
    {
        if (currentEnergy >= 20)
            return true;

        return false;
    }
}
