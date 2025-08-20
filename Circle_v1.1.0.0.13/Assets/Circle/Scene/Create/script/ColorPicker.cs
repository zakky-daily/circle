using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    [SerializeField, Range(0, 359)] int h; 

    float h_temp;

    void Start()//
    {
        var tex = new Texture2D(1024, 1024);

        for (int x = 0; x < tex.width; x++) for (int y = 0; y < tex.height; y++)
        {
            if (new Vector2(x - 511.5f, y - 511.5f).sqrMagnitude < Mathf.Pow(512, 2))
            {
                int h = (int)(Mathf.Atan2(y - 511.5f, x - 511.5f) * Mathf.Rad2Deg);

                if (h < 0) h += 360;

                int R; int G; int B;

                if (h < 60)
                {
                    R = 255;
                    G = (int)((h / 60f) * (255));
                    B = 0;
                }
                else if (h < 120)
                {
                    R = (int)(((120 - h) / 60f) * (255));
                    G = 255;
                    B = 0;
                }
                else if (h < 180)
                {
                    R = 0;
                    G = 255;
                    B = (int)(((h - 120) / 60f) * (255));
                }
                else if (h < 240)
                {
                    R = 0;
                    G = (int)(((240 - h) / 60f) * (255));
                    B = 255;
                }
                else if (h < 300)
                {
                    R = (int)(((h - 240) / 60f) * (255));
                    G = 0;
                    B = 255;
                }
                else
                {
                    R = 255;
                    G = 0;
                    B = (int)(((360 - h) / 60f) * (255));
                }

                tex.SetPixel(x, y, new Color32((byte)R, (byte)G, (byte)B, 255));
            }
            else
            {
                tex.SetPixel(x, y, new Color32(0, 0, 0, 0));
            }
        }

        tex.Apply();

        GetComponent<RawImage>().texture = tex;

        byte[] Data = tex.EncodeToPNG();

        GameManager.instance.SaveImage(Data, GameManager.instance.Path(0, "ColorCircle.png"));
    }

    void Update()
    {

    }


    void SetColor()
    {
        var tex = new Texture2D(256, 256);

        for (int s = 0; s < 256; s++) for (int v = 0; v < 256; v++)
        {
            int R;
            int G;
            int B;

            if (h < 60)
            {
                R = 255;
                G = (int)((h / 60f) * (255));
                B = 0;
            }
            else if (h < 120)
            {
                R = (int)(((120 - h) / 60f) * (255));
                G = 255;
                B = 0;
            }
            else if (h < 180)
            {
                R = 0;
                G = 255;
                B = (int)(((h - 120) / 60f) * (255));
            }
            else if (h < 240)
            {
                R = 0;
                G = (int)(((240 - h) / 60f) * (255));
                B = 255;
            }
            else if (h < 300)
            {
                R = (int)(((h - 240) / 60f) * (255));
                G = 0;
                B = 255;
            }
            else
            {
                R = 255;
                G = 0;
                B = (int)(((360 - h) / 60f) * (255));
            }

            tex.SetPixel(s, v, new Color32((byte)R, (byte)G, (byte)B, 255));

            
            if (s == 127 && v == 255)
            {
                Debug.Log(new Color32((byte)R, (byte)G, (byte)B, 255));
            }
            
        }

        tex.Apply();

        GetComponent<RawImage>().texture = tex;
    }
}
