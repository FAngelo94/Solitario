using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager instance;

    [SerializeField]
    public Text ScoreValue;
    [SerializeField]
    public Text MovesValue;


    public int Score
    {
        get;
        private set;
    }

    public int Moves
    {
        get;
        private set;
    }

    private void Awake()
    {
        Score = 0;
        Moves = 0;
    }

    private void Start()
    {
        if (instance == null)
            instance = this;
    }

    public void AddScore(int point)
    {
        Score += point;
        if (Score < 0)
            Score = 0;
        ScoreValue.text = Score.ToString();
    }

    public void IncrementMoves()
    {
        Moves++;
        MovesValue.text = Moves.ToString();
    }
}
