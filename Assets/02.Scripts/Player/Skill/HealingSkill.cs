using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingSkill : MonoBehaviour
{
    public void Healing()
    {
        transform.parent = GameObject.FindWithTag("Player").transform;
        transform.localPosition = Vector3.zero;
        if (PlayerManager.instance.playerHp < 5)
        {
            PlayerManager.instance.playerHp += 1;
            AllSceneCanvas.instance.PlayerHPChange(PlayerManager.instance.playerHp);
        }
        Destroy(gameObject, 2.0f);
    }
}