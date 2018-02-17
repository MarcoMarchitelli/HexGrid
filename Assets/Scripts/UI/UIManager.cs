using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {

    public TextMeshProUGUI topBigSection, leftMediumSection;

    public Transform currentInstantiatedCard;


    public void PrintTop(string msg)
    {
        topBigSection.text = msg;
    }

    public void PrintLeft(string msg)
    {
        leftMediumSection.text = msg;
    }

    public void SpawnCardCardOnClick(Transform cardToInstantiate)
    {
        Vector3 myMousePos = Input.mousePosition;
        myMousePos.y = 1f;
        currentInstantiatedCard = Instantiate(cardToInstantiate, Camera.main.ScreenToWorldPoint(myMousePos), Quaternion.identity);
    }

}
