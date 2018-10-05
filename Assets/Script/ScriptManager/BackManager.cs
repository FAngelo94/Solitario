using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackManager : MonoBehaviour {

    public static BackManager instance;

    private List<SaveMoves> movesSaved = new List<SaveMoves>();

    private void Start()
    {
        if (instance == null)
            instance = this;
    }

    public void BackMove()
    {

    }

    public void SaveMove(GameObject card, GameObject oldPosition, int points)
    {
        SaveMoves save = new SaveMoves
        {
            card = card,
            oldPosition = oldPosition,
            points = points
        };
        movesSaved.Add(save);
    }

    private struct SaveMoves
    {
        public GameObject card;
        public GameObject oldPosition;
        public int points;
    }	
}
