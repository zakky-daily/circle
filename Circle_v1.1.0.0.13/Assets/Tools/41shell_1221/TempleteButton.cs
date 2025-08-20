using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TempleteButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    GameManager GM;

    bool IsPush;
    float PushAnimation;

    [SerializeField] UnityEvent Push;

    [SerializeField] RectTransform ChildObject;


    void Start()
    {
        GM = GameManager.instance;
    }

    void Update()
    {
        GM.Animation(ref PushAnimation, 6, IsPush);

        GetComponent<Image>().color = new Color32(191, 191, 191, (byte)(PushAnimation * 255));
        if (ChildObject != null) ChildObject.localScale = new Vector3(1 - PushAnimation * 0.1f, 1 - PushAnimation * 0.1f, 1);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPush = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPush = false;

        Push.Invoke();
    }
}
