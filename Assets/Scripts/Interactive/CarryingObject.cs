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
    private BoxCollider2D boxColl;
    public LayerMask groundLayer;
    public Transform origin;
    private CircleCollider2D ccol;

    void Start() {
        player = GameObject.Find("Player");
        boxPos = GameObject.Find("BoxPos").transform;
        defaultSpeed = player.GetComponent<Controller>().playerSpeed;

        boxColl = gameObject.GetComponent<BoxCollider2D>();

        boxColl.isTrigger = true;
        ccol = gameObject.GetComponent<CircleCollider2D>();
    }

    public void Act() {
        player.GetComponent<Controller>().playerSpeed /= 2.5f;
        transform.parent = boxPos;
        transform.localPosition = Vector2.zero;
        boxColl.isTrigger = false;
        ccol.enabled = true;
    }

    public void Stop() {
        transform.parent = null;
        player.GetComponent<Controller>().playerSpeed = defaultSpeed;
        boxColl.isTrigger = true;

        RaycastHit2D ray;
        ray = Physics2D.Raycast(origin.position, Vector2.down, 100, groundLayer); 
        if (ray != null) {
            transform.position = new Vector3(transform.position.x, transform.position.y - ray.distance, 0);
            ccol.enabled = false;
        }
    }
}
