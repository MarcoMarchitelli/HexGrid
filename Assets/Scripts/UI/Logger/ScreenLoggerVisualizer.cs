using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScreenLoggerVisualizer : MonoBehaviour {

    public TextMeshProUGUI textArea;
	
	void Update () {
        textArea.text = CustomLogger.currentLogString;
	}
}
