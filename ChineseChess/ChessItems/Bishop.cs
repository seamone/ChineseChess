using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChineseChess.ChessItems
{
    /// <summary>
    ///象 
    /// </summary>
    public class Bishop : BaseChess
    {
        public Bishop()
        {
           
        }
        public Bishop(byte theType)
        {
            _pieceType = theType;
            if (_pieceType == 98)
            {
                Type = ChessType.Black;
                Text = "象";
            }
            else
            {
                Type = ChessType.Red;
                Text = "相";
            }
        }
        public override bool obeyTheLimit(int gridX, int gridY)
        {
            if (!base.obeyTheLimit(gridX, gridY))
            {
                return false;
            }
            if (isInOwnSide(gridX, gridY))
            {
                if (Math.Abs(gridX - GridX) == 2 && Math.Abs(gridY - GridY) == 2
               && !hasChessOnPoint((gridX + GridX) / 2, (gridY + GridY) / 2))
                {
                    return true;
                }
            }
         
            return false;

        }
    }
}
