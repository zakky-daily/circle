using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PW_AddMessageToggle : MonoBehaviour
{
    GameManager GM;
    GM_Create GM_Create;

    [SerializeField] GameObject NewButton;

    [SerializeField] GameObject SelectButton;

    [SerializeField] GameObject Cursor;
    Color CursorColor;

    bool IsNew;
    float NewAnimation;

    bool IsSelect;
    float SelectAnimation;

    public enum buttonType{ New, Select, Cursor }


    void Start()
    {
        GM = GameManager.instance;
        GM_Create = GM_Create.instance;


        EventTrigger.Entry entry = new EventTrigger.Entry();

        RegistListener(EventTriggerType.PointerDown, buttonType.New);
        RegistListener(EventTriggerType.PointerUp, buttonType.New);
        RegistListener(EventTriggerType.PointerDown, buttonType.Select);
        RegistListener(EventTriggerType.PointerUp, buttonType.Select);

        CursorColor = Cursor.GetComponent<Image>().color;
    }

    void Update()
    {
        GM.Animation(ref SelectAnimation, 6, IsSelect);

        SelectButton.GetComponent<Image>().color = new Color32(191, 191, 191, (byte)(SelectAnimation * 255));


        GM.Animation(ref NewAnimation, 6, IsNew);

        NewButton.GetComponent<Image>().color = new Color32(191, 191, 191, (byte)(NewAnimation * 255));


        float Color = (GM_Create.PW_AM_IsSelect ? 255 - 63 * SelectAnimation : 255 - 63 * NewAnimation);

        Cursor.GetComponent<Image>().color = CursorColor * new Color32((byte)Color, (byte)Color, (byte)Color, 255);
    }


    void RegistListener(EventTriggerType eventTriggerType, buttonType ButtonType)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = eventTriggerType;
        entry.callback.AddListener((eventDate) => Button(ButtonType, (eventTriggerType == EventTriggerType.PointerDown ? true : false)));

        switch (ButtonType)
        {
            case buttonType.Select:

                SelectButton.GetComponent<EventTrigger>().triggers.Add(entry);

            break;

            case buttonType.New:

                NewButton.GetComponent<EventTrigger>().triggers.Add(entry);

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
            ButtonType = (GM_Create.PW_AM_IsSelect ? buttonType.Select : buttonType.New);
        }

        if (ButtonType == buttonType.Select)
        {
            IsSelect = IsPushDown;
        }
        else
        {
            IsNew = IsPushDown;
        }

        if (!IsPushDown && GM_Create.PW_AM_IsSelect ^ ButtonType == buttonType.Select)
        {
            GM_Create.PW_AM_Toggle();
        }
    }
}
