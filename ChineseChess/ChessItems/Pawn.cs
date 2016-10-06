using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChineseChess.ChessItems
{
    /// <summary>
    /// 兵
    /// </summary>
    public class Pawn : BaseChess
    {
        public Pawn(byte theType)
        {
            _pieceType = theType;
            if (_pieceType == 112)
            {
                Type = ChessType.Black;
                Text = "卒";
            }
            else
            {
                Type = ChessType.Red;
                Text = "兵";
            }
        }
        public Pawn()
        {
            
        }
        public override bool obeyTheLimit(int gridX, int gridY)
        {
            bool isObey = false;
            if (!base.obeyTheLimit(gridX, gridY))
            {
                return false;
            }
            if (isMoveForwardOneStep(gridX, gridY))
            {
                return true;
            }
            else if (!isInOwnSide(gridX, gridY) && isHorizontalMoveOneStep(gridX, gridY))
            {
                return true;
            }
            return isObey;

        }
        private bool isHorizontalMoveOneStep(int gridX, int gridY)
        {
            if ((Math.Abs(GridX - gridX) == 1 && GridY == gridY))
            {
                return true;
            }
            return false;
        }
        private bool isMoveForwardOneStep(int gridX, int gridY)
        {
            if (GridX == gridX)
            {
                if (ChessUtils.ChessboardDirection == AtackDirection.BlackUpRedDown)
                {
                    if (GridY - gridY == 1 && Type == ChessType.Red)
                    {
                        return true;
                    }
                    else if (gridY - GridY == 1 && Type == ChessType.Black)
                    {
                        return true;
                    }

                }
                else if (ChessUtils.ChessboardDirection == AtackDirection.RedUpBlackDown)
                {
                    if (gridY - GridY == 1 && Type == ChessType.Red)
                    {
                        return true;
                    }
                    else if (GridY - gridY == 1 && Type == ChessType.Black)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
    }
}
