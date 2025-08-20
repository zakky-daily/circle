using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class CanvasSize : MonoBehaviour
{
    bool IsPortrait;

    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;

        GetComponent<RectTransform>().localScale = new Vector3(0.01f, 0.01f, 1);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }

    void Update()
    {
        Canvas();
    }

    public void Canvas()
    {
        GetComponent<CanvasScaler>().referenceResolution = ScreenSize(1800);
        GetComponent<RectTransform>().sizeDelta = ScreenSize(1800);
    }

    public Vector2 ScreenSize(float Times)
    {
        if ((float)Screen.height / (float)Screen.width > 1.5f)
        {
            return new Vector2(Times, (float)Screen.height / (float)Screen.width * Times);
        }
        else
        {
            return new Vector2((float)Screen.width / (float)Screen.height * Times, Times) * 1.5f;
        }
    }
}
