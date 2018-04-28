using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Othello
{

	/// <summary>
	/// Summary description for Square.
	/// </summary>
	public class Square : System.Windows.Forms.Control
	{
        private SquareState _state;
        private int _x, _y;
        private bool _showAsValid;
        private bool _isLastMovePlayed;
	    
		public Square(int x, int y) {
		    _x = x;
		    _y = y;
		}
		
        public int X { get { return _x; } }
        public int Y { get { return _y; } }

		protected override void OnPaint(PaintEventArgs pe)
		{
            Graphics g = pe.Graphics;
            //g.Clear(Color.Beige);
            int w = ClientRectangle.Width;
            int h = ClientRectangle.Height;
            Pen	pen	= new Pen(new SolidBrush(Color.Black));
            g.DrawRectangle(pen, 0, 0, w-1, h-1);

            DrawSquare(g, false);

			// Calling the base class OnPaint
			base.OnPaint(pe);
		}
		
		private void DrawSquare(Graphics g, bool mouseEnter) {
            int w = ClientRectangle.Width;
            int h = ClientRectangle.Height;

            Brush b;

            int border;
            
            if (_state == SquareState.Empty) 
            {
                if (!_showAsValid || !mouseEnter)
                    return;
                    
                border = (2*w)/5;
                b = new SolidBrush(Color.Red);
                g.FillEllipse(b, border, border, w-(border*2), h-(border*2));
                return;
            }
                
            border = w/10;
            b = new SolidBrush((_state == SquareState.Black) ? Color.Black: Color.White);
            g.FillEllipse(b, border, border, w-(border*2), h-(border*2));
            
            if (this._isLastMovePlayed) {
                Pen	pen	= new Pen(new SolidBrush(Color.Red), 3);
                g.DrawEllipse(pen, border, border, w-(border*2), h-(border*2));
            }
        }

        protected override void OnMouseEnter(EventArgs e) {
            if (!_showAsValid)
                return;

            using (Graphics g = CreateGraphics()) {
                DrawSquare(g, true);
            }
        }
        
        protected override void OnMouseLeave(EventArgs e) {
            Invalidate();
        }
        
        public void ShowAsValidMove(bool valid) 
        {
            if (_showAsValid == valid)
                return;
                
            _showAsValid = valid;
            Invalidate();
        }

        public void ShowAsLastMovePlayed(bool isLastMovePlayed) 
        {
            if (_isLastMovePlayed == isLastMovePlayed)
                return;
                
            _isLastMovePlayed = isLastMovePlayed;
            Invalidate();
        }

        public SquareState State 
        {
            get { return _state; }
            set {
                if (_state == value)
                    return;
                _state = value;
                Invalidate();
            }
        }
    }
}
