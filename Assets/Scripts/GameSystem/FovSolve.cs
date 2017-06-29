using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FovSolve : MonoBehaviour {
    public List<FieldOfView> Fovs;
    List<FieldOfView> enabledFovs;
    // Use this for initialization
    public void AddFov(FieldOfView fov)
    {
        Fovs.Add(fov);
    }
    public void RemoveFov(FieldOfView fov)
    {
        Fovs.Remove(fov);
    }
    public List<GameObject> InLight(Vector2 pos)
    {
        List<GameObject> gos = new List<GameObject>();
        foreach(FieldOfView fov in Fovs)
        {
            if (fov.lightEnable)
            {
                Vector2 start = Agent.XY(fov.gameObject.transform.position);
                Vector2 dir = pos - start;
                float distance = fov.ViewRadius;
                if (dir.magnitude <= distance)
                {
                    LayerMask layer = fov.ObstacleMask;
                    RaycastHit2D result = Physics2D.Raycast(start, dir.normalized, dir.magnitude, layer);
                    if (result)
                    {

                    }else
                    {
                        gos.Add(fov.gameObject);

                    }
                }
            }
        }
        if (gos.Count == 0) return null;
        return gos;
    }
    private void Awake()
    {
        Fovs = GameObject.Find("FovControllerCamera").GetComponent<FovsController>().fovs;
    }
}
