using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ProjectListObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    GameManager GM;

    GM_Home GM_Home;

    public int id;

    public TextMeshProUGUI ProjectName;
    public TextMeshProUGUI LatestUpdateDate;


    [SerializeField] RectTransform ChildObject;

    [SerializeField] Image Black;

    public GameObject Online;

    bool IsPush;
    float PushAnimation;

    public DetectExpandScroll ButtonScroll;


    void Start()
    {
        GM = GameManager.instance;

        GM_Home = GM_Home.instance;
    }

    void Update()
    {
        GM.Animation(ref PushAnimation, 6, IsPush);

        Black.color = new Color32(0, 0, 0, (byte)(PushAnimation * 63));

        //ChildObject.localScale = new Vector3(1 - PushAnimation * 0.1f, 1 - PushAnimation * 0.1f, 1);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPush = true;
    }

    public  void OnPointerUp(PointerEventData eventData)
    {
        IsPush = false;

        if (!ButtonScroll.IsScroll)
        {
            GM_Home.SwitchSelect(id);
        }
    }
}

