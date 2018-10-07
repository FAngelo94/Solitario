using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    [SerializeField]
    private float _cardMovementSpeed = 50;
    public float CardMovementSpeed
    {
        get { return _cardMovementSpeed; }
    }

    [SerializeField]
    private float _verticalSpaceBetweenCard = 0.4f;
    public float VerticalSpaceBetweenCard
    {
        get { return _verticalSpaceBetweenCard; }
    }
    [SerializeField]
    private float _orizzontalSpaceBetweenCard = 0.2f;
    public float OrizzontalSpaceBetweenCard
    {
        get { return _orizzontalSpaceBetweenCard; }
    }

    [SerializeField]
    private float _cardMovementSpeedOnBack = 50;
    public float CardMovementSpeedOnBack
    {
        get { return _cardMovementSpeedOnBack; }
    }

    [SerializeField]
    private float _delayFromDoubleClick = 0.2f;
    public float DelayFromDoubleClick
    {
        get { return _delayFromDoubleClick; }
    }

    private bool _runningGame;
    public bool RunningGame
    {
        get { return _runningGame; }
    }

    
    public int Draw3Mode
    {
        get { return PlayerPrefs.GetInt("Draw3"); }
    }

    [SerializeField]
    private float _cardHelpMovementSpeed = 20;
    public float CardHelpMovementSpeed
    {
        get { return _cardHelpMovementSpeed; }
    }

    [SerializeField]
    private Text UITime;
    private int time;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        _runningGame = false;
        time = 0;
    }

    // Use this for initialization
    void Start () {

        IEnumerator courutine = Timer();
        StartCoroutine(courutine);
    }

    IEnumerator Timer()
    {
        while (true)
        {
            if (_runningGame)
            {
                time++;
                UITime.text = time.ToString();
            }
            yield return new WaitForSeconds(1);
        }
    }

    public void ChangeGameMode()
    {
        if (PlayerPrefs.GetInt("Draw3") == 0)
            PlayerPrefs.SetInt("Draw3", 1);
        else
            PlayerPrefs.SetInt("Draw3", 0);
    }

    public void StopGame()
    {
        _runningGame = false;
    }
    public void StartGame()
    {
        _runningGame = true;
    }
}
