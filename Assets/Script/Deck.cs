using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour {

    private List<GameObject> cards = new List<GameObject>();

    private void Start()
    {
       
    }

    public void AddSingleCard(GameObject newCard)
    {
        cards.Add(newCard);
    }
}
