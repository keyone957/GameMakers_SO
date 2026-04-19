using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "HealSkillSO", menuName = "SO/HealSkillSO")]
public class HealSO : PlayerSkillSO
{
    public override void DOSkill()
    {
        GameObject healPrefab = PoolManager.Instance.GetObject("Healing", null, Quaternion.identity, null);
        healPrefab.GetComponent<HealingSkill>().Healing().Forget();
    }
}
