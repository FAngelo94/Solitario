using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Use this for initialization
    void Start () {
        if (instance == null)
            instance = this;
        DontDestroyOnLoad(this);
     
	}
}
