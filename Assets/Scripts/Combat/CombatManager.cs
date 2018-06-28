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

    [Header("Str Modifiers")]
    public float Mod0 = 0f;
    public float Mod1 = 0.025f;
    public float Mod2 = 0.05f;
    public float Mod4 = 0.1f;
    public float Mod6 = 0.15f;
    public float Mod8 = 0.20f;
    public float Mod10 = 0.25f;
    public float Mod15 = 0.375f;


    [Header("Inputs")]
    public KeyCode attackerInput = KeyCode.Alpha1;
    public KeyCode defenderInput = KeyCode.Alpha0;
    public KeyCode attackerInput2 = KeyCode.Keypad1;
    public KeyCode defenderInput2 = KeyCode.Keypad0;

    public UnityEvent OnFightFinish;
    public UnityEvent OnFightStart;
    public UnityEvent OnAttackerSelectStart;
    public UnityEvent OnDefenderSelectStart;
    public UnityEvent OnAttackerSelectEnd;
    public UnityEvent OnDefenderSelectEnd;
    public UnityEvent OnModifiersSelected;
    public UnityEvent OnResultsShowStart;
    public UnityEvent OnResultsShowEnd;

    [Header("UI Stuff")]
    public Slider FightSlider;
    public Slider SelectionSlider;
    public Animator CountdownAnimator;
    public Animator atkHandle;
    public Animator defHandle;
    public TextMeshProUGUI InfoTextAttacker;
    public TextMeshProUGUI InfoTextDefender;
    public TextMeshProUGUI CountdownText;
    public TextMeshProUGUI TimerText;
    public GameObject Fight_Results_Announcer;
    public PlayerUI attackerResults;
    public PlayerUI defenderResults;

    [Header("Particles")]
    public ParticleSystem HandleParticle;

    [Header("Scripts")]
    public ModifierButtonsController attackerModController;
    public ModifierButtonsController defenderModController;

    #endregion

    PlayerController attacker, defender, winner;

    float fightSliderValue = 0f;
    float attackerBonusStrength = 0f, defenderBonusStrength = 0f;
    int attackerModIndex = 0, defenderModIndex = 0;
    bool attackerModSet = false, defenderModSet = false;
    bool resultsShown = false;
    float Vbase = 5f / 7f;

    TextMeshProUGUI fight_result_text;
    Animator fight_result_animator;

    [HideInInspector]
    public FightSliderController fightSliderController;   

    public static CombatManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        fightSliderController = GetComponentInChildren<FightSliderController>();
        fight_result_text = Fight_Results_Announcer.GetComponentInChildren<TextMeshProUGUI>();
        fight_result_animator = Fight_Results_Announcer.GetComponent<Animator>();
    }

    private void Start()
    {
        FightSlider.maxValue = maxValue;
        FightSlider.minValue = minValue;
        SelectionSlider.maxValue = maxValue;
        SelectionSlider.minValue = minValue;
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
        AudioManager.instance.Play("FightStart");

        yield return StartCoroutine(ModifiersSelection());

        yield return StartCoroutine(CountDown(3));

        yield return StartCoroutine(ButtonMashFight());

        OnFightFinish.Invoke();

        yield return StartCoroutine(ShowResults());

        

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

        if (attackerModIndex != 0)
            yield return StartCoroutine(SelectionSliderAnim(fightSliderValue, 0.01f, true));
        else
            yield return new WaitForSeconds(.7f);

        OnAttackerSelectEnd.Invoke();

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

        if (defenderModIndex != 0)
            yield return StartCoroutine(SelectionSliderAnim(fightSliderValue, 0.01f, false));
        else
            yield return new WaitForSeconds(.7f);

        OnDefenderSelectEnd.Invoke();

        yield return new WaitForSeconds(1f);

        attackerResults.Refresh();
        defenderResults.Refresh();

        //selection end
        if (attackerModIndex > defenderModIndex)
            attackerBonusStrength += .05f;
        else if (attackerModIndex < defenderModIndex)
            defenderBonusStrength += .05f;

        var main = HandleParticle.main;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;

        OnModifiersSelected.Invoke();
    }

    IEnumerator CountDown(int seconds)
    {
        FightSlider.value = fightSliderValue;
        for (int i = seconds; i >= 0; i--)
        {
            if (i == 0)
            {
                CountdownText.text = "FIGHT!";
                AudioManager.instance.Play("FightAnnouncer");
                fightSliderController.StartRaysAnimation(.5f);
            }
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

                if (Input.GetKeyDown(attackerInput) || Input.GetKeyDown(attackerInput2))
                {
                    fightSliderValue += currentAttStr;
                }

                if (Input.GetKeyDown(defenderInput) || Input.GetKeyDown(defenderInput2))
                {
                    fightSliderValue -= currentDefStr;
                }

                FightSlider.value = fightSliderValue;

                yield return null;
            }
        }
        else
            while (fightSliderValue > minValue && fightSliderValue < maxValue)
            {
                timerForMultiplier += Time.deltaTime;

                currentAttStr = startAttStr * (timerForMultiplier * timerMultiplier);
                currentDefStr = startDefStr * (timerForMultiplier * timerMultiplier);

                if (Input.GetKeyDown(attackerInput) || Input.GetKeyDown(attackerInput2))
                {
                    fightSliderValue += currentAttStr;
                }

                if (Input.GetKeyDown(defenderInput) || Input.GetKeyDown(defenderInput2))
                {
                    fightSliderValue -= currentDefStr;
                }

                FightSlider.value = fightSliderValue;

                yield return null;
            }

        if (fightSliderValue <= minValue)
        {
            //defender won.
            print("DEFENDER WON!");
            winner = defender;
            AudioManager.instance.Play("AttackerLoss");
        }
        else if (fightSliderValue >= maxValue)
        {
            //attacker won.
            print("ATTACKER WON!");
            winner = attacker;
            AudioManager.instance.Play("AttackerWin");

            attacker.VictoryPoints++;
            if (defender.VictoryPoints > 0)
                defender.VictoryPoints--;
        }
        else if (timerForFightEnd <= 0 && fightSliderValue > maxValue *.5f)
        {
            //attacker won.
            print("TIME'S OVER! ATTACKER WON!");
            AudioManager.instance.Play("AttackerWin");
            winner = attacker;
            attacker.VictoryPoints++;
            if (defender.VictoryPoints > 0)
                defender.VictoryPoints--;
        }
        else if (timerForFightEnd <= 0 && fightSliderValue <= maxValue * .5f)
        {
            //defender won.
            print("TIME'S OVER! DEFENDER WON!");
            winner = defender;
            AudioManager.instance.Play("AttackerLoss");
        }
    }

    IEnumerator ShowResults()
    {
        defender.TurnOnParticles(false);
        attacker.TurnOnParticles(false);

        if(winner == defender)
        {
            defender.animator.SetTrigger("Win");
            attacker.animator.SetTrigger("Lose");

            fight_result_text.text = defender.type.ToString() + " defended himself from " + attacker.type.ToString();

            OnResultsShowStart.Invoke();
        }
        else
        {
            attacker.animator.SetTrigger("Win");
            defender.animator.SetTrigger("Lose");

            fight_result_text.text = attacker.type.ToString() + " stole from " + defender.type.ToString();

            OnResultsShowStart.Invoke();
        }

        yield return StartCoroutine(ShowResultsEnd());

    }

    IEnumerator ShowResultsEnd()
    {
        
        fight_result_animator.ResetTrigger("Hide");
        fight_result_animator.SetTrigger("Show");

        yield return new WaitForSeconds(1.5f);

        attackerResults.Refresh();
        yield return new WaitForSeconds(1.5f);

        defenderResults.Refresh();
        yield return new WaitForSeconds(1.5f);

        OnResultsShowEnd.Invoke();
        GameManager.instance.currentActivePlayer.TurnOnParticles(true);
        attacker.animator.SetTrigger("Idle");
        defender.animator.SetTrigger("Idle");
        atkHandle.SetTrigger("Stop");
        defHandle.SetTrigger("Stop");
        fight_result_animator.ResetTrigger("Show");
        fight_result_animator.SetTrigger("Hide");
    }

    public void SetAttackerModifier(int energy)
    {
        switch (energy)
        {
            case 0:
                attackerBonusStrength = Mod0;
                attackerModIndex = 0;
                break;
            case 1:
                attackerBonusStrength = Mod1;
                attackerModIndex = 1;
                break;
            case 2:
                attackerBonusStrength = Mod2;
                attackerModIndex = 2;
                break;
            case 4:
                attackerBonusStrength = Mod4;
                attackerModIndex = 3;
                break;
            case 6:
                attackerBonusStrength = Mod6;
                attackerModIndex = 4;
                break;
            case 8:
                attackerBonusStrength = Mod8;
                attackerModIndex = 5;
                break;
            case 10:
                attackerBonusStrength = Mod10;
                attackerModIndex = 6;
                break;
            case 15:
                attackerBonusStrength = Mod15;
                attackerModIndex = 7;
                break;
        }

        attacker.EnergyPoints -= energy;
        attackerModController.EnableModifierButtons(false);
        attackerModSet = true;

        fightSliderValue = maxValue*.5f + Vbase * (attackerModIndex - defenderModIndex);

        OnAttackerSelectEnd.Invoke();
    }

    public void SetDefenderModifier(int energy)
    {
        defenderModController.EnableModifierButtons(true);
        switch (energy)
        {
            case 0:
                defenderBonusStrength = Mod0;
                defenderModIndex = 0;
                break;
            case 1:
                defenderBonusStrength = Mod1;
                defenderModIndex = 1;
                break;
            case 2:
                defenderBonusStrength = Mod2;
                defenderModIndex = 2;
                break;
            case 4:
                defenderBonusStrength = Mod4;
                defenderModIndex = 3;
                break;
            case 6:
                defenderBonusStrength = Mod6;
                defenderModIndex = 4;
                break;
            case 8:
                defenderBonusStrength = Mod8;
                defenderModIndex = 5;
                break;
            case 10:
                defenderBonusStrength = Mod10;
                defenderModIndex = 6;
                break;
            case 15:
                defenderBonusStrength = Mod15;
                defenderModIndex = 7;
                break;
        }

        defender.EnergyPoints -= energy;
        defenderModSet = true;
        defenderModController.EnableModifierButtons(false);

        fightSliderValue = maxValue * .5f + Vbase * (attackerModIndex - defenderModIndex);

        OnDefenderSelectEnd.Invoke();
    }

    public void ResetValues()
    {
        fightSliderValue = maxValue * .5f;
        attackerBonusStrength = 0f;
        defenderBonusStrength = 0f;
        attackerModSet = false;
        defenderModSet = false;
        resultsShown = false;
        FightSlider.value = fightSliderValue;
        SelectionSlider.GetComponentsInChildren<Image>()[0].color = defender.color;
        SelectionSlider.GetComponentsInChildren<Image>()[1].color = attacker.color;
        SelectionSlider.value = fightSliderValue;
        attackerResults.SetPlayerReference(attacker);
        defenderResults.SetPlayerReference(defender);

        fightSliderController.atkRayImage.fillAmount = 0.06f;
        fightSliderController.defRayImage.fillAmount = 0.06f;

        var main = HandleParticle.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        switch (attacker.type)
        {
            case PlayerController.Type.Hypogeum:
                fightSliderController.atkRayAnimator.SetTrigger("SetHypo");
                atkHandle.SetTrigger("SetHypo");
                break;
            case PlayerController.Type.Underwater:
                fightSliderController.atkRayAnimator.SetTrigger("SetWater");
                atkHandle.SetTrigger("SetWater");
                break;
            case PlayerController.Type.Undergrowth:
                fightSliderController.atkRayAnimator.SetTrigger("SetForest");
                atkHandle.SetTrigger("SetForest");
                break;
            case PlayerController.Type.Underground:
                fightSliderController.atkRayAnimator.SetTrigger("SetGround");
                atkHandle.SetTrigger("SetGround");
                break;
        }

        switch (defender.type)
        {
            case PlayerController.Type.Hypogeum:
                fightSliderController.defRayAnimator.SetTrigger("SetHypo");
                defHandle.SetTrigger("SetHypo");
                break;
            case PlayerController.Type.Underwater:
                fightSliderController.defRayAnimator.SetTrigger("SetWater");
                defHandle.SetTrigger("SetWater");
                break;
            case PlayerController.Type.Undergrowth:
                fightSliderController.defRayAnimator.SetTrigger("SetForest");
                defHandle.SetTrigger("SetForest");
                break;
            case PlayerController.Type.Underground:
                fightSliderController.defRayAnimator.SetTrigger("SetGround");
                defHandle.SetTrigger("SetGround");
                break;
        }
    }

    public void ResultsShown()
    {
        resultsShown = true;
    }

    public void AttackerModSelected()
    {
        attackerModSet = true;
    }

    public void DefenderModSelected()
    {
        defenderModSet = true;
    }

    public void ResetPlayersAfterFight()
    {
        foreach (var player in GameManager.instance.players)
        {
            player.GetComponent<CapsuleCollider>().enabled = false;
        }
    }

    public IEnumerator SelectionSliderAnim(float targetValue, float speed, /*true right, false left*/bool direction)
    {
        print(targetValue);
        if (direction)
            while (SelectionSlider.value < targetValue)
            {
                SelectionSlider.value += speed;
                print(SelectionSlider.value);
                yield return null;
            }
        else
            while (SelectionSlider.value > targetValue)
            {
                SelectionSlider.value -= speed;
                print(SelectionSlider.value);
                yield return null;
            }
    }

}
