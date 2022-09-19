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

    // probably switch to the frog son
    public FrogSon Son;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = 100;
        level = 1;
        currentEnergy = 100;
        maxhealth = 100;
        maxEnergy = 100;
        speed = gameObject.GetComponent<ThirdPersonController>().MoveSpeed;
    }

    private void Update()
    {
        RegenerateEnergy();
    }

    public void GetHit()
    {
        Debug.Log("got hit");
    }

    public void PrimaryAttack()
    {
        Debug.Log("PAttack");

        // mechanic for saving Pattacks for combos down below
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
