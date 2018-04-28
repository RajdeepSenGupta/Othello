namespace Othello
{
    /// <summary>
    /// Greedy player with alpha beta search
    /// </summary>
    public class GreedyOthelloPlayer : AlphaBetaOthelloPlayer
    {
        protected override int GetBoardValue(OthelloBoard board)
        {
            return board.PlayerSquares - board.OpponentSquares;
        }
    }
}
