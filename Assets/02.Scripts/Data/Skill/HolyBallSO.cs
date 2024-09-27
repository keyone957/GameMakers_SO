using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HolyBallSkill", menuName = "SO/HolyBallSkillSO")]
public class HolyBallSO : PlayerSkillSO
{
    public GameObject skillPrefab;
    public int damage;

    public override void DOSkill()
    {
        GameObject holyBallObj = Instantiate(skillPrefab);
        holyBallObj.GetComponent<ShootingSkill>().InitValue(damage);
    }
}