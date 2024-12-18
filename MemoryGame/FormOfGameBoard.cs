using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace MemoryGame
{
    public partial class FormOfGameBoard : Form
    {
        private GameLogic m_GameLogic;
        private Label[,] m_BoardLabels;
        private Label r_CurrentPlayerLabel = new Label();
        private Label r_FirstPlayerScoreLabel = new Label();
        private Label r_SecondPlayerScoreLabel = new Label();
        private List<Label> m_SelectedLabels = new List<Label>();
        private int m_ClickedCount = 0;

        public FormOfGameBoard(string i_FirstPlayer, string i_SecondPlayer, bool i_IsAgainstComputer, int i_Rows, int i_Cols)
        {
            m_GameLogic = new GameLogic(i_FirstPlayer, i_SecondPlayer, i_IsAgainstComputer, i_Rows, i_Cols);
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Text = "Memory Game";
            this.Size = new Size(60 * m_GameLogic.Columns + 40, 60 * m_GameLogic.Rows + 120);
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeBoard();
            InitializePlayerInfo();
        }

        private void InitializeBoard()
        {
            m_BoardLabels = new Label[m_GameLogic.Rows, m_GameLogic.Columns];

            for (int i = 0; i < m_GameLogic.Rows; i++)
            {
                for (int j = 0; j < m_GameLogic.Columns; j++)
                {
                    Label card = new Label
                    {
                        BackColor = Color.LightGray,
                        Size = new Size(50, 50),
                        Location = new Point(20 + j * 55, 20 + i * 55),
                        Text = "",
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Arial", 12, FontStyle.Bold),
                        Tag = (i, j) // Store position in Tag
                    };

                    card.Click += Card_Click;
                    this.Controls.Add(card);
                    m_BoardLabels[i, j] = card;
                }
            }
        }

        private void InitializePlayerInfo()
        {
            r_CurrentPlayerLabel.Text = $"Current Player: {m_GameLogic.CurrentPlayer}";
            r_CurrentPlayerLabel.Location = new Point(20, this.ClientSize.Height - 80);
            r_CurrentPlayerLabel.Size = new Size(200, 20);
            this.Controls.Add(r_CurrentPlayerLabel);

            r_FirstPlayerScoreLabel.Text = $"{m_GameLogic.FirstPlayerName}: {m_GameLogic.GetPlayerScore(m_GameLogic.FirstPlayerName)} Pair(s)";
            r_FirstPlayerScoreLabel.Location = new Point(20, this.ClientSize.Height - 60);
            r_FirstPlayerScoreLabel.Size = new Size(200, 20);
            this.Controls.Add(r_FirstPlayerScoreLabel);

            r_SecondPlayerScoreLabel.Text = $"{m_GameLogic.SecondPlayerName}: {m_GameLogic.GetPlayerScore(m_GameLogic.SecondPlayerName)} Pair(s)";
            r_SecondPlayerScoreLabel.Location = new Point(20, this.ClientSize.Height - 40);
            r_SecondPlayerScoreLabel.Size = new Size(200, 20);
            this.Controls.Add(r_SecondPlayerScoreLabel);
        }

        private void Card_Click(object sender, EventArgs e)
        {
            if (m_ClickedCount >= 2) return;

            Label clickedCard = sender as Label;
            var (row, col) = ((ValueTuple<int, int>)clickedCard.Tag);

            if (clickedCard.Text != "") return; // Ignore already revealed cards

            clickedCard.Text = m_GameLogic.GetCardValue(row, col);
            clickedCard.BackColor = Color.LightBlue;
            m_SelectedLabels.Add(clickedCard);
            m_ClickedCount++;

            if (m_ClickedCount == 2)
            {
                this.Refresh();
                System.Threading.Tasks.Task.Delay(1000).ContinueWith(_ => CheckForMatch());
            }
        }

        private void CheckForMatch()
        {
            var first = (ValueTuple<int, int>)m_SelectedLabels[0].Tag;
            var second = (ValueTuple<int, int>)m_SelectedLabels[1].Tag;

            if (!m_GameLogic.CheckMatch(first, second))
            {
                m_SelectedLabels[0].Text = "";
                m_SelectedLabels[1].Text = "";
                m_BoardLabels[first.Item1, first.Item2].BackColor = Color.LightGray;
                m_BoardLabels[second.Item1, second.Item2].BackColor = Color.LightGray;
                m_GameLogic.SwitchPlayer();
            }
            else
            {
                m_SelectedLabels[0].BackColor = Color.LightGreen;
                m_SelectedLabels[1].BackColor = Color.LightGreen;
            }

            UpdateScores();
            m_SelectedLabels.Clear();
            m_ClickedCount = 0;

            // Check for a winner
            string winnerMessage = m_GameLogic.CheckWinner();
            if (!string.IsNullOrEmpty(winnerMessage))
            {
                MessageBox.Show(winnerMessage, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Close the form after the game ends
                return;
            }

            // If it's the computer's turn, let the computer play
            if (m_GameLogic.CurrentPlayer == m_GameLogic.SecondPlayerName && m_GameLogic.IsAgainstComputer)
            {
                List<(int, int)> hiddenCards = GetHiddenCards();
                var computerMove = m_GameLogic.ComputerPlayTurn(hiddenCards);

                SimulateComputerMove(computerMove[0]);
                Thread.Sleep(400);
                SimulateComputerMove(computerMove[1]);
                Thread.Sleep(200);
            }
        }

        private List<(int, int)> GetHiddenCards()
        {
            List<(int, int)> hiddenCards = new List<(int, int)>();
            for (int i = 0; i < m_GameLogic.Rows; i++)
            {
                for (int j = 0; j < m_GameLogic.Columns; j++)
                {
                    if (m_BoardLabels[i, j].Text == "")
                    {
                        hiddenCards.Add((i, j));
                    }
                }
            }
            return hiddenCards;
        }

        private void UpdateScores()
        {
            r_CurrentPlayerLabel.Text = $"Current Player: {m_GameLogic.CurrentPlayer}";
            r_FirstPlayerScoreLabel.Text = $"{m_GameLogic.FirstPlayerName}: {m_GameLogic.GetPlayerScore(m_GameLogic.FirstPlayerName)} Pair(s)";
            r_SecondPlayerScoreLabel.Text = $"{m_GameLogic.SecondPlayerName}: {m_GameLogic.GetPlayerScore(m_GameLogic.SecondPlayerName)} Pair(s)";
        }

        private void SimulateComputerMove((int, int) card)
        {
            Label label = m_BoardLabels[card.Item1, card.Item2];
            label.Text = m_GameLogic.GetCardValue(card.Item1, card.Item2);
            label.BackColor = Color.LightBlue;
            m_SelectedLabels.Add(label);
            m_ClickedCount++;

            if (m_ClickedCount == 2)
            {
                this.Refresh();
                System.Threading.Tasks.Task.Delay(1200).ContinueWith(_ => CheckForMatch());
            }
        }
    }
}
