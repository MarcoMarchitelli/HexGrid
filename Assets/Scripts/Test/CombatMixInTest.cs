using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CombatManager))]
public class CombatMixInTest : MonoBehaviour {

    CombatManager combatManager;

    public PlayerController attacker;
    public PlayerController defender;

    private void Awake()
    {
        combatManager = GetComponent<CombatManager>();
        if (!combatManager)
            Debug.LogWarning("CombatManager reference not found.");
    }

    public void StartFight()
    {
        if (attacker && defender)
            combatManager.StartFightFlow(attacker, defender);
    }

}
