using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectPlacement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    GM_Create GM_Create;

    GM_Execute GM_Execute;

    public int id;

    void Start()
    {
        GM_Create = GM_Create.instance;

        GM_Execute = GM_Execute.instance;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GM_Create != null)
        {
            GM_Create.PushObject(id);
        }

        if (GM_Execute != null)
        {
            GM_Execute.ObjectsTap[id].IsDown = true;
            GM_Execute.ObjectsTap[id].Is = true;
        }
    }

    public  void OnPointerUp(PointerEventData eventData)
    {
        if (GM_Execute != null)
        {
            GM_Execute.ObjectsTap[id].IsDown = false;
            GM_Execute.ObjectsTap[id].IsUp = true;
        }
    }
}
