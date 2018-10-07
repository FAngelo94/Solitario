using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackManager : TouchManager {

    public static BackManager instance;

    private List<MoveSaved> movesSaved;

    private Collider2D myCollider;

    private List<GameObject> cardsInMovement;
    private List<Vector3> finalPositions;

    private void Awake()
    {
        movesSaved = new List<MoveSaved>();
        cardsInMovement = new List<GameObject>();
        finalPositions = new List<Vector3>();
    }

    private void Start()
    {
        if (instance == null)
            instance = this;
        myCollider = gameObject.GetComponent<Collider2D>();
    }

    private void Update()
    {
        CheckTouchOnObject();
        if(cardsInMovement.Count>0)
        {
            for(int i=0;i<cardsInMovement.Count;i++)
            {
                if(cardsInMovement[i].transform.position.Equals(finalPositions[i]))
                {//card "i" reaches its final position
                    cardsInMovement[i].GetComponent<Card>().SetNewOriginalPosition(finalPositions[i]);
                    finalPositions[i] = new Vector3(-100, -100, -100);
                    cardsInMovement[i] = null;
                }
                else
                {//card "i" doesn't reach its final position
                    cardsInMovement[i].GetComponent<Card>().TraslateCard(finalPositions[i]);
                }
            }
            //remove null gameobject and the finalposition associated
            cardsInMovement.RemoveAll(x => x == null);
            finalPositions.RemoveAll(x => x == new Vector3(-100, -100, -100));

        }
    }

    protected override void SpritePressedBegan()
    { 
        if (myCollider == Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position)))
        {
            BackMove();
        }
    }

    public void SaveMove(GameObject card, Card cardAfter, Card cardBefore, int points)
    {
        MoveSaved save = new MoveSaved
        {
            card = card,
            cardBefore = cardBefore,
            cardAfter = cardAfter,
            points = points
        };
        movesSaved.Add(save);
    }

    private class MoveSaved
    {
        public GameObject card;
        public Card cardBefore;
        public Card cardAfter;
        public int points;
    }


     private  void BackMove()
    {
        if (movesSaved.Count > 0)
        {
            Debug.Log("move count=" + movesSaved.Count);
            MoveSaved lastMove = movesSaved[movesSaved.Count - 1];
            movesSaved.Remove(lastMove);
            IncrememntMovements();
            BackPoints(lastMove.points);
            if (lastMove.cardAfter.PositionGoal != null)//Card back from PositionGoal
            {
                if (lastMove.cardBefore.Column != null)//card go to Column
                {
                    FromPositionGoalToColumn(lastMove);
                }
                if (lastMove.cardBefore.Deck != null)//card go to Deck
                {
                    FromPositionGoalToDeck(lastMove);
                }
            }
            if (lastMove.cardAfter.Column != null)//Card back from Column
            {
                if (lastMove.cardBefore.PositionGoal != null)//card go to PositionGoal
                {
                    FromColumnToPositionGoal(lastMove);
                }
                if (lastMove.cardBefore.Column != null)//card go to Column
                {
                    FromColumnToColumn(lastMove);
                }
                if (lastMove.cardBefore.Deck != null)//card go to Deck
                {
                    FromColumnToDeck(lastMove);
                }
            }

            if (lastMove.cardAfter.Deck != null)//Card back from Deck
            {
                if (lastMove.cardBefore.Deck != null)//card go to Deck
                {
                    if (GameManager.instance.Draw3Mode == 0)
                        FromDeckToDeck(lastMove);
                    else
                    {
                        int count = 0;
                        while (count < 3 && lastMove.cardAfter.Deck != null && lastMove.cardBefore.Deck != null)
                        {
                            FromDeckToDeck(lastMove);
                            count++;
                            if (movesSaved.Count > 0 && count < 3)
                            {
                                lastMove = movesSaved[movesSaved.Count - 1];
                                if (lastMove.cardAfter.Deck != null && lastMove.cardBefore.Deck != null)
                                    movesSaved.Remove(lastMove);
                            }
                            else
                                break;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Card returns from Column to PositionGoal
    /// </summary>
    private void FromColumnToPositionGoal(MoveSaved lastMove)
    {
        Debug.Log("FromColumnToPositionGoal");
        //Remove the card from the Column and put it to the PositionGoal
        lastMove.cardAfter.Column.RemoveCard(lastMove.card);
        lastMove.cardBefore.PositionGoal.AddCard(lastMove.card);
        //remove the card from the actually father (if there)
        if (lastMove.card.GetComponent<Card>().FatherCard != null)
        {
            lastMove.card.GetComponent<Card>().FatherCard.GetComponent<Card>().ChildCard = null;
            lastMove.card.GetComponent<Card>().FatherCard = null;
        }
        //Find the finalPosition and active the transaction
        Vector3 finalPoint = lastMove.cardBefore.PositionGoal.transform.position;//get the position of PositionGoal gameobject
        finalPoint.z = lastMove.card.transform.position.z;
        if (lastMove.cardAfter.FatherCard != null)
        {//Before the card was above an other card
            lastMove.cardAfter.FatherCard.GetComponent<Card>().ChildCard = null;
        }
        else
        {
            //If the column is empty the collider column is activeted, otherwise
            //nothing happen
            lastMove.cardAfter.Column.RotateLastCard();
        }
        //Card and final position to the List in order to activate the transaction of card
        finalPositions.Add(finalPoint);
        cardsInMovement.Add(lastMove.card);        
    }

    /// <summary>
    /// Card returns from Column and go to a Column
    /// </summary>
    private void FromColumnToColumn(MoveSaved lastMove)
    {
        Debug.Log("FromColumnToColumn");
        //Remove the card from the new Column and put it in the old Column
        lastMove.cardAfter.Column.RemoveCard(lastMove.card);
        lastMove.cardBefore.Column.AddSingleCard(lastMove.card);
        //remove the card from the actually father (if there)
        if (lastMove.card.GetComponent<Card>().FatherCard != null)
        {
            lastMove.card.GetComponent<Card>().FatherCard.GetComponent<Card>().ChildCard = null;
            lastMove.card.GetComponent<Card>().FatherCard = null;
        }
        //Find the finalPosition and active the transaction
        Vector3 finalPoint = new Vector3();
        if (lastMove.cardBefore.FatherCard != null)
        {//Before the card was above an other card
            Debug.Log("IF");
            lastMove.card.GetComponent<Card>().FatherCard = lastMove.cardBefore.FatherCard;//set the old father
            lastMove.card.GetComponent<Card>().FatherCard.GetComponent<Card>().ChildCard = lastMove.card;//set the card to the old father
            finalPoint = lastMove.cardBefore.FatherCard.transform.position;
            finalPoint.y = lastMove.cardBefore.FatherCard.transform.position.y - GameManager.instance.VerticalSpaceBetweenCard;
            finalPoint.z = lastMove.card.transform.position.z;
        }
        else
        {
            Debug.Log("ELSE");
            //Check if in the column there was a hide card
            if (lastMove.cardBefore.Column.NumberOfCard() > 1)
            {

                lastMove.cardBefore.Column.HideLastCard();
                BackPoints(5);
            }
            finalPoint = lastMove.cardBefore.Column.transform.position;
            finalPoint.y = lastMove.cardBefore.Column.transform.position.y - GameManager.instance.VerticalSpaceBetweenCard * (lastMove.cardBefore.Column.NumberOfCard() - 1);
            finalPoint.z = lastMove.card.transform.position.z;
        }
        //Add card and final position to the List in order to activate the transaction of card
        finalPositions.Add(finalPoint);
        cardsInMovement.Add(lastMove.card);        
        //fix children of card
        if (lastMove.card.GetComponent<Card>().ChildCard!=null)
            FixedChildCards(lastMove.card,lastMove.card.GetComponent<Card>().ChildCard,finalPoint);
    }

    private void FixedChildCards(GameObject father, GameObject child,Vector3 finalPointFather)
    {
        Card scriptFather = father.GetComponent<Card>();
        Card scriptChild = child.GetComponent<Card>();
        scriptChild.Column.RemoveCardChild(child);
        scriptFather.Column.AddSingleCard(child);
        //Calculate the position of child respect to the father position
        Vector3 finalPoint = finalPointFather;
        finalPoint.z -= 1;
        finalPoint.y -= GameManager.instance.VerticalSpaceBetweenCard;
        //Add card and final position to the List in order to activate the transaction of card
        finalPositions.Add(finalPoint);
        cardsInMovement.Add(child);
        //Check if child has child
        if (scriptChild.ChildCard != null)
            FixedChildCards(child, scriptChild.ChildCard, finalPoint);
    }

    /// <summary>
    /// Card returns from PositionGoal and go to a Column
    /// </summary>
    private void FromPositionGoalToColumn(MoveSaved lastMove)
    {
        Debug.Log("FromPositionGoalToColumn");
        //Remove the card from the PositionGoal and put it in the Column
        lastMove.cardAfter.PositionGoal.RemoveCard();
        lastMove.cardBefore.Column.AddSingleCard(lastMove.card);
        //Find the finalPosition and active the transaction
        Vector3 finalPoint = new Vector3();
        if(lastMove.cardBefore.FatherCard!=null)
        {//Before the card was above an other card
            lastMove.card.GetComponent<Card>().FatherCard = lastMove.cardBefore.FatherCard;
            lastMove.card.GetComponent<Card>().FatherCard.GetComponent<Card>().ChildCard = lastMove.card;
            finalPoint = lastMove.cardBefore.FatherCard.transform.position;
            finalPoint.y = lastMove.cardBefore.FatherCard.transform.position.y - GameManager.instance.VerticalSpaceBetweenCard;
            finalPoint.z = lastMove.card.transform.position.z;
        }
        else
        {
            //Check if in the column there was a hide card
            if (lastMove.cardBefore.Column.NumberOfCard() > 1)
            {
                lastMove.cardBefore.Column.HideLastCard();
                BackPoints(5);
            }
            finalPoint = lastMove.cardBefore.Column.transform.position;
            finalPoint.y = lastMove.cardBefore.Column.transform.position.y - GameManager.instance.VerticalSpaceBetweenCard * (lastMove.cardBefore.Column.NumberOfCard()-1);
            finalPoint.z = lastMove.card.transform.position.z;
        }
        //Card and final position to the List in order to activate the transaction of card
        finalPositions.Add(finalPoint);
        cardsInMovement.Add(lastMove.card);        
    }

    /// <summary>
    /// Card returns from Deck and go to the Deck
    /// </summary>
    private void FromDeckToDeck(MoveSaved lastMove)
    {
        Debug.Log("FromDeckToDeck");
        bool undoResetDeck = lastMove.cardAfter.Deck.UndoTake();
        if(undoResetDeck)
        {
            SaveMove(lastMove.card, lastMove.cardAfter, lastMove.cardBefore, lastMove.points);
        }
    }

    /// <summary>
    /// Card returns from Column and go to the Deck
    /// </summary>
    private void FromColumnToDeck(MoveSaved lastMove)
    {
        Debug.Log("FromColumnToDeck");
        //Remove the card from the new Column and put it in the old Column
        lastMove.cardAfter.Column.RemoveCard(lastMove.card);
        lastMove.cardBefore.Deck.AddToWaste(lastMove.card);
        //remove the card from the actually father (if there)
        if (lastMove.card.GetComponent<Card>().FatherCard != null)
        {
            lastMove.card.GetComponent<Card>().FatherCard.GetComponent<Card>().ChildCard = null;
            lastMove.card.GetComponent<Card>().FatherCard = null;
        }
    }

    /// <summary>
    /// Card returns from PositionGoal and go to the Deck
    /// </summary>
    private void FromPositionGoalToDeck(MoveSaved lastMove)
    {
        Debug.Log("FromColumnToDeck");
        //Remove the card from the new Column and put it in the old Column
        lastMove.cardAfter.PositionGoal.RemoveCard();
        lastMove.cardBefore.Deck.AddToWaste(lastMove.card);
    }


    private void BackPoints(int points)
    {
        ScoreManager.instance.AddScore(-points);
    }
    private void IncrememntMovements()
    {
        ScoreManager.instance.IncrementMoves();
    }
}
