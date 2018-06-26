using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class FightSliderController : MonoBehaviour
{

    public Slider FightSlider;

    [Header("Rays Images")]
    public Image defRayImage;
    public Image atkRayImage;

    [Header("Animators")]
    public Animator defRayAnimator;
    public Animator atkRayAnimator;

    public UnityEvent OnRaysAnimationEnd;

    bool inFight = false;

    public void InFight(bool flag)
    {
        inFight = flag;
    }

    public void SetDefRayFill(float sliderValue)
    {
        if (inFight)
        {
            float sliderValuePercent = GetSliderValuePercent(sliderValue);
            defRayImage.fillAmount = 1 - sliderValuePercent;
        }
    }

    public float GetSliderValuePercent(float _sliderValue)
    {
        float sliderLenght = CombatManager.instance.maxValue + Mathf.Abs(CombatManager.instance.minValue);
        float percent = Mathf.Abs(_sliderValue) * 100 / sliderLenght;

        return percent / 100;
    }

    public void StartRaysAnimation(float duration)
    {
        StartCoroutine(RaysAnimation(duration));
    }

    IEnumerator RaysAnimation(float duration)
    {
        float timer = 0;
        float atkRayTarget = GetSliderValuePercent(FightSlider.value);
        float defRayTarget = GetSliderValuePercent(FightSlider.value);
        float atkRayStart = 0.06f;
        float defRayStart = 0.06f;

        atkRayAnimator.SetTrigger("Start");
        defRayAnimator.SetTrigger("Start");

        while (timer <= duration)
        {
            timer += Time.deltaTime / duration;
            atkRayImage.fillAmount = Mathf.Lerp(atkRayStart, atkRayTarget, timer);
            defRayImage.fillAmount = Mathf.Lerp(defRayStart, defRayTarget, timer);
            yield return null;
        }

        atkRayImage.fillAmount = 1;
        InFight(true);
        OnRaysAnimationEnd.Invoke();
    }

}
