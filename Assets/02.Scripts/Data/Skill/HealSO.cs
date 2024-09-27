using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "HealSkillSO", menuName = "SO/HealSkillSO")]
public class HealSO : PlayerSkillSO
{
    public GameObject skillPrefab;
    public override void DOSkill()
    {
        GameObject healPrefab = Instantiate(skillPrefab);
        healPrefab.GetComponent<HealingSkill>().Healing();
    }
}
