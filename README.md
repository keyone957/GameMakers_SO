# 슬라임의 모험

진행 기간: 2024. 05. ~ 2024. 06.

사용한 기술 스택: C#, Unity

개발 인원(역할): 개인

한 줄 설명: 2D 메트로베니아 + 캐주얼 미니게임

비고: 개인 프로젝트

## 게임 플레이 풀 영상

---

[게임 플레이 풀영상](https://youtu.be/uiCXmkgLdgo)

## 프로젝트 소개

---

- 2D 메트로배니아 장르 게임과 캐주얼 미니게임을 섞은 게임 입니다.
<p align="left">
    <img src="https://github.com/user-attachments/assets/53264f03-a2e7-40de-97da-5bca2d70eae2" width="500" height="300"/>
    <img src="https://github.com/user-attachments/assets/79e3d24e-8745-4131-867b-542a0de2146b" width="500" height="300"/>
</p>

<p align="left">
    <img src="https://github.com/user-attachments/assets/f0bc7fcf-fb2e-421d-a292-a1b385f27a17" width="500" height="300"/>
    <img src="https://github.com/user-attachments/assets/9c4266d6-836d-4ea0-a041-5aae6f643a52" width="500" height="300"/>
</p>

## 주요 기능 구현

---

1. 게임 시스템
   - 사운드 매니저
   - 플레이어 Stat, Input 매니저
   - 세이브/로드 기능
   - 씬 매니저
   - 게임 클리어·오버 로직
   - 던전 시스템
   - 보스 스테이지 시스템
   - 재화·보상 시스템

2. 게임 플레이 관련
   - 몬스터 AI (FSM) 구현
   - 플레이어 카메라
   - 맵 오브젝트 인터렉터 (포탈, 보물상자, 보스 공격 총알)
   - NPC/상점 기능
   - 플레이어 분열 스킬·공격 구현
   - 플레이어 애니메이션
   - Scriptable Object 기반 전략 패턴 & 이벤트 채널 패턴을 이용한 스킬 구현

<details>
<summary>PlayerSkillSO.cs</summary>

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerSkillSO : ScriptableObject
{
    public float coolDown;
    public abstract void DOSkill();
}

[CreateAssetMenu(fileName = "FireBallSkill", menuName = "SO/FireBallSkillSO")]
public class FireBallSO : PlayerSkillSO
{
    public GameObject skillPrefab;
    public int damage;

    public override void DOSkill()
    {
        var fireBallObj = Instantiate(skillPrefab);
        fireBallObj.GetComponent<ShootingSkill>().InitValue(damage);
    }
}

public class ShootingSkill : MonoBehaviour
{
    public int damage;
    private Vector3 shootDirection;

    private void Update()
    {
        transform.Translate(shootDirection * 15f * Time.deltaTime);
    }

    public void InitValue(int soDamage)
    {
        var player = GameObject.FindWithTag("Player");
        transform.position = player.transform.position;

        if (player.transform.rotation.y == 0)
        {
            shootDirection = Vector3.right;
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            shootDirection = Vector3.right;
        }

        damage = soDamage;
        Destroy(gameObject, 8f);
    }
}

[CreateAssetMenu(fileName = "FullAttack", menuName = "SO/FullAttackSkill")]
public class FullAttackSkillSO : PlayerSkillSO
{
    public GameObject skillPrefab;
    [SerializeField] private EventChannelSO m_fullAttackSO;

    public override void DOSkill()
    {
        var effect = Instantiate(
            skillPrefab,
            GameObject.FindWithTag("Player").transform
        );
        effect.transform.localScale = Vector3.one * 0.125f;
        effect.transform.localPosition = Vector3.zero;

        m_fullAttackSO.RaiseEvent();
        Destroy(effect, 2f);
    }
}
```

</details>

<details>
<summary>EventChannelListener.cs</summary>

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventChannelListener : MonoBehaviour
{
    [SerializeField] private EventChannelSO m_EventChannel;
    [SerializeField] private UnityEvent m_Response;

    private void OnEnable()
    {
        if (m_EventChannel != null)
            m_EventChannel.OnEventRaised += OnEventRaised;
    }

    private void OnDisable()
    {
        if (m_EventChannel != null)
            m_EventChannel.OnEventRaised -= OnEventRaised;
    }

    public void OnEventRaised()
    {
        m_Response.Invoke();
    }
}
```

</details>

    
3. 게임 내 모든 UI / UX 기능 구현

## 개발 내용 및 플레이 영상

---

### [게임 플레이]

<table>
  <tr>
    <td align="center">
      <img src="https://github.com/user-attachments/assets/1e04e85a-6194-414d-9baa-ea4af93bd388" width="480" height="300" alt="Player Attack"/><br/>
      플레이어 공격, 피격, 피격 시 무적 판정 기능 구현
    </td>
    <td align="center">
      <img src="https://github.com/user-attachments/assets/f76bc08a-abe8-4812-b4c8-f22ad6d9bed3" width="480" height="300" alt="Skill Pattern"/><br/>
      Scriptable Object 기반 전략 패턴, 이벤트 채널 패턴을 이용한 스킬 구현.
    </td>
  </tr>
</table>

<table>
  <tr>
    <td align="center">
      <img src="https://github.com/user-attachments/assets/362b4886-562a-49b1-b3aa-7c40c480a487" width="480" height="300" alt="Player Attack"/><br/>
    플레이어 분열 스킬을 사용하면 이동 속도 증가 및 공격력은 내려가지만 맵 내 일반 상태 일 때는 들어가지 못하는 공간에 들어갈 수 있다 
    </td>
  </tr>
</table> 

### **[미니게임]**

<p align="left">
    <img src="https://github.com/user-attachments/assets/65b90750-a7cd-4a65-8147-eb830f88cb2d" width="480" height="300"/>
    <img src="https://github.com/user-attachments/assets/fc3fa35e-c0fc-40af-a4a5-22be2d7d6944" width="480" height="300"/>
</p>

⇒ 일반 던전 맵 중간에 점프 맵, 코인 먹기의 미니게임을 클리어하여 보너스 보상을 얻을 수 있습니다

<p align="left">
    <img src="https://github.com/user-attachments/assets/11c90670-8366-459a-b9bb-89bc595d0268" width="480" height="300"/>
    <img src="https://github.com/user-attachments/assets/04c7053e-0f20-427c-8b7a-26ae87f12f62" width="480" height="300"/>
</p>


⇒ 마지막 보스에서는 날아오는 투사체들을 피하면서 보스를 물리쳐, 최종 보상을 얻을 수 있습니다.

### **[UI / UX]**

- 타이틀 씬

![image.png](image%204.png)

![image.png](image%205.png)

=⇒ 메인 화면에서 게임 시작, 환경 설정, 끝내기 버튼을 배치하였습니다.

- 인게임 UI

![image.png](image%206.png)

![image.png](image%207.png)

=⇒ 인게임에서 플레이어 상태, 스테이지, 등 필요한 UI를 배치하고 게임 도중에도 메뉴 창을 배치하였습니다. 

![image.png](image%208.png)

![image.png](image%209.png)

=⇒ Scriptable Object를 이용하여 상점 기능을 구현하였습니다.

## 프로젝트 사용기술

---

### ⚒️ 클라이언트

- Unity
- C#

### ⚒️ 버전 관리 및 협업

- Git
- Notion

### ⚒️ 개발 환경

- Visual Studio
