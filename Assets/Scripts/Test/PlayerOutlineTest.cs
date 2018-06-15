using UnityEngine;
using cakeslice;

public class PlayerOutlineTest : MonoBehaviour
{

    Transform[] children;

    private void Start()
    {
        children = new Transform[transform.childCount];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = transform.GetChild(i);
            children[i].gameObject.AddComponent<Outline>();
        }
        //ActivateOutline(false);
    }

    public void ActivateOutline(bool flag)
    {
        foreach (Transform child in children)
        {
            if (flag)
                child.GetComponent<Outline>().enabled = true;
            else
                child.GetComponent<Outline>().enabled = false;
        }
    }

    /// <summary>
    /// colorIndex goes from 0 to 2.
    /// </summary>
    /// <param name="colorIndex"></param>
    public void ChangeOutlineColor(int colorIndex)
    {
        if (colorIndex < 0 || colorIndex > 2)
            return;

        foreach (Transform child in children)
        {
            child.GetComponent<Outline>().color = colorIndex;
        }
    }
}
