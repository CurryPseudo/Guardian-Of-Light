using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillChargeAnnulus{
    Image image;
    float maxLen;
    SkillChargeAnnulusController controller;
    public SkillChargeAnnulus(int index,int maxCount,SkillChargeAnnulusController _controller)
    {
        float step = 0.02f;
        controller = _controller;
        GameObject ui = new GameObject();
        ui.name = "ui" + index;
        image =  ui.AddComponent<Image>();
        image.rectTransform.position = controller.transform.position;
        
        image.sprite = Resources.Load<Sprite>("Sprites/annulus");
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Radial360;
        image.fillOrigin = 2;
        if (maxCount == 1)
        {
            image.fillAmount = 1;
            maxLen = 1;

        }
        else
        {
            image.fillAmount = 1 / (float)maxCount - step;
            maxLen = 1 / (float)maxCount - step;
        }
        image.rectTransform.Rotate(new Vector3(0,0, -360*((((float)index-0.5f) / (float)maxCount)+ step/2)));
        ui.transform.SetParent(controller.transform);
        image.rectTransform.localScale = new Vector3(1, 1, 1);
        image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, controller.gameObject.GetComponent<RectTransform>().sizeDelta.x * 1.5f);
        image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, controller.gameObject.GetComponent<RectTransform>().sizeDelta.y * 1.5f);
    }
    public float chargeValue{
        get
        {
            return image.fillAmount / maxLen;
        }
        set
        {
            image.fillAmount = maxLen * value;
            image.rectTransform.position = controller.transform.position;
        }
    }
    
    public void destroy()
    {
        GameObject.Destroy(image.gameObject);
    }
}
