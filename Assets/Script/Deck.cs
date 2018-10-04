﻿using System;
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

    private MovementCard movementCard;
    private bool checkMoveCard;

    private void Start()
    {
        myCollider = gameObject.GetComponent<Collider2D>();
        movementCard = new MovementCard();
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
                movementCard.TranslateCard(card, newPoint);
                if (!card.transform.position.Equals(newPoint))
                    checkMoveCard = true;//we need move card because it doesn't reach the new point
                else
                    movementCard.RotateFrontCard(card);
            }
            count--;
            index++;
        }

        //Disactive old card
        for (int i = 0; i < wasteCards.Count - 3; i++)
        {
            wasteCards[i].SetActive(false);
        }
    }

    public void AddSingleCard(GameObject newCard)
    {
        newCard.GetComponent<Card>().SetDeck(this);
        deckCards.Add(newCard);
        newCard.transform.position = transform.position;
        newCard.SetActive(false);
    }

    protected override void SpritePressedBegan()
    {
        if (myCollider == Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position)))
        {
            if (deckCards.Count > 0)
                TakeACard();
            else
                ResetDeck();
        }
    }

    private void TakeACard()
    {
        wasteCards.Add(deckCards[0]);
        deckCards.Remove(deckCards[0]);
        //Move the card in waste
        wasteCards[wasteCards.Count - 1].SetActive(true);
        //Set card layout
        int index = wasteCards.Count - 1;
        while (index >= 0)
        {
            Debug.Log("index=" + index);
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
        Debug.Log("Carte Rimaste="+deckCards.Count);
        if (deckCards.Count == 0)
        {
            transform.Find("Background").gameObject.SetActive(true);
            transform.Find("Back").gameObject.SetActive(false);
        }
    }

    private void ResetDeck()
    {
        transform.Find("Background").gameObject.SetActive(false);
        transform.Find("Back").gameObject.SetActive(true);
        foreach (GameObject card in wasteCards)
        {
            AddSingleCard(card);
        }
        wasteCards=new List<GameObject>();
    }
}
