using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

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

    [Header("UI Stuff")]
    public GameObject SelectionPanel;
    public GameObject FightPanel;
    public Slider slider;

    [Header("Scripts")]
    public ModifierButtonsController attackerModController;
    public ModifierButtonsController defenderModController;

    #endregion

    float fightSliderValue = 0f;
    float attackerBonusStrength = 0f, defenderBonusStrength = 0f;
    bool attackerModSet = false, defenderModSet = false;

    private void Start()
    {
        slider.maxValue = maxValue;
        slider.minValue = minValue;
    }

    public void StartFightFlow(PlayerController attacker, PlayerController defender)
    {
        ResetValues();
        StartCoroutine(FightFlow(attacker, defender));
    }

    IEnumerator FightFlow(PlayerController attacker, PlayerController defender)
    {
        OnFightStart.Invoke();

        yield return StartCoroutine(ModifiersSelection(attacker, defender));

        yield return StartCoroutine(CountDown(3));

        yield return StartCoroutine(ButtonMashFight(attacker, defender));

        OnFightFinish.Invoke();
    }

    IEnumerator ModifiersSelection(PlayerController attacker, PlayerController defender)
    {
        SelectionPanel.SetActive(true);

        print("Attacker select your bonus!");

        attackerModController.ToggleModifierButtons(attacker);

        while (!attackerModSet)
        {
            yield return null;
        }

        print("Attacker added " + attackerBonusStrength + " bonus strength");
        print("Defender select your bonus!");

        defenderModController.ToggleModifierButtons(defender);

        while (!defenderModSet)
        {
            yield return null;
        }

        print("Defender added " + defenderBonusStrength + " bonus strength");

        SelectionPanel.SetActive(false);
    }

    IEnumerator CountDown(int seconds)
    {
        FightPanel.SetActive(true);
        for (int i = seconds; i >= 0; i--)
        {
            if (i == 0)
                print("GO!");
            else
                print(i);
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator ButtonMashFight(PlayerController attacker, PlayerController defender)
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

    public void SetAttackerModifier(float mod)
    {
        attackerBonusStrength = mod;
        attackerModSet = true;
    }

    public void SetDefenderModifier(float mod)
    {
        defenderBonusStrength = mod;
        defenderModSet = true;
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
