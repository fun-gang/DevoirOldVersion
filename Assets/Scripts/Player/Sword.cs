using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sword : MonoBehaviour
{
    public Animator playerAnim;
    public GameObject[] swordSounds;
    public bool isReady;
    public bool isBlock;

    void Start() {
        isReady = true;
    }

    void FixedUpdate() => playerAnim.SetBool("isBlock", isBlock);

    public void Attack(InputAction.CallbackContext value) {
        if (isReady && Movement.control) {
            isReady = false;
            playerAnim.SetBool("IsAttack", true);
            RandomSwordSound();
        }
    }

    public void SwordAnimDisable() {
        playerAnim.SetBool("IsAttack", false);
        isReady = true;
    }

    private void RandomSwordSound() {
        int rand = Random.Range(0, swordSounds.Length);
        Instantiate(swordSounds[rand]);
    }
}
