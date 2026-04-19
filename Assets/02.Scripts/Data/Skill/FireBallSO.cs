using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireBallSkill", menuName = "SO/FireBallSkillSO")]
public class FireBallSO : PlayerSkillSO
{
   [SerializeField] private int damage;
    public override void DOSkill()
    {
        GameObject fireBallObj = PoolManager.Instance.GetObject("Shooting",null,Quaternion.identity,null);
        fireBallObj.GetComponent<ShootingSkill>().InitValue(damage).Forget();

    }
}