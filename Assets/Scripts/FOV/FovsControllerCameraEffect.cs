using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FovsControllerCameraEffect : MonoBehaviour {
    public FovsController controller;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        ScreenTextureAllocator.allocateTexture(ref controller.finalTexture);
        controller.cam.targetTexture = controller.finalTexture;
    }


}
