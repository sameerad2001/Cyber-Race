using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountDownBeforeStart : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]
    [Range(0, 10)]
    int countDownTime = 5;

    GameObject[] aiCars;
    GameObject playerCar;

    [SerializeField]
    GameObject countDownText;

    // Start is called before the first frame update
    void Awake()
    {
        aiCars = GameObject.FindGameObjectsWithTag("AICar");
        playerCar = GameObject.FindGameObjectWithTag("PlayerCar");

        // Disable car handling in both the player and the ai cars
        playerCar.GetComponent<CarHandling>().enabled = false;
        foreach(GameObject aiCar in aiCars)
        {
            if (aiCar.GetComponent<AICarHandlingNodeSystem>() != null)
                aiCar.GetComponent<AICarHandlingNodeSystem>().enabled = false;
        }

        // Start countdown and then re-enable the car handling
        StartCoroutine(nameof(countDownAndEnableHandling));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator countDownAndEnableHandling()
    {
        for(int timeElapsed = 0; timeElapsed < countDownTime; timeElapsed++)
        {
            countDownText.GetComponent<TextMeshProUGUI>().text = (countDownTime - timeElapsed).ToString();
            // Wait one second
            yield return new WaitForSeconds(1f);

        }

        // Disable the count down text
        countDownText.SetActive(false);

        // Enable car handling in both the player and the ai cars
        playerCar.GetComponent<CarHandling>().enabled = true;
        foreach (GameObject aiCar in aiCars)
        {
            if (aiCar.GetComponent<AICarHandlingNodeSystem>() != null)
                aiCar.GetComponent<AICarHandlingNodeSystem>().enabled = true;
        }
    }
}
