using System.Collections;
using UnityEngine;

public class SmoothMoveAnimation : MonoBehaviour {

    public Transform TargetPosition;
    public float speed;

	public IEnumerator Animation()
    {
        if (gameObject.isStatic)
        {
            gameObject.isStatic = false;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.isStatic = false;
            }
        }            
        Vector3 target = TargetPosition.position;
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }

}
