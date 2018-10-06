using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpManager : MonoBehaviour {

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

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
}
