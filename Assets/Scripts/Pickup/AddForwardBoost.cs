using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForwardBoost : MonoBehaviour
{
    [SerializeField]
    [Range(1000, 10000)]
    int forceAmount = 10;

    Rigidbody playerRB;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerCar")
        {
            Rigidbody playerRB = GameObject.FindGameObjectWithTag("PlayerCar").GetComponent<Rigidbody>();
            Vector3 forwardForce = other.transform.forward * forceAmount;

            if(playerRB)    
               playerRB.AddForce(forwardForce, ForceMode.Impulse);
            else
                Debug.LogError("PlayerCar does not have a Rigidbody!");
        }
    }
}
