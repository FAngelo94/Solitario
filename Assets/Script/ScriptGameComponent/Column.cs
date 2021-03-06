﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour {

    private List<GameObject> cards = new List<GameObject>();

	// Use this for initialization
	void Start ()
    { 
    }

    /// <summary>
    /// Add a new card and change its Z position
    /// </summary>
    /// <param name="newCard">new card</param>
    public void AddSingleCard(GameObject newCard)
    {
        newCard.GetComponent<Card>().SetColumn(this);
        cards.Add(newCard);
        Vector3 newPos = newCard.transform.position;
        newPos.z = -cards.Count;
        newCard.transform.position = newPos;
        gameObject.GetComponent<Collider2D>().enabled = false;
    }

    public bool SetUpCards()
    {
        bool check = false;
        int count = 0;
        foreach (GameObject card in cards)
        {
            Vector3 cardPos = card.transform.position;
            Vector3 newPosition = new Vector3(transform.position.x, transform.position.y - GameManager.instance.VerticalSpaceBetweenCard * count,cardPos.z);
            if (!newPosition.Equals(cardPos))
            {
                card.GetComponent<Card>().TraslateCard(newPosition);
                check = true;
            }
            count++;
        }
        return check;
    }

    public void RotateLastCard()
    {
        if (cards.Count > 0)
        {
            foreach (GameObject card in cards)
                card.GetComponent<Card>().enabled = false;
            cards[cards.Count - 1].GetComponent<Card>().enabled = true;
            cards[cards.Count - 1].GetComponent<Card>().RotateFrontCard();
            Debug.Log("Rotate last card=" + cards[cards.Count - 1].GetComponent<Card>().Seed + "-" + cards[cards.Count - 1].GetComponent<Card>().Value);
            ScoreManager.instance.AddScore(5);
        }
    }

    public void RotateLastCardFistTime()
    {
        if (cards.Count > 0)
        {
            foreach (GameObject card in cards)
                card.GetComponent<Card>().enabled = false;
            cards[cards.Count - 1].GetComponent<Card>().enabled = true;
            cards[cards.Count - 1].GetComponent<Card>().RotateFrontCard();
        }
    }

    public void RemoveCard(GameObject card)
    {
        Debug.Log("Column count before=" + cards.Count);
        cards.Remove(card);
        if (cards.Count <= 0)
            gameObject.GetComponent<Collider2D>().enabled = true;
        Card scriptCard = card.GetComponent<Card>();
        if (scriptCard.ChildCard != null)
        {//check if the card has child cards (cards that are above it)
            RemoveChildCard(scriptCard.ChildCard);
        }
        if (scriptCard.FatherCard != null)
        {//check if the card has father cards (cards that are below it)
            scriptCard.FatherCard.GetComponent<Card>().ChildCard = null;
        }
        else
        {
            if (cards.Count > 0)
                RotateLastCard();
            else
                gameObject.GetComponent<Collider2D>().enabled = true;
        }
        Debug.Log("Column count after=" + cards.Count);
    }

    /// <summary>
    /// Function to remove every card above a removed card in a column
    /// </summary>
    /// <param name="childCard">card you want to remove</param>
    private void RemoveChildCard(GameObject childCard)
    {
        cards.Remove(childCard);
        Card scriptCard = childCard.GetComponent<Card>();
        Debug.Log("Remove child="+scriptCard.Value);
        if (scriptCard.ChildCard != null)
            RemoveChildCard(scriptCard.ChildCard);
    }

    /// <summary>
    /// Method used in back operation when we move more cards
    /// </summary>
    /// <param name="card"></param>
    public void RemoveCardChild(GameObject card)
    {
        cards.Remove(card);
    }

    public int NumberOfCard()
    {
        return cards.Count;
    }

    /// <summary>
    /// Check in the list if there are some hide card
    /// </summary>
    /// <returns>True if there are some hide cards</returns>
    public bool CheckHideCard()
    {
        foreach(GameObject card in cards)
        {
            if (card.GetComponent<Card>().GetRotateCard() != 1)
                return true;
        }
        return false;
    }

    public void HideLastCard()
    {
        
        if (cards.Count >= 2)
        {
            GameObject card = cards[cards.Count - 2];
            card.GetComponent<Card>().RotateBackCard();
            card.GetComponent<Card>().enabled = false;
        }
        else
            Debug.LogError("Problem");
    }

    public Card GetLastCard()
    {
        if (cards.Count > 0)
            return cards[cards.Count - 1].GetComponent<Card>();
        return null;
    }

    public List<Card> GetAllNotHideCards()
    {
        List<Card> notHideCards = new List<Card>();
        foreach(GameObject card in cards)
        {
            Card scriptCard = card.GetComponent<Card>();
            if (scriptCard.GetRotateCard() == 1)
                notHideCards.Add(scriptCard);
        }
        return notHideCards;
    }

    
}
