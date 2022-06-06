using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthSystem : MonoBehaviour
{
    public Slider HealthBar;
    public float Health = 100;

    private float currenthealth;

    private PlayerController tp;
    private JJ_Blink blk;
    private bool hasPlayed;

    public Vector3 lastCheckpoint;

    private float accelDefault;
    private float jumpDefault;
    private float zero = 0;

    //bool statsReset;

    //Adrian Betancourt
    public bool canTakeDamage; //Needed for immortality functionality, will allow for toggling if damage is allowable

    private void Awake()
    {
        currenthealth = Health;
        currenthealth = Mathf.Clamp(currenthealth, 0.001f, 100.0f);
        Health = Mathf.Clamp(Health, 0.001f, 100.0f);
        canTakeDamage = true; //Sets to true on awake, to make sure default functionality can occur and damage able to be applied correctly unless altered.
    }

    // Start is called before the first frame update
    void Start()
    {
        tp = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        blk = GameObject.FindGameObjectWithTag("Player").GetComponent<JJ_Blink>();
        accelDefault = tp.acceleration;
        jumpDefault = tp.jumpSpeed;
        //statsReset = false;
    }

    void Update()
    {
        //Adrian Betancourt
        

        // Clamp doesn't seem to be working, might be something with health pickups.
        // Either way this will set health to 100 if it happens to go over for whatever reason.
        if(Health > 100)
        {
            Health = 100;
            currenthealth = 100;
        }

        //if(statsReset == true)
        //{
        //    Respawn();
        //}
    }

    //Needed to extract into public method to be called in debug menu (and possibly other scripts) when needed
    public void DeathCheck()
    {
        if (Health <= 0.01f)
        {
            if (!hasPlayed)
            {
                IH_AudioManager.instance.PlayOneShot("KillPlayer");
                hasPlayed = true;
            }
            //player.useGravity = false;
            //player.detectCollisions = false;
            //player.constraints = RigidbodyConstraints.FreezeAll;
            tp.acceleration = zero;
            tp.jumpSpeed = zero;
            //tp.jumpHeight = 0;
            tp.enabled = false;
            blk.enabled = false;
            Debug.Log("You died");
            //StartCoroutine(Death());
            canTakeDamage = false;
            //hasPlayed = false;
            IH_AudioManager.instance.Stop("ChargeShot");
            IH_AudioManager.instance.Stop("CoolDown");
            StartCoroutine(ResetStats());
            //Invoke("ResetStats", 3f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (canTakeDamage) //Locks takedamage functionality to whether canTakeDamage is true or false. Needed for immortality, possibly other scripts.
        {
            Health -= damage;
            currenthealth -= damage;
            HealthBar.value = currenthealth;
        }

        DeathCheck();
    }

    //IEnumerator Death()
    //{
    //    canTakeDamage = false;
    //    yield return new WaitForSeconds(3f);

    //    //gameObject.transform.position = lastCheckpoint;
    //    //Health = 100;
    //    //currenthealth = 100;
    //    //tp.acceleration = 5;
    //    //tp.jumpSpeed = 19;
    //    //tp.enabled = true;
    //    //tp.shieldPwr = 100f;
    //    //tp.full = true;
    //    //tp.empty = false;
    //    //blk.enabled = true;
    //    //Debug.Log("Respawning");
    //    hasPlayed = false;
    //    //HealthBar.value = currenthealth;
    //    //canTakeDamage = true;
    //    Respawn();
    //}

    IEnumerator ResetStats()
    {
        yield return new WaitForSeconds(0.5f);
        //gameObject.transform.position = lastCheckpoint;
        Health = 100;
        Health = Mathf.Clamp(Health, 0.001f, 100.0f);
        currenthealth = 100;
        currenthealth = Mathf.Clamp(currenthealth, 0.001f, 100.0f);
        tp.acceleration = accelDefault;
        tp.jumpSpeed = jumpDefault;
        tp.enabled = true;
        tp.shieldPwr = 100.0f;
        tp.full = true;
        tp.empty = false;

        if (tp.shield.activeSelf == true)
        {
            tp.shield.SetActive(false);
            tp.shieldOn = false;
        }

        //tp.overHeated = false;
        //tp.charging = false;
        tp.dmg = tp.defaultDmg;
        //tp.playedCharge = false;
        blk.enabled = true;
        Debug.Log("Respawning");
        hasPlayed = false;
        HealthBar.value = currenthealth;
        canTakeDamage = true;
        gameObject.transform.position = lastCheckpoint;
        //statsReset = true;
    }

    //void Buffer()
    //{
    //    Respawn();
    //}

    //void Respawn()
    //{
    //    gameObject.transform.position = lastCheckpoint;
    //    //statsReset = false;
    //}
}
