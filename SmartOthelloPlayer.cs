using System;
using System.Collections;

namespace Othello
{
    /// <summary>
    /// Summary description for SmartOthelloPlayer.
    /// </summary>
    public class SmartOthelloPlayer : AlphaBetaOthelloPlayer
    {
        protected override int GetBoardValue(OthelloBoard board)
        {
            int ret = 0;

            /************** X-SQUARE VALUE *******/
            ret += XSquareValue(board);
            ret += SquareValue(board);

            /************** MOBILITY ***********/
            ret += Mobility(board);

            return ret;
        }

        /* returns the difference of possible moves for the players */
        private int Mobility(OthelloBoard board)
        {

            int ret = board.CountValidMoves();
            board.CurrentPlayer = board.OtherPlayer;
            ret -= board.CountValidMoves();
            board.CurrentPlayer = board.OtherPlayer;

            return ret;
        }

        /* Value of the X-squares */
        private int XSquareValue(OthelloBoard board)
        {
            int ret = 0;
            const int XSquarePenalty = 10;

            if (board[0, 0] == SquareState.Empty)
            {
                if (board[1, 1] == board.CurrentPlayer) ret -= XSquarePenalty;
                else if (board[1, 1] == board.OtherPlayer) ret += XSquarePenalty;
            }

            if (board[7, 0] == SquareState.Empty)
            {
                if (board[6, 1] == board.CurrentPlayer) ret -= XSquarePenalty;
                else if (board[6, 1] == board.OtherPlayer) ret += XSquarePenalty;
            }

            if (board[0, 7] == SquareState.Empty)
            {
                if (board[1, 6] == board.CurrentPlayer) ret -= XSquarePenalty;
                else if (board[1, 6] == board.OtherPlayer) ret += XSquarePenalty;
            }

            if (board[7, 7] == SquareState.Empty)
            {
                if (board[6, 6] == board.CurrentPlayer) ret -= XSquarePenalty;
                else if (board[6, 6] == board.OtherPlayer) ret += XSquarePenalty;
            }

            return ret;
        }

        private int SquareValue(OthelloBoard board)
        {
            int ret = 0;
            const int SquarePenalty = 10;

            if (board[0, 0] == board.CurrentPlayer) ret += SquarePenalty;
            else if (board[0, 0] == board.OtherPlayer) ret -= SquarePenalty;

            if (board[7, 0] == board.CurrentPlayer) ret += SquarePenalty;
            else if (board[7, 0] == board.OtherPlayer) ret -= SquarePenalty;

            if (board[0, 7] == board.CurrentPlayer) ret += SquarePenalty;
            else if (board[0, 7] == board.OtherPlayer) ret -= SquarePenalty;

            if (board[7, 7] == board.CurrentPlayer) ret += SquarePenalty;
            else if (board[7, 7] == board.OtherPlayer) ret -= SquarePenalty;

            return ret;
        }
    }
}
