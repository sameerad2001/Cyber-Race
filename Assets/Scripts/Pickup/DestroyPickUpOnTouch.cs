using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPickUpOnTouch : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    float delay = 0.3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerCar")
            Invoke(nameof(DestroyPickUp), delay);
    }   

    void DestroyPickUp()
    {
        Destroy(gameObject);
    }
}
