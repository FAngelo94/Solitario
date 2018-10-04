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
            if (tagCollision.Equals("PositionGoal"))
            {
                FromColumnToPositionGoal();
                newPositionIsValid = true;
            }
            if (tagCollision.Equals("Column"))
            {
                FromColumnToColumn();
                newPositionIsValid = true;
            }
            if (tagCollision.Equals("Card"))
            {
                if (newPosition.GetComponent<Card>().PositionGoal != null)
                {
                    FromColumnToPositionGoalCard();
                    newPositionIsValid = true;
                }
                if (newPosition.GetComponent<Card>().Column!=null)
                {
                    FromColumnToColumnCard();
                    newPositionIsValid = true;
                }
            }
        }
        if (card.GetComponent<Card>().PositionGoal != null)//User take a card from positionGoal
        {
            if (tagCollision.Equals("Column"))
            {
                FromPositionGoalToColumn();
                newPositionIsValid = true;
            }
            if (tagCollision.Equals("Card"))
            {
                if (newPosition.GetComponent<Card>().Column != null)
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
                    FromDeckToPositionGoalCard();
                    newPositionIsValid = true;
                }
                if (newPosition.GetComponent<Card>().Column != null)
                {
                    FromDeckToColumnCard();
                    newPositionIsValid = true;
                }
            }
            if (tagCollision.Equals("PositionGoal"))
            {
                FromDeckToPositionGoal();
                newPositionIsValid = true;
            }
            if (tagCollision.Equals("Column"))
            {
                FromDeckToColumn();
                newPositionIsValid = true;
            }
        }

        if (!newPositionIsValid)
        {
            card.GetComponent<Card>().SetOldOriginalPosition();
        }
    }

    /// <summary>
    /// User takes a card from Column and put it in the empty PositionGoal
    /// </summary>
    private void FromColumnToPositionGoal()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User takes a card from Column and put it in a empty Column
    /// </summary>
    private void FromColumnToColumn()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User takes a card from Column and put it above a card that is in the
    /// PositionGoal
    /// </summary>
    private void FromColumnToPositionGoalCard()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User takes a card from Column and put it above a card that is in a
    /// Column
    /// </summary>
    private void FromColumnToColumnCard()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// User takes a card from PositionGoal and put it in a empty Column
    /// </summary>
    private void FromPositionGoalToColumn()
    {
        Card scriptCard = card.GetComponent<Card>();
        if (scriptCard.Value == 13)
        {
            scriptCard.PositionGoal.RemoveCard();
            newPosition.GetComponent<Column>().AddSingleCard(card);
            //Set new position
            scriptCard.SetNewOriginalPosition(newPosition.transform.position);
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
        if (scriptCard.Value == scriptBelowCard.Value - 1 && CheckSeedInColumn(scriptCard.Seed, scriptBelowCard.Seed))//check value and seed
        {
            //remove from the positionGoal and add to the column
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
            //remove from deck and add to the column
            scriptCard.Deck.RemoveLastCardFromWaste();
            scriptBelowCard.PositionGoal.AddCard(card);
            //setup the position
            Vector3 newPos = newPosition.transform.position;
            newPos.z = card.transform.position.z;
            scriptCard.SetNewOriginalPosition(newPos);
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
        if(scriptCard.Value == scriptBelowCard.Value - 1 && CheckSeedInColumn(scriptCard.Seed,scriptBelowCard.Seed))//check value and seed
        {
            //remove from deck and add to the column
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
            scriptCard.Deck.RemoveLastCardFromWaste();
            scriptCard.SetNewOriginalPosition(newPosition.transform.position);
            newPosition.GetComponent<PositionGoalCard>().AddCard(card);
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
            scriptCard.Deck.RemoveLastCardFromWaste();
            scriptCard.SetNewOriginalPosition(newPosition.transform.position);
            newPosition.GetComponent<Column>().AddSingleCard(card);
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
}
