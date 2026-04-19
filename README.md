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
<summary><h2>ScriptableObject 기반 전략 패턴으로 확장 가능한 스킬 시스템 구조 개선</h2></summary>
스킬 실행 로직이 플레이어 입력 처리 컴포넌트에 직접 구현되어 있어 플레이어가 어떤 스킬을 사용할지와 스킬이 어떻게 동작하는지에 대한 책임이 분리되지 않아 OCP와 SRP 원칙이 지켜지지 않은 상태.

그로 인해 확장성이 떨어지고 유연한 스킬 구현이 어려웠음.
<details>
<summary><code>PlayerSkillSO.cs</code></summary>
    
```csharp

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerSkillSO : ScriptableObject
{
    public float coolDown;
    public abstract void DOSkill();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireBallSkill", menuName = "SO/FireBallSkillSO")]
public class FireBallSO : PlayerSkillSO
{
   [SerializeField] private int damage;
    public override void DOSkill()
    {
        GameObject fireBallObj = 
        PoolManager.Instance.GetObject("Shooting",null,Quaternion.identity,null);
        fireBallObj.GetComponent<ShootingSkill>().InitValue(damage).Forget();
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ShootingSkill : MonoBehaviour
{
  [SerializeField] private float shotSpeed;
  [SerializeField] private float shootingDuration;
  private Vector3 shootDirection;
  public int Damage { get; private set; }
  
  private void Update()
  {
    transform.Translate(shootDirection * shotSpeed * Time.deltaTime);
  }

  public async UniTaskVoid InitValue(int soDamage)
  {
    GameObject player = GameObject.FindWithTag("Player");
    transform.position = player.transform.position;
    
    if (player.transform.rotation.y == 0)
    {
      shootDirection = Vector3.right;
      transform.rotation=Quaternion.Euler(0,0,180);
    }
    else
    {
      shootDirection = Vector3.right;
    }
    Damage = soDamage;
    await UniTask.Delay(TimeSpan.FromSeconds(shootingDuration));
    PoolManager.Instance.ReturnObject("Shooting", gameObject);
  }
}

```
</details>
스킬 로직을 플레이어로부터 분리하기 위해 전략 패턴을 Scriptable Object 기반으로 적용함.

- `PlayerSkillSO.cs` 추상 클래스를 정의하여 스킬 실행 인터페이스를 통일
- 각 스킬을 개별 전략 객체로 구현
- 플레이어 컴포넌트는 구체적인 스킬 타입을 알 필요 없이 `DOSkill()` 메서드만 호출하도록 함.
- 또한 Scriptable Object를 사용함으로써 코드 수정 없이 SO만 갈아 끼우면 여러 스킬을 사용할 수 있음.

<details>
<summary><code> FullAttackSkillSO.cs+EventChannelListener.cs </code></summary>
    
```csharp

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
        GameObject fullAttackEffect=
        PoolManager.Instance.GetObject("FullAttack",null,quaternion.identity,GameObject.FindWithTag("Player").transform);
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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventChannelListener : MonoBehaviour
{
    
    [SerializeField] private EventChannelSO _eventChannel;
    [SerializeField] private UnityEvent _response;

    private void OnEnable()
    {
        if (_eventChannel!= null)
        {
            _eventChannel.OnEventRaised += OnEventRaised;
        }
    }

    private void OnDisable()
    {
        if (_eventChannel!= null)
        {
            _eventChannel.OnEventRaised -= OnEventRaised;
        }
    }

    public void OnEventRaised()
    {
        _response?.Invoke();
    }
}



```

</details>

스킬 중 전체 공격기는 SO기반 이벤트 버스 패턴을 적용하여 여러 시스템 간 결합도를 낮춤

- `FullAttackSkillSO`는 스킬 사용시 직접적인 대상 제어 없이 이벤트만 발행
- `EventChannelSO`를 이벤트 버스로 사용하여 로직간 직접 참조 제거
- 각 몬스터 피격은 `EventChannelListener`를 통해 이벤트를 구독하고 독립적으로 반응

</details>



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

- 타이틀 씬 & 인게임 UI

<p align="left">
    <img src="https://github.com/user-attachments/assets/d346adf1-384c-4470-93bd-4d57ce325b5a" width="480" height="300"/>
    <img src="https://github.com/user-attachments/assets/04d21f02-d974-4d8d-bc92-92c01e836338" width="480" height="300"/>
</p>

⇒ 인게임에서 플레이어 상태, 스테이지, 등 필요한 UI를 배치하고 게임 도중에도 메뉴 창을 배치하였습니다. 

<p align="left">
    <img src="https://github.com/user-attachments/assets/608b7f49-d331-44dd-9ca8-bf32f67192be" width="480" height="300"/>
    <img src="https://github.com/user-attachments/assets/7ef4c6bb-c6fb-48d7-87fd-a5bb892b0603" width="480" height="300"/>
</p>

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
