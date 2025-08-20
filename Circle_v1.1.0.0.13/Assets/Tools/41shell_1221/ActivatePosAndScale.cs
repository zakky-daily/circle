using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ActivatePosAndScale : MonoBehaviour
{
    [Serializable]
    class PosFormat
    {
        public RectTransform[] RectTransform;
        public float Number;
        public bool IsMinus;
    }

    [Serializable]
    class SizeFormat
    {
        public RectTransform[] RectTransform;
        public float Number;
    }

    [SerializeField] bool enable_Pos_X;
    [SerializeField] PosFormat Pos_X = new PosFormat();
    [SerializeField] bool enable_Pos_Y;
    [SerializeField] PosFormat Pos_Y = new PosFormat();
    [SerializeField] bool enable_Width;
    [SerializeField] SizeFormat Width = new SizeFormat();
    [SerializeField] bool enable_Height;
    [SerializeField] SizeFormat Height = new SizeFormat();

    void Start()
    {
        
    }

    void Update()
    {
        Vector2 vector;
        float sum;

        vector = GetComponent<RectTransform>().anchoredPosition;

        sum = 0;
        if (enable_Pos_X)
        {
            foreach (RectTransform RectTransform in Pos_X.RectTransform)
            {
                sum += RectTransform.sizeDelta.x;
            }
            vector.x = sum * (Pos_X.IsMinus ? -1 : 1) + Pos_X.Number;
        }

        sum = 0;
        if (enable_Pos_Y)
        {
            foreach (RectTransform RectTransform in Pos_Y.RectTransform)
            {
                sum += RectTransform.sizeDelta.y;
            }
            vector.y = sum * (Pos_Y.IsMinus ? -1 : 1) + Pos_Y.Number;
        }

        GetComponent<RectTransform>().anchoredPosition = vector;


        vector = GetComponent<RectTransform>().sizeDelta;
        
        sum = 0;
        if (enable_Width)
        {
            foreach (RectTransform RectTransform in Width.RectTransform)
            {
                sum += RectTransform.sizeDelta.x;
            }
            vector.x = sum + Width.Number;
        }

        sum = 0;
        if (enable_Height)
        {
            foreach (RectTransform RectTransform in Height.RectTransform)
            {
                sum += RectTransform.sizeDelta.y;
            }
            vector.y = sum + Height.Number;
        }

        GetComponent<RectTransform>().sizeDelta = vector;
    }
}
