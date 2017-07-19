using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Joystick : MonoBehaviour{
    Image background;
    Image stick;
    Vector2 inputVector;
    public void OnDrag(PointerEventData eventData)
    {
       
    }
    public void SetStickPos(Vector2 screenPos)
    {
        Vector3 pos = new Vector3(screenPos.x,screenPos.y) - background.rectTransform.position;
        //if (RectTransformUtility.ScreenPointToWorldPointInRectangle(background.rectTransform, screenPos,Camera.main, out pos))
        pos.x /= background.rectTransform.sizeDelta.x;
        pos.y /= background.rectTransform.sizeDelta.y;
        inputVector = pos;
        inputVector = inputVector.magnitude > 1 ? inputVector.normalized : inputVector;
        stick.rectTransform.anchoredPosition = new Vector3(inputVector.x * background.rectTransform.sizeDelta.x / 3.5f, inputVector.y * background.rectTransform.sizeDelta.y / 3.5f, 0);
    }
   /* public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }*/
    public void FingerLeave()
    {
        inputVector = Vector2.zero;
        stick.rectTransform.anchoredPosition = Vector3.zero;
        gameObject.SetActive(false);

    }
    /*public void OnPointerUp(PointerEventData eventData)
    {
       
    }
    */
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
