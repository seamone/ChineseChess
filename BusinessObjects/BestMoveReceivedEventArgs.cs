using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace BusinessObjects.Events
{
    public class BestMoveReceivedEventArgs : EventArgs
    {
        private string _bestMove;
        public string BestMove
        {
            get { return _bestMove; }
        }
        public BestMoveReceivedEventArgs(string theBestMove)
        {
            _bestMove = theBestMove;
        }
    }

    public delegate void BestMoveReceivedEventHandler(object sender, BestMoveReceivedEventArgs e);
}
