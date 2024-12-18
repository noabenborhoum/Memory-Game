using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MemoryGame
{
    public partial class StartForm : Form
    {
        // Shared dimensions for controls
        private const int k_LabelWidth = 120;
        private const int k_TextBoxWidth = 120;
        private const int k_ButtonWidth = 150;
        private const int k_ButtonHeight = 30;
        private const int k_SpacingBetweenControls = 20;

        // Positions
        private const int k_LeftMargin = 20;
        private const int k_TopMargin = 20;

        private readonly List<(int, int)> r_SizesOfGameBoard = new List<(int, int)>
        {
            (4, 4), (4, 5), (4, 6), (5, 4), (5, 6), (6, 4), (6, 5), (6, 6)
        };

        private int m_CurrentBoardSizeIndex = 0;
        private bool m_TextBoxFriendEnabled = false;

        // Controls
        private readonly Label r_FirstPlayerLabel = new Label();
        private readonly TextBox r_FirstPlayerName = new TextBox();
        private readonly Label r_SecondPlayerLabel = new Label();
        private readonly TextBox r_SecondPlayerName = new TextBox();
        private readonly Label r_BoardSizeLabel = new Label();
        private readonly Button r_AgainstAFriendButton = new Button();
        private readonly Button r_BoardSizeButton = new Button();
        private readonly Button r_StartButton = new Button();

        public StartForm()
        {
            InitializeComponent();
            this.Text = "Memory Game - Settings";
            this.Size = new Size(550, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeFormComponents();
        }

        private void InitializeFormComponents()
        {
            int currentTop = k_TopMargin;

            // First Player Label
            r_FirstPlayerLabel.Text = "First Player Name:";
            r_FirstPlayerLabel.Size = new Size(k_LabelWidth, k_ButtonHeight);
            r_FirstPlayerLabel.Location = new Point(k_LeftMargin, currentTop);

            // First Player Textbox
            r_FirstPlayerName.Size = new Size(k_TextBoxWidth, k_ButtonHeight);
            r_FirstPlayerName.Location = new Point(k_LeftMargin + k_LabelWidth + k_SpacingBetweenControls, currentTop);

            currentTop += k_ButtonHeight + k_SpacingBetweenControls;

            // Second Player Label
            r_SecondPlayerLabel.Text = "Second Player Name:";
            r_SecondPlayerLabel.Size = new Size(k_LabelWidth, k_ButtonHeight);
            r_SecondPlayerLabel.Location = new Point(k_LeftMargin, currentTop);

            // Second Player Textbox
            r_SecondPlayerName.Text = "- computer -";
            r_SecondPlayerName.Size = new Size(k_TextBoxWidth, k_ButtonHeight);
            r_SecondPlayerName.Location = new Point(k_LeftMargin + k_LabelWidth + k_SpacingBetweenControls, currentTop);
            r_SecondPlayerName.Enabled = false;

            // Against a Friend Button
            r_AgainstAFriendButton.Text = "Against a Friend";
            r_AgainstAFriendButton.Size = new Size(k_LabelWidth-10 , k_ButtonHeight -10);
            r_AgainstAFriendButton.Location = new Point(k_LeftMargin + k_LabelWidth + k_SpacingBetweenControls+k_TextBoxWidth+k_SpacingBetweenControls, currentTop);
            r_AgainstAFriendButton.Click += againstAFriendButton_Click;

            currentTop += k_ButtonHeight + 2 * k_SpacingBetweenControls;

            // Board Size Label
            r_BoardSizeLabel.Text = "Board Size:";
            r_BoardSizeLabel.Size = new Size(k_LabelWidth, k_ButtonHeight);
            r_BoardSizeLabel.Location = new Point(k_SpacingBetweenControls, currentTop);

            // Board Size Button
            r_BoardSizeButton.Text = $"{r_SizesOfGameBoard[m_CurrentBoardSizeIndex].Item1} x {r_SizesOfGameBoard[m_CurrentBoardSizeIndex].Item2}";
            r_BoardSizeButton.Size = new Size(k_ButtonWidth, k_ButtonHeight+ 2 * k_SpacingBetweenControls);
            r_BoardSizeButton.Location = new Point(k_SpacingBetweenControls, currentTop + 2*k_SpacingBetweenControls);
            r_BoardSizeButton.Click += boardSizeButton_Click;
            r_BoardSizeButton.BackColor = Color.PaleVioletRed;

            currentTop += k_ButtonHeight + k_SpacingBetweenControls;

            // Start Button
            r_StartButton.Text = "Start!";
            r_StartButton.Size = new Size(k_ButtonWidth - 2*k_SpacingBetweenControls, k_ButtonHeight);
            //r_StartButton.Location = new Point(k_LeftMargin, currentTop);
            r_StartButton.Location= new Point( this.ClientSize.Width - r_StartButton.Width - k_SpacingBetweenControls, this.ClientSize.Height - r_StartButton.Height - k_SpacingBetweenControls);
            r_StartButton.Click += startButton_Click;
            r_StartButton.BackColor = Color.PaleGreen;

            // Adding Controls to the Form
            this.Controls.AddRange(new Control[]
            {
                r_FirstPlayerLabel, r_FirstPlayerName,
                r_SecondPlayerLabel, r_SecondPlayerName,
                r_AgainstAFriendButton, r_BoardSizeButton, r_StartButton, r_BoardSizeLabel
            });
        }

        private void againstAFriendButton_Click(object sender, EventArgs e)
        {
            m_TextBoxFriendEnabled = !m_TextBoxFriendEnabled;

            if (m_TextBoxFriendEnabled)
            {
                r_SecondPlayerName.Enabled = true;
                r_SecondPlayerName.Text = "";
                r_AgainstAFriendButton.Text = "Against Computer";
            }
            else
            {
                r_SecondPlayerName.Enabled = false;
                r_SecondPlayerName.Text = "- computer -";
                r_AgainstAFriendButton.Text = "Against a Friend";
            }
        }

        private void boardSizeButton_Click(object sender, EventArgs e)
        {
            m_CurrentBoardSizeIndex = (m_CurrentBoardSizeIndex + 1) % r_SizesOfGameBoard.Count;
            var currentSize = r_SizesOfGameBoard[m_CurrentBoardSizeIndex];
            r_BoardSizeButton.Text = $"{currentSize.Item1} x {currentSize.Item2}";
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            string firstPlayer = r_FirstPlayerName.Text;
            bool isAgainstComputer = !m_TextBoxFriendEnabled;
            var boardSize = r_SizesOfGameBoard[m_CurrentBoardSizeIndex];
            string secondPlayer = m_TextBoxFriendEnabled ? r_SecondPlayerName.Text : "Computer";

            if (string.IsNullOrEmpty(firstPlayer))
            {
                MessageBox.Show("Please enter a name for the first player.");
                return;
            }
            
            if (m_TextBoxFriendEnabled && string.IsNullOrEmpty(secondPlayer))
            {
                MessageBox.Show("Please enter a name for the second player.");
                return;
            }
            
            this.DialogResult = DialogResult.OK;
            this.Hide();
            // Pass the parameters to the next form
            FormOfGameBoard newGame = new FormOfGameBoard(
                firstPlayer,
                secondPlayer,
                isAgainstComputer,
                boardSize.Item1, // Number of rows
                boardSize.Item2  // Number of columns
            );
            newGame.ShowDialog();
            this.Close();
        }
    }
}
