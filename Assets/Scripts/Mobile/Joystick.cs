using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Joystick : MonoBehaviour,IPointerDownHandler,IDragHandler,IPointerUpHandler {
    Image background;
    Image stick;
    Vector2 inputVector;
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(background.rectTransform, eventData.position, eventData.enterEventCamera,out pos))
        {
            pos.x /= background.rectTransform.sizeDelta.x/2;
            pos.y /= background.rectTransform.sizeDelta.y/2;
            inputVector = pos;
            inputVector = inputVector.magnitude > 1 ? inputVector.normalized : inputVector;
            stick.rectTransform.anchoredPosition = new Vector3(inputVector.x * background.rectTransform.sizeDelta.x / 3, inputVector.y * background.rectTransform.sizeDelta.y / 3, 0);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        stick.rectTransform.anchoredPosition = Vector3.zero;
    }

    private void Awake()
    {
        background = GetComponent<Image>();
        stick =transform.GetChild(0).GetComponent<Image>();
    }
    public float Horizontal
    {
        get
        {
            return inputVector.x;
        }
    }
    public float Vertical
    {
        get
        {
            return inputVector.y;
        }
    }
}
