using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private Rate rt;

    void Start() => rt = GetComponent<Rate>();

    void OnTriggerEnter2D(Collider2D hit) {
        if (hit.tag == "Hit1" || hit.tag == "Hit2") rt.ChangeRate(0, true);
    } 
}
