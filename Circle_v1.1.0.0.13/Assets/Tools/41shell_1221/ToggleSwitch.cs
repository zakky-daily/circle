using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleSwitch : MonoBehaviour
{
    public bool isOn;

    public bool IsAllowSwitch;

    [SerializeField] ButtonChild ONColor;
    [SerializeField] RectTransform Circle;

    public float PushAnimation;

    [SerializeField] UnityEvent PushEvent;


    void Start()
    {
        
    }

    void Update()
    {
        Animation(ref PushAnimation, 6, isOn);

        Color32 c = ONColor.DefaultColor;
        c.a = (byte)(PushAnimation * 255);
        ONColor.DefaultColor = c;

        Circle.anchorMin = new Vector2(PushAnimation, 0.5f);
        Circle.anchorMax = new Vector2(PushAnimation, 0.5f);

        Circle.pivot = new Vector2(PushAnimation, 0.5f);

        Circle.anchoredPosition = new Vector2((0.5f - PushAnimation) * 2 * 15, 0);
    }

    public void Push()
    {
        if (IsAllowSwitch)
        {
            isOn = !isOn;
        }
        
        if (PushEvent != null) PushEvent.Invoke();
    }


    void Animation(ref float timer, float speed, bool situation)
    {
        if (situation)
        {
            if (timer + Time.deltaTime * speed >= 1)
            {
                timer = 1;
            }
            else
            {
                timer += Time.deltaTime * speed;
            }
        }
        else
        {
            if (timer - Time.deltaTime * speed <= 0)
            {
                timer = 0;
            }
            else
            {
                timer -= Time.deltaTime * speed;
            }
        }
    }
}
