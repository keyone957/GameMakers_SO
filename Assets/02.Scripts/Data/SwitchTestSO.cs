using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTestSO : MonoBehaviour
{
    private PlayerInputController playerInputController;
    [SerializeField] private List<PlayerSkillSO> skillList=new List<PlayerSkillSO>();
    [SerializeField] private EventChannelSO fullAttckEvent;
    private void Start()
    {
        playerInputController = FindObjectOfType<PlayerInputController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchSkill(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchSkill(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchSkill(2);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            fullAttckEvent.RaiseEvent();
        }
    }

    private void SwitchSkill(int index)
    {
        playerInputController.nextSkillTime = 0.0f;
        playerInputController.playerSkillSO = skillList[index];
    }

}
