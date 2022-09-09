using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogCharacter : MonoBehaviour, IDamageable
{
    public int currentHealth;
    public int attackDamage;
    public int currentStamina;               // FROGMINA IN GAME LOL
    public int speed;
    public int maxhealth;
    public int maxStamina;

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
        currentStamina = 100;
        maxhealth = 100;
        maxStamina = 100;
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

    public void Dash()
    {

    }
}
