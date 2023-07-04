using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    public float speed = 5;

    void Start()
    {
    }

    void Update()
    {
        var cameraPosition = transform.position;
        var playerPosition = player.transform.position;
        var targetPosition = Vector3.Lerp(cameraPosition, playerPosition, Time.deltaTime * speed);
        targetPosition.z = cameraPosition.z;

        transform.position = targetPosition;
    }
}