using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardColumn : MonoBehaviour {

    private List<GameObject> cards = new List<GameObject>();

	// Use this for initialization
	void Start ()
    { 
    }
	
	public void AddSingleCard(GameObject newCard)
    {
        newCard.GetComponent<Card>().SetColumn(this);
        cards.Add(newCard);
    }

    public bool SetUpCards()
    {
        bool check = false;
        int count = 0;
        foreach (GameObject card in cards)
        {
            Vector2 newPosition = new Vector2(transform.position.x, transform.position.y - GameManager.instance.VerticalSpaceBetweenCard * count);
            Vector2 cardPos = card.transform.position;
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
        foreach (GameObject card in cards)
            card.GetComponent<Card>().enabled = false;
        cards[cards.Count - 1].GetComponent<Card>().enabled = true;
        cards[cards.Count-1].GetComponent<Card>().RotateCard();
    }

    public void RemoveCard(GameObject card)
    {
        cards.Remove(card);
        RotateLastCard();
    }
}
