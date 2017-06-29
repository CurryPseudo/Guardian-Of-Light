using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ViewImageEffect : MonoBehaviour {
    public Material m;
    public Material mH;
    float WorldSize = 1;
    public int textureWidth = 128;
    public List<FieldOfView> Fovs;
    float alpha = 0;
    Camera camera;
    // Use this for initialization
    private void Awake()
    {
        camera = GetComponent<Camera>();
        m.SetFloat("alpha", 0);
        WorldSize = GameObject.Find("Background").transform.lossyScale.x;

    }
    public void AddFov(FieldOfView fov)
    {
        Fovs.Add(fov);
    }
    public void RemoveFov(FieldOfView fov)
    {
        Fovs.Remove(fov);
    }
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void NightEnd(float time)
    {
        StartCoroutine(NightFade(time,1));
    }
    public void NightBegin(float time)
    {
        StartCoroutine(NightFade(time, 0));
    }
    IEnumerator NightFade(float fadeTime,float value)
    {
        float timeCount = 0;
        while (timeCount < fadeTime)
        {
            yield return null;
            timeCount += Time.deltaTime;
            m.SetFloat("alpha", alpha * (1 - timeCount / fadeTime) + value * timeCount / fadeTime);
        }
        m.SetFloat("alpha",value);
        alpha = value;
        yield break;
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        int width = Screen.width;
        int height = Screen.height;
        Vector3 RightTopPos = camera.ScreenToWorldPoint(new Vector3(width, height, 0));
        Vector3 LeftBottomPos = camera.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector4 LeftBottomAndRightTopPos = new Vector4(LeftBottomPos.x, LeftBottomPos.y, RightTopPos.x, RightTopPos.y);
        m.SetVector("cameraLeftBottomAndRightTop", LeftBottomAndRightTopPos);
        m.SetFloat("_WorldSize", WorldSize);
        List<FieldOfView> enabledFovs = new List<FieldOfView>();
        foreach (FieldOfView fov in Fovs)
        {
            if (fov.lightEnable) enabledFovs.Add(fov);
        }
        int lightCount = enabledFovs.Count;
        m.SetInt("_LightCount", lightCount);
        Vector4[] Poss = new Vector4[10];
        Texture2D fovInfoTexture = new Texture2D(2, lightCount,TextureFormat.RGBAFloat, true);
        Texture2D fovTexture = new Texture2D(textureWidth, lightCount, TextureFormat.RGBAFloat, true) ;
        for (int i = 0; i < lightCount; i++)
        {
            FieldOfView fov = enabledFovs[i];
            Vector3 pos = fov.transform.position;
            fovInfoTexture.SetPixel(0, i, new Color(fov.ObstacleMaskChangeDegree, fov.ViewRadius / WorldSize, fov.viewFadeRadiusOffset / WorldSize,0));
            fovInfoTexture.SetPixel(1, i, new Color(pos.x/WorldSize+0.5f, pos.y / WorldSize + 0.5f, 0,0));
            float[] meshlinerDistance = AngleToMeshlinerDistance(textureWidth, fov.nowLightVerteces);
            float[] nextMeshlinerDistance = AngleToMeshlinerDistance(textureWidth, fov.nextLightVerteces);
            for (int j = 0; j < textureWidth; j++)
            {
                Color pixelColor = new Color(meshlinerDistance[j] / fov.ViewRadius, nextMeshlinerDistance[j] / fov.ViewRadius, 0);
                fovTexture.SetPixel(j, i, pixelColor);
            }
        }
        fovTexture.Apply();
        m.SetTexture("_FovTexture", fovTexture);
        fovInfoTexture.Apply();
        m.SetTexture("_FovInfoTex", fovInfoTexture);
        Graphics.Blit(source, destination, m);
    }
    float[] AngleToMeshlinerDistance(int arrayLength, ArrayList meshVerteces)
    {
        int min = 0;
        int max = 1;
        float[] meshlinerDistance = new float[arrayLength];
        for (int i = 0; i < arrayLength; i++)
        {
            float angle = (float)i * 360.0f / (float)(arrayLength - 1);

            while (max < meshVerteces.Count - 1 && angle > ((FieldOfView.MeshVertex)meshVerteces[max]).angle)
            {
                min++;
                max++;
            }
            FieldOfView.MeshVertex mvMin = (FieldOfView.MeshVertex)meshVerteces[min];
            FieldOfView.MeshVertex mvMax = (FieldOfView.MeshVertex)meshVerteces[max];
            //meshlinerDistance[i] = Mathf.Lerp(mvMin.distance, mvMax.distance, (angle - mvMin.angle) / (mvMax.angle - mvMin.angle));
            //meshlinerDistance[i] = (angle - mvMin.angle) / (mvMax.angle - mvMin.angle) * (mvMax.distance - mvMin.distance) + mvMin.distance;
            //极坐标上的线性插值
            float k = 0;
            float dis = 0;
            if (mvMax.pos.y != mvMin.pos.y)
            {
                k = (mvMax.pos.x - mvMin.pos.x) / (mvMax.pos.y - mvMin.pos.y);
                dis = (mvMin.pos.x - k * mvMin.pos.y) / (Mathf.Sin(angle * Mathf.Deg2Rad) - k * Mathf.Cos(angle * Mathf.Deg2Rad));
            }else
            {
                dis = mvMin.pos.y / Mathf.Cos(angle * Mathf.Deg2Rad);
            }
            meshlinerDistance[i] = dis;
        }
        return meshlinerDistance;
    }
}
