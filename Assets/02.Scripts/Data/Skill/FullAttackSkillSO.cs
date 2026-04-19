using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(fileName = "FullAttack", menuName = "SO/FullAttackSkill")]
public class FullAttackSkillSO : PlayerSkillSO
{
    [SerializeField] private EventChannelSO fullAttackSO;
    public override void DOSkill()
    {
        GameObject fullAttackEffect=PoolManager.Instance.GetObject("FullAttack",null,quaternion.identity,GameObject.FindWithTag("Player").transform);
        fullAttackEffect.transform.localScale = new Vector3(0.125f, 0.125f, 0.125f);
        fullAttackEffect.transform.localPosition = Vector3.zero;
        
        fullAttackSO.RaiseEvent();
        DestroyEffect(fullAttackEffect).Forget();
    }

    private async UniTaskVoid DestroyEffect(GameObject effect)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2.0f));
        PoolManager.Instance.ReturnObject("FullAttack",effect);
        
    }
}