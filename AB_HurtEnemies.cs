using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AB_HurtEnemies : MonoBehaviour
{
    public float Health = 100;
    public float maxHealth = 100.000f;
    public float minHealth = 0.001f;
    public float currHealth;

    private void Awake()
    {
        currHealth = Health;
        currHealth = Mathf.Clamp(currHealth, minHealth, maxHealth);
    }

    void Start()
    {

    }
    
    void Update()
    {
        
        //EnemyDeathCheck();
        if (Health <= minHealth)
        {
            //Debug.Log("Enemy " + gameObject.name + " has been destroyed!");

            if (gameObject.tag == "Turret")
            {
                Debug.Log("Turret " + gameObject.name + " has been destroyed!");
                IH_AudioManager.instance.Play("BlowTurret");
                gameObject.SetActive(false);
            }
            else if(gameObject.tag == "Enemy")
            {
                Debug.Log("Enemy " + gameObject.name + " has been destroyed!");
                gameObject.SetActive(false);
            }
        }
    }

    public void EnemyTakeDamage(float damage)
    {
        Health -= damage;
        currHealth -= damage;
        //Debug.LogError("Enemy " + gameObject.name + " took " + damage + " damage"); The bad code can't hurt us anymore...we are free!
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Shield"))
        {
            gameObject.SetActive(false);
        }
    }

    private void EnemyDeathCheck()
    {
        if (Health <= 0.01f)
        {
            Debug.Log("Enemy " + gameObject.name + " has been destroyed!");

            if (gameObject.tag == "Turret")
            {
                Debug.Log("Turret " + gameObject.name + " has been destroyed!");
                IH_AudioManager.instance.Play("BlowTurret");
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }*/
}
