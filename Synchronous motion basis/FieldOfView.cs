using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour {
	//用于检测视野内物体以及计算视野角度

	public float viewRadius;
	[Range(0,360)]
	public float viewAngle;
	public float viewAngle_new;

	public LayerMask targetMask;
	public LayerMask obstacleMask;
	//[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();
	public List<Transform> visibleTargets_new = new List<Transform>();
	public List<Transform> allTargets = new List<Transform>();
	// public List<Transform> finalTargets = new List<Transform>();
	public List<Transform> randomNewTargets = new List<Transform>();
	public Transform tobeSelectedTarget ;
	public Transform selectedTarget ;
	public const float E = 2.7182f;

	void Start() {
		StartCoroutine ("FindTargetsWithDelay", .2f);
	}
	IEnumerator FindTargetsWithDelay(float delay) {
		while (true) {
			yield return new WaitForSeconds (delay);
			FindVisibleTargets (visibleTargets,viewAngle);
		// if(visibleTargets.Count>5)
		// {
		// 	viewAngle_new = viewAngle * 5 / visibleTargets.Count;
		// }
		// else
		// 	viewAngle_new = viewAngle;
			viewAngle_new = viewAngle-14/(1+Mathf.Pow(E,-0.4f*visibleTargets.Count+4.5f));
		
			FindVisibleTargets (visibleTargets_new,viewAngle_new);
		}
	}

	void FindVisibleTargets(List<Transform> myvisibleTargets,float myviewAngle) {
		myvisibleTargets.Clear ();
		Collider[] targetsInViewRadius = Physics.OverlapSphere (transform.position, viewRadius, targetMask);
		for (int i = 0; i < targetsInViewRadius.Length; i++) {
			Transform target = targetsInViewRadius [i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			if (Vector3.Angle (transform.forward, dirToTarget) < myviewAngle / 2) {
				float dstToTarget = Vector3.Distance (transform.position, target.position);
				// visibleTargets.Add(target);
				if (!Physics.Raycast (transform.position, dirToTarget, dstToTarget, obstacleMask)) {
					myvisibleTargets.Add (target);
					if (!allTargets.Contains(target)){
						allTargets.Add(target);
					}
				}
			}
		}
	}


	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
		if (!angleIsGlobal) {
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}
}

