using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PropertyWindowMultiLineTextScrollBar : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    GameManager GM;
    Color DefaultColor;

    bool IsPush;
    float Animation;

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPush = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPush = false;
    }

    void Start()
    {
        GM = GameManager.instance;
        DefaultColor = GetComponent<Image>().color;
    }

    void Update()
    {
        GM.Animation(ref Animation, 6, IsPush);
        
        GetComponent<Image>().color = new Color(DefaultColor.r, DefaultColor.g, DefaultColor.b, DefaultColor.a * (1 - Animation * 0.1f));
    }
}
