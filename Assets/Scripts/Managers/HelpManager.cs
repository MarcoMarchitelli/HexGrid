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

    [Header("Canvases")]
    public GraphicRaycaster[] OtherCanvases;

    [Header("Particles")]
    public GameObject HelpButtonParticle1;

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
        {
            HelpPanel.SetActive(false);
            GameManager.instance.helpOpened = false;
            HelpButtonParticle1.SetActive(false);
            Time.timeScale = 1;
            foreach (var canvas in OtherCanvases)
            {
                canvas.enabled = true;
            }
        }
        else
        {
            HelpPanel.SetActive(true);
            GameManager.instance.helpOpened = true;
            HelpButtonParticle1.SetActive(true);
            Time.timeScale = 0;
            foreach (var canvas in OtherCanvases)
            {
                canvas.enabled = false;
            }
        }

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