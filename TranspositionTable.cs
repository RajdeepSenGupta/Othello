using System;
using System.Text;
using System.Collections;

namespace Othello
{
    enum TranspositionTableElementType
    {
        Exact,
        Alpha,
        Beta
    }

    struct TranspositionTableElement
    {
        public int HashCode;
        public int Value;
        public TranspositionTableElementType ElementType;
        public byte Depth;
        public byte X;
        public byte Y;
        public bool HasMove;
    }

    /// <summary>
    /// Summary description for TranspositionTable.
    /// </summary>
    class TranspositionTable
    {
        const int _tableSize = 2000000;

        private TranspositionTableElement[] _table;
        internal int _wrongEntriesFound;
        internal int _tooShallowEntriesFound;

        public TranspositionTable()
        {
            _table = new TranspositionTableElement[_tableSize];

            for (int i = 0; i < _tableSize; i++)
                _table[i].Value = AlphaBetaOthelloPlayer.INVALID_MOVE;
        }

        public void AddEntry(int hashCode, int depth, int val,
            TranspositionTableElementType type, int x, int y)
        {

            uint index = (uint)hashCode % _tableSize;

            _table[index].HashCode = hashCode;
            _table[index].Depth = (byte)depth;
            _table[index].Value = val;
            _table[index].ElementType = type;
            if (x >= 0)
            {
                _table[index].HasMove = true;
                _table[index].X = (byte)x;
                _table[index].Y = (byte)y;
            }
            else
            {
                _table[index].HasMove = false;
            }
        }

        public int LookupEntry(int hashCode, int depth, int alpha, int beta, ref int x, ref int y)
        {
            TranspositionTableElement e = _table[(uint)hashCode % _tableSize];

            // Did we find something?
            if (e.Value == AlphaBetaOthelloPlayer.INVALID_MOVE)
                return AlphaBetaOthelloPlayer.INVALID_MOVE;

            // Is it the right one?                
            if (e.HashCode != hashCode)
            {
                _wrongEntriesFound++;
                return AlphaBetaOthelloPlayer.INVALID_MOVE;
            }

            // Is it deep enough?
            if (e.Depth < depth)
            {
                _tooShallowEntriesFound++;
                return AlphaBetaOthelloPlayer.INVALID_MOVE;
            }

            // No reason to return the move if the match is exact
            if (e.ElementType == TranspositionTableElementType.Exact)
                return e.Value;

            if (e.HasMove)
            {
                x = e.X;
                y = e.Y;
            }

            if (e.ElementType == TranspositionTableElementType.Alpha && e.Value < alpha)
                return alpha;

            if (e.ElementType == TranspositionTableElementType.Beta && e.Value > beta)
                return beta;

            return AlphaBetaOthelloPlayer.INVALID_MOVE;
        }
    }
}
