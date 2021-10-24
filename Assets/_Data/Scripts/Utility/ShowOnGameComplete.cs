using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trumps
{
    public class ShowOnGameComplete : MonoBehaviour
    {
        [SerializeField] GameObject gameObjectToShow;

        void Awake()
        {
            // Subscribe to events
            GameManager.onGameOver += Show;
            GameManager.onVictory += Show;
        }

        void Show()
        {
            gameObjectToShow.SetActive(true);
        }

        void OnDestroy()
        {
            // Unsubscribe from events
            GameManager.onGameOver -= Show;
            GameManager.onVictory -= Show;
        }
    }
}