using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CombatManager : MonoBehaviour
{
    public float currentAttStr = 0f, currentDefStr = 0f;

    #region public things

    public bool startOnPlay = false;

    public FloatEvent OnFightValueChange;
    public UnityEvent OnFightFinish;

    [Header("UI Panels")]
    public GameObject SelectionPanel;
    public GameObject FightPanel;

    [Header("Values")]
    public float fightSliderValue = 0f;
    public float baseStrength = .15f;

    [Header("Inputs")]
    public KeyCode attackerInput = KeyCode.Alpha1;
    public KeyCode defenderInput = KeyCode.Alpha0;

    #endregion

    float attackerBonusStrength = 0f, defenderBonusStrength = 0f;
    bool attackerModSet = false, defenderModSet = false;

    private void Start()
    {
        if (startOnPlay)
            StartFightFlow();
    }

    public void StartFightFlow()
    {
        ResetValues();
        StartCoroutine(FightFlow());
    }

    IEnumerator FightFlow()
    {
        yield return StartCoroutine(ModifiersSelection());

        yield return StartCoroutine(CountDown(3));

        yield return StartCoroutine(ButtonMashFight());

        OnFightFinish.Invoke();
    }

    IEnumerator ModifiersSelection()
    {
        SelectionPanel.SetActive(true);

        print("Attacker select your bonus!");

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

    IEnumerator ButtonMashFight()
    {
        float startAttStr = baseStrength + attackerBonusStrength;
        float startDefStr = baseStrength + defenderBonusStrength;

        float timer = 0f;

        while (fightSliderValue > -1 && fightSliderValue < 1)
        {
            timer += Time.deltaTime;

            currentAttStr = startAttStr * timer * .5f;
            currentDefStr = startDefStr * timer * .5f;

            if (Input.GetKeyDown(attackerInput))
            {
                fightSliderValue += currentAttStr;
            }

            if (Input.GetKeyDown(defenderInput))
            {
                fightSliderValue -= currentDefStr;
            }

            OnFightValueChange.Invoke(fightSliderValue);

            yield return null;
        }

        if (fightSliderValue <= -1)
        {
            //defender won.
            print("DEFENDER WON!");
        }
        else if (fightSliderValue >= 1)
        {
            //attacker won.
            print("ATTACKER WON!");
        }

        FightPanel.SetActive(false);

    }

    public void SetAttackerModifier(int mod)
    {
        attackerBonusStrength = mod;
        attackerModSet = true;
    }

    public void SetDefenderModifier(int mod)
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
    }

}
