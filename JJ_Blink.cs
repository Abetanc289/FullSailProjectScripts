using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JJ_Blink : MonoBehaviour
{
    //AB
    public CharacterController controller;
    public ParticleSystem particles;
    public float distance = 9.0f;
    public bool _blinkUsed = false;
    public Transform _player;
    private bool _obstacleBlocking = false;
    private bool _noBlockage = false;
    private bool noMovement = false;
    private bool wallTooClose = false;
    public bool _onCoolDown = false;

    private bool _PreviewShowing = false;
    private bool _isBlinking = false;

    public ImpactReceiver _ImpactReceiver;
    public GameObject _BlinkPreview;


    public float _coolDownTime = 1f;



    //public AudioSource BlinkSound;

    //Adrian Betancourt
    [Header("Phase Variables")]
    public GameObject[] ignoreObjects;

    void Start()
    {
        //BlinkSound = GetComponent<AudioSource>();
        // InvokeRepeating("TrackPos", 0.1f, 30.0f);
        controller = GetComponent<CharacterController>();
        ignoreObjects = GameObject.FindGameObjectsWithTag("Phase"); 
    }

    void ResetCooldown()
    {
        print("Blink not on cooldown");
        _onCoolDown = false;
    }

    void TrackPos()
    {
        transform.position = _player.position;
    }

    private void Update()
    {
        var _preVScroll = 0;

        if ((Input.GetAxis("Mouse ScrollWheel") > _preVScroll /*GetKeyDown(KeyCode.E)*/ && _isBlinking == false && _PreviewShowing == true))
        {
            BlinkPreviewTurnOff();
            
        }
        else if ((Input.GetAxis("Mouse ScrollWheel") > _preVScroll /*GetKeyDown(KeyCode.E)*/ && _isBlinking == false && _PreviewShowing == false))
        {
            ShowBlinkPreview();
        }

            if (Input.GetKey(KeyCode.LeftShift) /*|| Input.GetKey(KeyCode.R)*/ && wallTooClose == false && _onCoolDown == false) //&& _PreviewShowing == true)
            {

                //BlinkPreviewTurnOff();
            IH_AudioManager.instance.Play("BlinkDash");
                _ImpactReceiver.AddImpact(transform.forward, 170.0f);
                particles.Play();
            Invoke("ParticleOff", 0.2f);
                
                print("Blink Used, cooldown started");
                _isBlinking = true;
                _blinkUsed = true;
                _onCoolDown = true;
                Invoke("ResetBool", 0.3f);
                Invoke("ResetCooldown", 1f);






                /*if (_obstacleBlocking)
                {

                    BlinkForward();
                }
                else if (_obstacleBlocking == false)
                {
                    //BlinkSound.Play();
                    IH_AudioManager.instance.Play("BlinkDash");
                    BlinkForward();
                }*/


            }

        if (Input.GetKey(KeyCode.R) && wallTooClose == false && _onCoolDown == false) //&& _PreviewShowing == true)
        {

            //BlinkPreviewTurnOff();
            IH_AudioManager.instance.Play("BlinkDash");
            _ImpactReceiver.AddImpact(transform.forward, 170.0f);
            particles.Play();
            Invoke("ParticleOff", 0.2f);

            print("Blink Used, cooldown started");
            _isBlinking = true;
            _blinkUsed = true;
            _onCoolDown = true;
            Invoke("ResetBool", 0.3f);
            Invoke("ResetCooldown", 1f);






            /*if (_obstacleBlocking)
            {

                BlinkForward();
            }
            else if (_obstacleBlocking == false)
            {
                //BlinkSound.Play();
                IH_AudioManager.instance.Play("BlinkDash");
                BlinkForward();
            }*/


        }


    }

   /* void FixedUpdate()
    {
       

        RaycastHit _colliderContact;
        Ray _forwardRay = new Ray(_player.transform.position, Vector3.forward);

        if(Physics.Raycast(_forwardRay, out _colliderContact))
        {
            if(_colliderContact.distance <= 2f)
            {
                for (int i = 0; i < ignoreObjects.Length; i++)
                {
                    if (_colliderContact.collider != ignoreObjects[i].GetComponent<Collider>())
                    {
                        wallTooClose = true;
                    }
                }
            }
            else
            {
                wallTooClose = false; 
            }
        }

       /* if (Input.GetMouseButtonUp(1))
        {
            _blinkUsed = false;
        }

        
    }*/
//
    void ShowBlinkPreview()
    {
        _BlinkPreview.SetActive(true);
        Invoke("SetPreviewDelay", 0.2f);
        
    }

    void ParticleOff()
    {
        particles.Stop();
    }

    void SetPreviewDelay()
    {
        _PreviewShowing = true;
    }
//


    void BlinkPreviewTurnOff()
    {
        _BlinkPreview.SetActive(false);
        _PreviewShowing = false;
    }

   
    void ResetBool()
    {
        _blinkUsed = false;
        _isBlinking = false;
    }

    

    

    public void BlinkForward()
    {

        

        RaycastHit hit;

        
        
        Vector3 destination = _player.transform.position + _player.forward *  distance;

        //obstacle blocking
        if (Physics.Linecast(transform.position, destination, out hit))
        {
           if (hit.distance <= 1f)
            {
                _obstacleBlocking = true;
                
            }

            GetComponent<CharacterController>().enabled = false;
            destination = GetComponent<CharacterController>().transform.position + GetComponent<CharacterController>().transform.forward * (hit.distance - 0.05f);
            GetComponent<CharacterController>().enabled = true;

            //_player.transform.position + _player.forward * (hit.distance - 0.2f);

        }

        //no blockage
        if (Physics.Raycast(destination, -Vector3.up, out hit))
        {


            //GetComponent<Rigidbody>().Equals(true);
           _obstacleBlocking = false;

            GetComponent<CharacterController>().enabled = false;
            destination = GetComponent<CharacterController>().transform.position + GetComponent<CharacterController>().transform.forward;
            destination = hit.point;
            destination.y = GetComponent<CharacterController>().transform.position.y;
            GetComponent<CharacterController>().transform.position = destination;
            GetComponent<CharacterController>().enabled = true;

            // = _player.transform.position.y;

            //_player.position = destination;
             
        }
    }
}
