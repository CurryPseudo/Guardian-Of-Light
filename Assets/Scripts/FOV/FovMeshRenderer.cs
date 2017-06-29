using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FovMeshRenderer : MonoBehaviour{
    public Mesh mesh;
    Camera cam;
    public FieldOfView fov;
    public float alpha = 1;
    Mesh quadMesh;
    Material fovMeshM;
    Material texMeshM;
    Vector2 cameraSize;
    public FieldOfView Fov
    {
        get
        {
            return fov;
        }
    }
    RenderTexture finalTexture;
    public RenderTexture texture
    {
        get
        {
            return finalTexture;
        }
    }
    public string name
    {
        set
        {
            GameObject cameraGO = new GameObject(fov.transform.name+" "+value + " FovCamera");
            cameraGO.transform.position = fov.transform.position;
            cameraGO.transform.parent = GameObject.Find("FovCameras").transform;
            cam = cameraGO.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            cam.cullingMask = LayerMask.GetMask("FovCamera");
            cam.orthographic = true;
            cam.orthographicSize = Camera.main.orthographicSize;
            cam.depth = -2;
            FovMeshCameraEffect fmce = cameraGO.AddComponent<FovMeshCameraEffect>();
            fmce.renderer = this;
            finalTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            cam.targetTexture = finalTexture;
        }
    }
    public void LateUpdate()
    {
        if (cam != null) DrawMesh();
    }
    public void DrawMesh()
    {
        if (mesh != null)
        {
            ScreenTextureAllocator.allocateTexture(ref finalTexture);
            cam.targetTexture = finalTexture;
            mesh.MarkDynamic();
            if(!fovMeshM)
                fovMeshM = new Material(Shader.Find("2D/Fov Mesh"));
            Graphics.DrawMesh(mesh,Vector3.zero, Quaternion.identity, fovMeshM, LayerMask.NameToLayer("FovCamera"),cam,0);
            allocateQuadMesh();
            if (!texMeshM)
            {
                texMeshM = new Material(Shader.Find("2D/Texture Mix Mesh"));
            }
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            mpb.SetTexture("_MainTex", texture);
            Graphics.DrawMesh(quadMesh, Vector3.zero, Quaternion.identity, texMeshM, LayerMask.NameToLayer("FovCamera"), GameObject.Find("FovControllerCamera").GetComponent<FovsController>().cam, 0,mpb);

        }
    }
    void allocateQuadMesh()
    {
        if (!quadMesh || cameraSize.x != Camera.main.orthographicSize * Camera.main.aspect || cameraSize.y != Camera.main.orthographicSize)
        {
            quadMesh = new Mesh();
            cameraSize = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
            quadMesh.SetVertices(new List<Vector3>(new Vector3[] {
                    new Vector3(-cameraSize.x,cameraSize.y),
                    new Vector3(-cameraSize.x,-cameraSize.y),
                    new Vector3(cameraSize.x,-cameraSize.y),
                    new Vector3(cameraSize.x,cameraSize.y)
                }));
            quadMesh.SetUVs(0, new List<Vector2>(new Vector2[] { Vector2.zero, new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) }));
            quadMesh.SetIndices(new int[] { 0, 1, 2, 3 }, MeshTopology.Quads, 0);
        }
    }
}
