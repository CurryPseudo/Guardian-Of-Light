using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class FieldOfView : MonoBehaviour {
    public float viewRadius = 4;
    public int rayCount = 50;
    public LayerMask obstacleMask;
    public MeshFilter viewMeshFilter;
    public float edgeDstThreshold = .5f;
    public int FindEdgeTime;
    Mesh viewMesh;
	// Use this for initialization
	void Start () {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
	}
    Vector3 AngleToVector(float angle, bool global)
    {
        if (global)
        {
            angle += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0, Mathf.Cos(Mathf.Deg2Rad * angle));
    }
    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPos = Vector3.zero;
        Vector3 maxPos = Vector3.zero;
        for(int i = 0; i < FindEdgeTime; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);
            
            float dst = Mathf.Abs(newViewCast.dst - minViewCast.dst);
            if(newViewCast.hit == minViewCast.hit && dst<=edgeDstThreshold)
            {
                minAngle = angle;
                minPos = newViewCast.hitPosition;
            }else
            {
                maxAngle = angle;
                maxPos = newViewCast.hitPosition;
            }
        }
        return new EdgeInfo(minPos, maxPos);
    }
    ViewCastInfo ViewCast(float angle)
    {
        Vector3 dir = AngleToVector(angle, true);
        Vector3 pos = transform.position;
        RaycastHit hitResult;
        if (Physics.Raycast(pos, dir,out hitResult, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hitResult.point,hitResult.distance,angle);
        }else
        {
            return new ViewCastInfo(false, pos + dir * viewRadius,viewRadius,angle);
        }
    }
    void DrawFieldOfView()
    {
        float angleStep = 360.0f / rayCount;
        List<Vector3> viewCasts = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for(int i = 0; i <= rayCount; i++)
        {
            ViewCastInfo viewCast = ViewCast(angleStep * i);
            float dst = Mathf.Abs(viewCast.dst - oldViewCast.dst);
            if (i > 0)
            {
                if (oldViewCast.hit != viewCast.hit || (dst > edgeDstThreshold && oldViewCast.hit && viewCast.hit))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, viewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewCasts.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewCasts.Add(edge.pointB);
                    }
                }
            }
            viewCasts.Add(viewCast.hitPosition);
            oldViewCast = viewCast;
        }
        int vertexCount = viewCasts.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];
        vertices[0] = Vector3.zero;
        for(int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewCasts[i]);
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }
	// Update is called once per frame
	void Update () {
        DrawFieldOfView();
	}
    struct ViewCastInfo
    {
        public bool hit;
        public Vector3 hitPosition;
        public float dst;
        public float angle;
        public ViewCastInfo(bool _hit,Vector3 _hitPosition,float _dst,float _angle)
        {
            hit = _hit;
            hitPosition = _hitPosition;
            dst = _dst;
            angle = _angle;
        }
    }
    struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;
        public EdgeInfo (Vector3 _pointA,Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
