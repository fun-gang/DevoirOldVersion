using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [SerializeField] private int coutOfBullets;
    [SerializeField] private float timeBtwShots;
    [SerializeField] private float reloadTime;
    [SerializeField] private float damage;
    
    public GameObject bullet;
    public Transform firePos;
    public GameObject[] effects;
    private bool isReady = true;
    public Sword sword;

    public void Fire(InputAction.CallbackContext value) {
        if (isReady && Movement.control && !sword.isBlock) {
            isReady = false;
            StartCoroutine(FireCor());
        }
    }

    IEnumerator FireCor() {
        for (int k = 0; k < coutOfBullets; k ++) {
            foreach (GameObject i in effects) {
                yield return new WaitForSeconds(timeBtwShots);
                Instantiate(i, firePos.position, transform.rotation);
            }
            GameObject newBullet = Instantiate(bullet, firePos.position, Quaternion.FromToRotation (Vector3.right, transform.lossyScale.x * transform.right));
            newBullet.GetComponent<PlayerBullet>().damage = damage;
        }
        yield return new WaitForSeconds(reloadTime);
        isReady = true;
    }
}
