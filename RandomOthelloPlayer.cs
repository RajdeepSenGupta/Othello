using System;
using System.Collections;

namespace Othello
{
	/// <summary>
	/// Summary description for RandomOthelloPlayer.
	/// </summary>
	public class RandomOthelloPlayer: OthelloPlayer {
	    private Random rand = new Random();
	    
	    public override PlayerResult GetMove(OthelloBoard board) {
	        ArrayList moves = board.GetValidMoves();
	        int moveIndex = rand.Next(moves.Count);
	        PlayerResult result = new PlayerResult();
	        result.Move = (OthelloMove) moves[moveIndex];
	        return result;
	    }
	}
}
