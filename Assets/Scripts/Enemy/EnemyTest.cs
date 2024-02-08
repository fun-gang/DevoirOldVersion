using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    private float health = 1f;
    private Rate rt;

    void Start() {
        rt = GameObject.FindGameObjectWithTag("Player").GetComponent<Rate>();
    }
    
    void OnTriggerEnter2D(Collider2D hit) {
        if (hit.GetComponent<Sword>() != null || hit.GetComponent<PlayerBullet>() != null) {
            health -= 0.1f;
            rt.ChangeRate(1, false);
        }
        if (health <= 0) Destroy(gameObject);
    }
}
