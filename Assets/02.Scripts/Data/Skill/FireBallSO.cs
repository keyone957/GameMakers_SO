using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireBallSkill", menuName = "SO/FireBallSkillSO")]
public class FireBallSO : PlayerSkillSO
{
    public GameObject skillPrefab;
    public int damage;

    public override void DOSkill()
    {
        GameObject fireBallObj = Instantiate(skillPrefab);
        fireBallObj.GetComponent<ShootingSkill>().InitValue(damage);

    }
}