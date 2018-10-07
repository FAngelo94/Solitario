using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// In this class I develop the gameplay of the game, it's mean here
/// I analyze where is the card when user leave it in order to modify the position of 
/// that card in the game
/// </summary>
public class GamePlay:MonoBehaviour {

    public static GamePlay instance;

    [Header("Deck")]
    [SerializeField]
    private GameObject Deck;

    [Header("Position")]
    [SerializeField]
    private GameObject PositionGoalObject;

    [Header("TableColumns")]
    [SerializeField]
    private GameObject TableObject;
    private List<GameObject> columns;

    private List<GameObject> cardsInMovement;
    private List<Vector3> finalPositions;

    private void Awake()
    {
        cardsInMovement = new List<GameObject>();
        finalPositions = new List<Vector3>();
    }

    private void Start()
    {
        if (instance == null)
            instance = this;
        FindColumns();
    }

    private void Update()
    {
        if (cardsInMovement.Count > 0)
        {
            for (int i = 0; i < cardsInMovement.Count; i++)
            {
                if (cardsInMovement[i].transform.position.Equals(finalPositions[i]))
                {//card "i" reaches its final position
                    cardsInMovement[i].GetComponent<Card>().SetNewOriginalPosition(finalPositions[i]);
                    finalPositions[i] = new Vector3(-100, -100, -100);
                    if (cardsInMovement[i].GetComponent<Card>().ChildCard != null)
                    {
                        FixedChildCardsMovements(cardsInMovement[i], cardsInMovement[i].GetComponent<Card>().ChildCard);
                    }
                    cardsInMovement[i] = null;
                }
                else
                {//card "i" doesn't reach its final position
                    cardsInMovement[i].GetComponent<Card>().TraslateCard(finalPositions[i]);
                    if (cardsInMovement[i].GetComponent<Card>().ChildCard != null)
                    {
                        FixedChildCardsMovements(cardsInMovement[i], cardsInMovement[i].GetComponent<Card>().ChildCard);
                    }
                }
            }
            //remove null gameobject and the finalposition associated
            cardsInMovement.RemoveAll(x => x == null);
            finalPositions.RemoveAll(x => x == new Vector3(-100, -100, -100));

        }
    }

    public void WhereIsCard(GameObject card, Collider2D collision)
    {
        bool newPositionIsValid = false;
        
        GameObject newPosition = collision.gameObject;
        string tagCollision = newPosition.tag;

        if (card.GetComponent<Card>().Column != null)//User take a card from column
        {
            if (tagCollision.Equals("PositionGoal"))//put the card in a empty positionGoal
            {
                FromColumnToPositionGoal(card,newPosition);
                newPositionIsValid = true;
            }
            if (tagCollision.Equals("Column"))//put the card in a empty column
            {
                FromColumnToColumn(card, newPosition);
                newPositionIsValid = true;
            }
            if (tagCollision.Equals("Card"))
            {
                if (newPosition.GetComponent<Card>().PositionGoal != null)//put the card in a positionGoal with card
                {
                    FromColumnToPositionGoalCard(card, newPosition);
                    newPositionIsValid = true;
                }
                if (newPosition.GetComponent<Card>().Column!=null)//put the card in a column with card
                {
                    FromColumnToColumnCard(card, newPosition);
                    newPositionIsValid = true;
                }
            }
        }
        if (card.GetComponent<Card>().PositionGoal != null)//User take a card from positionGoal
        {
            if (tagCollision.Equals("Column"))//put the card in a empty column
            {
                FromPositionGoalToColumn(card, newPosition);
                newPositionIsValid = true;
            }
            if (tagCollision.Equals("Card"))
            {
                if (newPosition.GetComponent<Card>().Column != null)//put the card in a column with card
                {
                    FromPositionGoalToColumnCard(card, newPosition);
                    newPositionIsValid = true;
                }
            }
        }
        if (card.GetComponent<Card>().Deck != null)//User take a card from the waste of deck
        {
            if (tagCollision.Equals("Card"))
            {
                if (newPosition.GetComponent<Card>().PositionGoal != null)
                {
                    FromDeckToPositionGoalCard(card, newPosition);//put the card in a positionGoal with card
                    newPositionIsValid = true;
                }
                if (newPosition.GetComponent<Card>().Column != null)//put the card in a column with card
                {
                    FromDeckToColumnCard(card, newPosition);
                    newPositionIsValid = true;
                }
            }
            if (tagCollision.Equals("PositionGoal"))//put the card in a empty positionGoal
            {
                FromDeckToPositionGoal(card, newPosition);
                newPositionIsValid = true;
            }
            if (tagCollision.Equals("Column"))//put the card in a empty column
            {
                FromDeckToColumn(card, newPosition);
                newPositionIsValid = true;
            }
        }

        if (!newPositionIsValid)
        {//User put the card in a not valid position, so the card will
            //be put in its original position
            card.GetComponent<Card>().SetOldOriginalPosition();
        }
    }

    public void DoubleTapManage(GameObject card)
    {
        bool newPositionIsValid = false;
        if (card.GetComponent<Card>().Column != null)//User double click a card that is in a column
        {     
            if (CheckPositionGoalAvaiable(card))
                newPositionIsValid = true;
        }
        if (card.GetComponent<Card>().Deck != null)//User double click a card that is in the deck
        {
            if (CheckPositionGoalAvaiable(card))
                newPositionIsValid = true;
            else
            {
                if(CheckColumnAvailable(card))
                {
                    newPositionIsValid = true;
                }
            }

        }
        if (!newPositionIsValid)
        {//User put the card in a not valid position, so the card will
            //be put in its original position
            card.GetComponent<Card>().SetOldOriginalPosition();
        }
    }
    private void FindColumns()
    {
        columns = new List<GameObject>();
        for (int i = 0; i < 7; i++)
        {
            string nameColumn = "Column_" + i;
            columns.Add(TableObject.transform.Find(nameColumn).gameObject);
        }
    }
    private bool CheckPositionGoalAvaiable(GameObject card)
    {
        string seed = card.GetComponent<Card>().Seed;
        int value = card.GetComponent<Card>().Value;
        GameObject positionGoal = null;
        switch (seed)
        {
            case "H":
                positionGoal = PositionGoalObject.transform.Find("PositionHearts").gameObject;
                break;
            case "D":
                positionGoal = PositionGoalObject.transform.Find("PositionDiamonds").gameObject;
                break;
            case "C":
                positionGoal = PositionGoalObject.transform.Find("PositionClubs").gameObject;
                break;
            case "S":
                positionGoal = PositionGoalObject.transform.Find("PositionSpades").gameObject;
                break;
        }
        Card lastCard = positionGoal.GetComponent<PositionGoalCard>().GetLastCard();
        if (lastCard != null && lastCard.Value == value - 1)
        {
            if (card.GetComponent<Card>().Column != null)
                FromColumnToPositionGoalCard(card, lastCard.gameObject);
            else
                FromDeckToPositionGoalCard(card, lastCard.gameObject);
            return true;
        }
        if (lastCard == null && value == 1)
        {
            if (card.GetComponent<Card>().Column != null)
                FromColumnToPositionGoal(card,positionGoal);
            else
                FromDeckToPositionGoal(card,positionGoal);
            return true;
        }
        return false;
    }
    private bool CheckColumnAvailable(GameObject card)
    {
        string seed = card.GetComponent<Card>().Seed;
        int value = card.GetComponent<Card>().Value;
        foreach(GameObject column in columns)
        {
            Card lastCard = column.GetComponent<Column>().GetLastCard();
            if(lastCard!=null)
            {
                if (CheckSeedInColumn(seed, lastCard.Seed) && value == lastCard.Value - 1)
                {
                    FromDeckToColumnCard(card,lastCard.gameObject);
                    return true;
                }
            }
            else
            {
                if(value==13)
                {
                    FromDeckToColumn(card,column);
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// User takes a card from Column and put it in the empty PositionGoal
    /// </summary>
    private void FromColumnToPositionGoal(GameObject card,GameObject newPosition)
    {
        Debug.Log("FromColumnToPositionGoal");
        Card scriptCard = card.GetComponent<Card>();
        PositionGoalCard scriptPosGoal = newPosition.GetComponent<PositionGoalCard>();
        if (scriptCard.Value == 1 && scriptPosGoal.Seed.Equals(scriptCard.Seed))
        {
            //Save the state of card before change it
            Card beforeCardStatus = scriptCard.CopyCardClass();
            //Remove the card from the column and set new position
            AddNewCardMovement(card, newPosition.transform.position);
            scriptCard.Column.RemoveCard(card);
            //Add the card to the position goal
            newPosition.GetComponent<PositionGoalCard>().AddCard(card);
            //Modify score and moves
            ScoreAndMoves(10);
            //Save the move
            Card afterCardStatus = scriptCard.CopyCardClass();
            BackManager.instance.SaveMove(card,afterCardStatus, beforeCardStatus, 10);
        }
        else
            scriptCard.SetOldOriginalPosition();
    }

    /// <summary>
    /// User takes a card from Column and put it above a card that is in the
    /// PositionGoal
    /// </summary>
    private void FromColumnToPositionGoalCard(GameObject card, GameObject newPosition)
    {
        Debug.Log("FromColumnToPositionGoalCard");
        Card scriptCard = card.GetComponent<Card>();
        Card scriptBelowCard = newPosition.GetComponent<Card>();
        if (scriptCard.Value == scriptBelowCard.Value + 1 && scriptCard.Seed.Equals(scriptBelowCard.Seed) && scriptCard.ChildCard==null)//check value, seed and if the card have child
        {
            //Save the state of card before change it
            Card beforeCardStatus = scriptCard.CopyCardClass();
            //remove from column and add to the positionGoal
            scriptCard.Column.RemoveCard(card);
            scriptBelowCard.PositionGoal.AddCard(card);
            //setup the position
            Vector3 newPos = newPosition.transform.position;
            newPos.z = card.transform.position.z;
            AddNewCardMovement(card, newPos);
            //Modify score and moves
            ScoreAndMoves(10);
            //Save the move
            Card afterCardStatus = scriptCard.CopyCardClass();
            BackManager.instance.SaveMove(card, afterCardStatus, beforeCardStatus, 10);
        }
        else
            scriptCard.SetOldOriginalPosition();
    }

    /// <summary>
    /// User takes a card from Column and put it in a empty Column
    /// </summary>
    private void FromColumnToColumn(GameObject card, GameObject newPosition)
    {
        Debug.Log("FromColumnToColumn");
        Card scriptCard = card.GetComponent<Card>();
        if (scriptCard.Value == 13 && newPosition.GetComponent<Column>().NumberOfCard()==0)
        {
            //Save the state of card before change it
            Card beforeCardStatus = scriptCard.CopyCardClass();
            //Remove card frm the old column and put in the new empty column
            scriptCard.Column.RemoveCard(card);
            newPosition.GetComponent<Column>().AddSingleCard(card);
            //Set new position
            AddNewCardMovement(card, newPosition.transform.position);
            //Set child card
            if (scriptCard.ChildCard != null)
            {
                FixedChildCards(card, scriptCard.ChildCard);
            }
            //Modify score and moves
            ScoreAndMoves(0);
            //Save the move
            Card afterCardStatus = scriptCard.CopyCardClass();
            BackManager.instance.SaveMove(card, afterCardStatus, beforeCardStatus, 0);
        }
        else
            scriptCard.SetOldOriginalPosition();
    }

    /// <summary>
    /// User takes a card from Column and put it above a card that is in a
    /// Column
    /// </summary>
    private void FromColumnToColumnCard(GameObject card, GameObject newPosition)
    {
        Debug.Log("FromColumnToColumnCard");
        Card scriptCard = card.GetComponent<Card>();
        Card scriptBelowCard = newPosition.GetComponent<Card>();

        if (scriptCard.Value == scriptBelowCard.Value - 1 && CheckSeedInColumn(scriptCard.Seed, scriptBelowCard.Seed) && scriptBelowCard.ChildCard == null && scriptBelowCard.GetRotateCard() == 1)//check value, seed and that card below doesn't have child and it isn't hide
        {
            //Save the state of card before change it
            Card beforeCardStatus = scriptCard.CopyCardClass();
            //Remove card frm the old column and put in the new empty column
            scriptCard.Column.RemoveCard(card);
            scriptBelowCard.Column.AddSingleCard(card);
            //Set Position
            Vector3 fatherPos = newPosition.transform.position;
            fatherPos.y -= GameManager.instance.VerticalSpaceBetweenCard;
            fatherPos.z = -scriptCard.Column.NumberOfCard();
            AddNewCardMovement(card, fatherPos);
            //set parent relation
            scriptCard.FatherCard = newPosition;
            scriptBelowCard.ChildCard = card;
            //Set child card
            if (scriptCard.ChildCard != null)
            {
                FixedChildCards(card, scriptCard.ChildCard);
            }
            //Modify score and moves
            ScoreAndMoves(0);
            //Save the move
            Card afterCardStatus = scriptCard.CopyCardClass();
            BackManager.instance.SaveMove(card, afterCardStatus, beforeCardStatus, 0);
        }
        else
            scriptCard.SetOldOriginalPosition();
    }
    /// <summary>
    /// Method that modify the child when user move a list of cards between 2 column
    /// remove the child from the old column and add them to the new
    /// </summary>
    /// <param name="father"></param>
    /// <param name="child"></param>
    private void FixedChildCards(GameObject father, GameObject child)
    {
        Card scriptFather = father.GetComponent<Card>();
        Card scriptChild = child.GetComponent<Card>();
        //add the child cards in the new column
        scriptFather.Column.AddSingleCard(child);
        //check if the card has a child card
        if (scriptChild.ChildCard != null)
            FixedChildCards(child, scriptChild.ChildCard);

    }
    /// <summary>
    /// Method that modify only the position of the child when user move a list 
    /// of cards between 2 column
    /// </summary>
    /// <param name="father"></param>
    /// <param name="child"></param>
    private void FixedChildCardsMovements(GameObject father, GameObject child)
    {
        Card scriptChild = child.GetComponent<Card>();
        //set the new position
        Vector3 newPos = father.transform.position;
        newPos.z -= 1;
        newPos.y -= GameManager.instance.VerticalSpaceBetweenCard;
        scriptChild.SetNewOriginalPosition(newPos);
        //set the movement of child cards
        if (scriptChild.ChildCard != null)
            FixedChildCardsMovements(child, scriptChild.ChildCard);
    }

    /// <summary>
    /// User takes a card from PositionGoal and put it in a empty Column
    /// </summary>
    private void FromPositionGoalToColumn(GameObject card, GameObject newPosition)
    {
        Card scriptCard = card.GetComponent<Card>();
        Card scriptBelowCard = newPosition.GetComponent<Card>();
        if (scriptCard.Value == 13)
        {
            //Save the state of card before change it
            Card beforeCardStatus = scriptCard.CopyCardClass();
            //Remove card from positionGoam and add it to the column
            scriptCard.PositionGoal.RemoveCard();
            newPosition.GetComponent<Column>().AddSingleCard(card);
            //Set new position
            AddNewCardMovement(card, newPosition.transform.position);
            //Modify score and moves
            ScoreAndMoves(-15);
            //Save the move
            Card afterCardStatus = scriptCard.CopyCardClass();
            BackManager.instance.SaveMove(card, afterCardStatus, beforeCardStatus, -15);
        }
        else
            scriptCard.SetOldOriginalPosition();
    }

    /// <summary>
    /// User takes a card from PositionGoal and put it above a card that is in a
    /// Column
    /// </summary>
    private void FromPositionGoalToColumnCard(GameObject card, GameObject newPosition)
    {
        Debug.Log("FromPositionGoalToColumnCard");
        Card scriptCard = card.GetComponent<Card>();
        Card scriptBelowCard = newPosition.GetComponent<Card>();

        if (scriptCard.Value == scriptBelowCard.Value - 1 && CheckSeedInColumn(scriptCard.Seed, scriptBelowCard.Seed) && scriptBelowCard.ChildCard == null && scriptBelowCard.GetRotateCard() == 1)//check value, seed and that card below doesn't have child and it isn't hide
        {
            //Save the state of card before change it
            Card beforeCardStatus = scriptCard.CopyCardClass();
            //remove from the positionGoal and add it to the column
            scriptCard.PositionGoal.RemoveCard();
            scriptBelowCard.Column.AddSingleCard(card);
            //setup the position
            Vector3 fatherPos = newPosition.transform.position;
            fatherPos.y -= GameManager.instance.VerticalSpaceBetweenCard;
            fatherPos.z = -scriptCard.Column.NumberOfCard();
            AddNewCardMovement(card, fatherPos);
            //set parent relation
            scriptCard.FatherCard = newPosition;
            scriptBelowCard.ChildCard = card;
            //Modify score and moves
            ScoreAndMoves(-15);
            //Save the move
            Card afterCardStatus = scriptCard.CopyCardClass();
            BackManager.instance.SaveMove(card, afterCardStatus, beforeCardStatus, -15);
        }
        else
            scriptCard.SetOldOriginalPosition();
    }

    /// <summary>
    /// User takes a card from Deck and put it above a card that is in a
    /// PositionGoal
    /// </summary>
    private void FromDeckToPositionGoalCard(GameObject card, GameObject newPosition)
    {
        Debug.Log("FromDeckToPositionGoalCard");
        Card scriptCard = card.GetComponent<Card>();
        Card scriptBelowCard = newPosition.GetComponent<Card>();
        if (scriptCard.Value == scriptBelowCard.Value + 1 && scriptCard.Seed.Equals(scriptBelowCard.Seed))//check value and seed
        {
            //Save the state of card before change it
            Card beforeCardStatus = scriptCard.CopyCardClass();
            //remove from deck and add it to the column
            scriptCard.Deck.RemoveLastCardFromWaste();
            scriptBelowCard.PositionGoal.AddCard(card);
            //setup the position
            Vector3 newPos = newPosition.transform.position;
            newPos.z = card.transform.position.z;
            AddNewCardMovement(card, newPos);
            //Modify score and moves
            ScoreAndMoves(10);
            //Save the move
            Card afterCardStatus = scriptCard.CopyCardClass();
            BackManager.instance.SaveMove(card, afterCardStatus, beforeCardStatus, 10);
        }
        else
            scriptCard.SetOldOriginalPosition();
    }
    /// <summary>
    /// User takes a card from Deck and put it above a card that is in a
    /// Column
    /// </summary>
    private void FromDeckToColumnCard(GameObject card, GameObject newPosition)
    {
        Debug.Log("FromDeckToColumnCard");
        Card scriptCard = card.GetComponent<Card>();
        Card scriptBelowCard = newPosition.GetComponent<Card>();
        if(scriptCard.Value == scriptBelowCard.Value - 1 && CheckSeedInColumn(scriptCard.Seed,scriptBelowCard.Seed) && scriptBelowCard.ChildCard==null && scriptBelowCard.GetRotateCard() == 1)//check value, seed and that card below doesn't have child and it isn't hide
        {
            //Save the state of card before change it
            Card beforeCardStatus = scriptCard.CopyCardClass();
            //remove from deck and add it to the column
            scriptCard.Deck.RemoveLastCardFromWaste();
            scriptBelowCard.Column.AddSingleCard(card);
            //setup the position
            Vector3 fatherPos = newPosition.transform.position;
            fatherPos.y -= GameManager.instance.VerticalSpaceBetweenCard;
            fatherPos.z = -scriptCard.Column.NumberOfCard();
            AddNewCardMovement(card, fatherPos);
            //set parent relation
            scriptCard.FatherCard = newPosition;
            scriptBelowCard.ChildCard = card;
            //Modify score and moves
            ScoreAndMoves(5);
            //Save the move
            Card afterCardStatus = scriptCard.CopyCardClass();
            BackManager.instance.SaveMove(card, afterCardStatus, beforeCardStatus, 5);
        }
        else
            scriptCard.SetOldOriginalPosition();
    }

    /// <summary>
    /// User takes a card from Deck and put it in a empty PositionGoal
    /// </summary>
    private void FromDeckToPositionGoal(GameObject card, GameObject newPosition)
    {
        Debug.Log("FromDeckToPositionGoal");
        Card scriptCard = card.GetComponent<Card>();
        PositionGoalCard scriptPosGoal = newPosition.GetComponent<PositionGoalCard>();
        if (scriptCard.Value == 1 && scriptPosGoal.Seed.Equals(scriptCard.Seed))
        {
            //Save the state of card before change it
            Card beforeCardStatus = scriptCard.CopyCardClass();
            //Remove the card from the deck and add it to the positionalGoal
            scriptCard.Deck.RemoveLastCardFromWaste();
            newPosition.GetComponent<PositionGoalCard>().AddCard(card);
            //Set the new position
            AddNewCardMovement(card,newPosition.transform.position);
            //Modify score and moves
            ScoreAndMoves(10);
            //Save the move
            Card afterCardStatus = scriptCard.CopyCardClass();
            BackManager.instance.SaveMove(card, afterCardStatus, beforeCardStatus, 10);
        }
        else
            scriptCard.SetOldOriginalPosition();
    }

    /// <summary>
    /// User takes a card from Deck and put it in a empty Column
    /// </summary>
    private void FromDeckToColumn(GameObject card, GameObject newPosition)
    {
        Debug.Log("FromDeckToColumn");
        Card scriptCard = card.GetComponent<Card>();
        if (scriptCard.Value == 13)
        {
            //Save the state of card before change it
            Card beforeCardStatus = scriptCard.CopyCardClass();
            //Remove the card from the deck and add it to the empty column
            scriptCard.Deck.RemoveLastCardFromWaste();
            newPosition.GetComponent<Column>().AddSingleCard(card);
            //set the new position
            AddNewCardMovement(card, newPosition.transform.position);
            //Modify score and moves
            ScoreAndMoves(5);
            //Save the move
            Card afterCardStatus = scriptCard.CopyCardClass();
            BackManager.instance.SaveMove(card, afterCardStatus, beforeCardStatus, 5);
        }
        else
            scriptCard.SetOldOriginalPosition();
    }

    /// <summary>
    /// Check if 2 seed can be connected in column (in order to have a 
    /// sequence of red-black-red-...)
    /// </summary>
    /// <param name="s1">Seed of first card</param>
    /// <param name="s2">Seed of second card</param>
    private bool CheckSeedInColumn(String s1,String s2)
    {
        if ((s1.Equals("H") || s1.Equals("D")) && (s2.Equals("C") || s2.Equals("S")))
            return true;
        if ((s2.Equals("H") || s2.Equals("D")) && (s1.Equals("C") || s1.Equals("S")))
            return true;
        return false;
    }

    /// <summary>
    /// Increment score and moves
    /// </summary>
    /// <param name="points">value that are sum or decrement to the score</param>
    private void ScoreAndMoves(int points)
    {
        ScoreManager.instance.AddScore(points);
        ScoreManager.instance.IncrementMoves();
    }

    private void AddNewCardMovement(GameObject card,Vector3 point)
    {
        cardsInMovement.Add(card);
        finalPositions.Add(point);
        if(GameManager.instance.RunningGame)
            CheckAutomaticFinish();
    }

    /// <summary>
    /// After every move check if all the card not in positionalGoal
    /// are order in 4 column and, if it is so, put them automatically in the
    /// positionGoal
    /// </summary>
    private void CheckAutomaticFinish()
    {
        if(Deck.GetComponent<Deck>().NumberOfCardInDeck()==0)
        {//deck is empty, check if we have only 4 column not empty and without hide card
            int countNotEmptyCol = 0;
            foreach (GameObject column in columns)
            {
                Column scriptCol = column.GetComponent<Column>();
                if (scriptCol.CheckHideCard())
                    return;
                if (scriptCol.NumberOfCard() > 0)
                    countNotEmptyCol++;
            }
            if (countNotEmptyCol <= 4)
            {
                GameManager.instance.StopGame();
                IEnumerator courutine = AutomaticFinish();
                StartCoroutine(courutine);
            }
        }
    }
    IEnumerator AutomaticFinish()
    {
        Debug.Log("AutomaticFinish");
        bool SetCardInPositionGoal = true;
        while (SetCardInPositionGoal)
        {
            SetCardInPositionGoal = false;
            foreach (GameObject column in columns)
            {
                Column scriptCol = column.GetComponent<Column>();
                if(scriptCol.NumberOfCard()>0)
                {
                    Debug.Log("Set Finish Card");
                    yield return new WaitForSeconds(0.15f);
                    Card lastCard = scriptCol.GetLastCard();
                    DoubleTapManage(lastCard.gameObject);
                    SetCardInPositionGoal = true;
                    
                }
            }
        }
        GameManager.instance.StartGame();//active again the game
    }

    
}
