using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteAlways]
public class TMProSizeFit : MonoBehaviour
{
    [SerializeField] bool IsWidthAuto;
    [SerializeField] bool IsHeightAuto;

    void Start()
    {
        
    }

    void Update()
    {
        Vector2 SizeDelta = GetComponent<RectTransform>().sizeDelta;

        if (IsWidthAuto) SizeDelta.x = GetComponent<TextMeshProUGUI>().preferredWidth;
        if (IsHeightAuto) SizeDelta.y = GetComponent<TextMeshProUGUI>().preferredHeight;

        GetComponent<RectTransform>().sizeDelta = SizeDelta;
    }
}
