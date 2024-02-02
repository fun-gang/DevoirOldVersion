using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] public float reloadTime;
    private bool isReady = true;
    private Transform swordPos;
    private Animator anim;
    public GameObject splash;
    public Transform splashPos;
    public GameObject[] sound;

    void Start() {
        swordPos = GameObject.FindGameObjectWithTag("SwordPos").transform;
        anim = GameObject.Find("Player").GetComponent<Animator>();
        transform.parent = swordPos;
        transform.localPosition = Vector3.zero;
    }

    public void SwordAttack() {
        if (isReady) {
            isReady = false;
            StartCoroutine(SwordCor());
        }
    }

    IEnumerator SwordCor() {
        anim.Play("SwordAttack");
        RandomSound();
        Instantiate(splash, splashPos.position, Quaternion.FromToRotation (Vector3.right, transform.lossyScale.x * transform.right));
        yield return new WaitForSeconds(reloadTime);
        isReady = true;
    }

    private void RandomSound() {
        int rand = Random.Range(0, sound.Length);
        Instantiate(sound[rand]);
    }
}
