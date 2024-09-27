using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerSkillSO : ScriptableObject
{
    public float coolDown;
    public abstract void DOSkill();
}
