using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour {

    private List<GameObject> cards = new List<GameObject>();

	// Use this for initialization
	void Start ()
    { 
    }

    /// <summary>
    /// Function to remove every card above a removed card in a column
    /// </summary>
    /// <param name="childCard">card you want to remove</param>
    private void RemoveChildCard(GameObject childCard)
    {
        cards.Remove(childCard);
        Card scriptCard = childCard.GetComponent<Card>();
        if (scriptCard.ChildCard != null)
            RemoveChildCard(scriptCard.ChildCard);
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
}
