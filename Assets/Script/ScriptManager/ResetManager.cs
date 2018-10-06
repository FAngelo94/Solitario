using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetManager : TouchManager {



	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        CheckTouchOnObject();
	}

    protected override void SpritePressedBegan()
    {
        Debug.Log(gameObject.name + " -> SpritePressedBegan");
        if (gameObject.GetComponent<Collider2D>() == Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position)) && GameManager.instance.RunningGame)
        {
            Debug.Log(gameObject.name + " -> SpritePressedBegan IF");
            GameManager.instance.FinishGame();
            Scene loadedLevel = SceneManager.GetActiveScene();
            SceneManager.LoadScene(loadedLevel.buildIndex);
        }
    }
}
