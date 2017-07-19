using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestColliderBound : MonoBehaviour {
    Vector2 min;
    Vector2 max;
    new Collider2D collider;
    // Use this for initialization
    void Start () {
        
        
    }
    private void OnDrawGizmos()
    {
        if(collider == null) collider = GetComponentInChildren<Collider2D>();
        min = collider.bounds.center - collider.bounds.extents;
        max = collider.bounds.center + collider.bounds.extents;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(min, max);
    }
    // Update is called once per frame
    void Update () {
		
	}
}
