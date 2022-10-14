using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicTacToe.enums;

namespace TicTacToe
{
    public partial class TicTacToe : Form
    {
        private GameStateEnum _gameState = GameStateEnum.Continue;
        private int _pointsPlayer;
        private int _pointsBot;
        private const int NrOfRows = 3;
        private const int NrOfColumns = 3;
        private const string PlayerX = "x";
        private const string BotO = "o";

        public TicTacToe()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Text.Equals(""))
                {
                    if (_gameState == GameStateEnum.Continue)
                    {
                        button.Text = PlayerX;
                        await CheckForWin();
                        CheckForDraw();
                    }
                }
            }
        }
        
        private GameStateEnum CheckIfWon(List<List<string>> gameField)
        {
            if (gameField == null)
            {
                gameField = new List<List<string>>
                {
                    new List<string>(new[] { GetButtonText(1), GetButtonText(2), GetButtonText(3) }),
                    new List<string>(new[] { GetButtonText(4), GetButtonText(5), GetButtonText(6) }),
                    new List<string>(new[] { GetButtonText(7), GetButtonText(8), GetButtonText(9) })
                };
            }

            //CHECK HORIZONTAL
            for (int i = 0; i < NrOfRows; ++i)
            {
                if (gameField[i].Distinct().Count() == 1 && !gameField[i][0].Equals(""))
                {
                    Debug.Print(gameField[i][0] + " hor");
                    return gameField[i][0] == PlayerX ? GameStateEnum.Won : GameStateEnum.Lost;
                }
            }

            //CHECK VERTICAL
            for (int i = 0; i < NrOfColumns; ++i)
            {
                List<string> list = new List<string>(new[] { gameField[0][i], gameField[1][i], gameField[2][i] });
                if (list.Distinct().Count() == 1 && !list[0].Equals(""))
                {
                    Debug.Print(list[0] + " ver");
                    return list[0] == PlayerX ? GameStateEnum.Won : GameStateEnum.Lost;
                }
            }

            //CHECK DIAGONAL
            List<string> list1 = new List<string>(new[] { gameField[0][0], gameField[1][1], gameField[2][2] });
            List<string> list2 = new List<string>(new[] { gameField[0][2], gameField[1][1], gameField[2][0] });
            if (list1.Distinct().Count() == 1 && !list1[0].Equals(""))
            {
                Debug.Print(list1[0] + " dio1");
                return list1[0] == PlayerX ? GameStateEnum.Won : GameStateEnum.Lost;
            }

            if (list2.Distinct().Count() == 1 && !list2[0].Equals(""))
            {
                Debug.Print(list2[0] + " dio2");
                return list2[0] == PlayerX ? GameStateEnum.Won : GameStateEnum.Lost;
            }

            return GameStateEnum.Continue;
        }

        private string GetButtonText(int button)
        {
            return Controls.Find("button" + button, true)[0].Text;
        }

        private async Task CheckForWin()
        {
            if (CheckIfWon(null) == GameStateEnum.Won)
            {
                BackColor = Color.SeaGreen;
                _gameState = GameStateEnum.Won;
                ++_pointsPlayer;
                pointsPlayer.Text = @"You: " + _pointsPlayer;
            }
            else
            {
                await PlayOpponentTurn();
            }

            if (CheckIfWon(null) == GameStateEnum.Lost)
            {
                BackColor = Color.Tomato;
                _gameState = GameStateEnum.Lost;
                ++_pointsBot;
                pointsBot.Text = @"Bot: " + _pointsBot;
            }
        }

        private void CheckForDraw()
        {
            if (BackColor == Color.White)
            {
                for (int i = 1; i < 10; ++i)
                {
                    if (Controls.Find("button" + i, true)[0].Text == "")
                    {
                        return;
                    }
                }

                BackColor = Color.Yellow;
            }
        }

        private async Task PlayOpponentTurn()
        {
            int botAbleToWinNum = await IsAbleToWinNextRound(BotO, GameStateEnum.Lost);
            Debug.Print("bot: " + botAbleToWinNum);
            if (botAbleToWinNum != 0)
            {
                Debug.Print("BotWin");
                Controls.Find("button" + botAbleToWinNum, true)[0].Text = BotO;
                return;
            }

            int playerAbleToWinNum = await IsAbleToWinNextRound(PlayerX, GameStateEnum.Won);
            Debug.Print("player: " + playerAbleToWinNum);
            if (playerAbleToWinNum != 0)
            {
                Debug.Print("PlayerWin");
                Controls.Find("button" + playerAbleToWinNum, true)[0].Text = BotO;
                return;
            }

            for (int i = 0; i < 1000; ++i)
            {
                Random random = new Random();
                int button = random.Next(1, 10);
                if (GetButtonText(button).Equals(""))
                {
                    Controls.Find("button" + button, true)[0].Text = BotO;
                    return;
                }
            }

            //PLAY BACKUP (if it didn't find a spot in 1000 tries)
            for (int i = 1; i < 10; ++i)
            {
                if (GetButtonText(i) == "")
                {
                    Controls.Find("button" + i, true)[0].Text = BotO;
                    return;
                }
            }
        }

        private Task<int> IsAbleToWinNextRound(string team, GameStateEnum gameState)
        {
            for (int i = 1; i < 10; ++i)
            {
                List<List<string>> gameField = new List<List<string>>();
                gameField.Insert(0, new List<string>(new[] { GetButtonText(1), GetButtonText(2), GetButtonText(3) }));
                gameField.Insert(1, new List<string>(new[] { GetButtonText(4), GetButtonText(5), GetButtonText(6) }));
                gameField.Insert(2, new List<string>(new[] { GetButtonText(7), GetButtonText(8), GetButtonText(9) }));
                if (gameField[(i - 1) / 3][(i - 1) % 3] == "")
                {
                    gameField[(i - 1) / 3][(i - 1) % 3] = team;
                    if (CheckIfWon(gameField) == gameState) return Task.FromResult(i);
                }
            }
            return Task.FromResult(0);
        }

        private void retryButton_Click(object sender, EventArgs e)
        {
            for (int i = 1; i < 10; ++i)
            {
                Controls.Find("button" + i, true)[0].Text = "";
                BackColor = Color.White;
                _gameState = GameStateEnum.Continue;
            }
        }
    }
}