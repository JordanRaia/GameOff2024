using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Act Option", menuName = "Act System/Act Option")]
public class ActOption : ScriptableObject
{
    public string Act = "";
    public List<string> responses = new List<string>();
}