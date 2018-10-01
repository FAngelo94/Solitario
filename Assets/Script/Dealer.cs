using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dealer : MonoBehaviour {

    [SerializeField]
    [Header("Collection of all cards")]
    private GameObject _collection;
    private GameObject Collection
    {
        get { return _collection; }
    }

    [Header("Deck")]
    [SerializeField]
    private GameObject DeckObject;

    [Header("Table")]
    [SerializeField]
    private GameObject TableObject;
    private List<GameObject> cardColumns;

    [Header("List of seeds of cards in order (H, D, C, S)")]
    [SerializeField]
    private List<Sprite> SpriteSeeds;

    [Header("List of value of cards in order")]
    [SerializeField]
    private List<Sprite> SpriteValues;

    [Header("Prefab of card")]
    [SerializeField]
    private GameObject BasicCard;

    private List<string> seeds = new List<string> { "H", "D", "C", "S" };
    private int[] values = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };

    // Use this for initialization
    private void Start () {
        SetUpGame();

        /*FindColumns();
        GameObject newCard = CreateCard("H", 1);
        PutCardInColumn(newCard, 0);*/
    }

    private void Update()
    { 
        foreach(GameObject column in cardColumns)
        {
            CardColumn script = column.GetComponent<CardColumn>();
            script.SetUpCards();
        }
    }

    private void SetUpGame()
    {
        FindColumns();
        ShuffleLists();
        int count = 0;
        int column = 0;
        foreach (string seed in seeds)
        {
            foreach (int value in values)
            {
                GameObject newCard = CreateCard(seed, value);
                if (column <= 6)
                {
                    PutCardInColumn(newCard, column);
                    count++;
                    if (count > column)
                    {
                        column++;
                        count = 0;
                    }
                }
                //else
               //     PutCardInDeck(newCard);
            }
        }
        foreach(GameObject col in cardColumns)
        {
            CardColumn script = col.GetComponent<CardColumn>();
            script.RotateLastCard();
        }
    }

    private void FindColumns()
    {
        cardColumns = new List<GameObject>();
        for(int i = 0; i < 7; i++)
        {
            string nameColumn = "Column_" + i;
            cardColumns.Add(TableObject.transform.Find(nameColumn).gameObject);
        }

    }

    private void ShuffleLists()
    {
        for(int i=0;i<seeds.Count;i++)
        {
            int R = Random.Range(0, seeds.Count);
            string tmp = seeds[i];
            seeds[i] = seeds[R];
            seeds[R] = tmp;
        }
        for (int i = 0; i < values.Length; i++)
        {
            int R = Random.Range(0, seeds.Count);
            int tmp = values[i];
            values[i] = values[R];
            values[R] = tmp;
        }
    }

    private GameObject CreateCard(string seed, int value)
    {
        //Setup GameObject
        int indexSeed = seeds.IndexOf(seed);
        GameObject newCard = Instantiate(BasicCard) as GameObject;
        newCard.transform.position = DeckObject.transform.position;
        newCard.transform.rotation = new Quaternion(0, 0, 0, 0);
        newCard.transform.Find("Face/BigImage").GetComponent<SpriteRenderer>().sprite = SpriteSeeds[indexSeed];
        newCard.transform.Find("Face/LittleImage").GetComponent<SpriteRenderer>().sprite = SpriteSeeds[indexSeed];
        newCard.transform.Find("Face/Value").GetComponent<SpriteRenderer>().sprite = SpriteValues[value-1];
        newCard.transform.SetParent(Collection.transform);
        //Setup script of GameObject
        Card cardScript = newCard.GetComponent<Card>();
        cardScript.SetCard(value, seed);
        return newCard;
    }

    private void PutCardInColumn(GameObject newCard,int col)
    {
        Debug.Log("col=" + col);
        CardColumn script = cardColumns[col].GetComponent<CardColumn>();
        script.AddSingleCard(newCard);
    }
    
    private void PutCardInDeck(GameObject newCard)
    {
        Deck script = DeckObject.GetComponent<Deck>();
        script.AddSingleCard(newCard);
    }
}
