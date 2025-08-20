using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

[ExecuteAlways]
public class CameraSize : MonoBehaviour
{
    public static CameraSize instance;

    bool IsPortrait;

    void Awake() 
    {
        void CameraAwake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }  
        }

#if UNITY_EDITOR
        if (EditorApplication.isPlaying) CameraAwake();
#else
        CameraAwake();
#endif
    }

    void Start()
    {
        
    }

    void Update()
    {
        IsPortrait = (float)Screen.height / (float)Screen.width > 1.5f;

        if (IsPortrait)
            GetComponent<Camera>().orthographicSize = (float)Screen.height / (float)Screen.width * 9;
        else
            GetComponent<Camera>().orthographicSize = 13.5f;
    }
}
