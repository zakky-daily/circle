using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawLine : Graphic
{
    int vhCounter;

    [SerializeField] float Thickness;

    GameManager GM;
    GM_Create GM_Create;

    int Android_Status_Bar_Height;
    int Android_Navigation_Bar_Height;
    bool Android_Status_IsShow;
    bool Android_Navigation_IsShow;

    Vector2 ScreenPos;
    float ScreenScale;

    [SerializeField] RectTransform UpMenu;
    [SerializeField] RectTransform DownMenu;


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        GM = GameManager.instance;
        GM_Create = GM_Create.instance;

        Android_Status_Bar_Height = (GM == null ? 0 : GM.Android_Status_Bar_Height);
        Android_Navigation_Bar_Height = (GM == null ? 0 : GM.Android_Navigation_Bar_Height);
        Android_Status_IsShow = (GM == null ? false : GM.Android_Status_IsShow);
        Android_Navigation_IsShow = (GM == null ? false : GM.Android_Navigation_IsShow);


        vh.Clear();
        vhCounter = -1;

        ScreenPos = (GM_Create != null ? GM_Create.ScreenPos : new Vector2(0, 19));
        ScreenScale = (GM_Create != null ? GM_Create.ScreenScale : 1.3f);

        float Y_Correction = (UpMenu.sizeDelta.y - DownMenu.sizeDelta.y) / 2;


        float MagCorrection = Mathf.Log(ScreenScale, 2) + 0.2f;

        float HorizontalInterval = 360 / (Mathf.Pow(2, Mathf.Floor(MagCorrection)));
        float VerticalInterval = 640 / (Mathf.Pow(2, Mathf.Floor(MagCorrection)));

        var TransparentColor = new Color(color.r, color.g, color.b, color.a * (MagCorrection - Mathf.Floor(MagCorrection) < 0.25f ? (MagCorrection - Mathf.Floor(MagCorrection)) * 4 : 1));


        //横

        for (int y = Mathf.CeilToInt((-SafeAreaSize().Size.y / 2 + Y_Correction - ScreenPos.y) / (VerticalInterval * ScreenScale)); y < (SafeAreaSize().Size.y / 2 + Y_Correction - ScreenPos.y) / (VerticalInterval * ScreenScale); y += 1)
        {
            AddLine
            (
                vh,
                new Vector2(ScreenSize(-900).x, y * VerticalInterval * ScreenScale + ScreenPos.y),
                new Vector2(ScreenSize(900).x, y * VerticalInterval * ScreenScale + ScreenPos.y),
                Thickness,
                (y % 2 == 0 ? color : TransparentColor)
            );
        }


        //縦

        for (int x = Mathf.CeilToInt((ScreenSize(-900).x - ScreenPos.x) / (HorizontalInterval * ScreenScale)); x < (ScreenSize(900).x - ScreenPos.x) / (HorizontalInterval * ScreenScale); x += 1)
        {
            AddLine
            (
                vh,
                new Vector2(x * HorizontalInterval * ScreenScale + ScreenPos.x, -SafeAreaSize().Size.y / 2 + Y_Correction), 
                new Vector2(x * HorizontalInterval * ScreenScale + ScreenPos.x, SafeAreaSize().Size.y / 2 + Y_Correction),
                Thickness,
                (x % 2 == 0 ? color : TransparentColor)
            );
        }
    }

    void AddLine(VertexHelper vh, Vector2 StartPos, Vector2 EndPos, float Thickness, Color color)
    {
        Vector2 distance = EndPos - StartPos;
        float angle = Mathf.Atan2(distance.y, distance.x);

        vhCounter += 4;

        AddVert(vh, StartPos + new Vector2(Mathf.Cos(angle - Mathf.PI / 2), Mathf.Sin(angle - Mathf.PI / 2)) * Thickness / 2, color);
        AddVert(vh, EndPos + new Vector2(Mathf.Cos(angle - Mathf.PI / 2), Mathf.Sin(angle - Mathf.PI / 2)) * Thickness / 2, color);
        AddVert(vh, EndPos + new Vector2(Mathf.Cos(angle + Mathf.PI / 2), Mathf.Sin(angle + Mathf.PI / 2)) * Thickness / 2, color);
        AddVert(vh, StartPos + new Vector2(Mathf.Cos(angle + Mathf.PI / 2), Mathf.Sin(angle + Mathf.PI / 2)) * Thickness / 2, color);

        vh.AddTriangle(vhCounter, vhCounter - 3, vhCounter - 2);
        vh.AddTriangle(vhCounter, vhCounter - 1, vhCounter - 2);
    }

    void AddVert(VertexHelper vh, Vector2 pos, Color LineColor)
    {
        var vert = UIVertex.simpleVert;
        vert.position = pos;
        vert.color = LineColor;
        vh.AddVert(vert);
    }


    //スクリーンのサイズ

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


    public class PosAndSize
    {
        public Vector2 Pos;
        public Vector2 Size;
    }

    public PosAndSize SafeAreaSize()
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

        PosAndSize Output = new PosAndSize();

        Output.Pos = ((SafeArea_Top + SafeArea_Buttom) / 2 - new Vector2(0.5f, 0.5f)) * ScreenSize(1800);

        Output.Size = (SafeArea_Top - SafeArea_Buttom) * ScreenSize(1800);

        return Output;
    }

}
