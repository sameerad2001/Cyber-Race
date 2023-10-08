using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscilatingObject : MonoBehaviour
{
    Vector3 startingPosition;
    [SerializeField] Vector3 maximumDisplacementValue;
    [SerializeField] [Range(0, 1)] float displacementFactor; // Values will be between 0 and 1
    [SerializeField] float oscillationPeriod = 2f; // Oscillate every 2 seconds

    void Start()
    {
        startingPosition = gameObject.transform.position;
    }

    void Update()
    {
        if (oscillationPeriod <= Mathf.Epsilon)
            return;

        // Lets say 10 seconds has elapsed since the start of the game
        // Since our oscillation period is 2 seconds 
        // => The number of cycles = 5
        float numberOfCycles = (Time.time / oscillationPeriod) % 1;

        const float tau = Mathf.PI * 2;

        // Each cycle is of size 1 tau
        float rawSinWave = Mathf.Sin(numberOfCycles * tau);

        // We want the displacement factor's value to range in 0 to 1
        // since our sin wave ranges from -1 to 1. 
        // By adding 1 to the value the range becomes 0 to 2, hence we divide by 2 to make the range 0 to 1
        displacementFactor = (rawSinWave + 1f) / 2;

        Vector3 offset = maximumDisplacementValue * displacementFactor;
        gameObject.transform.position = startingPosition + offset;
    }
}
