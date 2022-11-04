using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, IGrabbable
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

    // Test
    float reviveCooldown = 2f;
    public float deathTime = 0;
    bool isDead = false;
    bool inAir = false;
    public ParticleSystem onHitVFX;

    private Rigidbody rigidbody;
    
    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        if (GetComponent<Animator>() != null)
            anim = GetComponent<Animator>();
        attackDamage = level * damage;
        player = GameManager.instance.myFrog.gameObject;
        rigidbody = GetComponent<Rigidbody>();
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
        if(health > 0)
            transform.LookAt(player.transform.position);
        else if(health <= 0 && !isDead)
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

    private void OnTriggerExit(Collider other)
    {
        anim.SetBool("Hit", false);
    }

    public IEnumerator Grab(Transform t_player, float pullSpeed)
    {
        rigidbody.isKinematic = false;
        //perform linear interpolation
        Vector3 origin = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 destination;
        float pullTime = (t_player.position - transform.position).sqrMagnitude / pullSpeed;
        float timer = 0;
        while(timer < pullTime){
            //destination and pullTime need to be updated each frame to account for player movement during the pull
            destination = t_player.position + ((transform.position - t_player.position).normalized);
            pullTime = (t_player.position - transform.position).sqrMagnitude / pullSpeed;

            transform.position = Vector3.Lerp(origin, destination, timer/pullTime);

            timer += Time.deltaTime;
            yield return null;
        }
        rigidbody.isKinematic = true;
    }

    public bool GetSwingable() { return false; }
}
