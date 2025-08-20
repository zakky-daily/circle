using NatSuite.Sharing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class GM_Home : MonoBehaviour
{
    GameManager GM;

    public static GM_Home instance;


    //プロジェクト一覧画面

    bool IsProjectList;

    [SerializeField] GameObject ProjectListObject;
    [SerializeField] GameObject TitleObject;
    
    float ProjectListAnimation;

    [SerializeField] GameObject PL_Prefab;
    [SerializeField] GameObject PL_Content;
    [SerializeField] DetectExpandScroll PL_Scroll;
    [SerializeField] GameObject PL_NoProject;

    List<GameObject> PL_ProjectObjectList = new List<GameObject>();


    //新規プロジェクト画面

    bool IsNewProject;

    [SerializeField] GameObject NewProjectObject;

    float NewProjectAnimation;


    //選択画面

    bool IsSelect;

    [SerializeField] GameObject SelectObject;

    float SelectAnimation;


    enum select_Page{ Root, ChangeName, Share }

    select_Page Select_Page;

    public int SelectID;

    [SerializeField] CanvasGroup Select_RootObject;
    [SerializeField] CanvasGroup Select_ChangeNameObject;
    [SerializeField] CanvasGroup Select_ShareObject;

    [SerializeField] TMP_InputField S_CN_InputField;
    [SerializeField] GameObject S_CN_NoTitle;

    [SerializeField] ToggleSwitch S_S_CheckBox;
    [SerializeField] CanvasGroup S_S_ShareInformation;
    [SerializeField] TextMeshProUGUI S_S_ErrorMessage;
    [SerializeField] TMP_InputField S_S_ShareCode;

    float Select_PageAnimation;


    //ローディング

    bool IsCreateLoading;

    float CreateLoadingAnimation;

    [SerializeField] GameObject LoadingObject;
    [SerializeField] Image LightFilter;

    [SerializeField] TMP_InputField NP_InputField;
    [SerializeField] GameObject NP_NoTitle;

    AsyncOperation CreateAsync;


    //Play画面

    bool IsPlayLoading;

    float PlayLoadingAnimation;

    AsyncOperation PlayAsync;


    void Awake()
    {
        instance = this;
    }


    void Start()
    {
        GM = GameManager.instance;


        GM.Android_Bar_Show(true, true, 0x00000000, 0x00000000, false, false);


        ProjectListObject.SetActive(true);
        TitleObject.SetActive(true);
        NewProjectObject.SetActive(true);

        Select_RootObject.gameObject.SetActive(true);
        Select_ChangeNameObject.gameObject.SetActive(true);
        Select_ShareObject.gameObject.SetActive(true);

        SelectObject.SetActive(true);


        if (GM.PsI.l == null) GM.PsI.l = new int[]{};

        PL_NoProject.SetActive(GM.PsI.l.Length == 0);

        PL_Content.GetComponent<RectTransform>().sizeDelta = new Vector2(PL_Content.GetComponent<RectTransform>().sizeDelta.x, 20 + GM.PsI.l.Length * 440);
        

        for (int i = 0; i < GM.PsI.l.Length; i++)
        {
            var NewObject = Instantiate(PL_Prefab, PL_Content.transform);

            NewObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -20 - i * 440);

            NewObject.GetComponent<ProjectListObject>().id = GM.PsI.l[i];
            NewObject.GetComponent<ProjectListObject>().ButtonScroll = PL_Scroll;

            NewObject.GetComponent<ProjectListObject>().ProjectName.text = GM.PsI.L[i].N;
            NewObject.GetComponent<ProjectListObject>().LatestUpdateDate.text = GM.PsI.L[i].D;

            NewObject.GetComponent<ProjectListObject>().Online.SetActive(GM.PsI.L[i].O);

            PL_ProjectObjectList.Add(NewObject);
        }
    }


    void Update()
    {
        //プロジェクト一覧画面

        TitleObject.GetComponent<CanvasGroup>().blocksRaycasts = !IsProjectList;

        GM.Animation(ref ProjectListAnimation, 4, IsProjectList);

        TitleObject.GetComponent<CanvasGroup>().alpha = 1 - ProjectListAnimation;
        TitleObject.GetComponent<RectTransform>().localScale = new Vector3(1 + ProjectListAnimation * 0.05f, 1 + ProjectListAnimation * 0.05f, 1);


        //選択画面

        SelectObject.GetComponent<CanvasGroup>().blocksRaycasts = IsSelect;

        GM.Animation(ref SelectAnimation, 6, IsSelect);

        SelectObject.GetComponent<CanvasGroup>().alpha = SelectAnimation;


        GM.AnimationUpOnly(ref Select_PageAnimation, 4);

        Select_RootObject.blocksRaycasts = (Select_Page == select_Page.Root);
        Select_RootObject.alpha = (Select_Page == select_Page.Root ? Select_PageAnimation : 0);


        Select_ChangeNameObject.blocksRaycasts = (Select_Page == select_Page.ChangeName);
        Select_ChangeNameObject.alpha = (Select_Page == select_Page.ChangeName ? Select_PageAnimation : 0);


        Select_ShareObject.blocksRaycasts = (Select_Page == select_Page.Share);
        Select_ShareObject.alpha = (Select_Page == select_Page.Share ? Select_PageAnimation : 0);


        S_S_ShareInformation.blocksRaycasts = S_S_CheckBox.isOn;
        S_S_ShareInformation.alpha = S_S_CheckBox.isOn ? 1 : 0.25f;


        //新規作成画面

        ProjectListObject.GetComponent<CanvasGroup>().blocksRaycasts = IsProjectList && !IsNewProject;//
        NewProjectObject.GetComponent<CanvasGroup>().blocksRaycasts = IsProjectList && IsNewProject;

        GM.Animation(ref NewProjectAnimation, 4, IsNewProject);

        ProjectListObject.GetComponent<CanvasGroup>().alpha = (IsNewProject ? 0 : ProjectListAnimation);
        NewProjectObject.GetComponent<CanvasGroup>().alpha = (IsNewProject ? ProjectListAnimation : 0);

        ProjectListObject.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = (IsNewProject ? 0 : 1 - NewProjectAnimation);
        NewProjectObject.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = (IsNewProject ? NewProjectAnimation : 0);


        //Create画面へ

        if (IsCreateLoading)
        {
            CreateLoadingAnimation += Time.deltaTime;

            LoadingObject.SetActive(CreateLoadingAnimation < 2.9f);

            LightFilter.raycastTarget = true;

            LightFilter.color = new Color32(255, 255, 255, (byte)((CreateLoadingAnimation < 2) ? 0 : CreateLoadingAnimation > 2.5f ? 255 : (CreateLoadingAnimation - 2) * 2 * 255));


            if (CreateLoadingAnimation > 3f)
            {
                CreateAsync.allowSceneActivation = true;
            }
        }


        //Play画面へ

        if (IsPlayLoading)
        {
            PlayLoadingAnimation += Time.deltaTime;

            LightFilter.raycastTarget = true;

            LightFilter.color = new Color32(255, 255, 255, (byte)(Mathf.Clamp(PlayLoadingAnimation * 4 * 255, 0, 255)));

            if (PlayLoadingAnimation > 1)
            {
                PlayAsync.allowSceneActivation = true;
            }
        }
    }


    public ProjectsList PsI_NowSI()
    {
        return GM.PsI.L[GM.PsI.l.ToList().IndexOf(SelectID)];
    }


    public void SwitchProjectList()
    {
        IsProjectList = !IsProjectList;
        
        if (IsProjectList)
        {
            IsNewProject = false;
        }
    }


    public void SwitchSelect(int id)
    {
        if (Select_Page == select_Page.ChangeName && S_CN_InputField.text == "")
        {
            S_CN_NoTitle.SetActive(true);
        }
        else
        {
            S_CN_NoTitle.SetActive(false);

            IsSelect = !IsSelect;

            if (IsSelect)
            {
                Select_Page = select_Page.Root;

                SelectID = id;
            }
            else
            {
                switch (Select_Page)
                {
                    case select_Page.ChangeName:

                        PsI_NowSI().N = S_CN_InputField.text;
                        PsI_NowSI().D = DateTime.Now.ToShortDateString();

                        GM.SaveJson(GM.PsI);

                        GM.PD = GM.LoadJson<projectdata>(GM.Path(2, SelectID.ToString(), "ProjectData.json"));

                        GM.PD.Ifm.N = S_CN_InputField.text;
                        GM.PD.Ifm.D = DateTime.Now.ToShortDateString();

                        GameObject ProjectObject = PL_ProjectObjectList[GM.PsI.l.ToList().IndexOf(SelectID)];

                        ProjectObject.GetComponent<ProjectListObject>().ProjectName.text = S_CN_InputField.text;
                        ProjectObject.GetComponent<ProjectListObject>().LatestUpdateDate.text = DateTime.Now.ToShortDateString();

                    break;
                }
            }
        }
    }

    public void Select_ChangePage(string To)
    {
        switch (To)
        {
            case "Root":

                Select_Page = select_Page.Root;

            break;

            case "ChangeName":

                Select_Page = select_Page.ChangeName;

                S_CN_InputField.text = PsI_NowSI().N;

            break;

            case "Share":

                Select_Page = select_Page.Share;

                S_S_CheckBox.GetComponent<ToggleSwitch>().isOn = GM.PsI.L[GM.PsI.l.ToList().IndexOf(SelectID)].O;
                S_S_CheckBox.GetComponent<ToggleSwitch>().PushAnimation = GM.PsI.L[GM.PsI.l.ToList().IndexOf(SelectID)].O ? 1 : 0;

                if (GM.PsI.L[GM.PsI.l.ToList().IndexOf(SelectID)].O)
                {
                    S_S_ShareCode.text = GM.PsI.L[GM.PsI.l.ToList().IndexOf(SelectID)].SK;
                }
                else
                {
                    S_S_ShareCode.text = "-非表示-";
                }

                S_S_ErrorMessage.text = "";

            break;
        }

        Select_PageAnimation = 0;
    }

    public void ShareProject()
    {
        StartCoroutine(ShareProjectIE());
    }

    public IEnumerator ShareProjectIE()
    {
        LoadingObject.SetActive(true);

        bool check = false;

        if (!S_S_CheckBox.isOn)
        {
            //プロジェクトを公開する場合

            IEnumerator GetExclusion = GM.OnlineGetData<string>(null, "Exclusion");

            yield return GetExclusion;

            if (GM.OnlineDataErrorMessage == "")
            {
                if (GetExclusion.Current.ToString() == "true")
                {
                    //書き込み可能が確定する. 排他制御をfalseにしておく
                    IEnumerator IE;


                    IE = GM.OnlinePostData<string>("false" , null, "Exclusion");

                    yield return IE;


                    //SKLを取得

                    IE = GM.OnlineGetData<onlineprojects>(null, "ShareKeyList");

                    yield return IE;

                    onlineprojects ShareKeyListJson = (onlineprojects)IE.Current;


                    //共有コード12桁発行&登録

                    string sharekey = "";

                    if (GM.PsI.L[GM.PsI.l.ToList().IndexOf(SelectID)].SK == "000000000000")
                    {
                        while (sharekey == "" || sharekey == "000000000000" || ShareKeyListJson.SKL.Contains(sharekey))
                        {
                            sharekey = "";

                            for (int i = 0; i < 12; i++)
                            {
                                int num = UnityEngine.Random.Range(0, 36);

                                sharekey += Encoding.UTF8.GetString(new byte[]{(byte)(num + (num < 10 ? 48 : 87))});
                            }
                        }

                        if (ShareKeyListJson.SKL == null) ShareKeyListJson.SKL = new string[]{};
                        if (ShareKeyListJson.R == null) ShareKeyListJson.R = new bool[]{};

                        ShareKeyListJson.SKL = ShareKeyListJson.SKL.Concat(new List<string>{ sharekey }).ToArray();
                        ShareKeyListJson.R = ShareKeyListJson.R.Concat(new List<bool>{ true }).ToArray();
                    }
                    else
                    {
                        sharekey = GM.PsI.L[GM.PsI.l.ToList().IndexOf(SelectID)].SK;

                        ShareKeyListJson.R[ShareKeyListJson.SKL.ToList().IndexOf(GM.PsI.L[GM.PsI.l.ToList().IndexOf(SelectID)].SK)] = true;
                    }

                    IE = GM.OnlinePostData<onlineprojects>(ShareKeyListJson, null, "ShareKeyList");

                    yield return IE;


                    //排他制御を解除

                    IE = GM.OnlinePostData<string>("true" , null, "Exclusion");

                    yield return IE;


                    //アップロード開始！！

                    var PD = GM.LoadJson<projectdata>(GM.Path(2, SelectID.ToString(), "ProjectData.json"));

                    IE = GM.OnlinePostData<projectdata>(PD, null, "Projects", sharekey, "ProjectData");

                    yield return IE;


                    LoadingObject.GetComponent<Loading>().IsShowMessage = true;


                    for (int i = 0; i < PD.I.Length; i++)
                    {
                        //Debug.Log(BitConverter.ToString(GM.LoadBytes(GM.Path(2, SelectID.ToString(), "Images", PD.i[i].ToString()))));

                        IE = GM.OnlinePostData<byte[]>(GM.LoadBytes(GM.Path(2, SelectID.ToString(), "Images", PD.i[i].ToString())), (progress) => 
                        {
                            LoadingObject.GetComponent<Loading>().LoadingMessage = $"Uploading ({ i + 1 }/{ PD.I.Length + PD.SI.Length }) { Mathf.Round(progress * 100) }%......";
                        }
                        , "Projects", sharekey, "Images", PD.i[i].ToString());

                        yield return IE;

                        if (GM.OnlineDataErrorMessage != "") break;
                    }

                    if (GM.OnlineDataErrorMessage == "")
                    {
                        for (int i = 0; i < PD.SI.Length; i++)
                        {
                            IE = GM.OnlinePostData<byte[]>(GM.LoadBytes(GM.Path(2, SelectID.ToString(), "System Images", i.ToString())), (progress) => 
                            {
                                LoadingObject.GetComponent<Loading>().LoadingMessage = $"Uploading ({ PD.I.Length + i + 1 }/{ PD.I.Length + PD.SI.Length }) { Mathf.Round(progress * 100) }%......";
                            }
                            , "Projects", sharekey, "SystemImages", i.ToString());

                            yield return IE;

                            if (GM.OnlineDataErrorMessage != "") break;
                        }

                        if (GM.OnlineDataErrorMessage == "")
                        {
                            GM.PsI.L[GM.PsI.l.ToList().IndexOf(SelectID)].SK = sharekey;
                            GM.PsI.L[GM.PsI.l.ToList().IndexOf(SelectID)].O = true;

                            GM.SaveJson(GM.PsI);

                            S_S_ShareCode.text = sharekey;

                            PL_ProjectObjectList[GM.PsI.l.ToList().IndexOf(SelectID)].GetComponent<ProjectListObject>().Online.SetActive(true);

                            check = true;
                            S_S_ErrorMessage.text = "";
                        }
                        else
                        {
                            S_S_ErrorMessage.text = GM.OnlineDataErrorMessage;
                        }
                    }
                    else
                    {
                        S_S_ErrorMessage.text = GM.OnlineDataErrorMessage;
                    }

                    LoadingObject.GetComponent<Loading>().IsShowMessage = false;
                    LoadingObject.GetComponent<Loading>().Update();
                }
                else
                {
                    S_S_ErrorMessage.text = "サーバーにアクセスが集中しています。\n数秒後に再トライしてみて下さい！";
                }
            }
            else
            {
                S_S_ErrorMessage.text = GM.OnlineDataErrorMessage;
            }
        }
        else
        {
            //プロジェクトを非公開にする場合

            IEnumerator GetExclusion = GM.OnlineGetData<string>(null, "Exclusion");

            yield return GetExclusion;

            if (GM.OnlineDataErrorMessage == "")
            {
                if (GetExclusion.Current.ToString() == "true")
                {
                    //書き込み可能が確定する. 排他制御をfalseにしておく
                    IEnumerator IE;

                    IE = GM.OnlinePostData<string>("false" , null, "Exclusion");

                    yield return IE;


                    //SKLを取得

                    IE = GM.OnlineGetData<onlineprojects>(null, "ShareKeyList");

                    yield return IE;

                    onlineprojects ShareKeyListJson = (onlineprojects)IE.Current;


                    ShareKeyListJson.R[ShareKeyListJson.SKL.ToList().IndexOf(GM.PsI.L[GM.PsI.l.ToList().IndexOf(SelectID)].SK)] = false;


                    IE = GM.OnlinePostData<onlineprojects>(ShareKeyListJson, null, "ShareKeyList");

                    yield return IE;


                    GM.PsI.L[GM.PsI.l.ToList().IndexOf(SelectID)].O = false;

                    GM.SaveJson(GM.PsI);

                    PL_ProjectObjectList[GM.PsI.l.ToList().IndexOf(SelectID)].GetComponent<ProjectListObject>().Online.SetActive(false);


                    IE = GM.OnlinePostData<string>("true" , null, "Exclusion");

                    yield return IE;
                }
                else
                {
                    S_S_ErrorMessage.text = "サーバーにアクセスが集中しています。\n数秒後に再トライしてみて下さい！";
                }
            }
            else
            {
                S_S_ErrorMessage.text = GM.OnlineDataErrorMessage;
            }

            check = true;

            S_S_ShareCode.text = "-未取得-";
        }

        if (check)
        {
            S_S_CheckBox.isOn = !S_S_CheckBox.isOn;
        }

        LoadingObject.SetActive(false);
    }

    public void SNS_Share()
    {
#if UNITY_EDITOR

        GUIUtility.systemCopyBuffer = GM.PsI.L[GM.PsI.l.ToList().IndexOf(SelectID)].SK;

        UnityEditor.EditorUtility.DisplayDialog("クリップボードに文字を貼り付けました", GUIUtility.systemCopyBuffer, "OK");

#else

        var payload = new SharePayload();

        payload.AddText("Circleで世界に一つのゲームを作ろう！");
        payload.Commit();

#endif
    }

    public void SwitchNewProject()
    {
        IsNewProject = !IsNewProject;

        if (IsNewProject)
        {
            NP_NoTitle.SetActive(false);
        }
    }

    public void GotoPlay()
    {
        PlayAsync = SceneManager.LoadSceneAsync("Play");
        PlayAsync.allowSceneActivation = false;

        IsPlayLoading = true;
    }

    public void GotoCreateOpen()
    {
        GM.ProjectLocalID = SelectID;

        IsCreateLoading = true;

        GM.PD = GM.LoadJson<projectdata>();

        CreateAsync = SceneManager.LoadSceneAsync("Create");
        CreateAsync.allowSceneActivation = false;

        
        GM.EditCapture.Clear();

        if (GM.PD.si == null) GM.PD.si = new int[]{};
        if (GM.PD.SI == null) GM.PD.SI = new Images[]{};

        for (int i = 0; i < GM.PD.si.Length; i++)
        {
            GM.EditCapture.Add(GM.LoadImage(GM.Path(3, "System Images", i.ToString()), GM.PD.SI[i].W, GM.PD.SI[i].H));
        }
    }

    public void GotoCreateNew()
    {
        if (NP_InputField.text == "")
        {
            NP_NoTitle.SetActive(true);
        }
        else
        {
            NP_NoTitle.SetActive(false);

            IsCreateLoading = true;


            //ファイル作成

            if (GM.PsI.l == null) GM.PsI.l = new int[]{};
            if (GM.PsI.L == null) GM.PsI.L = new ProjectsList[]{};

            GM.PsI.l = GM.PsI.l.Concat(new List<int>{ GM.PsI.l.Length == 0 ? 0 : GM.PsI.l.Max() + 1 }).ToArray();
            GM.PsI.L = GM.PsI.L.Concat(new List<ProjectsList>{ new ProjectsList
            {
                N = NP_InputField.text,
                D = DateTime.Now.ToShortDateString(),
                O = false,
                SK = "000000000000"
            }
            }).ToArray();

            GM.SaveJson(GM.PsI);


            GM.ProjectLocalID = GM.PsI.l.Last();


            if (!Directory.Exists(GM.Path(3)))
            {
                Directory.CreateDirectory(GM.Path(3));
            }

            if (!Directory.Exists(GM.Path(3, "Images")))
            {
                Directory.CreateDirectory(GM.Path(3, "Images"));
            }

            if (!Directory.Exists(GM.Path(3, "System Images")))
            {
                Directory.CreateDirectory(GM.Path(3, "System Images"));
            }

            if (!File.Exists(GM.Path(4)))
            {
                GM.PD = new projectdata
                {
                    s = new int[]{0},

                    Ns = 0,

                    S = new Scene[]{ new Scene
                    { 
                        N = "ホーム画面",
                        P = new Vector2(0, 19),
                        S = 1.3f,
                        si = -1
                    }
                    },

                    Ifm = new Information
                    {
                        N = GM.PsI.L[GM.ProjectLocalID].N,
                        D = DateTime.Now.ToShortDateString()
                    }
                };

                GM.SaveJson();
            }
            else
            {
                GM.PD = GM.LoadJson<projectdata>();
            }


            CreateAsync = SceneManager.LoadSceneAsync("Create");
            CreateAsync.allowSceneActivation = false;

            GM.EditCapture.Clear();
        }
    }
}
