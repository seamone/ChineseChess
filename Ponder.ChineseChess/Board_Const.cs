using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ponder.ChineseChess
{
    /// <summary>
    /// 与Board中位置Position有关的静态成员和方法
    /// </summary>
    public partial class Board
    {

        #region 在静态初始化里初始化的常量
        /// <summary>
        /// 初始时把边界和棋子空位都设置好
        /// </summary>
        private static readonly int[] NONE_BOARD;

        /// <summary>
        /// 是否在棋盘的9x10格子内
        /// </summary>
        private static readonly bool[] IS_IN_BOARD;

        /// <summary>
        /// 是否在红方区域内
        /// </summary>
        private static readonly bool[] IS_IN_RED_SIDE;
        /// <summary>
        /// 是否在黑方区域内
        /// </summary>
        private static readonly bool[] IS_IN_BLACK_SIDE;
        /// <summary>
        /// 是否在红方九宫
        /// </summary>
        private static readonly bool[] IS_IN_RED_PALACE;
        /// <summary>
        /// 是否在黑方九宫
        /// </summary>
        private static readonly bool[] IS_IN_BLACK_PALACE;

        #endregion
      
        /// <summary>
        /// 查表得到位置的列号，注意从0到8是棋盘，左边边界-4到-1，右边边界9到12
        /// </summary>
        private static readonly int[] FILE = 
        {
            -4,	-3,	-2,	-1,	0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10,	11,	12,
            -4,	-3,	-2,	-1,	0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10,	11,	12,
            -4,	-3,	-2,	-1,	0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10,	11,	12,
            -4,	-3,	-2,	-1,	0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10,	11,	12,
            -4,	-3,	-2,	-1,	0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10,	11,	12,
            -4,	-3,	-2,	-1,	0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10,	11,	12,
            -4,	-3,	-2,	-1,	0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10,	11,	12,
            -4,	-3,	-2,	-1,	0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10,	11,	12,
            -4,	-3,	-2,	-1,	0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10,	11,	12,
            -4,	-3,	-2,	-1,	0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10,	11,	12,
            -4,	-3,	-2,	-1,	0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10,	11,	12,
            -4,	-3,	-2,	-1,	0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10,	11,	12,
            -4,	-3,	-2,	-1,	0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10,	11,	12,
            -4,	-3,	-2,	-1,	0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10,	11,	12
        };

        /// <summary>
        /// 查表得到棋盘位置的行号，从0到9是实际棋盘，行号-2，-1，10，11是边界
        /// </summary>
        private static readonly int[] RANK =
        {
            -2,	-2,	-2,	-2,	-2,	-2,	-2,	-2,	-2,	-2,	-2,	-2,	-2,	-2,	-2,	-2,	-2,
            -1,	-1,	-1,	-1,	-1,	-1,	-1,	-1,	-1,	-1,	-1,	-1,	-1,	-1,	-1,	-1,	-1,
            0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,
            1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,
            2,	2,	2,	2,	2,	2,	2,	2,	2,	2,	2,	2,	2,	2,	2,	2,	2,
            3,	3,	3,	3,	3,	3,	3,	3,	3,	3,	3,	3,	3,	3,	3,	3,	3,
            4,	4,	4,	4,	4,	4,	4,	4,	4,	4,	4,	4,	4,	4,	4,	4,	4,
            5,	5,	5,	5,	5,	5,	5,	5,	5,	5,	5,	5,	5,	5,	5,	5,	5,
            6,	6,	6,	6,	6,	6,	6,	6,	6,	6,	6,	6,	6,	6,	6,	6,	6,
            7,	7,	7,	7,	7,	7,	7,	7,	7,	7,	7,	7,	7,	7,	7,	7,	7,
            8,	8,	8,	8,	8,	8,	8,	8,	8,	8,	8,	8,	8,	8,	8,	8,	8,
            9,	9,	9,	9,	9,	9,	9,	9,	9,	9,	9,	9,	9,	9,	9,	9,	9,
            10,	10,	10,	10,	10,	10,	10,	10,	10,	10,	10,	10,	10,	10,	10,	10,	10,
            11,	11,	11,	11,	11,	11,	11,	11,	11,	11,	11,	11,	11,	11,	11,	11,	11
        };

        public static readonly string[] POS_STR =
        {
            "",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",
            "",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",
            "",	"",	"",	"",	"a9",	"b9",	"c9",	"d9",	"e9",	"f9",	"g9",	"h9",	"i9",	"",	"",	"",	"",
            "",	"",	"",	"",	"a8",	"b8",	"c8",	"d8",	"e8",	"f8",	"g8",	"h8",	"i8",	"",	"",	"",	"",
            "",	"",	"",	"",	"a7",	"b7",	"c7",	"d7",	"e7",	"f7",	"g7",	"h7",	"i7",	"",	"",	"",	"",
            "",	"",	"",	"",	"a6",	"b6",	"c6",	"d6",	"e6",	"f6",	"g6",	"h6",	"i6",	"",	"",	"",	"",
            "",	"",	"",	"",	"a5",	"b5",	"c5",	"d5",	"e5",	"f5",	"g5",	"h5",	"i5",	"",	"",	"",	"",
            "",	"",	"",	"",	"a4",	"b4",	"c4",	"d4",	"e4",	"f4",	"g4",	"h4",	"i4",	"",	"",	"",	"",
            "",	"",	"",	"",	"a3",	"b3",	"c3",	"d3",	"e3",	"f3",	"g3",	"h3",	"i3",	"",	"",	"",	"",
            "",	"",	"",	"",	"a2",	"b2",	"c2",	"d2",	"e2",	"f2",	"g2",	"h2",	"i2",	"",	"",	"",	"",
            "",	"",	"",	"",	"a1",	"b1",	"c1",	"d1",	"e1",	"f1",	"g1",	"h1",	"i1",	"",	"",	"",	"",
            "",	"",	"",	"",	"a0",	"b0",	"c0",	"d0",	"e0",	"f0",	"g0",	"h0",	"i0",	"",	"",	"",	"",
            "",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",
            "",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	"",	""
        };


        static Board()
        {
            NONE_BOARD = new int[TOTAL_POS];
            IS_IN_BOARD = new bool[TOTAL_POS];
            IS_IN_RED_SIDE = new bool[TOTAL_POS];
            IS_IN_BLACK_SIDE = new bool[TOTAL_POS];
            IS_IN_RED_PALACE = new bool[TOTAL_POS];
            IS_IN_BLACK_PALACE = new bool[TOTAL_POS];

            for (int i = 0; i < 17 * 14; i++)
            {
                if (FILE[i] >= 0 && FILE[i] <= 8 && RANK[i] >= 0 && RANK[i] <= 9)
                {
                    NONE_BOARD[i] = NONE;    //空棋盘，里面没有一个子
                    IS_IN_BOARD[i] = true;   //边界内为true
                    IS_IN_RED_SIDE[i] = (RANK[i] >= 5 && RANK[i] <= 9);  //红方是从第5行到第9行
                    IS_IN_BLACK_SIDE[i] = (RANK[i] >= 0 && RANK[i] <= 4); //黑方是从第0行到第4行
                    IS_IN_RED_PALACE[i] = (FILE[i] >= 3 && FILE[i] <= 5 && RANK[i] >= 0 && RANK[i] <= 2);
                    IS_IN_BLACK_PALACE[i] = (FILE[i] >= 3 && FILE[i] <= 5 && RANK[i] >= 7 && RANK[i] <= 9);
                }
                else //边界位置
                {
                    NONE_BOARD[i] = BORDER;  // 边界上都写上一个特殊的常量，表示边界BORDER
                    IS_IN_BOARD[i] = false;  //边界上为false
                    IS_IN_RED_SIDE[i] = false;
                    IS_IN_BLACK_SIDE[i] = false;
                    IS_IN_RED_PALACE[i] = false;
                    IS_IN_BLACK_PALACE[i] = false;
                }
            }

            //   InitIncSliding();

        }

        /// <summary>
        /// 得到内部位置编号（0-237）的列号（0-8）
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static int File(int pos)
        {
            return FILE[pos];
        }

        /// <summary>
        /// 得到内部位置编号（0-237）的行号（0-9）
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static int Rank(int pos)
        {
            return RANK[pos];
        }



        /// <summary>
        /// 是否在黑方九宫中
        /// </summary>
        /// <param name="pos">位置</param>
        /// <returns>在黑方九宫为true，否则为false</returns>
        public static bool IsInBlackPalace(int pos)
        {
            return IS_IN_BLACK_PALACE[pos];
        }

        /// <summary>
        /// 是否在红方阵营
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool IsInRedSide(int pos)
        {
            return IS_IN_RED_SIDE[pos];
        }

        public static bool IsInBlackSide(int pos)
        {
            return IS_IN_BLACK_SIDE[pos];
        }

        /// <summary>
        /// 判断位置pos是否在真正的棋盘上，而不是处于17x14棋盘的边界上
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool IsInBoard(int pos)
        {
            return IS_IN_BOARD[pos];
        }


        /// <summary>
        /// 是否在红方九宫
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool IsInRedPalace(int pos)
        {
            return IS_IN_RED_PALACE[pos];
        }


        public static bool IsInRedAdvisorPlace(int pos)
        {
            if (pos == D0 || pos == F0 || pos == E1 || pos == D2 || pos == F2) return true;
            return false;
        }

        public static bool IsInRedBishopPlace(int pos)
        {
            if (pos == A2 || pos == C0 || pos == C4 || pos == E2
                          || pos == G0 || pos == G4 || pos == I2) return true;
            return false;
        }
        public static bool IsInBlackBishopPlace(int pos)
        {
            if (pos == A7 || pos == C9 || pos == C5 || pos == E7
                          || pos == G9 || pos == G5 || pos == I7) return true;
            return false;
        }

        public static bool IsInBlackAdvisorPlace(int pos)
        {
            if (pos == D9 || pos == F9 || pos == E8 || pos == D7 || pos == F7) return true;
            return false;
        }


    }
}
