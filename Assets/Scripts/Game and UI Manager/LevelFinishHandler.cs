using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelFinishHandler : MonoBehaviour
{
    CheckpointTracker CheckpointTracker;

    [SerializeField]
    GameObject LevelFinishUI;

    [SerializeField]
    TextMeshProUGUI levelStatusText;

    bool hasHandledLevelFinish = false;

    private void Awake()
    {
        CheckpointTracker = GameObject.FindObjectOfType<CheckpointTracker>();
    }

    void LateUpdate()
    {
        if (!CheckpointTracker.isLevelFinished || hasHandledLevelFinish)
            return;

        // Display the level finish UI
        LevelFinishUI.SetActive(true);

        // Turn off pause menu script : Prevents player from brining up the pause menu after the level finishes
        gameObject.GetComponent<PauseMenu>().enabled = false;

        if (CheckpointTracker.hasPlayerWon)
            levelStatusText.text = "You Won!";
        else
            levelStatusText.text = "You Lost!";

        // Stop the game
        Time.timeScale = 0;

        hasHandledLevelFinish = true;
    }
}
