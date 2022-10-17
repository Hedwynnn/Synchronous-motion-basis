using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;  //操作文件夹时需引用该命名空间
using System.Text;

public class TimeRecorder : MonoBehaviour
{
    TextAsset m_Txt;
    public static TimeRecorder Instance;
    private float startTime;
    StreamWriter sw;
    FileInfo fi;
    string path;
    int num = 0;
    private void Awake(){
        Instance = this;
    }

    void Start()
    {
        startTime = Time.time;        
        path = Application.dataPath + "/OutputInfo"+num.ToString()+".txt";
        while(File.Exists(path))
        {
            num++;
            path = Application.dataPath + "/OutputInfo"+num.ToString()+".txt";
        }
        fi = new FileInfo(path);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void AddTxtTextByFileInfo(string txtText, bool _printTime = true)
    { 
        if (!File.Exists(path))
        {
            sw = fi.CreateText();
        }
        else {
            sw = fi.AppendText();   //在原文件后面追加内容      
        }
        float curTime = Time.time - startTime;
        if(_printTime)
            sw.WriteLine(txtText + curTime.ToString());
        else
            sw.WriteLine(txtText);
        sw.Close();
        sw.Dispose();
    }

}



    