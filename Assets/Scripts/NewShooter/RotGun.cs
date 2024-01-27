using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotGun : MonoBehaviour
{
    public GameObject bullet;
    public Transform fireOrigin;
    public float reloadTime = 1.1f;
    public GameObject fireSound;
    private Transform gunPos;

    void Start() {
        gunPos = GameObject.Find("GunPos").transform;
    }

    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, gunPos.position, 1000);
    }

    public void Fire() {
        Instantiate(fireSound);
        Instantiate(bullet, fireOrigin.position, Quaternion.FromToRotation (Vector3.right, transform.lossyScale.x * transform.right));
    }
}
