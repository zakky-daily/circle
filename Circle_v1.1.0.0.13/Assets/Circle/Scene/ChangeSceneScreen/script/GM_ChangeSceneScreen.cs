using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GM_ChangeSceneScreen : MonoBehaviour
{
    GameManager GM;

    public static GM_ChangeSceneScreen instance;


    //シーン移動

    public bool IsChangeSceneScreen = true;

    float ChangeSceneScreenAniamtion;

    [SerializeField] GameObject ChangeSceneScreenObject;

    AsyncOperation CreateAsync;


    //ボタン表示

    [SerializeField] GameObject ScenePrefab;

    [SerializeField] GameObject SceneContent;

    public DetectExpandScroll Scroll;

    [SerializeField] RectTransform SceneAddButton;


    void Awake()
    {
        instance = this;
    }


    void Start()
    {
        GM = GameManager.instance;

        ChangeSceneScreenAniamtion = 0;


        //ボタン表示

        SceneContent.GetComponent<RectTransform>().sizeDelta = new Vector2(SceneContent.GetComponent<RectTransform>().sizeDelta.x, 20 + (GM.PD.S.Length + 2) / 2 * 1300);

        int index;

        for (index = 0; index < GM.PD.S.Length; index++)
        {
            var NewObject = Instantiate(ScenePrefab, SceneContent.transform);

            var rect = NewObject.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2((index % 2 == 0 ? 0 : 0.5f), 1);
            rect.anchorMax = new Vector2((index % 2 == 0 ? 0.5f : 1), 1);

            rect.offsetMin = new Vector2((index % 2 == 0 ? 60 : 30), rect.offsetMin.y);
            rect.offsetMax = new Vector2((index % 2 == 0 ? -30 : -60), rect.offsetMax.y);

            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -20 - index / 2 * 1300);


            var ScenePlacement = NewObject.GetComponent<ScenePlacement>();

            ScenePlacement.id = index;

            var id = GM.PD.si.ToList().IndexOf(GM.PD.S[index].si);

            if (id != -1) ScenePlacement.SceneImage.sprite = Sprite.Create(GM.EditCapture[id], new Rect(0, 0, GM.PD.SI[id].W, GM.PD.SI[id].H), Vector2.zero);

            ScenePlacement.SceneIndex.text = "#" + (index + 1).ToString();

            ScenePlacement.SceneName.text = GM.PD.S[index].N;
        }


        SceneAddButton.anchorMin = new Vector2((index % 2 == 0 ? 0 : 0.5f), 1);
        SceneAddButton.anchorMax = new Vector2((index % 2 == 0 ? 0.5f : 1), 1);

        SceneAddButton.offsetMin = new Vector2((index % 2 == 0 ? 60 : 30), SceneAddButton.offsetMin.y);
        SceneAddButton.offsetMax = new Vector2((index % 2 == 0 ? -30 : -60), SceneAddButton.offsetMax.y);

        SceneAddButton.anchoredPosition = new Vector2(SceneAddButton.anchoredPosition.x, -20 - index / 2 * 1300);
    }

    void Update()
    {
        //シーン移動

        GM.Animation(ref ChangeSceneScreenAniamtion, 6, IsChangeSceneScreen);

        ChangeSceneScreenObject.GetComponent<CanvasGroup>().alpha = ChangeSceneScreenAniamtion;

        ChangeSceneScreenObject.GetComponent<RectTransform>().anchorMin = new Vector2(-0.2f + ChangeSceneScreenAniamtion * 0.2f, 0);
        ChangeSceneScreenObject.GetComponent<RectTransform>().anchorMax = new Vector2(0.8f + ChangeSceneScreenAniamtion * 0.2f, 1);

        ChangeSceneScreenObject.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        ChangeSceneScreenObject.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        if (!IsChangeSceneScreen && ChangeSceneScreenAniamtion == 0)
        {
            CreateAsync.allowSceneActivation = true;
        }
    }

    public void GotoCreate()
    {
        IsChangeSceneScreen = false;

        CreateAsync = SceneManager.LoadSceneAsync("Create");

        CreateAsync.allowSceneActivation = false;
    }

    public void NewScene()
    {
        GM.PD.s = GM.PD.s.Concat(new List<int>(){ GM.PD.s.Max() + 1 }).ToArray();

        GM.PD.S = GM.PD.S.Concat(new List<Scene>(){ new Scene
        {
            N = "シーン" + GM.PD.s.Length,
            P = new Vector2(0, 19),
            S = 1.3f,
            si = -1
        }
        }).ToArray();

        GM.SaveJson();


        var index = GM.PD.S.Length - 1;


        var NewObject = Instantiate(ScenePrefab, SceneContent.transform);

        var rect = NewObject.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2((index % 2 == 0 ? 0 : 0.5f), 1);
        rect.anchorMax = new Vector2((index % 2 == 0 ? 0.5f : 1), 1);

        rect.offsetMin = new Vector2((index % 2 == 0 ? 60 : 30), rect.offsetMin.y);
        rect.offsetMax = new Vector2((index % 2 == 0 ? -30 : -60), rect.offsetMax.y);

        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -20 - index / 2 * 1300);


        var ScenePlacement = NewObject.GetComponent<ScenePlacement>();

        ScenePlacement.id = index;

        var id = GM.PD.si.ToList().IndexOf(GM.PD.S[index].si);

        if (id != -1) ScenePlacement.SceneImage.sprite = Sprite.Create(GM.EditCapture[id], new Rect(0, 0, GM.PD.SI[id].W, GM.PD.SI[id].H), Vector2.zero);

        ScenePlacement.SceneIndex.text = "#" + (index + 1).ToString();

        ScenePlacement.SceneName.text = GM.PD.S[index].N;


        SceneContent.GetComponent<RectTransform>().sizeDelta = new Vector2(SceneContent.GetComponent<RectTransform>().sizeDelta.x, 20 + (GM.PD.S.Length + 2) / 2 * 1300);


        index++;

        SceneAddButton.anchorMin = new Vector2((index % 2 == 0 ? 0 : 0.5f), 1);
        SceneAddButton.anchorMax = new Vector2((index % 2 == 0 ? 0.5f : 1), 1);

        SceneAddButton.offsetMin = new Vector2((index % 2 == 0 ? 60 : 30), SceneAddButton.offsetMin.y);
        SceneAddButton.offsetMax = new Vector2((index % 2 == 0 ? -30 : -60), SceneAddButton.offsetMax.y);

        SceneAddButton.anchoredPosition = new Vector2(SceneAddButton.anchoredPosition.x, -20 - index / 2 * 1300);
    }
}
