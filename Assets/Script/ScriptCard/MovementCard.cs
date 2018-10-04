using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class MovementCard {
   

    public void TranslateCard (GameObject card, Vector3 point)
    {
        float speed = GameManager.instance.CardMovementSpeed;
        Vector3 cardPos = card.transform.position;
        card.transform.position = Vector3.MoveTowards(cardPos, point, speed * Time.deltaTime);

    }

    public void RotateCard (GameObject card)
    {
        Animator cardAnimator = card.GetComponent<Animator>();
        int rotation=cardAnimator.GetInteger("Rotation");
        if(rotation==0)
            cardAnimator.SetInteger("Rotation", 1);
        else
            cardAnimator.SetInteger("Rotation", rotation*(-1));
    }

    public void RotateFrontCard(GameObject card)
    {
        Animator cardAnimator = card.GetComponent<Animator>();
        int rotation = cardAnimator.GetInteger("Rotation");
        if (rotation != 1)
            cardAnimator.SetInteger("Rotation", 1);
    }

    public void RotateBackCard(GameObject card)
    {
        Animator cardAnimator = card.GetComponent<Animator>();
        int rotation = cardAnimator.GetInteger("Rotation");
        if (rotation != -1)
            cardAnimator.SetInteger("Rotation", -1);        
    }
}
