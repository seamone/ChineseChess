using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ponder.ChineseChess
{
    /// <summary>
    /// 与Makemove和UnmakeMove有关的内容
    /// 为了以后能够并行计算，想把改变board中成员变量值的全部放在makemove和unmakemove中
    /// 其它方法都不改变board中子力的位置和索引等信息
    /// </summary>
    public partial class Board
    {
        private const int CONST_OFFSET = 161;

        // TODO: 要不要绝对值？
        private static readonly int[] FILE_DIST = 
        {
           -8,-7,-6,-5,-4,-3,-2,-1, 0, +1,+2,+3,+4,+5,+6,+7,+8, // -9
           -8,-7,-6,-5,-4,-3,-2,-1, 0, +1,+2,+3,+4,+5,+6,+7,+8, // -8
           -8,-7,-6,-5,-4,-3,-2,-1, 0, +1,+2,+3,+4,+5,+6,+7,+8, // -7
           -8,-7,-6,-5,-4,-3,-2,-1, 0, +1,+2,+3,+4,+5,+6,+7,+8, // -6
           -8,-7,-6,-5,-4,-3,-2,-1, 0, +1,+2,+3,+4,+5,+6,+7,+8, // -5
           -8,-7,-6,-5,-4,-3,-2,-1, 0, +1,+2,+3,+4,+5,+6,+7,+8, // -4
           -8,-7,-6,-5,-4,-3,-2,-1, 0, +1,+2,+3,+4,+5,+6,+7,+8, // -3
           -8,-7,-6,-5,-4,-3,-2,-1, 0, +1,+2,+3,+4,+5,+6,+7,+8, // -2
           -8,-7,-6,-5,-4,-3,-2,-1, 0, +1,+2,+3,+4,+5,+6,+7,+8, // -1
           -8,-7,-6,-5,-4,-3,-2,-1, 0, +1,+2,+3,+4,+5,+6,+7,+8, // 0
           -8,-7,-6,-5,-4,-3,-2,-1, 0, +1,+2,+3,+4,+5,+6,+7,+8, // +1
           -8,-7,-6,-5,-4,-3,-2,-1, 0, +1,+2,+3,+4,+5,+6,+7,+8, // +2
           -8,-7,-6,-5,-4,-3,-2,-1, 0, +1,+2,+3,+4,+5,+6,+7,+8, // +3
           -8,-7,-6,-5,-4,-3,-2,-1, 0, +1,+2,+3,+4,+5,+6,+7,+8, // +4
           -8,-7,-6,-5,-4,-3,-2,-1, 0, +1,+2,+3,+4,+5,+6,+7,+8, // +5
           -8,-7,-6,-5,-4,-3,-2,-1, 0, +1,+2,+3,+4,+5,+6,+7,+8, // +6
           -8,-7,-6,-5,-4,-3,-2,-1, 0, +1,+2,+3,+4,+5,+6,+7,+8, // +7
           -8,-7,-6,-5,-4,-3,-2,-1, 0, +1,+2,+3,+4,+5,+6,+7,+8, // +8
           -8,-7,-6,-5,-4,-3,-2,-1, 0, +1,+2,+3,+4,+5,+6,+7,+8  // +9
        };

        private static readonly int[] RANK_DIST = 
        {
            -9, -9, -9, -9, -9, -9, -9, -9, -9, -9, -9, -9, -9, -9, -9, -9, -9, // -9
            -8, -8, -8, -8, -8, -8, -8, -8, -8, -8, -8, -8, -8, -8, -8, -8, -8, // -8
            -7, -7, -7, -7, -7, -7, -7, -7, -7, -7, -7, -7, -7, -7, -7, -7, -7, // -7
            -6, -6, -6, -6, -6, -6, -6, -6, -6, -6, -6, -6, -6, -6, -6, -6, -6, // -6
            -5, -5, -5, -5, -5, -5, -5, -5, -5, -5, -5, -5, -5, -5, -5, -5, -5, // -5
            -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, // -4
            -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, // -3
            -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, -2, // -2
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, // -1
             0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, // 0               
             1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, // +1
             2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2,  2, // +2
             3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3,  3, // +3
             4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4,  4, // +4
             5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5,  5, // +5
             6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6, // +6
             7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7, // +7
             8,  8,  8,  8,  8,  8,  8,  8,  8,  8,  8,  8,  8,  8,  8,  8,  8, // +8
             9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9  // +9
        };


        //水平[-8..8]，共17，垂直[-9..9]，共19。17*19=323
        //new int[CONST_SLIDING + 17 * 19];
        private static readonly int[] INC_SLIDING = 
        {
            0, 0, 0, 0, 0, 0, 0, 0, -17, 0, 0, 0, 0, 0, 0, 0, 0, // -9
            0, 0, 0, 0, 0, 0, 0, 0, -17, 0, 0, 0, 0, 0, 0, 0, 0, // -8
            0, 0, 0, 0, 0, 0, 0, 0, -17, 0, 0, 0, 0, 0, 0, 0, 0, // -7
            0, 0, 0, 0, 0, 0, 0, 0, -17, 0, 0, 0, 0, 0, 0, 0, 0, // -6
            0, 0, 0, 0, 0, 0, 0, 0, -17, 0, 0, 0, 0, 0, 0, 0, 0, // -5
            0, 0, 0, 0, 0, 0, 0, 0, -17, 0, 0, 0, 0, 0, 0, 0, 0, // -4
            0, 0, 0, 0, 0, 0, 0, 0, -17, 0, 0, 0, 0, 0, 0, 0, 0, // -3
            0, 0, 0, 0, 0, 0, 0, 0, -17, 0, 0, 0, 0, 0, 0, 0, 0, // -2
            0, 0, 0, 0, 0, 0, 0, 0, -17, 0, 0, 0, 0, 0, 0, 0, 0, // -1
           -1,-1,-1,-1,-1,-1,-1,-1,  0, +1,+1,+1,+1,+1,+1,+1,+1, // 0
            0, 0, 0, 0, 0, 0, 0, 0,  17, 0, 0, 0, 0, 0, 0, 0, 0, // +1
            0, 0, 0, 0, 0, 0, 0, 0,  17, 0, 0, 0, 0, 0, 0, 0, 0, // +2
            0, 0, 0, 0, 0, 0, 0, 0,  17, 0, 0, 0, 0, 0, 0, 0, 0, // +3
            0, 0, 0, 0, 0, 0, 0, 0,  17, 0, 0, 0, 0, 0, 0, 0, 0, // +4
            0, 0, 0, 0, 0, 0, 0, 0,  17, 0, 0, 0, 0, 0, 0, 0, 0, // +5
            0, 0, 0, 0, 0, 0, 0, 0,  17, 0, 0, 0, 0, 0, 0, 0, 0, // +6
            0, 0, 0, 0, 0, 0, 0, 0,  17, 0, 0, 0, 0, 0, 0, 0, 0, // +7
            0, 0, 0, 0, 0, 0, 0, 0,  17, 0, 0, 0, 0, 0, 0, 0, 0, // +8
            0, 0, 0, 0, 0, 0, 0, 0,  17, 0, 0, 0, 0, 0, 0, 0, 0  // +9
        };

        // -17*9-8=-161   -17*9-7=-160   ...    -17*9-1=-154  -17*9=-153  -17*9-1=-152  ... -17*9+8=-145
        // -17*8-8=-144   -17*8-7=-143   ...    -17*8-1=-137  -17*8=-136  -17*8+1=-135  ... -17*8+8=-128
        //   ...            ...          ...       ...           ...         ...        ...      ...
        // -17*1-8        -17*1-7        ...    -17*1-1       -17*1= -17  -17*1+1= -16  ... -17*1+8=  -9
        //      -8             -7        ...         -1           0          +1         ...      +8
        // +17*1-8        +17*1-7        ...    +17*1-1       +17*1= +17  +17*1+1= -18  ... +17*1+8= +25
        //   ...            ...          ...       ...           ...         ...        ...      ...
        // +17*9-8=+145   +17*9-7=+146   ...    +17*9-1=+152  +17*9=+153  +17*9+1=+154  ... +17*9+8=+161
        // 可以用下面这段代码来初始化INC_SLIDING数组
        //public static void InitIncSliding()
        //{
        //    //盘面上格子最小的值是38，最大的是199，199-38=161
        //    //to - from的范围是[-161..+161]，共322个，都加上固定的偏移量161，可以使值都大于0
        //    for (int i = -161; i <= 161; i++)
        //    {
        //        if (i>= -8 && i <= 8)
        //        {
        //            INC_SLIDING[CONST_SLIDING + i] = Math.Sign(i);
        //        }
        //        else 
        //        {
        //            int file = i % 17;
        //            if (file == 0) INC_SLIDING[CONST_SLIDING + i] = Math.Sign(i) * 17;
        //        }
        //    }
        //}


        private static readonly int[] INC_BLOCK_KNIGHT_FROM = 
        {
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // -9
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // -8
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // -7
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // -6
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // -5
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // -4
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // -3
            0, 0, 0, 0, 0, 0, 0, -17, 0, -17,0, 0, 0, 0, 0, 0, 0, // -2
            0, 0, 0, 0, 0, 0,-1,  0,  0,  0, 1, 0, 0, 0, 0, 0, 0, // -1
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // 0
            0, 0, 0, 0, 0, 0,-1,  0,  0,  0, 1, 0, 0, 0, 0, 0, 0, // +1
            0, 0, 0, 0, 0, 0, 0, +17, 0, +17,0, 0, 0, 0, 0, 0, 0, // +2
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // +3
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // +4
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // +5
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // +6
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // +7
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // +8
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0  // +9
        };

        private static readonly int[] INC_BLOCK_KNIGHT_TO = 
        {
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // -9
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // -8
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // -7
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // -6
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // -5
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // -4
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // -3
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // -2
            0, 0, 0, 0, 0, 0, 0, -18, 0, -16,0, 0, 0, 0, 0, 0, 0, // -1
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // 0
            0, 0, 0, 0, 0, 0, 0, 16,  0, 18, 0, 0, 0, 0, 0, 0, 0, // +1
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // +2
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // +3
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // +4
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // +5
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // +6
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // +7
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, // +8
            0, 0, 0, 0, 0, 0, 0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0  // +9
        };

        public int FileDist(int from, int to)
        {
            return FILE_DIST[to - from + CONST_OFFSET];
        }

        public int RankDist(int from, int to)
        {
            return RANK_DIST[to - from + CONST_OFFSET];
        }

        public int IncSliding(int from, int to)
        {
            return INC_SLIDING[to - from + CONST_OFFSET];
        }

        public int IncBlockKnightTo(int from, int to)
        {
            return INC_BLOCK_KNIGHT_TO[to - from + CONST_OFFSET];
        }

        public int IncBlockKnightFrom(int from, int to)
        {
            return INC_BLOCK_KNIGHT_FROM[to - from + CONST_OFFSET];
        }
 

        /// <summary>
        /// 连续走多步
        /// </summary>
        /// <param name="moves"></param>
        public void MakeMoves(Move[] moves)
        {
            foreach (Move m in moves)
                MakeMove(m);
        }

        public void MakeMoves(string strMoves)
        {
            string[] s = strMoves.Split(' ');

            foreach (string move in s)
                MakeMove(BoardUtil.CreateMoveFromString(this, move));
        }

        /*! 
        * 走动一个着法，注意：此方法可能会修改move。
        * 在走动时可能会产生吃子，所以将move的引用传递进去，吃子时会修改move.Eaten的值。
        *  \param move 着法
        */

        //  ┌───┬───┬───┬───┬───┬───┬───┬───┬───┐    
        //9 │      │      │      │      │      │      │      │      │      │ 
        //  ├───┼───┼───┼───┼───┼───┼───┼───┼───┤    
        //8 │      │      │      │      │      │      │      │      │      │ 
        //  ├───┼───┼───┼───┼───┼───┼───┼───┼───┤    
        //7 │      │      │      │      │      │      │      │      │      │ 
        //  ├───┼───┼───┼───┼───┼───┼───┼───┼───┤    
        //6 │      │      │      │      │      │      │      │      │      │ 
        //  ├───┼───┼───┼───┼───┼───┼───┼───┼───┤    
        //5 │      │      │      │      │      │      │      │      │      │ 
        //  ├───┼───┼───┼───┼───┼───┼───┼───┼───┤    
        //4 │      │      │      │      │      │      │      │      │      │ 
        //  ├───┼───┼───┼───┼───┼───┼───┼───┼───┤    
        //3 │      │      │      │      │      │      │      │      │      │ 
        //  ├───┼───┼───┼───┼───┼───┼───┼───┼───┤    
        //2 │      │      │      │      │      │      │      │      │      │ 
        //  ├───┼───┼───┼───┼───┼───┼───┼───┼───┤    
        //1 │      │      │      │      │      │      │      │      │      │ 
        //  ├───┼───┼───┼───┼───┼───┼───┼───┼───┤    
        //0 │      │      │      │      │      │      │      │      │      │ 
        //  └───┴───┴───┴───┴───┴───┴───┴───┴───┘    
        //      a        b       c       d       e       f       g       h       i
        /// <summary>
        /// 注意：走动一步后，就自动该对方走棋了，里面调用了ChangeTurn()方法
        /// TODO: 应该增量更新board的hash值，需要在board中增加一个属性zobristHash
        /// </summary>
        /// <param name="move"></param>
        public void MakeMove(Move move)
        {
            int piece = move.Piece;
            int from = move.From;
            int to = move.To;

            //move.Capture = pieceData[to]; 

            pieceData[from] = 0;
            pieceData[to] = piece;

            UpdateIndexForMakemove(piece, from, to);

            if (move.Capture != 0)
            {
                UpdateIndexForMakemove(move.Capture, to);
            }

            //UpdateForeRedCannon();
            //UpdateForeBlackCannon();

            // 走动一步后，就设置为轮到对方走棋了
            ChangeTurn();

            // TODO: 增量更新！！
            this.ZobristKey = Zobrist.ZoristHash(this);
        }

        private void UpdateIndexForMakemove(int eatPiece, int to)
        {
            int idxPiece = Board.PieceType(eatPiece);

            if (IsRedPiece(eatPiece))
            {
                --TotalRedPiece;
                --RedPieceNum[idxPiece];
                int index = RedPieceIndex[to];
                int square = RedPiecePos[idxPiece, RedPieceNum[idxPiece]];
                RedPiecePos[idxPiece, index] = square;
                RedPieceIndex[square] = index;
            }
            else
            {
                --TotalBlackPiece;
                --BlackPieceNum[idxPiece];
                int index = BlackPieceIndex[to];
                int square = BlackPiecePos[idxPiece, BlackPieceNum[idxPiece]];
                BlackPiecePos[idxPiece, index] = square;
                BlackPieceIndex[square] = index;
            }


            //switch (eatPiece)
            //{
            //    case RED_ROOK:
            //        RedPieceCount--;
            //        RedPieces[TYPE_ROOK]--;
            //        index = RedIndex[to];
            //        square = RedPieceList[TYPE_ROOK, RedPieces[TYPE_ROOK]];
            //        RedPieceList[TYPE_ROOK, index] = square;
            //        RedIndex[square] = index;
            //        break;
            //    case RED_KNIGHT:
            //        RedPieceCount--;
            //        RedPieces[TYPE_KNIGHT]--;
            //        index = RedIndex[to];
            //        square = RedPieceList[TYPE_KNIGHT, RedPieces[TYPE_KNIGHT]];
            //        RedPieceList[TYPE_KNIGHT, index] = square;
            //        RedIndex[square] = index;
            //        break;
            //    case RED_CANNON:
            //        RedPieceCount--;
            //        RedPieces[TYPE_CANNON]--;
            //        index = RedIndex[to];
            //        square = RedPieceList[TYPE_CANNON, RedPieces[TYPE_CANNON]];
            //        RedPieceList[TYPE_CANNON, index] = square;
            //        RedIndex[square] = index;
            //        break;
            //    case RED_BISHOP:
            //        RedPieceCount--;
            //        RedPieces[TYPE_BISHOP]--;
            //        index = RedIndex[to];
            //        square = RedPieceList[TYPE_BISHOP, RedPieces[TYPE_BISHOP]];
            //        RedPieceList[TYPE_BISHOP, index] = square;
            //        RedIndex[square] = index;
            //        break;
            //    case RED_ADVISOR:
            //        RedPieceCount--;
            //        RedPieces[TYPE_ADVISOR]--;
            //        index = RedIndex[to];
            //        square = RedPieceList[TYPE_ADVISOR, RedPieces[TYPE_ADVISOR]];
            //        RedPieceList[TYPE_ADVISOR, index] = square;
            //        RedIndex[square] = index;
            //        break;
            //    case RED_PAWN:
            //        RedPieceCount--;
            //        RedPieces[TYPE_PAWN]--;
            //        index = RedIndex[to];
            //        square = RedPieceList[TYPE_PAWN, RedPieces[TYPE_PAWN]];
            //        RedPieceList[TYPE_PAWN, index] = square;
            //        RedIndex[square] = index;
            //        break;
            //    case BLACK_ROOK:
            //        BlackPieceCount--;
            //        BlackPieces[TYPE_ROOK]--;
            //        index = BlackIndex[to];
            //        square = BlackPieceList[TYPE_ROOK, BlackPieces[TYPE_ROOK]];
            //        BlackPieceList[TYPE_ROOK, index] = square;
            //        BlackIndex[square] = index;
            //        break;
            //    case BLACK_KNIGHT:
            //        BlackPieceCount--;
            //        BlackPieces[TYPE_KNIGHT]--;
            //        index = BlackIndex[to];
            //        square = BlackPieceList[TYPE_KNIGHT, BlackPieces[TYPE_KNIGHT]];
            //        BlackPieceList[TYPE_KNIGHT, index] = square;
            //        BlackIndex[square] = index;
            //        break;
            //    case BLACK_CANNON:
            //        BlackPieceCount--;
            //        BlackPieces[TYPE_CANNON]--;
            //        index = BlackIndex[to];
            //        square = BlackPieceList[TYPE_CANNON, BlackPieces[TYPE_CANNON]];
            //        BlackPieceList[TYPE_CANNON, index] = square;
            //        BlackIndex[square] = index;
            //        break;
            //    case BLACK_BISHOP:
            //        BlackPieceCount--;
            //        BlackPieces[TYPE_BISHOP]--;
            //        index = BlackIndex[to];
            //        square = BlackPieceList[TYPE_BISHOP, BlackPieces[TYPE_BISHOP]];
            //        BlackPieceList[TYPE_BISHOP, index] = square;
            //        BlackIndex[square] = index;
            //        break;
            //    case BLACK_ADVISOR:
            //        BlackPieceCount--;
            //        BlackPieces[TYPE_ADVISOR]--;
            //        index = BlackIndex[to];
            //        square = BlackPieceList[TYPE_ADVISOR, BlackPieces[TYPE_ADVISOR]];
            //        BlackPieceList[TYPE_ADVISOR, index] = square;
            //        BlackIndex[square] = index;
            //        break;
            //    case BLACK_PAWN:
            //        BlackPieceCount--;
            //        BlackPieces[TYPE_PAWN]--;
            //        index = BlackIndex[to];
            //        square = BlackPieceList[TYPE_PAWN, BlackPieces[TYPE_PAWN]];
            //        BlackPieceList[TYPE_PAWN, index] = square;
            //        BlackIndex[square] = index;
            //        break;
            //}
        }

        private void UpdateIndexForMakemove(int piece, int from, int to)
        {
            int idxPiece = Board.PieceType(piece);

            if (IsRedPiece(piece))
            {
                int index = RedPieceIndex[from];
                RedPieceIndex[to] = index;
                RedPiecePos[idxPiece, index] = to;
            }
            else
            {
                int index = BlackPieceIndex[from];
                BlackPieceIndex[to] = index;
                BlackPiecePos[idxPiece, index] = to;
            }

            //int index;
            //switch (piece)
            //{
            //    case RED_KING:
            //        RedKingPos = to;
            //        break;
            //    case RED_ROOK:
            //        index = redIndex[from];
            //        redIndex[to] = index;
            //        RedPieceList[IDX_ROOK, index] = to;
            //        break;
            //    case RED_KNIGHT:
            //        index = redIndex[from];
            //        redIndex[to] = index;
            //        RedPieceList[IDX_KNIGHT, index] = to;
            //        break;
            //    case RED_CANNON:
            //        index = redIndex[from];
            //        redIndex[to] = index;
            //        RedPieceList[IDX_CANNON, index] = to;
            //        break;
            //    case RED_BISHOP:
            //        index = redIndex[from];
            //        redIndex[to] = index;
            //        RedPieceList[IDX_BISHOP, index] = to;
            //        break;
            //    case RED_ADVISOR:
            //        index = redIndex[from];
            //        redIndex[to] = index;
            //        RedPieceList[IDX_ADVISOR, index] = to;
            //        break;
            //    case RED_PAWN:
            //        index = redIndex[from];
            //        redIndex[to] = index;
            //        RedPieceList[IDX_PAWN, index] = to;
            //        break;

            //    /////////////////////////////////
            //    case BLACK_KING:
            //        BlackKingPos = to;
            //        break;
            //    case BLACK_ROOK:
            //        index = blackIndex[from];
            //        blackIndex[to] = index;
            //        BlackPieceList[IDX_ROOK, index] = to;
            //        break;
            //    case BLACK_KNIGHT:
            //        index = blackIndex[from];
            //        blackIndex[to] = index;
            //        BlackPieceList[IDX_KNIGHT, index] = to;
            //        break;
            //    case BLACK_CANNON:
            //        index = blackIndex[from];
            //        blackIndex[to] = index;
            //        BlackPieceList[IDX_CANNON, index] = to;
            //        break;
            //    case BLACK_BISHOP:
            //        index = blackIndex[from];
            //        blackIndex[to] = index;
            //        BlackPieceList[IDX_BISHOP, index] = to;
            //        break;
            //    case BLACK_ADVISOR:
            //        index = blackIndex[from];
            //        blackIndex[to] = index;
            //        BlackPieceList[IDX_ADVISOR, index] = to;
            //        break;
            //    case BLACK_PAWN:
            //        index = blackIndex[from];
            //        blackIndex[to] = index;
            //        BlackPieceList[IDX_PAWN, index] = to;
            //        break;
            //}
        }

        /*! 撤消一步移动
        *  \param move 被撤消的着法
        */
        public void UnmakeMove(Move move)
        {
            int piece = move.Piece;// /* move.Piece; */ FindPiece(posTo);
            int from = move.From;
            int to = move.To;
            int eaten = move.Capture;


            pieceData[from] = piece;
            pieceData[to] = eaten;

            UpdatePieceListForUnmakemove(piece, from, to);

            if (eaten != 0)
            {
                UpdateIndexForUnmakemove(eaten, to);
            }


            ChangeTurn();

            // TODO: 增量更新！！
            this.ZobristKey = Zobrist.ZoristHash(this);
        }

        private void UpdateIndexForUnmakemove(int eatPiece, int to)
        {
            int idxPiece = Board.PieceType(eatPiece);

            if (IsRedPiece(eatPiece))
            {
                RedPiecePos[idxPiece, RedPieceNum[idxPiece]] = to;
                RedPieceIndex[to] = RedPieceNum[idxPiece];
                ++RedPieceNum[idxPiece];
                ++TotalRedPiece;
            }
            else
            {
                BlackPiecePos[idxPiece, BlackPieceNum[idxPiece]] = to;
                BlackPieceIndex[to] = BlackPieceNum[idxPiece];
                ++BlackPieceNum[idxPiece];
                ++TotalBlackPiece;
            }

            //switch (eatPiece)
            //{
            //    //这里面没有帅和将，不可能被吃掉
            //    case RED_ROOK:
            //        RedPieceList[TYPE_ROOK, RedPieces[TYPE_ROOK]] = to;
            //        RedIndex[to] = RedPieces[TYPE_ROOK];
            //        RedPieces[TYPE_ROOK]++;
            //        RedPieceCount++;
            //        break;
            //    case RED_KNIGHT:
            //        RedPieceList[TYPE_KNIGHT, RedPieces[TYPE_KNIGHT]] = to;
            //        RedIndex[to] = RedPieces[TYPE_KNIGHT];
            //        RedPieces[TYPE_KNIGHT]++;
            //        RedPieceCount++;
            //        break;
            //    case RED_CANNON:
            //        RedPieceList[TYPE_CANNON, RedPieces[TYPE_CANNON]] = to;
            //        RedIndex[to] = RedPieces[TYPE_CANNON];
            //        RedPieces[TYPE_CANNON]++;
            //        RedPieceCount++;
            //        break;
            //    case RED_BISHOP:
            //        RedPieceList[TYPE_BISHOP, RedPieces[TYPE_BISHOP]] = to;
            //        RedIndex[to] = RedPieces[TYPE_BISHOP];
            //        RedPieces[TYPE_BISHOP]++;
            //        RedPieceCount++;
            //        break;
            //    case RED_ADVISOR:
            //        RedPieceList[TYPE_ADVISOR, RedPieces[TYPE_ADVISOR]] = to;
            //        RedIndex[to] = RedPieces[TYPE_ADVISOR];
            //        RedPieces[TYPE_ADVISOR]++;
            //        RedPieceCount++;
            //        break;
            //    case RED_PAWN:
            //        RedPieceList[TYPE_PAWN, RedPieces[TYPE_PAWN]] = to;
            //        RedIndex[to] = RedPieces[TYPE_PAWN];
            //        RedPieces[TYPE_PAWN]++;
            //        RedPieceCount++;
            //        break;


            //    ///////////////////////

            //    case BLACK_ROOK:
            //        BlackPieceList[TYPE_ROOK, BlackPieces[TYPE_ROOK]] = to;
            //        BlackIndex[to] = BlackPieces[TYPE_ROOK];
            //        BlackPieces[TYPE_ROOK]++;
            //        BlackPieceCount++;
            //        break;
            //    case BLACK_KNIGHT:
            //        BlackPieceList[TYPE_KNIGHT, BlackPieces[TYPE_KNIGHT]] = to;
            //        BlackIndex[to] = BlackPieces[TYPE_KNIGHT];
            //        BlackPieces[TYPE_KNIGHT]++;
            //        BlackPieceCount++;
            //        break;
            //    case BLACK_CANNON:
            //        BlackPieceList[TYPE_CANNON, BlackPieces[TYPE_CANNON]] = to;
            //        BlackIndex[to] = BlackPieces[TYPE_CANNON];
            //        BlackPieces[TYPE_CANNON]++;
            //        BlackPieceCount++;
            //        break;
            //    case BLACK_BISHOP:
            //        BlackPieceList[TYPE_BISHOP, BlackPieces[TYPE_BISHOP]] = to;
            //        BlackIndex[to] = BlackPieces[TYPE_BISHOP];
            //        BlackPieces[TYPE_BISHOP]++;
            //        BlackPieceCount++;
            //        break;
            //    case BLACK_ADVISOR:
            //        BlackPieceList[TYPE_ADVISOR, BlackPieces[TYPE_ADVISOR]] = to;
            //        BlackIndex[to] = BlackPieces[TYPE_ADVISOR];
            //        BlackPieces[TYPE_ADVISOR]++;
            //        BlackPieceCount++;
            //        break;
            //    case BLACK_PAWN:
            //        BlackPieceList[TYPE_PAWN, BlackPieces[TYPE_PAWN]] = to;
            //        BlackIndex[to] = BlackPieces[TYPE_PAWN];
            //        BlackPieces[TYPE_PAWN]++;
            //        BlackPieceCount++;
            //        break;
            //}
        }

        private void UpdatePieceListForUnmakemove(int piece, int from, int to)
        {
            int idxPiece = Board.PieceType(piece);

            if (IsRedPiece(piece))
            {
                int index = RedPieceIndex[to];
                RedPieceIndex[from] = index;
                RedPiecePos[idxPiece, index] = from;
            }
            else
            {
                int index = BlackPieceIndex[to];
                BlackPieceIndex[from] = index;
                BlackPiecePos[idxPiece, index] = from;
            }

            //int index;
            //switch (piece)
            //{
            //    case RED_KING: //帅只有一个，其它信息不用更新
            //        RedPieceList[TYPE_KING, 0] = from;
            //        break;
            //    case RED_ROOK:
            //        index = RedIndex[to];
            //        RedIndex[from] = index;
            //        RedPieceList[TYPE_ROOK, index] = from;
            //        break;
            //    case RED_KNIGHT:
            //        index = RedIndex[to];
            //        RedIndex[from] = index;
            //        RedPieceList[TYPE_KNIGHT, index] = from;
            //        break;
            //    case RED_CANNON:
            //        index = RedIndex[to];
            //        RedIndex[from] = index;
            //        RedPieceList[TYPE_CANNON, index] = from;
            //        break;
            //    case RED_BISHOP:
            //        index = RedIndex[to];
            //        RedIndex[from] = index;
            //        RedPieceList[TYPE_BISHOP, index] = from;
            //        break;
            //    case RED_ADVISOR:
            //        index = RedIndex[to];
            //        RedIndex[from] = index;
            //        RedPieceList[TYPE_ADVISOR, index] = from;
            //        break;
            //    case RED_PAWN:
            //        index = RedIndex[to];
            //        RedIndex[from] = index;
            //        RedPieceList[TYPE_PAWN, index] = from;
            //        break;

            //    /////////////////////////////////
            //    case BLACK_KING:
            //        BlackPieceList[TYPE_KING, 0] = from;
            //        break;
            //    case BLACK_ROOK:
            //        index = BlackIndex[to];
            //        BlackIndex[from] = index;
            //        BlackPieceList[TYPE_ROOK, index] = from;
            //        break;
            //    case BLACK_KNIGHT:
            //        index = BlackIndex[to];
            //        BlackIndex[from] = index;
            //        BlackPieceList[TYPE_KNIGHT, index] = from;
            //        break;
            //    case BLACK_CANNON:
            //        index = BlackIndex[to];
            //        BlackIndex[from] = index;
            //        BlackPieceList[TYPE_CANNON, index] = from;
            //        break;
            //    case BLACK_BISHOP:
            //        index = BlackIndex[to];
            //        BlackIndex[from] = index;
            //        BlackPieceList[TYPE_BISHOP, index] = from;
            //        break;
            //    case BLACK_ADVISOR:
            //        index = BlackIndex[to];
            //        BlackIndex[from] = index;
            //        BlackPieceList[TYPE_ADVISOR, index] = from;
            //        break;
            //    case BLACK_PAWN:
            //        index = BlackIndex[to];
            //        BlackIndex[from] = index;
            //        BlackPieceList[TYPE_PAWN, index] = from;
            //        break;
            //}
        }

    }
}
