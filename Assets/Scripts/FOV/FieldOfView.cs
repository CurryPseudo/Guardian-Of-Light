using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FieldOfView : MonoBehaviour {
    #region internal type
    struct ViewCastInfo
    {
        public bool hit;
        public Vector2 hitPosition;
        public float angle;
        public float distance;
        public LayerMask layer;
        public ViewCastInfo(bool hit, Vector2 hitPosition, float angle, float distance, LayerMask layer)
        {
            this.hit = hit;
            this.hitPosition = hitPosition;
            this.angle = angle;
            this.distance = distance;
            this.layer = layer;
        }
    }
    struct EdgeInfo
    {
        public Vector2 minPos;
        public float minAngle;
        public Vector2 maxPos;
        public float maxAngle;
        public LayerMask minLayer;
        public LayerMask maxLayer;

        public EdgeInfo(Vector2 minPos, float minAngle, LayerMask minLayer, Vector2 maxPos, float maxAngle, LayerMask maxLayer)
        {
            this.minPos = minPos;
            this.minAngle = minAngle;
            this.maxPos = maxPos;
            this.maxAngle = maxAngle;
            this.minLayer = minLayer;
            this.maxLayer = maxLayer;
            return;
        }
    }
    public struct MeshVertex
    {
        public float distance;
        public float angle;
        public Vector2 pos;
        public LayerMask layer;
        public MeshVertex(float angle, float distance, LayerMask layer,Vector2 pos)
        {
            this.distance = distance;
            this.angle = angle;
            this.layer = layer;
            this.pos = pos;
        }
    }
    #endregion
    #region events,properties,fields

    public FovMeshRenderer nowMR;
    public FovMeshRenderer nextMR;

    /// <summary>
    /// 光线的物理碰撞和渲染半径
    /// </summary>
    float viewRadius = 3;
    /// <summary>
    /// 光圈碰撞点线性查找中，每度raycast几次。
    /// </summary>
    public float meshResolution = 1;
    /// <summary>
    /// 当前光线的碰撞layer。
    /// </summary>
    public LayerMask nowMask;
    /// <summary>
    /// Leap过渡过程中，过程结束时光线的碰撞layer。
    /// </summary>
    LayerMask nextMask = 0;
    /// <summary>
    /// Leap过程的变化系数(从0到1)。
    /// </summary>
    [HideInInspector]
    public float ObstacleMaskChangeDegree = 0;
    /// <summary>
    /// 二分raycast中，二分查找次数。
    /// </summary>
    public int edgeFindTimes=10;
    /// <summary>
    /// 只有当线性raycast的点之间距离大于这个值，才进行二分查找。
    /// </summary>
    public float edgeFindSetOff = 0.1f;
    /// <summary>
    /// 当前光圈边缘的顶点。
    /// </summary>
    [HideInInspector] [NonSerialized]
    public ArrayList nowLightVerteces = new ArrayList();
    /// <summary>
    /// Leap过程中，过程结束时的光圈边缘顶点。
    /// </summary>
    [HideInInspector][NonSerialized]
    public ArrayList nextLightVerteces = new ArrayList();
    /// <summary>
    /// 渲染效果中，光圈边缘的线性渐变半径。
    /// </summary>
    public float viewFadeRadiusOffset = 0.7f;
    /// <summary>
    /// 场景中GameSystem的实例
    /// </summary>
    GameObject gameSystem;
    /// <summary>
    /// Leap时，光圈变化过程的持续时间
    /// </summary>
    float LeapAnimateDelayTime = 0.2f;
    /// <summary>
    /// 光圈是否启用。
    /// </summary>
    public bool lightEnable = true;
    /// <summary>
    /// 光圈的顶点数。
    /// </summary>
    [HideInInspector][NonSerialized]
    public int count = 0;
    /// <summary>
    /// 光线的物理碰撞和渲染半径
    /// </summary>
    public float ViewRadius
    {
        get { return viewRadius; }
        set {
            viewRadius = value;
        }
    }
    /// <summary>
    /// Leap时，使其光圈渐变地缩小增大。
    /// </summary>
    /// <param name="viewRadius">光圈将要变化至的半径</param>
    /// <param name="LeapAnimateDelayTime">渐变过程时长</param>
    
    /// <summary>
    /// 光圈的碰撞mask。
    /// </summary>
    public LayerMask ObstacleMask
    {
        get { return nowMask; }
        set
        {
            if (value != nowMask)
            {
                StartCoroutine(DelaySetViewMask(value));
            }
        }
    }
    public bool isLeaping = false;
    public bool IsLeaping
    {
        get {
            return isLeaping;
            
        }
    }
    #endregion
    #region MonoBehaviour
    public void Awake()
    {
        if(!GameObject.Find("FovControllerCamera").GetComponent<FovsController>().fovs.Contains(this))
        GameObject.Find("FovControllerCamera").GetComponent<FovsController>().fovs.Add(this);
        gameSystem = GameObject.Find("GameSystem");
        nowMR = gameObject.AddComponent<FovMeshRenderer>();
        nowMR.fov = this;
        nowMR.name = "now";
        nextMR = gameObject.AddComponent<FovMeshRenderer>();
        nextMR.fov = this;
        nextMR.name = "next";
    }

    void Start()
    {
        nextMask = LayerMask.GetMask("HighObstacleLayer", "Background");

    }
    // Update is called once per frame
    private void OnDestroy()
    {
        if (GameObject.Find("FovControllerCamera") != null && GameObject.Find("FovControllerCamera").GetComponent<FovsController>().fovs.Contains(this))
            GameObject.Find("FovControllerCamera").GetComponent<FovsController>().fovs.Remove(this);
    }
    void Update()
    {
        GetFieldOfView(ref nowMR.mesh, nowMask);
        GetFieldOfView(ref nextMR.mesh, nextMask);
        nowMR.alpha = 1- ObstacleMaskChangeDegree;
        nextMR.alpha = ObstacleMaskChangeDegree;
        count = nowLightVerteces.Count;
        //Test();
    }
    private void LateUpdate()
    {
        
    }
    private void OnDrawGizmos()
    {
        if (nowMR != null && nowMR.texture != null && gameObject.name == "Player")
        {
           /* Gizmos.color = Color.white;
            Gizmos.DrawMesh(nowMR.mesh, Camera.main.transform.position);
            Gizmos.color = Color.blue;
            Gizmos.DrawMesh(nextMR.mesh, Camera.main.transform.position);*/
        }
    }
    #endregion
    #region Fov
    public void setViewRadiusLeaply(float viewRadius, float LeapAnimateDelayTime)
    {
        this.LeapAnimateDelayTime = LeapAnimateDelayTime;
        if (viewRadius != this.viewRadius)
        {
            StartCoroutine(SetViewRadius(viewRadius));
        }
    }
    IEnumerator DelaySetViewMask(LayerMask _obstacleMask)
    {
        nextMask = _obstacleMask;
        ObstacleMaskChangeDegree = 0;
        float timeCount = 0;
        while (timeCount < LeapAnimateDelayTime)
        {
            timeCount += Time.deltaTime;
            ObstacleMaskChangeDegree = timeCount / LeapAnimateDelayTime;
            yield return null;
        }
        nowMask = nextMask;
        ObstacleMaskChangeDegree = 0;
        //swapMR(ref nowMR, ref nextMR);
        yield break;
    }
    IEnumerator SetViewRadius(float value)
    {
        isLeaping = true;
        float nowRadius = ViewRadius;
        float timeCount = 0;
        while (timeCount <= LeapAnimateDelayTime)
        {
            timeCount += Time.deltaTime;
            viewRadius = Mathf.Lerp(nowRadius, value, timeCount / LeapAnimateDelayTime);
            //viewRadius = nowRadius + (timeCount / LeapAnimateDelayTime) * (value - nowRadius);
            yield return null;
        }
        viewRadius = value;
        isLeaping = false;
        yield break;
    }
    MeshVertex GetMeshVertex(int index)
    {
        return (MeshVertex)nowLightVerteces[index];
    }
    Vector2 AngleToDir(float angle)
    {
        return new Vector2(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle));
    }
    ViewCastInfo ViewCast(float angle, LayerMask ObstacleLayer)
    {
        Vector2 dir = AngleToDir(angle);
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        RaycastHit2D rayCastResult = Physics2D.Raycast(pos, dir, viewRadius, ObstacleLayer);
        if (rayCastResult)
        {

            return new ViewCastInfo(true, rayCastResult.point, angle, rayCastResult.distance, rayCastResult.collider.gameObject.layer);
        }
        else
        {
            return new ViewCastInfo(false, pos + dir * viewRadius, angle, viewRadius, 0);
        }
    }
    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast, LayerMask ObstacleLayer)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector2 minPos = Vector2.zero;
        Vector2 maxPos = Vector2.zero;
        LayerMask minLayer = 0;
        LayerMask maxLayer = 0;
        for (int i = 0; i < edgeFindTimes; i++)
        {
            float midAngle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(midAngle, ObstacleLayer);
            bool hitSame = !(Mathf.Abs(newViewCast.distance - minViewCast.distance) > edgeFindSetOff);
            if (newViewCast.hit == minViewCast.hit && hitSame)
            {
                minAngle = midAngle;
                minPos = newViewCast.hitPosition;
                minLayer = newViewCast.layer;
            }
            else
            {
                maxAngle = midAngle;
                maxPos = newViewCast.hitPosition;
                maxLayer = newViewCast.layer;
            }
        }
        return new EdgeInfo(minPos, minAngle, minLayer, maxPos, maxAngle, maxLayer);
    }
    void GetFieldOfView(ref Mesh mesh,LayerMask ObstacleLayer)
    {
        if (mesh == null) mesh = new Mesh();
        float rayCount = meshResolution * 360;
        float angleStep = 360 / rayCount;
        ViewCastInfo oldViewCast = new ViewCastInfo();
        List<Vector3> verticesList = new List<Vector3>();
        for (int i = 0; i <= rayCount; i++)
        {
            ViewCastInfo newViewCast = ViewCast(angleStep * i, ObstacleLayer);
            if (i > 0 && (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && Mathf.Abs((oldViewCast.distance - newViewCast.distance)) > edgeFindSetOff)))
            {

                EdgeInfo edge = FindEdge(oldViewCast, newViewCast, ObstacleLayer);
                if (edge.minPos != Vector2.zero)
                {
                    verticesList.Add(edge.minPos);

                }
                if (edge.maxPos != Vector2.zero)
                {
                    verticesList.Add(edge.maxPos);
                }

            }
            verticesList.Add(newViewCast.hitPosition);
            oldViewCast = newViewCast;
        }
        Vector3[] vertices = new Vector3[verticesList.Count + 1];
        //vertices[0] = transform.position;
        vertices[0] = new Vector3(transform.position.x, transform.position.y, 0);
        for (int i = 0; i < verticesList.Count; i++)
        {
            vertices[i + 1] = new Vector3(verticesList[i].x, verticesList[i].y, 0);
        }
        int[] triangles = new int[(verticesList.Count - 1) * 3];
        for(int i = 0; i < verticesList.Count - 1; i++)
        {
            triangles[3 * i] = 0;
            triangles[3 * i + 1] = i + 1;
            triangles[3 * i + 2] = i + 2;
        }
        mesh.Clear(true);
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return;

    }
    #endregion
    #region tests
    void TestMeshVerteces()
    {
        for(int i = 0; i < nowLightVerteces.Count; i++)
        {
            print("Angle: "+((MeshVertex)nowLightVerteces[i]).angle+"  Distance: " + ((MeshVertex)nowLightVerteces[i]).distance);
        }
    }
    #endregion
}
