using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class TMProRecieveText : MonoBehaviour
{
    public UnityEvent<string> OnValueChanged;

    public UnityEvent<string> OnEndEdit;

    public UnityEvent<string> OnSelect;

    TMP_InputField inputField;

    TouchScreenKeyboard.Status TouchScreenKeyboardStatus;

    string beforeInputText;
    string inputText;


    void Start()
    {
        inputField = GetComponent<TMP_InputField>();

        inputField.onTouchScreenKeyboardStatusChanged.AddListener(ProcessDonePressed);

        inputField.onValueChanged.AddListener((text) => ChangeText());

        inputField.onEndEdit.AddListener((text) => FinishEditText());

        inputField.onSelect.AddListener((text) => StartEditText());
    }

    void ProcessDonePressed(TouchScreenKeyboard.Status newStatus)
    {
        TouchScreenKeyboardStatus = newStatus;
    }


    public void ChangeText()
    {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR

        if (inputField.isFocused)
        {
            // 他のところをタップした時
            beforeInputText = inputText;
            inputText = inputField.text;
        }

        Debug.Log(beforeInputText);
        Debug.Log(inputText);

#endif

        OnValueChanged.Invoke(inputField.text);
    }


    public void FinishEditText()
    {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR

        if (TouchScreenKeyboardStatus == TouchScreenKeyboard.Status.Done || TouchScreenKeyboardStatus == TouchScreenKeyboard.Status.LostFocus)
        {
            // 入力完了時何かに渡す

        }
        else
        { 
            // 他の部分をタップした場合
            inputField.text = beforeInputText;
        }

#endif

        OnEndEdit.Invoke(inputField.text);
    }

    public void StartEditText()
    {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR

        beforeInputText = inputField.text;
        inputText = inputField.text;

        Debug.Log(beforeInputText);
        Debug.Log(inputText);

#endif

        OnSelect.Invoke(inputField.text);
    }
}
