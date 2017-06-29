using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTextureAllocator{
    static public void allocateTexture(ref RenderTexture texture)
    {
        if(texture.width != Screen.width || texture.height != Screen.height)
        {
          texture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        }
    }
}
