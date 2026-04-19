using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//플레이어 피격시 체력닳기

public class MonsterAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            
        }
    }
}