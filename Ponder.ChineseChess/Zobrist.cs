using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ponder.ChineseChess
{
    /// <summary>
    /// 实现Zobrist散列值
    /// </summary>
    public static class Zobrist
    {
        /// <summary>
        /// 红方7子，黑方7子，共14种子力
        /// </summary>
        private const int TOTAL_PIECE_TYPES = 14;

        /// <summary>
        /// 分配(14种棋子) * (17 x 14)个随机的64位整数
        /// 0-6,7-13：共14组，17x14代表棋盘的格子数
        /// </summary>
        private static ulong[,] redZobristKeys = new ulong[7, Board.TOTAL_POS];
        private static ulong[,] blackZobristKeys = new ulong[7, Board.TOTAL_POS];

        /// <summary>
        /// 如果是黑方走棋，则要另外与这个值进行异或
        /// </summary>
        private static ulong zobristKeysBlackTurn;

        /// <summary>
        /// Hashtable for finding the Zobrist key index of a piece type. 
        /// </summary>
        private static readonly Dictionary<Type, int> pieceZobristIndexTable;

        /// <summary>
        /// 静态初始化函数
        /// </summary>
        static Zobrist()
        {
            //Random r = new Random(1234);
            //byte[] b = new byte[8];
            //r.NextBytes(b);
            //zobristKeysBlackTurn = BitConverter.ToUInt64(b, 0);

            // 据说MersenneTwiser算法生成的随机机更好
            RandomMersenneTwister ran = new RandomMersenneTwister(1971);
            zobristKeysBlackTurn = ran.NextUInt64(); 

            for (int piece = 0; piece < 7; piece++)
                for(int pos =0;pos<Board.TOTAL_POS;pos++)
            {
                //r.NextBytes(b);
                redZobristKeys[piece,pos] = ran.NextUInt64(); //BitConverter.ToUInt64(b, 0);
                blackZobristKeys[piece, pos] = ran.NextUInt64(); //BitConverter.ToUInt64(b, 0);
            }

            //pieceZobristIndexTable = new Dictionary<Type, int>(TOTAL_PIECE_TYPES);

            ////// populate the hashtables
            //for (int pieceTypeIndex = 0; pieceTypeIndex < TOTAL_PIECE_TYPES; pieceTypeIndex++)
            //{
            //    //pieceZobristIndexTable.Add(pieceTypes[pieceTypeIndex], Board.SquareNo * pieceTypeIndex);
            //}
        }

        /// <summary>
        /// 计算一个盘面的Zobrist散列值，一般说来32位的整数就基本够用了，为了保险起见，用64位的整数
        /// </summary>
        /// <param name="board">盘面</param>
        /// <returns>64位（8个字节）的整数</returns>
        public static ulong ZoristHash(Board board)
        {
            ulong hashValue = 0;

            for (int i = 0; i < 7; i++)
            {
                int num = board.RedPieceNum[i];
                for (int j = 0; j < num; j++)
                {
                    int pos = board.RedPiecePos[i, j];
                    hashValue ^= redZobristKeys[i, pos];
                }

                num = board.BlackPieceNum[i];
                for (int j = 0; j < num; j++)
                {
                    int pos = board.BlackPiecePos[i, j];
                    hashValue ^= blackZobristKeys[i, pos];
                }
            }


            // TODO: 
            // 1-7是红方的7个位棋盘，9-15是黑方的7个位棋盘，第8个位棋盘要忽略掉
            //for (int pieceID = 1; pieceID <= 15; pieceID++)
            //{
            //    if (pieceID == 8) continue;  // 第8个位棋盘表示所有黑子，要忽略掉
            //    BitBoard bb = b[pieceID];
            //    while (bb.IsNotNull())
            //    {
            //        int pos = bb.GetLowestBitWithReset();
            //        hashValue ^= zobristKeys[pieceID * 90 + pos];
            //    }
            //}
            //if (b.WhichTurn != Board.TURN_RED) hashValue ^= zobristKeysBlackTurn;
            return hashValue;
        }



    }
}

