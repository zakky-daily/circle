using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddMenuButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    GameManager GM;
    GM_Create GM_Create;

    [SerializeField] RectTransform ChildObject;

    [SerializeField] Image Black;

    [SerializeField] UnityEvent Push;

    bool IsPush;
    float PushAnimation;

    [SerializeField] DetectExpandScroll ButtonScroll;


    void Start()
    {
        GM = GameManager.instance;
        GM_Create = GM_Create.instance;
    }

    void Update()
    {
        GM.Animation(ref PushAnimation, 6, IsPush);

        Black.color = new Color32(0, 0, 0, (byte)(PushAnimation * 63));

        ChildObject.localScale = new Vector3(1 - PushAnimation * 0.1f, 1 - PushAnimation * 0.1f, 1);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPush = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPush = false;

        if (!ButtonScroll.IsScroll)
        {
            Push.Invoke();
        }
    }
}
