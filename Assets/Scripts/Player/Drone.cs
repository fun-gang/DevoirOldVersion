using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Drone : MonoBehaviour
{
    private Vector2 movement = Vector2.zero;
    private Rigidbody2D rb = null;
    private CinemachineVirtualCamera cmv;
    private GameObject player;
    private bool isActive = false;
    public int speed;

    void Start() {
        rb = gameObject.GetComponent<Rigidbody2D>();
        cmv = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        player = GameObject.Find("Player");
    }
    public void Act() {
        isActive = true;
        cmv.Follow = transform;
        Controller.control = false;
    }

    public void Stop() {
        isActive = false;
        cmv.Follow = player.transform;
        Controller.control = true;
    }
    private void FixedUpdate()
    {
        if (isActive) MovePlayer();    
    }

    private void MovePlayer () {
        movement = player.GetComponent<Controller>().movement;
        if (movement.x > 0) transform.rotation = Quaternion.Euler(0, 0, 0);
        if (movement.x < 0) transform.rotation = Quaternion.Euler(0, 180, 0);
        rb.velocity = new Vector2(movement.x * speed * Time.fixedDeltaTime * 10, movement.y * speed * Time.fixedDeltaTime * 5);
    }
    
    public void Repair() {
        Debug.Log("repair");
    }
}