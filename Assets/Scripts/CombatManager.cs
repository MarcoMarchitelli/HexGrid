using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class CombatManager : MonoBehaviour
{
    #region public things

    [Header("Test Values")]
    public bool Timer = false;
    public float TimerDuration;
    public float baseStrength = .15f;
    public float timerMultiplier = .5f;
    public float maxValue = 1f;
    public float minValue = -1383838338f;

    [Header("Inputs")]
    public KeyCode attackerInput = KeyCode.Alpha1;
    public KeyCode defenderInput = KeyCode.Alpha0;

    public UnityEvent OnFightFinish;
    public UnityEvent OnFightStart;
    public UnityEvent OnAttackerSelectStart;
    public UnityEvent OnDefenderSelectStart;
    public UnityEvent OnAttackerSelectEnd;
    public UnityEvent OnDefenderSelectEnd;
    public UnityEvent OnModifiersSelected;

    [Header("UI Stuff")]
    public Slider slider;
    public Animator CountdownAnimator;
    public TextMeshProUGUI InfoTextAttacker;
    public TextMeshProUGUI InfoTextDefender;
    public TextMeshProUGUI CountdownText;
    public TextMeshProUGUI TimerText;

    [Header("Scripts")]
    public ModifierButtonsController attackerModController;
    public ModifierButtonsController defenderModController;

    [Header("Audio Sources")]
    public AudioSource AttackerWin;
    public AudioSource AttackerLoss;

    #endregion

    PlayerController attacker, defender;

    float fightSliderValue = 0f;
    float attackerBonusStrength = 0f, defenderBonusStrength = 0f;
    bool attackerModSet = false, defenderModSet = false;

    private void Start()
    {
        slider.maxValue = maxValue;
        slider.minValue = minValue;
    }

    public void StartFightFlow(PlayerController _attacker, PlayerController _defender)
    {
        attacker = _attacker;
        defender = _defender;
        ResetValues();
        StartCoroutine(FightFlow());
    }

    IEnumerator FightFlow()
    {
        OnFightStart.Invoke();

        yield return StartCoroutine(ModifiersSelection());

        yield return StartCoroutine(CountDown(3));

        yield return StartCoroutine(ButtonMashFight());

        OnFightFinish.Invoke();
        attacker.hasFought = false;
        GameManager.instance.ConfirmAction();
    }

    IEnumerator ModifiersSelection()
    {
        //setup
        attackerModController.ToggleModifierButtons(attacker);
        defenderModController.ToggleModifierButtons(defender);
        attackerModController.EnableModifierButtons(false);
        defenderModController.EnableModifierButtons(false);

        yield return new WaitForSeconds(1f);

        //attacker selection
        if (InfoTextAttacker)
            InfoTextAttacker.text = "Choose your bonus!";

        attackerModController.ToggleModifierButtons(attacker);
        OnAttackerSelectStart.Invoke();

        while (!attackerModSet)
        {
            yield return null;
        }
        print("Attacker added " + attackerBonusStrength + " bonus strength");

        yield return new WaitForSeconds(.7f);

        //defender selection
        if (InfoTextDefender)
            InfoTextDefender.text = "Choose your bonus!";

        defenderModController.ToggleModifierButtons(defender);
        OnDefenderSelectStart.Invoke();

        while (!defenderModSet)
        {
            yield return null;
        }
        print("Defender added " + defenderBonusStrength + " bonus strength");

        yield return new WaitForSeconds(1f);

        //selection end
        OnModifiersSelected.Invoke();
    }

    IEnumerator CountDown(int seconds)
    {

        for (int i = seconds; i >= 0; i--)
        {
            if (i == 0)
                CountdownText.text = "GO!";
            else
                CountdownText.text = i.ToString();
            if (CountdownAnimator)
                CountdownAnimator.SetTrigger("Show");
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator ButtonMashFight()
    {
        float startAttStr = baseStrength + attackerBonusStrength;
        float startDefStr = baseStrength + defenderBonusStrength;

        float currentAttStr = 0f, currentDefStr = 0f;

        float timerForMultiplier = 0f;
        float timerForFightEnd = 0f;

        if (Timer)
        {
            timerForFightEnd = TimerDuration;
            while (fightSliderValue > minValue && fightSliderValue < maxValue && timerForFightEnd > 0)
            {
                timerForMultiplier += Time.deltaTime;
                timerForFightEnd -= Time.deltaTime;
                TimerText.text = Mathf.RoundToInt(timerForFightEnd).ToString();

                currentAttStr = startAttStr * (timerForMultiplier * timerMultiplier);
                currentDefStr = startDefStr * (timerForMultiplier * timerMultiplier);

                if (Input.GetKeyDown(attackerInput))
                {
                    fightSliderValue += currentAttStr;
                }

                if (Input.GetKeyDown(defenderInput))
                {
                    fightSliderValue -= currentDefStr;
                }

                slider.value = fightSliderValue;

                yield return null;
            }
        }
        else
            while (fightSliderValue > minValue && fightSliderValue < maxValue)
            {
                timerForMultiplier += Time.deltaTime;

                currentAttStr = startAttStr * (timerForMultiplier * timerMultiplier);
                currentDefStr = startDefStr * (timerForMultiplier * timerMultiplier);

                if (Input.GetKeyDown(attackerInput))
                {
                    fightSliderValue += currentAttStr;
                }

                if (Input.GetKeyDown(defenderInput))
                {
                    fightSliderValue -= currentDefStr;
                }

                slider.value = fightSliderValue;

                yield return null;
            }

        if (fightSliderValue <= minValue)
        {
            //defender won.
            print("DEFENDER WON!");
            if (AttackerLoss != null)
                AttackerLoss.Play();
        }
        else if (fightSliderValue >= maxValue)
        {
            //attacker won.
            print("ATTACKER WON!");
            if (AttackerWin)
                AttackerWin.Play();
            attacker.VictoryPoints++;
            if (defender.VictoryPoints > 0)
                defender.VictoryPoints--;
        }
        else if (timerForFightEnd <= 0 && fightSliderValue > 0f)
        {
            //attacker won.
            print("TIME'S OVER! ATTACKER WON!");
            attacker.VictoryPoints++;
            if (defender.VictoryPoints > 0)
                defender.VictoryPoints--;
        }
        else if (timerForFightEnd <= 0 && fightSliderValue <= 0f)
        {
            //defender won.
            print("TIME'S OVER! DEFENDER WON!");
        }

        
    }

    public void SetAttackerModifier(int energy)
    {        
        switch (energy)
        {
            case 0:
                attackerBonusStrength = 0;
                break;
            case 1:
                attackerBonusStrength = 0.025f;
                break;
            case 2:
                attackerBonusStrength = 0.05f;
                break;
            case 4:
                attackerBonusStrength = 0.1f;
                break;
            case 6:
                attackerBonusStrength = 0.15f;
                break;
            case 8:
                attackerBonusStrength = 0.20f;
                break;
            case 10:
                attackerBonusStrength = 0.25f;
                break;
            case 15:
                attackerBonusStrength = 0.375f;
                break;
        }
        attacker.EnergyPoints -= energy;
        attackerModController.EnableModifierButtons(false);
        OnAttackerSelectEnd.Invoke();
    }

    public void SetDefenderModifier(int energy)
    {
        defenderModController.EnableModifierButtons(true);
        switch (energy)
        {
            case 0:
                defenderBonusStrength = 0;
                break;
            case 1:
                defenderBonusStrength = 0.025f;
                break;
            case 2:
                defenderBonusStrength = 0.05f;
                break;
            case 4:
                defenderBonusStrength = 0.1f;
                break;
            case 6:
                defenderBonusStrength = 0.15f;
                break;
            case 8:
                defenderBonusStrength = 0.20f;
                break;
            case 10:
                defenderBonusStrength = 0.25f;
                break;
            case 15:
                defenderBonusStrength = 0.375f;
                break;
        }
        defender.EnergyPoints -= energy;
        defenderModSet = true;
        defenderModController.EnableModifierButtons(false);
        OnDefenderSelectEnd.Invoke();
    }

    public void ResetValues()
    {
        fightSliderValue = 0f;
        attackerBonusStrength = 0f;
        defenderBonusStrength = 0f;
        attackerModSet = false;
        defenderModSet = false;
        slider.value = fightSliderValue;
    }

    public void AttackerModSelected()
    {
        attackerModSet = true;
    }

    public void DefenderModSelected()
    {
        defenderModSet = true;
    }

}
