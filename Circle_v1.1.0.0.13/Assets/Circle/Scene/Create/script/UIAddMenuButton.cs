using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIAddMenuButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    GameManager GM;
    GM_Create GM_Create;

    RectTransform ChildObject;

    [SerializeField] Image Character_Illust;

    [SerializeField] UnityEvent Push;

    bool IsPush;
    float PushAnimation;

    [SerializeField] DetectExpandScroll ButtonScroll;


    void Start()
    {
        GM = GameManager.instance;
        GM_Create = GM_Create.instance;

        ChildObject = transform.GetChild(0).GetComponent<RectTransform>();
    }

    void Update()
    {
        GM.Animation(ref PushAnimation, 6, IsPush);

        GetComponent<Image>().color = new Color32(191, 191, 191, (byte)(PushAnimation * 255));
        ChildObject.localScale = new Vector3(1 - PushAnimation * 0.1f, 1 - PushAnimation * 0.1f, 1);

        float CharacterColor = 255 - PushAnimation * 63;
        if (Character_Illust != null) Character_Illust.color = new Color32((byte)CharacterColor, (byte)CharacterColor, (byte)CharacterColor, 255);
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
