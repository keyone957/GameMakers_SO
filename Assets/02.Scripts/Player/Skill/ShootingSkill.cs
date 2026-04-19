using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ShootingSkill : MonoBehaviour
{
  [SerializeField] private float shotSpeed;
  [SerializeField] private float shootingDuration;
  private Vector3 shootDirection;
  public int Damage { get; private set; }
  
  private void Update()
  {
    transform.Translate(shootDirection * shotSpeed * Time.deltaTime);
  }

  public async UniTaskVoid InitValue(int soDamage)
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
    Damage = soDamage;
    await UniTask.Delay(TimeSpan.FromSeconds(shootingDuration));
    PoolManager.Instance.ReturnObject("Shooting", gameObject);
  }
}
