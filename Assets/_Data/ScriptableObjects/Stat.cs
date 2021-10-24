using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "ScriptableObjects/Stat", order = 1)]
public class Stat : ScriptableObject
{
    public string DisplayName { get { return displayName; } }
    public Sprite Icon { get { return icon; } }
    public bool IsHigherBetter { get { return isHigherBetter; } }

    [SerializeField] string displayName;
    [SerializeField] Sprite icon;
    [SerializeField] bool isHigherBetter = true;
}