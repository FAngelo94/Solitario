using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class MovementCard:MonoBehaviour {
   

    public void TranslateCard (GameObject card, Vector2 point)
    {
        float speed = GameManager.instance.CardMovementSpeed;
        Vector2 cardPos = card.transform.position;
        card.transform.position = Vector2.MoveTowards(cardPos, point, speed * Time.deltaTime);

    }

    public void RotateCard (GameObject card)
    {
        Animator cardAnimator = card.GetComponent<Animator>();
        bool front=cardAnimator.GetBool("Front");
        cardAnimator.SetBool("Front", !front);
    }
}
