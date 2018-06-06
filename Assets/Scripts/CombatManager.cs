using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class CombatManager : MonoBehaviour
{
    #region public things

    [Header("Values")]
    public float baseStrength = .15f;
    public float timerMultiplier = .5f;
    public float maxValue = 1f;
    public float minValue = -1f;

    [Header("Inputs")]
    public KeyCode attackerInput = KeyCode.Alpha1;
    public KeyCode defenderInput = KeyCode.Alpha0;

    public UnityEvent OnFightFinish;
    public UnityEvent OnFightStart;
    public UnityEvent OnModifiersSelected;

    [Header("UI Stuff")]
    public GameObject SelectionPanel;
    public GameObject FightPanel;
    public Slider slider;
    public Animator CountdownAnimator;
    public TextMeshProUGUI CountdownText;

    [Header("Scripts")]
    public ModifierButtonsController attackerModController;
    public ModifierButtonsController defenderModController;

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
    }

    IEnumerator ModifiersSelection()
    {

        print("Attacker select your bonus!");

        attackerModController.ToggleModifierButtons(attacker);
        defenderModController.ToggleModifierButtons(defender);

        while (!attackerModSet)
        {
            yield return null;
        }

        print("Attacker added " + attackerBonusStrength + " bonus strength");
        print("Defender select your bonus!");       

        while (!defenderModSet)
        {
            yield return null;
        }

        print("Defender added " + defenderBonusStrength + " bonus strength");

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
            CountdownAnimator.SetTrigger("Show");
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator ButtonMashFight()
    {
        float startAttStr = baseStrength + attackerBonusStrength;
        float startDefStr = baseStrength + defenderBonusStrength;

        float currentAttStr = 0f, currentDefStr = 0f;

        float timer = 0f;

        while (fightSliderValue > minValue && fightSliderValue < maxValue)
        {
            timer += Time.deltaTime;

            currentAttStr = startAttStr * (timer * timerMultiplier);
            currentDefStr = startDefStr * (timer * timerMultiplier);

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
        }
        else if (fightSliderValue >= maxValue)
        {
            //attacker won.
            print("ATTACKER WON!");
            attacker.victoryPoints++;
            if(defender.victoryPoints > 0)
                defender.victoryPoints--;
        }

        FightPanel.SetActive(false);

        GameManager.instance.ConfirmAction();
    }

    public void SetAttackerModifier(int energy)
    {
        attackerModController.EnableModifierButtons(true);
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
        attacker.energyPoints -= energy;
        attackerModSet = true;
        attackerModController.EnableModifierButtons(false);
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
        defender.energyPoints -= energy;
        defenderModSet = true;
        defenderModController.EnableModifierButtons(false);
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

}
