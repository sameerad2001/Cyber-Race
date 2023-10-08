using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarHandling : MonoBehaviour
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
    float maxTorque = 300f;

    [SerializeField]
    float breakingDistance = 30f, breakingSpeed = 20f, targetReachedBuffer = 12f;

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

    [Header("Car rigid body")]
    [SerializeField]
    Rigidbody carRigidBody;

    int currentTargetIndex = 1; //! NOT 0 because at index 0 we will find the transform of the parent GameObject

    Transform[] targets;

    [SerializeField]
    GameObject targetsParentGameObject;

    void Awake()
    {
        carRigidBody.centerOfMass = centerOfMass.localPosition;

        targets = targetsParentGameObject.GetComponentsInChildren<Transform>();
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

    void FixedUpdate()
    {
        float forwardAmount = 0f;
        float turnAmount = 0f;

        float distanceToTarget = Vector3.Distance(targets[currentTargetIndex].position, transform.position);
        if (distanceToTarget > targetReachedBuffer)
        {
            // Set forward (or reverse) amount__________________________________

            // Check if the target is infront or behind the car
            // Dot product : if > 1 => Target is infront else behind
            Vector3 directionToMove = (targets[currentTargetIndex].position - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, directionToMove);
            // Target is in front
            if (dotProduct > 0)
            {
                // Apply brakes if within braking distance and if speed is higher than breaking speed
                if (distanceToTarget < breakingDistance && carRigidBody.velocity.magnitude > breakingSpeed)
                    forwardAmount = -1f;
                else
                    forwardAmount = 1;
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
            // If you want the car to keep going around the track :
            currentTargetIndex = (currentTargetIndex + 1) % targets.Length;

            if (currentTargetIndex == 0)
                currentTargetIndex = 1; // O is the index where the parent GameObject of all targets is located

            // If you want the car to stop :
            //if (currentTargetIndex == targets.Length - 1)
            //{
            //    forwardAmount = 0f;
            //    turnAmount = 0f;
            //}
            //else
            //    currentTargetIndex++;
        }

        MoveCar(acceleration: forwardAmount);
        SteerCar(steering : turnAmount );
    }

    private void OnDrawGizmos()
    {
        // Break distance
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, breakingDistance);

        // Next target
        Gizmos.color = Color.blue;
        if (targets == null || targets.Length == 0)
            return;
        Gizmos.DrawSphere(targets[currentTargetIndex].position, targetReachedBuffer);

        // To target line
        Gizmos.DrawLine(transform.position, targets[currentTargetIndex].position);
    }
}
