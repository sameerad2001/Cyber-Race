using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    float shakeIntensity = 0.5f;

    [SerializeField]
    [Range(0f, 0.5f)]
    [Tooltip("Amount by which the camera can be offset when shaken")]
    float shakeOffset = 0.15f;

    [SerializeField]
    Rigidbody carRB;

    Vector3 initialPosition;

    float timeCounter = 0;

    [SerializeField]
    [Range(0, 100)]
    [Tooltip("If the car's speed is less than this value, no camera shake is applied")]
    float minShakeSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = gameObject.transform.localPosition;
    }

    Vector3 GetOffsetPosition()
    {
        timeCounter += Time.deltaTime * Mathf.Sqrt(shakeIntensity) * Mathf.Sqrt(carRB.velocity.magnitude);

        return new Vector3(
            // Position between -shakeOffset and +shakeOffset
            // How : perline noise returns a value between 0 and 1
            // If we subtract 0.5 and multiply by 2 we get a value between -1 and 1
            // If we multiply this value with our offset amount we get a value between -shakeOffset and +shakeOffset
            (Mathf.PerlinNoise(0, timeCounter) - 0.5f) * 2 * shakeOffset,
            (Mathf.PerlinNoise(1, timeCounter) - 0.5f) * 2 * shakeOffset,
            0
        );
    }

    // Update is called once per frame
    void Update()
    {
        if (carRB.velocity.magnitude < minShakeSpeed)
            return;

        gameObject.transform.localPosition = initialPosition + GetOffsetPosition();
    }
}
