using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : TouchManager {

    [SerializeField]
    [Header("Collection of all cards")]
    private GameObject _menu;

    // Use this for initialization
    void Start () {
        if (_menu != null)
            _menu.SetActive(false);
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
            GameManager.instance.StopGame();
            _menu.SetActive(true);
        }
    }

    public void NewGame()
    {
        GameManager.instance.StopGame();
        _menu.SetActive(false);
        Scene loadedLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(loadedLevel.buildIndex);
    }

    public void ResumeGame()
    {
        GameManager.instance.StartGame();
        _menu.SetActive(false);
    }
}
