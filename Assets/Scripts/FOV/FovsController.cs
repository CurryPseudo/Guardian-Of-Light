using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FovsController : MonoBehaviour {
    public List<FieldOfView> fovs = new List<FieldOfView>();
    public Camera cam;
    public RenderTexture finalTexture;
    public float alpha = 1;
    public RenderTexture texture
    {
        get
        {
            
            return finalTexture;
        }

    }
    private void Awake()
    {
        transform.position = Camera.main.transform.position;
        cam = GetComponent<Camera>();

        cam.orthographicSize = Camera.main.orthographicSize;
        GetComponent<FovsControllerCameraEffect>().controller = this;
        finalTexture = ScreenTextureAllocator.generateRenderTexture();
        cam.targetTexture = finalTexture;
    }
    private void Update()
    {
        transform.position = Camera.main.transform.position;
        cam.orthographicSize = Camera.main.orthographicSize;
    }

}
