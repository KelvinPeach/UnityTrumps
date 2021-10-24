using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trumps
{
    public class DeckManager : MonoBehaviour
    {
        public static int PlayerCardCount { get { return playerCards.Count; } }
        public static int ComputerCardCount { get { return computerCards.Count; } }

        static readonly float actionDelay = 2f;

        static Queue<Card> playerCards = new Queue<Card>();
        static Queue<Card> computerCards = new Queue<Card>();

        [SerializeField] Deck deck;
        [SerializeField] ActiveCardUI playerActiveCard;
        [SerializeField] ActiveCardUI computerActiveCard;

        [Header("Sound")]
        [SerializeField] AudioClip shuffleSound;
        [SerializeField] AudioClip cardPlaceSound;
        [SerializeField] AudioClip cardFlipSound;
        [SerializeField] AudioClip roundWinSound;
        [SerializeField] AudioClip roundLoseSound;
        [SerializeField] AudioClip gameWonSound;
        [SerializeField] AudioClip gameLostSound;

        // Events
        public delegate void OnDeckReady();
        public static event OnDeckReady onDeckReady;

        public delegate void OnPlayerWinsTurn();
        public static event OnPlayerWinsTurn onPlayerWinsTurn;
        public delegate void OnComputerWinsTurn();
        public static event OnComputerWinsTurn onComputerWinsTurn;
        public delegate void OnDrawTurn();
        public static event OnDrawTurn onDrawTurn;

        public delegate void OnPlayerOutOfCards();
        public static event OnPlayerOutOfCards onPlayerOutOfCards;
        public delegate void OnComputerOutOfCards();
        public static event OnComputerOutOfCards onComputerOutOfCards;

        // Cache
        AudioSource audioSrc;

        void Awake()
        {
            // Subscribe to events
            GameManager.onNewTurn += OnNewTurn;

            // Cache
            audioSrc = GetComponent<AudioSource>();
        }

        void Start()
        {
            // Reset cards from any previous matches
            playerCards.Clear();
            computerCards.Clear();

            // Shuffle deck
            // Source - https://forum.unity.com/threads/clever-way-to-shuffle-a-list-t-in-one-line-of-c-code.241052/
            List<Card> cards = deck.Cards.OrderBy(i => System.Guid.NewGuid()).ToList();

            // Play shuffle sound
            if (audioSrc && shuffleSound)
                audioSrc.PlayOneShot(shuffleSound);

            // Give half the cards to the player
            for (int i = 0; i < cards.Count / 2; i++)
            {
                playerCards.Enqueue(cards[i]);
            }

            // Give the remaining cards to the computer
            for (int i = cards.Count / 2; i < cards.Count; i++)
            {
                computerCards.Enqueue(cards[i]);
            }

            if (onDeckReady != null)
                onDeckReady();
        }

        void OnNewTurn()
        {
            // Assign top player deck card to active card
            if (playerCards.Count > 0)
            {
                // If it's the player's turn, re-enable their controls
                if (GameManager.GameState == GameState.PLAYER_TURN)
                {
                    // Make sure stat buttons are enabled
                    foreach (var item in playerActiveCard.StatButtons)
                    {
                        item.StatsButton.interactable = true;
                    }
                }

                SetupActiveCardUI(playerActiveCard, playerCards.Peek());
            }

            // Assign top computer deck card to active card
            if (computerCards.Count > 0)
            {
                // Hide the computer's card
                computerActiveCard.BackImage.SetActive(true);

                SetupActiveCardUI(computerActiveCard, computerCards.Peek());

                // Is it the computer's turn?
                if (GameManager.GameState == GameState.COMPUTER_TURN)
                {
                    // IEnumerator so there is a delay (otherwise it will all happen to fast)
                    StartCoroutine(ComputerSelectStat(actionDelay));
                }
            }

            // Reset button colours
            foreach (var item in computerActiveCard.StatButtons)
            {
                item.StatsButton.image.color = Color.white;
            }
            foreach (var item in playerActiveCard.StatButtons)
            {
                item.StatsButton.image.color = Color.white;
            }

            // Play card place sound
            if (audioSrc && cardPlaceSound)
                audioSrc.PlayOneShot(cardPlaceSound);
        }

        void SetupActiveCardUI(ActiveCardUI activeCard, Card cardInDeck)
        {
            activeCard.CardNameText.text = cardInDeck.DisplayName;

            activeCard.Icon.sprite = cardInDeck.Icon;

            // Assign the six stats
            for (int i = 0; i < 6; i++)
            {
                // Stat
                activeCard.StatButtons[i].Stat = cardInDeck.Stats[i].stat;

                // Name
                activeCard.StatButtons[i].StatNameText.text = cardInDeck.Stats[i].stat.DisplayName;

                // Value
                activeCard.StatButtons[i].StatValueText.text = cardInDeck.Stats[i].value.ToString();
            }

            activeCard.DescriptionText.text = cardInDeck.Description;
        }

        IEnumerator ComputerSelectStat(float delay)
        {
            yield return new WaitForSeconds(delay);

            // TODO computer should take into account lower stats being better for some stats

            // Find the highest stat for the current card
            float highestValue = 0;
            string highestStat = "";
            bool isHigherBetter = false;

            foreach (var item in computerActiveCard.StatButtons)
            {
                // Determine if a higher number is better or worse
                if (float.Parse(item.StatValueText.text) > highestValue)
                {
                    highestValue = float.Parse(item.StatValueText.text);
                    highestStat = item.StatNameText.text;

                    // Determine if a higher number is better or worse
                    isHigherBetter = item.Stat.IsHigherBetter;
                }
            }

            // Choose the winner
            StartCoroutine(DetermineTurnWinner(highestStat, isHigherBetter));
        }

        IEnumerator DetermineTurnWinner(string statName, bool isHigherBetter)
        {
            // Show the computer's card
            computerActiveCard.BackImage.SetActive(false);

            // Play card flip sound
            if (audioSrc && cardFlipSound)
                audioSrc.PlayOneShot(cardFlipSound);

            // Check which card wins (is a higher number)
            float playerValue = 0;
            float computerValue = 0;

            // Get player value
            foreach (var item in playerActiveCard.StatButtons)
            {
                if (item.StatNameText.text == statName)
                {
                    playerValue = float.Parse(item.StatValueText.text);
                }
            }

            // Get computer value
            foreach (var item in computerActiveCard.StatButtons)
            {
                if (item.StatNameText.text == statName)
                {
                    computerValue = float.Parse(item.StatValueText.text);
                }
            }

            // Check if higher is better
            // TODO refactor this so code isn't identical apart from playerValue > computerValue or playerValue < computerValue
            if (isHigherBetter)
            {
                // Player wins
                if (playerValue > computerValue)
                {
                    PlayerWinsTurn(statName);

                    // Delay so the player can see what happened
                    yield return new WaitForSeconds(actionDelay);

                    // Has the computer got anymore cards to play?
                    if (computerCards.Count > 0)
                    {
                        if (onPlayerWinsTurn != null)
                            onPlayerWinsTurn();
                    }
                    else
                    {
                        // Disable player's buttons to prevent them continuing to play
                        foreach (var item in playerActiveCard.StatButtons)
                        {
                            item.StatsButton.interactable = false;
                        }

                        // Play victory sound
                        if (audioSrc && gameWonSound)
                            audioSrc.PlayOneShot(gameWonSound);

                        if (onComputerOutOfCards != null)
                            onComputerOutOfCards();
                    }
                }
                // Computer wins
                else if (playerValue < computerValue)
                {
                    ComputerWinsTurn(statName);

                    yield return new WaitForSeconds(actionDelay);

                    // Has the player got anymore cards to play?
                    if (playerCards.Count > 0)
                    {
                        if (onComputerWinsTurn != null)
                            onComputerWinsTurn();
                    }
                    else
                    {
                        // Play game over sound
                        if (audioSrc && gameLostSound)
                            audioSrc.PlayOneShot(gameLostSound);

                        if (onPlayerOutOfCards != null)
                            onPlayerOutOfCards();
                    }
                }
                // Draw
                else
                {
                    DrawTurn(statName);

                    yield return new WaitForSeconds(actionDelay);

                    if (onDrawTurn != null)
                        onDrawTurn();
                }
            }
            else
            {
                // Player wins
                if (playerValue < computerValue)
                {
                    PlayerWinsTurn(statName);

                    // Delay so the player can see what happened
                    yield return new WaitForSeconds(actionDelay);

                    // Has the computer got anymore cards to play?
                    if (computerCards.Count > 0)
                    {
                        if (onPlayerWinsTurn != null)
                            onPlayerWinsTurn();
                    }
                    else
                    {
                        // Disable player's buttons to prevent them continuing to play
                        foreach (var item in playerActiveCard.StatButtons)
                        {
                            item.StatsButton.interactable = false;
                        }

                        // Play victory sound
                        if (audioSrc && gameWonSound)
                            audioSrc.PlayOneShot(gameWonSound);

                        if (onComputerOutOfCards != null)
                            onComputerOutOfCards();
                    }
                }
                // Computer wins
                else if (playerValue > computerValue)
                {
                    ComputerWinsTurn(statName);

                    yield return new WaitForSeconds(actionDelay);

                    // Has the player got anymore cards to play?
                    if (playerCards.Count > 0)
                    {
                        if (onComputerWinsTurn != null)
                            onComputerWinsTurn();
                    }
                    else
                    {
                        // Play game over sound
                        if (audioSrc && gameLostSound)
                            audioSrc.PlayOneShot(gameLostSound);

                        if (onPlayerOutOfCards != null)
                            onPlayerOutOfCards();
                    }
                }
                // Draw
                else
                {
                    DrawTurn(statName);

                    yield return new WaitForSeconds(actionDelay);

                    if (onDrawTurn != null)
                        onDrawTurn();
                }
            }
        }

        void PlayerWinsTurn(string statName)
        {
            // The player takes the computer's card (if they have one)
            if (computerCards.Count > 0)
                playerCards.Enqueue(computerCards.Dequeue());

            // The player's current card goes to the bottom of the pile
            playerCards.Enqueue(playerCards.Dequeue());

            // Highlight player's winning stat
            foreach (var item in playerActiveCard.StatButtons)
            {
                if (item.StatNameText.text == statName)
                {
                    item.StatsButton.image.color = Color.green;
                }
            }

            // Highlight computer's losing stat
            foreach (var item in computerActiveCard.StatButtons)
            {
                if (item.StatNameText.text == statName)
                {
                    item.StatsButton.image.color = Color.red;
                }
            }

            // Play round win sound
            if (audioSrc && roundWinSound)
                audioSrc.PlayOneShot(roundWinSound);
        }

        void ComputerWinsTurn(string statName)
        {
            // The computer takes the player's card (if they have one)
            if (playerCards.Count > 0)
                computerCards.Enqueue(playerCards.Dequeue());

            // The computer's current card goes to the bottom of the pile
            computerCards.Enqueue(computerCards.Dequeue());

            // Highlight computer's winning stat
            foreach (var item in computerActiveCard.StatButtons)
            {
                if (item.StatNameText.text == statName)
                {
                    item.StatsButton.image.color = Color.green;
                }
            }

            // Highlight player's losing stat
            foreach (var item in playerActiveCard.StatButtons)
            {
                if (item.StatNameText.text == statName)
                {
                    item.StatsButton.image.color = Color.red;
                }
            }

            // Play round lose sound
            if (audioSrc && roundLoseSound)
                audioSrc.PlayOneShot(roundLoseSound);
        }

        void DrawTurn(string statName)
        {
            // The player's current card goes to the bottom of the pile
            playerCards.Enqueue(playerCards.Dequeue());

            // The computer's current card goes to the bottom of the pile
            computerCards.Enqueue(computerCards.Dequeue());

            // Highlight computer's winning stat
            foreach (var item in computerActiveCard.StatButtons)
            {
                if (item.StatNameText.text == statName)
                {
                    item.StatsButton.image.color = Color.green;
                }
            }

            // Highlight player's winning stat
            foreach (var item in playerActiveCard.StatButtons)
            {
                if (item.StatNameText.text == statName)
                {
                    item.StatsButton.image.color = Color.green;
                }
            }
        }

        public void PlayerSelectStat(UnityEngine.UI.Text statName)
        {
            // Make sure it's the player's turn
            if (GameManager.GameState == GameState.PLAYER_TURN)
            {
                bool isHigherBetter = false;

                // Disable player stat buttons to prevent the player pressing again
                foreach (var item in playerActiveCard.StatButtons)
                {
                    item.StatsButton.interactable = false;

                    // Determine if a higher number is better or worse
                    if (item.StatNameText == statName)
                    {
                        isHigherBetter = item.Stat.IsHigherBetter;
                    }
                }

                StartCoroutine(DetermineTurnWinner(statName.text, isHigherBetter));
            }
        }

        void OnDestroy()
        {
            // Unsubscribe from events
            GameManager.onNewTurn -= OnNewTurn;
        }
    }
}