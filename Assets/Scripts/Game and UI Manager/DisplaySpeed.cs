using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplaySpeed : MonoBehaviour
{

    TextMeshProUGUI speedText;
    Rigidbody playerCar;

    [SerializeField] string playerCarTag;

    [SerializeField]
    [Range(1, 3)]
    [Tooltip("If your car is slow and you want the player to feel like they are going fast, display a fake increased speed")]
    float fakeSpeedMultiplier = 1.5f;

    void Awake()
    {
        speedText = gameObject.GetComponent<TextMeshProUGUI>();
        playerCar = GameObject.FindGameObjectWithTag(playerCarTag).GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        speedText.text = Mathf.FloorToInt(playerCar.velocity.magnitude * (18 / 5) * fakeSpeedMultiplier) + " km/h";
    }
}
