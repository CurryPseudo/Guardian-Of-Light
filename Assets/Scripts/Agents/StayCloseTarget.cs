using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayCloseTarget : MonoBehaviour {
    public Rigidbody2D target;
    new Rigidbody2D rigidbody;
	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody2D>();
        if (target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void FixedUpdate()
    {
        if (target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y,transform.position.z);
        }else
        {
            Destroy(gameObject);
        }
    }
}
