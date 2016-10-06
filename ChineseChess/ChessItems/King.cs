using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ChineseChess.ChessItems
{
    /// <summary>
    /// 将军
    /// </summary>
    public class King : BaseChess
    {
        public King(Chessboard theChessBoard)
            : base(theChessBoard)
        {

        }
        public King()
        {
            
        }
        public King(byte theType)
        {
            _pieceType = theType;
            if (_pieceType == 75)
            {
                Type = ChessType.Red;
                Text = "将";
            }
            else
            {
                Type = ChessType.Black;
                Text = "帅";
            }
        }
        public event EventHandler BeRemoved;
        public event EventHandler IsMoved;
        public override bool obeyTheLimit(int gridX, int gridY)
        {
            if (!base.obeyTheLimit(gridX, gridY))
            {
                return false;
            }

            if (isInSquareBox(gridX, gridY))
            {
                if (GridX == gridX && (Math.Abs(GridY - gridY) == 1)
                    || GridY == gridY && (Math.Abs(GridX - gridX) == 1))
                {

                    return true;
                }
            }
           
            return false;

        }

        

        //public override bool move(Point nextLocation)
        //{
        //    if (base.move(nextLocation))
        //    {
        //        //IsMoved(this, null);
        //        return true;
        //    }
        //    return false;
        //}


        public override void remove()
        {
            BeRemoved(this, null);
            base.remove();
        }
    }
}
