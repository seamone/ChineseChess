using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ponder.ChineseChess
{
    /// <summary>
    /// 为了着法生成等方法的效率，这里的Move要求piece,from,to,capture四个信息都不能少
    /// 实现IComparable接口是为了对Move.Score进行排序，从而可以提高剪枝的效率
    /// </summary>
    public class Move : IComparable  
    {
        /// <summary>
        /// 起点
        /// </summary>
        public int From;

        /// <summary>
        /// 终点
        /// </summary>
        public int To;

        /// <summary>
        /// //走动的子
        /// </summary>
        public int Piece;  

        /// <summary>
        /// 吃掉的子
        /// </summary>
        public int Capture;

        /// <summary>
        /// 2013.9.25 为了进行Move Ordering，可能会提高剪枝的效率
        /// </summary>
        public int Score;

        public bool Irreversible; //吃子着法和兵的前进是不可逆着法，其它子的移动都是可逆的


        /// <summary>
        /// 构造函数，直接调用这个方法会有些麻烦
        /// 用Board中的CreateMove方法更方便
        /// 例如：board.CreateMove("a0a1");
        /// </summary>
        /// <param name="from">出发点</param>
        /// <param name="to">终点</param>
        /// <param name="piece">走动的棋子</param>
        /// <param name="capture">吃掉的对方棋子的编号，这个信息在UnmakeMove时会用到</param>
        public Move(int from, int to, int piece, int capture)
        {
            From = from;
            To = to;
            Piece = piece;
            Capture = capture;
            Irreversible = false;
            //Score = 0;
        }

        public Move(Move m)
        {
            From = m.From;
            To = m.To;
            Piece = m.Piece;
            Capture = m.Capture;
            Score = m.Score;
            Irreversible = m.Irreversible;
        }
        //public Move(Board board, string strMove)
        //{
        //    Move m = board.CreateMoveFromString(strMove);
        //    From = m.From;
        //    To = m.To;
        //    Piece = m.Piece;
        //    Capture = m.Capture;
        //}

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Move m = obj as Move;
            if (m == null) return false;
            return this.Equals(m);
        }
        public override int GetHashCode()
        {
            return From ^ To;
        }

        public bool Equals(Move move)
        {
            return (From==move.From) && (To == move.To) && (Piece == move.Piece) && (Capture == move.Capture);
        }

        /// <summary>
        /// 实现IComparable接口，用Score做比较
        /// 注意！！！这里默认按从大到小排序
        /// </summary>
        /// <param name="obj">比较对象</param>
        /// <returns>比较结果</returns>
        public int CompareTo(object obj)
        {
            if (obj is Move)
            {
                return ((Move)obj).Score.CompareTo(Score); //2个Score反过来就是从小到大排列了
            }

            return 1;
        }


        public override string ToString()
        {
            return Board.POS_STR[From] + Board.POS_STR[To];
           //return From.ToString() + "-" + To.ToString();
        }

        public string ToChineseNotation(Board board)
        {
            return MoveNotation.NotationConvertedFromMove(board, this);
        }

        
        //public Move(){}
    }
}










