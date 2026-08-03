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
<summary><h2>Scriptable Object 기반 전략 패턴을 활용한 스킬 실행 구조 개선</h2></summary>

<h3>문제 상황</h3>
초기에는 스킬 실행 로직이 플레이어 입력 컴포넌트에 결합되어 신규 스킬 추가 시 기존 입력 코드까지 수정해야 했으며, 입력 처리와 구체적인 스킬 동작의 책임도 분리되지 않은 상태.

또한 전체 공격이 씬의 몬스터를 직접 탐색하면 스킬과 몬스터 사이의 의존성이 높아지는 문제가 있었음.

<h3>해결 1. PlayerSkillSO 전략 적용</h3>

스킬의 공통 쿨타임과 스킬 실행을 추상 <code>PlayerSkillSO</code>로 정의하고, 각 스킬이 자신의 실행 로직을 구현하도록 구성
    
```csharp

public abstract class PlayerSkillSO : ScriptableObject
{
    [SerializeField] private float coolDown;
    public float CoolDown => coolDown;
    public abstract void DOSkill();
}

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

```

<code>PlayerInputController</code>는 구체적인 스킬 타입을 판별하지 않고 현재 장착된 PlayerSkillSO의 실행 메서드와 쿨타임만 사용

```csharp

private void UseSkill()
{
    if (Time.time < nextSkillTime)
        return;

    playerSkillSO.DOSkill();
    nextSkillTime = Time.time + playerSkillSO.CoolDown;
}

```

- 기존 스킬은 SO 참조만 교체하여 사용
- 신규 스킬은 구현체와 SO 에셋만 추가하고 기존 `UseSkill()`은 재사용
- 스킬 호출부의 조건 분기를 제거하여 코드 수정 범위 최소화
- 스킬 호출부에 OCP를 적용하고 실행 흐름의 책임을 분리

<h3>해결 2. EventChannelSO로 전체 공격의 직접 참조 제거</h3>

전체 공격은 개별 몬스터를 직접 탐색하거나 참조 하지 않고 <code>EventChannelSO</code>를 통해 공격 이벤트만 발행하도록 구성

```csharp

[CreateAssetMenu(menuName = "SO/Events")]
public class EventChannelSO : ScriptableObject
{
    public event UnityAction OnEventRaised;

    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}

```

```csharp

public class FullAttackSkillSO : PlayerSkillSO
{
    [SerializeField]
    private EventChannelSO fullAttackChannel;

    public override void DOSkill()
    {
        // 전체 공격 이펙트 처리 생략
        fullAttackChannel.RaiseEvent();
    }
}

```
각 필드 몬스터에 연결된 <code>EventChannelListener</code>는 이벤트를 구독하고 이벤트가 발생하면 등록된 <code>FullDamaged()</code>를 실행함.

```csharp

public class EventChannelListener : MonoBehaviour
{
    [SerializeField] private EventChannelSO eventChannel;
    [SerializeField] private UnityEvent response;

    private void OnEnable()
    {
        if (eventChannel != null)
            eventChannel.OnEventRaised += OnEventRaised;
    }

    private void OnDisable()
    {
        if (eventChannel != null)
            eventChannel.OnEventRaised -= OnEventRaised;
    }

    private void OnEventRaised()
    {
        response?.Invoke();
    }
}

```

이를 통해 전체 공격 스킬은 이벤트 발행만 담당하고 실제 피격 처리는 각 몬스터가 담당하도록 책임을 분리함.
 
- 전체 공격 스킬과 개별 몬스터 사이의 직접 참조 제거
- 신규 몬스터도 Listener 연결만으로 전체 공격 대상에 추가 가능


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
