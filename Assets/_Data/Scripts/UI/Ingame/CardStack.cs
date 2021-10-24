using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trumps
{
    public class CardStack : MonoBehaviour
    {
        [SerializeField] bool isPlayerStack;
        [SerializeField] Text cardCountText;

        void Awake()
        {
            // Subscribe to events
            GameManager.onNewTurn += UpdateUI;
            GameManager.onVictory += UpdateUI;
            GameManager.onGameOver += UpdateUI;
        }

        void UpdateUI()
        {
            if (isPlayerStack)
            {
                cardCountText.text = DeckManager.PlayerCardCount.ToString();
            }
            else
            {
                cardCountText.text = DeckManager.ComputerCardCount.ToString();
            }
        }

        void OnDestroy()
        {
            // Unsubscribe from events
            GameManager.onNewTurn -= UpdateUI;
            GameManager.onVictory -= UpdateUI;
            GameManager.onGameOver -= UpdateUI;
        }
    }
}