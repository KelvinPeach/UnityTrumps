using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trumps
{
    public class GameManager : MonoBehaviour
    {
        public static GameState GameState { get { return gameState; } }

        static GameState gameState;

        // Events
        public delegate void OnNewTurn();
        public static event OnNewTurn onNewTurn;
        public delegate void OnPlayerTurn();
        public static event OnPlayerTurn onPlayerTurn;
        public delegate void OnComputerTurn();
        public static event OnComputerTurn onComputerTurn;
        public delegate void OnVictory();
        public static event OnVictory onVictory;
        public delegate void OnGameOver();
        public static event OnGameOver onGameOver;

        void Awake()
        {
            // Subscribe to events
            DeckManager.onDeckReady += OnDeckReady;
            DeckManager.onPlayerWinsTurn += OnPlayerWinsTurn;
            DeckManager.onComputerWinsTurn += OnComputerWinsTurn;
            DeckManager.onDrawTurn += OnDrawTurn;
            DeckManager.onComputerOutOfCards += OnComputerOutOfCards;
            DeckManager.onPlayerOutOfCards += OnPlayerOutOfCards;
        }

        void OnDeckReady()
        {
            SetState(GameState.PLAYER_TURN);
        }

        void SetState(GameState newState)
        {
            gameState = newState;

            switch (newState)
            {
                case GameState.INTRO:
                    break;
                case GameState.PLAYER_TURN:

                    if (onNewTurn != null)
                        onNewTurn();

                    if (onPlayerTurn != null)
                        onPlayerTurn();

                    break;
                case GameState.COMPUTER_TURN:

                    if (onNewTurn != null)
                        onNewTurn();

                    if (onComputerTurn != null)
                        onComputerTurn();

                    break;
                case GameState.VICTORY:

                    if (onVictory != null)
                        onVictory();

                    break;
                case GameState.GAME_OVER:

                    if (onGameOver != null)
                        onGameOver();

                    break;
                default:
                    break;
            }
        }

        void OnPlayerWinsTurn()
        {
            SetState(GameState.PLAYER_TURN);
        }

        void OnComputerWinsTurn()
        {
            SetState(GameState.COMPUTER_TURN);
        }

        void OnDrawTurn()
        {
            // If there is a draw, let the current person keep playing
            if (gameState == GameState.COMPUTER_TURN)
            {
                SetState(GameState.COMPUTER_TURN);
            }
            else
            {
                SetState(GameState.PLAYER_TURN);
            }
        }

        void OnPlayerOutOfCards()
        {
            SetState(GameState.GAME_OVER);
        }

        void OnComputerOutOfCards()
        {
            SetState(GameState.VICTORY);
        }

        void OnDestroy()
        {
            // Unsubscribe from events
            DeckManager.onDeckReady -= OnDeckReady;
            DeckManager.onPlayerWinsTurn -= OnPlayerWinsTurn;
            DeckManager.onComputerWinsTurn -= OnComputerWinsTurn;
            DeckManager.onDrawTurn -= OnDrawTurn;
            DeckManager.onComputerOutOfCards -= OnComputerOutOfCards;
            DeckManager.onPlayerOutOfCards -= OnPlayerOutOfCards;
        }
    }

    public enum GameState { INTRO, PLAYER_TURN, COMPUTER_TURN, VICTORY, GAME_OVER }
}