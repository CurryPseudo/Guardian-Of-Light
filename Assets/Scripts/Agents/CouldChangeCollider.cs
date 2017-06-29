using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CouldChangeCollider : MonoBehaviour {
    public GameObject colliderObjects;
    bool collidersInit = false;
    Collider2D[] colliders;
    // Use this for initialization
    private void Awake()
    {
    }
    void initColliders()
    {
        if (colliderObjects != null)
        {
            colliders = new Collider2D[colliderObjects.transform.childCount];
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i] = colliderObjects.transform.GetChild(i).GetComponent<Collider2D>();
            }
        }
        else
        {
            Debug.LogError("Colliders in" + gameObject + " is null");
        }
    }
    void Start () {
        
    }
    public void setCollider(int colliderIndex,bool enable)
    {
        if (!collidersInit) initColliders();
        colliders[colliderIndex].enabled = enable;
    }

}
