using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRespawnManager : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject[] turrets;
    public GameObject[] platforms;
    public GameObject[] pickups;
    private GameObject player;

    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        turrets = GameObject.FindGameObjectsWithTag("Turret");
        platforms = GameObject.FindGameObjectsWithTag("Platform");
        pickups = GameObject.FindGameObjectsWithTag("Pickup");
    }

    IEnumerator Respawn()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<AB_HurtEnemies>().Health = 100;
            enemies[i].SetActive(true);
            enemies[i].transform.position = enemies[i].GetComponent<EnemyPos>().currPos;
        }
        for (int i = 0; i < turrets.Length; i++)
        {
            turrets[i].GetComponent<AB_HurtEnemies>().Health = 100;
            turrets[i].SetActive(true);
        }
        yield return new WaitForSeconds(3.0f);
        for (int i = 0; i < platforms.Length; i++)
        {
            platforms[i].GetComponent<Rigidbody>().isKinematic = true;
            platforms[i].GetComponent<JJ_DissapearinPlatforms>().TriggerEnterAlready = false;
            platforms[i].GetComponent<JJ_DissapearinPlatforms>().hasTouched = false;
            platforms[i].transform.position = platforms[i].GetComponent<JJ_DissapearinPlatforms>().currPos;
            platforms[i].SetActive(true);
        }
        for (int i = 0; i < pickups.Length; i++)
        {
            pickups[i].SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<HealthSystem>().Health <= 0)
        {
            StartCoroutine(Respawn());
        }
    }
}
