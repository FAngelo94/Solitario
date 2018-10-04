using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    [SerializeField]
    private float _cardMovementSpeed = 2;
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

    // Use this for initialization
    void Start () {
        if (instance == null)
            instance = this;
        DontDestroyOnLoad(this);
     
	}
}
