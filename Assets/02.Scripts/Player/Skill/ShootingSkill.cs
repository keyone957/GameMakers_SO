using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingSkill : MonoBehaviour
{
  
  public int damage;
  private Vector3 shootDirection;
  private void Update()
  {
    transform.Translate(shootDirection * 15f * Time.deltaTime);
  }

  public void InitValue(int soDamage)
  {
    GameObject player = GameObject.FindWithTag("Player");
    transform.position = player.transform.position;
    
    if (player.transform.rotation.y == 0)
    {
      shootDirection = Vector3.right;
      transform.rotation=Quaternion.Euler(0,0,180);
    }
    else
    {
      shootDirection = Vector3.right;
    }
    damage = soDamage;
    Destroy(gameObject, 8f);
  }
}
