using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Drawing.Drawing2D;

namespace Othello
{
	///	<summary>
	///	Summary	description	for	Form1.
	///	</summary>
	public class Form1 : System.Windows.Forms.Form
	{
		///	<summary>
		///	Required designer variable.
		///	</summary>
		private	System.ComponentModel.Container	components = null;
		
        private Square[,] _squares;
        private OthelloBoard _board;
        private OthelloMove _lastPlayedMove;
        
        private OthelloPlayer _blackPlayer, _whitePlayer, _currentPlayer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label BlackCount;
        private System.Windows.Forms.Label WhiteCount;
        private System.Windows.Forms.Button LoadGame;
        private System.Windows.Forms.Button Play;
        private System.Windows.Forms.Button SaveGame;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label EmptyCount;
        private System.Windows.Forms.Label Status;

		public Form1() {
		    this.SuspendLayout();
            CreateSquares();
			InitializeComponent();
			this.ResumeLayout(true);

            CreateDefaultBoard();
            //_blackPlayer = new SmartOthelloPlayer();
            _whitePlayer = new SmartOthelloPlayer();
            SyncBoardToUI();

			AsyncTaskManager.RegisterUIThread(this);
		}

        private void OnPlayerStatus(object sender, AsyncTaskResultPostedEventArgs e) {
            string status = e.Data as string;
            if (status != null) {
                Status.Text = status;
                return;
            }
            PlayerResult result = (PlayerResult) e.Data;
            Status.Text = result.Status;
            PlayMove(result.Move);
        }

        private void CreateSquares() 
        {
            _squares = new Square[8,8];
            for	(byte y=0; y<8; y++)	{
                for (byte x=0; x<8; x++) {
                    _squares[x,y] = new Square(x, y);
                    _squares[x,y].BackColor = System.Drawing.Color.FromArgb(((System.Byte)(200)), ((System.Byte)(200)), ((System.Byte)(100)));
                    _squares[x,y].Click += new System.EventHandler(this.OnSquareClicked);
                    Controls.Add(_squares[x,y]);
                }
            }
        }
            
        private void CreateDefaultBoard() {
            _board = OthelloBoard.CreateStartingBoard();
        }

        private void SyncBoardToUI() {
            _currentPlayer = _board.CurrentPlayer == SquareState.Black ? _blackPlayer : _whitePlayer;

            for	(int y=0; y<8; y++)	{
                for (int x=0; x<8; x++) {
                    _squares[x,y].State = _board[x,y];
                    
                    _squares[x,y].ShowAsValidMove((_currentPlayer == null) &&
                        _board.IsValidMove(new OthelloMove(x,y)));

                    _squares[x,y].ShowAsLastMovePlayed(_lastPlayedMove != null &&
                        _lastPlayedMove.X == x && _lastPlayedMove.Y == y);
                }
            }
            
            BlackCount.Text = _board.BlackSquares.ToString();
            WhiteCount.Text = _board.WhiteSquares.ToString();
            EmptyCount.Text = _board.EmptySquares.ToString();
        }

        private void OnSquareClicked(object sender, System.EventArgs e) {
            Square square = (Square) sender;
            
            PlayMove(new OthelloMove(square.X,square.Y));
        }
        
        private void PlayMove(OthelloMove move) {
            if (!_board.IsValidMove(move))
                return;

            SquareState previousPlayerColor = _board.CurrentPlayer;
            _board.PlayMove(move);
            _lastPlayedMove = move;
            _board.FixUpCurrentPlayer();
            
            SyncBoardToUI();

            this.BeginInvoke(new PostMoveMethodDelegate(DoPostMoveProcessing),
                new object[] {previousPlayerColor});
        }
        
        private delegate void PostMoveMethodDelegate(SquareState previousPlayerColor);
        
        private void DoPostMoveProcessing(SquareState previousPlayerColor) {
            if (_board.GameOver) {
                MessageBox.Show("Game over!  Black: " + _board.BlackSquares +
                    " White: " + _board.WhiteSquares);
            }
            else if (_board.CurrentPlayer == previousPlayerColor)
            {
                MessageBox.Show("No moves for " + OthelloBoard.ColorNameFromSquareState(
                    _board.OtherPlayer));
            }
            PlayNextMove();
        }

        private void PlayNextMove() {
            if (_board.GameOver)
                return;

            if (_currentPlayer != null) {
                _currentPlayer.SetBoard(_board);
                _currentPlayer.Start(new AsyncTaskResultPostedEventHandler(this.OnPlayerStatus));
            }
        }

        protected override void OnLayout(LayoutEventArgs e) {

            base.OnLayout(e);

            int size = ClientRectangle.Height;
        
            int padding = 30;
            size -= padding*2;
        
            int squareSize = size/8;
        
            for	(int y=0; y<8; y++)	
            {
                for (int x=0; x<8; x++) 
                {
                    Square square = _squares[x,y];
                    square.Width = squareSize;
                    square.Height = squareSize;
                    square.Left = x*squareSize + padding;
                    square.Top = y*squareSize + padding;
                    square.Invalidate();
                }
            }
        }

		///	<summary>
		///	Clean up any resources being used.
		///	</summary>
		protected override void	Dispose( bool disposing	)
		{
			if(	disposing )
			{
				if (components != null)	
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing	);
		}

		#region	Windows	Form Designer generated	code
		///	<summary>
		///	Required method	for	Designer support - do not modify
		///	the	contents of	this method	with the code editor.
		///	</summary>
		private	void InitializeComponent()
		{
            this.Play = new System.Windows.Forms.Button();
            this.Status = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BlackCount = new System.Windows.Forms.Label();
            this.WhiteCount = new System.Windows.Forms.Label();
            this.LoadGame = new System.Windows.Forms.Button();
            this.SaveGame = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.EmptyCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Play
            // 
            this.Play.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.Play.ForeColor = System.Drawing.Color.Gold;
            this.Play.Location = new System.Drawing.Point(624, 472);
            this.Play.Name = "Play";
            this.Play.Size = new System.Drawing.Size(88, 32);
            this.Play.TabIndex = 0;
            this.Play.Text = "Make computer play";
            this.Play.Click += new System.EventHandler(this.button1_Click);
            // 
            // Status
            // 
            this.Status.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.Status.ForeColor = System.Drawing.Color.Gold;
            this.Status.Location = new System.Drawing.Point(536, 328);
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(176, 112);
            this.Status.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.label1.ForeColor = System.Drawing.Color.Gold;
            this.label1.Location = new System.Drawing.Point(616, 232);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 23);
            this.label1.TabIndex = 2;
            this.label1.Text = "Black:";
            // 
            // label2
            // 
            this.label2.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.label2.ForeColor = System.Drawing.Color.Gold;
            this.label2.Location = new System.Drawing.Point(616, 256);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "White:";
            // 
            // BlackCount
            // 
            this.BlackCount.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.BlackCount.ForeColor = System.Drawing.Color.Gold;
            this.BlackCount.Location = new System.Drawing.Point(656, 232);
            this.BlackCount.Name = "BlackCount";
            this.BlackCount.Size = new System.Drawing.Size(32, 23);
            this.BlackCount.TabIndex = 3;
            // 
            // WhiteCount
            // 
            this.WhiteCount.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.WhiteCount.ForeColor = System.Drawing.Color.Gold;
            this.WhiteCount.Location = new System.Drawing.Point(656, 256);
            this.WhiteCount.Name = "WhiteCount";
            this.WhiteCount.Size = new System.Drawing.Size(32, 23);
            this.WhiteCount.TabIndex = 3;
            // 
            // LoadGame
            // 
            this.LoadGame.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.LoadGame.ForeColor = System.Drawing.Color.Gold;
            this.LoadGame.Location = new System.Drawing.Point(616, 160);
            this.LoadGame.Name = "LoadGame";
            this.LoadGame.TabIndex = 4;
            this.LoadGame.Text = "Load Game";
            this.LoadGame.Click += new System.EventHandler(this.LoadGame_Click);
            // 
            // SaveGame
            // 
            this.SaveGame.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.SaveGame.ForeColor = System.Drawing.Color.Gold;
            this.SaveGame.Location = new System.Drawing.Point(616, 192);
            this.SaveGame.Name = "SaveGame";
            this.SaveGame.TabIndex = 4;
            this.SaveGame.Text = "Save Game";
            this.SaveGame.Click += new System.EventHandler(this.SaveGame_Click);
            // 
            // label3
            // 
            this.label3.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.label3.ForeColor = System.Drawing.Color.Gold;
            this.label3.Location = new System.Drawing.Point(616, 280);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 23);
            this.label3.TabIndex = 2;
            this.label3.Text = "Empty:";
            // 
            // EmptyCount
            // 
            this.EmptyCount.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.EmptyCount.ForeColor = System.Drawing.Color.Gold;
            this.EmptyCount.Location = new System.Drawing.Point(656, 280);
            this.EmptyCount.Name = "EmptyCount";
            this.EmptyCount.Size = new System.Drawing.Size(32, 23);
            this.EmptyCount.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(736, 525);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.LoadGame,
                                                                          this.BlackCount,
                                                                          this.label1,
                                                                          this.Status,
                                                                          this.Play,
                                                                          this.label2,
                                                                          this.WhiteCount,
                                                                          this.SaveGame,
                                                                          this.label3,
                                                                          this.EmptyCount});
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "Form1";
            this.Text = "Reversi";
            this.ResumeLayout(false);

        }
		#endregion

		///	<summary>
		///	The	main entry point for the application.
		///	</summary>
		[STAThread]
		static void	Main() 
		{
			Application.Run(new	Form1());
		}

        private void button1_Click(object sender, System.EventArgs e) {
            PlayNextMove();
        }

        private void SaveGame_Click(object sender, System.EventArgs e) {
            _board.SaveToFile("game.txt");
        }

        private void LoadGame_Click(object sender, System.EventArgs e) {
            _board = OthelloBoard.LoadFromFile("game.txt");
            _lastPlayedMove = null;
            SyncBoardToUI();
        }

	}
}
