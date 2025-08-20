using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GM_Execute : MonoBehaviour
{
    GameManager GM;

    public static GM_Execute instance;


    //ポーズ

    public bool IsPause;

    [SerializeField] CanvasGroup PauseObject;

    float PauseAnimation;

    
    //編集画面へ

    AsyncOperation CreateAsync;

    [SerializeField] Image CreateFade;


    //再生

    public bool IsGameStart;

    [SerializeField] GameObject GameStartObject;


    //オブジェクト配置

    public List<GameObject> UserObjects;

    public class objectsTap
    {
        public bool IsDown;
        public bool Is;
        public bool IsUp;
    }

    public Dictionary<int, objectsTap> ObjectsTap = new Dictionary<int, objectsTap>();


    [SerializeField] Transform EditObjectsParent;

    public Dictionary<int, (bool IsExecute, bool IsContact, int ObjectID)> AllMessage = new Dictionary<int, (bool, bool, int)>();


    public int FrameCounter;


    public int SceneIndex;

    int temp_SceneIndex = -1;



    void Awake()
    {
        instance = this;
    }


    void Start()
    {
        GM = GameManager.instance;


        GM.Android_Bar_Show(false, false);


        CreateFade.gameObject.SetActive(true);


        SceneReset(0);
    }

    void Update()
    {
        //ポーズ

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SwitchPause();
        }


        PauseObject.blocksRaycasts = IsPause;

        GM.Animation(ref PauseAnimation, 6, IsPause);

        PauseObject.alpha = PauseAnimation;


        //編集画面へ

        CreateFade.raycastTarget = !GM.IsExecute;

        CreateFade.color = new Color32(0, 0, 0, (byte)(GM.ExecuteAnimation < 0.5f ? 255 : ((1 - GM.ExecuteAnimation) * 2) * 255));

        if (!GM.IsExecute && GM.ExecuteAnimation == 0)
        {
            CreateAsync.allowSceneActivation = true;
        }


        //再生

        GameStartObject.SetActive(!IsGameStart);


        //実行

        FrameCounter++;

        if (IsGameStart && !IsPause)
        {
            ///受信

            for (int i = 0; i < GM.PD.S[SceneIndex].O.Length; i++)
            {
                if (GM.PD.S[SceneIndex].O[i].G.r == null) GM.PD.S[SceneIndex].O[i].G.r = new int[]{};
                if (GM.PD.S[SceneIndex].O[i].G.R == null) GM.PD.S[SceneIndex].O[i].G.R = new ExecuteMessage[]{};

                foreach (var ReceiveMessage in GM.PD.S[SceneIndex].O[i].G.R)
                {
                    bool Overlaps = false;

                    if (AllMessage[ReceiveMessage.i].ObjectID != -1)
                    {
                        Rect ReceiveSize = new Rect
                        (
                            GM.PD.S[SceneIndex].O[i].P.x - GM.PD.S[SceneIndex].O[i].S.x / 2,
                            GM.PD.S[SceneIndex].O[i].P.y - GM.PD.S[SceneIndex].O[i].S.y / 2,
                            GM.PD.S[SceneIndex].O[i].S.x,
                            GM.PD.S[SceneIndex].O[i].S.y
                        );

                        int o = GM.PD.S[SceneIndex].o.ToList().IndexOf(AllMessage[ReceiveMessage.i].ObjectID);

                        Rect SendSize = new Rect
                        (
                            GM.PD.S[SceneIndex].O[o].P.x - GM.PD.S[SceneIndex].O[o].S.x / 2,
                            GM.PD.S[SceneIndex].O[o].P.y - GM.PD.S[SceneIndex].O[o].S.y / 2,
                            GM.PD.S[SceneIndex].O[o].S.x,
                            GM.PD.S[SceneIndex].O[o].S.y
                        );

                        Overlaps = ReceiveSize.Overlaps(SendSize);
                    }

                    if (AllMessage[ReceiveMessage.i].IsExecute && (!AllMessage[ReceiveMessage.i].IsContact || Overlaps))
                    {
                        if (ReceiveMessage.p == null) ReceiveMessage.p = new int[]{};
                        if (ReceiveMessage.P == null) ReceiveMessage.P = new ProgrammingIcon[]{};

                        for (int j = 0; j < ReceiveMessage.p.Length; j++)
                        {
                            switch (ReceiveMessage.P[j].T)
                            {
                                case "Move_Right":

                                    GM.PD.S[SceneIndex].O[i].P.x += ReceiveMessage.P[j].O.F[0];

                                break;

                                case "Move_Left":

                                    GM.PD.S[SceneIndex].O[i].P.x -= ReceiveMessage.P[j].O.F[0];

                                break;

                                case "Move_Up":

                                    GM.PD.S[SceneIndex].O[i].P.y += ReceiveMessage.P[j].O.F[0];

                                break;

                                case "Move_Down":

                                    GM.PD.S[SceneIndex].O[i].P.y -= ReceiveMessage.P[j].O.F[0];

                                break;

                                case "Move_Rotate":

                                    GM.PD.S[SceneIndex].O[i].A += ReceiveMessage.P[j].O.F[0];

                                break;

                                case "Move_Random":

                                    GM.PD.S[SceneIndex].O[i].P = new Vector2
                                    (
                                        UnityEngine.Random.Range(-360 + GM.PD.S[SceneIndex].O[i].S.x / 2, 360 - GM.PD.S[SceneIndex].O[i].S.x / 2),
                                        UnityEngine.Random.Range(-640 + GM.PD.S[SceneIndex].O[i].S.y / 2, 640 - GM.PD.S[SceneIndex].O[i].S.y / 2)
                                    );

                                break;

                                case "Move_Position":

                                    GM.PD.S[SceneIndex].O[i].P = new Vector2(ReceiveMessage.P[j].O.F[0], ReceiveMessage.P[j].O.F[1]);

                                break;

                                case "Move_Angle":

                                    GM.PD.S[SceneIndex].O[i].A = ReceiveMessage.P[j].O.F[0];

                                break;

                                case "Pause":

                                    if (!IsPause) SwitchPause();

                                break;

                                case "Text":

                                    switch (ReceiveMessage.P[j].O.I[0])
                                    {
                                        case 0:

                                            GM.PD.S[SceneIndex].O[i].O.S[0] = ReceiveMessage.P[j].O.S[0];

                                        break;

                                        case 1:

                                            float a = 0;
                                            float b = 0;

                                            if (float.TryParse(GM.PD.S[SceneIndex].O[i].O.S[0], out a) && float.TryParse(ReceiveMessage.P[j].O.S[0], out b))
                                            {
                                                GM.PD.S[SceneIndex].O[i].O.S[0] = (a + b).ToString();
                                            }

                                        break;

                                        case 2:

                                            float c = 0;
                                            float d = 0;

                                            if (float.TryParse(GM.PD.S[SceneIndex].O[i].O.S[0], out c) && float.TryParse(ReceiveMessage.P[j].O.S[0], out d))
                                            {
                                                GM.PD.S[SceneIndex].O[i].O.S[0] = (c - d).ToString();
                                            }

                                        break;
                                    }

                                break;

                                case "ChangeScene":

                                    temp_SceneIndex = Mathf.Clamp(ReceiveMessage.P[j].O.I[0], 0, GM.PD.S.Length);

                                break;
                            }
                        }

                        GM.ReflectJsonToObject(UserObjects[i], GM.PD.S[SceneIndex].O[i], false);

                        if (temp_SceneIndex != -1)
                        {
                            break;
                        }
                    }
                }

                if (temp_SceneIndex != -1)
                {
                    break;
                }
            }


            if (temp_SceneIndex == -1)
            {
                ///Tapの解除

                foreach (var ObjectsTap in ObjectsTap.Values)
                {
                    if (ObjectsTap.IsDown)
                    {
                        ObjectsTap.IsDown = false;
                    }

                    if (ObjectsTap.IsUp)
                    {
                        ObjectsTap.Is = false;
                        ObjectsTap.IsUp = false;
                    }
                }


                ///送信

                AllMessage.Clear();

                foreach (var Message in GM.PD.S[SceneIndex].m)
                {
                    AllMessage.Add(Message, (false, false, -1));
                }


                for (int i = 0; i < GM.PD.S[SceneIndex].O.Length; i++)
                {
                    if (GM.PD.S[SceneIndex].O[i].G.s == null) GM.PD.S[SceneIndex].O[i].G.s = new int[]{};
                    if (GM.PD.S[SceneIndex].O[i].G.S == null) GM.PD.S[SceneIndex].O[i].G.S = new ExecuteMessage[]{};

                    foreach (var SendMessage in GM.PD.S[SceneIndex].O[i].G.S)
                    {
                        if (SendMessage.p == null) SendMessage.p = new int[]{};
                        if (SendMessage.P == null) SendMessage.P = new ProgrammingIcon[]{};

                        bool GroupJudge = (SendMessage.p.Length >= 1);

                        bool IsContact = false;

                        for (int j = 0; j < SendMessage.p.Length; j++)
                        {
                            switch (SendMessage.P[j].T)
                            {
                                case "Tap":

                                    switch (SendMessage.P[j].O.I[0])
                                    {
                                        case 0:

                                            if (GroupJudge) GroupJudge = ObjectsTap[GM.PD.S[SceneIndex].o[i]].IsDown;

                                        break;

                                        case 1:

                                            if (GroupJudge) GroupJudge = ObjectsTap[GM.PD.S[SceneIndex].o[i]].Is;

                                        break;

                                        case 2:

                                            if (GroupJudge) GroupJudge = ObjectsTap[GM.PD.S[SceneIndex].o[i]].IsUp;

                                        break;
                                    }

                                break;

                                case "Forever":

                                    if (GroupJudge) GroupJudge = true;

                                break;

                                case "Interval":

                                    if (GroupJudge) GroupJudge = (FrameCounter % SendMessage.P[j].O.I[0] == 0);

                                break;

                                case "Contact":

                                    IsContact = true;

                                break;
                            }
                        }

                        if (!AllMessage[SendMessage.i].IsExecute) AllMessage[SendMessage.i] = (GroupJudge, IsContact, GM.PD.S[SceneIndex].o[i]);//
                    }
                }
            }
            else
            {
                SceneReset(temp_SceneIndex);

                temp_SceneIndex = -1;
            }
        }
    }


    public void ObjectStartArrange(Transform EditObjectsParent, List<GameObject> UserObjects)
    {
        foreach (var uo in UserObjects)
        {
            Destroy(uo);
        }

        UserObjects.Clear();


        if (GM.PD.S[SceneIndex].o == null) GM.PD.S[SceneIndex].o = new int[]{};

        if (GM.PD.S[SceneIndex].O == null) GM.PD.S[SceneIndex].O = new Objects[]{};

        for (int i = 0; i < GM.PD.S[SceneIndex].o.Length; i++)
        {
            GameObject NewObject = Instantiate(GM.EditObjectsPrefab, EditObjectsParent);
            
            NewObject.GetComponent<ObjectPlacement>().id = GM.PD.S[SceneIndex].o[i];

            UserObjects.Add(NewObject);

            GM.ReflectJsonToObject(NewObject, GM.PD.S[SceneIndex].O[i]);
        }
    }


    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && !IsPause && IsGameStart)
        {
            SwitchPause();
        }   
    }


    public void SwitchPause()
    {
        IsPause = !IsPause;
    }


    public void GotoCreate()
    {
        if (GM.Path_IsDownload)
        {
            CreateAsync = SceneManager.LoadSceneAsync("Play");
            CreateAsync.allowSceneActivation = false;

            GM.Path_IsDownload = false;
        }
        else
        {
            CreateAsync = SceneManager.LoadSceneAsync("Create");
            CreateAsync.allowSceneActivation = false;
        }

        GM.IsExecute = false;
    }


    public void GameStart()
    {
        IsGameStart = true;
    }


    void SceneReset(int LocalSceneIndex)
    {
        SceneIndex = LocalSceneIndex;


        ObjectStartArrange(EditObjectsParent, UserObjects);


        ObjectsTap.Clear();

        foreach (var i in GM.PD.S[SceneIndex].o)
        {
            ObjectsTap.Add(i, new objectsTap());
        }


        if (GM.PD.S[SceneIndex].m == null) GM.PD.S[SceneIndex].m = new int[]{};

        AllMessage.Clear();

        foreach (var Message in GM.PD.S[SceneIndex].m)
        {
            AllMessage.Add(Message, (false, false, -1));
        }

        FrameCounter = 0;
    }
}
