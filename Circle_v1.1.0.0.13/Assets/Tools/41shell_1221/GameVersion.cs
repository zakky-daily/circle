using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteAlways]
public class GameVersion : MonoBehaviour
{
    [SerializeField] string Sentence;

    void Update()
    {
        GetComponent<TextMeshProUGUI>().text = Sentence + " " + Application.version;
    }
}
