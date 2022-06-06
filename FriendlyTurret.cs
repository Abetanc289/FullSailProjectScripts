using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyTurret : MonoBehaviour {

    private Transform target = null;
    private float disengageTimer;
    private float shootTimer;

    //Allow debug logs to print to the console
    [SerializeField] private bool debug;

    private void DebugLog(string msg)
    {
        if (debug)
        {
            Debug.Log(msg);
        }
    }

    [Header("Oscillation Variables")]
    [SerializeField] private float oscillateSpeed;
    [SerializeField] private float oscillateAngle;

    [Header("Turret Components")]
    [SerializeField] private Transform t_Pivot;
    [SerializeField] private Transform t_Sight;
    [SerializeField] private Transform t_Muzzle;
    //[SerializeField] private Transform t_BarrFire;

    [Header("Targeting Variables")]
    [SerializeField] private LayerMask mask;
    [SerializeField] private float distance;
    [SerializeField] private float speed;
    [SerializeField] private float cancelTime;

    [Header("Shooting Variables")]
    //[SerializeField] private GameObject barrFire;
    [SerializeField] private GameObject ammo;
    [SerializeField] private float rate;
    [SerializeField] private float force;

    [SerializeField] private Material og_Material;
    [SerializeField] private Material dg_Material;
    [SerializeField] private GameObject turretStand, turretBody, turretBarrel;

    private void Oscillate()
    {
        float Y = Mathf.Cos(oscillateSpeed * Time.time) * oscillateAngle;
        t_Pivot.localRotation = Quaternion.Euler(0, Y, 0);

        TargetSearch();
    }

    private void TargetSearch()
    {
        RaycastHit hit;
        Ray ray = new Ray(t_Sight.position, t_Sight.forward);

        if (Physics.Raycast(ray, out hit, distance, mask))
        {
            if (hit.collider.tag == "Enemy")
            {
                DebugLog("Enemy Found");

                //GameObject bFire = Instantiate(barrFire, t_BarrFire.position, t_BarrFire.rotation);

                //StartCoroutine(Target());
                target = hit.transform;      

                //IEnumerator Target()
                //{
                //    yield return new WaitForSeconds(1);
                //    target = hit.transform;
                //}
            }
            if (hit.collider.tag == "Turret")
            {
                DebugLog("Turret Found");

                //GameObject bFire = Instantiate(barrFire, t_BarrFire.position, t_BarrFire.rotation);

                //StartCoroutine(Target());
                target = hit.transform;

                //IEnumerator Target()
                //{
                //    yield return new WaitForSeconds(1);
                //    target = hit.transform;
                //}
            }
        }
    }

    private void Targeting()
    {
        Vector3 targetPosition = new Vector3(target.position.x, t_Pivot.position.y, target.position.z);

        Vector3 targetDirection = targetPosition - t_Pivot.position;

        Vector3 newDirection = Vector3.RotateTowards(t_Pivot.forward, targetDirection, speed * Time.deltaTime, 0f);

        t_Pivot.rotation = Quaternion.LookRotation(newDirection);

        float half = (360f - (oscillateAngle * 2f)) / 2f;

        if (t_Pivot.localRotation.eulerAngles.y > oscillateAngle && t_Pivot.localRotation.eulerAngles.y < oscillateAngle + half)
        {
            t_Pivot.localRotation = Quaternion.Euler(new Vector3(0, oscillateAngle, 0));
        }
        else if (t_Pivot.localRotation.eulerAngles.y < 360 - oscillateAngle && t_Pivot.localRotation.eulerAngles.y >= oscillateAngle + half)
        {
            t_Pivot.localRotation = Quaternion.Euler(new Vector3(0, 360 - oscillateAngle, 0));
        }

        ShootProjectile();
        DisengageCheck();
    }

    private void DisengageCheck()
    {
        RaycastHit hit;
        Ray ray = new Ray(t_Sight.position, t_Sight.forward);
        if (Physics.Raycast(ray, out hit, distance, mask))
        {
            if (hit.collider.tag == "Enemy" || hit.collider.tag == "Turret")
            {
                disengageTimer = 0;
            }
            else
            {
                disengageTimer += Time.deltaTime;
            }
        }
        else { disengageTimer += Time.deltaTime; }

        if (disengageTimer >= cancelTime)
        {
            DebugLog("Target Lost");

            target = null;
        }
    }

    private void ShootProjectile()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= rate)
        {
            GameObject shot = Instantiate(ammo, t_Muzzle.position, t_Muzzle.rotation);

            shot.GetComponent<Rigidbody>().velocity = shot.transform.forward * force;

            DebugLog("Projectile Shot");

            shootTimer = 0;
        }
    }

	// Use this for initialization
	void Start()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (target == null)
        {           
            //DebugLog("Oscillating");

            if(turretStand == null)
            {
                Debug.Log("If this is an enemy shoulder turret, disregar, otherwise set the turret stand into the inspector slot.");
                return;
            }
            else
            {
                turretStand.GetComponent<Renderer>().material = og_Material;
            }
            turretBody.GetComponent<Renderer>().material = og_Material;
            turretBarrel.GetComponent<Renderer>().material = og_Material;

            Oscillate();
        }
        else
        {
            DebugLog("Targeting");

            if (turretStand == null)
            {
                Debug.Log("If this is an enemy shoulder turret, disregar, otherwise set the turret stand into the inspector slot.");
                return;
            }
            else
            {
                turretStand.GetComponent<Renderer>().material = dg_Material;
            }
            turretBody.GetComponent<Renderer>().material = dg_Material;
            turretBarrel.GetComponent<Renderer>().material = dg_Material;

            Targeting();
        }
	}
}
