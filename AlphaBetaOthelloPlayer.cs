#define USE_ITERATIVE_DEEPENING
#define USE_TRANSPOSITION_TABLE

using System;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Othello
{
    /// <summary>
    /// Summary description for AlphaBetaOthelloPlayer.
    /// </summary>
    public abstract class AlphaBetaOthelloPlayer : OthelloPlayer
    {
        const int BEST_MOVE = 10000;
        const int WORST_MOVE = -BEST_MOVE;
        internal const int INVALID_MOVE = 7777777;

        private int _finalEvaluations;
        private int _boardEvaluations;
#if USE_TRANSPOSITION_TABLE
        private int _transpositionHits;
        private static TranspositionTable _transpositionTable = new TranspositionTable();
#endif

        class MoveWithValue : IComparable
        {
            public OthelloMoveWithData Move;
            public int Value;

            public virtual int CompareTo(object o)
            {
                return ((MoveWithValue)o).Value - Value;
            }
        }

        static AlphaBetaOthelloPlayer()
        {
            //ReadBook();
        }

        static void ReadBook()
        {
#if USE_TRANSPOSITION_TABLE
            StreamReader reader = null;

            try
            {
                reader = new StreamReader("book.txt");
            }
            catch
            {
                // No opening book, oh well...
                return;
            }

            // Each line is a game, and looks like: +d3-c3+c4-e3+c2-b3+d2...-a5+a7-a8+b8-b7: -04 10
            // -04 is the final score for black.  10 is always there and should be ignored
            for (int x = 0; ; x++)
            {
                string s = reader.ReadLine();
                if (s == null)
                    break;

                OthelloBoard board = OthelloBoard.CreateStartingBoard();

                int i;
                i = s.LastIndexOf(':');
                int val = int.Parse(s.Substring(i + 2, 3));
                if (val < 0)
                    val -= 500;
                else if (val > 0)
                    val += 500;

                // Only use the first 10 moves to limit size
                for (i = 0; i < (10 * 3); i += 3)
                {
                    if (s[i] == ':')
                        break;
                    OthelloMove move = new OthelloMove((s[i + 1] - 'a'), (s[i + 2] - '1'));
                    board.PlayMove(move);
                    board.FixUpCurrentPlayer();
                    _transpositionTable.AddEntry(board.GetHashCode(), 60 /*ply*/,
                        (board.CurrentPlayer == SquareState.Black) ? val : -val,
                        TranspositionTableElementType.Exact, -1, -1);
                    move = null;
                }

                board.FixUpCurrentPlayer();
                /*
                int expectedVal = board.BlackSquares - board.WhiteSquares;
                Debug.Assert(val == expectedVal);
                Debug.Assert(board.GameOver);
                */
                board = null;
            }

            reader.Close();
#endif
        }

        public override PlayerResult GetMove(OthelloBoard board)
        {

            StringBuilder statusString = new StringBuilder(); ;

            _finalEvaluations = 0;
            _boardEvaluations = 0;
#if USE_TRANSPOSITION_TABLE
            _transpositionHits = 0;
            _transpositionTable._wrongEntriesFound = 0;
            _transpositionTable._tooShallowEntriesFound = 0;
#endif
            DateTime startTime = DateTime.Now;

            OthelloMoveWithData[] moves = board.GetValidMovesWithData();
            MoveWithValue[] movesWithValue = new MoveWithValue[moves.Length];
            for (int i = 0; i < moves.Length; i++)
            {
                movesWithValue[i] = new MoveWithValue();
                movesWithValue[i].Move = moves[i];
            }

            int currentPly;
#if USE_ITERATIVE_DEEPENING
            for (currentPly = 0; ; currentPly++)
            {
                //for (currentPly = 0; currentPly<2; currentPly++) {
#else
            for (int currentPly = ply-1; currentPly<ply; currentPly++) {
#endif

                statusString.Length = 0;
                DateTime startIterationTime = DateTime.Now;

                statusString.Append("Depth " + (currentPly + 1).ToString() + ": ");

                // Should we do a final search
                if (board.EmptySquares <= 15 && currentPly > 5)
                    currentPly = board.EmptySquares;

                int alpha = WORST_MOVE;
                int beta = BEST_MOVE;
                foreach (MoveWithValue move in movesWithValue)
                {
                    board.PlayMove(move.Move);

                    statusString.Append(move.Move.ToString());
                    this.PostStatus(statusString.ToString());

                    move.Value = (-GetBoardValueRecursive(board, currentPly, 2, -beta, -alpha));

                    if (move.Value != BEST_MOVE && move.Value != WORST_MOVE)
                    {
                        statusString.Append("(" + move.Value.ToString() + ")");
                    }
                    statusString.Append(" ");
                    this.PostStatus(statusString.ToString());

                    board.UnplayMove(move.Move);

                    if (move.Value > alpha) alpha = move.Value;
                    if (alpha >= beta) break;
                }

                Array.Sort(movesWithValue);

                // If we've already search to the end, we're done
                if (currentPly == board.EmptySquares)
                    break;

                if (movesWithValue[0].Value < -500 || movesWithValue[0].Value > 500)
                    break;

                TimeSpan t = DateTime.Now - startTime;
                if (t.TotalSeconds >= 3)
                    break;
            }

            PlayerResult result = new PlayerResult();
            result.Move = movesWithValue[0].Move;
            TimeSpan processingTime = DateTime.Now - startTime;
            statusString.Append("\r\nEvals: " + _boardEvaluations + " Final evals: " + _finalEvaluations + "\r\n");
#if USE_TRANSPOSITION_TABLE
            /*
            statusString.Append("Transp: " + _transpositionHits +
                " wrong hits: " + _transpositionTable._wrongEntriesFound +
                " shallow hits: " + _transpositionTable._tooShallowEntriesFound + "\r\n");
            */
#endif
            statusString.Append("Score: " + movesWithValue[0].Value + " (" + processingTime + ")");
            result.Status = statusString.ToString();
            return result;
        }

        protected abstract int GetBoardValue(OthelloBoard board);

        private void MyTrace(int ply, string message)
        {
            for (int i = 0; i < 10 - ply; i++)
                Trace.Write(" ");
            Trace.WriteLine(message);
        }

        private int GetBoardValueRecursive(OthelloBoard board, int ply,
            int notStuck, int alpha, int beta)
        {
            //MyTrace(ply, "left="+board.EmptySquares+" alpha="+alpha+" beta="+beta);

            /* If the game is over */
            if (notStuck == 0 || board.EmptySquares == 0)
            {

                _finalEvaluations++;
                int squareDifference = board.PlayerSquares - board.OpponentSquares;

                if (squareDifference > 0) return squareDifference + 1000;
                if (squareDifference < 0) return squareDifference - 1000;

                // Tie
                return 0;
            }

#if USE_TRANSPOSITION_TABLE
            int moveToTryFirst_X = -1, moveToTryFirst_Y = -1;
            bool needToTryFirstMove = false;
            int boardHashCode = board.GetHashCode();
            int transpositionValue = _transpositionTable.LookupEntry(boardHashCode, ply,
                alpha, beta, ref moveToTryFirst_X, ref moveToTryFirst_Y);

            if (transpositionValue != AlphaBetaOthelloPlayer.INVALID_MOVE)
            {
                _transpositionHits++;
                return transpositionValue;
            }

            if (moveToTryFirst_X >= 0)
                needToTryFirstMove = true;

            TranspositionTableElementType elementType = TranspositionTableElementType.Alpha;
            OthelloMoveWithData bestMove = null;
#endif

            if (ply == 0)
            {
                _boardEvaluations++;
                int val = GetBoardValue(board);
#if USE_TRANSPOSITION_TABLE
                _transpositionTable.AddEntry(boardHashCode, 0, val,
                    TranspositionTableElementType.Exact, -1, -1);
#endif
                return val;
            }

            int count = 0;
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    OthelloMoveWithData move;

#if USE_TRANSPOSITION_TABLE
                    if (needToTryFirstMove)
                    {
                        move = board.GetMoveInfoIfValid(moveToTryFirst_X, moveToTryFirst_Y);
                        needToTryFirstMove = false;

                        // Set x to -1 to make sure that (0,0) gets process next time around
                        x = -1;

                        // This could happen if we found an incorrect entry in the table
                        if (move == null) continue;
                    }
                    else
                    {
                        // Skip this move if we've already tried it
                        if (x == moveToTryFirst_X && y == moveToTryFirst_Y)
                            continue;

                        move = board.GetMoveInfoIfValid(x, y);
                        if (move == null) continue;
                    }
#else
                    move = board.GetMoveInfoIfValid(x, y);
                    if (move == null) continue;
#endif

                    count++;

                    /* perform the move */
                    board.PlayMove(move);

                    //MyTrace(ply, move.X + "," + move.Y + " alpha="+alpha+" beta="+beta);
                    int val = (-GetBoardValueRecursive(board, ply - 1, 2, -beta, -alpha));
                    //MyTrace(ply, move.X + "," + move.Y + " alpha="+alpha+" beta="+beta+" val="+val);

                    board.UnplayMove(move);

                    if (val >= beta)
                    {
#if USE_TRANSPOSITION_TABLE
                        _transpositionTable.AddEntry(boardHashCode, ply, beta,
                            TranspositionTableElementType.Beta, move.X, move.Y);
#endif
                        return BEST_MOVE;
                    }

                    if (val > alpha)
                    {
                        alpha = val;
#if USE_TRANSPOSITION_TABLE
                        elementType = TranspositionTableElementType.Exact;
                        bestMove = move;
#endif
                    }
                }
            }

            if (count == 0)
            {        /* if no possible move */
                board.SwitchPlayer();
                int val = -GetBoardValueRecursive(board, ply, notStuck - 1, -beta, -alpha);
                board.SwitchPlayer();
                return val;
            }

#if USE_TRANSPOSITION_TABLE
            if (bestMove != null)
                _transpositionTable.AddEntry(boardHashCode, ply, alpha, elementType, bestMove.X, bestMove.Y);
            else
                _transpositionTable.AddEntry(boardHashCode, ply, alpha, elementType, -1, -1);
#endif

            return alpha;
        }
    }
}
