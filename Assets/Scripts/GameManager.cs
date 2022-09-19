using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Slider healthSlider;
    public Slider energySlider;
    public FrogCharacter myFrog;
    public FrogSon mySon;
    public bool hudUpdate = false;
    // Basic Singleton
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    private void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        if (hudUpdate)
            UpdateHUD();
    }

    /// <summary>
    /// Updates HUD when told
    /// </summary>
    private void UpdateHUD()
    {
        healthSlider.value = myFrog.currentHealth;
        energySlider.value = myFrog.currentEnergy;
        hudUpdate = !hudUpdate;
    }

}
