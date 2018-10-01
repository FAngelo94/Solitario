using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : TouchManager  {

    private string _seed;
    public string Seed
    {
        get{ return _seed; }
    }
    private int _value;

    private MovementCard managerMovement;
    private bool selected;
    private Vector2 originalPosition;
    private Collider2D myCollider;
    
	// Use this for initialization
	void Start () {
        managerMovement = new MovementCard();
        selected = false;
        originalPosition = transform.position;
        myCollider = gameObject.GetComponent<Collider2D>();
    }
	
	// Update is called once per frame
	void Update () {
        CheckTouchOnObject();
    }

    public void SetCard(int v,string s)
    {
        _seed = s;
        _value = v;
    }
    
    protected override void SpritePressedBegan()
    {
        Debug.Log("Began");
        if (myCollider == Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position)))
        {
            selected = true;
            originalPosition = transform.position;
        }
    }
    protected override void SpritePressedEnded()
    {
        Debug.Log("Ended");
        selected = false;
    }
    protected override void SpritePressedMoved()
    {
        if (selected)
        {
            Vector2 p = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            managerMovement.TranslateCard(gameObject, p);
        }
        
    }
    protected override void SpritePressedStationary()
    {
        if (selected)
        {
            Vector2 p = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            managerMovement.TranslateCard(gameObject, p);
        }
    }
}
