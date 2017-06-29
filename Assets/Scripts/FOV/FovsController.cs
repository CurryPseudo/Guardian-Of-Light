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
        finalTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        cam.targetTexture = finalTexture;
    }
    
}
