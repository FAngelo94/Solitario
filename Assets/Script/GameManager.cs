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
    private float _verticalSpaceBetweenCard = 0.1f;
    public float VerticalSpaceBetweenCard
    {
        get { return _verticalSpaceBetweenCard; }
    }

    // Use this for initialization
    void Start () {
        if (instance == null)
            instance = this;
        DontDestroyOnLoad(this);
     
	}
}
