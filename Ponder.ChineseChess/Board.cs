using System;
using System.Text;


/********************************
 * 几个原则：
 * 简单、效率，
 * 方便改为其它语言
 * 方便将来支持更大的棋盘结构的游戏
 * 不用范型、索引器等C#的特殊语法
 * 
 * 
 * Board类比较复杂，用部分类的方式来管理：
 * TODO: 可以安装NDoc或GhostDoc来生成注释！
 * 
 */


namespace Ponder.ChineseChess
{
    /// <summary>
    /// 内部用17*14=238的数组来实现的棋盘类
    /// 棋盘的90个交叉点是按如下方式编号的
    /// 请注意在Board_Const.cs中的常量定义
    /// 9     38,  39,  40,  41,  42,  43,  44,  45,  46, 初始盘面时，这边是黑方阵营
    /// 8     55,  56,  57,  58,  59,  60,  61,  62,  63,
    /// 7     72,  73,  74,  75,  76,  77,  78,  79,  80,
    /// 6     89,  90,  91,  92,  93,  94,  95,  96,  97,
    /// 5    106, 107, 108, 109, 110, 111, 112, 113, 114,
    /// 4    123, 124, 125, 126, 127, 128, 129, 130, 131,
    /// 3    140, 141, 142, 143, 144, 145, 146, 147, 148,
    /// 2    157, 158, 159, 160, 161, 162, 163, 164, 165,
    /// 1    174, 175, 176, 177, 178, 179, 180, 181, 182,
    /// 0    191, 192, 193, 194, 195, 196, 197, 198, 199 初始盘面时，这边是红方阵营
    ///       a    b    c    d    e    f    g    h    i
    ///       0    1    2    3    4    5    6    7    8 
    /// </summary>
    public partial class Board
    {
        /// <summary>
        /// 整个棋盘都采用17x14的一维数组表示法，棋盘左边加4列，右边加4列，上边加2行，下边加2行
        /// </summary>
        public const int NX = 17;  // 9 + 4 + 4
        public const int NY = 14;  // 10 + 2 + 2
        public const int TOTAL_POS = NX * NY;

        /// <summary>
        /// 7种棋子类型，0-6的编号是不可更改的！
        /// 在下面这些数组中，这些序号是与其序号相对应的！
        ///  RedIndex，BlackIndex，RedPieces，BlackPieces，RedPieceList，BlackPieceList;
        /// </summary>
        public const int TYPE_KING = 0;
        public const int TYPE_ROOK = 1;
        public const int TYPE_KNIGHT = 2;
        public const int TYPE_CANNON = 3;
        public const int TYPE_BISHOP = 4;
        public const int TYPE_ADVISOR = 5;
        public const int TYPE_PAWN = 6;

        public const int NONE = 0;
        public const int BORDER = 512;
        public const int RED_PIECE_MASK = 256;
        public const int BLACK_PIECE_MASK = 128;

        /// <summary>
        /// ├───┼───┼───┼───┼───┼───┼───┼───┼────┼───┤    
        /// │   9  │  8   │   7  │  6   │  5   │  4   │  3   │  2   │   1    │   0  │ 
        /// ├───┼───┼───┼───┼───┼───┼───┼───┼────┼───┤    
        /// │Border│ Red  │Black │ King │Rook  │kNight│Cannon│Bishop│Advisor │ Pawn │ 
        /// ├───┼───┼───┼───┼───┼───┼───┼───┼────┼───┤   
        /// </summary>
        public const int RED_KING = RED_PIECE_MASK + (1 << TYPE_KING);
        public const int RED_ROOK = RED_PIECE_MASK + (1 << TYPE_ROOK);
        public const int RED_KNIGHT = RED_PIECE_MASK + (1 << TYPE_KNIGHT);
        public const int RED_CANNON = RED_PIECE_MASK + (1 << TYPE_CANNON);
        public const int RED_BISHOP = RED_PIECE_MASK + (1 << TYPE_BISHOP);
        public const int RED_ADVISOR = RED_PIECE_MASK + (1 << TYPE_ADVISOR);
        public const int RED_PAWN = RED_PIECE_MASK + (1 << TYPE_PAWN);

        public const int BLACK_KING = BLACK_PIECE_MASK + (1 << TYPE_KING);
        public const int BLACK_ROOK = BLACK_PIECE_MASK + (1 << TYPE_ROOK);
        public const int BLACK_KNIGHT = BLACK_PIECE_MASK + (1 << TYPE_KNIGHT);
        public const int BLACK_CANNON = BLACK_PIECE_MASK + (1 << TYPE_CANNON);
        public const int BLACK_BISHOP = BLACK_PIECE_MASK + (1 << TYPE_BISHOP);
        public const int BLACK_ADVISOR = BLACK_PIECE_MASK + (1 << TYPE_ADVISOR);
        public const int BLACK_PAWN = BLACK_PIECE_MASK + (1 << TYPE_PAWN);


        #region 表示棋盘位置的90个常量，A0-I9
        public const int A0 = 191;
        public const int A1 = 174;
        public const int A2 = 157;
        public const int A3 = 140;
        public const int A4 = 123;
        public const int A5 = 106;
        public const int A6 = 89;
        public const int A7 = 72;
        public const int A8 = 55;
        public const int A9 = 38;

        public const int B0 = 192;
        public const int B1 = 175;
        public const int B2 = 158;
        public const int B3 = 141;
        public const int B4 = 124;
        public const int B5 = 107;
        public const int B6 = 90;
        public const int B7 = 73;
        public const int B8 = 56;
        public const int B9 = 39;

        public const int C0 = 193;
        public const int C1 = 176;
        public const int C2 = 159;
        public const int C3 = 142;
        public const int C4 = 125;
        public const int C5 = 108;
        public const int C6 = 91;
        public const int C7 = 74;
        public const int C8 = 57;
        public const int C9 = 40;

        public const int D0 = 194;
        public const int D1 = 177;
        public const int D2 = 160;
        public const int D3 = 143;
        public const int D4 = 126;
        public const int D5 = 109;
        public const int D6 = 92;
        public const int D7 = 75;
        public const int D8 = 58;
        public const int D9 = 41;

        public const int E0 = 195;
        public const int E1 = 178;
        public const int E2 = 161;
        public const int E3 = 144;
        public const int E4 = 127;
        public const int E5 = 110;
        public const int E6 = 93;
        public const int E7 = 76;
        public const int E8 = 59;
        public const int E9 = 42;

        public const int F0 = 196;
        public const int F1 = 179;
        public const int F2 = 162;
        public const int F3 = 145;
        public const int F4 = 128;
        public const int F5 = 111;
        public const int F6 = 94;
        public const int F7 = 77;
        public const int F8 = 60;
        public const int F9 = 43;

        public const int G0 = 197;
        public const int G1 = 180;
        public const int G2 = 163;
        public const int G3 = 146;
        public const int G4 = 129;
        public const int G5 = 112;
        public const int G6 = 95;
        public const int G7 = 78;
        public const int G8 = 61;
        public const int G9 = 44;

        public const int H0 = 198;
        public const int H1 = 181;
        public const int H2 = 164;
        public const int H3 = 147;
        public const int H4 = 130;
        public const int H5 = 113;
        public const int H6 = 96;
        public const int H7 = 79;
        public const int H8 = 62;
        public const int H9 = 45;

        public const int I0 = 199;
        public const int I1 = 182;
        public const int I2 = 165;
        public const int I3 = 148;
        public const int I4 = 131;
        public const int I5 = 114;
        public const int I6 = 97;
        public const int I7 = 80;
        public const int I8 = 63;
        public const int I9 = 46;

        #endregion



        /// <summary>
        /// 内部用17x14表示的棋盘，存储棋子的编号
        /// 为了着法生成，需要public
        /// </summary>
        public int[] pieceData;

        /// <summary>
        /// 记录同类棋子的序号
        /// </summary>
        public int[] RedPieceIndex;
        public int[] BlackPieceIndex;
        public int[] RedPieceNum;
        public int[] BlackPieceNum;
        public int[,] RedPiecePos;
        public int[,] BlackPiecePos;

        // 下面几个是要增量更新的
        public int TotalRedPiece;
        public int TotalBlackPiece;
        // TODO: 随时记着hash值，在makemove时要增量更新它。ZobristHash
        public ulong ZobristKey;


        /// <summary>
        /// 轮到哪方走棋
        /// </summary>
        public bool IsRedTurn;

        /// <summary>
        /// 取得红帅的位置，只能在九宫中
        /// </summary>
        /// <returns>红帅的位置，只能是下列9个数之一：66,67,68,75,76,77,84,85,86</returns>
        public int RedKingPos
        {
            //借用了RedPieces和RedPieceList的表示法，帅只有1个，也就是说
            // RedPieces[TYPE_KING] 永远为1
            // RedPieceList[TYPE_KING,0]就是帅的位置
            get { return RedPiecePos[TYPE_KING, 0]; }
            // set { RedPieceList[TYPE_KING,0] = value; }
        }

        /// <summary>
        ///  取得黑将的位置，只能在九宫中。
        /// </summary>
        /// <returns>黑将的位置，只能是下列9个数之一：3,4,5,12,13,14,21,22,23</returns>

        public int BlackKingPos
        {
            get { return BlackPiecePos[TYPE_KING, 0]; }
            //  set { BlackPieceList[TYPE_KING,0] = value; }
        }

        public int TotalPieceNum
        {
            get { return TotalRedPiece + TotalBlackPiece; }
        }



        /// 构建函数，并把它初始化为象棋开局状态
        /// </summary>
        public Board()
            : this("rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR w")
        {
        }

        public Board(Board b)
        {
            this.pieceData = new int[TOTAL_POS];
            Array.Copy(b.pieceData, this.pieceData, TOTAL_POS);

            RedPieceIndex = new int[TOTAL_POS];
            Array.Copy(b.RedPieceIndex, this.RedPieceIndex, TOTAL_POS);
            BlackPieceIndex = new int[TOTAL_POS];
            Array.Copy(b.BlackPieceIndex, this.BlackPieceIndex, TOTAL_POS);
            RedPieceNum = new int[7];  //7子
            Array.Copy(b.RedPieceNum, this.RedPieceNum, 7);
            BlackPieceNum = new int[7];
            Array.Copy(b.BlackPieceNum, this.BlackPieceNum, 7);
            RedPiecePos = new int[7, 5];  //最多有5个兵
            BlackPiecePos = new int[7, 5];
            for (int i = 0; i < 7; i++)
                for (int j = 0; j < 5; j++)
                {
                    RedPiecePos[i, j] = b.RedPiecePos[i, j];
                    BlackPiecePos[i, j] = b.BlackPiecePos[i, j];
                }
            IsRedTurn = b.IsRedTurn;
            TotalRedPiece = b.TotalRedPiece;
            TotalBlackPiece = b.TotalBlackPiece;
            ZobristKey = b.ZobristKey;
        }

        /// <summary>
        /// 构建函数，根据FEN字符串初始化盘面状态
        /// </summary>
        public Board(string fen)
        {
            pieceData = new int[TOTAL_POS];
            Array.Copy(NONE_BOARD, pieceData, TOTAL_POS);

            RedPieceIndex = new int[TOTAL_POS];
            BlackPieceIndex = new int[TOTAL_POS];
            RedPieceNum = new int[7];  //7子
            BlackPieceNum = new int[7];
            RedPiecePos = new int[7, 5];  //最多有5个兵
            BlackPiecePos = new int[7, 5];

            /*
            RedPieces[IDX_RED_ROOK] = 0;
            RedPieces[IDX_RED_KNIGHT] = 0;
            RedPieces[IDX_RED_CANNON] = 0;
            RedPieces[IDX_RED_BISHOP] = 0;
            RedPieces[IDX_RED_ADVISOR] = 0;
            RedPieces[IDX_RED_PAWN] = 0;

            BlackPieces[IDX_RED_ROOK] = 0;
            BlackPieces[IDX_RED_KNIGHT] = 0;
            BlackPieces[IDX_RED_CANNON] = 0;
            BlackPieces[IDX_RED_BISHOP] = 0;
            BlackPieces[IDX_RED_ADVISOR] = 0;
            BlackPieces[IDX_RED_PAWN] = 0;

            redRookList = new int[2];
            redKnightList = new int[2];
            redCannonList = new int[2];
            redBishopList = new int[2];
            redAdvisorList = new int[2];
            redPawnList = new int[5];

            blackRookList = new int[2];
            blackKnightList = new int[2];
            blackCannonList = new int[2];
            blackBishopList = new int[2];
            blackAdvisorList = new int[2];
            blackPawnList = new int[5];
            */
            Fen.InitFromFEN(this, fen);

            //redExposedCannonPos = new int[2];
            //blackExposedCannonPos = new int[2];
            //redThreatingPos = new int[10];
            //blackThreatingPos = new int[10];

            //UpdateForeRedCannon();
            //UpdateForeBlackCannon();
            //UpdateRedThreating();
            //UpdateBlackThreating();
        }


        /// <summary>
        /// 得到棋子的类型，是7种类型之一，实际对应着从1到7的整数
        /// 通过把它们从0到6编号，可以在编程中减少许多if...else if或switch语句
        /// </summary>
        /// <param name="piece">棋子的ID号：RED_ROOK,RED_KNIGHT,...RED_KING,BLACK_ROOK,BLACK_KNIGHT,...BLACK_KING</param>
        /// <returns>返回TYPE_ROOK,...TYPE_KING这7个整数之一；其它情况都返回-1</returns>
        public static int PieceType(int piece)
        {
            // 提高性能，因为b0,b1,...,b6分别代表不同的棋子，这样只要找到1出现的位置就是棋子的类型
            // 参看TYPE_ROOK和RED_ROOK, BLACK_ROOK等常量的定义即可
            return BitHack64.Log2((ulong)piece & 0x7F);

            //if (piece == RED_ROOK) return TYPE_ROOK;
            //else if (piece == RED_KNIGHT) return TYPE_KNIGHT;
            //else if (piece == RED_CANNON) return TYPE_CANNON;
            //else if (piece == RED_BISHOP) return TYPE_BISHOP;
            //else if (piece == RED_ADVISOR) return TYPE_ADVISOR;
            //else if (piece == RED_PAWN) return TYPE_PAWN;
            //else if (piece == RED_KING) return TYPE_KING;

            //else if (piece == BLACK_ROOK) return TYPE_ROOK;
            //else if (piece == BLACK_KNIGHT) return TYPE_KNIGHT;
            //else if (piece == BLACK_CANNON) return TYPE_CANNON;
            //else if (piece == BLACK_BISHOP) return TYPE_BISHOP;
            //else if (piece == BLACK_ADVISOR) return TYPE_ADVISOR;
            //else if (piece == BLACK_PAWN) return TYPE_PAWN;
            //else if (piece == BLACK_KING) return TYPE_KING;
            //else return -1;
        }







        /// <summary>
        /// 改成由对方走棋
        /// </summary>
        public void ChangeTurn()
        {
            IsRedTurn = !IsRedTurn;
        }


        public static bool IsRedPiece(int piece)
        {
            return ((piece & RED_PIECE_MASK) != 0);
        }
        public static bool IsBlackPiece(int piece)
        {
            return ((piece & BLACK_PIECE_MASK) != 0);
        }

        ///// <summary>
        ///// 如果是黑子或空白，则返回true
        ///// </summary>
        ///// <param name="piece">棋子编号</param>
        ///// <returns></returns>
        //public static bool IsBlackOrNone(int piece)
        //{
        //    return ((piece & (BORDER | RED_PIECE_MASK)) == 0);
        //    // 与下面这句是一个意思
        //    //return piece == NONE || IsBlackPiece(piece);
        //}

        /// <summary>
        /// 如果是红子或空白，则返回true
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        //public static bool IsRedOrNone(int piece)
        //{
        //    return ((piece & (BORDER | BLACK_PIECE_MASK)) == 0);
        //    // 与下面这句是一个意思
        //    //return piece == NONE || IsRedPiece(piece);
        //}



        ///// <summary>
        ///// 判断在某个位置上是不是空着，就是没有棋子
        ///// </summary>
        ///// <param name="pos">位置</param>
        ///// <returns>空着返回true，否则返回false</returns>
        //public bool IsNullAtPos(int pos)
        //{
        //    return pieceData[pos] == NONE;
        //}


        public Move CreateMove(int from, int to)
        {
            // 要把吃的棋子记下来，在UnmakeMove的时候要恢复回来
            return new Move(from, to, pieceData[from], pieceData[to]);
        }

        private int CountPiece(int from, int to)
        {
            int n = 0;
            int inc = IncSliding(from, to);
            if (inc == 0) return 0;
            for (int pos = from + inc; pieceData[pos] == NONE; pos += inc)
                ++n;
            return n;
        }

        static char[] boardChar = 
        {
                 '┏','┯','┯','┯','┯','┯','┯','┯','┓',
                 '┠','┼','┼','┼','╳','┼','┼','┼','┨',
                 '┠','╬','┼','┼','┼','┼','┼','╬','┨',
                 '╠','┼','╬','┼','╬','┼','╬','┼','╣',
                 '┠','┴','┴','┴','┴','┴','┴','┴','┨',
                 '┠','┬','┬','┬','┬','┬','┬','┬','┨',
                 '╠','┼','╬','┼','╬','┼','╬','┼','╣',
                 '┠','╬','┼','┼','┼','┼','┼','╬','┨',
                 '┠','┼','┼','┼','╳','┼','┼','┼','┨',
                 '┗','┷','┷','┷','┷','┷','┷','┷','┛'
        };

        /// <summary>
        /// 为方便调试，用一个简洁的方式来显示棋盘情况
        /// TODO: 不知道是否影响性能
        /// </summary>
        /// <returns>
        /// 盘面字符串
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    int piece = pieceData[BoardUtil.Pos(i, j)];

                    if (piece != 0)
                    {
                        sb.Append(BoardUtil.PieceChineseNameForPrint(piece));
                    }
                    else
                    {
                        sb.Append(boardChar[j * 9 + i]);
                    }
                }
                sb.Append(System.Environment.NewLine);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 检查盘面中pieceData与pieceList是不是记录的信息相一致
        /// </summary>
        /// <param name="board">盘面</param>
        /// <returns>盘面信息正确时返回true</returns>
        public static bool CheckBoard(Board board)
        {
            // TODO: 代码可以精练了，把TYPE_ROOK和RED_ROOK对应起来
            if (board.pieceData[board.RedKingPos] != RED_KING) return false;

            for (int i = 0; i < board.RedPieceNum[TYPE_ROOK]; i++)
            {
                int pos = board.RedPiecePos[TYPE_ROOK, i];
                if (board.pieceData[pos] != RED_ROOK) return false;
            }
            for (int i = 0; i < board.RedPieceNum[TYPE_KNIGHT]; i++)
            {
                int pos = board.RedPiecePos[TYPE_KNIGHT, i];
                if (board.pieceData[pos] != RED_KNIGHT) return false;
            }
            for (int i = 0; i < board.RedPieceNum[TYPE_CANNON]; i++)
            {
                int pos = board.RedPiecePos[TYPE_CANNON, i];
                if (board.pieceData[pos] != RED_CANNON) return false;
            }
            for (int i = 0; i < board.RedPieceNum[TYPE_BISHOP]; i++)
            {
                int pos = board.RedPiecePos[TYPE_BISHOP, i];
                if (board.pieceData[pos] != RED_BISHOP) return false;
            }
            for (int i = 0; i < board.RedPieceNum[TYPE_ADVISOR]; i++)
            {
                int pos = board.RedPiecePos[TYPE_ADVISOR, i];
                if (board.pieceData[pos] != RED_ADVISOR) return false;
            }
            for (int i = 0; i < board.RedPieceNum[TYPE_PAWN]; i++)
            {
                int pos = board.RedPiecePos[TYPE_PAWN, i];
                if (board.pieceData[pos] != RED_PAWN) return false;
            }



            if (board.pieceData[board.BlackKingPos] != BLACK_KING) return false;

            for (int i = 0; i < board.BlackPieceNum[TYPE_ROOK]; i++)
            {
                int pos = board.BlackPiecePos[TYPE_ROOK, i];
                if (board.pieceData[pos] != BLACK_ROOK) return false;
            }
            for (int i = 0; i < board.BlackPieceNum[TYPE_KNIGHT]; i++)
            {
                int pos = board.BlackPiecePos[TYPE_KNIGHT, i];
                if (board.pieceData[pos] != BLACK_KNIGHT) return false;
            }
            for (int i = 0; i < board.BlackPieceNum[TYPE_CANNON]; i++)
            {
                int pos = board.BlackPiecePos[TYPE_CANNON, i];
                if (board.pieceData[pos] != BLACK_CANNON) return false;
            }
            for (int i = 0; i < board.BlackPieceNum[TYPE_BISHOP]; i++)
            {
                int pos = board.BlackPiecePos[TYPE_BISHOP, i];
                if (board.pieceData[pos] != BLACK_BISHOP) return false;
            }
            for (int i = 0; i < board.BlackPieceNum[TYPE_ADVISOR]; i++)
            {
                int pos = board.BlackPiecePos[TYPE_ADVISOR, i];
                if (board.pieceData[pos] != BLACK_ADVISOR) return false;
            }
            for (int i = 0; i < board.BlackPieceNum[TYPE_PAWN]; i++)
            {
                int pos = board.BlackPiecePos[TYPE_PAWN, i];
                if (board.pieceData[pos] != BLACK_PAWN) return false;
            }

            int countRed = 0;
            int countBlack = 0;
            for (int i = 0; i < board.pieceData.Length; i++)
            {
                int piece = board.pieceData[i];
                if (IsRedPiece(piece)) countRed++;
                if (IsBlackPiece(piece)) countBlack++;
            }
            if (board.TotalRedPiece != countRed) return false;
            if (board.TotalBlackPiece != countBlack) return false;
            if (board.TotalPieceNum != board.TotalRedPiece + board.TotalBlackPiece) return false;

            return true;
        }

    }


}
