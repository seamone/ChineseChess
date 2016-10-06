using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChineseChess.ChessItems
{
    /// <summary>
    /// 士
    /// </summary>
    public class Advisor : BaseChess
    {
        public Advisor()
        {
           
        }
        public Advisor(byte theType)
        {
            _pieceType = theType;
            if (_pieceType == 97)
            {
                Type = ChessType.Black;
                Text = "仕";
            }
            else
            {
                Type = ChessType.Red;
                Text = "士";
            }
        }
        public override bool obeyTheLimit(int gridX, int gridY)
        {
            if (!base.obeyTheLimit(gridX, gridY))
            {
                return false;
            }
            if (isInSquareBox(gridX, gridY))
            {
                if (Math.Abs(gridX - GridX) == 1 && Math.Abs(gridY - GridY) == 1)
                {
                    return true;
                }
            }
            
            return false;

        }
    }
}
