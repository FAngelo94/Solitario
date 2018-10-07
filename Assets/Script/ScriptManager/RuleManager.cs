using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RuleManager : TouchManager {

    [SerializeField]
    public GameObject ConfirmDraw3Mode;

    [SerializeField]
    public GameObject ConfirmNormalMode;

    // Use this for initialization
    void Start () {
        ConfirmDraw3Mode.SetActive(false);
        ConfirmNormalMode.SetActive(false);
        if (GameManager.instance.Draw3Mode == 0)
        {
            transform.Find("Normal").gameObject.SetActive(false);
            transform.Find("Draw3").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("Draw3").gameObject.SetActive(false);
            transform.Find("Normal").gameObject.SetActive(true);
        }
    }
	
	// Update is called once per frame
	void Update () {
        CheckTouchOnObject();
    }

    protected override void SpritePressedBegan()
    {
        if (gameObject.GetComponent<Collider2D>() == Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position)) && GameManager.instance.RunningGame)
        {
            GameManager.instance.StopGame();
            if (GameManager.instance.Draw3Mode == 1)
                ConfirmNormalMode.SetActive(true);
            else
                ConfirmDraw3Mode.SetActive(true);

        }
    }

    public void Yes()
    {
        ConfirmDraw3Mode.SetActive(false);
        ConfirmNormalMode.SetActive(false);
        GameManager.instance.ChangeGameMode();
        Scene loadedLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(loadedLevel.buildIndex);
    }

    public void No()
    {
        ConfirmDraw3Mode.SetActive(false);
        ConfirmNormalMode.SetActive(false);
        GameManager.instance.StartGame();
    }
}
