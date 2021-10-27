using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class CSVExport : MonoBehaviour
{
    public bool Export;
    private StreamWriter sw;

    void Start()
    {
        DateTime dt = DateTime.Now;
        string filePath = @"3FloorLv4_result_" + dt.ToString("yyyy-MM-dd-HH-mm-ss") + ".csv";
        if (Export)
        {
            sw = new StreamWriter(filePath, true, Encoding.GetEncoding("UTF-8"));
            string[] s1 = { "X", "Z", "Success?", "ReachStair_Floor1", "ReachStair_Floor2", "Water_height" };
            string s2 = string.Join(",", s1);
            sw.WriteLine(s2);
        }
        else Debug.LogWarning("Boolean value, Export, is false.");
    }

    public void SaveData(string txt1, string txt2, string txt3, string txt4, string txt5, string txt6)
    {
        if (Export)
        {
            string[] s1 = { txt1, txt2, txt3, txt4, txt5, txt6 };
            string s2 = string.Join(",", s1);
            sw.WriteLine(s2);
            sw.Flush();
        }
    }

    public void FinishCSVExport()
    {
        if (Export)
        {
            sw.Flush();
            sw.Close();
            Debug.Log("Saved CSV file.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && Export)
        {
            sw.Flush();
            sw.Close();
            Debug.Log("Saved CSV file.");
        }
    }
}