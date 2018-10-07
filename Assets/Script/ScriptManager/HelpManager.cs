using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpManager : TouchManager {

    [Header("Deck")]
    [SerializeField]
    private GameObject DeckObject;

    [Header("Position")]
    [SerializeField]
    private GameObject PositionGoalObject;

    [Header("TableColumns")]
    [SerializeField]
    private GameObject TableObject;
    private List<GameObject> columns;

    [Header("Prefab of fake card")]
    [SerializeField]
    private GameObject FakeCard;

    [Header("Dealer Script")]
    [SerializeField]
    private Dealer Dealer;

    [Header("Warning Text")]
    [SerializeField]
    private GameObject NotFoundMove;



    private GameObject helpObject;
    
    /// <summary>
    /// Save here the card that user can move
    /// </summary>
    private Card helpCardMove;

    /// <summary>
    /// Save here the position that helpCardMove must reach
    /// </summary>
    private Vector3 helpFinalPosition;

    private bool checkMove;
    private bool checkHelpPressed;

    // Use this for initialization
    void Start () {
        checkMove = false;
        checkHelpPressed = true;
        helpObject = new GameObject();
        helpObject.transform.position = Vector3.forward * -80;
        helpObject.name = "HelpObject";
        NotFoundMove.SetActive(false);
        FindColumns();
    }
	
	// Update is called once per frame
	void Update () {
        CheckTouchOnObject();
        if (checkMove)
        {
            if (helpObject.transform.position.Equals(helpFinalPosition))
            {
                Debug.Log("Arrived");
                helpObject.SetActive(false);
                foreach (Transform child in helpObject.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
                checkMove = false;
                checkHelpPressed = true;
            }
            else
            {
                Debug.Log("Not Arrived");
                helpObject.transform.position = Vector3.MoveTowards(helpObject.transform.position, helpFinalPosition, GameManager.instance.CardHelpMovementSpeed);
            }
        }
    }

    protected override void SpritePressedBegan()
    {
        if (gameObject.GetComponent<Collider2D>() == Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position)) && checkHelpPressed)
        {
            Debug.Log("HELP!!!!");
            checkHelpPressed = false;
            HelpUser();
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

    private void HelpUser()
    {
        bool checkHelpFound = false;
        if (!checkHelpFound)
        {
            Debug.Log("CheckFromDeckToPositionGoal");
            checkHelpFound = CheckFromDeckToPositionGoal();
        }
        if (!checkHelpFound)
        {
            Debug.Log("CheckFromColumnToPositionGoal");
            checkHelpFound = CheckFromColumnToPositionGoal();
        }
        if (!checkHelpFound)
        {
            Debug.Log("CheckFromDeckToColumn");
            checkHelpFound = CheckFromDeckToColumn();
        }
        if (!checkHelpFound)
        {
            Debug.Log("CheckFromColumnToColumn");
            checkHelpFound = CheckFromColumnToColumn();
        }
        if(checkHelpFound)
        {
            Debug.Log("Found");
            CreateFakeCard();
        }
        else
        {
            
            checkHelpFound = CheckFromDeckToDeck();
            if (checkHelpFound)
            {
                Debug.Log("Found - CheckFromDeckToDeck");
                CreateFakeCardHide();
            }
            else
            {
                checkHelpPressed = true;
                Debug.Log("Not Found");
                NotFoundMove.SetActive(true);
                IEnumerator courutine = DisappearText();
                StartCoroutine(courutine);
            }
        }
    }

    IEnumerator DisappearText()
    {
        yield return new WaitForSeconds(2);
        NotFoundMove.SetActive(false);
    }

    /// <summary>
    /// Check if there are some card that can be moved from deck to a positionGoal
    /// </summary>
    private bool CheckFromDeckToPositionGoal()
    {
        
        Deck scriptDeck = DeckObject.GetComponent<Deck>();
        Card scriptCard = scriptDeck.GetActiveCardFromWaste();
        if (scriptCard != null)
        {
            PositionGoalCard positionGoal = GetExactPositionGoal(scriptCard.Seed);
            Card lastCard = positionGoal.GetLastCard();
            if ((lastCard != null && scriptCard.Value == lastCard.Value + 1) || (lastCard == null && scriptCard.Value == 1))
            {
                helpCardMove = scriptCard;
                helpFinalPosition = positionGoal.transform.position;
                return true;
            }
           
        }
        return false;
    }

    /// <summary>
    /// Check if there are some card that can be moved from column to positionGoal
    /// </summary>
    private bool CheckFromColumnToPositionGoal()
    {
        foreach (GameObject col in columns)
        {
            Column scriptCol = col.GetComponent<Column>();
            Card lastCardCol = scriptCol.GetLastCard();
            if (lastCardCol != null)
            {
                PositionGoalCard positionGoal = GetExactPositionGoal(lastCardCol.Seed);
                Card lastCardPos = positionGoal.GetLastCard();
                if ((lastCardPos != null && lastCardCol.Value == lastCardPos.Value + 1) || (lastCardPos == null && lastCardCol.Value == 1))
                {
                    helpCardMove = lastCardCol;
                    helpFinalPosition = positionGoal.transform.position;
                    return true;
                }
            }
        }
        return false;
    }

    private PositionGoalCard GetExactPositionGoal(string seed)
    {
        PositionGoalCard positionGoal = null;
        switch (seed)
        {
            case "H":
                positionGoal = PositionGoalObject.transform.Find("PositionHearts").gameObject.GetComponent<PositionGoalCard>();
                break;
            case "D":
                positionGoal = PositionGoalObject.transform.Find("PositionDiamonds").gameObject.GetComponent<PositionGoalCard>();
                break;
            case "C":
                positionGoal = PositionGoalObject.transform.Find("PositionClubs").gameObject.GetComponent<PositionGoalCard>();
                break;
            case "S":
                positionGoal = PositionGoalObject.transform.Find("PositionSpades").gameObject.GetComponent<PositionGoalCard>();
                break;
        }
        return positionGoal;
    }

    /// <summary>
    /// Check if there are some card that can be moved from deck to a column
    /// </summary>
    private bool CheckFromDeckToColumn()
    {
        Deck scriptDeck = DeckObject.GetComponent<Deck>();
        Card lastCardDeck = scriptDeck.GetActiveCardFromWaste();
        if (lastCardDeck != null)
        {
            foreach (GameObject col in columns)
            {
                Column scriptCol = col.GetComponent<Column>();
                Card lastCardCol = scriptCol.GetLastCard();
                if (lastCardCol != null && lastCardCol.Value == lastCardDeck.Value + 1 && CheckSeedInColumn(lastCardCol.Seed,lastCardDeck.Seed))
                {
                    helpCardMove = lastCardDeck;
                    helpFinalPosition = lastCardCol.transform.position;
                    return true;
                }
                if((lastCardCol == null && lastCardDeck.Value == 13))
                {
                    helpCardMove = lastCardDeck;
                    helpFinalPosition = col.transform.position;
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Check if there are some card that can be moved between 2 columns
    /// </summary>
    private bool CheckFromColumnToColumn()
    {
        foreach (GameObject col1 in columns)
        {
            foreach (GameObject col2 in columns)
            {
                Card lastCardCol = col2.GetComponent<Column>().GetLastCard();
                List<Card> cards = col1.GetComponent<Column>().GetAllNotHideCards();
                foreach(Card card in cards)
                {
                    if (card.FatherCard == null)
                    {
                        if (lastCardCol != null && lastCardCol.Value == card.Value + 1 && CheckSeedInColumn(lastCardCol.Seed, card.Seed))
                        {
                            helpCardMove = card;
                            helpFinalPosition = lastCardCol.transform.position;
                            return true;
                        }
                        if ((lastCardCol == null && card.Value == 13 && col1.GetComponent<Column>().CheckHideCard()))
                        {
                            helpCardMove = card;
                            helpFinalPosition = col2.transform.position;
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Check in the entire deck if there are some card that can be moved from deck
    /// to any place
    /// </summary>
    private bool CheckFromDeckToDeck()
    {
        Debug.Log("CheckFromDeckToDeck");
        Deck scriptDeck = DeckObject.GetComponent<Deck>();
        List<GameObject> deckCards = scriptDeck.GetAllCardsFromDeck();
        if (deckCards != null)
        {
            Debug.Log("deckCards count="+deckCards.Count);
            //check if I can put some card in positionGoal
            foreach (GameObject card in deckCards)
            {
                Card scriptCard = card.GetComponent<Card>();
                PositionGoalCard positionGoal = GetExactPositionGoal(scriptCard.Seed);
                Card positionGoalCard = positionGoal.GetComponent<PositionGoalCard>().GetLastCard();
                if ((positionGoalCard != null && scriptCard.Value == positionGoalCard.Value + 1) || (positionGoalCard == null && scriptCard.Value == 1))
                {
                    return true;
                }
            }
            //check if I can put some card in column
            foreach (GameObject card in deckCards)
            {
                foreach (GameObject col in columns)
                {
                    Card scriptCardDeck = card.GetComponent<Card>();
                    Column scriptCol = col.GetComponent<Column>();
                    Card lastCardCol = scriptCol.GetLastCard();
                    if (lastCardCol != null && lastCardCol.Value == scriptCardDeck.Value + 1 && CheckSeedInColumn(lastCardCol.Seed, scriptCardDeck.Seed))
                    {
                        return true;
                    }
                    if ((lastCardCol == null && scriptCardDeck.Value == 13))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Create the fake cards to show to the user the move he can do
    /// </summary>
    private void CreateFakeCard()
    {
        Vector3 posHelp = helpCardMove.transform.position;
        posHelp.z = -70;
        helpObject.transform.position = posHelp;
        GameObject cardHelp = Instantiate(FakeCard) as GameObject;
        cardHelp.transform.position = DeckObject.transform.position;
        cardHelp.transform.Find("Face/BigImage").GetComponent<SpriteRenderer>().sprite = helpCardMove.transform.Find("Face/BigImage").GetComponent<SpriteRenderer>().sprite;
        cardHelp.transform.Find("Face/LittleImage").GetComponent<SpriteRenderer>().sprite = helpCardMove.transform.Find("Face/LittleImage").GetComponent<SpriteRenderer>().sprite;
        cardHelp.transform.Find("Face/Value").GetComponent<SpriteRenderer>().sprite = helpCardMove.transform.Find("Face/Value").GetComponent<SpriteRenderer>().sprite;
        cardHelp.transform.SetParent(helpObject.transform);
        cardHelp.transform.localPosition = Vector3.zero;
        if (helpCardMove.GetComponent<Card>().ChildCard != null)
            CreateFakeCardChild(helpCardMove.gameObject, helpCardMove.GetComponent<Card>().ChildCard,-1);
        helpObject.SetActive(true);
        checkMove = true;
    }

    private void CreateFakeCardChild(GameObject father,GameObject child,float Z)
    {
        Vector3 positionFather = Vector3.zero;
        positionFather.y += GameManager.instance.VerticalSpaceBetweenCard*Z;
        positionFather.z = Z;
        Debug.Log("Father Position=" + positionFather);
        GameObject cardHelp = Instantiate(FakeCard) as GameObject;
        cardHelp.transform.Find("Face/BigImage").GetComponent<SpriteRenderer>().sprite = child.transform.Find("Face/BigImage").GetComponent<SpriteRenderer>().sprite;
        cardHelp.transform.Find("Face/LittleImage").GetComponent<SpriteRenderer>().sprite = child.transform.Find("Face/LittleImage").GetComponent<SpriteRenderer>().sprite;
        cardHelp.transform.Find("Face/Value").GetComponent<SpriteRenderer>().sprite = child.transform.Find("Face/Value").GetComponent<SpriteRenderer>().sprite;
        cardHelp.transform.SetParent(helpObject.transform);
        cardHelp.transform.localPosition = positionFather;
        if (child.GetComponent<Card>().ChildCard != null)
            CreateFakeCardChild(child, child.GetComponent<Card>().ChildCard,Z-1);
    }

    /// <summary>
    /// Create the fake cards to show to the user the move he can do when
    /// the only move is push on deck
    /// </summary>
    private void CreateFakeCardHide()
    {
        Vector3 posHelp = DeckObject.transform.position;
        posHelp.z = -70;
        helpObject.transform.position = posHelp;
        GameObject cardHelp = Instantiate(FakeCard) as GameObject;
        cardHelp.transform.position = DeckObject.transform.position;
        cardHelp.transform.Find("Back").gameObject.SetActive(true);
        cardHelp.transform.SetParent(helpObject.transform);
        cardHelp.transform.localPosition = Vector3.zero;
        helpObject.SetActive(true);
        helpFinalPosition=DeckObject.GetComponent<Deck>().getWaste.transform.position;
        checkMove = true;
    }

    /// <summary>
    /// Check if 2 seed can be connected in column (in order to have a 
    /// sequence of red-black-red-...)
    /// </summary>
    /// <param name="s1">Seed of first card</param>
    /// <param name="s2">Seed of second card</param>
    private bool CheckSeedInColumn(string s1, string s2)
    {
        if ((s1.Equals("H") || s1.Equals("D")) && (s2.Equals("C") || s2.Equals("S")))
            return true;
        if ((s2.Equals("H") || s2.Equals("D")) && (s1.Equals("C") || s1.Equals("S")))
            return true;
        return false;
    }
}
