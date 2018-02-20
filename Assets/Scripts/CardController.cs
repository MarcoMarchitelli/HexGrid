using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour {

    public enum State
    {
        inHand, selectedFromHand, selectedFromMap, placed
    }
    public State state = State.inHand;

    public bool isShield = true;

    void Update()
    {
        if (state == State.selectedFromHand)
        {
            Vector3 myMousePos = Input.mousePosition;
            myMousePos.z = 19f;
            transform.position = Camera.main.ScreenToWorldPoint(myMousePos);
            if (Input.GetKeyDown(KeyCode.A))
            {
                transform.Rotate(Vector3.up * -60);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                transform.Rotate(Vector3.up * 60);
            }
        }

        if (state == State.selectedFromMap)
        {
            transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
            if (Input.GetKeyDown(KeyCode.A))
            {
                transform.Rotate(Vector3.up * -60);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                transform.Rotate(Vector3.up * 60);
            }
        }
    }

    public void Place(Vector3 _position)
    {
        transform.position = _position + Vector3.up * .5f;
        state = State.placed;
    }
}