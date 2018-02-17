using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour {

    bool isPlaced = false;
    public bool isBeingMod = false;

    void Update()
    {
        if (!isPlaced)
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

        if (isBeingMod)
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
        isPlaced = true;
        isBeingMod = false;
    }
}