using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class Agent : MonoBehaviour {
    protected FSM fsm = new FSM();
    public GameObject Player;
    public Rigidbody2D rigidbody;
    protected Vector2 forces = new Vector2(0, 0);
    #region Wander
    public float wanderDistance = 2;
    public float wanderRadius = 1;
    public float wanderRandomMoveDistance = 0.3f;
    public float wanderForcePower = 1;
    protected Vector2 WanderPoint;
    #endregion
    #region EscapeObstacle
    public float escapeObstacleDistance = 1;
    public float escapeObstacleForcePower = 1;
    public int escapeRaycastTimes = 20;
    #endregion
    protected float maxSpeed = 2;
    const float PursuitDistance = 0.5f;
    public LayerMask ObstacleLayer;
    protected GameObject gameSystem;
    public float MaxSpeed
    {
        get { return maxSpeed; }
    }
    public Vector2 wanderTarget2D;
    public float rotateAngle = 40;
    // Use this for initialization
    public List<GameObject> InLight()
    {
        return gameSystem.GetComponent<FovSolve>().InLight(rigidbody.position);
    }
    protected void AgentAwake()
    {
        Player = GameObject.Find("Player");
        gameSystem = GameObject.Find("GameSystem");
        InitRule();
        rigidbody = GetComponent<Rigidbody2D>();
        WanderPoint = new Vector2(wanderRadius, 0);

    }
    protected void AgentUpdate()
    {

    }
    protected void AgentFixedUpdate()
    {
        fsm.Update(Time.fixedDeltaTime);
        rigidbody.AddForce(forces);
        if (rigidbody.velocity.magnitude > maxSpeed)
        {
            rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
        }
    }
    protected abstract void InitRule();
    // Update is called once per frame
    void FixedUpdate()
    {
        fsm.Update(Time.fixedDeltaTime);
        rigidbody.AddForce(forces);
        if (rigidbody.velocity.magnitude > maxSpeed)
        {
            rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
        }
    }
    public void ForceClear()
    {
        forces = new Vector2(0, 0);
    }
    public static Vector2 XY(Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.y);
    }
    public static Vector3 V2toV3(Vector2 vector, float z = 0)
    {
        return new Vector3(vector.x, vector.y, z);
    }
    public float GetVelocityAngle()
    {
        Vector2 velocity = rigidbody.velocity.normalized;
        float angle = Vector2.Angle(Vector2.right, velocity);
        float sign = Mathf.Sign(velocity.y);
        return sign * angle;
    }
    public Vector3 RotateVector(Vector3 vector)
    {

        return Quaternion.AngleAxis(GetVelocityAngle(), Vector3.forward) * vector;
    }
    public Vector2 RotateVector(Vector2 vector)
    {
        Vector3 vector3D = V2toV3(vector);
        vector3D = Quaternion.AngleAxis(GetVelocityAngle(), Vector3.forward) * vector3D;
        return XY(vector3D);
    }
    public Vector2 Wander(float time)
    {
        float randomAngle = UnityEngine.Random.value * Mathf.PI * 2;
        WanderPoint += new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * wanderRandomMoveDistance * time;
        WanderPoint = WanderPoint.normalized * wanderRadius;
        wanderTarget2D = RotateVector(WanderPoint + wanderDistance * Vector2.right);
        Vector2 force = wanderTarget2D.normalized * wanderForcePower;
        if (force == Vector2.zero)
        {
            return Vector2.right * wanderForcePower;
        }
        return force;
    }

    public Vector2 EscapeFromObstacle(float time)
    {
        float rotateRadians = rotateAngle * Mathf.Deg2Rad;
        Vector2[] Ray = new Vector2[3];
        Ray[0] = Vector2.right;
        Ray[1] = new Vector2(Mathf.Cos(rotateRadians), Mathf.Sin(rotateRadians));
        Ray[2] = new Vector2(Mathf.Cos(-rotateRadians), Mathf.Sin(-rotateRadians));
        Vector2 force = Vector2.zero;
        for (int i = 0; i < 3; i++)
        {
            Vector2 direction = RotateVector(Ray[i]);
            RaycastHit2D result = Physics2D.Raycast(rigidbody.position, direction, escapeObstacleDistance, ObstacleLayer);
            if (result)
            {
                force += result.normal.normalized * (escapeObstacleDistance - result.distance) / escapeObstacleDistance;
            }
        }
        return force.normalized * escapeObstacleForcePower;
    }
    public bool InView(GameObject gameObject, float viewRadius)
    {
        if (gameObject == null) return false;
        Vector2 direction = gameObject.GetComponent<Rigidbody2D>().position - rigidbody.position;
        float distance = direction.magnitude;
        if (distance > viewRadius)
        {
            return false;
        }
        RaycastHit2D result = Physics2D.Raycast(rigidbody.position, direction.normalized, viewRadius, ObstacleLayer);
        if (result) return false;
        return true;
    }
    public List<GameObject> FindGameObjectsInView(String layerName, float findRadius)
    {
        Collider2D[] GameObjects = Physics2D.OverlapCircleAll(rigidbody.position, findRadius, 1 << LayerMask.NameToLayer(layerName));
        if (GameObjects.Length == 0) return null;
        List<GameObject> GameObjectList = new List<GameObject>();
        foreach (Collider2D GameObject in GameObjects)
        {
            if (InView(GameObject.gameObject, findRadius))
            {
                GameObjectList.Add(GameObject.gameObject);
            }
        }
        if (GameObjectList.Count == 0) return null;
        return GameObjectList;

    }
    public Vector2 SeekPoint(Vector2 Point)
    {
        Vector2 targetVelocity = (Point - rigidbody.position).normalized * maxSpeed;
        Vector2 targetForce = (targetVelocity - rigidbody.velocity) / maxSpeed;
        return targetForce;
    }
    public Vector2 PursuitGameObject(GameObject gameObject)
    {
        if (gameObject == null) return Vector2.zero;
        Vector2 gameObjectPosition = gameObject.GetComponent<Rigidbody2D>().position;
        if (Vector2.Dot(rigidbody.velocity.normalized, (gameObjectPosition  - rigidbody.position).normalized) >Mathf.Cos(Mathf.Deg2Rad * 20) || (gameObjectPosition - rigidbody.position).magnitude < PursuitDistance)
            return SeekPoint(gameObjectPosition);
        float arriveTime = (gameObjectPosition - rigidbody.position).magnitude / maxSpeed;
        Vector2 targetPoint = gameObject.GetComponent<Rigidbody2D>().velocity * arriveTime + gameObjectPosition;
        return SeekPoint(targetPoint);
    }
    public Vector2 EvadeGameObject (GameObject gameObject)
    {
        if (gameObject == null) return Vector2.zero;
        Vector2 gameObjectPosition = gameObject.GetComponent<Rigidbody2D>().position;
        if (Vector2.Dot(rigidbody.velocity.normalized, (gameObject.GetComponent<Rigidbody2D>().position - rigidbody.position).normalized) >Mathf.Cos(Mathf.Deg2Rad * 20))
            return FleePoint(gameObjectPosition);
        float arriveTime = 0;
        if (gameObject.tag == "Light")
        {
            arriveTime = (gameObjectPosition - rigidbody.position).magnitude / gameObject.GetComponent<PlayerController>().moveSpeed;
        }
        else
        {
            arriveTime = (gameObjectPosition - rigidbody.position).magnitude / gameObject.GetComponent<Agent>().MaxSpeed;
        }
        Vector2 targetPoint = gameObject.GetComponent<Rigidbody2D>().velocity * arriveTime + gameObjectPosition;
        return FleePoint(targetPoint);
    }
    public Vector2 StraightCatch(GameObject gameObject)
    {
        if (gameObject == null) return Vector2.zero;
        Vector2 direction = (gameObject.GetComponent<Rigidbody2D>().position - rigidbody.position);
        return direction;
    }
    public Vector2 FleePoint (Vector2 Point)
    {
        Vector2 targetVelocity = (rigidbody.position - Point).normalized * maxSpeed;
        Vector2 targetForce = (targetVelocity - rigidbody.velocity) / maxSpeed;
        return targetForce;
    }
}
