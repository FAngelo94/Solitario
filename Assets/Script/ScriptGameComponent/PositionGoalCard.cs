using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionGoalCard : MonoBehaviour {

    [SerializeField]
    [Header("Simbol of seed (H,D,C,S)")]
    private string _seed;
    public string Seed
    {
        get { return _seed; }
    }

    private List<GameObject> cards = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Card script = collision.gameObject.GetComponent<Card>();
    }

    /// <summary>
    /// Add a new card and change its Z position
    /// </summary>
    /// <param name="newCard">new card</param>
    public void AddCard(GameObject newCard)
    {
        Debug.Log("AddCard");
        newCard.GetComponent<Card>().SetPositionGoal(this);
        if (cards.Count > 0)
            cards[cards.Count - 1].GetComponent<Collider2D>().enabled=false;
        cards.Add(newCard);
        Vector3 newPos = newCard.transform.position;
        newPos.z = -cards.Count;
        newCard.transform.position = newPos;
        gameObject.GetComponent<Collider2D>().enabled = false;
    }

    public void RemoveCard()
    {
        Debug.Log("count=" + cards.Count + "-" + cards[cards.Count - 1].GetComponent<Card>().Value);
        cards.Remove(cards[cards.Count-1]);
        if(cards.Count>0)
            cards[cards.Count - 1].GetComponent<Collider2D>().enabled = true;
        else
            gameObject.GetComponent<Collider2D>().enabled = true;
    }

    public Card GetLastCard()
    {
        if (cards.Count > 0)
            return cards[cards.Count - 1].GetComponent<Card>();
        return null;
    }
}
