using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ProgrammingPlacement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    GameManager GM;
    GM_Create GM_Create;

    public int id;

    public RectTransform small;

    public TextMeshProUGUI text;

    public Image Icon;


    bool IsPush;

    [SerializeField] Image Black;

    float Animation;


    public DetectExpandScroll ScrollRect;

    public bool interactable;


    void Start()
    {
        GM = GameManager.instance;
        GM_Create = GM_Create.instance;
    }

    void Update()
    {
        GM.Animation(ref Animation, 6, IsPush);

        Black.color = new Color32(0, 0, 0, (byte)(Animation * 63));

        small.localScale = new Vector3(1 - Animation * 0.05f, 1 - Animation * 0.05f, 1);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (interactable)
        {
            IsPush = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (interactable)
        {
            IsPush = false;

            if (!ScrollRect.IsScroll)
            {
                switch (GM.PD_NowM().P[GM.PD_NowM().p.ToList().IndexOf(id)].T)
                {
                    case "Text":

                        GM_Create.PW_MD_SwitchTextEdit(id);

                    break;

                    case "ChangeScene":

                        GM_Create.PW_MD_SwitchCSEdit(id);

                    break;
                }
            }
        }
    }
}
