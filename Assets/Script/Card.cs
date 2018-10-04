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
    private List<Collider2D> externalColliders=new List<Collider2D>();

    public GameObject FatherCard { get; set; }//card below this
    public GameObject ChildCard { get; set; }//card above this

    private void Awake()
    {
        managerMovement = new MovementCard();
    }

    // Use this for initialization
    void Start () {
        //managerMovement = 
        selected = false;
        originalPosition = transform.position;
        myCollider = gameObject.GetComponent<Collider2D>();

        FatherCard = null;
        ChildCard = null;
    }
	
	// Update is called once per frame
	void Update () {
        CheckTouchOnObject();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("enter=" + collision);
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
            originalPosition = transform.position;
            selected = true;
        }
    }
    protected override void SpritePressedEnded()
    {
        if (selected)
        {
            if (externalColliders.Count==0)
                transform.position = originalPosition;
            else
            {
                Collider2D nearestCollider = null;
                float minDistance = float.MaxValue;
                foreach(Collider2D c in externalColliders)
                {
                    float tmpDist = Vector2.Distance(transform.position, c.transform.position);
                    if (tmpDist < minDistance)
                    {
                        minDistance = tmpDist;
                        nearestCollider = c;
                    }
                }

                GamePlay.instance.WhereIsCard(gameObject, nearestCollider);
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
            transform.position = new Vector3(transform.position.x, transform.position.y, -50);
            managerMovement.TranslateCard(gameObject, p);
        }
    }


    //Public Methods
    public void SetNewOriginalPosition(Vector3 newPoint)
    {
        originalPosition = newPoint;
        transform.position = originalPosition;
    }
    public void SetOldOriginalPosition()
    {
        transform.position = originalPosition;
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

    //Methods for move and rotate the cards from the other classes
    public void TraslateCard(Vector3 point)
    {
        managerMovement.TranslateCard(gameObject, point);
    }
    public void RotateCard()
    {
        managerMovement.RotateCard(gameObject);
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
