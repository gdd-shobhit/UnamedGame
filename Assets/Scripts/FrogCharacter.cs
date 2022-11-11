using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using System.Linq;

public class FrogCharacter : MonoBehaviour, IDamageable, IDataPersistence
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
    [SerializeField] private Camera camera;
    [SerializeField] float tongueLength = 1.0f; //how far away from the player can the tongue reach to grab things
    [SerializeField] float pullSpeed = 1.0f; //how quickly a grabbed object will be pulled to the player

    // Narrative
    public bool inDialog;

    //Shader/VFX
    private float noiseScale = 0.1f;
    private Material croakMat;
    private Material swordMat;
    private int dissolvePercent = 0;
    private int materializePercent = 0;

    // Death and Respawn
    public bool isDead = false;
    public float deathTime = 0;
    protected float reviveCooldown = 5f;
    private Vector3 respawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        level = 1;
        currentEnergy = 100;
        maxhealth = 100;
        maxEnergy = 100;
        attackDamage = 20;
        speed = gameObject.GetComponent<ThirdPersonController>().MoveSpeed;
        
        croakMat = weapon[2].GetComponent<Renderer>().material;
        swordMat = weapon[0].GetComponent<Renderer>().material;

        respawnPoint = transform.position;
    }

    public void LoadData(GameData data)
    {
        this.respawnPoint = data.respawnPoint;
        //this.currentHealth = data.currentHealth;
        this.currentHealth = 100;
        this.transform.position = respawnPoint;
    }

    public void SaveData(ref GameData data)
    {
        if (!isDead)
        {
            data.respawnPoint = this.respawnPoint;
            data.currentHealth = this.currentHealth;
        }
    }

    private void Update()
    {
        //Debug.Log(currentHealth);
        RegenerateEnergy();
        PComboDone();
        SheathWeapon();

        if (currentHealth <= 0 && !isDead)
        {
            Dead();
        }

        if (Time.time - deathTime > reviveCooldown && isDead)
        {
            Respawn();
        }

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
        if (GetComponent<StarterAssetsInputs>().hAttack)
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

        if (GetComponent<StarterAssetsInputs>().tongue)
        {
            Debug.Log("tongue pressed");
            TongueGrab();
            GetComponent<StarterAssetsInputs>().tongue = false;
        }

        //update material

        if (weapon[2].activeSelf && weapon[0].activeSelf)
        {
            swordMat.SetFloat("_CutoffHeight", swordMat.GetFloat("_CutoffHeight") - 0.01f);
        }
        //croakMat.SetFloat("_CutoffHeight", croakMat.GetFloat("_CutoffHeight") - 0.01f);
        else
        {
            if (weapon[2].activeSelf)
            {
                croakMat.SetFloat("_CutoffHeight", weapon[2].GetComponent<Transform>().position.y + 1);
            }

            if (weapon[0].activeSelf)
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

    public void Dead()
    {
        deathTime = Time.time;
        isDead = true;
        anim.SetBool("isDead", isDead);
    }

    public void Respawn()
    {
        isDead = false;
        anim.SetBool("isDead", isDead);
        DataPersistenceManager.instance.LoadGame();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            Debug.Log("checkpoint");

            respawnPoint = other.transform.position;
        }
    }

    void TongueGrab(){
        // a little yucky but it works
        // adding Vector3.up adjusts for the player object's anchor being on the floor, and adding the forward vector of the camera ensures we don't accidentally detect the shield or weapon objects
            // camera forward offset could be replaced by a layermask later for a more robust implementation
        Vector3 tonguePosStart = transform.position + Vector3.up + camera.transform.forward; 
        Vector3 tongueDirection = camera.transform.forward;
        tongueDirection.y = 0;
        Debug.Log("tongue");
        RaycastHit raycast = new RaycastHit();

        bool tongueHasHit = false;
        while (!tongueHasHit && tongueDirection.y < 0.5)
        {
            Debug.DrawLine(tonguePosStart, tonguePosStart + tongueDirection * tongueLength, Color.red, 1.0f);

            Physics.Raycast(tonguePosStart, tongueDirection, out raycast);
            if (raycast.collider)
            {
                IGrabbable g = raycast.collider.gameObject.GetComponent<IGrabbable>();
                if (g != null)
                {
                    tongueHasHit = true;
                    if (g.GetSwingable())
                    {
                        StartCoroutine(TongueSwing(raycast.point));
                    }
                    else {
                        Vector3 playerToEnemy = transform.position - raycast.collider.gameObject.transform.position;
                        Debug.DrawLine(transform.position, raycast.collider.gameObject.transform.position, Color.green, 1.0f);
                        StartCoroutine(g.Grab(transform, pullSpeed));
                    }
                }
            }
            tongueDirection.y += 0.05f;
        }
    }

    IEnumerator TongueSwing(Vector3 anchor)
    {
        //define a sphere whose center is at the anchor point of the tongue and radius is the distance between the anchor point and the player
        //snap the player to the surface of the sphere for the duration of the swing
        //x = (r * cos(s) * sin(t)) + anchor.x
        //z = (r * sin(s) * cos(t)) + anchor.z
        //y = (r * cos(t))          + anchor.y
        //where s = angle around the y axis between the player and the anchor
        //and
        //t = angle between y axis centered on the sphere and the player
        //alter the trajectory of the player by changing the values of s and t

        //find initial values of angles s and t
        float r = (anchor - transform.position).magnitude;
        float t = Mathf.Acos((transform.position.y - anchor.y) / r);
        float s = Mathf.Acos(((transform.position.x - anchor.x) / r) / Mathf.Sin(t));

        float timer = 0;

        while (timer < 10.0f)
        {
            timer += Time.deltaTime;
            t += 0.001f;
            Vector3 newPos = new Vector3();
            newPos.x = (r * Mathf.Cos(s) * Mathf.Sin(t)) + anchor.x;
            newPos.z = (r * Mathf.Sin(s) * Mathf.Cos(t)) + anchor.z;
            newPos.y = (r * Mathf.Cos(t)) + anchor.y;

            transform.position = newPos;
            yield return null;
        }
        
        yield return null;
    }
}
