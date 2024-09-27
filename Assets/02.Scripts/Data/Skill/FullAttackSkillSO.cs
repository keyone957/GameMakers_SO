using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "FullAttack", menuName = "SO/FullAttackSkill")]
public class FullAttackSkillSO : PlayerSkillSO
{
    public GameObject skillPrefab;
    [SerializeField] private EventChannelSO m_fullAttackSO;
    public override void DOSkill()
    {
        GameObject fullAttackEffect = Instantiate(skillPrefab , GameObject.FindWithTag("Player").transform);
        fullAttackEffect.transform.localScale = new Vector3(0.125f, 0.125f, 0.125f);
        fullAttackEffect.transform.localPosition = Vector3.zero;
        
        m_fullAttackSO.RaiseEvent();
        Destroy(fullAttackEffect,2f);
    }
}