using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;


public class Run : MonoBehaviour
{
	public Queue<double>[] locations = new Queue<double>[3];	//自身坐标
	FieldOfView fow;	//目标检测器
	Random_obj random_Obj;
	private int seed;	//随机种子
	int num = 0;	//计时器
	public Transform handTransform;	//检测点
	public bool _isbegin = false;	//是否开始变色
	public bool _isRaybegin = true;	//是否根据射线检测
	public bool _play = false;
	public int gameCount = 0;// 记录游戏运行次数 
	public SelectStatePattern ssPattern;
	public Queue<double> targetLocations;//复制目标坐标用于计算相关系数
	public Queue<double>[] locationsCopy;//复制自身坐标用于计算相关系数
	public StartButton buttonController;//绑定脚本用于显示ui界面
	public LayerMask targetMask;

	public double max;
	double _max;
	
	void Start()
	{
		_max = max;
		GameObject.Find("OVRPlayerController").transform.localEulerAngles = new Vector3 (0.0f,0.0f,0.0f); 
		ClearConsole();
		fow = gameObject.GetComponent<FieldOfView>();
		random_Obj = GameObject.Find("GameEvent").GetComponent<Random_obj>();
		//存放自身坐标
		locations[0] = new Queue<double>();
		locations[1] = new Queue<double>();
		locations[2] = new Queue<double>();

		ssPattern =  new SelectStatePattern(fow, this);
		// ssPattern.SetState(new MotionmatchingState());	//记得要删
	}

	void Update()
	{
		if (Input.GetButtonDown("XRI_Right_TriggerButton"))
		{
			ssPattern.GetTriggerButton_down();
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			ssPattern.GetTriggerButton_down();
		}
		else if (Input.GetButtonUp("XRI_Right_TriggerButton"))
		{
			ssPattern.GetTriggerButton_up();
		}
		else if(Input.GetButtonUp("XRI_Right_PrimaryButton"))
		{
			ssPattern.GetPrimaryButton_up();
		}
		if( _isbegin && ssPattern.currentState.name == "MotionmatchingState")
		{
			RecordPosition();
			if(num%10 == 0)
				CalculateCorr();
			ChangeMaterial();
		}else if(_isRaybegin && ssPattern.currentState.name == "RayState" )
		{
			
			RaycastHit hit;
			GameObject rightHand = GameObject.Find("RightControllerAnchor");
        	Transform rightHandTransform = rightHand.transform;
       		// RaycastHit[] hits = Physics.RaycastAll(rightHandTransform.position,rightHandTransform.forward,40);
			// fow.selectedTarget = hits[0].transform;
			if (Physics.Raycast(rightHandTransform.position,rightHandTransform.forward, out hit, Mathf.Infinity,targetMask))
			{
				fow.selectedTarget = hit.transform;
				// Debug.Log(hit.transform.name);
			}else
			fow.selectedTarget = null;
			ClearMaterial();
		}
	}

	void ChangeMaterial()
	{
		Material materialBlue = new Material(Shader.Find("Universal Render Pipeline/Lit"));
		materialBlue.color = Color.blue;

		Material materialGreen = new Material(Shader.Find("Universal Render Pipeline/Lit"));
		materialGreen.color = Color.green;

		Material materialRed = new Material(Shader.Find("Universal Render Pipeline/Lit"));
		materialRed.color = Color.red;

		Material materialb = new Material(Shader.Find("Shader Graphs/1"));

		foreach (Transform Target in fow.allTargets)
		{
			if (!fow.visibleTargets_new.Contains(Target))
			{
				if(Target != null )
				{
					if(Target == fow.selectedTarget)
					{
						Target.GetComponent<Renderer>().material = materialRed;
					}else if(Target == fow.tobeSelectedTarget)
					{
						Target.GetComponent<Renderer>().material = materialGreen;
					}else
					{
						Target.GetComponent<Renderer>().material = materialBlue;
					}
				}
					
			}
		}

		UnityEngine.Vector4 mDirection = new UnityEngine.Vector4(0f, 0f, 0f, 0f);
		UnityEngine.Vector4 mSpeed = new UnityEngine.Vector4(0f, 0f, 0f, 0f);

		// 给三种方向随机排序避免重复出现
		float[] by = new float[6] { 0f, 1f, 0f, 1f, 0f, 1f};
		// int j, temp;
		// for (int x = 0; x < 10; x++)
		// {
		// 	j = UnityEngine.Random.Range(0, 6); //随机生成的位置在0~9之间
		// 	temp = by[j];
		// 	for (int i = j; i < 5; i++)//左移
		// 		by[i] = by[i + 1];
		// 	by[5] = temp;
		// }
		// int num = 0;

		for(int i =0;i < fow.visibleTargets_new.Count;i++)
		{	
			Transform visibleTarget = fow.visibleTargets_new[i];
			//	设定随机种子保证相邻的随机到不同结果
			//	同时避免视角摇晃带来的material重置
			seed = (int)(visibleTarget.position.x*100+visibleTarget.position.y*100+visibleTarget.position.z*100);
			UnityEngine.Random.InitState(seed);
			if (visibleTarget.GetComponent<Renderer>().material.shader != materialb.shader)
			{
				//	避免给已经开始的变化目标再次变化纹路
				int houkou_seed = UnityEngine.Random.Range(0, 6);
				float houkou = by[houkou_seed];
				// switch (by[num])
				switch (houkou)
				{
					case 0:
						mDirection = new UnityEngine.Vector4(1f, 0f, 0f, 0f);
						break;
					case 1:
						mDirection = new UnityEngine.Vector4(0f, 1f, 0f, 0f);
						break;
					case 2:
						mDirection = new UnityEngine.Vector4(0f, 0f, 1f, 0f);
						break;
				}
				materialb.SetVector("_Vector3", mDirection);

				float speed = UnityEngine.Random.Range(6, 16)/2;
				materialb.SetFloat("_Speed", speed);

				float theta = UnityEngine.Random.Range(0, 30)/5;
				materialb.SetFloat("_Theta", theta);

				// if(visibleTarget == fow.selectedTarget)
				// {
				// 	materialb.SetColor("_Color", Color.red);
				// }else if(visibleTarget == fow.tobeSelectedTarget)
				// {	
				// 	materialb.SetColor("_Color", Color.green);
				// }
				visibleTarget.GetComponent<Renderer>().material = materialb;
				// materialb = new Material(Shader.Find("Shader Graphs/1"));
			}
			if(visibleTarget == fow.selectedTarget)
			{
				Material materialbRed = visibleTarget.GetComponent<Renderer>().material;
				materialbRed.SetColor("_Color", Color.red);
				visibleTarget.GetComponent<Renderer>().material = materialbRed;
			}else if(visibleTarget == fow.tobeSelectedTarget)
			{	
				Material materialbGreen = visibleTarget.GetComponent<Renderer>().material;
				materialbGreen.SetColor("_Color", Color.green);
				visibleTarget.GetComponent<Renderer>().material = materialbGreen;
			}else{
				Material materialbBlue = visibleTarget.GetComponent<Renderer>().material;
				materialbBlue.SetColor("_Color", Color.blue);
				visibleTarget.GetComponent<Renderer>().material = materialbBlue;
			}
			materialb = new Material(Shader.Find("Shader Graphs/1"));
			// num = num + 1;
			// if (num > 2)
			// {
			// 	num = 0;
			// }
		}
	}
	void RecordPosition()
	{
		if(num%1 == 0)
		{
			locations[0].Enqueue(handTransform.position.x);
			locations[1].Enqueue(handTransform.position.y);
			locations[2].Enqueue(handTransform.position.z);
		}
		if(locations[0].Count > 100)
		{
			for(int i = 0;i<3;i++)
			{
				locations[i].Dequeue();
			}
		}
		num++;
	}
	
	public void ClearMaterial(){
		Material materialBlue = new Material(Shader.Find("Universal Render Pipeline/Lit"));
		materialBlue.color = Color.blue;
		Material materialGreen = new Material(Shader.Find("Universal Render Pipeline/Lit"));
		materialGreen.color = Color.green;
		Material materialRed = new Material(Shader.Find("Universal Render Pipeline/Lit"));
		materialRed.color = Color.red;

		foreach (Transform Target in fow.allTargets)
		{
			if(Target != null)
			{
				if(Target == fow.selectedTarget)
				{
					Target.GetComponent<Renderer>().material = materialRed;
				}else if(Target == fow.tobeSelectedTarget)
				{
					Target.GetComponent<Renderer>().material = materialGreen;
				}else
				{
					Target.GetComponent<Renderer>().material = materialBlue;
				}
			}

		}
	}

	public void CalculateCorr()
	{
		// if(num % 50 == 0)
		// {
			max = _max;
		// }
		double r = 0;
		if(locations[0].Count >= 50)
		{
			var count = locations[0].Count;
			locationsCopy = locations;
			double[] datax = new double[100];
			double[] datay = new double[100];
			double[] dataz = new double[100];
			for (var i = 0; i < count; i++)
			{
				datax = locations[0].ToArray();
				datay = locations[1].ToArray();
				dataz = locations[2].ToArray();
			}

			foreach (Transform Target in fow.visibleTargets_new)
			{
				RecordPosition targetData = Target.GetComponent<RecordPosition>();
				// targetLocations = targetData.locations;
				// for (var i = 0; i < count; i++)
				// {
				// 	// if(targetData.locations.Count > 0)
				// 		// datab[i] = targetData.locations.Dequeue();
				// 	if(targetLocations.Count > 0)
				// 		datab[i] = targetLocations.Dequeue();
				// }
				var datab = targetData.locations.ToArray();
				if(targetData.dir.x == 1)
				{
					r = Pearson(datax,datab);
				}
				else if(targetData.dir.y == 1)
				{
					r = Pearson(datay,datab);
				}
				else if(targetData.dir.z == 1)
				{
					r = Pearson(dataz,datab);
				}
				if(r > max)
				{
					// fow.finalTargets.Clear();
					// fow.finalTargets.Add(Target);
					fow.selectedTarget = Target;
					max = r;
				}
			}
		}
	}

	public static double Pearson(IEnumerable<double> dataA, IEnumerable<double> dataB)
	{
		int n = 0;
		double r = 0.0;

		double meanA = 0;
		double meanB = 0;
		double varA = 0;
		double varB = 0;

		using (IEnumerator<double> ieA = dataA.GetEnumerator())
		using (IEnumerator<double> ieB = dataB.GetEnumerator())
		{
			while (ieA.MoveNext())
			{
				if(ieB.MoveNext())
				{
					double currentA = ieA.Current;
					double currentB = ieB.Current;

					double deltaA = currentA - meanA;
					double scaleDeltaA = deltaA/++n;

					double deltaB = currentB - meanB;
					double scaleDeltaB = deltaB/n;

					meanA += scaleDeltaA;
					meanB += scaleDeltaB;

					varA += scaleDeltaA*deltaA*(n - 1);
					varB += scaleDeltaB*deltaB*(n - 1);
					r += (deltaA*deltaB*(n - 1))/n;
				}


			}
		}

		return r/Math.Sqrt(varA*varB);
	}

	//清空控制台
	public static void ClearConsole()
	{
		Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
		System.Type type = assembly.GetType("UnityEditor.LogEntries");
		MethodInfo method = type.GetMethod("Clear");
		method.Invoke(new object(), null);
	}
}
