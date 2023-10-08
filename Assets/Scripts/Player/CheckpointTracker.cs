using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointTracker : MonoBehaviour
{
    int indexOfLastCheckpointCrossed = -1;
    int numberOfCheckpoints = 0;
    int numberOfAIs;

    public bool hasPlayerWon = false;
    public bool isLevelFinished = false;

    private void Awake()
    {
        numberOfCheckpoints = GameObject.FindObjectsOfType<Checkpoint>().Length;
        numberOfAIs = GameObject.FindObjectsOfType<AICarHandlingNodeSystem>().Length;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Checkpoint" || isLevelFinished)
            return;

        Checkpoint checkpoint = other.GetComponent<Checkpoint>();

        if (checkpoint.checkPointIndex == (indexOfLastCheckpointCrossed + 1) % numberOfCheckpoints)
        {
            if (indexOfLastCheckpointCrossed == numberOfCheckpoints - 1)
            {
                //! IMPORTANT___________________________________________________
                // The final checkpoint of a level is the first checkpoint because a circuit is a *loop*
                // If the number of AI crossing this checkpoint is grater than the total number of AIs in the level
                // ... it means that a few or all the AIs have crossed this checkpoint twice i.e finished the level before the player
                if (checkpoint.numberOfAIWhoCrossedCheckpoint <= numberOfAIs)
                    hasPlayerWon = true;
                else
                    hasPlayerWon = false;

                isLevelFinished = true;
            }

            indexOfLastCheckpointCrossed = checkpoint.checkPointIndex;
        }
    }
}
