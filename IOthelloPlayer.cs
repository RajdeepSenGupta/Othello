using System;

namespace Othello
{
    /// <summary>
    /// Summary description for IOthelloPlayer.
    /// </summary>
    public abstract class OthelloPlayer: AsyncTask {
        private OthelloBoard _board;
        private bool _playNow;
    
        protected override void PerformTask() {
            PlayerResult result = GetMove(_board);
            PostResults(result, 100, true);
        }
        
        protected void PostStatus(string status) {
            PostResults(status, 100, true);
        }
        
        public abstract PlayerResult GetMove(OthelloBoard board);

        public void ForcePlayNow() {
            _playNow = true;
        }
        
        protected bool MustPlayNow { get { return _playNow; } }
        
        public void SetBoard(OthelloBoard board) {
            _board = board;
        }
    }

    public class PlayerResult {
        public OthelloMove Move;
        public string Status;
    }
}
