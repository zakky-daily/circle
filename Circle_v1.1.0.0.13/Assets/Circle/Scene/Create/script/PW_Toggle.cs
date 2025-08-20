using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PW_Toggle : MonoBehaviour
{
    GameManager GM;
    GM_Create GM_Create;

    [SerializeField] GameObject ReceiveButton;
    [SerializeField] GameObject ReceiveText;

    [SerializeField] GameObject SendButton;
    [SerializeField] GameObject SendText;

    [SerializeField] GameObject Cursor;
    Color CursorColor;

    bool IsRecieve;
    float ReceiveAnimation;

    bool IsSend;
    float SendAnimation;

    public enum buttonType{ Receive, Send, Cursor }


    void Start()
    {
        GM = GameManager.instance;
        GM_Create = GM_Create.instance;


        EventTrigger.Entry entry = new EventTrigger.Entry();

        RegistListener(EventTriggerType.PointerDown, buttonType.Receive);
        RegistListener(EventTriggerType.PointerUp, buttonType.Receive);
        RegistListener(EventTriggerType.PointerDown, buttonType.Send);
        RegistListener(EventTriggerType.PointerUp, buttonType.Send);

        CursorColor = Cursor.GetComponent<Image>().color;
    }

    void Update()
    {
        GM.Animation(ref SendAnimation, 6, IsSend);

        SendButton.GetComponent<Image>().color = new Color32(191, 191, 191, (byte)(SendAnimation * 255));


        GM.Animation(ref ReceiveAnimation, 6, IsRecieve);

        ReceiveButton.GetComponent<Image>().color = new Color32(191, 191, 191, (byte)(ReceiveAnimation * 255));


        float Color = (GM.PW_IsSend ? 255 - 63 * SendAnimation : 255 - 63 * ReceiveAnimation);

        Cursor.GetComponent<Image>().color = CursorColor * new Color32((byte)Color, (byte)Color, (byte)Color, 255);
    }


    void RegistListener(EventTriggerType eventTriggerType, buttonType ButtonType)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = eventTriggerType;
        entry.callback.AddListener((eventDate) => Button(ButtonType, (eventTriggerType == EventTriggerType.PointerDown ? true : false)));

        switch (ButtonType)
        {
            case buttonType.Send:

                SendButton.GetComponent<EventTrigger>().triggers.Add(entry);

            break;

            case buttonType.Receive:

                ReceiveButton.GetComponent<EventTrigger>().triggers.Add(entry);

            break;

            case buttonType.Cursor:

                Cursor.GetComponent<EventTrigger>().triggers.Add(entry);

            break;
        }
    }


    public void Button(buttonType ButtonType, bool IsPushDown)
    {
        if (ButtonType == buttonType.Cursor)
        {
            ButtonType = (GM.PW_IsSend ? buttonType.Send : buttonType.Receive);
        }

        if (ButtonType == buttonType.Send)
        {
            IsSend = IsPushDown;
        }
        else
        {
            IsRecieve = IsPushDown;
        }

        if (!IsPushDown && GM.PW_IsSend ^ ButtonType == buttonType.Send)
        {
            GM_Create.PW_SwitchToggle();
        }
    }
}
