using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsing : MonoBehaviour {

	void Update () {

		var maxDist = 5;
		var speed = 1;

		Renderer renderer = GetComponent<Renderer> ();
		Material mat = renderer.material;

		float emission = Mathf.PingPong (Time.time * speed, maxDist)+0.5f;
		Color baseColor = Color.white; //Replace this with whatever you want for your base color at emission level '1'

		Color finalColor = baseColor * Mathf.LinearToGammaSpace (emission);

		mat.SetColor ("_EmissionColor", finalColor);
	}
}
