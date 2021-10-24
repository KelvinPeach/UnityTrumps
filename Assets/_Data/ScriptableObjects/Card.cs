using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card", order = 1)]
public class Card : ScriptableObject
{
    public string DisplayName { get { return displayName; } }
    public string Description { get { return description; } }
    public Sprite Icon { get { return icon; } }
    public StatValue[] Stats { get { return stats; } }

    [SerializeField] string displayName;
    [SerializeField] string description;
    [SerializeField] Sprite icon;
    //[SerializeField] int[] statValue = new int[6];
    [SerializeField] StatValue[] stats = new StatValue[6];
}

[System.Serializable]
public struct StatValue
{
    public Stat stat;
    public float value;
}