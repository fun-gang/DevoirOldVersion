using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class Drone : MonoBehaviour
{
    private Gameplay controls = null;
    private Vector2 movement = Vector2.zero;
    private Rigidbody2D rb = null;
    private CinemachineVirtualCamera cmv;
    private GameObject player;
    private bool isActive = false;
    public int speed;

    void Start() {
        controls = new Gameplay();
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
        if (movement.x != 0) {
            if (movement.x > 0) transform.localScale = new Vector3(0,180,0);
            else transform.localScale = new Vector3(0,0,0);
        }
        rb.velocity = new Vector2(movement.x * speed * Time.fixedDeltaTime * 10, rb.velocity.y);
    }
    private void OnMovePerformed(InputAction.CallbackContext value) => movement = value.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext value) => movement = Vector2.zero;
    
    private void Repair(InputAction.CallbackContext value) {
        Debug.Log("repair");
    }

    private void OnEnable() {
        controls.Enable();
        controls.Player.Move.performed += OnMovePerformed;
        controls.Player.Move.canceled += OnMoveCanceled;
        controls.Player.Act.performed += Repair;
    }

    private void OnDisable() {
        controls.Disable();
        controls.Player.Move.performed -= OnMovePerformed;
        controls.Player.Move.canceled -= OnMoveCanceled;
    }
}