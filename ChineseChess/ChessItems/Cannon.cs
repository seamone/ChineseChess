using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChineseChess.ChessItems
{
    /// <summary>
    /// 炮
    /// </summary>
    public class Cannon : BaseChess
    {
        public Cannon()
        {
            
        }
        public Cannon(byte theType)
        {
            _pieceType = theType;
            if (_pieceType == 99)
            {
                Type = ChessType.Black;

            }
            else
            {
                Type = ChessType.Red;

            }
            Text = "炮";
        }
        public override bool obeyTheLimit(int gridX, int gridY)
        {
            //EA1195551847BE
            int min;
            int max;
            int countOfChess = -1;
            if (GridX == gridX)
            {
                countOfChess = 0;
                ChessUtils.getMinMax(GridY, gridY, out min, out max);
                for (int i = min + 1; i < max; i++)
                {
                    if (hasChessOnPoint(GridX, i))
                    {
                        countOfChess++;
                    }
                }


            }
            else if (GridY == gridY)
            {
                countOfChess = 0;
                ChessUtils.getMinMax(GridX, gridX, out min, out max);
                for (int i = min + 1; i < max; i++)
                {
                    if (hasChessOnPoint(i, GridY))
                    {
                        countOfChess++;
                    }
                }
            }

            if (countOfChess == 0)
            {
                if (hasChessOnPoint(gridX, gridY))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (countOfChess == 1)
            {
                if (hasChessOnPoint(gridX, gridY))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }


        }
    }
}
