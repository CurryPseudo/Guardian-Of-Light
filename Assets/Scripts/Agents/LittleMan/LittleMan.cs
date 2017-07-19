using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LittleMan : Agent
{
    LittleManData littleManData;
    float health = 100;
    public float Health
    {
        get
        {
            return littleManData.Health(health);
        }
        set
        {
            health = value;
        }
    }
    public float OriginalHealth
    {
        get { return health; }
    }
    #region CloserLight
    public float closerLightDistance = 1;
    public float closerLightForcePower = 1;
    public float closerLightKeepTime = 4;
    public float closerLightCoolTime = 12;
    public float closerLightKeepTimeCount = 0;
    public float closerLightCoolTimeCount = 0;
    #endregion
    #region EscapeMonster
    public float escapeMonsterRadius = 1;
    public float escapeMonsterForcePower = 2;
    #endregion
    #region Speed
    public float wanderMaxSpeed = 0.4f;
    public float escapeMonsterMaxSpeed = 1;
    public float haveBeenCatchedMaxSpeed = 0.1f;
    #endregion
    public GameObject warnPrefab = null;
    GameObject warn=null;
    List<GameObject> escapingMonsters;
    Color OriginalColor;
    public Color CoolColor;
    public void Dead()
    {
        fsm.ChangeState(FSM.State.Wander);
        gameSystem.SendMessage("LittleManCountDecline");
        Destroy(gameObject);
    }
    public bool IsCatched()
    {
        return fsm.currentState == FSM.State.HaveBeenCatched;
    }
    public void BeCatched()
    {
        fsm.ChangeState(FSM.State.HaveBeenCatched);
    }
    public void NotBeCatched()
    {
        fsm.ChangeState(FSM.State.Wander);
    }
    #region MonoBehaviour
    private void OnDestroy()
    {
        if(gameSystem!=null)
        gameSystem.GetComponent<GameSystem>().littleMans.Remove(gameObject);

    }
    void Awake()
    {
        AgentAwake();
        littleManData = Singleton<Datas>.Instance.LittleManData;
        gameSystem.GetComponent<GameSystem>().littleMans.Add(gameObject);
    }
    void Start()
    {
        gameSystem.SendMessage("LittleManCountIncrease");
        OriginalColor = GetComponent<SpriteRenderer>().color;
        CoolColor = Color.gray;
        fsm.ChangeState(FSM.State.Wander);
        closerLightKeepTimeCount = closerLightKeepTime;
    }
    void Update()
    {
        AgentUpdate();
    }
    private void FixedUpdate()
    {
        AgentFixedUpdate();
    }
    /*
    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Vector3 WanderCenter = RotateVector(wanderDistance * Vector3.right);
        Gizmos.DrawLine(transform.position, transform.position + WanderCenter);
        Gizmos.DrawWireSphere(transform.position + WanderCenter, wanderRadius);
        Gizmos.DrawLine(transform.position, transform.position + V2toV3(wanderTarget2D));
        
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        float rotateRadians = rotateAngle * Mathf.Deg2Rad;
        Vector2[] Ray = new Vector2[3];
        Ray[0] = Vector2.right;
        Ray[1] = new Vector2(Mathf.Cos(rotateRadians), Mathf.Sin(rotateRadians));
        Ray[2] = new Vector2(Mathf.Cos(-rotateRadians), Mathf.Sin(-rotateRadians));
        Vector2 force = Vector2.zero;
        for (int i = 0; i < 3; i++)
        {
            Vector2 direction = RotateVector(Ray[i]);
            Gizmos.DrawLine(transform.position, transform.position + V2toV3(direction * escapeObstacleDistance));
        }
        Gizmos.color = Color.yellow;
        if (Player)
        {
            Gizmos.DrawWireSphere(Player.transform.position, Player.GetComponent<FieldOfView>().ViewRadius);
            Gizmos.DrawWireSphere(Player.transform.position, Player.GetComponent<FieldOfView>().ViewRadius - closerLightDistance);
        }

    }
    */
    #endregion
    #region Force
    public Vector2 EscapeFromMonsters()
    {
        Vector2 forces = Vector2.zero;
        foreach (GameObject monster in escapingMonsters)
        {
            if (InView(monster, escapeMonsterRadius))
            {
                Vector2 direction = (rigidbody.position - monster.GetComponent<Rigidbody2D>().position);
                forces += EvadeGameObject(monster) * (escapeMonsterRadius - direction.magnitude) / escapeMonsterRadius;
            }
        }
        return forces.normalized * escapeMonsterForcePower;
    }
    public Vector2 CloserToLight(float time, FieldOfView fov)
    {
        float viewRadius = fov.ViewRadius;
        Vector2 direction = Agent.XY(fov.transform.position) - rigidbody.position;
        return (closerLightDistance - viewRadius + direction.magnitude) / closerLightDistance * closerLightForcePower * PursuitGameObject(fov.gameObject);
    }

    #endregion
    #region FSM
    protected override void InitRule()
    {
        FSM.State state = FSM.State.Wander;
        fsm.rule[state] = new Dictionary<FSM.StateInput, Action<float>>();
        fsm.rule[state][FSM.StateInput.Enter] = (time) =>
        {
            maxSpeed = wanderMaxSpeed;
            return;
        };
        fsm.rule[state][FSM.StateInput.Excute] = (time) =>
        {
            List<GameObject> monsters = FindGameObjectsInView("Monster", escapeMonsterRadius);
            if (monsters != null && InLight()== null)
            {
                escapingMonsters = monsters;
                fsm.ChangeState(FSM.State.EscapeFromMonster);
            }
            forces = Vector2.zero;
            forces += Wander(time);
            forces += EscapeFromObstacle(time);
            if (closerLightCoolTimeCount > 0)
            {
                closerLightCoolTimeCount -= time;
            }
            else
            {
                List<GameObject> fovs = InLight();
                if (fovs!=null)
                {
                    if (closerLightKeepTimeCount > 0)
                    {
                        closerLightKeepTimeCount -= time;
                        Vector2 CloserToLightForces = Vector2.zero;
                        foreach (GameObject fov in fovs)
                        {
                            CloserToLightForces += CloserToLight(time, fov.GetComponent<FieldOfView>());
                        }
                        CloserToLightForces /= fovs.Count;
                        forces += CloserToLightForces;

                    }
                    else
                    {
                        closerLightKeepTimeCount = closerLightKeepTime;
                        closerLightCoolTimeCount = closerLightCoolTime;
                    }
                }
            }

            return;
        };
        fsm.rule[state][FSM.StateInput.Exit] = (time) =>
        {
            GetComponent<SpriteRenderer>().color = OriginalColor;

        };
        state = FSM.State.EscapeFromMonster;
        fsm.rule[state] = new Dictionary<FSM.StateInput, Action<float>>();
        fsm.rule[state][FSM.StateInput.Enter] = (time) =>
        {
            maxSpeed = escapeMonsterMaxSpeed;
            return;
        };
        fsm.rule[state][FSM.StateInput.Excute] = (time) =>
        {
            forces = Vector2.zero;
            forces += Wander(time) * 0.5f;
            forces += EscapeFromObstacle(time);
            Vector2 escapeMonsterForce = EscapeFromMonsters();
            if (escapeMonsterForce == Vector2.zero)
            {
                fsm.ChangeState(FSM.State.Wander);
            }
            forces += escapeMonsterForce;

        };
        fsm.rule[state][FSM.StateInput.Exit] = (time) =>
        {
            escapingMonsters = null;
        };
        state = FSM.State.HaveBeenCatched;
        fsm.rule[state] = new Dictionary<FSM.StateInput, Action<float>>();
        fsm.rule[state][FSM.StateInput.Enter] = (time) =>
        {
            maxSpeed = haveBeenCatchedMaxSpeed;
            if (warnPrefab)
            {
                warnPrefab.GetComponent<StayCloseTarget>().target = GetComponent<Rigidbody2D>() ;
                warn = Instantiate(warnPrefab, transform) as GameObject;
            }
            return;
        };
        fsm.rule[state][FSM.StateInput.Excute] = (time) =>
        {
            forces = Vector2.zero;
            forces += Wander(time);
            forces += EscapeFromObstacle(time);
        };
        fsm.rule[state][FSM.StateInput.Exit] = (time) =>
        {
            if (warn)
            {
                Destroy(warn);
            }
        };

    }

    #endregion
    #region CounterAttack
    public void tryCounterAttack()
    {

        if (UnityEngine.Random.value > littleManData.counterAttack.Probability)
        {
            return;
        }
        CounterAttackLight counterAttackLight = GetComponent<CounterAttackLight>();
        counterAttackLight.StartCounterAttack(littleManData.counterAttack.viewRadius, littleManData.counterAttack.keepTime);
    }
    #endregion
    #region HandOfProtection
    public void beProtected()
    {
        GameObject lightPrefab = Resources.Load<GameObject>("SceneObjects/UnseenLight");
        lightPrefab.GetComponent<Light>().keepTime = 99999999;//= =
        lightPrefab.GetComponent<Light>().startShrinkTime = 99999999;//= =
        lightPrefab.GetComponent<Light>().originalViewRadius = Singleton<Datas>.Instance.PlayerData.light.ViewRadius / 2;
        lightPrefab.GetComponent<StayCloseTarget>().target = GetComponent<Rigidbody2D>();
        GameObject lightInstance = Instantiate<GameObject>(lightPrefab);
        
        StartCoroutine(ifDayUnprotected(lightInstance));
    }
    IEnumerator ifDayUnprotected(GameObject light)
    {
        while (true)
        {
            if (!Singleton<Datas>.Instance.isNight)
            {
                Destroy(light);
                yield break;
            }
            yield return null;
        }
    }
    #endregion

}
