using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Loading : MonoBehaviour
{
    [SerializeField, Range(0.5f, 2)] float Speed;

    float Timer;

    public string LoadingMessage;

    public bool IsShowMessage;

    [SerializeField] RectTransform Background;
    [SerializeField] GameObject Progress;

    void Start()
    {
        
    }

    public void Update()
    {
        if (IsShowMessage)
        {
            Background.sizeDelta = new Vector2(1080, 540);
            Progress.SetActive(true);

            Progress.GetComponent<TextMeshProUGUI>().text = LoadingMessage;
        }
        else
        {
            Background.sizeDelta = new Vector2(400, 400);
            Progress.SetActive(false);
        }

        Timer += Time.deltaTime;

        for (int i = 0; i < 8; i++)
        {
            float color = 255 - ((Timer / Speed + i / 8f) % 1) * 159;
            transform.GetChild(0).GetChild(0).GetChild(i).GetComponent<Image>().color = new Color32((byte)color, (byte)color, (byte)color, 255);
        }
    }
}
