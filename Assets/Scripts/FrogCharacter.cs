using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using System.Linq;

public class FrogCharacter : MonoBehaviour, IDamageable
{
    [SerializeField]
    int sheathTime = 2;
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
    public bool isAttacking = false;
    public float cooldownTime = 1f;
    public float nextAttackTime = 0f;
    public int noOfAttacks = 0;
    public float lastAttackTime = 0;
    public float deltaTimeBetweenCombos = 1f;
    public List<GameObject> weapon;
    float maxComboDelay = 0.55f;
    // probably switch to the frog son
    public FrogSon Son;

    // Narrative
    public bool inDialog;

    //Shader/VFX
    private float noiseScale = 50.0f;
    private Material croakMat;
    private Material swordMat;
    private int dissolvePercent = 0;
    private int materializePercent = 0;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = 100;
        level = 1;
        currentEnergy = 100;
        maxhealth = 100;
        maxEnergy = 100;
        attackDamage = 20;
        speed = gameObject.GetComponent<ThirdPersonController>().MoveSpeed;
        
        croakMat = weapon[2].GetComponent<Renderer>().material;
        swordMat = weapon[0].GetComponent<Renderer>().material;
    }

    private void Update()
    {
        RegenerateEnergy();
        PComboDone();
        SheathWeapon();

        if (Time.time - lastAttackTime > maxComboDelay)
        {
            noOfAttacks = 0;
        }

        // If Player is in Dialog Sequence disable combat controls until finished
        if (inDialog)
        {
            return;
        }

        if (GetComponent<StarterAssetsInputs>().pAttack)
        {
            if (!weapon[0].activeSelf)
            {
                weapon[0].SetActive(true);
                weapon[2].SetActive(false);
            }
                
            noOfAttacks++;
            PrimaryAttack();         
            GetComponent<StarterAssetsInputs>().pAttack = false;
        }
        if(GetComponent<StarterAssetsInputs>().hAttack)
        {
            // Sheath/Unsheath
            if (!weapon[0].activeSelf)
            {
                weapon[0].SetActive(true);
                weapon[2].SetActive(false);
            }

            HeavyAttack();
        
            GetComponent<StarterAssetsInputs>().hAttack = false;
        }

        //update material

        if(weapon[2].activeSelf && weapon[0].activeSelf)
        {
            swordMat.SetFloat("_CutoffHeight", swordMat.GetFloat("_CutoffHeight") - 0.01f);
            croakMat.SetFloat("_CutoffHeight", croakMat.GetFloat("_CutoffHeight") - 0.01f);
        }
        else
        {
            if (weapon[2].activeSelf)
            {
                croakMat.SetFloat("_CutoffHeight", weapon[2].GetComponent<Transform>().position.y + 1);
            }
        
            if(weapon[0].activeSelf)
            {
                swordMat.SetFloat("_CutoffHeight", weapon[0].GetComponent<Transform>().position.y + 1);
            }
        }
    }

    #region COMBAT_SYSTEM


    void CheckHit()
    {
        Collider[] hits = Physics.OverlapSphere(weapon[0].transform.position, 0.5f);
        //Collider[] hits2 = Physics.OverlapSphere(GameManager.instance.myFrog.weapon[1].transform.position, 0.5f);
        //hits = hits.Concat(hits2).ToArray();
        foreach (Collider hit in hits)
        {
            if (hit.tag == "Enemy")
            {
                hit.gameObject.GetComponent<Animator>().SetBool("Hit", true);
                hit.gameObject.GetComponent<Enemy>().lastGotHit = Time.time;
                hit.gameObject.GetComponent<Enemy>().GetHit(attackDamage);
            }
        }
    }

    public void HeavyAttack()
    {
        if (noOfAttacks >= 2 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && GetComponent<StarterAssetsInputs>().hAttack
           && anim.GetCurrentAnimatorStateInfo(0).IsName("PAttack2"))
        {
            anim.SetBool("PAttack2", false);
            anim.SetBool("HAttackC", true);
            CheckHit();
        }
        else
        {
            anim.SetBool("HAttack", true);
            CheckHit();
        }
    }

    public void PrimaryAttack()
    {
        lastAttackTime = Time.time;
        if (noOfAttacks == 1)
        {
            // play PAttack1
            anim.SetBool("PAttack1", true);
            CheckHit();
        }
        // mechanic for saving Pattacks for combos down below

        noOfAttacks = Mathf.Clamp(noOfAttacks, 0, 3);

        PrimaryAttack2();
        PrimaryAttack3();

    }

    public void PrimaryAttack2()
    {
        if (noOfAttacks >= 2 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("PAttack1"))
        {
            anim.SetBool("PAttack1", false);
            anim.SetBool("PAttack2", true);
            CheckHit();
        }
    }

    public void PrimaryAttack3()
    {
       
        if (noOfAttacks >= 3 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("PAttack2"))
        {
            anim.SetBool("PAttack2", false);
            anim.SetBool("PAttack3", true);
            CheckHit();
        }
    }

    public void PComboDone()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
        {
            anim.SetBool("PAttack3", false);
            anim.SetBool("PAttack2", false);
            anim.SetBool("PAttack1", false);
            anim.SetBool("HAttackC", false);
            anim.SetBool("HAttack", false);
            noOfAttacks = 0;
        }
    }
    #endregion

    public void OnLevelUp()
    {
        // Designers change the stats
        level++;
        skillPoints++;
        attackDamage += 2;
        maxEnergy += 2;
        maxhealth += 2;
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

    /// <summary>
    /// Sheath/Unsheath depending on last time attacked
    /// </summary>
    public void SheathWeapon()
    {
        if(Time.time - lastAttackTime > sheathTime)
        {
            if(Time.time - lastAttackTime >= sheathTime + 0.5)
            {
                weapon[0].SetActive(false);
            }
            weapon[2].SetActive(true);
        }
    }
}
