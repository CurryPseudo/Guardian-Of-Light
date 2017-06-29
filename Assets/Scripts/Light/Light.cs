using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(FieldOfView))]
public class Light : MonoBehaviour {
    public float keepTime = 5.0f;
    public float startShrinkTime = 2.5f;
    public FieldOfView fov;
    public float originalViewRadius = 4;
    // Use this for initialization
    private void Awake()
    {
        fov = GetComponent<FieldOfView>();

    }
    void Start () {
        fov.ViewRadius = originalViewRadius;
        StartCoroutine(LightDown());
    }
    IEnumerator LightDown()
    {
        float timeCount = 0;
        while(timeCount< keepTime)
        {
            yield return null;
            timeCount += Time.deltaTime;
            if (timeCount > startShrinkTime)
            {
                fov.ViewRadius = (originalViewRadius * (1 - (timeCount - startShrinkTime) / (keepTime - startShrinkTime)));
            }
        }
        Destroy(gameObject);
        yield break;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, originalViewRadius);

    }
    // Update is called once per frame
    void Update () {
        
    }
}
