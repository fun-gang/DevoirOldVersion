using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour {
    public GameObject bullet;
    public Transform fireOrigin;
    public float reloadTime;
    private bool isReady = true;
    public GameObject fireSound;
    private bool isPicked = false;
    private Transform player;
    public Transform origin;
    public LayerMask groundLayer;


    void Start() {
        player = GameObject.Find("GunPos").transform;
    }

    public void Act() {
        if (isPicked) {
                if (isReady && Controller.control) {
                isReady = false;
                StartCoroutine(Reload());
            }
        }
        else {
            transform.parent = player;
            transform.localPosition = Vector3.zero;
            isPicked = true;
        }
    }

    public void Stop() {
        transform.parent = null;
        isPicked = false;

        RaycastHit2D ray;
        ray = Physics2D.Raycast(origin.position, Vector2.down, 100, groundLayer); 
        if (ray != null) {
            transform.position = new Vector3(transform.position.x, transform.position.y - ray.distance, 0);
        }
    }

    IEnumerator Reload() {
        Instantiate(fireSound);
        Instantiate(bullet, fireOrigin.position, Quaternion.FromToRotation (Vector3.right, transform.lossyScale.x * transform.right));
        yield return new WaitForSeconds(reloadTime);
        isReady = true;
    }
}