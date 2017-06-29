using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Monster : Agent {

    #region MonsterGroup
    CouldChangeCollider couldChangeColliders;
    public float attackPower = 20;
    public float AttackPower
    {
        get
        {
            return attackPower * group.NowInfo.originalAttack * MutationStatus.getMutationValue("额外攻击");
        }
    }
    public bool JumpAttack
    {
        get
        {
            return MutationStatus.getMutationValue("跳劈") == 1;
        }
    }
    public MonsterGroup group;
    private MutationStatus mutationStatus;
    public void updateStatus()
    {
        if (couldChangeColliders == null) couldChangeColliders = GetComponent<CouldChangeCollider>();
        if (MutationStatus.getMutationValue("飞行") == 0) {
            ObstacleLayer = LayerMask.GetMask("Background", "HighObstacleLayer", "ObstacleLayer");
            couldChangeColliders.setCollider(0, true);
            couldChangeColliders.setCollider(1, false);
        }
        else
        {
            ObstacleLayer = LayerMask.GetMask("Background", "HighObstacleLayer");
            couldChangeColliders.setCollider(0, false);
            couldChangeColliders.setCollider(1, true);
            
        }
    }
    public void frozeDown()
    {
        fsm.ChangeState(FSM.State.Wander);
        enabled = false;
        gameObject.SetActive(false);
    }
    public void activeAgain()
    {
        enabled = true;
        gameObject.SetActive(true);
    }
    #endregion
    #region EscapeLight
    public float escapeLightPower = 3;
    #endregion
    #region Speed
    public float wanderMaxSpeed = 1;
    public float escapeMaxSpeed = 2;
    public float catchMaxSpeed = 2;
    #endregion
    #region CatchLittleMan
    public GameObject CatchingLittleMan = null;
    public float catchLittleManRadius = 1;
    public float catchLittleManPower= 3;
    public float catchedDistance = 0.1f;
    public float catchedTime = 0;
    IEnumerator catchLittleMan(float time)
    {
        bool checkCounterAttack = false;
        float timeCount=0;
        while (timeCount < time)
        {
            yield return null;
            timeCount += Time.deltaTime;
            if (timeCount > time / 2 && !checkCounterAttack)
            {
                checkCounterAttack = true;
                CatchingLittleMan.GetComponent<LittleMan>().tryCounterAttack();
            }
            if (InLight() != null)
            {
                fsm.ChangeState(FSM.State.Wander);
                yield break;
            }
            forces = Vector2.zero;
            if (CatchingLittleMan == null)
            {
                fsm.ChangeState(FSM.State.Wander);
                yield break;
            }
            rigidbody.position = CatchingLittleMan.GetComponent<Rigidbody2D>().position;
        }
        CatchingLittleMan.GetComponent<LittleMan>().Dead();
        CatchingLittleMan = null;
        fsm.ChangeState(FSM.State.Wander);
        yield break;
    }
    #endregion
    public List<GameObject> InlightList;

    public float WanderMaxSpeed
    {
        get
        {
            return wanderMaxSpeed * group.NowInfo.originalSpeed * MutationStatus.getMutationValue("额外速度");
        }
    }
    public float EscapeMaxSpeed
    {
        get
        {
            return escapeMaxSpeed * group.NowInfo.originalEscapeSpeed;
        }
    }
    public float CatchMaxSpeed
    {
        get
        {
            return catchMaxSpeed *  MutationStatus.getMutationValue("额外速度");
        }
    }

    public MutationStatus MutationStatus
    {
        get
        {
            return mutationStatus;
        }

        set
        {
            mutationStatus = value;
            if (mutationStatus != null)
            {
                updateStatus();
            }
        }
    }
    #region MonoBehaviour
    private void OnDestroy()
    {
        if(gameSystem!=null)
        gameSystem.GetComponent<GameSystem>().monsters.Remove(gameObject);
        group.monsters.Remove(this);
        fsm.ChangeState(FSM.State.Wander);
    }
    private void Awake()
    {
        AgentAwake();
        gameSystem.GetComponent<GameSystem>().AddMonster(gameObject);
        couldChangeColliders = GetComponent<CouldChangeCollider>();
    }
    void Start()
    {

        fsm.ChangeState(FSM.State.Wander);
    }
    void Update()
    {
        AgentUpdate();
    }
    private void FixedUpdate()
    {
        InlightList = InLight();
        if (InLight()!=null)
        {
            fsm.ChangeState(FSM.State.EscapeFromLight);
        }
        AgentFixedUpdate();
    }
    void OnDrawGizmos()
    {
        /*Gizmos.color = Color.black;
        Vector3 WanderCenter = RotateVector(WanderDistance * Vector3.right);
        Gizmos.DrawLine(transform.position, transform.position + WanderCenter);
        Gizmos.DrawWireSphere(transform.position + WanderCenter, WanderRadius);
        Gizmos.DrawLine(transform.position, transform.position + V2toV3(WanderTarget2D));
        */
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
        Gizmos.DrawWireSphere(rigidbody.position, catchLittleManRadius);

    }

    #endregion
    #region FSM
    protected override void InitRule()
    {
        FSM.State state = FSM.State.Wander;
        fsm.rule[state] = new Dictionary<FSM.StateInput, Action<float>>();
        fsm.rule[state][FSM.StateInput.Enter] = (time) =>
        {
            maxSpeed = WanderMaxSpeed;
            return;
        };
        fsm.rule[state][FSM.StateInput.Excute] = (time) =>
        {
            List<GameObject> littleMans = FindGameObjectsInView("LittleMan", catchLittleManRadius);
            if (littleMans != null)
            {
                float minDistance = catchLittleManRadius;
                GameObject littleMan = null;
                foreach (GameObject gameObject in littleMans)
                {
                    Vector2 direction = gameObject.GetComponent<Rigidbody2D>().position - rigidbody.position;
                    if (direction.magnitude <= minDistance && !gameObject.GetComponent<LittleMan>().IsCatched())
                    {
                        minDistance = direction.magnitude;
                        littleMan = gameObject;
                    }
                }
                if (littleMan)
                {
                    CatchingLittleMan = littleMan;
                    fsm.ChangeState(FSM.State.CatchLittleMan);
                }
            }
            forces = Vector2.zero;
            forces += Wander(time);
            forces += EscapeFromObstacle(time);
            return;
        };
        fsm.rule[state][FSM.StateInput.Exit] = (time) =>
        {
            return;
        };
        state = FSM.State.EscapeFromLight;
        fsm.rule[state] = new Dictionary<FSM.StateInput, Action<float>>();
        fsm.rule[state][FSM.StateInput.Enter] = (time) =>
        {
            maxSpeed = EscapeMaxSpeed;
            return;
        };
        fsm.rule[state][FSM.StateInput.Excute] = (time) =>
        {
            forces = Vector2.zero;
            forces += Wander(time) * 0.5f;
            forces += EscapeFromObstacle(time);
            List<GameObject> fovs = InLight();
            if (fovs == null)
            {

                StartCoroutine(DelayChangeState(FSM.State.Wander, 1.5f));
                return;
            }
            else
            {
                Vector2 EscapeFromLightForces = Vector2.zero;
                foreach (GameObject fov in fovs)
                {
                    EscapeFromLightForces += EscapeFromLight(time, fov);
                }
                EscapeFromLightForces /= fovs.Count;
                forces += EscapeFromLightForces;
            }
            return;
        };
        fsm.rule[state][FSM.StateInput.Exit] = (time) =>
        {
            return;
        };
        state = FSM.State.CatchLittleMan;
        fsm.rule[state] = new Dictionary<FSM.StateInput, Action<float>>();
        fsm.rule[state][FSM.StateInput.Enter] = (time) =>
        {
            maxSpeed = CatchMaxSpeed;
            return;
        };
        fsm.rule[state][FSM.StateInput.Excute] = (time) =>
        {

            forces = Vector2.zero;
            forces += Wander(time) * 0.5f;
            forces += EscapeFromObstacle(time);
            if (InView(CatchingLittleMan, catchLittleManRadius) && !CatchingLittleMan.GetComponent<LittleMan>().IsCatched())
            {
                forces += CatchLittleMan(time);
                if ((CatchingLittleMan.GetComponent<Rigidbody2D>().position - rigidbody.position).magnitude < catchedDistance || JumpAttack)
                {
                    CatchingLittleMan.GetComponent<LittleMan>().BeCatched();
                    fsm.ChangeState(FSM.State.HaveCatchedLittleMan);
                }
            }
            else
            {
                fsm.ChangeState(FSM.State.Wander);
            }
            return;
        };
        fsm.rule[state][FSM.StateInput.Exit] = (time) =>
        {
            return;
        };
        state = FSM.State.HaveCatchedLittleMan;
        fsm.rule[state] = new Dictionary<FSM.StateInput, Action<float>>();
        fsm.rule[state][FSM.StateInput.Enter] = (time) =>
        {
            maxSpeed = EscapeMaxSpeed;
            catchedTime = CatchingLittleMan.GetComponent<LittleMan>().Health/AttackPower;
            StartCoroutine(catchLittleMan(catchedTime));
            return;
        };
        fsm.rule[state][FSM.StateInput.Excute] = (time) =>
        {
            
        };
        fsm.rule[state][FSM.StateInput.Exit] = (time) =>
        {
            if (CatchingLittleMan)
                CatchingLittleMan.GetComponent<LittleMan>().NotBeCatched();
            return;
        };

    }
    IEnumerator DelayChangeState(FSM.State state, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        fsm.ChangeState(state);
        yield break;
    }

    #endregion
    #region Force
    public Vector2 EscapeFromLight(float time, GameObject fov)
    {
        return FleePoint(fov.GetComponent<Rigidbody2D>().position) * escapeLightPower;
    }
    public Vector2 CatchLittleMan(float time)
    {
        return SeekPoint(CatchingLittleMan.GetComponent<Rigidbody2D>().position) * catchLittleManPower;

    }

    #endregion

    #region test
    #endregion
}
