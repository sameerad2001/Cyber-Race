using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarHandlingNodeSystem : MonoBehaviour
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
    [Range(0f, 1000f)]
    float maxTorque = 500f;

    [SerializeField]
    [Tooltip("Maximum distance up to which reversing is allowed")]
    float maxReverseDistance = 10f;

    [SerializeField]
    Transform centerOfMass;

    [Header("Ackerman steering")]
    [SerializeField]
    float wheelBase;
    [SerializeField]
    float rearTrackSize, radius;

    Rigidbody carRigidBody;

    [SerializeField]
    Node currentNode;

    bool isBreaking = false;
    float initialDrag, initialAngularDrag;

    [Header("Breaking parameters")]
    [SerializeField]
    [Tooltip("Drag applied to the car when breaking")]
    float breakingDrag = 0.5f;

    [SerializeField]
    [Tooltip("Angular drag applied to the car when breaking")]
    float breakingAngularDrag = 0.2f;

    bool shouldStop = false;

    void Awake()
    {
        carRigidBody = gameObject.GetComponent<Rigidbody>();
        carRigidBody.centerOfMass = centerOfMass.localPosition;
        initialDrag = carRigidBody.drag;
        initialAngularDrag = carRigidBody.angularDrag;
    }

    void MoveCar(float acceleration)
    {
        for (int i = 0; i < wheelColliders.Length; i++)
            wheelColliders[i].GetComponent<WheelCollider>().motorTorque = acceleration * maxTorque;
    }

    //! Only apply steering to the front wheels
    void SteerCar(float steering)
    {
        float innerTurnRadius = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius - (rearTrackSize / 2))) * steering;
        float outerTurnRadius = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius + (rearTrackSize / 2))) * steering;

        //* Ackerman steering
        if (steering > 0)
        {
            wheelColliders[0].GetComponent<WheelCollider>().steerAngle = innerTurnRadius;
            wheelColliders[1].GetComponent<WheelCollider>().steerAngle = outerTurnRadius;
        }
        else if (steering < 0)
        {
            wheelColliders[0].GetComponent<WheelCollider>().steerAngle = outerTurnRadius;
            wheelColliders[1].GetComponent<WheelCollider>().steerAngle = innerTurnRadius;
        }
        else
        {
            wheelColliders[0].GetComponent<WheelCollider>().steerAngle = 0f;
            wheelColliders[1].GetComponent<WheelCollider>().steerAngle = 0f;
        }
    }

    void HandleBraking()
    {
        // Drag
        if (isBreaking)
        {
            carRigidBody.drag = breakingDrag;
            carRigidBody.angularDrag = breakingAngularDrag;
        }
        else
        {
            carRigidBody.drag = initialDrag;
            carRigidBody.angularDrag = initialAngularDrag;
        }
    }

    void FixedUpdate()
    {
        if (shouldStop)
            return;

        float forwardAmount = 0f;
        float turnAmount = 0f;

        float distanceToTarget = Vector3.Distance(currentNode.transform.position, transform.position);
        if (distanceToTarget > currentNode.targetReachedBuffer)
        {
            // Set forward (or reverse) amount__________________________________

            // Check if the target is infront or behind the car
            // Dot product : if > 1 => Target is infront else behind
            Vector3 directionToMove = (currentNode.transform.position - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, directionToMove);
            // Target is in front
            if (dotProduct > 0)
            {
                // Apply brakes if within braking distance and if speed is higher than breaking speed
                if (distanceToTarget < currentNode.breakingDistance && carRigidBody.velocity.magnitude * (18 / 5) > currentNode.breakingSpeed)
                {
                    forwardAmount = -1f;
                    isBreaking = true;
                }
                else
                {
                    forwardAmount = 1;
                    isBreaking = false;
                }
            }
            // Target is behind
            else
            {
                // The car is not allowed to reverse if the distance to target is very large
                if (distanceToTarget > maxReverseDistance)
                    forwardAmount = 1;
                else
                    forwardAmount = -1;
            }

            // Set turn amount__________________________________________________

            // Calculate the angle between the car's forward direction and the direction to the target
            float angleToTarget = Vector3.SignedAngle(transform.forward, directionToMove, Vector3.up) / 1.05f;
            if (angleToTarget > 8) turnAmount = 1f; // 8 degrees of buffer is provided, to stop the car from wobbling 
            else if (angleToTarget < -8) turnAmount = -1f;
            else turnAmount = 0;
        }
        else
        {
            if (currentNode.exitNodes.Length <= 0)
            {
                shouldStop = true;
                return;
            }

            currentNode = currentNode.exitNodes[Random.Range(0, currentNode.exitNodes.Length)];
        }

        MoveCar(acceleration: forwardAmount);
        SteerCar(steering: turnAmount);
        //HandleBraking();
    }

    private void OnDrawGizmos()
    {
        Vector3 directionToMove = (currentNode.transform.position - transform.position).normalized;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, directionToMove);
    }
}
