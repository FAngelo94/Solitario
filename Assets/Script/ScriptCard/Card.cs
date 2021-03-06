﻿using System;
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
    public int Value
    {
        get { return _value; }
    }

    private Column _column;
    public Column Column
    {
        get { return _column; }
    }

    private Deck _deck;
    public Deck Deck
    {
        get { return _deck; }
    }

    private PositionGoalCard _positionGoal;
    public PositionGoalCard PositionGoal
    {
        get { return _positionGoal; }
    }

    private MovementCard managerMovement;
    private bool selected;
    private Vector3 originalPosition;
    private Collider2D myCollider;
    private List<Collider2D> externalColliders;

    public GameObject FatherCard { get; set; }//card below this
    public GameObject ChildCard { get; set; }//card above this

    private bool checkFollowCardDelay;

    private float checkDoubleClick;

    private void Awake()
    {
        managerMovement = new MovementCard();
        externalColliders = new List<Collider2D>();
    }

    // Use this for initialization
    void Start () {
        //managerMovement = 
        selected = false;
        originalPosition = transform.position;
        myCollider = gameObject.GetComponent<Collider2D>();

        FatherCard = null;
        ChildCard = null;
        checkFollowCardDelay = true;
    }
	
	// Update is called once per frame
	void Update () {
        CheckTouchOnObject();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        externalColliders.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        externalColliders.Remove(collision);
    }

    protected override void SpritePressedBegan()
    {

        if (myCollider == Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position)))
        {
            if (Time.time - checkDoubleClick <= GameManager.instance.DelayFromDoubleClick)
            {
                GamePlay.instance.DoubleTapManage(gameObject);
            }
            else
            {
                checkDoubleClick = Time.time;
                originalPosition = transform.position;
                selected = true;
            }
        }
    }
    protected override void SpritePressedEnded()
    {
        if (selected)
        {
            if (externalColliders.Count == 0)
                SetOldOriginalPosition();
            else
            {
                Collider2D nearestCollider = null;
                float minDistance = float.MaxValue;
                foreach (Collider2D c in externalColliders)
                {
                    float tmpDist = Vector2.Distance(transform.position, c.transform.position);
                    if (tmpDist < minDistance && !CheckChild(c))
                    {
                        minDistance = tmpDist;
                        nearestCollider = c;
                    }
                }
                if (nearestCollider != null)
                    GamePlay.instance.WhereIsCard(gameObject, nearestCollider);
                else
                    SetOldOriginalPosition();
            }
        }
        selected = false;
    }

    protected override void SpritePressedMoved()
    {
        FollowFinger();
    }
    protected override void SpritePressedStationary()
    {
        FollowFinger();
    }

    private void FollowFinger()
    {
        if (selected)
        {
            Vector3 p = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            p.z = -50;
            transform.position = new Vector3(transform.position.x, transform.position.y, -50);//Set the z immediatly
            managerMovement.TranslateCard(gameObject, p);
            if (ChildCard != null)
            {
                ChildCard.GetComponent<Card>().FollowCard(-51);
            }
        }
    }

    /// <summary>
    /// Check if this card is collided with its child or with
    /// a child of its child
    /// </summary>
    /// <param name="c">Collision considered</param>
    /// <returns>true if this card is collided with its child or with a 
    /// chils of its child</returns>
    private bool CheckChild(Collider2D c)
    {
        Card script = c.gameObject.GetComponent<Card>();
        if(script!=null)
        {
            
            if (c.gameObject.Equals(ChildCard))//This card collide with its child
                return true;
            while (script.ChildCard != null)
            {
                if (c.gameObject.Equals(script.ChildCard))
                    return true;
                script=script.ChildCard.GetComponent<Card>();
            }
        }
        return false;
    }

    //Public Methods
    public void FollowCard(float z)
    {
        if (checkFollowCardDelay)
        {
            checkFollowCardDelay = false;
            IEnumerator courutine = FollowCardDelay(z);
            StartCoroutine(courutine);
        }
    }
    IEnumerator FollowCardDelay(float z)
    {
        Vector3 p = FatherCard.transform.position;
        p.y = FatherCard.transform.position.y - GameManager.instance.VerticalSpaceBetweenCard;
        p.z = z;
        transform.position = new Vector3(transform.position.x, transform.position.y, z);//Set the z immediatly
        managerMovement.TranslateCard(gameObject, p);
        yield return new WaitForSeconds(0.005f);
        if (ChildCard != null)
        {
            ChildCard.GetComponent<Card>().FollowCard(z - 1);
        }
        checkFollowCardDelay = true;
    }

    public void SetNewOriginalPosition(Vector3 newPoint)
    {
        originalPosition = newPoint;
        transform.position = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z);
    }
    public void SetOldOriginalPosition()
    {
        transform.position = new Vector3(originalPosition.x,originalPosition.y,originalPosition.z);
        IEnumerator setupChildren = SetOldOriginalPositionDelay();
        StartCoroutine(setupChildren);
    }
    /// <summary>
    /// I need to wait before to set the original position of child because maybe they are moving
    /// in order to follow the father
    /// </summary>
    /// <returns></returns>
    IEnumerator SetOldOriginalPositionDelay()
    {
        yield return new WaitForSeconds(0.01f);
        if (ChildCard != null)
            ChildCard.GetComponent<Card>().SetOldOriginalPosition();
    }

    public void SetCard(int v, string s)
    {
        _seed = s;
        _value = v;
    }

    public void SetColumn(Column column)
    {
        _deck = null;
        _positionGoal = null;
        _column = column;
    }
    public void SetDeck(Deck deck)
    {
        _positionGoal = null;
        _column = null;
        FatherCard = null;
        ChildCard = null;
        _deck = deck;
    }

    public void SetPositionGoal(PositionGoalCard goal)
    {
        _column = null;
        _deck = null;
        FatherCard = null;
        ChildCard = null;
        _positionGoal = goal;
    }

    public Card CopyCardClass()
    {
        Card copy = new Card
        {
            _column = _column,
            _deck = _deck,
            _positionGoal = _positionGoal,
            _seed = _seed,
            _value = _value,
            originalPosition = originalPosition,
            FatherCard = FatherCard,
            ChildCard = ChildCard
        };
        return copy;
    }

    //Methods for move and rotate the cards from the other classes
    public void TraslateCard(Vector3 point)
    {
        managerMovement.TranslateCard(gameObject, point);
    }
    public int GetRotateCard()
    {
        return gameObject.GetComponent<Animator>().GetInteger("Rotation");
    }
    public void RotateFrontCard()
    {
        managerMovement.RotateFrontCard(gameObject);
    }
    public void RotateBackCard()
    {
        managerMovement.RotateBackCard(gameObject);
    }
}
