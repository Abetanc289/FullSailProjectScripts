using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AB_ShieldUI : MonoBehaviour
{   
    //Created with love by Adrian Betancourt and Joshua McGee
    //ADD THIS SCRIPT TO THE "ShieldBar" GAMEOBJECT OF THE ShieldCooldownUI PREFAB

    public Slider shieldCooldown;
    public Image shieldBackground;
    public GameObject player;
    public Color defColor;
    public Color strobeOne = Color.red;
    public Color strobeTwo = Color.yellow;
    private float Timer;
    private float minTime = 0.0f;
    private float maxTime = 20.0f;


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        defColor = shieldBackground.color;
        Timer = maxTime;
    }
    void Update()
    {
        shieldCooldown.value = player.GetComponent<PlayerController>().shieldPwr;
        StrobeOnEmpty();
    }

    private void StrobeOnEmpty()
    {
        if (player.GetComponent<PlayerController>().empty)
        {
            Timer -= Time.deltaTime;
            Timer = Mathf.Clamp(Timer, minTime, maxTime);
            if (Timer <= 19.9f && Timer >= 19.0f)
            {
                shieldBackground.color = strobeOne;
            }
            if (Timer <= 18.9f && Timer >= 18.0f)
            {
                shieldBackground.color = strobeTwo;
            }
            if (Timer <= 17.9f && Timer >= 17.0f)
            {
                shieldBackground.color = strobeOne;
            }
            if (Timer <= 16.9f && Timer >= 16.0f)
            {
                shieldBackground.color = strobeTwo;
            }
            if (Timer <= 15.9f && Timer >= 15.0f)
            {
                shieldBackground.color = strobeOne;
            }
            if (Timer <= 14.9f && Timer >= 14.0f)
            {
                shieldBackground.color = strobeTwo;
            }
            if (Timer <= 13.9f && Timer >= 13.0f)
            {
                shieldBackground.color = strobeOne;
            }
            if (Timer <= 12.9f && Timer >= 12.0f)
            {
                shieldBackground.color = strobeTwo;
            }
            if (Timer <= 11.9f && Timer >= 11.0f)
            {
                shieldBackground.color = strobeOne;
            }
            if (Timer <= 10.9f && Timer >= 10.0f)
            {
                shieldBackground.color = strobeTwo;
            }
            if (Timer <= 9.9f && Timer >= 9.0f)
            {
                shieldBackground.color = strobeOne;
            }
            if (Timer <= 8.9f && Timer >= 8.0f)
            {
                shieldBackground.color = strobeTwo;
            }
            if (Timer <= 7.9f && Timer >= 7.0f)
            {
                shieldBackground.color = strobeOne;
            }
            if (Timer <= 6.9f && Timer >= 6.0f)
            {
                shieldBackground.color = strobeTwo;
            }
            if (Timer <= 5.9f && Timer >= 5.0f)
            {
                shieldBackground.color = strobeOne;
            }
            if (Timer <= 4.9f && Timer >= 4.0f)
            {
                shieldBackground.color = strobeTwo;
            }
            if (Timer <= 3.9f && Timer >= 3.0f)
            {
                shieldBackground.color = strobeOne;
            }
            if (Timer <= 2.9f && Timer >= 2.0f)
            {
                shieldBackground.color = strobeTwo;
            }
            if (Timer <= 1.9f && Timer >= 1.0f)
            {
                shieldBackground.color = strobeOne;
            }
            if (Timer <= minTime)
            {
                Timer = maxTime;
            }
        }
        else if (player.GetComponent<PlayerController>().empty == false)
        {
            Timer = maxTime;
            shieldBackground.color = defColor;
        }
    }
}
