using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Random_obj : MonoBehaviour
{
    FieldOfView fow;
    public GameObject obj;  //用于复制的目标
    public bool waiting = false;    //程序是否在运行,避免连续输入导致多次刷新
    Run run;

    public Transform Targets2;

    public Transform Targets;
    public GameObject layPointer;
    public int num = 0; //固定
    int[] by = new int[20] {1, 2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20};
    Transform[] childrens;

    void Start()
    {
        run = GameObject.Find("Player").GetComponent<Run>();
        fow = GameObject.Find("Player").GetComponent<FieldOfView>();

        childrens = Targets.GetComponentsInChildren<Transform>();

    }

    // 固定目标
    public void StartGame()
    {        
        foreach(Transform children in childrens)
        {
            if(children.GetComponent<Renderer>() != null)
                fow.allTargets.Add(children);
        }
        Targets.gameObject.SetActive(true);
        
		int j, temp;
		for (int x = 0; x < 100; x++)
		{
			j = UnityEngine.Random.Range(0, 20); //随机生成的位置在0~9之间
			temp = by[j];
			for (int i = j; i < 19; i++)//左移
				by[i] = by[i + 1];
			by[19] = temp;
		}
        fow.tobeSelectedTarget = childrens[by[num]];
        Material materialGreen = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        materialGreen.color = Color.green;
        fow.tobeSelectedTarget.GetComponent<Renderer>().material = materialGreen;
        
        switch(run.ssPattern.currentState.name)
        {
            case "RayState":
                layPointer.SetActive(true);
                break;
            case "MotionmatchingState":
                layPointer.SetActive(false);
                break;
            case "NullState":
                layPointer.SetActive(false);
                break;
        }
        waiting = false;
        num++;
        run.ClearMaterial();
        TimeRecorder.Instance.AddTxtTextByFileInfo("目标出现:");
    }

    public void nextGame()
    {
        // Debug.Log(by[num]);

        
        // fow.tobeSelectedTarget = childrens[by[num]];
        fow.tobeSelectedTarget = childrens[num];
        Material materialGreen = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        materialGreen.color = Color.green;
        fow.tobeSelectedTarget.GetComponent<Renderer>().material = materialGreen;
        
        switch(run.ssPattern.currentState.name)
        {
            case "RayState":
                layPointer.SetActive(true);
                break;
            case "MotionmatchingState":
                layPointer.SetActive(false);
                break;
            case "NullState":
                layPointer.SetActive(false);
                break;
        }
        waiting = false;
        num++;
        run.ClearMaterial();
        TimeRecorder.Instance.AddTxtTextByFileInfo("目标出现:");
    }

    // 随即目标
    // public void StartGame()
    // {
    //     ClearAll();
    //     for (int i = 0; i < 40; i++) 
    //     {
    //         GameObject clone =Instantiate(obj, new Vector3(Random.Range(-5f,5f),
    //                                                     Random.Range(0f,10f),
    //                                                     Random.Range(-5f,5f)), Quaternion.identity);
    //         clone.SetActive(true);
    //         fow.randomNewTargets.Add(clone.transform);                
    //     } 

    //     int rand = UnityEngine.Random.Range(0, fow.randomNewTargets.Count);
    //     fow.tobeSelectedTarget = fow.randomNewTargets[rand];
    //     Material materialGreen = new Material(Shader.Find("Universal Render Pipeline/Lit"));
    //     materialGreen.color = Color.green;
    //     fow.tobeSelectedTarget.GetComponent<Renderer>().material = materialGreen;
        
    //     switch(run.ssPattern.currentState.name)
    //     {
    //         case "RayState":
    //             layPointer.SetActive(true);
    //             break;
    //         case "MotionmatchingState":
    //             layPointer.SetActive(false);
    //             break;
    //         case "NullState":
    //             layPointer.SetActive(false);
    //             break;
    //     }
    //     waiting = false;

    //     TimeRecorder.Instance.AddTxtTextByFileInfo("目标出现:");
    // }

    public void closeTarget()
    {
        Targets.gameObject.SetActive(false);
    }

    public void changeTargets()
    {
        Targets = Targets2;
        Targets.gameObject.SetActive(true);
        
        run = GameObject.Find("Player").GetComponent<Run>();
        fow = GameObject.Find("Player").GetComponent<FieldOfView>();

        childrens = Targets.GetComponentsInChildren<Transform>();
        foreach(Transform children in childrens)
        {
            if(children.GetComponent<Renderer>() != null)
                fow.allTargets.Add(children);
        }

		int j, temp;
		for (int x = 0; x < 100; x++)
		{
			j = UnityEngine.Random.Range(0, 20); //随机生成的位置在0~9之间
			temp = by[j];
			for (int i = j; i < 19; i++)//左移
				by[i] = by[i + 1];
			by[19] = temp;
		}
        // fow.tobeSelectedTarget = childrens[by[num]];
        num = 1;
        fow.tobeSelectedTarget = childrens[num];
        Material materialGreen = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        materialGreen.color = Color.green;
        fow.tobeSelectedTarget.GetComponent<Renderer>().material = materialGreen;
        
        switch(run.ssPattern.currentState.name)
        {
            case "RayState":
                layPointer.SetActive(true);
                break;
            case "MotionmatchingState":
                layPointer.SetActive(false);
                break;
            case "NullState":
                layPointer.SetActive(false);
                break;
        }
        waiting = false;
        num++;
        run.ClearMaterial();
        TimeRecorder.Instance.AddTxtTextByFileInfo("testkaishi:");

    }

    public void ClearAll()
    {
        foreach (Transform Target in fow.randomNewTargets)
            Destroy(Target.gameObject);
        fow.randomNewTargets.Clear();
        fow.allTargets.Clear();
        fow.visibleTargets.Clear();
        fow.visibleTargets_new.Clear();
        fow.selectedTarget = null;
        fow.tobeSelectedTarget = null;
        // fow.finalTargets.Clear();
    }
}
