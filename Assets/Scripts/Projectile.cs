using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.GetComponent<FrogCharacter>().currentHealth -= 10;
        GameManager.instance.hudUpdate = true;
        Destroy(gameObject);
    }
}
