using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PW_AM_SelectCandidacy : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    GameManager GM;
    GM_Create GM_Create;

    public int id;

    public TextMeshProUGUI text;

    public DetectExpandScroll ScrollRect;

    bool IsPush;

    float Animation;
    
    void Start()
    {
        GM = GameManager.instance;
        GM_Create = GM_Create.instance;
    }
    
    void Update()
    {
        GM.Animation(ref Animation, 6, IsPush);

        GetComponent<Image>().color = new Color32(0, 0, 0, (byte)(Animation * 63));
        text.GetComponent<RectTransform>().localScale = new Vector3(1 - Animation * 0.15f, 1 - Animation * 0.15f, 1);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPush = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPush = false;

        if (!ScrollRect.IsScroll) GM_Create.PW_AM_SelectMessage(id);
    }
}
