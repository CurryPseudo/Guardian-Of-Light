using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTextureAllocator{
    static public float sizeScale = 0.5f;
    static public void allocateTexture(ref RenderTexture texture)
    {
        if(texture.width != (int)(Screen.width * sizeScale) || texture.height != (int)(Screen.height * sizeScale))
        {
            RenderTexture.Destroy(texture);
            texture = generateRenderTexture(); 
        }
    }
    static public RenderTexture generateRenderTexture()
    {
        return new RenderTexture((int)(Screen.width * sizeScale), (int)(Screen.height * sizeScale), 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

    }
    static public bool fovEnabled = true;

}
