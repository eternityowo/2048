using System;
using System.Windows.Forms;

namespace Game2048
{
    class GameModel
    {
        #region Fields
        public event Action GridUpdated;
        public event Action<string> GameOver;
        public readonly int Size;
        public int[,] mainGrid;

        private int[,] _backupGrid;
        private Random _rndIndex;
        private bool _canUndo = false;
        private int _maxTile;
        private int _winTile;
        private int _turnScore;
        private int _currentScore;
        private bool[] _moveLock = new bool[4];
        #endregion

        #region Cosntructors
        public GameModel(int size)
        {
            Size = size;
        }
        #endregion

        #region MainLogic
        public void InitBoard()
        {
            mainGrid = new int[Size, Size];

            _rndIndex = new Random(DateTime.Now.Millisecond);
            _backupGrid = new int[Size, Size];
            _canUndo = false;

            _currentScore = 0;
            _maxTile = 0;
            _winTile = 64;
            _turnScore = 0;

            SpawnTile(true);
            SpawnTile(true);

            Repaint();
        }

        public void GameStep(KeyEventArgs args)
        {
            var keyCode = args.KeyValue;

            switch (keyCode)
            {
                case 37:
                case 38:
                case 39:
                case 40:
                case 87:
                case 65:
                case 83:
                case 68:
                    CreateGridBackup();
                    MoveTile(keyCode);
                    MergeTile(keyCode);
                    MoveTile(keyCode);
                    FindGreatestTile();
                    GameStateAction();
                    break;
                case 85: // u
                    Undo();
                    Repaint();
                    break;
                case 82: // r
                    InitBoard();
                    break;
            }
        }

        public void MoveTile(int keyCode)
        {
            switch (keyCode)
            {
                case 37: // 65, 37 A, <
                case 65:
                    for (int row = 0; row < Size; row++)
                        for (int column = 0; column < Size; column++)
                            if (mainGrid[row, column] == 0)
                                for (int k = column + 1; k < Size; k++)
                                    if (mainGrid[row, k] != 0)
                                    {
                                        mainGrid[row, column] = mainGrid[row, k];
                                        mainGrid[row, k] = 0;
                                        break;
                                    }
                    break;
                case 38: // 87, 38 w ^
                case 87:
                    for (int row = 0; row < Size; row++)
                        for (int column = 0; column < Size; column++)
                            if (mainGrid[column, row] == 0)
                                for (int k = column + 1; k < Size; k++)
                                    if (mainGrid[k, row] != 0)
                                    {
                                        mainGrid[column, row] = mainGrid[k, row];
                                        mainGrid[k, row] = 0;
                                        break;
                                    }

                    break;
                case 39: // 68, 39 D >
                case 68:
                    for (int row = 0; row < Size; row++)
                        for (int column = Size - 1; column >= 0; column--)
                            if (mainGrid[row, column] == 0)
                                for (int k = column - 1; k >= 0; k--)
                                    if (mainGrid[row, k] != 0)
                                    {
                                        mainGrid[row, column] = mainGrid[row, k];
                                        mainGrid[row, k] = 0;
                                        break;
                                    }

                    break;
                case 40: // 83, 40 s !^
                case 83:
                    for (int row = 0; row < Size; row++)
                        for (int column = Size - 1; column >= 0; column--)
                            if (mainGrid[column, row] == 0)
                                for (int k = column - 1; k >= 0; k--)
                                    if (mainGrid[k, row] != 0)
                                    {
                                        mainGrid[column, row] = mainGrid[k, row];
                                        mainGrid[k, row] = 0;
                                        break;
                                    }
                    break;
            }
        }

        private void MergeTile(int keyCode)
        {
            _turnScore = 0;
            int mergePairScore;

            switch (keyCode)
            {
                case 37: // 65, 37 A, <
                case 65:
                    for (int row = 0; row < Size; row++)
                        for (int column = 0; column < Size - 1; column++)
                            if (mainGrid[row, column] == mainGrid[row, column + 1])
                            {
                                mainGrid[row, column] += mainGrid[row, column + 1];
                                mainGrid[row, column + 1] = 0;
                                mergePairScore = CalculateScore(row, column);
                                _currentScore += mergePairScore;
                                _turnScore += mergePairScore;
                            }
                    if (_turnScore != 0)
                        Array.Clear(_moveLock, 0, 4);
                    else
                        _moveLock[0] = true;
                    break;
                case 38:
                case 87:
                    for (int row = 0; row < Size; row++)
                        for (int column = 0; column < Size - 1; column++)
                            if (mainGrid[column, row] == mainGrid[column + 1, row])
                            {
                                mainGrid[column, row] += mainGrid[column + 1, row];
                                mainGrid[column + 1, row] = 0;
                                mergePairScore = CalculateScore(column, row);
                                _currentScore += mergePairScore;
                                _turnScore += mergePairScore;
                            }
                    if (_turnScore != 0)
                        Array.Clear(_moveLock, 0, 4);
                    else
                        _moveLock[1] = true;
                    break;
                case 39: // 68, 39 D >
                case 68:
                    for (int row = 0; row < Size; row++)
                        for (int column = Size - 1; column > 0; column--)
                            if (mainGrid[row, column] == mainGrid[row, column - 1])
                            {
                                Array.Clear(_moveLock, 0, 4);
                                mainGrid[row, column] += mainGrid[row, column - 1];
                                mainGrid[row, column - 1] = 0;
                                mergePairScore = CalculateScore(row, column);
                                _currentScore += mergePairScore;
                                _turnScore += mergePairScore;
                            }
                    if (_turnScore != 0)
                        Array.Clear(_moveLock, 0, 4);
                    else
                        _moveLock[2] = true;
                    break;
                case 40: // 83, 40 s !^
                case 83:
                    for (int row = 0; row < Size; row++)
                        for (int column = Size - 1; column > 0; column--)
                            if (mainGrid[column, row] == mainGrid[column - 1, row])
                            {
                                Array.Clear(_moveLock, 0, 4);
                                mainGrid[column, row] += mainGrid[column - 1, row];
                                mainGrid[column - 1, row] = 0;
                                mergePairScore = CalculateScore(column, row);
                                _turnScore += mergePairScore;
                                _currentScore += mergePairScore;
                            }
                    if (_turnScore != 0)
                        Array.Clear(_moveLock, 0, 4);
                    else
                        _moveLock[3] = true;
                    break;
            }
        }

        private void GameStateAction()
        {
            int lockCount = 0;
            for (int i = 0; i < 4; i++) lockCount += _moveLock[i] ? 1 : 0;

            if (GridFull() && lockCount == 4)
            {
                GameOverMessage("LOSE");
            }
            else if (GridChanged())
            {
                SpawnTile();
                Repaint();
                if (_maxTile == _winTile)
                    GameOverMessage("WIN");
            }
        }

        private void CreateGridBackup()
        {
            _backupGrid = (int[,])mainGrid.Clone();
        }

        private void Undo()
        {
            if (!_canUndo)
                return;

            _currentScore -= _turnScore;
            mainGrid = (int[,])_backupGrid.Clone();

            _canUndo = false;
        }

        private void FindGreatestTile()
        {
            for (int row = 0; row < Size; row++)
                for (int column = 0; column < Size; column++)
                    if (mainGrid[row, column] > _maxTile)
                        _maxTile = mainGrid[row, column];
        }

        private bool GridFull()
        {
            for (int row = 0; row < Size; row++)
                for (int column = 0; column < Size; column++)
                {
                    if (mainGrid[row, column] == 0)
                        return false;
                }
            return true;
        }

        private bool GridChanged()
        {
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    if (_backupGrid[i, j] != mainGrid[i, j])
                    {
                        _canUndo = true;
                        return true;
                    }
            return false;
        }

        private void SpawnTile(bool initBlock = false)
        {
            int chanceSpawnBigBlock;
            int row, column;

            row = _rndIndex.Next(Size);
            column = _rndIndex.Next(Size);
            do
            {
                row = _rndIndex.Next(Size);
                column = _rndIndex.Next(Size);
                chanceSpawnBigBlock = _rndIndex.Next(10);
            } while (mainGrid[row, column] != 0);

            if (chanceSpawnBigBlock < 2 && !initBlock)
                mainGrid[row, column] = 4;
            else
                mainGrid[row, column] = 2;
        }
        #endregion

        #region EventInvoke
        private void Repaint()
        {
            GridUpdated?.Invoke();
        }

        private void GameOverMessage(string msg)
        {
            GameOver?.Invoke(msg);
        }
        #endregion

        #region FieldsAccses
        public void SetNewWinTile()
        {
            _winTile *= 2;
        }

        public string GetStats()
        {
            return $"Score: {_currentScore}    Win: {_winTile}";
        }

        public string GetFinalyStats()
        {
            return $"Score: {_currentScore}\nMax Tile: {_maxTile}\nWin Tile: { _winTile}\nNext Win Tile: {_winTile * 2}";
        }
        #endregion

        #region Helpers
        private int CalculateScore(int iRow, int iColumn)
        {
            return (int)(Math.Log(mainGrid[iRow, iColumn], 2) - 1) * mainGrid[iRow, iColumn];
        }

        #endregion
    }
}
