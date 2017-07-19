using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesRandGenerate : MonoBehaviour {
    //public int obstacleCount = 0;
    public int horizontalObstacleCount = 3;
    public int verticalObstacleCount = 2;
    Transform backGroundTransform;
    // Use this for initialization
    private void Awake()
    {
        backGroundTransform = GameObject.Find("Background").transform;

    }
    void Start () {
        Generate();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void Generate()
    {
        List<GameObject> obstaclePrefabLibrary = new List<GameObject>();
        int count = 0;
        GameObject obstaclePrefab = Resources.Load("SceneObjects/Obstacles/Obstacle" + count) as GameObject;
        while (obstaclePrefab != null)
        {
            obstaclePrefabLibrary.Add(obstaclePrefab);
            obstaclePrefab = Resources.Load("SceneObjects/Obstacles/Obstacle" + ++count) as GameObject;
        }
        for (int i = 0; i < horizontalObstacleCount; i++)
        {
            for (int j = 0; j < verticalObstacleCount; j++)
            {


                int randIndex = (int)(Random.value * obstaclePrefabLibrary.Count);
                obstaclePrefab = obstaclePrefabLibrary[randIndex];
                Vector3 position;
                Quaternion rotation;
                GameObject obstacleInstance = Instantiate<GameObject>(obstaclePrefab, backGroundTransform.TransformPoint(1,1,0), new Quaternion());
                obstacleInstance.transform.SetParent(GameObject.Find("Obstacles").transform, true);
                ObstacleInfo info;
                do
                {
                    position = backGroundTransform.TransformPoint((Random.value + i) / (float)horizontalObstacleCount - 0.5f, (Random.value + j) / (float)verticalObstacleCount - 0.5f, 0);
                    rotation = Quaternion.Euler(0, 0, Random.value * 360);
                    info = new ObstacleInfo(obstacleInstance, position, rotation);
                } while (IfObstacleNotOverlapedPutItInScene(info));
            }
        }
    }
    bool IfObstacleNotOverlapedPutItInScene(ObstacleInfo info)
    {
        info.instance.transform.rotation = info.rotation;
        Collider2D[] colliders = info.instance.GetComponentsInChildren<Collider2D>();
        foreach (var collider in colliders)
        {
            Vector3 worldSizev3 = backGroundTransform.TransformPoint(1, 1, 0);
            Vector3 min = info.position - worldSizev3 + collider.bounds.center - collider.bounds.extents;
            Vector3 max = info.position - worldSizev3 + collider.bounds.center + collider.bounds.extents;
            Collider2D[] result = Physics2D.OverlapAreaAll(min, max, LayerMask.GetMask("ObstacleLayer", "HighObstacleLayer", "Background"));
            if (result.Length != 0)
            {
                return true;
            }
        }
       
       info.instance.transform.position = info.position;
        return false;
    }
    struct ObstacleInfo
    {
        public GameObject instance;
        public Vector3 position;
        public Quaternion rotation;
        public ObstacleInfo(GameObject instance, Vector3 position, Quaternion rotation)
        {
            this.instance = instance;
            this.position = position;
            this.rotation = rotation;
        }
    }
}
