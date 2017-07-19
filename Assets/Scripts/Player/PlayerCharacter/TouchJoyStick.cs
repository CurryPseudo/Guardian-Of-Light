using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
public class TouchJoyStick : MonoBehaviour {
    enum JoystickStatus
    {
        Default,
        Idle
    }
    JoystickStatus status = JoystickStatus.Default;
    Vector2 JoystickZeroPos = Vector2.zero;
    int nowTouchFingerId;
    public Joystick joystick;
    Image backgroundImage;
    private void Awake()
    {
        joystick = GameObject.Find("JoystickBackground").GetComponent<Joystick>();
        backgroundImage = joystick.GetComponent<Image>();
    }
    // Use this for initialization
    void Start () {
        joystick.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update () {

        switch (status)
        {
            case JoystickStatus.Default:
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.touches[i];
                    if (touch.position.x < Screen.width / 2 && touch.phase == TouchPhase.Began)
                    {
                        nowTouchFingerId =  touch.fingerId;
                        status = JoystickStatus.Idle;
                        joystick.gameObject.SetActive(true);
                        joystick.GetComponent<RectTransform>().position = touch.position;
                        joystick.SetStickPos(touch.position);
                        break;
                    }
                }
                break;
            case JoystickStatus.Idle:
                Touch? nowTouchCouldNull = null;
                for(int i = 0; i < Input.touchCount;i++)
                {
                    Touch touch = Input.touches[i];
                    if(touch.fingerId == nowTouchFingerId)
                    {
                        nowTouchCouldNull = touch;
                    }
                }
                if(nowTouchCouldNull == null)
                {
                    EndIdleStatus();
                    break;
                }
                Touch nowTouch = nowTouchCouldNull.Value;
                if(nowTouch.phase==TouchPhase.Moved|| nowTouch.phase == TouchPhase.Stationary)
                {
                    joystick.SetStickPos(nowTouch.position);
                }else
                {
                    EndIdleStatus();
                }
                break;
        }
	}
    void EndIdleStatus()
    {
        joystick.FingerLeave();
        status = JoystickStatus.Default;

    }
}
