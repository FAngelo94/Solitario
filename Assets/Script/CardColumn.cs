using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardColumn : MonoBehaviour {

    private List<GameObject> cards = new List<GameObject>();
    private MovementCard movementCard = new MovementCard();

	// Use this for initialization
	void Start () {
                
	}
	
	public void AddSingleCard(GameObject newCard)
    {
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
                movementCard.TranslateCard(card, newPosition);
                check = true;
            }
            count++;
        }
        return check;
    }

    public void RotateLastCard()
    {
        movementCard.RotateCard(cards[cards.Count - 1]);
    }
}
