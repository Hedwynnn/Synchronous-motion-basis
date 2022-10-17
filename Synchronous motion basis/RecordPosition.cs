using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordPosition : MonoBehaviour
{
    //添加给每个物体记录移动的纹理的坐标
    public Queue<double> locations = new Queue<double>();
    // int num = 0;
    public float speed;//shader速度
    public float theta;//shader位相
    [System.NonSerialized]  public Vector4 dir;//shader方向
    public double asda;
    public Color mColor;

    void Start()
    {
        mColor = gameObject.GetComponent<Renderer>().material.color;
    }
    void Update()
    {
        Material materialb = new Material(Shader.Find("Shader Graphs/1"));
        if(transform.GetComponent<Renderer>().material.shader == materialb.shader)
        {
            speed = transform.GetComponent<Renderer>().material.GetFloat("_Speed");
            theta = transform.GetComponent<Renderer>().material.GetFloat("_Theta");
            dir = transform.GetComponent<Renderer>().material.GetVector("_Vector3");
            // if(num%1 == 0)
            // {
            locations.Enqueue(Mathf.Sin(speed*Time.time+theta));
            asda = locations.Count;
            if(locations.Count > 100)
            {
                locations.Dequeue();
            }
            // }
        }
        // num++;
    }
}
