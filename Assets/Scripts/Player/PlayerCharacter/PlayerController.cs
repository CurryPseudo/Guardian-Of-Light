using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class PlayerController : MonoBehaviour
{
    PlayerData playerData;
    SkillCharges skillCharges;
    SkillChargeData skillChargeData;
    UpgradeList upgradeList;
    SkillTable skillTable;
    GameSystem gameSystem;
    Joystick joystick;
    CouldChangeCollider couldChangeColliders;
    #region MonoBehaviour
    private void Awake()
    {
        playerData = Singleton<Datas>.Instance.PlayerData;
        skillCharges = GetComponent<SkillCharges>();
        upgradeList = Singleton<Datas>.Instance.UpgradeList;
        skillChargeData = Singleton<Datas>.Instance.SkillChargeData;
        skillTable = Singleton<Datas>.Instance.SkillTable;
        gameSystem = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        joystick = GameObject.Find("JoystickBackground").GetComponent<Joystick>();
        couldChangeColliders = GetComponent<CouldChangeCollider>();
        fov = GetComponent<FieldOfView>();
        fov.nowMask = 1 << LayerMask.NameToLayer("HighObstacleLayer") | 1 << LayerMask.NameToLayer("ObstacleLayer") | 1 << LayerMask.NameToLayer("Background");
        if (lightPrefab == null) lightPrefab = Resources.Load("SceneObjects/LittleLight") as GameObject;
    }
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        InitRule();
        InitRuleUp();
        fsm.ChangeState(FSM.State.Normal);
        fsmUp.ChangeState(FSM.State.Normal);
        originalScale = transform.localScale.x;
        skillChargesInit();
    }
    void Update()
    {
        if (joystick.Horizontal != 0 || joystick.Vertical != 0)
        {
            velocity = new Vector2(joystick.Horizontal, joystick.Vertical).normalized * moveSpeed;
        }
        else
        {
            velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * moveSpeed;

        }

    }
    void FixedUpdate()
    {
        skillCharges.findSkillCharge("保护之手").ifButtonPushedUseCharge();
        skillCharges.findSkillCharge("怪物感应").ifButtonPushedUseCharge();
        skillCharges.findSkillCharge("召唤圣光").ifButtonPushedUseCharge();
        skillCharges.findSkillCharge("杀死怪物").ifButtonPushedUseCharge();
        fsmUp.Update(Time.fixedDeltaTime);
        fsm.Update(Time.fixedDeltaTime);

        rigidbody2d.MovePosition(rigidbody2d.position + velocity * Time.fixedDeltaTime);
    }

    #endregion
    #region FSM
    protected FSM fsm = new FSM();
    FSM fsmUp = new FSM();
    public void RefleshUpgrades()
    {
        NormalViewRadiusInit = false;
        ChangeState(FSM.State.Normal);

    }
    public void ChangeState(FSM.State state)
    {
        fsm.ChangeState(state);
    }
    public void ChangeUpState(FSM.State state)
    {
        fsmUp.ChangeState(state);
    }
    protected void InitRule()
    {
        FSM.State state = FSM.State.Normal;
        fsm.rule[state] = new Dictionary<FSM.StateInput, Action<float>>();
        fsm.rule[state][FSM.StateInput.Enter] = (time) =>
        {
            moveSpeed = playerData.physic.MoveSpeed;
            if (!NormalViewRadiusInit)
            {
                fov.ViewRadius = playerData.light.ViewRadius;
                NormalViewRadiusInit = true;
            }
            expandLightTimeCount = 0;
            return;
        };
        fsm.rule[state][FSM.StateInput.Excute] = (time) =>
        {
            skillCharges.findSkillCharge("飞跃").ifButtonPushedUseCharge();
            expandLightTimeCount += Time.fixedDeltaTime;
            if (!fov.IsLeaping)
            {
                fov.ViewRadius = doExpandLight(playerData.light.ViewRadius);
            }
        };
        fsm.rule[state][FSM.StateInput.Exit] = (time) =>
        {
            return;
        };
        state = FSM.State.Leap;
        fsm.rule[state] = new Dictionary<FSM.StateInput, Action<float>>();
        fsm.rule[state][FSM.StateInput.Enter] = (time) =>
        {
            moveSpeed = playerData.leap.MoveSpeed;
            fov.ObstacleMask = (playerData.leap.LeapHighObstacles ? 0 : 1 << LayerMask.NameToLayer("HighObstacleLayer")) | 1 << LayerMask.NameToLayer("Background");
            fov.setViewRadiusLeaply(playerData.leap.ViewRadius, playerData.leap.AnimateDelayTime);
            StartCoroutine(DelayChangeCollider());
            StartCoroutine(LinerChangeScale(leapScale * originalScale));
            StartCoroutine(DelayChangeState(FSM.State.Normal, playerData.leap.KeepTime, fsm));
            StartCoroutine(DelayLeapAnimate());
            return;
        };
        fsm.rule[state][FSM.StateInput.Excute] = (time) =>
        {
            return;
        };
        fsm.rule[state][FSM.StateInput.Exit] = (time) =>
        {
            

            return;
        };
    }
    IEnumerator DelayChangeCollider()
    {
        float timeCount = 0;
        while (timeCount < playerData.leap.AnimateDelayTime)
        {
            timeCount += Time.deltaTime;
            yield return null;
        }
        couldChangeColliders.setCollider(0, false);
        couldChangeColliders.setCollider(1, !playerData.leap.LeapHighObstacles);
    }
    IEnumerator DelayLeapAnimate()
    {
        float timeCount = 0;
        while (true)
        {
            timeCount += Time.deltaTime;
            if (timeCount > playerData.leap.keepTime - playerData.leap.AnimateDelayTime) break;
            yield return null;
        }
        couldChangeColliders.setCollider(0, true);
        couldChangeColliders.setCollider(1, false);
        fov.ObstacleMask = 1 << LayerMask.NameToLayer("HighObstacleLayer") | 1 << LayerMask.NameToLayer("ObstacleLayer") | 1 << LayerMask.NameToLayer("Background");
        fov.setViewRadiusLeaply(playerData.light.ViewRadius, playerData.leap.AnimateDelayTime);
        StartCoroutine(LinerChangeScale(originalScale));
        yield break;
    }
    void InitRuleUp()
    {
        FSM.State state = FSM.State.Normal;
        fsmUp.rule[state] = new Dictionary<FSM.StateInput, Action<float>>();
        fsmUp.rule[state][FSM.StateInput.Enter] = (time) =>
        {

        };
        fsmUp.rule[state][FSM.StateInput.Excute] = (time) =>
        {
            skillCharges.findSkillCharge("光球").ifButtonPushedUseCharge();
        };
        fsmUp.rule[state][FSM.StateInput.Exit] = (time) =>
        {
            return;
        };
        state = FSM.State.WaitPutLight;
        fsmUp.rule[state] = new Dictionary<FSM.StateInput, Action<float>>();
        fsmUp.rule[state][FSM.StateInput.Enter] = (time) =>
        {
            StartCoroutine(DelayChangeState(FSM.State.Normal, 0.5f, fsmUp));
        };
        fsmUp.rule[state][FSM.StateInput.Excute] = (time) =>
        {
            return;
        };
        fsmUp.rule[state][FSM.StateInput.Exit] = (time) =>
        {
            return;
        };
        state = FSM.State.WaitForUpgrade;
        fsmUp.rule[state] = new Dictionary<FSM.StateInput, Action<float>>();
        fsmUp.rule[state][FSM.StateInput.Enter] = (time) =>
        {
            fov.lightEnable = false;
        };
        fsmUp.rule[state][FSM.StateInput.Excute] = (time) =>
        {
            return;
        };
        fsmUp.rule[state][FSM.StateInput.Exit] = (time) =>
        {
            fov.lightEnable = true;
            return;
        };
    }
    IEnumerator DelayChangeState(FSM.State state, float delayTime, FSM fsm)
    {
        yield return new WaitForSeconds(delayTime);
        fsm.ChangeState(state);
        yield break;
    }

    #endregion
    #region Leap
    float originalScale = 0;
    public float leapScale = 1.3f;
    IEnumerator LinerChangeScale(float value)
    {
        float now = transform.localScale.x;
        float timeCount = 0;
        while (timeCount <= playerData.leap.AnimateDelayTime)
        {
            timeCount += Time.deltaTime;
            transform.localScale = new Vector3(1, 1, 1) * (now + (timeCount / playerData.leap.AnimateDelayTime) * (value - now));
            yield return null;
        }
        yield break;
    }

    #endregion
    # region PutLight
    public GameObject lightPrefab;
    void GenerateALight()
    {
        lightPrefab.GetComponent<Light>().keepTime = playerData.putLight.KeepTime;
        lightPrefab.GetComponent<Light>().startShrinkTime = playerData.putLight.StartShrinkTime;
        lightPrefab.GetComponent<Light>().originalViewRadius = playerData.putLight.ViewRadius;
        Instantiate(lightPrefab, transform.position, transform.rotation);
    }
    bool notInObstacle()
    {
        Vector2[] direction = { Vector2.right, Vector2.down, Vector2.left, Vector2.up };
        for (int i = 0; i < 4; i++)
        {
            Vector2 origin = rigidbody2d.position + direction[i] * originalScale / 2 * 0.8f;
            float distance = 0.01f;
            RaycastHit2D result = Physics2D.Raycast(origin, direction[i], distance, 1 << LayerMask.NameToLayer("HighObstacleLayer") | 1 << LayerMask.NameToLayer("ObstacleLayer"));
            if (result)
            {
                return false;
            }
        }
        return true;
    }
    #endregion
    #region Physic
    public float moveSpeed = 3;
    Rigidbody2D rigidbody2d;
    public Vector2 velocity;
    #endregion
    #region FOV
    public FieldOfView fov;
    bool NormalViewRadiusInit = false;
    public float expandLightTimeCount = 0;
    public float doExpandLight(float originalViewRadius)
    {
        if (playerData.light.ExpandDegree == 1)
        {
            return originalViewRadius;
        }
        expandLightTimeCount = expandLightTimeCount % playerData.light.ExpandTime;
        float degree = playerData.light.ExpandValueDuringTime(expandLightTimeCount / playerData.light.ExpandTime);
        float expandDegree = Mathf.Lerp(1, playerData.light.ExpandDegree, degree);
        return expandDegree * originalViewRadius;

    }
    #endregion
    #region skillCharge
    public void skillChargesInit()
    {
        skillCharges.findSkillCharge("飞跃").skillEvent += () =>
        {
            fsm.ChangeState(FSM.State.Leap);
        };
        skillCharges.findSkillCharge("光球").skillEvent += () =>
        {
            if (!notInObstacle())
            {
                skillCharges.findSkillCharge("光球").revokeUse();
                return;
            }
            GenerateALight();
            fsmUp.ChangeState(FSM.State.WaitPutLight);
        };
        skillCharges.findSkillCharge("召唤圣光").skillEvent += () =>
        {
            GenerateSummonALight();
        };
        skillCharges.findSkillCharge("杀死怪物").skillEvent += () =>
        {
            if (!doKillMonster())
            {
                skillCharges.findSkillCharge("杀死怪物").revokeUse();
                return;
            }

        };
        skillCharges.findSkillCharge("怪物感应").skillEvent += () =>
        {
            doMonsterInduction();
        };
        skillCharges.findSkillCharge("保护之手").skillEvent += () =>
        {
            doHandOfProtection();
        };
    }
    #endregion
    #region SummonTheLight
    void GenerateSummonALight()
    {
        GameObject summonLightPrefab = Resources.Load<GameObject>("SceneObjects/UnseenLight");
        summonLightPrefab.GetComponent<Light>().keepTime = playerData.summonTheLight.KeepTime;
        summonLightPrefab.GetComponent<Light>().startShrinkTime = playerData.summonTheLight.StartShrinkTime;
        summonLightPrefab.GetComponent<Light>().originalViewRadius = playerData.summonTheLight.ViewRadius;
        summonLightPrefab.GetComponent<StayCloseTarget>().target = GetComponent<Rigidbody2D>();
        summonLightPrefab.GetComponent<FieldOfView>().nowMask = 1 << LayerMask.NameToLayer("Background");
        Instantiate(summonLightPrefab, transform.position, transform.rotation);
    }
    #endregion
    #region KillMonster
    public List<GameObject> getObjectsInView(float viewRadius, LayerMask objectLayer)
    {
        List<GameObject> monsters = gameSystem.monsters;
        List<GameObject> objects = new List<GameObject>();
        foreach (GameObject monster in monsters)
        {
            Monster _monster = monster.GetComponent<Monster>();
            if (_monster.InLight() != null)
            {
                objects.Add(monster);
            }
        }
        return objects;
    }
    public bool doKillMonster()
    {
        List<GameObject> monsters = getObjectsInView(playerData.light.ViewRadius, 1 << LayerMask.NameToLayer("Monster"));
        if (monsters.Count == 0) return false;
        int length = monsters.Count;
        int index = (int)UnityEngine.Random.value * length;
        Destroy(monsters[index]);
        return true;
    }
    #endregion
    #region MonsterInduction
    List<GameObject> inductWarns = new List<GameObject>();
    public void doMonsterInduction()
    {
        startSeen();
        GameSystem.delayDoSomething(this, playerData.monsterInduction.KeepTime, unableSeen);
        //StartCoroutine(checkMonsterInLight());
    }
    IEnumerator checkMonsterInLight()
    {
        while (true)
        {
            foreach (GameObject warn in inductWarns)
            {
                if (warn.GetComponent<StayCloseTarget>().GetComponent<Monster>().InLight() != null)
                {
                    inductWarns.Remove(warn);
                    Destroy(warn);
                }
            }
            if (inductWarns.Count == 0) yield break;
            yield return null;
        }

    }
    public void startSeen()
    {
        GameObject prefab = Resources.Load<GameObject>("SceneObjects/MonsterWarn");
        List<GameObject> monsters = gameSystem.monsters;
        for (int i = 0; i < monsters.Count; i++)
        {
            prefab.GetComponent<StayCloseTarget>().target = monsters[i].GetComponent<Rigidbody2D>();
            GameObject instance = Instantiate<GameObject>(prefab);
            inductWarns.Add(instance);
        }
    }
    public void unableSeen()
    {
        foreach (GameObject warn in inductWarns)
        {
            Destroy(warn);
        }
        inductWarns.Clear();
    }
    #endregion
    #region HandOfProtection
    void doHandOfProtection()
    {
        List<GameObject> littleMans = gameSystem.littleMans;
        int randIndex = (int)UnityEngine.Random.value * littleMans.Count;
        littleMans[randIndex].GetComponent<LittleMan>().beProtected();
    }
    #endregion
}
