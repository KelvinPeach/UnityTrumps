using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trumps
{
    public class StatButton : MonoBehaviour
    {
        [HideInInspector] public Stat Stat;

        public Button StatsButton { get { return statButton; } }
        public Text StatNameText { get { return statNameText; } }
        public Text StatValueText { get { return statValueText; } }

        [SerializeField] Button statButton;
        [SerializeField] Text statNameText;
        [SerializeField] Text statValueText;
    }
}