using System;
using System.Collections;
using System.IO;
using System.Diagnostics;

namespace Othello
{
    public enum SquareState { Empty, Black, White };

	public class OthelloMove {
	    public OthelloMove(int x, int y) {
	        X = x;
	        Y = y;
	    }
	    public int X;
	    public int Y;
	    
	    public override string ToString() {
	        return ((char)('A'+X)).ToString() + ((7-Y)+1).ToString();
	    }
	}

	public class OthelloMoveWithData: OthelloMove {
	    public OthelloMoveWithData(int x, int y) : base(x,y) {}

	    internal int[] CaptureInDirection = new int[8];
	    internal int PlayerSquares, OpponentSquares;
	    internal SquareState CurrentPlayer;
	}

	/// <summary>
	/// Summary description for OthelloBoard.
	/// </summary>
	public class OthelloBoard: ICloneable {
	
        private static int[,] _directions = { {0,-1},{1,-1},{1,0},{1,1},{0,1},{-1,1},{-1,0},{-1,-1} };

        private static int[,,] _hashValues;
        
        private int _hashCode;
	    private SquareState[,] _squares;
	    private SquareState _currentPlayer, _otherPlayer;
	    private int _playerSquares, _opponentSquares;
	    private bool _gameOver;
	
		static OthelloBoard() {
    	    Random rand = new Random(17);
		    _hashValues = new int[2,8,8];
            for	(int squareState=0; squareState<2; squareState++)	{
                for	(int y=0; y<8; y++)	{
                    for (int x=0; x<8; x++) {
                        _hashValues[squareState,x,y] = rand.Next();
                    }
                }
            }
		}

        public static string ColorNameFromSquareState(SquareState state) {
            switch (state) {
            case (SquareState.Black): return "black";
            case (SquareState.White): return "white";
            case (SquareState.Empty): return "empty";
            }
            
            throw new Exception();
        }

        public static OthelloBoard CreateStartingBoard() {
            OthelloBoard board = new OthelloBoard(SquareState.Black);
            board.SetToStartingPosition();
            return board;
        }

		private OthelloBoard() {}

		public OthelloBoard(SquareState currentPlayer) {
		    _squares = new SquareState[8,8];
		    CurrentPlayer = currentPlayer;
		}
		
        public void SetToStartingPosition() {
            this[3,3] = SquareState.White;
            this[3,4] = SquareState.Black;
            this[4,3] = SquareState.Black;
            this[4,4] = SquareState.White;
        }

        private static SquareState SquareStateFromChar(char ch) {
            switch (ch) {
            case '.':
                return SquareState.Empty;
            case '*':
                return SquareState.Black;
            case 'O':
                return SquareState.White;
            }
            
            throw new Exception("Unexpected char '" + ch + "'");
        }

        private static char CharFromSquareState(SquareState state) {
            switch (state) {
            case SquareState.Empty:
                return '.';
            case SquareState.Black:
                return '*';
            case SquareState.White:
                return 'O';
            }
            
            throw new Exception("Unexpected state '" + state + "'");
        }

        public override int GetHashCode() {
            if (_currentPlayer == SquareState.Black)
                return _hashCode;
            else
                return ~_hashCode;
        }
        
        public override bool Equals(object o) {
            OthelloBoard other = (OthelloBoard) o;
            if (other.CurrentPlayer != this.CurrentPlayer)
                return false;
                
            for	(int y=0; y<8; y++)	{
                for (int x=0; x<8; x++) {
                    if (other[x,y] != this[x,y])
                        return false;
                }
            }
            
            return true;
        }

        public static OthelloBoard LoadFromFile(string path) {
            StreamReader reader = new StreamReader(path);

            string s = reader.ReadLine();
            SquareState currentPlayer = SquareStateFromChar(s[0]);
            OthelloBoard ret = new OthelloBoard(currentPlayer);

            for	(int y=0; y<8; y++)	{
                s = reader.ReadLine();
                for (int x=0; x<8; x++) {
                    ret[x,y] = SquareStateFromChar(s[x]);
                }
            }

            reader.Close();
            
            return ret;
        }

        public void SaveToFile(string path) {
            StreamWriter writer = new StreamWriter(path);

            writer.Write(CharFromSquareState(CurrentPlayer));
            writer.WriteLine();

            for	(int y=0; y<8; y++)	{
                for (int x=0; x<8; x++) {
                    writer.Write(CharFromSquareState(this[x,y]));
                }
                writer.WriteLine();
            }

            writer.Close();
        }

		public virtual object Clone() {
		    OthelloBoard ret = new OthelloBoard();
		    ret._squares = (SquareState[,]) this._squares.Clone();
		    ret.CurrentPlayer = this._currentPlayer;
		    ret._playerSquares = this._playerSquares;
		    ret._opponentSquares = this._opponentSquares;
		    return ret;
		}

        public SquareState this[int x, int y] {
            get { return _squares[x,y]; }
            set {
                if (value == _currentPlayer)
                    _playerSquares++;
                else if (value == _otherPlayer)
                    _opponentSquares++;
                SetSquare(x, y, value);
            }
        }
        
        private void SetSquare(int x, int y, SquareState state) {
            if (_squares[x,y] == SquareState.Black)
                _hashCode ^= _hashValues[0,x,y];
            else if (_squares[x,y] == SquareState.White)
                _hashCode ^= _hashValues[1,x,y];

            if (state == SquareState.Black)
                _hashCode ^= _hashValues[0,x,y];
            else if (state == SquareState.White)
                _hashCode ^= _hashValues[1,x,y];

            _squares[x,y] = state;
        }
        
        public SquareState CurrentPlayer {
            get { return _currentPlayer; }
            set {
                _currentPlayer = value;
                _otherPlayer = (_currentPlayer == SquareState.White) ? SquareState.Black : SquareState.White;
            }
        }

        public SquareState OtherPlayer {
            get { return _otherPlayer; }
        }

        public bool GameOver { get { return _gameOver; } }

        public void SwitchPlayer() {
            CurrentPlayer = OtherPlayer;
            int temp = _playerSquares;
            _playerSquares = _opponentSquares;
            _opponentSquares = temp;
        }

        public void FixUpCurrentPlayer() {
            if (!HasValidMoves()) {
                SwitchPlayer();
                if (!HasValidMoves()) {
                    _gameOver = true;
                }
            }
        }

        private void PlayMove(int x, int y) {
            Debug.Assert(IsValidMove(x,y));
            
            for (int dir=0; dir<8; dir++)
                TakeInDirection(x, y, _directions[dir,0], _directions[dir,1]);

            this[x,y] = CurrentPlayer;

            SwitchPlayer();

            if (EmptySquares == 0) {
                _gameOver = true;
                return;
            }
        }

        public void PlayMove(OthelloMove move) {
            PlayMove(move.X, move.Y);
        }

        public void PlayMove(OthelloMoveWithData move) {
        
            for (int dir=0; dir<8; dir++) {
                int captureInDirection = move.CaptureInDirection[dir];
                if (captureInDirection > 0) {
                    TakeInDirection(move.X, move.Y, _directions[dir,0],
                        _directions[dir,1], move.CaptureInDirection[dir]);
                }
            }

            this[move.X,move.Y] = CurrentPlayer;

            SwitchPlayer();

            if (EmptySquares == 0) {
                _gameOver = true;
                return;
            }
        }

        public void UnplayMove(OthelloMoveWithData move) {
            this.CurrentPlayer = move.CurrentPlayer;
            this._playerSquares = move.PlayerSquares;
            this._opponentSquares = move.OpponentSquares;
            _gameOver = false;

            for (int dir=0; dir<8; dir++) {
                int captureInDirection = move.CaptureInDirection[dir];
                if (captureInDirection > 0) {
                    UntakeInDirection(move.X, move.Y, _directions[dir,0],
                        _directions[dir,1], move.CaptureInDirection[dir]);
                }
            }

            this[move.X,move.Y] = SquareState.Empty;
        }

        private int NumToTakeInDirection(int x, int y, int dx, int dy) {
            int count=0;
            for (x+=dx, y+=dy; ValidPosition(x,y) && _squares[x,y]==OtherPlayer;
                x+=dx, y+=dy, count++);
            if (count==0 || !ValidPosition(x,y) || _squares[x,y]!=CurrentPlayer)
                return 0;
            else
                return count;
        }

        public int NumberCaptured(OthelloMove move) {
            return NumberCaptured(move.X, move.Y);
        }
        
        private int NumberCaptured(int x, int y) {
            if (_squares[x,y] != SquareState.Empty)
                return 0;
                
            int ret=0;
            for (int dir=0; dir<8; dir++)
                ret += NumToTakeInDirection(x, y, _directions[dir,0], _directions[dir,1]);

            return ret;
        }
        
        private void TakeInDirection(int x, int y, int dx, int dy) {
            int numToTake = NumToTakeInDirection(x, y, dx, dy);
            TakeInDirection(x, y, dx, dy, numToTake);
        }

        private void TakeInDirection(int x, int y, int dx, int dy, int numToTake) {
            int taken=0;
            for (x+=dx, y+=dy; taken<numToTake; x+=dx, y+=dy, taken++)
                SetSquare(x, y, CurrentPlayer);
            _playerSquares += numToTake;
            _opponentSquares -= numToTake;
        }

        private void UntakeInDirection(int x, int y, int dx, int dy, int numToTake) {
            int taken=0;
            for (x+=dx, y+=dy; taken<numToTake; x+=dx, y+=dy, taken++)
                SetSquare(x, y, OtherPlayer);
        }

        private bool IsValidMove(int x, int y) {
            if (_squares[x,y] != SquareState.Empty)
                return false;
                
            for (int dir=0; dir<8; dir++) {
                if (NumToTakeInDirection(x, y, _directions[dir,0], _directions[dir,1]) > 0)
                    return true;
            }

            return false;
        }
        
        public bool IsValidMove(OthelloMove move) {
            return IsValidMove(move.X, move.Y);
        }
   
/*
OthelloMoveWithData qqq(int x, int y) {
int i=1;
if (i==0)
    throw new Exception("Bogus exception just to prevent method inlining");

OthelloMoveWithData moveInfo = new OthelloMoveWithData(x, y);
moveInfo.CurrentPlayer = this.CurrentPlayer;
moveInfo.PlayerSquares = this._playerSquares;
moveInfo.OpponentSquares = this._opponentSquares;
return moveInfo;
}
*/

        public OthelloMoveWithData GetMoveInfoIfValid(int x, int y) {
            if (_squares[x,y] != SquareState.Empty)
                return null;
            
            OthelloMoveWithData moveInfo = null;
            for (int dir=0; dir<8; dir++) {
                int num = NumToTakeInDirection(
                    x, y, _directions[dir,0], _directions[dir,1]);
                if (num > 0) {
                    if (moveInfo == null) {
                        moveInfo = new OthelloMoveWithData(x, y);
                        moveInfo.CurrentPlayer = this.CurrentPlayer;
                        moveInfo.PlayerSquares = this._playerSquares;
                        moveInfo.OpponentSquares = this._opponentSquares;
                    }
                    moveInfo.CaptureInDirection[dir] = num;
                }
            }

            return moveInfo;
        }
        
        public ArrayList GetValidMoves() {

            ArrayList validMoves = null;
            
            for	(int y=0; y<8; y++)	{
                for (int x=0; x<8; x++) {
                    if (IsValidMove(x,y)) {
                        if (validMoves == null)
                            validMoves = new ArrayList();
                        validMoves.Add(new OthelloMove(x, y));
                    }
                }
            }
            
            return validMoves;
        }

        // Not thread safe if the algorithm becomes multi-threaded!
        private OthelloMoveWithData[] _tempMoveWithDataArray = new OthelloMoveWithData[40];
        public OthelloMoveWithData[] GetValidMovesWithData() {

            int count = 0;
            for	(int y=0; y<8; y++)	{
                for (int x=0; x<8; x++) {
                    OthelloMoveWithData move = GetMoveInfoIfValid(x, y);
                    if (move != null)
                        _tempMoveWithDataArray[count++] = move;
                }
            }
            
            if (count == 0) return null;

            OthelloMoveWithData[] validMoves = new OthelloMoveWithData[count];
            for (int i=0; i<count; i++) {
                validMoves[i] = _tempMoveWithDataArray[i];
            }
            
            return validMoves;
        }

        public int CountValidMoves() {

            int count = 0;
            
            for	(int y=0; y<8; y++)	{
                for (int x=0; x<8; x++) {
                    if (IsValidMove(x,y)) {
                        count++;
                    }
                }
            }
            
            return count;
        }
        
        public bool HasValidMoves() {
            if (WhiteSquares == 0)
                return false;
                
            for	(int y=0; y<8; y++)	{
                for (int x=0; x<8; x++) {
                    if (IsValidMove(x,y))
                        return true;
                }
            }
            
            return false;
        }

        public int PlayerSquares { get { return _playerSquares; } }
        public int OpponentSquares { get { return _opponentSquares; } }
        public int EmptySquares { get { return 64-_playerSquares-_opponentSquares; } }
        public int BlackSquares {
            get { return _currentPlayer == SquareState.Black ? _playerSquares: _opponentSquares; }
        }
        public int WhiteSquares {
            get { return _currentPlayer == SquareState.White ? _playerSquares: _opponentSquares; }
        }

        private static bool ValidPosition(int x, int y) {
            return ((x|y) & 0xF8) == 0;       /* x and y are both in range 0..7 */
        }
	}
}
