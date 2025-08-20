using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FieldSize : MonoBehaviour
{
    GameManager GM;

    [SerializeField] RectTransform ObjectParent;

    void Start()
    {
        GM = GameManager.instance;
    }

    void Update()
    {
        const float Aspect = 16f / 9;

        if (SafeAreaSize().Size.y / SafeAreaSize().Size.x > Aspect)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(SafeAreaSize().Size.x, SafeAreaSize().Size.x * Aspect);

            ObjectParent.localScale = new Vector3(SafeAreaSize().Size.x / 720, SafeAreaSize().Size.x / 720, 1);
        }
        else
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(SafeAreaSize().Size.y / Aspect, SafeAreaSize().Size.y);

            ObjectParent.localScale = new Vector3(SafeAreaSize().Size.y / 1280, SafeAreaSize().Size.y / 1280, 1);
        }
    }



    Vector2 ScreenSize(float Times)
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

    class PosAndSize
    {
        public Vector2 Pos;
        public Vector2 Size;
    }

    PosAndSize SafeAreaSize()
    {
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

            if (GM.Android_Status_IsShow)
            {
                yMax = 1 - ((float)GM.Android_Status_Bar_Height / resolition.height);
            }
            else
            {
                yMax = area.yMax / resolition.height;
            }

            if (GM.Android_Navigation_IsShow)
            {
                yMin = (float)GM.Android_Navigation_Bar_Height / resolition.height;
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

        PosAndSize Output = new PosAndSize();

        Output.Pos = ((SafeArea_Top + SafeArea_Buttom) / 2 - new Vector2(0.5f, 0.5f)) * ScreenSize(1800);

        Output.Size = (SafeArea_Top - SafeArea_Buttom) * ScreenSize(1800);

        return Output;
    }
}
