using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Deck", menuName = "ScriptableObjects/Deck", order = 1)]
public class Deck : ScriptableObject
{
    public string DisplayName { get { return displayName; } }
    public string Description { get { return description; } }
    public Stat[] Stats { get { return stats; } }
    public Card[] Cards { get { return cards; } }

    [SerializeField] string displayName;
    [SerializeField] string description;
    [SerializeField] Stat[] stats = new Stat[6];
    [SerializeField] Card[] cards;
}