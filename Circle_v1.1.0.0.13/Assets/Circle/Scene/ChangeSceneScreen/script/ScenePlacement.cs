using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ScenePlacement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    GameManager GM;

    GM_ChangeSceneScreen GM_ChangeSceneScreen;

    public int id;

    public Image SceneImage;

    public TextMeshProUGUI SceneIndex;

    public TextMeshProUGUI SceneName;

    public TMP_InputField SceneNameInput;


    bool IsPush;

    [SerializeField] RectTransform PushObject;

    [SerializeField] Image Black;

    float PushAnimation;



    void Start()
    {
        GM = GameManager.instance;

        GM_ChangeSceneScreen = GM_ChangeSceneScreen.instance;

        SceneNameInput.GetComponent<TMProRecieveText>().OnEndEdit.AddListener(text => EndNameEdit());
    }


    void Update()
    {
        GM.Animation(ref PushAnimation, 6, IsPush);

        PushObject.localScale = new Vector3(1 - PushAnimation * 0.1f, 1 - PushAnimation * 0.1f, 1);

        Black.color = new Color32(0, 0, 0, (byte)(PushAnimation * 63));
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        IsPush = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPush = false;

        if (!GM_ChangeSceneScreen.Scroll.IsScroll)
        {
            GM.PD.Ns = id;

            GM.SaveJson();

            GM_ChangeSceneScreen.GotoCreate();
        }
    }


    public void StartNameEdit()
    {
        SceneName.gameObject.SetActive(false);
        SceneNameInput.gameObject.SetActive(true);

        SceneNameInput.text = SceneName.text;

        SceneNameInput.Select();
    }

    public void EndNameEdit()
    {
        SceneName.gameObject.SetActive(true);
        SceneNameInput.gameObject.SetActive(false);

        SceneName.text = SceneNameInput.text;

        GM.PD.S[GM.PD.s.ToList().IndexOf(id)].N = SceneNameInput.text;

        GM.SaveJson();
    }
}
