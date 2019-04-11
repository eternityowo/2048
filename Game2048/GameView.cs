using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game2048
{
    class GameView : Form
    {
        GameModel game;
        TableLayoutPanel table;
        TableLayoutPanel grid;
        Label stats;

        private readonly Dictionary<int, Color> _colorDictionary = new Dictionary<int, Color>();

        public GameView(GameModel game)
        {
            // Init color Dictionary
            _colorDictionary.Add(0, Color.FromArgb(51, 63, 77));
            _colorDictionary.Add(2, Color.FromArgb(129, 128, 215));
            _colorDictionary.Add(4, Color.FromArgb(162, 90, 214));
            _colorDictionary.Add(8, Color.FromArgb(38, 105, 221));
            _colorDictionary.Add(16, Color.FromArgb(33, 185, 213));
            _colorDictionary.Add(32, Color.FromArgb(0, 202, 155));
            _colorDictionary.Add(64, Color.FromArgb(67, 207, 23));
            _colorDictionary.Add(128, Color.FromArgb(247, 192, 1));
            _colorDictionary.Add(256, Color.FromArgb(245, 129, 20));
            _colorDictionary.Add(512, Color.FromArgb(255, 84, 61));
            _colorDictionary.Add(1024, Color.FromArgb(255, 20, 145));
            _colorDictionary.Add(2048, Color.FromArgb(190, 0, 129));
            _colorDictionary.Add(4096, Color.FromArgb(20, 255, 255));
            _colorDictionary.Add(8192, Color.FromArgb(255, 144, 222));
            _colorDictionary.Add(16384, Color.FromArgb(60, 0, 219));
            _colorDictionary.Add(32768, Color.FromArgb(255, 207, 171));
            _colorDictionary.Add(65536, Color.FromArgb(255, 0, 45));
            _colorDictionary.Add(131072, Color.FromArgb(255, 255, 0));

            // Init Form
            Text = "Game 2048";
            BackColor = Color.FromArgb(75, 88, 103);

            this.game = game;

            // Init table 
            table = new TableLayoutPanel();
            table.Dock = DockStyle.Fill;
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 90));

            // Init grid
            grid = new TableLayoutPanel();
            grid.Dock = DockStyle.Fill;
            for (int i = 0; i < game.Size; i++)
            {
                grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / game.Size));
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / game.Size));
            }
            // cache grid cell 
            var tiles = new Label[game.Size, game.Size];

            for (int column = 0; column < game.Size; column++)
                for (int row = 0; row < game.Size; row++)
                {
                    var cell = new Label
                    {
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Fill,
                        Margin = new Padding(1)
                    };
                    tiles[row, column] = cell;
                    grid.Controls.Add(cell, column, row);
                }

            // Init stats
            stats = new Label()
            {
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 20F, FontStyle.Bold)
            };

            // add all element to Form
            table.Controls.Add(stats);
            table.Controls.Add(grid);
            Controls.Add(table);

            // View notify Model (User Input)
            KeyDown += (sender, args) =>
            {
                game.GameStep(args);
            };

            // Model update View
            game.GridUpdated += () =>
            {
                stats.Text = game.GetStats();

                for (int row = 0; row < game.Size; row++)
                    for (int column = 0; column < game.Size; column++)
                    {
                        var cell = tiles[row, column];
                        var value = game.mainGrid[row, column];
                        cell.Text = value.ToString();
                        if (_colorDictionary.ContainsKey(value))
                            cell.BackColor = _colorDictionary[value];
                        else
                            cell.BackColor = Color.White;
                    }
            };

            game.GameOver += (message) =>
            {
                var WIN = message.Equals("WIN");
                stats.Text = WIN ? "You win!" : "You lose!";

                if (MessageBox.Show(game.GetFinalyStats(), WIN ? "Continue ?" : "Try again ?", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if(WIN)
                        game.SetNewWinTile();
                    else
                        game.SetNewWinTile();
                }
                else
                {
                    Dispose();
                }
            };

            game.InitBoard();
        }

        static void Main(string[] args)
        {
            var size = 8;
            var cellSize = 100;
            Application.Run(new GameView(new GameModel(size)) { ClientSize = new Size(size * cellSize, size * cellSize) });
        }
    }
}
