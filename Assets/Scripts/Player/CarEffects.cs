using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarEffects : MonoBehaviour
{
    int brakeInput = 0;

    CarInputActions carInputActions;

    [Header("Trail renderers")]
    [SerializeField]
    TrailRenderer[] trails;

    [Header("Break light")]
    [SerializeField]
    Light brakeLights;

    void Awake()
    {
        carInputActions = new CarInputActions();

        carInputActions.Drive.Brake.performed += (InputAction.CallbackContext context) =>
       {
           brakeInput = 1;
       };
        carInputActions.Drive.Brake.canceled += (InputAction.CallbackContext context) =>
        {
            brakeInput = 0;
        };
    }

    void OnEnable()
    {
        carInputActions.Drive.Enable();
    }

    void OnDisable()
    {
        carInputActions.Drive.Disable();
    }

    bool isGrounded = false;

    void Update()
    {
        isGrounded = Physics.CheckSphere(gameObject.transform.position + new Vector3(0, 1, 0), 1.2f, LayerMask.GetMask("Ground")) ||
                     Physics.CheckSphere(gameObject.transform.position + new Vector3(0, 1, 0), 1.2f, LayerMask.GetMask("Road"));

        if (brakeInput == 1)
        {
            brakeLights.intensity = 0.5f;

            if (isGrounded)
                StartEmittingTrail();
            else
                StopEmittingTrail();
        }
        else
        {
            StopEmittingTrail();
            brakeLights.intensity = 0f;
        }
    }

    void StartEmittingTrail()
    {
        foreach (var trail in trails)
            trail.emitting = true;
    }

    void StopEmittingTrail()
    {
        foreach (var trail in trails)
            trail.emitting = false;
    }
}
