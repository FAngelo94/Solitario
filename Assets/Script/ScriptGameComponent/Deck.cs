using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : TouchManager
{

    [SerializeField]
    [Header("WasteObject")]
    private GameObject Waste;

    private List<GameObject> deckCards = new List<GameObject>();
    private List<GameObject> wasteCards = new List<GameObject>();
    
    private Collider2D myCollider;
    
    private bool checkMoveCard;

    private void Start()
    {
        myCollider = gameObject.GetComponent<Collider2D>();
        checkMoveCard = false;
    }

    private void Update()
    {
        CheckTouchOnObject();
        if (checkMoveCard)
        { 
            checkMoveCard = false;
            MoveWasteCards();
        }
    }

    private void MoveWasteCards()
    {
        int index = wasteCards.Count - 3;
        int count = 2;
        while (index < wasteCards.Count)
        {
            if (index >= 0)
            {
                GameObject card = wasteCards[index];
                Vector3 newPoint = Waste.transform.position;
                newPoint.x -= GameManager.instance.OrizzontalSpaceBetweenCard * count;
                newPoint.z = card.transform.position.z;
                card.GetComponent<Card>().TraslateCard(newPoint);
                if (!card.transform.position.Equals(newPoint))
                    checkMoveCard = true;//we need move card because it doesn't reach the new point
                else
                    card.GetComponent<Card>().RotateFrontCard();
            }
            count--;
            index++;
        }

        //Disactive old card
        for (int i = 0; i < wasteCards.Count - 3; i++)
        {
            if (wasteCards[i].activeSelf)
            {
                wasteCards[i].SetActive(false);
            }
        }
    }

    protected override void SpritePressedBegan()
    {
        if (myCollider == Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position)))
        {
            if (deckCards.Count > 0)
                TakeACard();
            else
                ResetDeck();
            if (deckCards.Count + wasteCards.Count != 0)
                ScoreManager.instance.IncrementMoves();
        }
    }

    private void TakeACard()
    {
        wasteCards.Add(deckCards[0]);
        deckCards.Remove(deckCards[0]);
        //Move the card in waste
        wasteCards[wasteCards.Count - 1].SetActive(true);
        Debug.Log("Deck in waste=" + (wasteCards[wasteCards.Count - 1].GetComponent<Card>().Deck == null));
        //Set card layout
        int index = wasteCards.Count - 1;
        while (index >= 0)
        {
            Vector3 cardPos = wasteCards[index].transform.position;
            wasteCards[index].transform.position = new Vector3(cardPos.x, cardPos.y, -index);
            index--;
        }
        //set card scripts
        for(int i = 0; i < wasteCards.Count; i++)
        {//disable the script of all card except the last
            Card script = wasteCards[i].GetComponent<Card>();
            script.enabled = (i == wasteCards.Count - 1 ? true : false);
        }
        checkMoveCard = true;
        if (deckCards.Count == 0)
        {
            transform.Find("Background").gameObject.SetActive(true);
            transform.Find("Back").gameObject.SetActive(false);
        }
    }

    private void ResetDeck()
    {
        if (wasteCards.Count > 0)
        {
            transform.Find("Background").gameObject.SetActive(false);
            transform.Find("Back").gameObject.SetActive(true);
            foreach (GameObject card in wasteCards)
            {
                AddSingleCard(card);
            }
            wasteCards = new List<GameObject>();
            ScoreManager.instance.AddScore(-100);
        }
    }

    //Public method

    public void AddSingleCard(GameObject newCard)
    {
        deckCards.Add(newCard);
        newCard.GetComponent<Card>().SetDeck(this);
        newCard.transform.position = transform.position;
        newCard.SetActive(false);
    }

    public void RemoveLastCardFromWaste()
    {
        Debug.Log("RemoveLastCardFromWaste");
        GameObject lastCard = wasteCards[wasteCards.Count - 1];
        wasteCards.Remove(lastCard);
        if (wasteCards.Count > 0)
        {
            wasteCards[wasteCards.Count - 1].GetComponent<Card>().enabled = true;
            if (wasteCards.Count >= 3)
            {
                wasteCards[wasteCards.Count - 3].SetActive(true);
            }
            checkMoveCard = true;
        }
    }
}
