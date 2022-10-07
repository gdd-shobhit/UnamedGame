using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using System.Linq;

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
    [SerializeField] private Camera camera;
    public float tongueLength = 1.0f; //how far away from the player can the tongue reach to grab things

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
    }

    private void Update()
    {
        RegenerateEnergy();
        //if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("PAttack1"))
        //    anim.SetBool("PAttack1", false);
        //if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("PAttack2"))
        //    anim.SetBool("PAttack2", false);
        //if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("PAttack3"))
        //{
        //    anim.SetBool("PAttack3", false);
        //    noOfAttacks = 0;
        //}
        PComboDone();

        if (Time.time - lastAttackTime > maxComboDelay)
        {
            noOfAttacks = 0;
        }

        if (GetComponent<StarterAssetsInputs>().pAttack)
        {
            noOfAttacks++;
            PrimaryAttack();         
            GetComponent<StarterAssetsInputs>().pAttack = false;
        }
        if(GetComponent<StarterAssetsInputs>().hAttack)
        {
            HeavyAttack();
        
            GetComponent<StarterAssetsInputs>().hAttack = false;
        }
        if(GetComponent<StarterAssetsInputs>().tongue){
            TongueGrab();
            GetComponent<StarterAssetsInputs>().tongue = false;
        }
    }

    #region COMBAT_SYSTEM


    void CheckHit()
    {
        Collider[] hits = Physics.OverlapSphere(GameManager.instance.myFrog.weapon[0].transform.position, 0.5f);
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

    void TongueGrab(){
        // a little yucky but it works
        // adding Vector3.up adjusts for the player object's anchor being on the floor, and adding the forward vector of the camera ensures we don't accidentally detect the shield or weapon objects
            // camera forward offset could be replaced by a layermask later for a more robust implementation
        Vector3 tonguePosStart = transform.position + Vector3.up + camera.transform.forward; 
        Vector3 tongueDirection = camera.transform.forward;
        tongueDirection.y = 0;
        Debug.DrawLine(tonguePosStart, tonguePosStart + tongueDirection * tongueLength, Color.red, 1.0f);

        RaycastHit raycast = new RaycastHit();
        Physics.Raycast(tonguePosStart, tongueDirection, out raycast);
        if(raycast.collider) {
            IGrabbable g = raycast.collider.gameObject.GetComponent<IGrabbable>();
            if (g != null)
            {
                Vector3 playerToEnemy = transform.position - raycast.collider.gameObject.transform.position;
                Debug.DrawLine(transform.position, raycast.collider.gameObject.transform.position, Color.green, 1.0f);
                StartCoroutine(g.GrabbablePull(transform, 1.0f));
            }
        }
        
    }
}
