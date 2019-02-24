using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHit : MonoBehaviour
{
    void OnTriggerEnter(Collider col) {
        if (col.tag == "Enemy") {
            col.gameObject.SendMessage("Hit");
        }
    }
}
