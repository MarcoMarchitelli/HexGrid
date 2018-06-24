using UnityEngine;
using UnityEngine.UI;

public class FightSliderController : MonoBehaviour {

    public Image blueRayImage;

    public void SetBlueRayFill(float sliderValue)
    {
        float sliderValuePercent = GetSliderValuePercent(sliderValue);
        blueRayImage.fillAmount = 1-sliderValuePercent;
    }
	
    public float GetSliderValuePercent(float _sliderValue)
    {
        float sliderLenght = CombatManager.instance.maxValue + Mathf.Abs(CombatManager.instance.minValue);
        float percent = Mathf.Abs(_sliderValue) * 100 / sliderLenght;

        return percent/100; 
    }

}
