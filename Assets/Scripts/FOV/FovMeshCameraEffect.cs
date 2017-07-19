using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FovMeshCameraEffect : MonoBehaviour {
    Material m;
    public new FovMeshRenderer renderer;
	// Use this for initialization
	void Start () {
		m = new Material(Shader.Find("CameraEffect/FovMeshEffect"));
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = Camera.main.transform.position;

	}
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (renderer != null)
        {
            Vector3 lightPos = Camera.main.WorldToViewportPoint(renderer.Fov.transform.position);
            Vector3 lightRadius = (Camera.main.WorldToViewportPoint(new Vector3(1,1,0) * renderer.Fov.ViewRadius) - Camera.main.WorldToViewportPoint(Vector3.zero));
            m.SetVector("_LightPos", lightPos);
            m.SetVector("_Radius", lightRadius);
            m.SetFloat("_Alpha", renderer.alpha);
            Graphics.Blit(source, destination, m);
        }
    }
}
