using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarHandling : MonoBehaviour
{
    //! This script assumes that the front wheels are located at indices 0 and 1
    //! Make sure that this is the case otherwise the script will not work as intended
    [Header("Wheel colliders : Place front wheel colliders at indices 0 and 1")]
    [SerializeField]
    GameObject[] wheelColliders;

    [Header("Wheel meshes")]
    [SerializeField]
    GameObject[] wheelMeshes;

    [Header("Car handling")]
    [SerializeField]
    [Range(0f, 500f)]
    float maxTorque = 300f;

    [SerializeField]
    bool useNaiveSteering = false;

    [SerializeField]
    [Range(0f, 50f)]
    float maxSteerAngle = 20f;

    [SerializeField]
    [Range(5000f, 500000f)]
    float maxBrakingTorque = 8000f;

    [SerializeField]
    Transform centerOfMass;

    [Header("Ackerman steering")]
    [SerializeField]
    float wheelBase;
    [SerializeField]
    float rearTrackSize, radius;

    CarInputActions carInputActions;

    float verticalInput = 0, horizontalInput = 0;
    int brakeInput = 0;

    [Header("Car rigid body")]
    [SerializeField]
    Rigidbody carRigidBody;

    float initialDrag, initialAngularDrag;

    void Awake()
    {
        carRigidBody.centerOfMass = centerOfMass.localPosition;

        carInputActions = new CarInputActions();

        initialDrag = carRigidBody.drag;
        initialAngularDrag = carRigidBody.angularDrag;

        carInputActions.Drive.Move.performed += (InputAction.CallbackContext context) =>
        {
            Vector2 inputValues = context.ReadValue<Vector2>();
            verticalInput = inputValues.y;
            horizontalInput = inputValues.x;
        };
        carInputActions.Drive.Move.canceled += (InputAction.CallbackContext context) =>
        {
            verticalInput = horizontalInput = 0f;
        };

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

    void FixedUpdate()
    {
        MoveCar(acceleration: verticalInput);
        SteerCar(steering: horizontalInput);
        HandleBraking(braking: brakeInput);
        AnimateWheels();
    }

    void MoveCar(float acceleration)
    {
        acceleration = Mathf.Clamp(acceleration, -1.0f, 1.0f);

        for (int i = 0; i < wheelColliders.Length; i++)
            wheelColliders[i].GetComponent<WheelCollider>().motorTorque = acceleration * maxTorque;
    }

    //! Only apply steering to the front wheels
    void SteerCar(float steering)
    {
        steering = Mathf.Clamp(steering, -1.0f, 1.0f);

        if (useNaiveSteering)
            //! Naive car steering. Use Ackerman steering
            for (int i = 0; i < 2; i++)
                wheelColliders[i].GetComponent<WheelCollider>().steerAngle = steering * maxSteerAngle;
        else
        {
            //* Ackerman steering
            if (steering > 0)
            {
                wheelColliders[0].GetComponent<WheelCollider>().steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius - (rearTrackSize / 2))) * steering;
                wheelColliders[1].GetComponent<WheelCollider>().steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius + (rearTrackSize / 2))) * steering;
            }
            else if (steering < 0)
            {
                wheelColliders[0].GetComponent<WheelCollider>().steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius + (rearTrackSize / 2))) * steering;
                wheelColliders[1].GetComponent<WheelCollider>().steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius - (rearTrackSize / 2))) * steering;
            }
            else
            {
                wheelColliders[0].GetComponent<WheelCollider>().steerAngle = 0f;
                wheelColliders[1].GetComponent<WheelCollider>().steerAngle = 0f;
            }
        }
    }

    void HandleBraking(float braking)
    {
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            //! Only apply breaks on the rear wheel
            if (i > 2)
                wheelColliders[i].GetComponent<WheelCollider>().brakeTorque = braking * maxBrakingTorque;

            // Drag
            if (braking == 1)
            {
                carRigidBody.drag = 0.5f;
                carRigidBody.angularDrag = 0.2f;
            }
            else
            {
                carRigidBody.drag = initialDrag;
                carRigidBody.angularDrag = initialAngularDrag;
            }
        }
    }

    void AnimateWheels()
    {
        Vector3 wheelPosition;
        Quaternion wheelRotation;

        for (int i = 0; i < wheelColliders.Length; i++)
        {
            wheelColliders[i].GetComponent<WheelCollider>().GetWorldPose(out wheelPosition, out wheelRotation);

            // Assign the position and rotation to the meshes
            wheelMeshes[i].transform.position = Vector3.Lerp(wheelMeshes[i].transform.position, wheelPosition, 0.1f);
            wheelMeshes[i].transform.rotation = Quaternion.Lerp(wheelMeshes[i].transform.rotation, wheelRotation, 0.1f);
        }
    }
}
