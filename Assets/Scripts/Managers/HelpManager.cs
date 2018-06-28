using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HelpManager : MonoBehaviour {

    public Sprite[] HelpSlides;
    public Image SlidesVisualizer;
    public GameObject HelpPanel;

    [Header("Nav Buttons")]
    public GameObject PrevButton;
    public GameObject NextButton;

    [Header("Texts")]
    public TextMeshProUGUI CurrentSlideNumber;
    public TextMeshProUGUI MaxSlideNumber;

    int slideIndex = 0;

    public void Refresh()
    {
        //refresh slide
        SlidesVisualizer.sprite = HelpSlides[slideIndex];

        //refresh nav buttons
        if (slideIndex == 0)
            PrevButton.SetActive(false);
        else if (slideIndex == HelpSlides.Length - 1)
            NextButton.SetActive(false);
        else
        {
            PrevButton.SetActive(true);
            NextButton.SetActive(true);
        }

        CurrentSlideNumber.text = (slideIndex + 1).ToString();
        MaxSlideNumber.text = HelpSlides.Length.ToString();

    }

    public void HelpToggle()
    {
        if (HelpPanel.activeSelf)
            HelpPanel.SetActive(false);
        else
            HelpPanel.SetActive(true);

        slideIndex = 0;

        Refresh();
    }
	
    public void NextSlide()
    {
        if (slideIndex != HelpSlides.Length - 1)
            slideIndex++;

        Refresh();
    }

    public void PreviousSlide()
    {
        if (slideIndex != 0)
            slideIndex--;

        Refresh();
    }

}