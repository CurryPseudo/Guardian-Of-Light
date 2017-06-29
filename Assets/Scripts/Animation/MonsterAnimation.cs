using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimation : MonoBehaviour {
    Animator animator;
    Rigidbody2D rigidbody;
    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        string animateName = DirectionAnimation.DirectionOfVelocity(rigidbody.velocity);
        if (animateName != null)
        {
            animator.Play(animateName);
        }
    }
}
