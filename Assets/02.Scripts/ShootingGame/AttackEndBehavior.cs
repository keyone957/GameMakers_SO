using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 애니메이션 끝나길 기다리는 함수
public class AttackEndBehaviour : StateMachineBehaviour
{
    [SerializeField] private Animator anim;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("IsAttack"))
        {
            var player = animator.GetComponent<Player>();
            if (player != null)
            {
                player.EndAttack();

            }
        }
    }
}