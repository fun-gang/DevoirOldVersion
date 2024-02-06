using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sword : MonoBehaviour
{
    public Animator playerAnim;
    public GameObject[] swordSounds;
    private bool isReady;

    void Start() {
        isReady = true;
    }

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
