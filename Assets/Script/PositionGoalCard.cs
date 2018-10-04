using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionGoalCard : MonoBehaviour {

    [SerializeField]
    [Header("Simbol of seed (H,D,C,S)")]
    private string Seed;

    private List<GameObject> cards = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Card script = collision.gameObject.GetComponent<Card>();
        Debug.Log("Trigger"+script.Seed + " - " + script.Value);
    }

    public void AddCard(GameObject card)
    {
        Card script = card.GetComponent<Card>();
        if (script.Seed.Equals(Seed)) {
            bool checkNewCard = false;
            if (cards.Count == 0 && script.Value==0)
            {
                checkNewCard = true;
            }
            if(cards.Count > 0 && cards.Count+1 == script.Value)
            {
                checkNewCard = true;
            }
            if (checkNewCard)
            {
                cards.Add(card);
                script.SetNewOriginalPosition(transform.position);
            }
        }
    }

    public void RemoveCard(GameObject card)
    {
        cards.Remove(card);
    }
}
