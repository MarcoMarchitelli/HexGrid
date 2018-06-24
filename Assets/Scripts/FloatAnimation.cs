using UnityEngine;
using System.Collections;

public class FloatAnimation : MonoBehaviour {

    public float speed = 0.5f;
    public RectTransform peek;
    public RectTransform bottom;

    // Use this for initialization
    void Start () {
        StartCoroutine(AnimUp());
	}
	
	public IEnumerator AnimUp()
    {
        while(transform != peek)
        {
            Vector2.MoveTowards(transform.position, peek.position, speed * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(AnimDown());
    }

    public IEnumerator AnimDown()
    {
        while (transform != bottom)
        {
            Vector2.MoveTowards(transform.position, peek.position, speed * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(AnimUp());
    }
}
