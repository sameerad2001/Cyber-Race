using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSound : MonoBehaviour
{
    Rigidbody CarRigidBody;
    AudioSource CarAudioSource;

    [Header("Speed parameter")]
    [SerializeField]
    float minSpeed;

    [Header("Sound parameter")]
    [SerializeField]
    float minPitch;
    [SerializeField]
    float maxPitch;

    void Start()
    {
        CarRigidBody = GetComponent<Rigidbody>();
        CarAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        PlayEngineSound();
    }

    void PlayEngineSound()
    {
        float currentSpeed = CarRigidBody.velocity.magnitude;
        float variablePitch = currentSpeed / 50f;

        if (currentSpeed < minSpeed)
            CarAudioSource.pitch = minPitch;
        else
            CarAudioSource.pitch = Mathf.Min(maxPitch, minPitch + variablePitch);
    }
}
