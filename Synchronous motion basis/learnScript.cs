using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class learnScript : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        // player = GameObject.Find("Player");
        player.GetComponent<Run>().ssPattern.SetState(new MotionmatchingState());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
