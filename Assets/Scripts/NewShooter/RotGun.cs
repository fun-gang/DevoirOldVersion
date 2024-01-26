using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotGun : MonoBehaviour
{
    public GameObject bullet;
    public Transform fireOrigin;
    public float reloadTime = 1.1f;
    public GameObject fireSound;

    public void Fire() {
        Instantiate(fireSound);
        Instantiate(bullet, fireOrigin.position, Quaternion.FromToRotation (Vector3.right, transform.lossyScale.x * transform.right));
    }
}
