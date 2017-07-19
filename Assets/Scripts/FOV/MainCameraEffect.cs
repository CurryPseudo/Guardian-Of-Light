using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraEffect : MonoBehaviour {
    Material m;
	// Use this for initialization
	void Start () {
        if (ScreenTextureAllocator.fovEnabled == false)
        {
            enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
       
        if (!m)
        {
            m = new Material(Shader.Find("CameraEffect/Texture Mix"));
        }
        if (GameObject.Find("FovControllerCamera").GetComponent<FovsController>().texture)
        {
            m.SetTexture("_ExtraTex", GameObject.Find("FovControllerCamera").GetComponent<FovsController>().texture);
            m.SetFloat("_Alpha", GameObject.Find("FovControllerCamera").GetComponent<FovsController>().alpha);
            Graphics.Blit(source, destination, m);
        }
    }
    public void nightBegin(float time)
    {
        StartCoroutine(changeAlpha(1, time));
    }
    IEnumerator changeAlpha(float value,float time)
    {
        float timeCount = 0;
        float preValue = GameObject.Find("FovControllerCamera").GetComponent<FovsController>().alpha;
        while (timeCount < time)
        {
            timeCount += Time.deltaTime;
            GameObject.Find("FovControllerCamera").GetComponent<FovsController>().alpha = Mathf.Lerp(preValue, value, timeCount / time);
            yield return null;
        }
        GameObject.Find("FovControllerCamera").GetComponent<FovsController>().alpha = value;
        yield break;
    }
    public void nightEnd(float time)
    {
        StartCoroutine(changeAlpha(0, time));

    }
}
