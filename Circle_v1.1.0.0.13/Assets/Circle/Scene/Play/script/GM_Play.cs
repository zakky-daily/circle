using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GM_Play : MonoBehaviour
{
    GameManager GM;

    public static GM_Play instance;


    //GotoHome

    bool IsGotoHome;

    float GotoHomeAnimation;

    [SerializeField] Image GotoFilter;

    AsyncOperation GotoHomeAsync;


    //GotoExecute

    bool IsGotoExecute;

    float GotoExecuteAnimation;

    AsyncOperation GotoExecuteAsync;



    //画面遷移

    bool IsSaveList;

    [SerializeField] RectTransform SearchObject;
    [SerializeField] RectTransform SaveListObject;

    [SerializeField] Transform SearchButton;
    [SerializeField] Transform SaveListButton;

    Color32 batch;

    [SerializeField] GameObject Save_Nt;
    [SerializeField] TextMeshProUGUI Save_Nt_Count;

    float SaveListAnimation;

    bool S_IsDiscover;

    [SerializeField] CanvasGroup ShareNowObject;

    [SerializeField] CanvasGroup DiscoverObject;

    float S_DiscoverAnimation;


    [SerializeField] GameObject LoadingObject;


    [SerializeField] TMP_InputField S_SN_KeyInput;
    [SerializeField] TextMeshProUGUI S_SN_Error;

    [SerializeField] TextMeshProUGUI S_D_Error;

    public bool S_D_IsOpen;

    [SerializeField] TextMeshProUGUI S_D_ProjectTitle;
    [SerializeField] TextMeshProUGUI S_D_ProjectInf;

    [SerializeField] TextMeshProUGUI S_D_Message1;
    [SerializeField] TextMeshProUGUI S_D_ButtonName;


    //保存済みリスト

    public List<GameObject> OnlineProjectObjectList = new List<GameObject>();

    [SerializeField] GameObject OnlinePrefab;
    [SerializeField] GameObject OnlineContent;
    [SerializeField] DetectExpandScroll OnlineScrollRect;


    void Awake()
    {
        instance = this;
    }


    void Start()
    {
        GM = GameManager.instance;

        GM.Android_Bar_Show(false, true, 0x00000000, 0xff000000, true, false);

        ShareNowObject.gameObject.SetActive(true);
        DiscoverObject.gameObject.SetActive(true);

        batch = SearchButton.GetChild(0).GetComponent<Image>().color;

        for (int i = 0; i < GM.DsI.l.Length; i++)
        {
            var NewObject = Instantiate(OnlinePrefab, OnlineContent.transform);

            OnlineProjectObjectList.Add(NewObject);

            ReflectProjectObject(NewObject, GM.DsI.l[i]);
        }
    }

    void Update()
    {
        //GotoHome

        if (IsGotoHome)
        {
            GotoHomeAnimation += Time.deltaTime;

            GotoFilter.raycastTarget = true;
            
            GotoFilter.color = new Color32(0, 0, 0, (byte)Mathf.Clamp(GotoHomeAnimation * 4 * 255, 0, 255));

            if (GotoHomeAnimation > 1)
            {
                GotoHomeAsync.allowSceneActivation = true;
            }
        }


        //GotoExecute

        if (IsGotoExecute)
        {
            GotoExecuteAnimation += Time.deltaTime;

            GotoFilter.raycastTarget = true;
            
            GotoFilter.color = new Color32(0, 0, 0, (byte)Mathf.Clamp(GotoExecuteAnimation * 2 * 255, 0, 255));

            if (GotoExecuteAnimation > 0.5f)
            {
                GotoExecuteAsync.allowSceneActivation = true;
            }
        }


        //保存済みの通知

        Save_Nt.SetActive(GM.DsI.D_NC != 0);
        Save_Nt_Count.text = (GM.DsI.D_NC > 9 ? "9+" : GM.DsI.D_NC.ToString());


        //画面遷移

        GM.Animation(ref SaveListAnimation, 6, IsSaveList);

        SearchObject.anchorMin = new Vector2(- SaveListAnimation, 0);
        SearchObject.anchorMax = new Vector2(1 - SaveListAnimation, 1);

        SaveListObject.anchorMin = new Vector2(1 - SaveListAnimation, 0);
        SaveListObject.anchorMax = new Vector2(2 - SaveListAnimation, 1);

        var gray = new Color32(159, 159, 159, 255);

        for (int i = 0; i < 2; i++)
        {
            SearchButton.GetChild(i).GetComponent<ButtonChild>().DefaultColor = (IsSaveList ? gray : batch);
            SaveListButton.GetChild(i).GetComponent<ButtonChild>().DefaultColor = (IsSaveList ? batch : gray);
        }


        ShareNowObject.blocksRaycasts = !S_IsDiscover;
        DiscoverObject.blocksRaycasts = S_IsDiscover;

        GM.Animation(ref S_DiscoverAnimation, 2, S_IsDiscover);

        if (S_IsDiscover)
        {
            ShareNowObject.alpha = Mathf.Clamp((0.5f - S_DiscoverAnimation) * 2, 0, 255);
        }
        else
        {
            ShareNowObject.alpha = Mathf.Clamp(1 - S_DiscoverAnimation, 0, 255);
        }
        DiscoverObject.alpha = (S_DiscoverAnimation == 1 ? 1 : 0);


        //作品発見ページ

        if (S_D_IsOpen)
        {
            S_D_Message1.text = "正常にダウンロードできました！！";
            S_D_ButtonName.text = "開く！";
        }
        else
        {
            S_D_Message1.text = "プロジェクトが見つかりました！\nダウンロードしますか？";
            S_D_ButtonName.text = "ダウンロード！";
        }
    }


    public void GotoHome()
    {
        IsGotoHome = true;

        GotoHomeAsync = SceneManager.LoadSceneAsync("Home");

        GotoHomeAsync.allowSceneActivation = false;
    }


    public void GotoExecute()
    {
        IsGotoExecute = true;

        GM.Path_IsDownload = true;
        GM.PD = GM.LoadJson<projectdata>(GM.Path(4));

        GotoExecuteAsync = SceneManager.LoadSceneAsync("Execute");

        GotoExecuteAsync.allowSceneActivation = false;

        GM.IsExecute = true;
    }


    public void SwitchSaveList(bool Is)
    {
        IsSaveList = Is;

        if (Is)
        {
            GM.DsI.D_NC = 0;
            GM.SaveJson(GM.DsI, GM.Path(1, "Downloads/ProjectsInformation.json"));
        }
    }


    public void ReflectProjectObject(GameObject AddObject, int id)
    {
        var PO = AddObject.GetComponent<OnlineProjectListObject>();

        PO.id = id;

        PO.ButtonScroll = OnlineScrollRect;

        var pl = GM.DsI.L[GM.DsI.l.ToList().IndexOf(id)];

        PO.ProjectName.text = pl.N;

        PO.LatestUpdateDate.text = $"制作者: {pl.U}\n最終制作日: {pl.D}";

        AddObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(AddObject.GetComponent<RectTransform>().anchoredPosition.x, -10 - 560 * GM.DsI.l.ToList().IndexOf(id));


        OnlineContent.GetComponent<RectTransform>().sizeDelta = new Vector2(OnlineContent.GetComponent<RectTransform>().sizeDelta.x, GM.DsI.l.Length * 560 + 10);
    }


    public void SwitchS_Discover()
    {
        if (!S_IsDiscover)
        {
            if (S_SN_KeyInput.text == "")
            {
                S_SN_Error.text = "共有コードを入力してください！";
            }
            else if (S_SN_KeyInput.text.Length != 12)
            {
                S_SN_Error.text = "共有コードは12文字です！";
            }
            else
            {
                StartCoroutine(ProjectDiscover());
            }
        }
        else
        {
            S_IsDiscover = false;
        }
    }

    public IEnumerator ProjectDiscover()
    {
        LoadingObject.SetActive(true);
        
        IEnumerator IE;

        IE = GM.OnlineGetData<onlineprojects>(null, "ShareKeyList");

        yield return IE;

        if (GM.OnlineDataErrorMessage == "")
        {
            onlineprojects ShareKeyListJson = (onlineprojects)IE.Current;

            if (ShareKeyListJson.SKL.Contains(S_SN_KeyInput.text))
            {
                if (ShareKeyListJson.R[ShareKeyListJson.SKL.ToList().IndexOf(S_SN_KeyInput.text)])
                {
                    IE = GM.OnlineGetData<projectdata>(null, "Projects", S_SN_KeyInput.text, "ProjectData");
                    yield return IE;

                    var PD = IE.Current as projectdata;

                    S_D_ProjectTitle.text = PD.Ifm.N;
                    S_D_ProjectInf.text = $"制作者: {PD.Ifm.U}\n最終制作日: {PD.Ifm.D}";


                    S_SN_Error.text = "";
                    S_IsDiscover = true;

                    S_D_IsOpen = false;
                }
                else
                {
                    S_SN_Error.text = "このプロジェクトは現在非公開です！";
                }
            }
            else
            {
                S_SN_Error.text = "この共有コードのプロジェクトは存在しません！\n入力ミスはありませんか？";
            }
        }
        else
        {
            S_SN_Error.text = GM.OnlineDataErrorMessage;
        }

        LoadingObject.SetActive(false);
    }

    public void DownloadProject()
    {
        if (S_D_IsOpen)
        {
            SwitchSaveList(true);
        }
        else
        {
            StartCoroutine(DownloadProjectIE());
        }
    }

    public IEnumerator DownloadProjectIE()
    {        
        IEnumerator IE;

        IE = GM.OnlineGetData<string>(null, "Exclusion");

        yield return IE;

        if (GM.OnlineDataErrorMessage == "")
        {
            LoadingObject.SetActive(true);


            GM.DsI.l = GM.DsI.l.Concat(new List<int>{ GM.DsI.l.Length == 0 ? 0 : GM.DsI.l.Max() + 1 }).ToArray();

            if (!Directory.Exists(GM.Path(1, "Downloads", GM.DsI.l.Last().ToString())))
            {
                Directory.CreateDirectory(GM.Path(1, "Downloads", GM.DsI.l.Last().ToString()));
            }

            IE = GM.OnlineGetData<projectdata>(null, "Projects", S_SN_KeyInput.text, "ProjectData");

            yield return IE;

            var PD = IE.Current as projectdata;

            GM.DsI.L = GM.DsI.L.Concat(new List<ProjectsList>{ new ProjectsList
            {
                N = PD.Ifm.N,
                D = PD.Ifm.D,
                O = false
            }
            }).ToArray();

            GM.SaveJson(IE.Current, GM.Path(1, "Downloads", GM.DsI.l.Last().ToString(), "ProjectData.json"));


            LoadingObject.GetComponent<Loading>().IsShowMessage = true;


            if (!Directory.Exists(GM.Path(1, "Downloads", GM.DsI.l.Last().ToString(), "Images")))
            {
                Directory.CreateDirectory(GM.Path(1, "Downloads", GM.DsI.l.Last().ToString(), "Images"));
            }

            for (int i = 0; i < PD.I.Length; i++)
            {
                IE = GM.OnlineGetData<byte[]>((progress) => 
                {
                    LoadingObject.GetComponent<Loading>().LoadingMessage = $"Downloading ({ i + 1 }/{ PD.I.Length + PD.SI.Length }) { Mathf.Round(progress * 100) }%......";
                }
                , "Projects", S_SN_KeyInput.text, "Images", PD.i[i].ToString());

                yield return IE;

                GM.SaveImage((byte[])IE.Current, GM.Path(1, "Downloads", GM.DsI.l.Last().ToString(), "Images", PD.i[i].ToString()));

                if (GM.OnlineDataErrorMessage != "") break;
            }

            if (GM.OnlineDataErrorMessage == "")
            {
                if (!Directory.Exists(GM.Path(1, "Downloads", GM.DsI.l.Last().ToString(), "System Images")))
                {
                    Directory.CreateDirectory(GM.Path(1, "Downloads", GM.DsI.l.Last().ToString(), "System Images"));
                }

                for (int i = 0; i < PD.SI.Length; i++)
                {
                    IE = GM.OnlineGetData<byte[]>((progress) => 
                    {
                        LoadingObject.GetComponent<Loading>().LoadingMessage = $"Downloading ({ PD.I.Length + i + 1 }/{ PD.I.Length + PD.SI.Length }) { Mathf.Round(progress * 100) }%......";
                    }
                    , "Projects", S_SN_KeyInput.text, "SystemImages", PD.i[i].ToString());

                    yield return IE;

                    GM.SaveImage((byte[])IE.Current, GM.Path(1, "Downloads", GM.DsI.l.Last().ToString(), "System Images", PD.i[i].ToString()));

                    if (GM.OnlineDataErrorMessage != "") break;
                }

                if (GM.OnlineDataErrorMessage == "")
                {
                    GM.DsI.D_NC += 1;

                    S_D_IsOpen = true;

                    S_D_Error.text = "";


                    var NewObject = Instantiate(OnlinePrefab, OnlineContent.transform);

                    OnlineProjectObjectList.Add(NewObject);

                    ReflectProjectObject(NewObject, GM.DsI.l.Last());
                }
                else
                {
                    S_D_Error.text = GM.OnlineDataErrorMessage;
                }
            }
            else
            {
                S_D_Error.text = GM.OnlineDataErrorMessage;
            }

            LoadingObject.GetComponent<Loading>().IsShowMessage = false;
            LoadingObject.GetComponent<Loading>().Update();
        }
        else
        {
            S_D_Error.text = GM.OnlineDataErrorMessage;
        }


        GM.SaveJson(GM.DsI, GM.Path(1, "Downloads/ProjectsInformation.json"));

        LoadingObject.SetActive(false);
    }
}
