using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AB_InvisibleBarrierToggle : MonoBehaviour
{
    public GameObject invisibleBarrier;
    
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            invisibleBarrier.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            invisibleBarrier.SetActive(false);
        }

    }
}
