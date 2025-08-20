using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteAlways]
public class TMProFrameColor : MonoBehaviour
{
    public Color FaceColor = new Color32(255, 255, 255, 255);
    public Color OutlineColor = new Color32(0, 0, 0, 255);
    [Range(-1, 1)] public float FaceDilate;
    [Range(0, 1)] public float OutlineThickness;

    Color _OutlineColor;
    float _FaceDilate;
    float _OutlineThickness;

    Material NewTMPro;

    void Start()
    {
        NewTMPro = GetComponent<TextMeshProUGUI>().fontMaterial;
    }

    void Update()
    {
        NewTMPro.SetColor("_FaceColor", FaceColor);
        NewTMPro.SetColor("_OutlineColor", OutlineColor);
        NewTMPro.SetFloat("_FaceDilate", FaceDilate);
        NewTMPro.SetFloat("_OutlineWidth", OutlineThickness);
        NewTMPro.name = "Font SDF (Instance)";

        GetComponent<TextMeshProUGUI>().fontMaterial = NewTMPro;
    }
}
