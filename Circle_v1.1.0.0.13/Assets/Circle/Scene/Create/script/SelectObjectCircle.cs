using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectObjectCircle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    GM_Create GM_Create;

    [SerializeField] Vector2 CirclePos;

    void Start()
    {
        GM_Create = GM_Create.instance;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GM_Create.PushCursor_Corner(CirclePos);
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
}
