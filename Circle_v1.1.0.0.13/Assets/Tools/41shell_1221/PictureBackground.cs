using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class PictureBackground : MonoBehaviour
{
    public enum FitOrZoom
    {
        Fit, Zoom
    }
    [SerializeField] FitOrZoom stretch;

    [SerializeField] GameObject ClipObject;

    void Start()
    {

    }

    public void Update()
    {
        Rect ClipObjectRect;
        if (ClipObject == null)
        {
            ClipObjectRect = transform.parent.gameObject.GetComponent<RectTransform>().rect;
        }
        else
        {
            ClipObjectRect = ClipObject.GetComponent<RectTransform>().rect;
        }

        Rect rect = (GetComponent<Image>().sprite != null ? GetComponent<Image>().sprite.rect : new Rect(0, 0, 1, 1));

        if (rect.height / rect.width < ClipObjectRect.height / ClipObjectRect.width ^ stretch == FitOrZoom.Fit)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2
            (
                ClipObjectRect.height / rect.height * rect.width,
                ClipObjectRect.height
            );
        }
        else
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2
            (
                ClipObjectRect.width,
                ClipObjectRect.width / rect.width * rect.height
            );
        }
    }

    public Vector2 ScreenSize(float Times)
    {
        if ((float)Screen.height / (float)Screen.width > 1.5f)
        {
            return new Vector2(Times, (float)Screen.height / (float)Screen.width * Times);
        }
        else
        {
            return new Vector2((float)Screen.width / (float)Screen.height * Times, Times) * 1.5f;
        }
    }
}
