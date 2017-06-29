using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    Animator animator;
    PlayerController player;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        string animateName = DirectionAnimation.DirectionOfVelocity(player.velocity);
        if (animateName != null)
        {
            animator.Play(animateName);
        }
    }
}

