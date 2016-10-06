using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ponder.ChineseChess;

namespace Ponder.Engine
{
    /// <summary>
    /// 局面评估
    /// 2013/03/11 slb  把以前的c++改为c#，子力评估和位置评估
    /// </summary>
    public class Evaluator
    {

        public const int MIN_EVAL_VALUE = -30000;
        public const int MAX_EVAL_VALUE = 30000;



        /*! 子力价值
         * 排列顺序:
         * RED_PIECES，RED_ROOK，RED_KNIGHT，RED_CANNON，RED_BISHOP，RED_ADVISOR，RED_PAWN，RED_KING
         * BLACK_PIECES，BLACK_ROOK，BLACK_KNIGHT，BLACK_CANNON，BLACK_BISHOP，BLACK_ADVISOR，BLACK_PAWN，BLACK_KING
         */
        public static int[] RedMaterialValue = 
        {
        //  KING,     ROOK,  KNIGHT,  CANNON,  BISHOP,  ADVISOR, PAWN
            5000,     1000,  450,     450,     200,     200,     100
        };

        public static int[] BlackMaterialValue = 
        {
        //  KING,     ROOK,  KNIGHT,  CANNON,  BISHOP,  ADVISOR, PAWN
           -5000,    -1000, -450,    -450,    -200,    -200,    -100
        };

        public static int[] RED_KING_POSITION_VALUE =
        {                 
             0, 0,  0,  0,     0,  0,  0,  0,  0,  0,  0,  0,  0,    0, 0,  0,  0, 
             0, 0,  0,  0,     0,  0,  0,  0,  0,  0,  0,  0,  0,    0, 0,  0,  0, 
             0, 0,  0,  0,     0,  0,  0,  0,  0,  0,  0,  0,  0,    0, 0,  0,  0, 
             0, 0,  0,  0,     0,  0,  0,  0,  0,  0,  0,  0,  0,    0, 0,  0,  0, 
             0, 0,  0,  0,     0,  0,  0,  0,  0,  0,  0,  0,  0,    0, 0,  0,  0, 
             0, 0,  0,  0,     0,  0,  0,  0,  0,  0,  0,  0,  0,    0, 0,  0,  0, 
             0, 0,  0,  0,     0,  0,  0,  0,  0,  0,  0,  0,  0,    0, 0,  0,  0, 
             0, 0,  0,  0,     0,  0,  0,  0,  0,  0,  0,  0,  0,    0, 0,  0,  0, 
             0, 0,  0,  0,     0,  0,  0,  0,  0,  0,  0,  0,  0,    0, 0,  0,  0, 
             0, 0,  0,  0,     0,  0,  0,  1,  1,  1,  0,  0,  0,    0, 0,  0,  0, 
             0, 0,  0,  0,     0,  0,  0,  2,  2,  2,  0,  0,  0,    0, 0,  0,  0, 
             0, 0,  0,  0,     0,  0,  0, 11, 15, 11,  0,  0,  0,    0, 0,  0,  0,
             0, 0,  0,  0,     0,  0,  0,  0,  0,  0,  0,  0,  0,    0, 0,  0,  0, 
             0, 0,  0,  0,     0,  0,  0,  0,  0,  0,  0,  0,  0,    0, 0,  0,  0 
      };

        /*! 红车的位置评估
         * 这里按黑方在上，红方在下的方式排列，在初始化Initilize()的代码里
         * 用(89-pos)把它们与内部表示法相对应起来
         */
        public static int[] RED_ROOK_POSITION_VALUE =
        {
           0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0, 
           0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0, 
            0, 0,  0,  0,  14, 14, 12, 18, 16, 18, 12, 14, 14, 0, 0,  0,  0, 
            0, 0,  0,  0,  16, 20, 18, 24, 26, 24, 18, 20, 16, 0, 0,  0,  0, 
            0, 0,  0,  0,  12, 12, 12, 18, 18, 18, 12, 12, 12, 0, 0,  0,  0, 
            0, 0,  0,  0,  12, 18, 16, 22, 22, 22, 16, 18, 12, 0, 0,  0,  0, 
            0, 0,  0,  0,  12, 14, 12, 18, 18, 18, 12, 14, 12, 0, 0,  0,  0, 
            0, 0,  0,  0,  12, 16, 14, 20, 20, 20, 14, 16, 12, 0, 0,  0,  0, 
            0, 0,  0,  0,  6, 10,  8, 14, 14, 14,  8, 10,  6,  0, 0,  0,  0, 
            0, 0,  0,  0,  4,  8,  6, 14, 12, 14,  6,  8,  4,  0, 0,  0,  0, 
            0, 0,  0,  0,  8,  4,  8, 16,  8, 16,  8,  4,  8,  0, 0,  0,  0, 
            0, 0,  0,  0,  -2, 10,  6, 14, 12, 14,  6, 10, -2,  0, 0,  0,  0,
           0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0, 
           0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0
        };

        public static int[] RED_KNIGHT_POSITION_VALUE = 
        {
            0, 0,  0,  0,       0,   0,  0,  0,   0,   0,  0,  0,  0,     0, 0,  0,  0, 
            0, 0,  0,  0,       0,   0,  0,  0,   0,   0,  0,  0,  0,     0, 0,  0,  0, 
            0, 0,  0,  0,       4,   8, 16, 12,   4,  12, 16,  8,  4,     0, 0,  0,  0, 
            0, 0,  0,  0,       4,  10, 28, 16,   8,  16, 28, 10,  4,     0, 0,  0,  0, 
            0, 0,  0,  0,      12,  14, 16, 20,  18,  20, 16, 14, 12,     0, 0,  0,  0, 
            0, 0,  0,  0,       8,  24, 18, 24,  20,  24, 18, 24,  8,     0, 0,  0,  0, 
            0, 0,  0,  0,       6,  16, 14, 18,  16,  18, 14, 16,  6,     0, 0,  0,  0, 
            0, 0,  0,  0,       4,  12, 16, 14,  12,  14, 16, 12,  4,     0, 0,  0,  0, 
            0, 0,  0,  0,       2,   6,  8,  6,  10,   6,  8,  6,  2,     0, 0,  0,  0, 
            0, 0,  0,  0,       4,   2,  8,  8,   4,   8,  8,  2,  4,     0, 0,  0,  0, 
            0, 0,  0,  0,       0,   2,  4,  4,  -2,   4,  4,  2,  0,     0, 0,  0,  0, 
            0, 0,  0,  0,       0,  -4,  0,  0,   0,   0,  0, -4,  0,     0, 0,  0,  0,
            0, 0,  0,  0,       0,   0,  0,  0,   0,   0,  0,  0,  0,     0, 0,  0,  0, 
            0, 0,  0,  0,       0,   0,  0,  0,   0,   0,  0,  0,  0,     0, 0,  0,  0
     };

        public static int[] RED_CANNON_POSITION_VALUE =
        {
            0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0, 
           0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0, 
          0, 0,  0,  0,   6,  4,  0, -10, -12, -10, 0, 4, 6, 0, 0,  0,  0, 
           0, 0,  0,  0,   2,  2,  0, -4,  -14,  -4, 0, 2, 2, 0, 0,  0,  0, 
           0, 0,  0,  0,   2,  2,  0, -10,  -8, -10, 0, 2, 2, 0, 0,  0,  0, 
           0, 0,  0,  0,   0,  0, -2,  4,   10,   4, -2, 0, 0, 0, 0,  0,  0, 
           0, 0,  0,  0,   0,  0,  0,  2,    8,   2, 0, 0, 0, 0, 0,  0,  0, 
           0, 0,  0,  0,   -2, 0,  4,  2,    6,   2, 4, 0, -2, 0, 0,  0,  0, 
           0, 0,  0,  0,   0,  0,  0,  2,    4,   2, 0, 0, 0, 0, 0,  0,  0, 
           0, 0,  0,  0,   4,  0,  8,  6,   10,   6, 8, 0, 4, 0, 0,  0,  0, 
           0, 0,  0,  0,   0,  2,  4,  6,    6,   6, 4, 2, 0, 0, 0,  0,  0, 
           0, 0,  0,  0,   0,  0,  2,  6,    6,   6, 2, 0, 0,  0, 0,  0,  0,
             0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0, 
           0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0
      };

        public static int[] RED_BISHOP_POSITION_VALUE =
        {
            0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0, 
           0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0, 
          0, 0,  0,  0,   0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 0,  0,  0, 
           0, 0,  0,  0,   0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 0,  0,  0, 
           0, 0,  0,  0,   0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 0,  0,  0, 
           0, 0,  0,  0,   0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 0,  0,  0, 
           0, 0,  0,  0,   0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 0,  0,  0, 
           0, 0,  0,  0,   0,  0, 40,  0,  0,  0, 40,  0,  0,  0, 0,  0,  0, 
           0, 0,  0,  0,   0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 0,  0,  0, 
           0, 0,  0,  0,   38,  0,  0,  0, 43,  0,  0,  0, 38, 0, 0,  0,  0,  
           0, 0,  0,  0,   0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 0,  0,  0, 
           0, 0,  0,  0,   0,  0, 40,  0,  0,  0, 40,  0,  0,  0, 0,  0,  0,
              0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0, 
           0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0
     };

        public static int[] RED_ADVISOR_POSITION_VALUE =
        {
           0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0, 
           0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0, 
            0, 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 0,  0,  0, 
            0, 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 0,  0,  0, 
            0, 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 0,  0,  0, 
            0, 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 0,  0,  0, 
            0, 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 0,  0,  0, 
            0, 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 0,  0,  0, 
            0, 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 0,  0,  0, 
            0, 0,  0,  0,  0,  0,  0, 20,  0, 20,  0,  0,  0,  0, 0,  0,  0, 
            0, 0,  0,  0,  0,  0,  0,  0, 23,  0,  0,  0,  0,  0, 0,  0,  0, 
            0, 0,  0,  0,  0,  0,  0, 20,  0, 20,  0,  0,  0,   0, 0,  0,  0, 
           0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0, 
           0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0
        };

        public static int[] RED_PAWN_POSITION_VALUE =
        {
           0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0, 
           0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0, 
           0, 0,  0,  0,   0,   3,  6,  9, 12,    9,  6,  3,  0,   0, 0,  0,  0, 
           0, 0,  0,  0,   18, 36, 56, 80, 120,  80, 56, 36, 18,   0, 0,  0,  0, 
           0, 0,  0,  0,   14, 26, 42, 60,  80,  60, 42, 26, 14,   0, 0,  0,  0, 
           0, 0,  0,  0,   10, 20, 30, 34,  40,  34, 30, 20, 10,   0, 0,  0,  0, 
           0, 0,  0,  0,   6,  12, 18, 18,  20,  18, 18, 12,  6,   0, 0,  0,  0, 
           0, 0,  0,  0,   2,   0,  8,  0,   8,   0,  8,  0,  2,   0, 0,  0,  0, 
           0, 0,  0,  0,   0,   0, -2,  0,   4,   0, -2,  0,  0,   0, 0,  0,  0, 
           0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0, 
           0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0, 
           0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0, 
           0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0, 
           0, 0,  0,  0,   0,   0,  0,  0,   0,   0,  0,  0,  0,   0, 0,  0,  0
        };




        // 0-6表示棋子的ID, 17*14个位置
        public static int[,] RedPositionValue;   //[7][17*14];
        public static int[,] BlackPositionValue; //[7][17*14];


        /*! 初始化位置评估的初始值
        */
        static Evaluator()
        {
            RedPositionValue = new int[7, Board.NX * Board.NY];
            BlackPositionValue = new int[7, Board.NX * Board.NY];

            for (int pos = 0; pos < Board.NX * Board.NY; pos++)
            {
                RedPositionValue[Board.TYPE_KING, pos] = RED_KING_POSITION_VALUE[pos];
                RedPositionValue[Board.TYPE_ROOK, pos] = RED_ROOK_POSITION_VALUE[pos];
                RedPositionValue[Board.TYPE_KNIGHT, pos] = RED_KNIGHT_POSITION_VALUE[ pos];
                RedPositionValue[Board.TYPE_CANNON, pos] = RED_CANNON_POSITION_VALUE[ pos];
                RedPositionValue[Board.TYPE_BISHOP, pos] = RED_BISHOP_POSITION_VALUE[ pos];
                RedPositionValue[Board.TYPE_ADVISOR, pos] = RED_ADVISOR_POSITION_VALUE[ pos];
                RedPositionValue[Board.TYPE_PAWN, pos] = RED_PAWN_POSITION_VALUE[ pos];

                //黑方的情况与红方的值正好对称，互为相反数,17*14-1=237
                BlackPositionValue[Board.TYPE_KING, pos] = -RED_KING_POSITION_VALUE[237-pos];
                BlackPositionValue[Board.TYPE_ROOK, pos] = -RED_ROOK_POSITION_VALUE[237 - pos];
                BlackPositionValue[Board.TYPE_KNIGHT, pos] = -RED_KNIGHT_POSITION_VALUE[237 - pos];
                BlackPositionValue[Board.TYPE_CANNON, pos] = -RED_CANNON_POSITION_VALUE[237 - pos];
                BlackPositionValue[Board.TYPE_BISHOP, pos] = -RED_BISHOP_POSITION_VALUE[237 - pos];
                BlackPositionValue[Board.TYPE_ADVISOR, pos] = -RED_ADVISOR_POSITION_VALUE[237 - pos];
                BlackPositionValue[Board.TYPE_PAWN, pos] = -RED_PAWN_POSITION_VALUE[237 - pos];
            }
        }


        /*! 这个评估方法与轮哪方走棋有关，这个评估方法会用在NegaMax等搜索方法中！
         * 当从红方的角度看来，如果对自己（红方）有利时，返回一个正值，如+100；
         * 当从黑方的角度看来，如果对自己（黑方）有利时，也返回一个正值。
         * \param board 盘面
         * \return 当前轮到谁走棋，就从谁的角度来得到评估值，正值表示对自己有利，负值表示对自己不利
         */
        public static int EvaluateAllWithSide(Board board)
        {
            int v = EvaluateAll(board);
            return board.IsRedTurn ? v : -v;
        }

        /*! 将各种评估值累加起来，就是最后的静态评估值
         * 这个评估值与轮到哪方走棋无关。当红方有利时，返回一个正值，如+100；而对于黑方有利时，返回-100。
         * \param board 盘面
         * \return 评估值，与轮到谁走无关
         */
        public static int EvaluateAll(Board board)
        {
            int em = EvaluateMaterial(board);
            int ep = EvaluatePosition(board);

#if TEST_ALPHA_BETA  //在测试alphabeta搜索算法时，设置这个变量，只评估子力和位置
            int e = em + ep;
            return e;
#else
            int e = em + ep;
            return e;
#endif
        }


        /*! 子力评估值
        * \param board 盘面
        * \return 评估值，与轮到谁走无关
        */
        public static int EvaluateMaterial(Board board)
        {
            int v = 0;
            for (int i = 1; i <= 6; i++)  //不用计算“帅”“将”
            {
                v += board.RedPieceNum[i] * RedMaterialValue[i];
                v += board.BlackPieceNum[i] * BlackMaterialValue[i];
            }

            return v;
        }


        /*! 位置评估值
        * \param board 盘面
        * \return 评估值，与轮到谁走无关
        */
        public static int EvaluatePosition(Board board)
        {
            int v = 0;

            for (int piece = 0; piece < 7; piece++)
            {
                for (int idx = 0; idx < board.RedPieceNum[piece]; idx++)
                {
                    int pos = board.RedPiecePos[piece,idx];
                    v += RedPositionValue[piece, pos];
                }
                for (int idx = 0; idx < board.BlackPieceNum[piece]; idx++)
                {
                    int pos = board.BlackPiecePos[piece, idx];
                    v += BlackPositionValue[piece, pos];
                }
            }
            return v;
        }




    }
}
