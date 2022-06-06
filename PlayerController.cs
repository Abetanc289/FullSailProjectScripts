using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    //Character variables
    private Transform playerTransfrom;
    CharacterController controller;

    //Camera variables
    Camera mainCamVirtual;
    CinemachineFreeLook CM_ThirdPerson;
    private float ogVertSpeedThirdPersonCam;    

    [Header("Movement Variables")]
    Vector3 lastPos = Vector3.zero;
    [SerializeField] float currentMovementSpeed = 0;//should not be SerializeField for debug purposes only
    [SerializeField] float currentStrafeSpeed = 0;
    [SerializeField] float currentJumpSpeed = 0;//should not be SerializeField for debug purposes only
    [SerializeField] public float gravity = 20.0f;
    [SerializeField] public float jumpSpeed = 15.0f;    
    [SerializeField] public float testSpeed;
    [SerializeField] public float acceleration = 20.0f;
    [SerializeField] float strafeAcceleration = 20.0f;
    private float tempAccel;
    [SerializeField] float slowDownSpeed = 40.0f;
    [SerializeField] float velocityLimit = 7.0f;
    float moveAndStrafeVelocity = 5f;
    [SerializeField] float playerRotationSpeed = 60f;
    private float apexFreezeTime = 0.0f;
    private float ogApexFreezeTime;

    bool stopGravity = false;
    //private bool speedBoost;

    [Header("Boost Jump Variables")]
    [SerializeField] public float boostJumpSpeed = 17.0f;
    [SerializeField] private bool hasJumped = false;
    [SerializeField] private bool canDoubleJump;

    bool hasFired1;
    bool hasFired2;

    [Header("Boost Boots Components")]
    [SerializeField] Transform jetPoint1;
    [SerializeField] Transform jetPoint2;
    [SerializeField] GameObject fireTrail1;
    [SerializeField] GameObject fireTrail2;
    [SerializeField] private Material off_Material;
    [SerializeField] private Material on_Material;
    [SerializeField] private GameObject[] Indicators;

    float actualTime = 0f;

    private HealthSystem health;

    //Adrian Betancourt
    [Header("Shield Variables")]
    public GameObject shield;
    public float shieldPwr;
    public float minPwr = 0.0f;
    public float maxPwr = 100.0f;
    private float drainRate = 25.0f;
    public bool full;
    public bool shieldOn;
    public bool empty;

    //Adrian Betancourt
    [Header("Charge Shot Variables")]
    public Transform firePointRotator;
    public Transform firePoint;
    public GameObject reticle;
    [SerializeField] float firePointPositionDifferential = 1.5f;
    [SerializeField] LayerMask m_LayerMask;
    public GameObject playerAmmo;
    public Material chargeMat;
    public Color lowCharge = Color.blue;
    //public Color medCharge = Color.yellow;
    //public Color fullCharge = Color.red;
    public float force = 30.0f;
    //public float chargeRate = 25.0f;
    public float dmg;
    public float defaultDmg = 1.000f;
    public float maxDmg = 100.000f;
    //public bool charging = false;
    //public bool overHeated = false;

    //public RaycastHit hitInfo;

    //Adrian Betancourt
    [Header("Phase Blink Variables")]
    public JJ_Blink blinkScript;
    public GameObject[] phaseObjects;
    public Collider charCol;

    [Header("Companion Ref")]
    [SerializeField] GameObject companion;

    public bool playedCharge;
    public bool playedOverHeat;
    private bool playedCoolDown;
    private bool playedPlayerShoot;

    bool hasPlayedJets = false;
    bool hasPlayedJump = false;

    float tempVelocity;

    public bool hasPlayedBurnPlayer = false;

    public bool hasPlayedBossLaugh = false;

    private void Awake()
    {        
        playerTransfrom = GameObject.Find("IH_ThrdPrsPlyr").GetComponent<Transform>();
        controller = GetComponent<CharacterController>();

        mainCamVirtual = Camera.main;
        CM_ThirdPerson = GameObject.Find("CM_ThirdPerson").GetComponent<CinemachineFreeLook>();
        //ogVertSpeedThirdPersonCam = CM_ThirdPerson.m_YAxis.m_MaxSpeed;

        health = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthSystem>();
        blinkScript = GetComponent<JJ_Blink>();
        phaseObjects = GameObject.FindGameObjectsWithTag("Phase");
    }

    void Start()
    {
        hasPlayedBossLaugh = false;
        hasPlayedBurnPlayer = false;

        hasPlayedJets = false;
        hasPlayedJump = false;

        actualTime = Time.time;

        lastPos = transform.position;
        currentJumpSpeed = currentMovementSpeed = 0;
        stopGravity = false;

        tempAccel = acceleration;

        //speedBoost = false;

        canDoubleJump = false;

        ogApexFreezeTime = apexFreezeTime;

        shield.SetActive(false);
        //shieldPwr = minPwr;
        shieldPwr = maxPwr;
        dmg = defaultDmg;
        full = true;
        //empty = true;

        tempVelocity = velocityLimit;
    }
    void Update()
    {
        CharacterRotation();
        CharacterStrafe();
        DoJump();

        Vector3 directionToMove = DoAvatarRotationAndFindSpeed();
        Vector3 directionToStrafe = CharacterStrafe();

        controller.Move((directionToMove * Time.deltaTime * currentMovementSpeed) +
                       (transform.up * Time.deltaTime * currentJumpSpeed)
                       + (directionToStrafe * Time.deltaTime * currentStrafeSpeed));

        //Adrian Betancourt
        MouseButtonInputCheck();//checks for mouse input to control shield and charge shot. will hopefully add blink activation/functionality at later date to consolidate scripts
        ShieldDrain();//will only start when sheild is activated. stops when shield is deactivated.
        //ShotCooldown();
        PhaseCheck();
        //CompanionCall();

        //Check companion null
        //if (companion == null)
        //{

        //}




        if (currentStrafeSpeed > 0 && currentMovementSpeed > 0)
        {
            velocityLimit = moveAndStrafeVelocity;
        }
        else
        {
            velocityLimit = tempVelocity;
        }

        


    }
    
    private void FixedUpdate()
    {
        Ray rayOrigin = mainCamVirtual.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        
        if (Physics.Raycast(rayOrigin, out hitInfo/*, m_LayerMask*/))
        {

            if (hitInfo.collider != null)
            {
                Vector3 direction = hitInfo.point - firePoint.position;
                firePoint.rotation = Quaternion.LookRotation(direction);

                Debug.DrawRay(firePoint.position, firePoint.forward * 22f, Color.red);
            }

            if(hitInfo.collider == null)
            {
                Debug.LogWarning("hitInfo.collider == null");
            }
        }
    }

    //void CompanionCall()
    //{
    //    if (Input.GetKey(KeyCode.Z))
    //    {
    //        Ray ray = mainCamVirtual.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
    //        RaycastHit hit;
    //        if (Physics.Raycast(ray, out hit))
    //        {
    //            if (hit.transform.tag == "Enemy")
    //            {
    //                companion.GetComponent<MH_Companion>().target = hit.transform;
    //                companion.GetComponent<MH_Companion>().currentEnemy = hit.collider.gameObject;
    //                companion.GetComponent<MH_Companion>().stoppingDistance = 0;
    //            }
    //        }
    //    }
    //}

    //This activates the fireJet thrusters particle system. I couldn't figure out how to turn them off from here so I made
    //a separate script. The name of the script is DestroyFireJets and it's on the fire jet prefab. That script destroys the
    //fire jets when you let go of Jump.
    void FireJets()
    {
        if (!hasFired1 && !hasFired2)
        {
            foreach(GameObject indicator in Indicators)
            {
                indicator.GetComponent<Renderer>().material = on_Material;
            }

            GameObject jet1 = Instantiate(fireTrail1, jetPoint1.position, jetPoint1.rotation);
            jet1.transform.parent = jetPoint1.transform;
            //jet1.GetComponent<Rigidbody>().velocity = jet1.transform.forward * (.001f / 2) * Time.deltaTime;

            hasFired1 = true;

            GameObject jet2 = Instantiate(fireTrail2, jetPoint2.position, jetPoint2.rotation);
            jet2.transform.parent = jetPoint2.transform;
            //jet2.GetComponent<Rigidbody>().velocity = jet2.transform.forward * (.001f / 2) * Time.deltaTime;
            hasFired2 = true;

            if (!hasPlayedJets)
            {
                IH_AudioManager.instance.Play("JetPackJets");
                hasPlayedJets = true;
            }
        }
    }

    void CharacterRotation()
    {
        transform.Rotate(Vector3.up, Input.GetAxisRaw("Mouse X") * playerRotationSpeed);

        firePointRotator.rotation = mainCamVirtual.transform.rotation;

        //firePoint.rotation = mainCamVirtual.transform.rotation;
    }

    Vector3 CharacterStrafe()
    {
        Vector3 strafeAcceleration = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, 0.0f);
        Vector3 directionToStrafe = strafeAcceleration;

        if (strafeAcceleration.sqrMagnitude == 0)
        {
            currentStrafeSpeed = Mathf.Clamp(currentStrafeSpeed - Time.deltaTime * slowDownSpeed, 0, velocityLimit);
        }
        else
        {
            strafeAcceleration = transform.TransformDirection(strafeAcceleration.normalized);
            currentStrafeSpeed = Mathf.Clamp(currentStrafeSpeed + strafeAcceleration.magnitude * Time.deltaTime
                                                    * this.strafeAcceleration, 0f, velocityLimit);

            float dotStrafeWithPlayer = Vector3.Dot(strafeAcceleration, transform.right);

            //about 3 degrees
            if (dotStrafeWithPlayer > 0.999f)
            {
                transform.Translate(transform.position + strafeAcceleration);
            }
            else if (dotStrafeWithPlayer < -0.99)
            {
                transform.Translate(transform.position - strafeAcceleration);
                return transform.right * -1f;
            }
        }
        return transform.right;
    }

    Vector3 DoAvatarRotationAndFindSpeed()
    {
        Vector3 acceleration = new Vector3(0.0f, 0.0f, Input.GetAxisRaw("Vertical"));
        Vector3 directionToMove = acceleration;

        if (acceleration.sqrMagnitude == 0)
        {
            currentMovementSpeed = Mathf.Clamp(currentMovementSpeed - Time.deltaTime * slowDownSpeed, 0, velocityLimit);
        }
        else
        {
            acceleration = transform.TransformDirection(acceleration.normalized);
            currentMovementSpeed = Mathf.Clamp(currentMovementSpeed + acceleration.magnitude * Time.deltaTime
                                                    * this.acceleration, 0f, velocityLimit);

            float dotMoveWithPlayer = Vector3.Dot(acceleration, transform.forward);

            //about 3 degrees
            if (dotMoveWithPlayer > 0.999f)
            {
                transform.LookAt(transform.position + acceleration);
            }
            else if (dotMoveWithPlayer < -0.99)
            {
                transform.LookAt(transform.position - acceleration);
                return transform.forward * -1f;
            }
        }
        return transform.forward;
    }

    void DoJump()
    {
        if (Input.GetButtonUp("Jump"))
        {
            foreach (GameObject indicator in Indicators)
            {
                indicator.GetComponent<Renderer>().material = off_Material;
            }
            //IH_AudioManager.instance.Stop("JetPackJets");
            hasPlayedJets = false;
        }
        if (Input.GetKeyDown("space") && hasJumped && canDoubleJump)
        {
            hasJumped = false;
            canDoubleJump = false;
            if (!hasPlayedJump)
            {
                IH_AudioManager.instance.Play("Jump");
                hasPlayedJump = true;
            }
            currentJumpSpeed = boostJumpSpeed;
            FireJets();
        }
        else if (Input.GetKeyDown("space") && !hasJumped && canDoubleJump)
        {
            if (!hasPlayedJump)
            {
                IH_AudioManager.instance.Play("Jump");
                hasPlayedJump = true;
            }
            currentJumpSpeed = jumpSpeed;
            canDoubleJump = true;
            hasJumped = true;
        }
        else if (controller.isGrounded)
        {
            hasPlayedJump = false;
            currentJumpSpeed = 0.0f;
            canDoubleJump = true;
            hasJumped = false;

            hasFired1 = false;
            hasFired2 = false;
        }
        else if (stopGravity)
        {
            currentJumpSpeed = 0;
        }
        else
        {
            testSpeed = currentJumpSpeed;
            currentJumpSpeed -= gravity * Time.deltaTime;
            //test to see if the avatar is at the apex of the jump
            if (testSpeed * currentJumpSpeed < 0)
            {
                stopGravity = true;
                Invoke("ReturnGravity", apexFreezeTime);
            }
        }
    }
    void ReturnGravity()
    {
        stopGravity = false;
    }

    private void PhaseCheck()
    {
        if (blinkScript._blinkUsed)
        {
            for (int i = 0; i < phaseObjects.Length; i++)
            {
                Physics.IgnoreCollision(controller, phaseObjects[i].GetComponent<Collider>(), true);
            }
        }
        else
        {
            for (int i = 0; i < phaseObjects.Length; i++)
            {
                Physics.IgnoreCollision(controller, phaseObjects[i].GetComponent<Collider>(), false);
            }
        }
    }

    //Adrian Betancourt
    private void MouseButtonInputCheck()
    {
        if (Input.GetMouseButtonDown(2))
        {
            ToggleShield();
        }

        ChargeShot();
    }

    //Adrian Betancourt
    private void ChargeShot()
    {
        //this checks if the button is held
       /* if (Input.GetMouseButtonDown(0))
        {
            if (!overHeated)
            {
                charging = true;
            }
        }
        //checks if charging
        if (charging)
        {
            playedCoolDown = false;
            playedPlayerShoot = false;
            if (!playedCharge)
            {
                IH_AudioManager.instance.Play("ChargeShot");
                playedCharge = true;
            }
            //increments dmg value by charge rate every second within the range of defaultDmg and maxDmg
            dmg += chargeRate * Time.smoothDeltaTime;
            dmg = Mathf.Clamp(dmg, defaultDmg, maxDmg);
            if (dmg >= maxDmg)
            {
                overHeated = true;
                IH_AudioManager.instance.Stop("ChargeShot");
                if (!playedOverHeat)
                {
                    IH_AudioManager.instance.Play("OverHeat");
                    playedOverHeat = true;
                }
            }
        }*/

        //when player releases left click
        if (Input.GetMouseButtonUp(0))
        {
            /*
            //reset charging to false
            charging = false;
            IH_AudioManager.instance.Stop("ChargeShot");
            playedCharge = false;
            playedOverHeat = false;
            if (!overHeated)
            {*/
            dmg = defaultDmg;

                //calls shoot method and passes dmg value into the method to be applied to projectile's hurt player value
                ShootProjectile(dmg);

            IH_AudioManager.instance.Play("PlayerShoot");

            //reset dmg value
            //dmg = defaultDmg;
            //}
        }
        //this displays the dmg value in debug log
        //Debug.Log("Damage Set To: " + dmg + " ");
    }

    /*private void ShotCooldown()
    {
        if (overHeated && charging == false)
        {
            if (!playedCoolDown)
            {
                //yield return new WaitForSeconds(0.2f);
                IH_AudioManager.instance.Play("CoolDown");
                playedCoolDown = true;
            }
            //StartCoroutine(PlayCoolDown());
            Debug.Log("Shot Cooldown Started");
            dmg -= chargeRate * Time.deltaTime;
            dmg = Mathf.Clamp(dmg, defaultDmg, maxDmg);
            if (dmg <= defaultDmg)
            {
                overHeated = false;
                Debug.Log("Cooled, may now fire");
                IH_AudioManager.instance.Stop("CoolDown");
            }
        }
    }*/
    //Adrian Betancourt
    public void ToggleShield()
    {
        //turns on shield if shield is not active
        if (!shield.activeSelf && !empty)
        {
            shield.SetActive(true);
            shieldOn = true;
            Debug.Log("Shield activated");
            IH_AudioManager.instance.Play("ShieldUp");
            hasPlayedBurnPlayer = true;
        }
        
        //turns off shield if shield is active
        else if (shield.activeSelf)
        {
            shield.SetActive(false);
            shieldOn = false;
            Debug.Log("Shield deactivated");
            IH_AudioManager.instance.Play("ShieldDown");
            hasPlayedBurnPlayer = false;
        }
    }

    //Adrian Betancourt
    public void ShootProjectile(float setDmg)
    {
        GameObject chargedShot;
        Material shotMat;
        Collider shotCol;
        Collider shieldCol;
        Collider bubbleCol;

        //Vector3 shotDirection = hitInfo.point - firePoint.position;

        //instantiates projectile prefab at the position and rotation of the fire point
        chargedShot = Instantiate(playerAmmo, firePoint.position + firePoint.forward * firePointPositionDifferential, firePoint.rotation);
        shotMat = chargedShot.GetComponentInChildren<Renderer>().material;
        SetShotColor(shotMat);
        shieldCol = shield.GetComponent<Collider>();
        //bubbleCol = GetComponentInChildren<JM_BubbleAOE>().Bubble.GetComponent<Collider>();
        shotCol = chargedShot.GetComponentInChildren<Collider>();
        Physics.IgnoreCollision(shotCol, shieldCol);
        //Physics.IgnoreCollision(shotCol, bubbleCol);

        if (!playedPlayerShoot)
        {
            IH_AudioManager.instance.Play("PlayerShoot");
            playedPlayerShoot = true;
        }

        //adds a forward velocity to the projectile
        chargedShot.GetComponent<Rigidbody>().velocity = firePoint.forward * force;

        

        //sets the triggerDamage value of the instantiated projectile's hurtplayer script to the setDmg value recieved from the charge shot method
        chargedShot.GetComponent<IH_HurtPlayer>().triggerDamage = setDmg;

        //debug log shit
        Debug.Log("Projectile Shot");
    }

    private void SetShotColor(Material shotMat)
    {
        chargeMat = shotMat;
        chargeMat.color = lowCharge;

        /*if (dmg >= defaultDmg && dmg <= 49.9f)
        {
            chargeMat.color = lowCharge;
        }
        if (dmg >= 50.0f && dmg <= 75.0f)
        {
            chargeMat.color = medCharge;
        }
        if (dmg >= 75.1f && dmg <= maxDmg)
        {
            chargeMat.color = fullCharge;
        }*/
    }

    //Adrian Betancourt
    public void ShieldDrain()
    {
        //checks if the shield is active
        if (shield.activeSelf)
        {
            //decrements shieldPwr value by the drainRate every second within the range of minPwr and maxPwr
            shieldPwr -= drainRate * Time.deltaTime;
            shieldPwr = Mathf.Clamp(shieldPwr, minPwr, maxPwr);
            //sets full to false
            full = false;
            //checks if shieldPwr is less or equal to minPwr
            if (shieldPwr <= minPwr)
            {
                //sets empty to true
                empty = true;
                //checks if empty is true
                if (empty)
                {
                    //deactivates shield
                    shield.SetActive(false);
                    shieldOn = false;
                    hasPlayedBurnPlayer = false;
                    Debug.Log("Shield Depleted");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            gameObject.GetComponentInParent<HealthSystem>().lastCheckpoint = other.GetComponent<Checkpoint>().lastCheckpoint;
            Debug.Log("Player hit checkpoint");

            //IH_AudioManager.instance.Play("Checkpoint");
        }

        if (other.tag == "FireBall")
        {
            Debug.LogWarning("Player hit by fireball");

            if (!hasPlayedBurnPlayer && GetComponent<HealthSystem>().Health > 0.01f && shieldOn == false)
            {
                IH_AudioManager.instance.PlayOneShot("BurnPlayer");

                hasPlayedBurnPlayer = true;

                StartCoroutine(BurnSFXDelay());
            }
        }

        if (other.tag == "SkeletonArm")
        {
            if (!hasPlayedBossLaugh && GetComponent<HealthSystem>().Health > 0.01f)
            {
                IH_AudioManager.instance.Play("BossLaugh");
                IH_AudioManager.instance.Play("BossHit");

                hasPlayedBossLaugh = true;

                StartCoroutine(BossLaughDelay());
            }
        }
    }

    IEnumerator BossLaughDelay()
    {
        yield return new WaitForSeconds(4f);

        hasPlayedBossLaugh = false;
    }

    IEnumerator BurnSFXDelay()
    {
        yield return new WaitForSeconds(3f);

        hasPlayedBurnPlayer = false;
    }
}
