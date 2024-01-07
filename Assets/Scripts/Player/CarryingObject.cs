using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryingObject : MonoBehaviour
{
    private bool isCarrying = false;
    private GameObject player;
    private Transform boxPos;
    private float defaultSpeed = 0;
    private float playerSpeed = 0;

    void Start() {
        player = GameObject.Find("Player");
        boxPos = GameObject.Find("BoxPos").transform;
        defaultSpeed = player.GetComponent<Controller>().playerSpeed;
    }

    public void Act() {
        transform.parent = boxPos;
        transform.localPosition = Vector2.zero;
        player.GetComponent<Controller>().playerSpeed /= 1.5f;
    }

    public void Stop() {
        transform.parent = null;
        player.GetComponent<Controller>().playerSpeed = defaultSpeed;
    }
}
