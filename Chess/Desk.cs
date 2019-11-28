﻿using Chess.Figures;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    public class Desk
    {
        const int DESKSIZE = 8;
        Figure[,] _deskGrid = new Figure[DESKSIZE, DESKSIZE];
        SFigurePosition _activeFigure = new SFigurePosition(-1, -1);
        FigureColor _activePlayerColor = FigureColor.White;

        Player _playerWhite = new Player(FigureColor.White);
        Player _playerBlack = new Player(FigureColor.Black);

        public delegate void ChangeColor(int i, int j);
        public event ChangeColor RepaintCell;

        public delegate void RefreshForm();
        public event RefreshForm Refresh;

        public void ClearDesk()
        {
            for (int i = 0; i < DESKSIZE; i++)
            {
                _deskGrid[1, i] = new Pawn(FigureColor.Black);
                _deskGrid[6, i] = new Pawn(FigureColor.White);
            }
            _deskGrid[0, 0] = new Rook(FigureColor.Black);
            _deskGrid[0, 7] = new Rook(FigureColor.Black);
            _deskGrid[7, 0] = new Rook(FigureColor.White);
            _deskGrid[7, 7] = new Rook(FigureColor.White);

            _deskGrid[0, 1] = new Horse(FigureColor.Black);
            _deskGrid[0, 6] = new Horse(FigureColor.Black);
            _deskGrid[7, 1] = new Horse(FigureColor.White);
            _deskGrid[7, 6] = new Horse(FigureColor.White);

            _deskGrid[0, 2] = new Bishop(FigureColor.Black);
            _deskGrid[0, 5] = new Bishop(FigureColor.Black);
            _deskGrid[7, 2] = new Bishop(FigureColor.White);
            _deskGrid[7, 5] = new Bishop(FigureColor.White);

            _deskGrid[0, 3] = new Queen(FigureColor.Black);
            _deskGrid[0, 4] = new King(FigureColor.Black);
            _deskGrid[7, 3] = new Queen(FigureColor.White);
            _deskGrid[7, 4] = new King(FigureColor.White);

            _activePlayerColor = FigureColor.White;
        }
        public Figure this[int i, int j]
        {
            get => _deskGrid[i, j];
        }

        public void ChoseAction(int i, int j)
        {
            if (_activeFigure.X == -1 || _activeFigure.Y == -1)
            {
                if (_deskGrid[i, j] == null || _activePlayerColor != _deskGrid[i, j]?.Color)
                {
                    return;
                }

                SetActive(i, j);
            }
            else
            {
                if (_activePlayerColor == _deskGrid[i, j]?.Color)
                {
                    SetActive(i, j);
                }

                Move(i, j);                
            }
        }
        void SetActive(int i, int j)
        {
            _activeFigure.SetPosition(i, j);

            for (int idx = 0; idx < DESKSIZE; idx++)
            {
                for (int idy = 0; idy < DESKSIZE; idy++)
                {
                    if (_deskGrid[i, j].CheckMove(new SFigurePosition(idx, idy),
                        new SFigurePosition(i, j),
                        ref _deskGrid) != MoveState.Cannot)
                    {
                        RepaintCell?.Invoke(idx, idy);
                    }
                }
            }
        }
        void Move(int i, int j)
        {
            var moveState = _deskGrid[_activeFigure.X, _activeFigure.Y].CheckMove(
                   new SFigurePosition(i, j),
                   new SFigurePosition(_activeFigure.X, _activeFigure.Y),
                   ref _deskGrid);

            if (moveState == MoveState.Cannot)
            {
                _activeFigure.SetPosition(-1, -1);
                return;
            }

            if (moveState == MoveState.Fight)
            {
                if (_activePlayerColor == FigureColor.White)
                {
                    _playerBlack._deadFigures.Add(_deskGrid[i, j]);
                }
                else
                {
                    _playerWhite._deadFigures.Add(_deskGrid[i, j]);
                }
            }

            _deskGrid[i, j] = _deskGrid[_activeFigure.X, _activeFigure.Y];
            _deskGrid[_activeFigure.X, _activeFigure.Y] = null;
            _activeFigure.SetPosition(-1, -1);

            Refresh.Invoke();

            _activePlayerColor = (_activePlayerColor == FigureColor.White) ? FigureColor.Black : FigureColor.White;
        }
    }
}
