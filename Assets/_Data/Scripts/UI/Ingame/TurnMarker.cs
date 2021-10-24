using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trumps
{
    [RequireComponent(typeof(RectTransform))]
    public class TurnMarker : MonoBehaviour
    {
        [SerializeField] float playerPosX = -800f;
        [SerializeField] float computerPosX = 800f;

        // Cache
        RectTransform rectTransform;

        void Awake()
        {
            // Subscribe to events
            GameManager.onPlayerTurn += MoveToPlayerPosition;
            GameManager.onComputerTurn += MoveToComputerPosition;

            // Cache
            rectTransform = GetComponent<RectTransform>();
        }

        void MoveToPlayerPosition()
        {
            // Left side of the screen
            rectTransform.anchoredPosition = new Vector2(playerPosX, rectTransform.anchoredPosition.y);
        }

        void MoveToComputerPosition()
        {
            // Right side of the screen
            rectTransform.anchoredPosition = new Vector2(computerPosX, rectTransform.anchoredPosition.y);
        }

        void OnDestroy()
        {
            // Unsubscribe from events
            GameManager.onPlayerTurn -= MoveToPlayerPosition;
            GameManager.onComputerTurn -= MoveToComputerPosition;
        }
    }
}