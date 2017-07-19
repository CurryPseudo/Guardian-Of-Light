using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObstacleBorder : MonoBehaviour {
    ObstaclesRandGenerate org;
    Transform backgroundTransform;
	// Use this for initialization
	void Start () {
       
        backgroundTransform = GameObject.Find("Background").transform;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnDrawGizmos()
    {
        if(org == null) org = GetComponent<ObstaclesRandGenerate>();
        if(backgroundTransform == null) backgroundTransform = GameObject.Find("Background").transform; 
        Gizmos.color = Color.blue;
        for(int i = 1; i < org.horizontalObstacleCount; i++)
        {
            Gizmos.DrawLine(backgroundTransform.TransformPoint((float)i / org.horizontalObstacleCount - 0.5f, -0.5f, 0),
                backgroundTransform.TransformPoint((float)i / org.horizontalObstacleCount - 0.5f, 0.5f, 0)
                );
        }
        for (int i = 1; i < org.verticalObstacleCount; i++)
        {
            Gizmos.DrawLine(backgroundTransform.TransformPoint(-0.5f, (float)i / org.verticalObstacleCount - 0.5f, 0),
                backgroundTransform.TransformPoint(0.5f, (float)i / org.verticalObstacleCount - 0.5f, 0)
                );
        }
    }

}
