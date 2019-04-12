using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game2048
{
    class GameView : ThemeForm
    {
        GameModel game;
        TableLayoutPanel table;
        TableLayoutPanel grid;
        Label stats;

        public GameView(GameModel game)
        {
            this.game = game;

            // Init Form
            MinimumSize = new Size(250, 250);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

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
            var tiles = new ThemeTile[game.Size, game.Size];

            for (int column = 0; column < game.Size; column++)
                for (int row = 0; row < game.Size; row++)
                {
                    var cell = new ThemeTile();
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

            SizeChanged += (sender, args) =>
            {
                Font = new Font("Arial", ClientSize.Height / 50F, FontStyle.Bold);
                stats.Font = new Font("Arial", ClientSize.Height / 35F, FontStyle.Bold);
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
                        if (ThemeTile.TileColors.ContainsKey(value))
                            cell.BackColor = ThemeTile.TileColors[value];
                        else
                            cell.BackColor = Color.White;
                    }
            };

            game.GameOver += (message) =>
            {
                var WIN = message.Equals("WIN");
                stats.Text = WIN ? "You win!" : "You lose!";

                if (MessageBox.Show(game.GetFinalyStats(), WIN ? "Continue ?" : "Try again ?", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    if(WIN)
                        game.SetNewWinTile();
                    else
                        game.InitBoard();
                else
                    Dispose();
            };

            game.InitBoard();
        }

        static void Main(string[] args)
        {
            var size = 4;
            var cellSize = 80;
            Application.Run(new GameView(new GameModel(size, 64)) { ClientSize = new Size(size * cellSize, size * cellSize) });
        }
    }
}
