using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Battle System/Enemy")]
public class BattleEnemy : ScriptableObject
{
    public string enemyName = "";
    public List<string> startingDialogue = new List<string>();
    public List<ActOption> actOptions = new List<ActOption>();
    // public string enemyName;
    // public int maxHealth;
    // public int attack;
    // public int experience;
    // public int gold;
}