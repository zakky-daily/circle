using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class BatchColorChange : MonoBehaviour
{
#if UNITY_EDITOR

    [SerializeField, Range(0, 4)] int Index;

    [Serializable]
    public class colorJson
    {
        public ColorInt[] ColorList;
    }

    public Color32 Color;
    Color32 TempColor;

    colorJson ColorJson;

    void Start()
    {
        ColorJson = LoadJson();
        Color = new Color32((byte)ColorJson.ColorList[Index].R, (byte)ColorJson.ColorList[Index].G, (byte)ColorJson.ColorList[Index].B, 255);
        TempColor = Color;
    }

    void Update()
    {
        if (!EditorApplication.isPlaying)
        {
            if ((Color)Color != (Color)TempColor)
            {
                ColorJson.ColorList[Index] = new ColorInt{ R = Color.r, G = Color.g, B = Color.b, A = 255};
                TempColor = Color;
                SaveJson();
            }
            else
            {
                ColorJson = LoadJson();

                if ((Color)Color != new Color32((byte)ColorJson.ColorList[Index].R, (byte)ColorJson.ColorList[Index].G, (byte)ColorJson.ColorList[Index].B, 255))
                {
                    Color = new Color32((byte)ColorJson.ColorList[Index].R, (byte)ColorJson.ColorList[Index].G, (byte)ColorJson.ColorList[Index].B, 255);
                    TempColor = Color;
                }
            }


            if (GetComponent<ButtonChild>() != null)
            {
                GetComponent<ButtonChild>().DefaultColor = Color; 
            }
            else if (GetComponent<Image>() != null)
            {
                GetComponent<Image>().color = Color; 
            }
            else if (GetComponent<TextMeshProUGUI>() != null)
            {
                GetComponent<TextMeshProUGUI>().color = Color;
            }
        }
    }


    public void SaveJson()
    {
        string datastr = JsonUtility.ToJson(ColorJson, true);
        File.WriteAllText("Assets/Circle/Resources/BatchColor.json", datastr);
    }

    public colorJson LoadJson()
    {
        string datastr = File.ReadAllText("Assets/Circle/Resources/BatchColor.json");

        return JsonUtility.FromJson<colorJson>(datastr);
    }

#endif
}
