using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GM_Create : MonoBehaviour
{
    GameManager GM;

    public static GM_Create instance;


    //ユーザーの追加したオブジェクト一覧

    public List<GameObject> UserObjects = new List<GameObject>();


    //アップメニューのUI
    [SerializeField] TextMeshProUGUI SceneName;

    [SerializeField] TextMeshProUGUI SceneIndex;


    //ポーズのアニメーション
    public bool IsBreak;
    [SerializeField] GameObject BreakObject;
    float BreakCounter;
    float BreakAnimation;
    bool BreakStating;


    //オブジェクトの追加・削除

    [SerializeField] GameObject EditObjectsParent;


    //unimgpicker

    [SerializeField] Kakera.Unimgpicker imagePicker;


    //オブジェクトの移動・拡大縮小

    [SerializeField] RectTransform[] EditSpaceExpand;
    
    public enum adjustStartObject
    {
        None,
        Background,
        Object,
        Cursor_Center,
        Curcor_Corner
    }
    [SerializeField] adjustStartObject AdjustStartObject;

    public enum adjustMode
    {
        None,
        StandBy,
        ScreenMove,
        ScreenExpand,
        ObjectMove,
        ObjectExpand
    }
    [SerializeField] adjustMode AdjustMode;

    public Vector2 ScreenPos;
    public float ScreenScale;
    public Vector2 MoveObjectPos;
    public Vector2 MoveObjectSize;

    Vector2 Set1stFinger;
    Vector2 Set2ndFinger;
    Vector2 SetScreenPos;
    float SetScreenScale;
    Vector2 SetMoveObjectPos;
    Vector2 SetMoveObjectSize;

    int Tempid;
    Vector2 CirclePos;
    float MoveCounter;

    public bool IsViewCursor;
    float ViewCursorAnimation;

    [SerializeField] GameObject ViewCursor;
    [SerializeField] GameObject CursorFrame;
    [SerializeField] GameObject[] CursorCircle;

    [SerializeField] DrawLine DrawLine;


    //再生画面

    [SerializeField] Image ExecuteFade;

    AsyncOperation ExecuteaAsync;


    //終了画面

    public bool IsFinish;

    [SerializeField] CanvasGroup FinishObject;

    float FinishAnimation;


    public bool IsGotoHome;

    [SerializeField] GameObject GotoHomeLoading;
    [SerializeField] Image GotoHomeFilter;

    float GotoHomeAnimation;

    AsyncOperation HomeAsync;


    //追加メニュー

    public bool IsAddMenu;

    [SerializeField] GameObject AddMenuCursor;
    [SerializeField] GameObject AddMenuButton;

    float AddMenuAnimation;

    bool IsAddMenuRoot = true;
    float AddMenuPageAnimation;


    ///オブジェクト

    [SerializeField] GameObject UIAddMenuObject;

    enum uiAddMenuPage{ StanderedShape }
    
    uiAddMenuPage UIAddMenuPage;

    [SerializeField] GameObject UIAddMenu_Root;
    [SerializeField] GameObject UIAddMenu_StanderedShape;


    ///プログラミングのアイコン

    [SerializeField] GameObject ProgrammingAddMenuObject;

    [Serializable]
    class messageAddMenu_Receive
    {
        public enum page{ Move }
        public page Page;
        
        public GameObject Parent;
        public GameObject Root;
        public GameObject Move;
    }
    [SerializeField] messageAddMenu_Receive MessageAddMenu_Receive;


    [Serializable]
    class messageAddMenu_Send
    {
        public enum page{ }
        public page Page;

        public GameObject Parent;
        public GameObject Root;
    }
    [SerializeField] messageAddMenu_Send MessageAddMenu_Send;


    //削除

    [SerializeField] Button ObjectDeleteButton;


    //プログラミングメニュー

    public bool IsProgrammingWindow;

    [SerializeField] GameObject ProgrammingWindowObject;
    [SerializeField] GameObject ProgrammingWindowCursor;
    [SerializeField] GameObject ProgrammingWindowButton;

    float ProgrammingWindowAnimation;

    [SerializeField] GameObject PW_ObjectImage;
    [SerializeField] TextMeshProUGUI PW_ObjectName;


    //トグル

    [SerializeField] GameObject PW_ToggleCursor;
    [SerializeField] RectTransform PW_ReceiveList;
    [SerializeField] RectTransform PW_SendList;

    float PW_ToggleAnimation;

    [SerializeField] GameObject PW_ReceiveContent;
    [SerializeField] GameObject PW_SendContent;

    [SerializeField] DetectExpandScroll PW_ReceiveScrollRect;
    [SerializeField] DetectExpandScroll PW_SendScrollRect;

    [SerializeField] GameObject PW_ReceivePrefab;
    [SerializeField] GameObject PW_SendPrefab;
    [SerializeField] GameObject PW_AddButtonPrefab;


    //追加ウィンドウ

    public bool PW_IsAddMessageWindow;

    [SerializeField] GameObject PW_AddMessageObject;

    float PW_AddMessageWindowAnimation;


    ///新規

    [SerializeField] TMP_InputField PW_AM_InputField;

    [SerializeField] TextMeshProUGUI PW_AM_ErrorMessage;


    ///選択

    [SerializeField] GameObject PW_AM_SelectPrefab;

    [SerializeField] GameObject PW_AM_SelectParent;

    [SerializeField] RectTransform PW_AM_SelectContent;

    List<GameObject> PW_AM_SelectObjectList = new List<GameObject>();

    [SerializeField] GameObject PW_AM_NoMessage;


    int PW_AM_SelectID = -1;

    [SerializeField] GameObject PW_AM_SelectCursor;
    [SerializeField] Image PW_AM_SelectCursorBlack;

    public float PW_AM_SelectMessageAnimation;

    [SerializeField] DetectExpandScroll PW_AM_ScrollRect;


    //追加ウィンドウのトグル

    public bool PW_AM_IsSelect;

    [SerializeField] GameObject PW_AM_ToggleCursor;
    [SerializeField] RectTransform PW_AM_NewList;
    [SerializeField] RectTransform PW_AM_SelectList;

    float PW_AM_ToggleAnimation;


    [SerializeField] Button PW_AM_CreateButton;
    [SerializeField] GameObject PW_AM_CreateText;


    //各メッセージ設定

    public bool PW_IsMessageDetail;

    [SerializeField] GameObject PW_MessageDetailObject;
    [SerializeField] GameObject PW_MessageDetailWindow;

    float PW_MessageDetailAnimation;


    [SerializeField] TextMeshProUGUI PW_MD_MessageName;


    [SerializeField] List<GameObject> PW_MD_ProgrammingObjectList = new List<GameObject>();

    [SerializeField] DetectExpandScroll PW_MD_ScrollRect;
    [SerializeField] RectTransform PW_MD_Content;

    [SerializeField] GameObject PW_MD_ProgrammingPrefab;

    [SerializeField] GameObject PW_MD_NoMessage;


    public bool PW_MD_IsTextEdit;

    [SerializeField] CanvasGroup PW_MD_TextEditObject;

    [SerializeField] TMP_Dropdown PW_MD_TE_Dropdown;
    [SerializeField] TMP_InputField PW_MD_TE_Value;

    float PW_MD_TextEditAnimation;

    
    public bool PW_MD_IsCSEdit;

    [SerializeField] CanvasGroup PW_MD_CSEditObject;

    [SerializeField] TMP_InputField PW_MD_CS_Value;

    float PW_MD_CSEditAnimation;


    //色ウィンドウ

    public bool IsColorWindow;

    [SerializeField] CanvasGroup ColorWindowObject;

    float ColorWindowAnimation;

    [SerializeField] Button ColorWindowButton;

    [SerializeField] GameObject ColorWindowButtonCursor;


    int CW_H; int CW_S; int CW_V;

    int CW_R; int CW_G; int CW_B;


    public bool CW_IsSVMove;

    [SerializeField] RectTransform CW_SVCursorObject;

    [SerializeField] Image CW_SVColorObject;

    Vector2 CW_SVCursorPos;

    Vector2 CW_SetSVCursorPos;


    public bool CW_IsHMove;

    [SerializeField] RectTransform CW_HCursorObject;

    float CW_HCursorAngle;

    Vector2 CW_SetHCursorPos;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GM = GameManager.instance;

        GM.Android_Bar_Show(false, true, 0x00000000, 0xff000000, true, false);

        
        GM.Nowscene = GM.PD.Ns;


        //unimgpicker

        imagePicker.Completed += (string path) =>
        {
            StartCoroutine(UIAddMenuDeviceResponse(path));
        };


        //データ読み込み
        GM.PD = GM.LoadJson<projectdata>();


        //非アクティブ化を解除

        BreakObject.SetActive(true);

        ExecuteFade.gameObject.SetActive(true);

        FinishObject.gameObject.SetActive(true);
        GotoHomeFilter.gameObject.SetActive(true);

        UIAddMenuObject.SetActive(false);
        ProgrammingAddMenuObject.SetActive(false);

        ProgrammingWindowObject.SetActive(true);

        PW_ReceiveList.gameObject.SetActive(true);

        PW_SendList.gameObject.SetActive(true);

        PW_AddMessageObject.SetActive(true);

        PW_AM_NewList.gameObject.SetActive(true);
        PW_AM_SelectList.gameObject.SetActive(true);

        PW_AM_SelectCursor.SetActive(true);

        PW_MessageDetailObject.SetActive(true);

        PW_MD_TextEditObject.gameObject.SetActive(true);

        ColorWindowObject.gameObject.SetActive(true);


        //ゲームオブジェクトの初期配置

        StartCoroutine(GM.ObjectStartArrange(EditObjectsParent.transform, UserObjects));


        //諸設定

        ScreenPos = GM.PD_NowS().P;
        ScreenScale = GM.PD_NowS().S;

        foreach(var rect in EditSpaceExpand)
        {
            rect.anchoredPosition = ScreenPos;
            rect.localScale = new Vector3(ScreenScale, ScreenScale, 1);
        }

        DrawLine.SetVerticesDirty();

        AdjustPush();

        SceneName.text = GM.PD_NowS().N;
        SceneIndex.text = (GM.PD.s.ToList().IndexOf(GM.Nowscene) + 1).ToString();

        StartCoroutine(ReflectJsonToAddMessageSelect());
    }

    void Update()
    {
        //オブジェクトの移動・拡大縮小

        if (AdjustMode != adjustMode.None)
        {
            MoveCounter += Time.deltaTime;

            //Mode, Objectの変更

            if (GM.posCount() == 0 || GM.posCount() >= 3)
            {
                if (AdjustMode == adjustMode.StandBy)
                {
                    if (AdjustStartObject == adjustStartObject.Background || AdjustStartObject == adjustStartObject.Cursor_Center)
                    {
                        IsViewCursor = false;
                    }
                    else if (AdjustStartObject == adjustStartObject.Object)
                    {
                        if (!IsViewCursor)
                        {
                            GM.View_SelectObject = Tempid;
                            AdjustPush();
                            IsViewCursor = true;
                            StartCoroutine(ReflectJsonToMessageAll());
                        }
                        else
                        {
                            if (GM.View_SelectObject == Tempid)
                            {
                                IsViewCursor = false;
                            }
                            else
                            {
                                GM.View_SelectObject = Tempid;
                                ViewCursorAnimation = 0;
                                AdjustPush();
                                StartCoroutine(ReflectJsonToMessageAll());
                            }
                        }
                    }
                }
                
                AdjustStartObject = adjustStartObject.None;
                AdjustMode = adjustMode.None;

                GM.PD_NowS().P = ScreenPos;
                GM.PD_NowS().S = ScreenScale;

                GM.PD_NowO().P = MoveObjectPos;
                GM.PD_NowO().S = MoveObjectSize;

                GM.SaveJson();
            }

            switch (AdjustStartObject)
            {
                case adjustStartObject.Background:

                    switch (AdjustMode)
                    {
                        case adjustMode.StandBy:

                            if (GM.posCount() == 2)
                            {
                                AdjustPush();
                                AdjustMode = adjustMode.ScreenExpand;
                            }
                            else if ((GM.pos() - Set1stFinger).sqrMagnitude > 100)
                            {
                                AdjustPush();
                                AdjustMode = adjustMode.ScreenMove;
                            }

                        break;

                        case adjustMode.ScreenMove:

                            if (GM.posCount() == 2)
                            {
                                AdjustPush();
                                AdjustMode = adjustMode.ScreenExpand;
                            }

                        break;

                        case adjustMode.ScreenExpand:

                            if (GM.posCount() == 1)
                            {
                                AdjustPush();
                                AdjustMode = adjustMode.ScreenMove;
                            }

                        break;
                    }

                break;

                case adjustStartObject.Object:

                    switch (AdjustMode)
                    {
                        case adjustMode.StandBy:

                            if (GM.posCount() == 2)
                            {
                                AdjustPush();
                                AdjustStartObject = adjustStartObject.Background;
                                AdjustMode = adjustMode.ScreenExpand;
                            }
                            else if ((GM.pos() - Set1stFinger).sqrMagnitude > 100)
                            {
                                AdjustPush();
                                AdjustStartObject = adjustStartObject.Background;
                                AdjustMode = adjustMode.ScreenMove;
                            }
                            else if (MoveCounter > 0.2f)
                            {
                                ViewCursorAnimation = 0;
                                GM.View_SelectObject = Tempid;
                                AdjustPush();
                                AdjustMode = adjustMode.ObjectMove;
                                IsViewCursor = true;
                                StartCoroutine(ReflectJsonToMessageAll());
                            }

                        break;

                        case adjustMode.ObjectMove:

                            if (GM.posCount() == 2)
                            {
                                AdjustPush();
                                AdjustStartObject = adjustStartObject.Background;
                                AdjustMode = adjustMode.ScreenExpand;
                            }

                        break;
                    }

                break;

                case adjustStartObject.Cursor_Center:

                    switch (AdjustMode)
                    {
                        case adjustMode.StandBy:

                            if (GM.posCount() == 2)
                            {
                                AdjustPush();
                                AdjustStartObject = adjustStartObject.Background;
                                AdjustMode = adjustMode.ScreenExpand;
                            }
                            else if ((GM.pos() - Set1stFinger).sqrMagnitude > 100)
                            {
                                AdjustPush();
                                AdjustStartObject = adjustStartObject.Object;
                                AdjustMode = adjustMode.ObjectMove;
                            }

                        break;
                    }

                break;

                case adjustStartObject.Curcor_Corner:

                    switch (AdjustMode)
                    {
                        case adjustMode.ObjectExpand:

                            if (GM.posCount() == 2)
                            {
                                AdjustPush();
                                AdjustStartObject = adjustStartObject.Background;
                                AdjustMode = adjustMode.ScreenExpand;
                            }

                        break;
                    }

                break;
            }


            //ScreenやObjectを移動

            switch (AdjustStartObject)
            {
                case adjustStartObject.Background:

                    switch (AdjustMode)
                    {
                        case adjustMode.ScreenMove:

                            ScreenPos = SetScreenPos + GM.pos() - Set1stFinger;

                            ScreenScale = SetScreenScale;

                        break;

                        case adjustMode.ScreenExpand:

                            float expand = (GM.pos(0) - GM.pos(1)).magnitude / (Set1stFinger - Set2ndFinger).magnitude;

                            ScreenPos = (GM.pos(0) + GM.pos(1)) / 2 + (SetScreenPos - (Set1stFinger + Set2ndFinger) / 2) * expand;

                            ScreenScale = SetScreenScale * expand;

                        break;
                    }

                break;

                case adjustStartObject.Object:

                    switch (AdjustMode)
                    {
                        case adjustMode.ObjectMove:

                            MoveObjectPos = SetMoveObjectPos + (GM.pos() - Set1stFinger) / ScreenScale;

                        break;
                    }

                break;

                case adjustStartObject.Curcor_Corner:

                    switch (AdjustMode)
                    {
                        case adjustMode.ObjectExpand:

                            float SetCoefficient = Vector2.Dot((GM.pos() - Set1stFinger) / ScreenScale, Quaternion.Euler(0, 0, GM.PD_NowO().A) * CirclePos * SetMoveObjectSize) / (CirclePos * SetMoveObjectSize).sqrMagnitude;

                            MoveObjectPos = (Quaternion.Euler(0, 0, GM.PD_NowO().A) * (SetMoveObjectSize * CirclePos)) / 2 * SetCoefficient;
                            MoveObjectPos += SetMoveObjectPos;
                            MoveObjectSize = SetMoveObjectSize * (SetCoefficient * new Vector2(Mathf.Abs(CirclePos.x), Mathf.Abs(CirclePos.y)) + Vector2.one);

                        break;
                    }

                break;
            }

            foreach(var rect in EditSpaceExpand)
            {
                rect.anchoredPosition = ScreenPos;
                rect.localScale = new Vector3(ScreenScale, ScreenScale, 1);
            }

            DrawLine.SetVerticesDirty();

            if (NowUserObject() != null)
            {
                NowUserObject().GetComponent<RectTransform>().anchoredPosition = MoveObjectPos;
                NowUserObject().GetComponent<RectTransform>().localScale = new Vector3(Mathf.Sign(MoveObjectSize.x), Mathf.Sign(MoveObjectSize.y), 1);
                NowUserObject().GetComponent<RectTransform>().sizeDelta = new Vector3(Mathf.Abs(MoveObjectSize.x), Mathf.Abs(MoveObjectSize.y), 1);
            }
        }

        //Cursorの位置、表示

        if (IsViewCursor)
        {
            Vector2 pos = (AdjustMode == adjustMode.None ? GM.PD_NowO().P : MoveObjectPos);
            Vector2 size = (AdjustMode == adjustMode.None ? GM.PD_NowO().S : MoveObjectSize);

            CursorFrame.GetComponent<RectTransform>().localScale = new Vector3(Mathf.Sign(size.x), Mathf.Sign(size.y), 1);

            size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));

            CursorFrame.GetComponent<RectTransform>().anchoredPosition = pos * ScreenScale + ScreenPos;
            CursorFrame.GetComponent<RectTransform>().sizeDelta = size * ScreenScale;

            CursorFrame.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, GM.PD_NowO().A);

            float PaddingValue = (120 - Mathf.Min(size.x * ScreenScale, size.y * ScreenScale)) / 4;

            if (PaddingValue < -40) PaddingValue = -40;
            if (PaddingValue > 0) PaddingValue = 0;

            foreach (var circle in CursorCircle)
            {
                circle.GetComponent<Image>().raycastPadding = new Vector4(PaddingValue, PaddingValue, PaddingValue, PaddingValue);
            }
        
        }

        ViewCursor.GetComponent<CanvasGroup>().blocksRaycasts = IsViewCursor;

        GM.Animation(ref ViewCursorAnimation, 6, IsViewCursor);

        ViewCursor.GetComponent<CanvasGroup>().alpha = ViewCursorAnimation;


        //マウスでのスクロール
        
        if (!IsAddMenu && !IsProgrammingWindow)
        {
            float multiplication = 1.1f;

            if (Input.mouseScrollDelta.y != 0)
            {
                if (Input.mouseScrollDelta.y > 0)
                {
                    ScreenPos *= multiplication;
                    ScreenScale *= multiplication;
                }
                else
                {
                    ScreenPos /= multiplication;
                    ScreenScale /= multiplication;
                }

                GM.PD_NowS().P = ScreenPos;
                GM.PD_NowS().S = ScreenScale;
                GM.SaveJson();

                foreach(var rect in EditSpaceExpand)
                {
                    rect.anchoredPosition = ScreenPos;
                    rect.localScale = new Vector3(ScreenScale, ScreenScale, 1);
                }

                DrawLine.SetVerticesDirty();
            }
        }


        //ポーズアニメーション

        if (BreakStating && !IsBreak)
        {
            BreakCounter += Time.deltaTime;

            if (BreakCounter >= 2)
            {
                IsBreak = true;

                if (IsAddMenu)
                {
                    SwitchAddMenu();
                }

                if (IsProgrammingWindow)
                {
                    SwitchProgrammingWindow();
                }
            }
        }

        BreakObject.GetComponent<CanvasGroup>().blocksRaycasts = IsBreak;

        GM.Animation(ref BreakAnimation, 6, IsBreak);
        BreakObject.GetComponent<RectTransform>().localScale = new Vector3(0.95f + (BreakAnimation * 0.05f), 0.95f + (BreakAnimation * 0.05f), 1);
        BreakObject.GetComponent<CanvasGroup>().alpha = BreakAnimation;


        //シーン変更画面




        //再生画面

        ExecuteFade.raycastTarget = GM.IsExecute;
        ExecuteFade.color = new Color32(0, 0, 0, (byte)(GM.ExecuteAnimation > 0.5f ? 255 : GM.ExecuteAnimation * 2 * 255));

        if (GM.IsExecute && GM.ExecuteAnimation == 1)
        {
            ExecuteaAsync.allowSceneActivation = true;
        }


        //終了画面

        FinishObject.blocksRaycasts = IsFinish;

        GM.Animation(ref FinishAnimation, 6, IsFinish);

        FinishObject.alpha = FinishAnimation;


        GotoHomeLoading.SetActive(IsGotoHome && GotoHomeAnimation < 2.9f);
        GotoHomeFilter.raycastTarget = IsGotoHome; 

        if (IsGotoHome)
        {
            GotoHomeAnimation += Time.deltaTime;

            if (GotoHomeAnimation > 3)
            {
                HomeAsync.allowSceneActivation = true;
            }
        }

        GotoHomeFilter.color = new Color32(0, 0, 0, (byte)(GotoHomeAnimation < 2 ? 0 : GotoHomeAnimation > 2.5f ? 255 : (GotoHomeAnimation - 2) * 2 * 255));



        //UI追加メニュー

        var AddMenuObject = (PW_IsMessageDetail ? ProgrammingAddMenuObject : UIAddMenuObject);

        AddMenuObject.GetComponent<CanvasGroup>().blocksRaycasts = IsAddMenu;

        GM.Animation(ref AddMenuAnimation, 6, IsAddMenu);

        AddMenuObject.GetComponent<CanvasGroup>().alpha = AddMenuAnimation;

        AddMenuCursor.GetComponent<CanvasGroup>().alpha = AddMenuAnimation;
        AddMenuCursor.GetComponent<RectTransform>().anchorMin = new Vector2(0.2f * (1 - AddMenuAnimation), 0);
        AddMenuCursor.GetComponent<RectTransform>().anchorMax = new Vector2(0.2f * (4 + AddMenuAnimation), 0);


        AddMenuButton.GetComponent<Button>().interactable = !IsBreak && !IsColorWindow && (!IsProgrammingWindow || PW_IsMessageDetail);


        GM.Animation(ref AddMenuPageAnimation, 6, !IsAddMenuRoot);


        if (PW_IsMessageDetail)
        {
            if (GM.PW_IsSend)
            {
                MessageAddMenu_Send.Root.GetComponent<RectTransform>().anchorMin = new Vector2(- AddMenuPageAnimation, 0);
                MessageAddMenu_Send.Root.GetComponent<RectTransform>().anchorMax = new Vector2(1 - AddMenuPageAnimation, 1);
            }
            else
            {
                MessageAddMenu_Receive.Root.GetComponent<RectTransform>().anchorMin = new Vector2(- AddMenuPageAnimation, 0);
                MessageAddMenu_Receive.Root.GetComponent<RectTransform>().anchorMax = new Vector2(1 - AddMenuPageAnimation, 1);

                if (MessageAddMenu_Receive.Page == messageAddMenu_Receive.page.Move)
                {
                    MessageAddMenu_Receive.Move.GetComponent<RectTransform>().anchorMin = new Vector2(1 - AddMenuPageAnimation, 0);
                    MessageAddMenu_Receive.Move.GetComponent<RectTransform>().anchorMax = new Vector2(2 - AddMenuPageAnimation, 1);
                }
            }
        }
        else
        {
            UIAddMenu_Root.GetComponent<RectTransform>().anchorMin = new Vector2(- AddMenuPageAnimation, 0);
            UIAddMenu_Root.GetComponent<RectTransform>().anchorMax = new Vector2(1 - AddMenuPageAnimation, 1);

            if (UIAddMenuPage == uiAddMenuPage.StanderedShape)
            {
                UIAddMenu_StanderedShape.GetComponent<RectTransform>().anchorMin = new Vector2(1 - AddMenuPageAnimation, 0);
                UIAddMenu_StanderedShape.GetComponent<RectTransform>().anchorMax = new Vector2(2 - AddMenuPageAnimation, 1);
            }
        }


        //削除

        ObjectDeleteButton.interactable = !IsBreak && !IsColorWindow && IsViewCursor && !IsAddMenu && !IsProgrammingWindow;



        //プログラミングウィンドウ

        ProgrammingWindowObject.GetComponent<CanvasGroup>().blocksRaycasts = IsProgrammingWindow;

        GM.Animation(ref ProgrammingWindowAnimation, 6, IsProgrammingWindow);

        ProgrammingWindowObject.GetComponent<CanvasGroup>().alpha = ProgrammingWindowAnimation;

        ProgrammingWindowCursor.GetComponent<CanvasGroup>().alpha = ProgrammingWindowAnimation;
        ProgrammingWindowCursor.GetComponent<RectTransform>().anchorMin = new Vector2(0.2f * (1 - ProgrammingWindowAnimation), 0);
        ProgrammingWindowCursor.GetComponent<RectTransform>().anchorMax = new Vector2(0.2f * (4 + ProgrammingWindowAnimation), 0);

        ProgrammingWindowButton.GetComponent<Button>().interactable = IsViewCursor && !IsColorWindow && !IsBreak && !IsAddMenu && !PW_IsMessageDetail;


        //トグル

        GM.Animation(ref PW_ToggleAnimation, 6, GM.PW_IsSend);

        PW_ToggleCursor.GetComponent<RectTransform>().anchorMin = new Vector2((PW_ToggleAnimation) * 0.5f, 0);
        PW_ToggleCursor.GetComponent<RectTransform>().anchorMax = new Vector2((1 + PW_ToggleAnimation) * 0.5f, 1);

        PW_ToggleCursor.GetComponent<RectTransform>().offsetMin = new Vector2(PW_ToggleAnimation * 10, 0);
        PW_ToggleCursor.GetComponent<RectTransform>().offsetMax = new Vector2((PW_ToggleAnimation - 1) * 10, 0);

        PW_ReceiveList.anchorMin = new Vector2((- PW_ToggleAnimation), 0);
        PW_ReceiveList.anchorMax = new Vector2((1 - PW_ToggleAnimation), 1);

        PW_SendList.anchorMin = new Vector2((1 - PW_ToggleAnimation), 0);
        PW_SendList.anchorMax = new Vector2((2 - PW_ToggleAnimation), 1);


        //追加ウィンドウ

        PW_AddMessageObject.GetComponent<CanvasGroup>().blocksRaycasts = PW_IsAddMessageWindow;

        GM.Animation(ref PW_AddMessageWindowAnimation, 6, PW_IsAddMessageWindow);

        PW_AddMessageObject.GetComponent<CanvasGroup>().alpha = PW_AddMessageWindowAnimation;


        PW_AM_CreateButton.interactable = (PW_AM_IsSelect ? (PW_AM_SelectID != -1) : (PW_AM_InputField.text != ""));
        Color32 CreateColor = PW_AM_CreateText.GetComponent<ButtonChild>().DefaultColor;
        PW_AM_CreateText.GetComponent<ButtonChild>().DefaultColor = new Color32((byte)CreateColor.r, (byte)CreateColor.g, (byte)CreateColor.b, (byte)(PW_AM_CreateButton.interactable ? 255: 127));


        //追加ウィンドウのトグル

        GM.Animation(ref PW_AM_ToggleAnimation, 6, PW_AM_IsSelect);

        PW_AM_ToggleCursor.GetComponent<RectTransform>().anchorMin = new Vector2((PW_AM_ToggleAnimation) * 0.5f, 0);
        PW_AM_ToggleCursor.GetComponent<RectTransform>().anchorMax = new Vector2((1 + PW_AM_ToggleAnimation) * 0.5f, 1);

        PW_AM_ToggleCursor.GetComponent<RectTransform>().offsetMin = new Vector2(PW_AM_ToggleAnimation * 10, 0);
        PW_AM_ToggleCursor.GetComponent<RectTransform>().offsetMax = new Vector2((PW_AM_ToggleAnimation - 1) * 10, 0);

        PW_AM_NewList.anchorMin = new Vector2((- PW_AM_ToggleAnimation), 0);
        PW_AM_NewList.anchorMax = new Vector2((1 - PW_AM_ToggleAnimation), 1);

        PW_AM_SelectList.anchorMin = new Vector2((1 - PW_AM_ToggleAnimation), 0);
        PW_AM_SelectList.anchorMax = new Vector2((2 - PW_AM_ToggleAnimation), 1);


        //追加ウィンドウの選択画面

        PW_AM_SelectCursor.GetComponent<CanvasGroup>().blocksRaycasts = (PW_AM_SelectID != -1);

        GM.Animation(ref PW_AM_SelectMessageAnimation, 6, (PW_AM_SelectID != -1));

        PW_AM_SelectCursor.GetComponent<CanvasGroup>().alpha = ((PW_AM_SelectID == -1) ? 0 : PW_AM_SelectMessageAnimation);


        //各メッセージ設定

        PW_MessageDetailObject.GetComponent<CanvasGroup>().blocksRaycasts = PW_IsMessageDetail;

        GM.Animation(ref PW_MessageDetailAnimation, 6, PW_IsMessageDetail);

        PW_MessageDetailObject.GetComponent<CanvasGroup>().alpha = PW_MessageDetailAnimation;

        PW_MessageDetailWindow.GetComponent<RectTransform>().offsetMin = new Vector2(40 - PW_MessageDetailAnimation * 40, 80 - PW_MessageDetailAnimation * 70);
        PW_MessageDetailWindow.GetComponent<RectTransform>().offsetMax = new Vector2(PW_MessageDetailAnimation * 40 - 40, PW_MessageDetailAnimation * 70 - 80);

        PW_MessageDetailWindow.GetComponent<RoundedShape>().Radius = (int)((1 - PW_MessageDetailAnimation) * 100);


        //テキスト編集ウィンドウ

        PW_MD_TextEditObject.blocksRaycasts = PW_MD_IsTextEdit;

        GM.Animation(ref PW_MD_TextEditAnimation, 6, PW_MD_IsTextEdit);

        PW_MD_TextEditObject.alpha = PW_MD_TextEditAnimation;


        //シーン変更先ウィンドウ

        PW_MD_CSEditObject.blocksRaycasts = PW_MD_IsCSEdit;

        GM.Animation(ref PW_MD_CSEditAnimation, 6, PW_MD_IsCSEdit);

        PW_MD_CSEditObject.alpha = PW_MD_CSEditAnimation;


        //色ウィンドウ

        ColorWindowObject.blocksRaycasts = IsColorWindow;

        GM.Animation(ref ColorWindowAnimation, 6, IsColorWindow);

        ColorWindowObject.alpha = ColorWindowAnimation;

        
        ColorWindowButton.interactable = !IsBreak && IsViewCursor && !IsProgrammingWindow;

        ColorWindowButtonCursor.GetComponent<CanvasGroup>().alpha = ColorWindowAnimation;
        ColorWindowButtonCursor.GetComponent<RectTransform>().anchorMin = new Vector2(0.2f * (1 - ColorWindowAnimation), 0);
        ColorWindowButtonCursor.GetComponent<RectTransform>().anchorMax = new Vector2(0.2f * (4 + ColorWindowAnimation), 0);


        if (CW_IsSVMove)
        {
            CW_SVCursorPos = CW_SetSVCursorPos + GM.pos() - Set1stFinger;

            CW_SVCursorPos.x = Mathf.Clamp(CW_SVCursorPos.x, -320, 320);

            CW_SVCursorPos.y = Mathf.Clamp(CW_SVCursorPos.y, -320, 320);

            CW_SVCursorObject.anchoredPosition = CW_SVCursorPos;

            CW_S = (int)((CW_SVCursorPos.x / 640f + 0.5f) * 255);
            CW_V = (int)((CW_SVCursorPos.y / 640f + 0.5f) * 255);
        }

        if (CW_IsHMove)
        {
            var FreePos = CW_SetHCursorPos + GM.pos() - Set1stFinger;

            CW_HCursorAngle = Mathf.Atan2(FreePos.y, FreePos.x);

            CW_HCursorObject.anchoredPosition = 590 * new Vector2(Mathf.Cos(CW_HCursorAngle), Mathf.Sin(CW_HCursorAngle));

            CW_H = (int)(CW_HCursorAngle * Mathf.Rad2Deg);
            if (CW_H < 0) CW_H += 360;
        }

        if (IsColorWindow)
        {
            for (int i = 0; i < 2; i++)
            {
                int s; int v;

                if (i == 0)
                {
                    s = CW_S; v = CW_V;
                }
                else
                {
                    s = 255; v = 255;
                }

                int max = v;
                int min = (int)(max - s / 255f * max);

                int r; int g; int b;

                if (CW_H < 60)
                {
                    r = max;
                    g = (int)(CW_H / 60f * (max - min) + min);
                    b = min;
                }
                else if (CW_H < 120)
                {
                    r = (int)((120 - CW_H) / 60f * (max - min) + min);
                    g = max;
                    b = min;
                }
                else if (CW_H < 180)
                {
                    r = min;
                    g = max;
                    b = (int)((CW_H - 120) / 60f * (max - min) + min);
                }
                else if (CW_H < 240)
                {
                    r = min;
                    g = (int)((240 - CW_H) / 60f * (max - min) + min);
                    b = max;
                }
                else if (CW_H < 300)
                {
                    r = (int)((CW_H - 240) / 60f * (max - min) + min);
                    g = min;
                    b = max;
                }
                else
                {
                    r = max;
                    g = min;
                    b = (int)((360 - CW_H) / 60f * (max - min) + min);
                }

                if (i == 0)
                {
                    //Debug.Log(g); //($"{r} , {g} , {b}");

                    CW_R = r; CW_G = g; CW_B = b;
                }
                else
                {
                    CW_SVColorObject.color = new Color32((byte)r, (byte)g, (byte)b, 255);
                }
            }
        }
    }


    //関数一覧

    //オブジェクト操作に関するやつ

    public void PushBackground()
    {
        if (GM.posCount() == 2)
        {
            AdjustStartObject = adjustStartObject.Background;
            AdjustMode = adjustMode.ScreenExpand;
        }
        else if (GM.posCount() == 1)
        {
            AdjustStartObject = adjustStartObject.Background;
            AdjustMode = adjustMode.StandBy;
        }

        AdjustPush();
    }

    public void PushObject(int id)
    {
        if (GM.posCount() == 2)
        {
            AdjustStartObject = adjustStartObject.Background;
            AdjustMode = adjustMode.ScreenExpand;
        }
        else if (GM.posCount() == 1)
        {
            Tempid = id;

            AdjustStartObject = adjustStartObject.Object;
            AdjustMode = adjustMode.StandBy;
        }

        AdjustPush();
    }

    public void PushCursor_Center()
    {
        if (GM.posCount() == 2)
        {
            AdjustStartObject = adjustStartObject.Background;
            AdjustMode = adjustMode.ScreenExpand;
        }
        else if (GM.posCount() == 1)
        {
            AdjustStartObject = adjustStartObject.Cursor_Center;
            AdjustMode = adjustMode.StandBy;
        }

        AdjustPush();
    }

    public void PushCursor_Corner(Vector2 circlePos)
    {
        if (GM.posCount() == 2)
        {
            AdjustStartObject = adjustStartObject.Background;
            AdjustMode = adjustMode.ScreenExpand;
        }
        else if (GM.posCount() == 1)
        {
            AdjustStartObject = adjustStartObject.Curcor_Corner;
            AdjustMode = adjustMode.ObjectExpand;

            CirclePos = circlePos;
        }

        AdjustPush();
    }

    void AdjustPush()
    {
        Set1stFinger = GM.pos(0);
        Set2ndFinger = GM.pos(1);
        SetScreenPos = EditSpaceExpand[0].GetComponent<RectTransform>().anchoredPosition;
        SetScreenScale = EditSpaceExpand[0].GetComponent<RectTransform>().localScale.x;
        SetMoveObjectPos = GM.PD_NowO().P;
        MoveObjectPos = GM.PD_NowO().P;
        SetMoveObjectSize = GM.PD_NowO().S;
        MoveObjectSize = GM.PD_NowO().S;

        MoveCounter = 0;
    }


    //選択中のオブジェクト（画面上のリアルの）を探す

    public GameObject NowUserObject()
    {
        if (GM.PD_NowS().O == null || GM.PD_NowS().O.Length == 0 || !GM.PD_NowS().o.Contains(GM.View_SelectObject))
        {
            return null;
        }
        else
        {
            return UserObjects[GM.PD_NowS().o.ToList().IndexOf(GM.View_SelectObject)];
        }
    }


    //隠し要素（休憩画面）

    public void SwitchBreak()
    {
        if (!IsBreak)
        {
            BreakStating = true;
        }
        else
        {
            IsBreak = false;
            BreakStating = false;
            BreakCounter = 0;
        }
    }

    public void StopBreakCounter()
    {
        if (!IsBreak)
        {
            BreakStating = false;
            BreakCounter = 0;
        }
    }


    //再生画面へ

    public void GotoExecute()
    {
        ExecuteaAsync = SceneManager.LoadSceneAsync("Execute");
        ExecuteaAsync.allowSceneActivation = false;

        GM.IsExecute = true;
    }


    //シーン変更画面
    public void GotoChangeSceneScreen()
    {
        //ScreenCapture.CaptureScreenshot(GM.Path(0, "snapshot.png"));
        
        StartCoroutine(GM.UpdateEditCapture());

        StartCoroutine(GotoChangeSceneScreenAsync());

        //SceneManager.LoadScene("ChangeSceneScreen");
    }


    public IEnumerator GotoChangeSceneScreenAsync()
    {
        var ao = SceneManager.LoadSceneAsync("ChangeSceneScreen");
        ao.allowSceneActivation = false;

        while (true)
        {
            yield return null;

            if (ao.progress >= 0.9f)
            {
                ao.allowSceneActivation = true;
                break;
            }
        }
    }


    //終了画面へ

    public void SwitchFinish()
    {
        IsFinish = !IsFinish;
    }

    public void GotoHome()
    {
        HomeAsync = SceneManager.LoadSceneAsync("Home");
        HomeAsync.allowSceneActivation = false;

        IsGotoHome = true;
    }


    //UI追加メニュー（表示・非表示）に関するやつ

    public void SwitchAddMenu()
    {
        IsAddMenu = !IsAddMenu;

        if (IsAddMenu)
        {
            IsAddMenuRoot = true;
            AddMenuPageAnimation = 0;

            UIAddMenuObject.SetActive(!PW_IsMessageDetail);
            ProgrammingAddMenuObject.SetActive(PW_IsMessageDetail);

            if (PW_IsMessageDetail)
            {
                MessageAddMenu_Receive.Parent.SetActive(!GM.PW_IsSend);
                MessageAddMenu_Send.Parent.SetActive(GM.PW_IsSend);
            }
        }
    }

    public void AddMenuGotoPage(string PageName)
    {
        if (PW_IsMessageDetail)
        {
            if (GM.PW_IsSend)
            {

            }
            else
            {
                if (PageName == "Move")
                {
                    MessageAddMenu_Receive.Page = messageAddMenu_Receive.page.Move;
                }
            }
        }
        else
        {
            if (PageName == "StanderedShape")
            {
                UIAddMenuPage = uiAddMenuPage.StanderedShape;
            }
        }

        IsAddMenuRoot = false;
    }

    public void AddMenuBackRootPage()
    {
        IsAddMenuRoot = true;
    }


    //削除

    public void ObjectDelete()
    {
        int index = GM.PD_NowS().o.ToList().IndexOf(GM.View_SelectObject);
        
        List<int> o_Delete = GM.PD_NowS().o.ToList();
        o_Delete.RemoveAt(index);
        GM.PD_NowS().o = o_Delete.ToArray();

        List<Objects> O_Delete = GM.PD_NowS().O.ToList();
        O_Delete.RemoveAt(index);
        GM.PD_NowS().O = O_Delete.ToArray();

        Destroy(UserObjects[index]);
        UserObjects.RemoveAt(index);

        GM.SaveJson();

        IsViewCursor = false;
        GM.View_SelectObject = -1;
    }


    //プログラミングウィンドウ（表示・非表示）に関するやつ

    ///基底画面

    public void SwitchProgrammingWindow()
    {
        IsProgrammingWindow = !IsProgrammingWindow;

        PW_IsAddMessageWindow = false;

        PW_IsMessageDetail = false;
    }

    public void PW_SwitchToggle()
    {
        GM.PW_IsSend = !GM.PW_IsSend;
    }

    ///AddMessage

    public void PW_SwitchAddMessageWindow()
    {
        PW_IsAddMessageWindow = !PW_IsAddMessageWindow;

        if (PW_IsAddMessageWindow)
        {
            PW_AM_InputField.text = "";
            PW_AM_ErrorMessage.text = "";

            PW_AM_SelectID = -1;
        }
    }

    public void PW_AM_Toggle()
    {
        PW_AM_IsSelect = !PW_AM_IsSelect;
    }

    public void PW_AM_SelectMessage(int id)
    {
        PW_AM_SelectID = id;
        PW_AM_SelectMessageAnimation = 0;

        PW_AM_SelectCursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(PW_AM_SelectCursor.GetComponent<RectTransform>().anchoredPosition.x, -130 - 280 * id);
    }

    ///MessageDetail

    public void PW_SwitchMessageDetail(int id)
    {
        PW_IsMessageDetail = !PW_IsMessageDetail;

        if (PW_IsMessageDetail)
        {
            if (GM.PW_IsSend)
            {
                for (int i = 0; i < GM.PD_NowO().G.S.Length; i++)
                {
                    if (GM.PD_NowO().G.S[i].i == id)
                    {
                        GM.Nowexecutemessage = i;
                        break;
                    }
                }

                PW_MD_MessageName.text = GM.PD_NowS().M[GM.PD_NowM().i].N + " (送信)";
            }
            else
            {
                for (int i = 0; i < GM.PD_NowO().G.R.Length; i++)
                {
                    if (GM.PD_NowO().G.R[i].i == id)
                    {
                        GM.Nowexecutemessage = i;
                        break;
                    }
                }

                PW_MD_MessageName.text = GM.PD_NowS().M[GM.PD_NowM().i].N + " (受信)";
            }

            StartCoroutine(ReflectJsonToProgrammingAll());
        }
    }

    public void PW_MD_SwitchTextEdit(int id)
    {
        PW_MD_IsTextEdit = !PW_MD_IsTextEdit;

        if (PW_MD_IsTextEdit)
        {
            GM.Nowprogrammingicon = id;

            PW_MD_TE_Dropdown.value = GM.PD_NowP().O.I[0];
            PW_MD_TE_Value.text = GM.PD_NowP().O.S[0];
        }
        else
        {
            if (id == -2)
            {
                GM.PD_NowP().O.I[0] = PW_MD_TE_Dropdown.value;
                GM.PD_NowP().O.S[0] = PW_MD_TE_Value.text;

                GM.SaveJson();
            }
        }
    }

    public void PW_MD_SwitchCSEdit(int id)
    {
        int result = 0;

        if (!PW_MD_IsCSEdit || int.TryParse(PW_MD_CS_Value.text, out result))
        {
            PW_MD_IsCSEdit = !PW_MD_IsCSEdit;

            if (PW_MD_IsCSEdit)
            {
                GM.Nowprogrammingicon = id;

                PW_MD_CS_Value.text = (GM.PD_NowP().O.I[0] + 1).ToString();
            }
            else
            {
                if (id == -2)
                {
                    GM.PD_NowP().O.I[0] = result - 1;

                    GM.SaveJson();
                }
            }
        }
    }


    //色ウィンドウ

    public void SwitchColorWindow(bool IsSave)
    {
        IsColorWindow = !IsColorWindow;

        if (IsColorWindow)
        {
            int max = Mathf.Max(GM.PD_NowO().C.R, GM.PD_NowO().C.G, GM.PD_NowO().C.B);
            int min = Mathf.Min(GM.PD_NowO().C.R, GM.PD_NowO().C.G, GM.PD_NowO().C.B);

            if (max == 0)
            {
                CW_S = 0;
            }
            else
            {
                CW_S = (int)((max - min) * 255f / max);
            }

            CW_V = max;

            if (max == min)
            {
                CW_H = 0;
            }
            else
            {
                if (max == GM.PD_NowO().C.R)
                {
                    CW_H = (int)(60f * (GM.PD_NowO().C.G - GM.PD_NowO().C.B) / (max - min));
                }
                else if (max == GM.PD_NowO().C.G)
                {
                    CW_H = (int)(60f * (GM.PD_NowO().C.B - GM.PD_NowO().C.R) / (max - min)) + 120;
                }
                else
                {
                    CW_H = (int)(60f * (GM.PD_NowO().C.R - GM.PD_NowO().C.G) / (max - min)) + 240;
                }

                if (CW_H < 0) CW_H += 360;
            }

            CW_SVCursorObject.anchoredPosition = new Vector2((CW_S / 255f - 0.5f) * 640, (CW_V / 255f - 0.5f) * 640);

            CW_HCursorObject.anchoredPosition = 590 * new Vector2(Mathf.Cos(CW_H * Mathf.Deg2Rad), Mathf.Sin(CW_H * Mathf.Deg2Rad));
        }
        else
        {
            if (IsSave)
            {
                GM.PD_NowO().C.R = CW_R;
                GM.PD_NowO().C.G = CW_G;
                GM.PD_NowO().C.B = CW_B;

                GM.SaveJson();

                GM.ReflectJsonToObject(NowUserObject(), GM.PD_NowO(), false);
            }
        }
    }

    public void CW_SVCursorPush(bool IsDown)
    {
        CW_IsSVMove = IsDown;

        if (CW_IsSVMove)
        {
            Set1stFinger = GM.pos();

            CW_SetSVCursorPos = CW_SVCursorObject.anchoredPosition;
        }
    }

    public void CW_HCursorPush(bool IsDown)
    {
        CW_IsHMove = IsDown;

        if (CW_IsHMove)
        {
            Set1stFinger = GM.pos();

            CW_SetHCursorPos = CW_HCursorObject.anchoredPosition;
        }
    }


    //オブジェクト新規追加

    public void UIAdd(string AddType)
    {
        if (AddType == "Device") //デバイスからのアップロードは特別！
        {
            imagePicker.Show("Select Image", "unimgpicker", 1024);
        }
        else
        {
            //Image保管データに追加する時だけ追加　※Device2はIEnumeratorで追加済み

            if (GM.PD.i == null) GM.PD.i = new int[]{ };
            
            int istart = (GM.PD.i.Length == 0 ? 0 : GM.PD.i.Max() + 1);
            var iadd = new List<int>();
            var AddImages = new List<Images>();

            switch (AddType)
            {
                case "Character":

                    for (int i = 0; i < 1; i++)
                    {
                        iadd.Add(istart + i);

                        AddImages.Add(new Images{ W = 300, H = 600 });

                        GM.SaveImage(GM.ImageByte.Character[i], GM.Path(3, "Images", iadd.Last().ToString()));
                    }

                break;

                case "Rectangle":

                    iadd.Add(istart);
                    AddImages.Add(new Images{ W = 1200, H = 1200 });
                    GM.SaveImage(GM.ImageByte.Rectangle, GM.Path(3, "Images", iadd.Last().ToString()));

                break;

                case "Circle":

                    iadd.Add(istart);
                    AddImages.Add(new Images{ W = 1200, H = 1200 });
                    GM.SaveImage(GM.ImageByte.Circle, GM.Path(3, "Images", iadd.Last().ToString()));

                break;

                case "Triangle":

                    iadd.Add(istart);
                    AddImages.Add(new Images{ W = 1200, H = 1200 });
                    GM.SaveImage(GM.ImageByte.Triangle, GM.Path(3, "Images", iadd.Last().ToString()));

                break;
            }

            GM.PD.i = GM.PD.i.Concat(iadd).ToArray();

            if (GM.PD.I == null) GM.PD.I = new Images[]{ };

            GM.PD.I = GM.PD.I.Concat(AddImages).ToArray();


            //Jsonにおけるオブジェクトの追加へ

            if (GM.PD_NowS().o == null) GM.PD_NowS().o = new int[]{ };
        
            GM.PD_NowS().o = GM.PD_NowS().o.Concat(new List<int>{ (GM.PD_NowS().o.Length == 0 ? 0 : GM.PD_NowS().o.ToList().Max() + 1)} ).ToArray();

            if (GM.PD_NowS().O == null) GM.PD_NowS().O = new Objects[]{ };


            Objects AddObjectJson = new Objects
            {
                P = (new Vector2(0, 19) - ScreenPos) / ScreenScale,
                A = 0,
                Ni = 0,
            };

            switch (AddType)
            {
                case "Character":

                    AddObjectJson = new Objects
                    {
                        N = "キャラクター",
                        T = "Image",
                        P = AddObjectJson.P,
                        A = AddObjectJson.A,
                        S = new Vector2(120, 240),
                        C = new ColorInt{ R = 255, G = 255, B = 255, A = 255},
                        i = iadd.ToArray(),
                        Ni = AddObjectJson.Ni,
                        O = new Option()
                    };

                break;

                case "Rectangle":

                    AddObjectJson = new Objects
                    {
                        N = "長方形",
                        T = "Image",
                        P = AddObjectJson.P,
                        A = AddObjectJson.A,
                        S = new Vector2(180, 180),
                        C = new ColorInt{ R = 255, G = 159, B = 63, A = 255 },
                        i = iadd.ToArray(),
                        Ni = AddObjectJson.Ni,
                        O = new Option()
                    };

                break;

                case "Circle":

                    AddObjectJson = new Objects
                    {
                        N = "円",
                        T = "Image",
                        P = AddObjectJson.P,
                        A = AddObjectJson.A,
                        S = new Vector2(180, 180),
                        C = new ColorInt{ R = 63, G = 207, B = 63, A = 255 },
                        i = iadd.ToArray(),
                        Ni = AddObjectJson.Ni,
                        O = new Option()
                    };

                break;

                case "Triangle":

                    AddObjectJson = new Objects
                    {
                        N = "三角形",
                        T = "Image",
                        P = AddObjectJson.P,
                        A = AddObjectJson.A,
                        S = new Vector2(180, 180),
                        C = new ColorInt{ R = 63, G = 255, B = 255, A = 255 },
                        i = iadd.ToArray(),
                        Ni = AddObjectJson.Ni,
                        O = new Option()
                    };

                break;

                case "Text":

                    AddObjectJson = new Objects
                    {
                        N = "テキスト",
                        T = "Text",
                        P = AddObjectJson.P,
                        A = AddObjectJson.A,
                        S = new Vector2(240, 120),
                        C = new ColorInt{ R = 0, G = 0, B = 0, A = 255 },
                        i = new int[]{},
                        Ni = AddObjectJson.Ni,
                        O = new Option{ S = new String[]{ "Text" } }
                    };

                break;

                case "Device2":

                    AddObjectJson = new Objects
                    {
                        N = "追加した画像",
                        T = "Image",
                        P = AddObjectJson.P,
                        A = AddObjectJson.A,
                        S = new Vector2(GM.PD.I.Last().W, GM.PD.I.Last().H),
                        C = new ColorInt{ R = 255, G = 255, B = 255, A = 255 },
                        i = new int[]{ GM.PD.i.Last() },
                        Ni = AddObjectJson.Ni,
                        O = new Option()
                    };

                break;
            }

            GM.PD_NowS().O = GM.PD_NowS().O.Concat(new List<Objects>{ AddObjectJson }).ToArray();

            GM.SaveJson();


            //画面に追加していく作業

            GameObject NewObject = Instantiate(GM.EditObjectsPrefab, EditObjectsParent.transform);
            
            NewObject.GetComponent<ObjectPlacement>().id = GM.PD_NowS().o.Last();

            UserObjects.Add(NewObject);

            GM.ReflectJsonToObject(NewObject, GM.PD_NowS().O.Last());


            //追加メニューを閉じる

            IsAddMenu = false;


            //カーソルを合わせる

            GM.View_SelectObject = GM.PD_NowS().o.Last();
            IsViewCursor = true;
            ViewCursorAnimation = 0;


            //プログラム画面の用意
            StartCoroutine(ReflectJsonToMessageAll());
        }
    }


    private IEnumerator UIAddMenuDeviceResponse(string path)
    {
        var url = "file://" + path;
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
        }
        else
        {
            var texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            if (GM.PD.i == null) GM.PD.i = new int[]{ };
            
            int istart = (GM.PD.i.Length == 0 ? 0 : GM.PD.i.Max() + 1);
            var iadd = new List<int>();
            var AddImages = new List<Images>();

            iadd.Add(istart);
            AddImages.Add(new Images{ W = texture.width, H = texture.height });
            GM.SaveImage(texture, GM.Path(3, "Images", iadd.Last().ToString()));

            GM.PD.i = GM.PD.i.Concat(iadd).ToArray();

            if (GM.PD.I == null) GM.PD.I = new Images[]{ };

            GM.PD.I = GM.PD.I.Concat(AddImages).ToArray();

            UIAdd("Device2");
        }
    }


    //Json上のメッセージをリアルのメッセージに反映する

    //一括
    public IEnumerator ReflectJsonToMessageAll()
    {
        //タイトル、イメージアイコンの変更
        switch (GM.PD_NowO().T)
        {
            case "Image":

                PW_ObjectImage.GetComponent<PictureBackground>().enabled = true;
                PW_ObjectImage.GetComponent<Image>().sprite = NowUserObject().GetComponent<Image>().sprite;
                PW_ObjectImage.GetComponent<Image>().color = GM.ColorIntToColor(GM.PD_NowO().C);

            break;

            case "Text":

                PW_ObjectImage.GetComponent<PictureBackground>().enabled = false;
                PW_ObjectImage.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 140);

                Texture2D tex = Resources.Load<Texture2D>("Icons8/テキスト");
                tex.wrapMode = TextureWrapMode.Clamp;

                PW_ObjectImage.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                PW_ObjectImage.GetComponent<Image>().color = new Color32(15, 15, 15, 255);

            break;
        }

        PW_ObjectName.text = GM.PD_NowO().N;


        if (GM.PD_NowO().G == null) GM.PD_NowO().G = new MessageGroup();
        
        GameObject NewObject;
        int index;


        if (GM.PD_NowO().G.r == null) GM.PD_NowO().G.r = new int[]{};
        if (GM.PD_NowO().G.s == null) GM.PD_NowO().G.s = new int[]{};

        for (int i = 0; i < 2; i++)
        {
            GameObject Content = (i == 0 ? PW_ReceiveContent : PW_SendContent);
            int Length = (i == 0 ? GM.PD_NowO().G.r.Length : GM.PD_NowO().G.s.Length);

            foreach (Transform t in Content.transform)
            {
                Destroy(t.gameObject);
            }

            Content.GetComponent<RectTransform>().sizeDelta = new Vector2(Content.GetComponent<RectTransform>().sizeDelta.x, 10 + (Length / 2 + 1) * 640);

            for (index = 0; index < Length; index++)
            {
                NewObject = Instantiate((i == 0 ? PW_ReceivePrefab : PW_SendPrefab), Content.transform);

                var RT = NewObject.GetComponent<RectTransform>();

                RT.anchorMin = new Vector2((index % 2 == 0 ? 0 : 0.5f), 1);
                RT.anchorMax = new Vector2((index % 2 == 0 ? 0.5f : 1), 1);

                RT.offsetMin = new Vector2((index % 2 == 0 ? 0 : 20), RT.offsetMin.y);
                RT.offsetMax = new Vector2((index % 2 == 0 ? -20 : 0), RT.offsetMax.y);

                RT.anchoredPosition = new Vector2(RT.anchoredPosition.x, (-(index / 2) * 640 - 310));


                NewObject.GetComponent<MessagePlacement>().id = (i == 0 ? GM.PD_NowO().G.R[index].i : GM.PD_NowO().G.S[index].i);
                NewObject.GetComponent<MessagePlacement>().Text.text = GM.PD_NowS().M[NewObject.GetComponent<MessagePlacement>().id].N;

                NewObject.GetComponent<MessagePlacement>().ScrollRect = (i == 0 ? PW_ReceiveScrollRect : PW_SendScrollRect);


                if (index % 5 == 0) yield return null;
            }

            NewObject = Instantiate(PW_AddButtonPrefab, Content.transform);

            NewObject.GetComponent<RectTransform>().anchorMin = new Vector2((index % 2 == 0 ? 0.25f : 0.75f), 1);
            NewObject.GetComponent<RectTransform>().anchorMax = new Vector2((index % 2 == 0 ? 0.25f : 0.75f), 1);

            NewObject.GetComponent<RectTransform>().anchoredPosition = new Vector2((index % 2 == 0 ? -5 : 5), (-(index / 2) * 640 - 310));

            NewObject.GetComponent<Button>().onClick.AddListener(() => PW_SwitchAddMessageWindow());
        }
    }


    //メッセージ追加の選択肢をリアルに反映する

    public IEnumerator ReflectJsonToAddMessageSelect()
    {
        if (GM.PD_NowS().m == null) GM.PD_NowS().m = new int[]{};
        
        PW_AM_SelectContent.sizeDelta = new Vector2(PW_AM_SelectContent.sizeDelta.x, 280 * GM.PD_NowS().m.Length - 20);

        if (PW_AM_SelectObjectList.Count == 0 && GM.PD_NowS().m.Length == 0)
        {
            PW_AM_NoMessage.SetActive(true);
        }
        else
        {
            PW_AM_NoMessage.SetActive(false);


            int ObjectCount = 0;

            var ids = GM.PD_NowS().m.ToList();


            while (ObjectCount < PW_AM_SelectObjectList.Count)
            {
                if (GM.PD_NowS().m.Contains(PW_AM_SelectObjectList[ObjectCount].GetComponent<PW_AM_SelectCandidacy>().id))
                {
                    int index = GM.PD_NowS().m.ToList().IndexOf(PW_AM_SelectObjectList[ObjectCount].GetComponent<PW_AM_SelectCandidacy>().id);

                    PW_AM_SelectObjectList[ObjectCount].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, index * -280 - 130);

                    PW_AM_SelectObjectList[ObjectCount].GetComponent<PW_AM_SelectCandidacy>().text.text = GM.PD_NowS().M[index].N;

                    ids.Remove(index);

                    ObjectCount++;
                }
                else
                {
                    Destroy(PW_AM_SelectObjectList[ObjectCount]);
                    PW_AM_SelectObjectList.RemoveAt(ObjectCount);
                }

                if (ObjectCount % 5 == 0) yield return null;
            }

            foreach (var id in ids)
            {
                GameObject NewObject = Instantiate(PW_AM_SelectPrefab, PW_AM_SelectParent.transform);

                PW_AM_SelectObjectList.Add(NewObject);

                NewObject.name = GM.PD_NowS().M[id].N;

                NewObject.GetComponent<PW_AM_SelectCandidacy>().id = id;

                NewObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, id * -280 - 130);

                NewObject.GetComponent<PW_AM_SelectCandidacy>().text.text = GM.PD_NowS().M[id].N;

                NewObject.GetComponent<PW_AM_SelectCandidacy>().ScrollRect = PW_AM_ScrollRect;
            }
        }
    }
    

    //メッセージ新規追加

    public void MessageAdd()
    {
        if (PW_AM_IsSelect)
        {
            if (GM.PW_IsSend)
            {
                if (GM.PD_NowO().G.s == null) GM.PD_NowO().G.s = new int[]{};
                if (GM.PD_NowO().G.S == null) GM.PD_NowO().G.S = new ExecuteMessage[]{};
                
                GM.PD_NowO().G.s = GM.PD_NowO().G.s.Concat(new List<int>{ GM.PD_NowO().G.s.Length == 0 ? 0 : GM.PD_NowO().G.s.Max() + 1 }).ToArray();
                GM.PD_NowO().G.S = GM.PD_NowO().G.S.Concat(new List<ExecuteMessage>{ new ExecuteMessage(){ i = PW_AM_SelectID }}).ToArray();
            }
            else
            {
                if (GM.PD_NowO().G.r == null) GM.PD_NowO().G.r = new int[]{};
                if (GM.PD_NowO().G.R == null) GM.PD_NowO().G.R = new ExecuteMessage[]{};

                GM.PD_NowO().G.r = GM.PD_NowO().G.r.Concat(new List<int>{ GM.PD_NowO().G.r.Length == 0 ? 0 : GM.PD_NowO().G.r.Max() + 1 }).ToArray();
                GM.PD_NowO().G.R = GM.PD_NowO().G.R.Concat(new List<ExecuteMessage>{ new ExecuteMessage(){ i = PW_AM_SelectID }}).ToArray();
            }

            GM.SaveJson();

            StartCoroutine(ReflectJsonToMessageAll());

            StartCoroutine(ReflectJsonToAddMessageSelect());

            PW_SwitchAddMessageWindow();
        }
        else
        {
            PW_AM_InputField.text = PW_AM_InputField.text.TrimEnd();

            if (GM.PD_NowS().m == null) GM.PD_NowS().m = new int[]{};
            if (GM.PD_NowS().M == null) GM.PD_NowS().M = new Message[]{};

            bool Check = false;
            foreach (var mg in GM.PD_NowS().M)
            {
                if (mg.N == PW_AM_InputField.text)
                {
                    Check = true;
                    break;
                }
            }

            if (Check)
            {
                PW_AM_ErrorMessage.text = "同じ文字のメッセージが既に追加されてます！";
            }
            else
            {
                PW_AM_ErrorMessage.text = "";


                GM.PD_NowS().m = GM.PD_NowS().m.Concat(new List<int>{ GM.PD_NowS().m.Length == 0 ? 0 : GM.PD_NowS().m.Max() + 1 }).ToArray();

                GM.PD_NowS().M = GM.PD_NowS().M.Concat(new List<Message>{ new Message(){ N = PW_AM_InputField.text } }).ToArray();

                if (GM.PW_IsSend)
                {
                    if (GM.PD_NowO().G.s == null) GM.PD_NowO().G.s = new int[]{};
                    if (GM.PD_NowO().G.S == null) GM.PD_NowO().G.S = new ExecuteMessage[]{};

                    GM.PD_NowO().G.s = GM.PD_NowO().G.s.Concat(new List<int>{ GM.PD_NowO().G.s.Length == 0 ? 0 : GM.PD_NowO().G.s.Max() + 1 }).ToArray();
                    GM.PD_NowO().G.S = GM.PD_NowO().G.S.Concat(new List<ExecuteMessage>{ new ExecuteMessage(){ i = GM.PD_NowS().m.Last() }}).ToArray();
                }
                else
                {
                    if (GM.PD_NowO().G.r == null) GM.PD_NowO().G.r = new int[]{};
                    if (GM.PD_NowO().G.R == null) GM.PD_NowO().G.R = new ExecuteMessage[]{};

                    GM.PD_NowO().G.r = GM.PD_NowO().G.r.Concat(new List<int>{ GM.PD_NowO().G.r.Length == 0 ? 0 : GM.PD_NowO().G.r.Max() + 1 }).ToArray();
                    GM.PD_NowO().G.R = GM.PD_NowO().G.R.Concat(new List<ExecuteMessage>{ new ExecuteMessage(){ i = GM.PD_NowS().m.Last() }}).ToArray();
                }

                GM.SaveJson();

                StartCoroutine(ReflectJsonToMessageAll());

                StartCoroutine(ReflectJsonToAddMessageSelect());

                PW_SwitchAddMessageWindow();
            }
        }
    }


    //Json上のプログラミングをリアルのプログラミングに反映する

    public IEnumerator ReflectJsonToProgrammingAll()
    {
        var Length = (GM.PW_IsSend ? GM.PD_NowO().G.s.Length : GM.PD_NowO().G.r.Length);


        if (GM.PD_NowM().p == null) GM.PD_NowM().p = new int[]{};

        PW_MD_Content.sizeDelta = new Vector2(PW_MD_Content.sizeDelta.x, 40 + 340 * Length);

        if (GM.PD_NowM().P == null) GM.PD_NowM().P = new ProgrammingIcon[]{};


        if (GM.PD_NowM().p.Length == 0)
        {
            PW_MD_NoMessage.SetActive(true);
        }
        else
        {
            PW_MD_NoMessage.SetActive(false);
        }

        int ObjectCount = 0;

        var ids = GM.PD_NowM().p.ToList();

        while (ObjectCount < PW_MD_ProgrammingObjectList.Count)
        {
            if (GM.PD_NowM().p.Contains(PW_MD_ProgrammingObjectList[ObjectCount].GetComponent<ProgrammingPlacement>().id))
            {
                int index = GM.PD_NowM().p.ToList().IndexOf(PW_MD_ProgrammingObjectList[ObjectCount].GetComponent<ProgrammingPlacement>().id);

                PW_MD_ProgrammingObjectList[ObjectCount].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, index * -340);

                PW_MD_ProgrammingObjectList[ObjectCount].GetComponent<ProgrammingPlacement>().text.text = GM.ProgrammingType_Lang[GM.PD_NowM().P[index].T];

                PW_MD_ProgrammingObjectList[ObjectCount].GetComponent<ProgrammingPlacement>().Icon.sprite = GM.ProgrammingIcon[GM.PD_NowM().P[index].T];

                ids.Remove(index);

                ObjectCount++;
            }
            else
            {
                Destroy(PW_MD_ProgrammingObjectList[ObjectCount]);
                PW_MD_ProgrammingObjectList.RemoveAt(ObjectCount);
            }

            if (ObjectCount % 5 == 0) yield return null;
        }

        foreach (var id in ids)
        {
            GameObject NewObject = Instantiate(PW_MD_ProgrammingPrefab, PW_MD_Content.transform);

            PW_MD_ProgrammingObjectList.Add(NewObject);

            NewObject.name = GM.PD_NowM().P[id].T;

            NewObject.GetComponent<ProgrammingPlacement>().id = id;

            NewObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, id * -340);

            NewObject.GetComponent<ProgrammingPlacement>().text.text = GM.ProgrammingType_Lang[GM.PD_NowM().P[id].T];

            NewObject.GetComponent<ProgrammingPlacement>().Icon.sprite = GM.ProgrammingIcon[GM.PD_NowM().P[id].T];

            NewObject.GetComponent<ProgrammingPlacement>().ScrollRect = PW_MD_ScrollRect;

            NewObject.GetComponent<ProgrammingPlacement>().interactable = (GM.PD_NowM().P[id].T == "Text") || (GM.PD_NowM().P[id].T == "ChangeScene");
        }
    }


    //プログラミング追加

    public void ProgrammingAdd(string AddType)
    {
        if (GM.PD_NowM().p == null) GM.PD_NowM().p = new int[]{};

        GM.PD_NowM().p = GM.PD_NowM().p.Concat(new List<int>{ GM.PD_NowM().p.Length == 0 ? 0 : GM.PD_NowM().p.Max() + 1 }).ToArray();

        if (GM.PD_NowM().P == null) GM.PD_NowM().P = new ProgrammingIcon[]{};

        var AddProgrammingIcon = new ProgrammingIcon();

        switch (AddType)
        {
            case "Move_Right":
            case "Move_Left":
            case "Move_Up":
            case "Move_Down":

                AddProgrammingIcon = new ProgrammingIcon
                {
                    T = AddType,
                    O = new Option
                    {
                        F = new float[]{ 10 }
                    }
                };

            break;

            case "Move_Rotate":

                AddProgrammingIcon = new ProgrammingIcon
                {
                    T = AddType,
                    O = new Option
                    {
                        F = new float[]{ 45 }
                    }
                };

            break;

            case "Move_Position":

                AddProgrammingIcon = new ProgrammingIcon
                {
                    T = AddType,
                    O = new Option
                    {
                        F = new float[]{ 0, 0 }
                    }
                };

            break;

            case "Move_Angle":

                AddProgrammingIcon = new ProgrammingIcon
                {
                    T = AddType,
                    O = new Option
                    {
                        F = new float[]{ 0 }
                    }
                };

            break;

            case "Notification":

                AddProgrammingIcon = new ProgrammingIcon
                {
                    T = AddType,
                    O = new Option
                    {
                        S = new string[]{ "Message" }
                    }
                };

            break;

            case "Tap":

                AddProgrammingIcon = new ProgrammingIcon
                {
                    T = AddType,
                    O = new Option
                    {
                        I = new int[]{ 1 }
                    }
                };

            break;

            case "Interval":

                AddProgrammingIcon = new ProgrammingIcon
                {
                    T = AddType,
                    O = new Option
                    {
                        I = new int[]{ 40 }
                    }
                };

            break;
            
            case "ChangeScene":

                AddProgrammingIcon = new ProgrammingIcon
                {
                    T = AddType,
                    O = new Option
                    {
                        I = new int[]{ 0 }
                    }
                };

            break;

            case "Text":

                AddProgrammingIcon = new ProgrammingIcon
                {
                    T = AddType,
                    O = new Option
                    {
                        I = new int[]{ 0 },
                        S = new string[]{ "Text"}
                    }
                };

            break;

            default:

                AddProgrammingIcon = new ProgrammingIcon
                {
                    T = AddType,
                    O = new Option
                    {

                    }
                };

            break;
        }

        GM.PD_NowM().P = GM.PD_NowM().P.Concat(new List<ProgrammingIcon>{ AddProgrammingIcon }).ToArray();

        GM.SaveJson();

        StartCoroutine(ReflectJsonToProgrammingAll());

        SwitchAddMenu();
    }
}