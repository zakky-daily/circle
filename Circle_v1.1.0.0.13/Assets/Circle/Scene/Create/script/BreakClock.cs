using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteAlways]
public class BreakClock : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI NowTime;
    [SerializeField] TextMeshProUGUI NowDate;

    void Start()
    {
        
    }

    void Update()
    {
        NowTime.text = DateTime.Now.ToString("HH:mm");
        
        string[] week = {"Sun.", "Mon.", "Tue.", "Wed.", "Thu.", "Fri.", "Sat."};
        NowDate.text = DateTime.Now.ToString("MM/dd") + " " + week[(int)DateTime.Now.DayOfWeek];
    }
}
