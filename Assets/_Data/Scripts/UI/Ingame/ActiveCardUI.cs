using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trumps
{
    public class ActiveCardUI : MonoBehaviour
    {
        public Text CardNameText { get { return cardNameText; } }
        public Image Icon { get { return icon; } }
        public StatButton[] StatButtons { get { return statButtons; } }
        public Text DescriptionText { get { return descriptionText; } }
        public GameObject BackImage { get { return backImage; } }

        [SerializeField] Text cardNameText;
        [SerializeField] Image icon;
        [SerializeField] StatButton[] statButtons;
        [SerializeField] Text descriptionText;
        [SerializeField] GameObject backImage;
    }
}