using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// In this class I develop the gameplay of the game, it's mean here
/// I analyze where is the card when user leave it in order to modify the position of 
/// that card in the game
/// </summary>
public class GamePlay:MonoBehaviour {

    public static GamePlay instance;

    /// <summary>
    /// Card the player want to move
    /// </summary>
    private GameObject card;

    /// <summary>
    /// Gameobject that are in collision with card 
    /// when user leave it.
    /// It can be an other card, a column or a positionGoal object
    /// </summary>
    private GameObject newPosition;

    private void Start()
    {
        if (instance == null)
            instance = this;
    }


    public void WhereIsCard(GameObject card, Collider2D collision)
    {
        bool newPositionIsValid = false;

        this.card = card;
        newPosition = collision.gameObject;
        string tagCollision = newPosition.tag;

        if (card.GetComponent<Card>().Column != null)//User take a card from column
        {
            if (tagCollision.Equals("PositionGoal"))//put the card in a empty positionGoal
            {
                FromColumnToPositionGoal();
                newPositionIsValid = true;
            }
            if (tagCollision.Equals("Column"))//put the card in a empty column
            {
                FromColumnToColumn();
                newPositionIsValid = true;
            }
            if (tagCollision.Equals("Card"))
            {
                if (newPosition.GetComponent<Card>().PositionGoal != null)//put the card in a positionGoal with card
                {
                    FromColumnToPositionGoalCard();
                    newPositionIsValid = true;
                }
                if (newPosition.GetComponent<Card>().Column!=null)//put the card in a column with card
                {
                    FromColumnToColumnCard();
                    newPositionIsValid = true;
                }
            }
        }
        if (card.GetComponent<Card>().PositionGoal != null)//User take a card from positionGoal
        {
            if (tagCollision.Equals("Column"))//put the card in a empty column
            {
                FromPositionGoalToColumn();
                newPositionIsValid = true;
            }
            if (tagCollision.Equals("Card"))
            {
                if (newPosition.GetComponent<Card>().Column != null)//put the card in a column with card
                {
                    FromPositionGoalToColumnCard();
                    newPositionIsValid = true;
                }
            }
        }
        if (card.GetComponent<Card>().Deck != null)//User take a card from the waste of deck
        {
            if (tagCollision.Equals("Card"))
            {
                if (newPosition.GetComponent<Card>().PositionGoal != null)
                {
                    FromDeckToPositionGoalCard();//put the card in a positionGoal with card
                    newPositionIsValid = true;
                }
                if (newPosition.GetComponent<Card>().Column != null)//put the card in a column with card
                {
                    FromDeckToColumnCard();
                    newPositionIsValid = true;
                }
            }
            if (tagCollision.Equals("PositionGoal"))//put the card in a empty positionGoal
            {
                FromDeckToPositionGoal();
                newPositionIsValid = true;
            }
            if (tagCollision.Equals("Column"))//put the card in a empty column
            {
                FromDeckToColumn();
                newPositionIsValid = true;
            }
        }

        if (!newPositionIsValid)
        {//User put the card in a not valid position, so the card will
            //be put in its original position
            card.GetComponent<Card>().SetOldOriginalPosition();
        }
    }

    /// <summary>
    /// User takes a card from Column and put it in the empty PositionGoal
    /// </summary>
    private void FromColumnToPositionGoal()
    {
        Debug.Log("FromColumnToPositionGoal");
        Card scriptCard = card.GetComponent<Card>();
        PositionGoalCard scriptPosGoal = newPosition.GetComponent<PositionGoalCard>();
        if (scriptCard.Value == 1 && scriptPosGoal.Seed.Equals(scriptCard.Seed))
        {
            //Remove the card from the column and set new position
            scriptCard.SetNewOriginalPosition(newPosition.transform.position);
            scriptCard.Column.RemoveCard(card);
            //Add the card to the position goal
            newPosition.GetComponent<PositionGoalCard>().AddCard(card);
            //Modify score and moves
            ScoreAndMoves(10);
        }
        else
            scriptCard.SetOldOriginalPosition();
    }

    /// <summary>
    /// User takes a card from Column and put it above a card that is in the
    /// PositionGoal
    /// </summary>
    private void FromColumnToPositionGoalCard()
    {
        Debug.Log("FromColumnToPositionGoalCard");
        Card scriptCard = card.GetComponent<Card>();
        Card scriptBelowCard = newPosition.GetComponent<Card>();
        if (scriptCard.Value == scriptBelowCard.Value + 1 && scriptCard.Seed.Equals(scriptBelowCard.Seed) && scriptCard.ChildCard==null)//check value, seed and if the card have child
        {
            //remove from column and add to the positionGoal
            scriptCard.Column.RemoveCard(card);
            scriptBelowCard.PositionGoal.AddCard(card);
            //setup the position
            Vector3 newPos = newPosition.transform.position;
            newPos.z = card.transform.position.z;
            scriptCard.SetNewOriginalPosition(newPos);
            //Modify score and moves
            ScoreAndMoves(10);
        }
        else
            scriptCard.SetOldOriginalPosition();
    }

    /// <summary>
    /// User takes a card from Column and put it in a empty Column
    /// </summary>
    private void FromColumnToColumn()
    {
        Debug.Log("FromColumnToColumn");
        Card scriptCard = card.GetComponent<Card>();
        if (scriptCard.Value == 13 && newPosition.GetComponent<Column>().NumberOfCard()==0)
        {
            //Remove card frm the old column and put in the new empty column
            scriptCard.Column.RemoveCard(card);
            newPosition.GetComponent<Column>().AddSingleCard(card);
            //Set new position
            scriptCard.SetNewOriginalPosition(newPosition.transform.position);
            //Set child card
            if (scriptCard.ChildCard != null)
            {
                FixedChildCards(card, scriptCard.ChildCard);
            }
            //Modify score and moves
            ScoreAndMoves(0);
        }
        else
            scriptCard.SetOldOriginalPosition();
    }

    /// <summary>
    /// User takes a card from Column and put it above a card that is in a
    /// Column
    /// </summary>
    private void FromColumnToColumnCard()
    {
        Debug.Log("FromColumnToColumnCard");
        Card scriptCard = card.GetComponent<Card>();
        Card scriptBelowCard = newPosition.GetComponent<Card>();
        if (scriptCard.Value == scriptBelowCard.Value - 1 && CheckSeedInColumn(scriptCard.Seed, scriptBelowCard.Seed) && scriptBelowCard.ChildCard == null)//check value, seed and that card below doesn't have child
        {
            //Remove card frm the old column and put in the new empty column
            scriptCard.Column.RemoveCard(card);
            scriptBelowCard.Column.AddSingleCard(card);
            //Set Position
            Vector3 fatherPos = newPosition.transform.position;
            fatherPos.y -= GameManager.instance.VerticalSpaceBetweenCard;
            fatherPos.z = card.transform.position.z;
            scriptCard.SetNewOriginalPosition(fatherPos);
            //set parent relation
            scriptCard.FatherCard = newPosition;
            scriptBelowCard.ChildCard = card;
            //Set child card
            if (scriptCard.ChildCard != null)
            {
                FixedChildCards(card, scriptCard.ChildCard);
            }
            //Modify score and moves
            ScoreAndMoves(0);
        }
        else
            scriptCard.SetOldOriginalPosition();
    }

    private void FixedChildCards(GameObject father, GameObject child)
    {
        Card scriptFather = father.GetComponent<Card>();
        Card scriptChild = child.GetComponent<Card>();
        //add the child cards in the new column
        scriptFather.Column.AddSingleCard(child);
        //set the new position
        Vector3 newPos = father.transform.position;
        newPos.z -= 1;
        newPos.y -= GameManager.instance.VerticalSpaceBetweenCard;
        scriptChild.SetNewOriginalPosition(newPos);
        //check if the card has a child card
        if (scriptChild.ChildCard != null)
            FixedChildCards(child, scriptChild.ChildCard);
    }

    /// <summary>
    /// User takes a card from PositionGoal and put it in a empty Column
    /// </summary>
    private void FromPositionGoalToColumn()
    {
        Card scriptCard = card.GetComponent<Card>();
        Card scriptBelowCard = newPosition.GetComponent<Card>();
        if (scriptCard.Value == 13)
        {
            //Remove card from positionGoam and add it to the column
            scriptCard.PositionGoal.RemoveCard();
            newPosition.GetComponent<Column>().AddSingleCard(card);
            //Set new position
            scriptCard.SetNewOriginalPosition(newPosition.transform.position);
            //Modify score and moves
            ScoreAndMoves(-15);
        }
        else
            scriptCard.SetOldOriginalPosition();
    }

    /// <summary>
    /// User takes a card from PositionGoal and put it above a card that is in a
    /// Column
    /// </summary>
    private void FromPositionGoalToColumnCard()
    {
        Debug.Log("FromPositionGoalToColumnCard");
        Card scriptCard = card.GetComponent<Card>();
        Card scriptBelowCard = newPosition.GetComponent<Card>();
        if (scriptCard.Value == scriptBelowCard.Value - 1 && CheckSeedInColumn(scriptCard.Seed, scriptBelowCard.Seed) && scriptBelowCard.ChildCard == null)//check value, seed and that card below doesn't have child
        {
            //remove from the positionGoal and add it to the column
            scriptCard.PositionGoal.RemoveCard();
            scriptBelowCard.Column.AddSingleCard(card);
            //setup the position
            Vector3 fatherPos = newPosition.transform.position;
            fatherPos.y -= GameManager.instance.VerticalSpaceBetweenCard;
            fatherPos.z = card.transform.position.z;
            scriptCard.SetNewOriginalPosition(fatherPos);
            //set parent relation
            scriptCard.FatherCard = newPosition;
            scriptBelowCard.ChildCard = card;
            //Modify score and moves
            ScoreAndMoves(-15);
        }
        else
            scriptCard.SetOldOriginalPosition();
    }

    /// <summary>
    /// User takes a card from Deck and put it above a card that is in a
    /// PositionGoal
    /// </summary>
    private void FromDeckToPositionGoalCard()
    {
        Debug.Log("FromDeckToPositionGoalCard");
        Card scriptCard = card.GetComponent<Card>();
        Card scriptBelowCard = newPosition.GetComponent<Card>();
        if (scriptCard.Value == scriptBelowCard.Value + 1 && scriptCard.Seed.Equals(scriptBelowCard.Seed))//check value and seed
        {
            //remove from deck and add it to the column
            scriptCard.Deck.RemoveLastCardFromWaste();
            scriptBelowCard.PositionGoal.AddCard(card);
            //setup the position
            Vector3 newPos = newPosition.transform.position;
            newPos.z = card.transform.position.z;
            scriptCard.SetNewOriginalPosition(newPos);
            //Modify score and moves
            ScoreAndMoves(10);
        }
        else
            scriptCard.SetOldOriginalPosition();
    }
    /// <summary>
    /// User takes a card from Deck and put it above a card that is in a
    /// Column
    /// </summary>
    private void FromDeckToColumnCard()
    {
        Debug.Log("FromDeckToColumnCard");
        Card scriptCard = card.GetComponent<Card>();
        Card scriptBelowCard = newPosition.GetComponent<Card>();
        if(scriptCard.Value == scriptBelowCard.Value - 1 && CheckSeedInColumn(scriptCard.Seed,scriptBelowCard.Seed) && scriptBelowCard.ChildCard==null)//check value, seed and that card below doesn't have child
        {
            //remove from deck and add it to the column
            scriptCard.Deck.RemoveLastCardFromWaste();
            scriptBelowCard.Column.AddSingleCard(card);
            //setup the position
            Vector3 fatherPos = newPosition.transform.position;
            fatherPos.y -= GameManager.instance.VerticalSpaceBetweenCard;
            fatherPos.z = card.transform.position.z;
            scriptCard.SetNewOriginalPosition(fatherPos);
            //set parent relation
            scriptCard.FatherCard = newPosition;
            scriptBelowCard.ChildCard = card;
            //Modify score and moves
            ScoreAndMoves(5);
        }
        else
            scriptCard.SetOldOriginalPosition();
    }

    /// <summary>
    /// User takes a card from Deck and put it in a empty PositionGoal
    /// </summary>
    private void FromDeckToPositionGoal()
    {
        Debug.Log("FromDeckToPositionGoal");
        Card scriptCard = card.GetComponent<Card>();
        PositionGoalCard scriptPosGoal = newPosition.GetComponent<PositionGoalCard>();
        if (scriptCard.Value == 1 && scriptPosGoal.Seed.Equals(scriptCard.Seed))
        {
            //Remove the card from the deck and add it to the positionalGoal
            scriptCard.Deck.RemoveLastCardFromWaste();
            newPosition.GetComponent<PositionGoalCard>().AddCard(card);
            //Set the new position
            scriptCard.SetNewOriginalPosition(newPosition.transform.position);
            //Modify score and moves
            ScoreAndMoves(10);
        }
        else
            scriptCard.SetOldOriginalPosition();
    }

    /// <summary>
    /// User takes a card from Deck and put it in a empty Column
    /// </summary>
    private void FromDeckToColumn()
    {
        Debug.Log("FromDeckToColumn");
        Card scriptCard = card.GetComponent<Card>();
        if (scriptCard.Value == 13)
        {
            //Remove the card from the deck and add it to the empty column
            scriptCard.Deck.RemoveLastCardFromWaste();
            newPosition.GetComponent<Column>().AddSingleCard(card);
            //set the new position
            scriptCard.SetNewOriginalPosition(newPosition.transform.position);
            //Modify score and moves
            ScoreAndMoves(5);
        }
        else
            scriptCard.SetOldOriginalPosition();
    }

    /// <summary>
    /// Check if 2 seed can be connected in column (in order to have a 
    /// sequence of red-black-red-...)
    /// </summary>
    /// <param name="s1">Seed of first card</param>
    /// <param name="s2">Seed of second card</param>
    private bool CheckSeedInColumn(String s1,String s2)
    {
        if ((s1.Equals("H") || s1.Equals("D")) && (s2.Equals("C") || s2.Equals("S")))
            return true;
        if ((s2.Equals("H") || s2.Equals("D")) && (s1.Equals("C") || s1.Equals("S")))
            return true;
        return false;
    }

    /// <summary>
    /// Increment score and moves
    /// </summary>
    /// <param name="points">value that are sum or decrement to the score</param>
    private void ScoreAndMoves(int points)
    {
        ScoreManager.instance.AddScore(points);
        ScoreManager.instance.IncrementMoves();
    }
}
