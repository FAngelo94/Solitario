﻿using System.Collections;
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
    private List<GameObject> columns;

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
    private int[] values = { 3, 3, 3, 4, 5, 6, 7, 1, 2, 10, 11, 12, 13 };

    private bool setCardPosition;
    private bool finishSetUp;

    // Use this for initialization
    private void Start () {
        setCardPosition = true;
        finishSetUp = false;
        SetUpGame();

        /*FindColumns();
        GameObject newCard = CreateCard("H", 1);
        PutCardInColumn(newCard, 0);*/
    }

    private void Update()
    {
        if (setCardPosition && !finishSetUp)
        {
            bool needMove = false;
            foreach (GameObject column in columns)
            {
                Column script = column.GetComponent<Column>();
                if (script.SetUpCards())
                    needMove = true;
            }
            setCardPosition = !finishSetUp?true:needMove;
            
        }
    }

    private void SetUpGame()
    {
        FindColumns();
        //ShuffleLists();
        IEnumerator courutine = SetUpCourutine();
        StartCoroutine(courutine);
        
    }

    IEnumerator SetUpCourutine()
    {
        int count = 0;
        int column = 0;
        foreach (int value in values)
        {
            foreach (string seed in seeds) 
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
                    yield return new WaitForSeconds(0.2f);
                }
                else
                     PutCardInDeck(newCard);
            }
        }
        finishSetUp = true;
        foreach (GameObject col in columns)
        {
            Column script = col.GetComponent<Column>();
            script.RotateLastCard();
        }
        yield return null;
    }

    private void FindColumns()
    {
        columns = new List<GameObject>();
        for(int i = 0; i < 7; i++)
        {
            string nameColumn = "Column_" + i;
            columns.Add(TableObject.transform.Find(nameColumn).gameObject);
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
        Column script = columns[col].GetComponent<Column>();
        script.AddSingleCard(newCard);
    }
    
    private void PutCardInDeck(GameObject newCard)
    {
        Deck script = DeckObject.GetComponent<Deck>();
        script.AddSingleCard(newCard);
    }
}