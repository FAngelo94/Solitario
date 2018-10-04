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

    private CardColumn _cardColumn;
    public CardColumn CardColumn
    {
        get { return _cardColumn; }
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
    private Collider2D externalCollider;

    // Use this for initialization
    void Start () {
        managerMovement = new MovementCard();
        selected = false;
        originalPosition = transform.position;
        myCollider = gameObject.GetComponent<Collider2D>();
        externalCollider = null;
        _cardColumn = null;
        _deck = null;
    }
	
	// Update is called once per frame
	void Update () {
        CheckTouchOnObject();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Card Trigger Enter");
        externalCollider = collision;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Card Trigger Exit");
        externalCollider = null;
    }

    protected override void SpritePressedBegan()
    {
        if (myCollider == Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position)))
        {
            originalPosition = transform.position;
            selected = true;
        }
    }
    protected override void SpritePressedEnded()
    {
        if (selected)
        {
            if(externalCollider == null)
                transform.position = originalPosition;
            else
                AnalyzeCollision();
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
            transform.position = new Vector3(transform.position.x, transform.position.y, -50);
            managerMovement.TranslateCard(gameObject, p);
        }
    }

    /// <summary>
    /// In this method I analyze where is the card when user
    /// release it if the card enter in contanct with something
    /// </summary>
    private void AnalyzeCollision()
    {
        string tagCollision = externalCollider.gameObject.tag;
        if(tagCollision.Equals("Card"))
        {
            Debug.Log(tagCollision);
        }
        if (tagCollision.Equals("PositionGoal"))
        {
            Debug.Log(tagCollision);
        }
        if (tagCollision.Equals("CardColumn"))
        {
            Debug.Log(tagCollision);
        }
    }

    //Public Methods
    public void SetNewOriginalPosition(Vector3 newPoint)
    {
        originalPosition = newPoint;
    }

    public void SetCard(int v, string s)
    {
        _seed = s;
        _value = v;
    }

    public void SetColumn(CardColumn column)
    {
        _deck = null;
        _positionGoal = null;
        _cardColumn = column;
    }
    public void SetDeck(Deck deck)
    {
        _positionGoal = null;
        _cardColumn = null;
        _deck = deck;
    }
    public void SetPositionGoal(PositionGoalCard goal)
    {
        if (CardColumn != null)
        {
            _cardColumn.RemoveCard(gameObject);
            _cardColumn = null;
        }
        _deck = null;
        _positionGoal = goal;
    }

    //Methods for move and rotate the cards from the other classes
    public void TraslateCard(Vector3 point)
    {
        managerMovement.TranslateCard(gameObject, point);
    }
    public void RotateCard()
    {
        managerMovement.RotateCard(gameObject);
    }
}
