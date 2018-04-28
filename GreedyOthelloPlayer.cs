using System;
using System.Collections;

namespace Othello
{
    /// <summary>
    /// Summary description for GreedyOthelloPlayer.
    /// </summary>
    public class GreedyOthelloPlayer : OthelloPlayer
    {
        public override PlayerResult GetMove(OthelloBoard board)
        {
            ArrayList moves = board.GetValidMoves();

            int currentBestCapture = 0;
            OthelloMove currentBestMove = null;
            foreach (OthelloMove move in moves)
            {
                int capture = board.NumberCaptured(move);
                if (capture > currentBestCapture)
                {
                    currentBestMove = move;
                    currentBestCapture = capture;
                }
            }

            PlayerResult result = new PlayerResult();
            result.Move = currentBestMove;
            result.Status = "Greedy: " + currentBestCapture.ToString();
            return result;
        }
    }
}
