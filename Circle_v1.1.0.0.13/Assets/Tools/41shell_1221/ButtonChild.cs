using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class ButtonChild : MonoBehaviour
{
    public Color DefaultColor;
    [SerializeField] bool Alpha0Disable = true;
    [SerializeField] GameObject TargetObject;

    GameObject Parent;

    void Start()
    {
        if (GetComponent<Image>() != null)
        {
            DefaultColor = GetComponent<Image>().color;
        }
        else if (GetComponent<TextMeshProUGUI>() != null)
        {
            DefaultColor = GetComponent<TextMeshProUGUI>().color;
        }

        if (TargetObject == null)
        {
            Parent = gameObject;

            while (Parent.transform.parent != null && (Parent.GetComponent<Button>() == null && Parent.GetComponent<TMP_Dropdown>() == null))
            {
                Parent = Parent.transform.parent.gameObject;
            }
        }
        else
        {
            Parent = TargetObject;
        }

        Update();
    }

    void Update()
    {
        if (Parent.GetComponent<Button>() != null)
        {
            Color ParentNowColor = Parent.GetComponent<Button>().targetGraphic.canvasRenderer.GetColor();

            AttachColor(ParentNowColor);
        }
        else if (Parent.GetComponent<TMP_Dropdown>() != null)
        {
            Color ParentNowColor = Parent.GetComponent<TMP_Dropdown>().targetGraphic.canvasRenderer.GetColor();

            AttachColor(ParentNowColor);
        }
    }

    void AttachColor(Color ParentNowColor)
    {
        if (Alpha0Disable) ParentNowColor.a = 1;
        
        if (GetComponent<Image>() != null)
        {
            GetComponent<Image>().color = ParentNowColor * DefaultColor;
        }
        else if (GetComponent<TextMeshProUGUI>() != null)
        {
            GetComponent<TextMeshProUGUI>().color = ParentNowColor * DefaultColor;
        }
    }
}
