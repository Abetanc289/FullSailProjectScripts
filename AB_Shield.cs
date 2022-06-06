using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AB_Shield : MonoBehaviour
{
    //[Header("Adjustable size of collision box")]
    //[SerializeField] Vector3 collisionBox;

    //[Header("Center of collision box where box is created from")]
    //[SerializeField] GameObject CollisionBoxCenter;

    //[Header("Layer Mask for specific layer detection. Set to FireBall since that's all we're looking for.")]
    //[SerializeField] LayerMask m_LayerMask;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            AB_HurtEnemies hurtEnemies = collision.gameObject.GetComponent<AB_HurtEnemies>();
            hurtEnemies.EnemyTakeDamage(100.0f);
        }
    }

    //private void FixedUpdate()
    //{
    //    ShieldCollisions();
    //}

    //void ShieldCollisions()
    //{
    //    //Use the OverlapBox to detect if there are any other colliders within this box area.
    //    //Use the GameObject's centre, half the size (as a radius) and rotation. This creates an invisible box around your GameObject.
    //    Collider[] shieldHitColliders = Physics.OverlapBox(CollisionBoxCenter.transform.position, collisionBox / 2, Quaternion.identity, m_LayerMask);

    //    int i = 0;

    //    //Check when there is a new collider coming into contact with the box
    //    while (i < shieldHitColliders.Length)
    //    {
    //        //Output all of the collider names
    //        Debug.Log("Top Check Box Hit : " + shieldHitColliders[i].name + i);

    //        if (shieldHitColliders[i].tag == "FireBall")
    //        {
    //            shieldHitColliders[i].enabled = false;
    //        }

    //        //Increase the number of Colliders in the array
    //        i++;
    //    }
    //}

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireCube(CollisionBoxCenter.transform.position, collisionBox);
    //}
}
