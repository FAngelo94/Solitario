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
    private bool checkUndoMove;

    private List<int> pastScore;

    private void Awake()
    {
        pastScore = new List<int>();
    }

    private void Start()
    {
        myCollider = gameObject.GetComponent<Collider2D>();
        checkMoveCard = false;
        checkUndoMove = false;
    }

    private void Update()
    {
        CheckTouchOnObject();
        if (checkMoveCard)
        { 
            checkMoveCard = false;
            MoveWasteCards();
        }
        if (checkUndoMove)
        {
            checkUndoMove = false;
            MoveUndoCards();
        }
    }

    protected override void SpritePressedBegan()
    {
        if (myCollider == Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position)))
        {
            if (deckCards.Count > 0)
            {
                if (GameManager.instance.Draw3Mode == 0)
                    TakeACard(true);
                else
                {
                    for (int i = 0; i < 3; i++)
                        TakeACard(true);
                }
            }
            else
                ResetDeck();
            if (deckCards.Count + wasteCards.Count != 0)
                ScoreManager.instance.IncrementMoves();
        }
    }

    /// <summary>
    /// Method that move the card from the deck to the waste
    /// </summary>
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

    /// <summary>
    /// Method that move the card from the waste to the deck
    /// </summary>
    private void MoveUndoCards()
    {
        //fixed the cards in waste
        int indexWaste = wasteCards.Count - 1;
        int countWaste = 0;
        while(countWaste<3)
        {
            if (indexWaste >= 0)
            {
                GameObject card = wasteCards[indexWaste];
                Vector3 newPoint = Waste.transform.position;
                newPoint.x -= GameManager.instance.OrizzontalSpaceBetweenCard * countWaste;
                newPoint.z = card.transform.position.z;
                card.GetComponent<Card>().TraslateCard(newPoint);
                if (!card.transform.position.Equals(newPoint))
                    checkUndoMove = true;//we need move card because it doesn't reach the new point
            }
            countWaste++;
            indexWaste--;
        }
        //move cards from the waste to the deck
        foreach (GameObject deckCard in deckCards)
        {
            if (deckCard.activeSelf)
            {
                deckCard.GetComponent<Card>().TraslateCard(transform.position);
                if (!deckCard.transform.position.Equals(transform.position))
                    checkUndoMove = true;
                else
                    deckCard.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Method to put a card from deck to the waste
    /// </summary>
    /// <param name="saveMove">If True user want a card and then the method save the move</param>
    private void TakeACard(bool saveMove)
    {
        if (deckCards.Count > 0)
        {
            wasteCards.Add(deckCards[deckCards.Count - 1]);
            deckCards.Remove(deckCards[deckCards.Count - 1]);
            //Move the card in waste
            wasteCards[wasteCards.Count - 1].SetActive(true);
            //Set card layout
            int index = wasteCards.Count - 1;
            while (index >= 0)
            {
                Vector3 cardPos = wasteCards[index].transform.position;
                wasteCards[index].transform.position = new Vector3(cardPos.x, cardPos.y, -index);
                index--;
            }
            //set card scripts
            for (int i = 0; i < wasteCards.Count; i++)
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
            //Save move
            if (saveMove)
            {
                Card scriptCard = wasteCards[wasteCards.Count - 1].GetComponent<Card>();
                BackManager.instance.SaveMove(wasteCards[wasteCards.Count - 1], scriptCard, scriptCard, 0);
            }
        }
    }

    /// <summary>
    /// When all cards are in the waste and user press deck this method put all
    /// the cards again in the deck
    /// </summary>
    private void ResetDeck()
    {
        if (wasteCards.Count > 0)
        {
            transform.Find("Background").gameObject.SetActive(false);
            transform.Find("Back").gameObject.SetActive(true);
            for (int i = wasteCards.Count - 1; i >= 0; i--)
            {
                AddSingleCard(wasteCards[i]);
            }
            wasteCards = new List<GameObject>();
            if (GameManager.instance.Draw3Mode==0)
            {//Not modify the score when deck is reseted if the player are in draw 3 mode
                pastScore.Add(ScoreManager.instance.Score);
                ScoreManager.instance.AddScore(-100);
            }
        }
    }

    //Public method
    /// <summary>
    /// Add single card to the deck
    /// </summary>
    /// <param name="newCard"></param>
    public void AddSingleCard(GameObject newCard)
    {
        deckCards.Add(newCard);
        newCard.GetComponent<Card>().SetDeck(this);
        newCard.transform.position = transform.position;
        newCard.SetActive(false);
    }

    /// <summary>
    /// Remove the last card from the waste when user take it and put
    /// in an other place
    /// </summary>
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

    /// <summary>
    /// Method uses when user undo a deck taken
    /// </summary>
    public bool UndoTake()
    {
        if (wasteCards.Count > 0)
        {
            deckCards.Add(wasteCards[wasteCards.Count - 1]);
            wasteCards.Remove(wasteCards[wasteCards.Count - 1]);
            if (wasteCards.Count >= 3)
            {
                wasteCards[wasteCards.Count - 3].SetActive(true);
                wasteCards[wasteCards.Count - 3].GetComponent<Card>().RotateFrontCard();
            }
            deckCards[deckCards.Count - 1].GetComponent<Card>().RotateBackCard();
            checkUndoMove = true;
            //Set active the deck if before there aren't card and now yes
            if (deckCards.Count == 1)
            {
                transform.Find("Background").gameObject.SetActive(false);
                transform.Find("Back").gameObject.SetActive(true);
            }
            //active last card in waste if there
            if (wasteCards.Count > 0)
                wasteCards[wasteCards.Count - 1].GetComponent<Card>().enabled = true;
            return false;
        }
        else
        {//Put all the card in waste
            int count = deckCards.Count;
            for (int i = 0; i < count; i++)
                    TakeACard(false);
           if(pastScore.Count>0)
            {
                ScoreManager.instance.AddScore(pastScore[pastScore.Count - 1]);
                pastScore[pastScore.Count - 1] = -1;
                pastScore.RemoveAll(x => x==-1);
            }
            return true;
        }
    }

    /// <summary>
    /// Method uses when user undo a move with a card taken from waste
    /// </summary>
    public void AddToWaste(GameObject card)
    {
        card.GetComponent<Card>().SetDeck(this);
        wasteCards.Add(card);
        //set layout
        Vector3 cardPos = card.transform.position;
        card.transform.position = new Vector3(cardPos.x, cardPos.y, -wasteCards.Count);
        //Disable script of the past first waste card
        if (wasteCards.Count > 1)
            wasteCards[wasteCards.Count - 2].GetComponent<Card>().enabled = false;
        //start the transition of card
        checkMoveCard = true;
    }

    /// <summary>
    /// </summary>
    /// <returns>Number of cards remain in deck or waste</returns>
    public int NumberOfCardInDeck()
    {
        return deckCards.Count + wasteCards.Count;
    }

    /// <summary>
    /// </summary>
    /// <returns>The Card script of the active card in the waste</returns>
    public List<GameObject> GetActiveCardFromWaste()
    {
        List<GameObject> allDeckCards = new List<GameObject>();
        allDeckCards.AddRange(deckCards);
        allDeckCards.AddRange(wasteCards);
        if (wasteCards.Count > 0)
            return allDeckCards;
        return null;
    }
}
