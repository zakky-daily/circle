using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class RoundedShape : MonoBehaviour
{
    [Range(1, 600)] public int Radius = 1;

    void Start()
    {
        GetComponent<Image>().sprite = Resources.Load<Sprite>("RoundShape");
        GetComponent<Image>().type = Image.Type.Sliced;
    }

    void Update()
    {
        GetComponent<Image>().pixelsPerUnitMultiplier = 600f / Radius;
    }
}
