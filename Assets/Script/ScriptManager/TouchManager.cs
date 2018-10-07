using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{

    protected void CheckTouchOnObject()
    {

        if (Input.touchCount > 0 && GameManager.instance.RunningGame)
        {
            switch (Input.GetTouch(0).phase)
            {
                case TouchPhase.Began:
                    SpritePressedBegan();
                    break;
                case TouchPhase.Ended:
                    SpritePressedEnded();
                    break;
                case TouchPhase.Moved:
                    SpritePressedMoved();
                    break;
                case TouchPhase.Stationary:
                    SpritePressedStationary();
                    break;
            }
        }
    }

    protected virtual void SpritePressedBegan()
    {
        //Debug.Log(gameObject.name + " -> SpritePressedBegan");
    }
    protected virtual void SpritePressedEnded()
    {
       // Debug.Log(gameObject.name + " -> SpritePressedEnded");
    }
    protected virtual void SpritePressedMoved()
    {
        //Debug.Log(gameObject.name + " -> SpritePressedMoved");
    }
    protected virtual void SpritePressedStationary()
    {
       // Debug.Log(gameObject.name + " -> SpritePressedStationary");
    }

};
