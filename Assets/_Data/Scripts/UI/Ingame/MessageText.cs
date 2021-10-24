using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trumps
{
    public class MessageText : MonoBehaviour
    {
        [SerializeField] Text messageText;

        void Awake()
        {
            // Subscribe to events
            GameManager.onPlayerTurn += OnPlayerTurn;
            GameManager.onComputerTurn += OnComputerTurn;
            GameManager.onVictory += OnVictory;
            GameManager.onGameOver += OnGameOver;
        }

        #region Message event listeners

        void OnPlayerTurn()
        {
            messageText.text = "Your turn. Press a stat button.";
        }

        void OnComputerTurn()
        {
            messageText.text = "Computer turn";
        }

        void OnVictory()
        {
            messageText.text = "You win :)";
        }

        void OnGameOver()
        {
            messageText.text = "You lose :(";
        }

        #endregion

        void OnDestroy()
        {
            // Unsubscribe from events
            GameManager.onPlayerTurn -= OnPlayerTurn;
            GameManager.onComputerTurn -= OnComputerTurn;
            GameManager.onVictory -= OnVictory;
            GameManager.onGameOver -= OnGameOver;
        }
    }
}