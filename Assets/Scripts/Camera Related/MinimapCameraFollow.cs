using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    [SerializeField]
    Transform player;

    [SerializeField]
    [Range(10, 300)]
    float cameraHeight = 180f;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.position.x, cameraHeight, player.position.z);
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}
