using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class SafeArea : MonoBehaviour
{
    public enum areatype
    {
        Middle, Top, Under, Left, Right, Top_Middle, Middle_Under
    }

    [SerializeField] areatype AreaType;

    GameManager GM;

    void Awake()
    {
        Update();
    }

    void Start()
    {
        Update();
    }

    void Update()
    {
        if (GM == null)
        {
            GM = GameManager.instance;
        }

        int Android_Status_Bar_Height = (GM == null ? 0 : GM.Android_Status_Bar_Height);
        int Android_Navigation_Bar_Height = (GM == null ? 0 : GM.Android_Navigation_Bar_Height);
        bool Android_Status_IsShow = (GM == null ? false : GM.Android_Status_IsShow);
        bool Android_Navigation_IsShow = (GM == null ? false : GM.Android_Navigation_IsShow);

        var rect = GetComponent<RectTransform>();
        rect.sizeDelta = Vector2.zero;

        bool IsActive;

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        IsActive = true;
#else
        IsActive = (Screen.currentResolution.refreshRate == 0);
#endif

        Vector2 SafeArea_Top;
        Vector2 SafeArea_Buttom;

        if (IsActive)
        {
            var area = Screen.safeArea;
            var resolition = Screen.currentResolution;

            float yMax;
            float yMin;

#if !UNITY_EDITOR && UNITY_ANDROID

            if (Android_Status_IsShow)
            {
                yMax = 1 - ((float)Android_Status_Bar_Height / resolition.height);
            }
            else
            {
                yMax = area.yMax / resolition.height;
            }

            if (Android_Navigation_IsShow)
            {
                yMin = (float)Android_Navigation_Bar_Height / resolition.height;
            }
            else
            {
                yMin = area.yMin / resolition.height;
            }
            
#else
            yMax = area.yMax / resolition.height;
            yMin = area.yMin / resolition.height;
#endif

            SafeArea_Top = new Vector2(area.xMax / resolition.width, yMax);
            SafeArea_Buttom = new Vector2(area.xMin / resolition.width, yMin);
        }
        else
        {
            SafeArea_Top = new Vector2(1, 1);
            SafeArea_Buttom = new Vector2(0, 0);
        }

        switch (AreaType)
        {
            case areatype.Top:
                rect.anchorMax = new Vector2(1, 1);
                rect.anchorMin = new Vector2(SafeArea_Buttom.x, SafeArea_Top.y);
                break;

            case areatype.Middle:
                rect.anchorMax = SafeArea_Top;
                rect.anchorMin = SafeArea_Buttom;
                break;

            case areatype.Under:
                rect.anchorMax = new Vector2(SafeArea_Top.x, SafeArea_Buttom.y);
                rect.anchorMin = new Vector2(SafeArea_Buttom.x, 0);
                break;

            case areatype.Left:
                rect.anchorMax = new Vector2(SafeArea_Buttom.x, SafeArea_Top.y);
                rect.anchorMin = new Vector2(0, 0);
                break;

            case areatype.Right:
                rect.anchorMax = new Vector2(1, 1);
                rect.anchorMin = new Vector2(SafeArea_Top.x, 0);
                break;

            case areatype.Top_Middle:
                rect.anchorMax = new Vector2(1, 1);
                rect.anchorMin = SafeArea_Buttom;
                break;

            case areatype.Middle_Under:
                rect.anchorMax = SafeArea_Top;
                rect.anchorMin = new Vector2(SafeArea_Buttom.x, 0);
                break;
        }
    }
}
