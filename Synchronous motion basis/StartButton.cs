using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StartButton : MonoBehaviour
{
    public GameObject shitenn;
    public GameObject UICanvas;
    public GameObject startButton;
    public GameObject distanceButton10;
    public GameObject distanceButton15;
    public GameObject distanceButton20;
    public GameObject selectStateButton1;
    public GameObject selectStateButton2;
    public GameObject testButton;


    public GameObject player;
    public GameObject playerController;
    public SelectState mySelectState;
    GameObject ray;

    // Start is called before the first frame update
    void Start()
    {
        // mySelectState = new NullState();
        mySelectState = new MotionmatchingState();
        startButton.GetComponent<Button>().onClick.AddListener(onButtonClick);
        distanceButton10.GetComponent<Button>().onClick.AddListener(changeDistance10);
        distanceButton15.GetComponent<Button>().onClick.AddListener(changeDistance15);
        distanceButton20.GetComponent<Button>().onClick.AddListener(changeDistance20);
        selectStateButton1.GetComponent<Button>().onClick.AddListener(changeSelectState1);
        selectStateButton2.GetComponent<Button>().onClick.AddListener(changeSelectState2);
        
        testButton.GetComponent<Button>().onClick.AddListener(setSelectState);
        testButton.GetComponent<Button>().onClick.AddListener(GameObject.Find("GameEvent").GetComponent<Random_obj>().changeTargets);
        testButton.GetComponent<Button>().onClick.AddListener(hideButton);
        

        startButton.GetComponent<Button>().onClick.AddListener(hideButton);
        startButton.GetComponent<Button>().onClick.AddListener(GameObject.Find("GameEvent").GetComponent<Random_obj>().StartGame);
        player = GameObject.Find("Player");
        // GameObject.Find("LaserPointer").SetActive(false);
        ray = GameObject.Find("LaserPointer");

        distanceButton10.GetComponent<Image>().color = Color.yellow;
        selectStateButton1.GetComponent<Image>().color = Color.yellow;
    }

    public void onButtonClick()
    {
        player.GetComponent<Run>().ssPattern.SetState(mySelectState);
        // player.GetComponent<Run>().ssPattern.SetState(new MotionmatchingState());
        TimeRecorder.Instance.AddTxtTextByFileInfo("GameStart:");
    }

    public void hideButton()
    {
        UICanvas.SetActive(false);
    }

    public void showButton()
    {
        UICanvas.SetActive(true);
    }

    public void setSelectState()
    {
        player.GetComponent<Run>().ssPattern.SetState(mySelectState);
    }

    public void changeDistance10()
    {
        playerController.transform.position = new Vector3(0,5f,-10f);
        distanceButton10.GetComponent<Image>().color = Color.yellow;
        distanceButton15.GetComponent<Image>().color = Color.white;
        distanceButton20.GetComponent<Image>().color = Color.white;
    }

    public void changeDistance15()
    {
        playerController.transform.position = new Vector3(0,5f,-15f);
        distanceButton15.GetComponent<Image>().color = Color.yellow;
        distanceButton10.GetComponent<Image>().color = Color.white;
        distanceButton20.GetComponent<Image>().color = Color.white;
    }

    public void changeDistance20()
    {
        playerController.transform.position = new Vector3(0,5f,-20f);
        distanceButton20.GetComponent<Image>().color = Color.yellow;
        distanceButton10.GetComponent<Image>().color = Color.white;
        distanceButton15.GetComponent<Image>().color = Color.white;
    }

    public void changeSelectState1()
    {
        mySelectState = new MotionmatchingState();
        // ray.SetActive(false);
        shitenn.SetActive(true);
        selectStateButton1.GetComponent<Image>().color = Color.yellow;
        selectStateButton2.GetComponent<Image>().color = Color.white;
    }

    public void changeSelectState2()
    {
        mySelectState = new RayState();
        // ray.SetActive(true);
        shitenn.SetActive(false);
        selectStateButton2.GetComponent<Image>().color = Color.yellow;
        selectStateButton1.GetComponent<Image>().color = Color.white;
    }

}
