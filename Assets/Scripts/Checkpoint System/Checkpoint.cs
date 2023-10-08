using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Index of the current checkpoint
    public int checkPointIndex = 0;

    // Denotes the number of AI entites who have crossed this checkpoint
    public int numberOfAIWhoCrossedCheckpoint = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AICar")
            numberOfAIWhoCrossedCheckpoint++;
    }
}
