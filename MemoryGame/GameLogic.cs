using System;
using System.Collections.Generic;

namespace MemoryGame
{
    public class GameLogic
    {
        private string[,] m_BoardValues;
        private HashSet<(int, int)> m_RevealedCards = new HashSet<(int, int)>();
        private Dictionary<string, int> m_PlayerPairs = new Dictionary<string, int>();
        private Dictionary<string, List<(int, int)>> m_CardMemory = new Dictionary<string, List<(int, int)>>();
        private Random m_Random = new Random();

        public string FirstPlayerName { get; private set; }
        public string SecondPlayerName { get; private set; }
        public bool IsAgainstComputer { get; private set; }
        public string CurrentPlayer { get; private set; }
        public int Rows { get; private set; }
        public int Columns { get; private set; }

        public GameLogic(string i_FirstPlayer, string i_SecondPlayer, bool i_IsAgainstComputer, int i_Rows, int i_Columns)
        {
            FirstPlayerName = i_FirstPlayer;
            SecondPlayerName = i_SecondPlayer;
            IsAgainstComputer = i_IsAgainstComputer;
            Rows = i_Rows;
            Columns = i_Columns;
            CurrentPlayer = FirstPlayerName;

            m_PlayerPairs[FirstPlayerName] = 0;
            m_PlayerPairs[SecondPlayerName] = 0;

            InitializeBoard();
        }

        private void InitializeBoard()
        {
            List<string> cardValues = GenerateCardValues();
            m_BoardValues = new string[Rows, Columns];

            int index = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    m_BoardValues[i, j] = cardValues[index++];
                }
            }
        }

        private List<string> GenerateCardValues()
        {
            List<string> values = new List<string>();
            int totalCards = Rows * Columns;
            char card = 'A';

            for (int i = 0; i < totalCards / 2; i++)
            {
                values.Add(card.ToString());
                values.Add(card.ToString());
                card++;
            }

            Shuffle(values);
            return values;
        }

        private void Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = m_Random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public string GetCardValue(int row, int col)
        {
            return m_BoardValues[row, col];
        }

        public bool CheckMatch((int, int) firstCard, (int, int) secondCard)
        {
            bool isMatch = m_BoardValues[firstCard.Item1, firstCard.Item2] == m_BoardValues[secondCard.Item1, secondCard.Item2];

            if (isMatch)
            {
                m_RevealedCards.Add(firstCard);
                m_RevealedCards.Add(secondCard);
                m_PlayerPairs[CurrentPlayer]++;

                // Remove matched cards from memory
                RemoveCardFromMemory(firstCard);
                RemoveCardFromMemory(secondCard);
            }
            else
            {
                // Keep unmatched cards in memory for better guesses
                AddCardToMemory(firstCard);
                AddCardToMemory(secondCard);
            }

            return isMatch;
        }

        public void SwitchPlayer()
        {
            CurrentPlayer = (CurrentPlayer == FirstPlayerName) ? SecondPlayerName : FirstPlayerName;
        }

        public int GetPlayerScore(string player)
        {
            return m_PlayerPairs[player];
        }

        public List<(int, int)> ComputerPlayTurn(List<(int, int)> currentlyHiddenCards)
        {
            // Look for a known pair in memory
            foreach (var entry in m_CardMemory)
            {
                if (entry.Value.Count >= 2)
                {
                    return new List<(int, int)> { entry.Value[0], entry.Value[1] };
                }
            }

            // If no pairs are known, choose one random hidden cards
            (int, int) firstChoice = currentlyHiddenCards[m_Random.Next(currentlyHiddenCards.Count)];
            AddCardToMemory(firstChoice);
            (int, int) secondChoice;
            // Look for a known pair in memory
            foreach (var entry in m_CardMemory)
            {
                if (entry.Value.Count >= 2)
                {
                    return new List<(int, int)> { entry.Value[0], entry.Value[1] };
                }
            }
            do // Makes sure the second choice is different from the first
            {
                secondChoice = currentlyHiddenCards[m_Random.Next(currentlyHiddenCards.Count)];
            } while (firstChoice == secondChoice);

            return new List<(int, int)> { firstChoice, secondChoice };
        }

        public string CheckWinner()
        {
            int totalRevealedCards = m_RevealedCards.Count;
            int totalCards = Rows * Columns;

            // Check if all cards are revealed
            if (totalRevealedCards == totalCards)
            {
                int firstPlayerScore = m_PlayerPairs[FirstPlayerName];
                int secondPlayerScore = m_PlayerPairs[SecondPlayerName];

                if (firstPlayerScore > secondPlayerScore)
                {
                    return $"{FirstPlayerName} wins with {firstPlayerScore} pairs!";
                }
                else if (secondPlayerScore > firstPlayerScore)
                {
                    return $"{SecondPlayerName} wins with {secondPlayerScore} pairs!";
                }
                else
                {
                    return "It's a tie!";
                }
            }

            return null; // No winner yet
        }

        private void AddCardToMemory((int, int) card)
        {
            string value = GetCardValue(card.Item1, card.Item2);

            if (!m_CardMemory.ContainsKey(value))
            {
                m_CardMemory[value] = new List<(int, int)>();
            }

            if (!m_CardMemory[value].Contains(card))
            {
                m_CardMemory[value].Add(card);
            }
        }

        private void RemoveCardFromMemory((int, int) card)
        {
            foreach (var value in new List<string>(m_CardMemory.Keys))
            {
                m_CardMemory[value].Remove(card);

                if (m_CardMemory[value].Count == 0)
                {
                    m_CardMemory.Remove(value);
                }
            }
        }
    }
}
