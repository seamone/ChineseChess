using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Ponder.ChineseChess
{
    /// <summary>
    /// 与着法生成有关的内容
    /// </summary>
    public partial class MoveGenerator
    {
        /// <summary>
        /// 表示4个方向的增量
        /// </summary>
        private static readonly int[] INC4 = { -17, -1, +1, +17 };

        private readonly Board board;

        static MoveGenerator()
        {
            // 着法生成有关的预置表都要先初始化好
            InitAllPreset();
        }


        public MoveGenerator(Board b)
        {
           // board = new Board(b);
            board = b;
        }

        public void GenRedPawnMovelist(int from, Move[] movelist, ref int countMove)
        {
            //为了着法生成的效率，不进行容错性的判断
            foreach (int to in MoveGenerator.PRESET_RED_PAWN[from])
            {
                int capture = board.pieceData[to];
                if (capture == Board.NONE)
                    AddRedMove(from, to, Board.RED_PAWN, movelist, ref countMove);
                else if (Board.IsBlackPiece(capture))
                    AddRedCaptureMove(from, to, Board.RED_PAWN, capture, movelist, ref countMove);
            }
        }

        public void GenBlackPawnMovelist(int from, Move[] movelist, ref int countMove)
        {
            //为了着法生成的效率，不进行容错性的判断
            foreach (int to in MoveGenerator.PRESET_BLACK_PAWN[from])
            {
                int capture = board.pieceData[to];
                if (capture == Board.NONE)
                    AddBlackMove(from, to, Board.BLACK_PAWN, movelist, ref countMove);
                else if (Board.IsRedPiece(capture))
                    AddBlackCaptureMove(from, to, Board.BLACK_PAWN, capture,movelist, ref countMove);
            }
        }


        public void GenRedCannonMovelist(int from, Move[] movelist, ref int countMove)
        {
            foreach (int inc in INC4)
            {
                int to;
                // 平移着法
                for (to = from + inc; board.pieceData[to] == Board.NONE; to += inc)
                    AddRedMove(from, to, Board.RED_CANNON, movelist, ref countMove);

                // 吃子着法
                to = FindNextPos(to + inc, inc);
                int capture = board.pieceData[to];
                if (Board.IsBlackPiece(capture)) 
                    AddRedCaptureMove(from, to, Board.RED_CANNON,capture, movelist, ref countMove);
            }
        }

        public void GenBlackCannonMovelist(int from, Move[] movelist, ref int countMove)
        {
            //为了着法生成的效率，不进行容错性的判断
            foreach (int inc in INC4)
            {
                int to;
                for (to = from + inc; board.pieceData[to] == Board.NONE; to += inc)
                    AddBlackMove(from, to, Board.BLACK_CANNON, movelist, ref countMove);

                // 吃子着法
                to = FindNextPos(to + inc, inc);
                int capture = board.pieceData[to];
                if (Board.IsRedPiece(capture)) 
                    AddBlackCaptureMove(from, to, Board.BLACK_CANNON, capture,movelist, ref countMove);
            }
        }

        /// <summary>
        /// 红车的着法生成
        /// </summary>
        /// <param name="from">红车出发的位置</param>
        /// <param name="movelist"></param>
        /// <param name="countMove"></param>
        public void GenRedRookMovelist(int from, Move[] movelist, ref int countMove)
        {
            foreach (int inc in INC4)
            {
                int to;
                // 平移着法
                for (to = from + inc; board.pieceData[to] == Board.NONE; to += inc)
                    AddRedMove(from, to, Board.RED_ROOK, movelist, ref countMove);
                // 吃子着法
                int capture = board.pieceData[to];
                if (Board.IsBlackPiece(capture))
                    AddRedCaptureMove(from, to, Board.RED_ROOK, capture, movelist, ref countMove);
            }
        }

        public void GenBlackRookMovelist(int from, Move[] movelist, ref int countMove)
        {

            //为了着法生成的效率，不进行容错性的判断
            foreach (int inc in INC4)
            {
                int to;
                for (to = from + inc; board.pieceData[to] == Board.NONE; to += inc)
                    AddBlackMove(from, to, Board.BLACK_ROOK, movelist, ref countMove);
                int capture = board.pieceData[to];
                if (Board.IsRedPiece(capture)) // 吃子着法
                    AddBlackCaptureMove(from, to, Board.BLACK_ROOK,capture, movelist, ref countMove);
            }
        }


        /// <summary>
        /// 红帅着法生成
        /// </summary>
        public void GenRedKingMovelist(int from, Move[] movelist, ref int countMove)
        {
            //为了着法生成的效率，不进行容错性的判断
            foreach (int to in MoveGenerator.PRESET_KING[from])
            {
                int capture = board.pieceData[to];
                if (capture == Board.NONE)
                    AddRedMove(from, to, Board.RED_KING, movelist, ref countMove);
                else if (Board.IsBlackPiece(capture))
                    AddRedCaptureMove(from, to, Board.RED_KING,capture, movelist, ref countMove);

            }
        }

        /// <summary>
        /// 这里要判断着法是
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="piece"></param>
        /// <param name="movelist"></param>
        /// <param name="countMove"></param>
        private void AddRedMove(int from, int to, int piece, Move[] movelist, ref int countMove)
        {
            Move m = new Move(from, to, piece, 0);
            if (IsLegalRedMove(m))
                movelist[countMove++] = m;
        }

        private void AddRedCaptureMove(int from, int to, int piece, int capture, Move[] movelist, ref int countMove)
        {
            Move m = new Move(from, to, piece, capture);
            if (IsLegalRedMove(m))
                movelist[countMove++] = m;
        }


        /// <summary>
        /// 着法生成
        /// </summary>
        public void GenBlackKingMovelist(int from, Move[] movelist, ref int countMove)
        {
            //为了着法生成的效率，不进行容错性的判断
            foreach (int to in MoveGenerator.PRESET_KING[from])
            {
                int capture = board.pieceData[to];
                if (capture == Board.NONE)
                    AddBlackMove(from, to, Board.BLACK_KING, movelist, ref countMove);
                else if (Board.IsRedPiece(capture))
                    AddBlackCaptureMove(from, to, Board.BLACK_KING,capture, movelist, ref countMove);
            }
        }

        private void AddBlackMove(int from, int to, int piece, Move[] list, ref int countMove)
        {
            Move move = new Move(from, to, piece, 0);
            if (IsLegalBlackMove(move))
                list[countMove++] = move;
        }

        private void AddBlackCaptureMove(int from, int to, int piece, int capture, Move[] list, ref int countMove)
        {
            Move move = new Move(from, to, piece, capture);
            if (IsLegalBlackMove(move))
                list[countMove++] = move;
        }

        public void GenRedKnightMovelist(int from, Move[] movelist, ref int countMove)
        {
            foreach (int to in MoveGenerator.PRESET_KNIGHT[from])
            {
                int blockPos = from + IncBlockPos(to - from);  //蹩马腿的位置

                if (board.pieceData[blockPos] == Board.NONE)
                {
                    int capture = board.pieceData[to];
                    if (capture == Board.NONE)
                        AddRedMove(from, to, Board.RED_KNIGHT, movelist, ref countMove);
                    else if (Board.IsBlackPiece(capture))
                        AddRedCaptureMove(from, to, Board.RED_KNIGHT, capture, movelist, ref countMove);
                }
            }
        }

        public void GenBlackKnightMovelist(int from, Move[] movelist, ref int countMove)
        {
            foreach (int to in MoveGenerator.PRESET_KNIGHT[from])
            {
                int blockPos = from + IncBlockPos(to - from);  //蹩马腿的位置
                if (board.pieceData[blockPos] == Board.NONE)
                {
                    int capture = board.pieceData[to];
                    if (capture == Board.NONE)
                        AddBlackMove(from, to, Board.BLACK_KNIGHT, movelist, ref countMove);
                    else if (Board.IsRedPiece(capture))
                        AddBlackCaptureMove(from, to, Board.BLACK_KNIGHT, capture, movelist, ref countMove);
                }
            }
        }



        // 马可能移动的八个方向，排列顺序如下，*表示蹩马腿的位置
        //  .    -35     .    -33    .
        //	-19   .   *(-17)   .    -15
        //	.   *(-1)   马   *(+1)   .
        //	+15   .   *(+17)   .    +19
        //	.    +33     .    +35    .
        private static int IncBlockPos(int diff)
        {
            switch (diff)
            {
                case -35:
                case -33:
                    return -17;
                case -15:
                case +19:
                    return +1;
                case +33:
                case +35:
                    return +17;
                case +15:
                case -19:
                    return -1;
                default:  //不应该执行到这个地方
                    return 0;
            }
        }



        /// <summary>
        /// 红相着法生成
        /// </summary>
        public void GenRedBishopMovelist(int from, Move[] movelist, ref int countMove)
        {
            foreach (int to in MoveGenerator.PRESET_BISHOP_ADVISOR[from])
            {
                int blockPos = (to + from) / 2;  //相眼的位置
                if (board.pieceData[blockPos] == Board.NONE)
                {
                    int capture = board.pieceData[to];
                    if (capture == Board.NONE)
                        AddRedMove(from, to, Board.RED_BISHOP, movelist, ref countMove);
                    else if (Board.IsBlackPiece(capture))
                        AddRedCaptureMove(from, to, Board.RED_BISHOP,capture, movelist, ref countMove);

                }
            }
        }

        public void GenBlackBishopMovelist(int from, Move[] movelist, ref int countMove)
        {
            foreach (int to in MoveGenerator.PRESET_BISHOP_ADVISOR[from])
            {
                int blockPos = (to + from) / 2;  //相眼的位置
                if (board.pieceData[blockPos] == Board.NONE)
                {
                    int capture = board.pieceData[to];
                    if (capture == Board.NONE)
                        AddBlackMove(from, to, Board.BLACK_BISHOP, movelist, ref countMove);
                    else if(Board.IsRedPiece(capture))
                        AddBlackCaptureMove(from, to, Board.BLACK_BISHOP,capture, movelist, ref countMove);
                }
            }
        }

        public void GenRedAdvisorMovelist(int from, Move[] movelist, ref int countMove)
        {
            foreach (int to in MoveGenerator.PRESET_BISHOP_ADVISOR[from])
            {
                int capture = board.pieceData[to];
                if (capture == Board.NONE)
                    AddRedMove(from, to, Board.RED_ADVISOR, movelist, ref countMove);
                else if (Board.IsBlackPiece(capture))
                    AddRedCaptureMove(from, to, Board.RED_ADVISOR, capture, movelist, ref countMove);
                    
            }
        }

        public void GenBlackAdvisorMovelist(int from, Move[] movelist, ref int countMove)
        {
            foreach (int to in MoveGenerator.PRESET_BISHOP_ADVISOR[from])
            {
                int capture = board.pieceData[to];
                if (capture == Board.NONE)
                    AddBlackMove(from, to, Board.BLACK_ADVISOR, movelist, ref countMove);
                else if (Board.IsRedPiece(capture))
                    AddBlackCaptureMove(from, to, Board.BLACK_ADVISOR,capture, movelist, ref countMove);
            }
        }




        /// <summary>
        /// 根据当前轮到谁走棋，来生成所有的着法
        /// </summary>
        /// <param name="movelist"></param>
        /// <returns>返回着法的个数</returns>
        public static int GenAllMoveList(Board board, Move[] movelist)
        {
            MoveGenerator mg = new MoveGenerator(board);

            int countMove = 0;
            if (board.IsRedTurn)
            {
                mg.GenAllRedMovelist(movelist, ref countMove);
            }
            else
            {
                mg.GenAllBlackMovelist(movelist, ref countMove);
            }
            return countMove;
        }

        /// <summary>
        /// 生成红方所有着法列表
        /// </summary>
        /// <param name="movelist"></param>
        /// <param name="countMove"></param>
        private void GenAllRedMovelist(Move[] movelist, ref int countMove)
        {
            for (int i = 0; i < board.RedPieceNum[Board.TYPE_ROOK]; i++)
                GenRedRookMovelist(board.RedPiecePos[Board.TYPE_ROOK, i], movelist, ref countMove);
            for (int i = 0; i < board.RedPieceNum[Board.TYPE_KNIGHT]; i++)
                GenRedKnightMovelist(board.RedPiecePos[Board.TYPE_KNIGHT, i], movelist, ref countMove);
            for (int i = 0; i < board.RedPieceNum[Board.TYPE_CANNON]; i++)
                GenRedCannonMovelist(board.RedPiecePos[Board.TYPE_CANNON, i], movelist, ref countMove);
            for (int i = 0; i < board.RedPieceNum[Board.TYPE_BISHOP]; i++)
                GenRedBishopMovelist(board.RedPiecePos[Board.TYPE_BISHOP, i], movelist, ref countMove);
            for (int i = 0; i < board.RedPieceNum[Board.TYPE_ADVISOR]; i++)
                GenRedAdvisorMovelist(board.RedPiecePos[Board.TYPE_ADVISOR, i], movelist, ref countMove);
            for (int i = 0; i < board.RedPieceNum[Board.TYPE_PAWN]; i++)
                GenRedPawnMovelist(board.RedPiecePos[Board.TYPE_PAWN, i], movelist, ref countMove);

            GenRedKingMovelist(board.RedKingPos, movelist, ref countMove);
        }

        /// <summary>
        /// 生成黑方所有着法列表
        /// </summary>
        /// <param name="movelist"></param>
        /// <param name="countMove"></param>
        private void GenAllBlackMovelist(Move[] movelist, ref int countMove)
        {
            for (int i = 0; i < board.BlackPieceNum[Board.TYPE_ROOK]; i++)
                GenBlackRookMovelist(board.BlackPiecePos[Board.TYPE_ROOK, i], movelist, ref countMove);
            for (int i = 0; i < board.BlackPieceNum[Board.TYPE_KNIGHT]; i++)
                GenBlackKnightMovelist(board.BlackPiecePos[Board.TYPE_KNIGHT, i], movelist, ref countMove);
            for (int i = 0; i < board.BlackPieceNum[Board.TYPE_CANNON]; i++)
                GenBlackCannonMovelist(board.BlackPiecePos[Board.TYPE_CANNON, i], movelist, ref countMove);
            for (int i = 0; i < board.BlackPieceNum[Board.TYPE_BISHOP]; i++)
                GenBlackBishopMovelist(board.BlackPiecePos[Board.TYPE_BISHOP, i], movelist, ref countMove);
            for (int i = 0; i < board.BlackPieceNum[Board.TYPE_ADVISOR]; i++)
                GenBlackAdvisorMovelist(board.BlackPiecePos[Board.TYPE_ADVISOR, i], movelist, ref countMove);
            for (int i = 0; i < board.BlackPieceNum[Board.TYPE_PAWN]; i++)
                GenBlackPawnMovelist(board.BlackPiecePos[Board.TYPE_PAWN, i], movelist, ref countMove);

            GenBlackKingMovelist(board.BlackKingPos, movelist, ref countMove);
        }

        /// <summary>
        /// TODO: 还有问题，增量式检测，以后有时间再弄
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool IsLegalRedKingMove(Move m)
        {
            int to = m.To;
            if (board.pieceData[to - 1] == Board.BLACK_PAWN
                || board.pieceData[to + 1] == Board.BLACK_PAWN
                || board.pieceData[to - 17] == Board.BLACK_PAWN) return false; //被邻近的黑卒攻击
            // TODO: 此时帅还没有移动到to位置上，有些判断是错误，并且还可以优化！
            foreach (int inc in INC4)
            {
                int numBlock = 0;
                for (int pos = to + inc; board.pieceData[pos] != Board.BORDER; pos += inc)
                {
                    if (numBlock == 0 && board.pieceData[pos] == Board.BLACK_ROOK) return false; //被黑车攻击
                    if (numBlock == 0 && board.pieceData[pos] == Board.BLACK_KING) return false; //与黑将对脸！
                    if (numBlock == 1 && board.pieceData[pos] == Board.BLACK_CANNON) return false; //被黑炮攻击
                    ++numBlock;  // 下面只有可能被炮攻击了
                    if (numBlock >= 2) break;  //一条线有2个棋子，帅肯定安全了}
                }
            }

            // 通过了全部检测，说明红帅是安全的
            return true;
        }

        /// <summary>
        /// 判断位置是否被红兵攻击
        /// </summary>
        /// <param name="pos">检测的位置</param>
        /// <returns>被攻击返回true，否则返回false</returns>
        public bool IsAttackedByRedPawn(int pos)
        {
            if (board.pieceData[pos + 17] == Board.RED_PAWN) return true;
            if (Board.IsInBlackSide(pos))  //过河
            {
                if (board.pieceData[pos - 1] == Board.RED_PAWN) return true;
                if (board.pieceData[pos + 1] == Board.RED_PAWN) return true;
            }
            return false;
        }


        /// <summary>
        /// 判断位置是否被黑卒攻击
        /// </summary>
        /// <param name="pos">检测的位置</param>
        /// <returns>被攻击返回true，否则返回false</returns>
        public bool IsAttackedByBlackPawn(int pos)
        {
            if (board.pieceData[pos - 17] == Board.BLACK_PAWN) return true;
            if (Board.IsInRedSide(pos))  //过河
            {
                if (board.pieceData[pos - 1] == Board.BLACK_PAWN) return true;
                if (board.pieceData[pos + 1] == Board.BLACK_PAWN) return true;
            }
            return false;
        }

        /// <summary>
        /// 检测红帅是否安全，即没有被将军
        /// </summary>
        /// <param name="move">着法</param>
        /// <returns>红帅安全返回true，否则返回false</returns>
        public bool IsRedKingSafe()
        {
            // 被黑马攻击
            if (IsAttackedByBlackKnight(board.RedKingPos)) return false;

            //被邻近的黑卒攻击
            if (IsAttackedByBlackPawn(board.RedKingPos)) return false;

            // 检测四个方向上的安全性，主要是车、炮的攻击
            foreach (int inc in INC4)
            {
                int pos = FindNextPos(board.RedKingPos + inc, inc);
                if (board.pieceData[pos] == Board.BLACK_ROOK) return false; //被黑车攻击
                if (inc == -17 && board.pieceData[pos] == Board.BLACK_KING) return false; //与黑将对脸！
                    
                pos = FindNextPos(pos + inc, inc);
                if (board.pieceData[pos] == Board.BLACK_CANNON) return false; //被黑炮攻击
            }

            // 通过了全部检测，说明红帅是安全的
            return true;
        }

        public bool IsBlackKingSafe()
        {
            // 被马攻击
            if (IsAttackedByRedKnight(board.BlackKingPos)) return false;

            //被邻近的红兵攻击
            if (IsAttackedByRedPawn(board.BlackKingPos)) return false;

            // 检测四个方向上的安全性，主要是车、炮的攻击
            foreach (int inc in INC4)
            {
                int pos = FindNextPos(board.BlackKingPos + inc, inc);
                if (board.pieceData[pos] == Board.RED_ROOK) return false; //被车攻击
                if (inc == 17 && board.pieceData[pos] == Board.RED_KING) return false; //与帅对脸！

                pos = FindNextPos(pos + inc, inc);
                if (board.pieceData[pos] == Board.RED_CANNON) return false; //被红炮攻击
            }

            // 通过了全部检测，说明黑将是安全的
            return true;
        }

        
        public bool IsLegalRedMove(Move move)
        {
            board.MakeMove(move);
            bool safe = IsRedKingSafe();
            board.UnmakeMove(move);
            return safe;
        }

        public bool IsLegalBlackMove(Move move)
        {
            board.MakeMove(move);
            bool safe = IsBlackKingSafe();
            board.UnmakeMove(move);
            return safe;
        }

        /// <summary>
        /// 增量检测
        /// 注意一个前提，就是在检测前红帅是处于安全状态的
        /// 着法是不是合法，所谓合法，就是没有被黑方将军
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public bool IsLegalRedMoveError(Move move)
        {
            // 是帅移动的着法，需要单独处理
            int kingPos = board.RedKingPos;
            int from = move.From;
            if (from == kingPos)
            {
                return IsLegalRedKingMove(move);
            }

            int to = move.To;

            int inc = board.IncSliding(kingPos, from);
            // 如果from与帅不在一条直线上，即处于这些*位置上时
            //   *          |       
            //       *      |      *
            //              |   *      
            //     *        |       
            //  ------------K--------------
            //        *     |     *
            //    *         | *
            if (inc == 0)
            {
                // 如果from以前是蹩马腿的位置，则要判断被黑马攻击的可能性
                //           n|n          
                //          n*|*n       
                //  ----------K---------
                //          n*|*n      
                //           n|n  
                if (IsCheckedByKnightWhenLeave(kingPos, from)) return false;

                //判断炮架子的情况
                // 如果一个子移动到帅和黑方的空头炮之间，则会被将军
                //              |    (from)
                //              |      |    
                //              |      | 
                //  ------------K----(to)---黑炮---
                if (IsCheckedByBlackCannon(to)) return false;

                // 一个着法，如果不与帅在一条线上，又没有买挪走蹩马腿的棋子
                // 并走到黑方空头炮之间当炮架子，则是安全的
                return true;
            }

            //////////////////////////////////////
            // 以下都是from与帅在一条直线上的情况
            //         |       
            //         |       
            //  -------K--(from)-----
            //         |      
            //         |  

            // 下面是这种情况：某子与帅相同的直线上移动
            //  帅 ->  from -> to  ->  除了红炮打出去的情况外，其它都是安全的
            //  帅 -> to <- from ->  这种情况永远是安全的
            int incTo = board.IncSliding(kingPos, to);
            if (inc == incTo)
            {
                // 如果只是平移，没有吃子，则是安全的
                // K ----- from ----- to ----------
                // K ------to ------ from ----- 
                if (move.Capture == Board.NONE) return true;

                // 如果产生了吃子，可能还会出现这些情况：
                //  帅 -> 红车(或红兵)from ->黑子(to) ->黑炮 
                //  帅 ->黑子(to) -> 红车(或红兵)from  ->黑炮 
                //  帅 -> 只有一个其它子 -> 红炮from ->黑炮 -> 黑子to
                //  帅 -> 红炮from ->黑车(或黑将） -> 黑子to
                // 在这里不会出现这种情况：to <- 帅 -> 红炮from -> 只有一个其它子 -> 黑炮 
                // 因为to的方向与from的方向不一样
                if (move.Piece == Board.RED_CANNON)
                {
                    //  帅 -> 只有一个其它子 -> 红炮from ->黑炮 -> 黑子to
                    //  帅 -> 红炮from ->黑车(或黑将） -> 黑子to
                    // 下面这种情况也是安全的
                    //  帅 -> 其它子 -> 其它子 -> 红炮from ->黑炮 -> 黑子to
                    //         pos1
                    if (CanRedPieceJumpOut(kingPos, inc, from, to)) return true;
                    return false;
                }
                else if (move.Piece == Board.RED_ROOK || move.Piece == Board.RED_PAWN)
                {
                    // TODO: 兵正好吃掉黑炮时有错
                    // TODO: 后面有2个炮是有错！
                    //  帅 -> 红车(或红兵)from ->黑子(to) ->黑炮 
                    //  帅 ->黑子(to) -> 红车(或红兵)from  ->黑炮 
                    if (FindForward(kingPos, inc, from, to, Board.BLACK_CANNON)) return false;
                }
                return true;
            }


            // 先检查to到达的位置是不是被黑炮将军
            // 如果有吃子情况，那么红帅会更安全，所以只需要检测没有吃子的着法
            // 检测to的位置是不是走到了黑方空头炮的中间
            if (move.Capture == Board.NONE)
            {
                if (IsCheckedByBlackCannon(to)) return false;
            }

            //          to     to <-- 
            //           |      |    \ 
            //           |      |     \
            //  帅 ->  from    帅 ->  from 
            // 
            //  to -> 帅 -> from 
            // 上面三种情况，只需检测from如果走开后，是否会被黑车或黑炮将军，甚至对脸！

            if (CanRedPieceJumpOut(kingPos, inc, from, to)) return true;
            return false;
        }

        /// <summary>
        /// 从from位置查找，每次增量为inc，一直找到一个不为空的棋子位置
        /// 注意：可能会找到边界位置
        /// </summary>
        /// <param name="from">出发位置</param>
        /// <param name="inc">位置编号的增量</param>
        /// <returns>一个位置编号，该位置上或者是边界，或者是一枚棋子</returns>
        private int FindNextPos(int from, int inc)
        {
            while (board.pieceData[from] == Board.NONE)
                from += inc;
            return from;
        }

        /// <summary>
        /// 判断to位置是否位于黑方的空头炮中
        /// 如果有棋子移动到to位置，可能正好给黑方的空头炮当炮架子
        /// 帅 -> to -> 黑炮 
        /// </summary>
        /// <param name="to">准备到达的位置</param>
        /// <returns>被空头炮将军时返回true，否则返回false</returns>
        public bool IsCheckedByBlackCannon(int to)
        {
            // 检查to与帅是不是在一条线上
            int inc = board.IncSliding(board.RedKingPos, to);
            if (inc == 0) return false;

            int pos = FindNextPos(board.RedKingPos + inc, inc);
            if (board.pieceData[pos] == Board.BLACK_CANNON)
            {
                // 这种情况不算：帅 ->  黑炮 -> to 
                if (inc == board.IncSliding(to, pos))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 是否被红马攻击
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private bool IsAttackedByRedKnight(int pos)
        {
            return IsAttackedByKnight(pos, Board.RED_KNIGHT);
        }
        /// <summary>
        /// 是否被黑马攻击
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private bool IsAttackedByBlackKnight(int pos)
        {
            return IsAttackedByKnight(pos, Board.BLACK_KNIGHT);
        }

        /// <summary>
        /// 判断pos这个位置是否受到马的攻击
        /// </summary>
        /// <param name="pos">检测的位置</param>
        /// <returns>被攻击时返回true，安全返回false</returns>
        private bool IsAttackedByKnight(int pos, int knight)
        {
            // 在*处的8个位置只要有一个有“马”，则pos位置会被攻击
            // .    *     .     *     .
            // *  b(-18)  .   b(-16)  *     如果在b这些位置没子，则会被*位置上的马攻击
            // .    .   pos     .     .
            // *  b(16)   .   b(+18)  *
            // .    *     .     *     .
            if (board.pieceData[pos - 18] == Board.NONE)
            {
                if (board.pieceData[pos - 35] == knight) return true;
                if (board.pieceData[pos - 19] == knight) return true;
            }

            if (board.pieceData[pos - 16] == Board.NONE)
            {
                if (board.pieceData[pos - 33] == knight) return true;
                if (board.pieceData[pos - 15] == knight) return true;
            }

            if (board.pieceData[pos + 16] == Board.NONE)
            {
                if (board.pieceData[pos + 15] == knight) return true;
                if (board.pieceData[pos + 33] == knight) return true;
            }

            if (board.pieceData[pos + 18] == Board.NONE)
            {
                if (board.pieceData[pos + 19] == knight) return true;
                if (board.pieceData[pos + 35] == knight) return true;
            }

            // 8个位置都没有受到攻击
            return false;
        }

        /// <summary>
        /// 判断一个子如果从from离开时，是否会被黑马将军
        /// </summary>
        /// <param name="kingPos">红帅的位置</param>
        /// <param name="from">一个棋子要从这里离开</param>
        /// <returns></returns>
        private bool IsCheckedByKnightWhenLeave(int kingPos, int from)
        {
            // 这个from位置必须是蹩着马腿的地方
            // 如果当前的子从from处离开，并且*处是黑马，则会被黑马将军
            //      *                  *
            //*  from(-18)    .     from(-16)  *     
            //      .      kingPos     .
            //*  from(16)     .     from(+18)  *
            //      *                  *
            int incBlocker = board.IncBlockKnightTo(kingPos, from);
            if (incBlocker != 0)
            {
                switch (incBlocker)
                {
                    case -18:
                        if (board.pieceData[kingPos - 35] == Board.BLACK_KNIGHT) return true;
                        if (board.pieceData[kingPos - 19] == Board.BLACK_KNIGHT) return true;
                        break;
                    case -16:
                        if (board.pieceData[kingPos - 33] == Board.BLACK_KNIGHT) return true;
                        if (board.pieceData[kingPos - 15] == Board.BLACK_KNIGHT) return true;
                        break;
                    case 16:
                        if (board.pieceData[kingPos + 15] == Board.BLACK_KNIGHT) return true;
                        if (board.pieceData[kingPos + 33] == Board.BLACK_KNIGHT) return true;
                        break;
                    case 18:
                        if (board.pieceData[kingPos + 19] == Board.BLACK_KNIGHT) return true;
                        if (board.pieceData[kingPos + 35] == Board.BLACK_KNIGHT) return true;
                        break;
                }
            }
            return false;

        }

        /// <summary>
        /// 判断是否红子是否可以跳走(这里以红炮为例)
        /// 下面这2种情况会被将军或对脸：
        /// 帅 -> 只有一个其它子 -> 红炮from ->黑炮 -> 黑子to
        /// 帅 -> 红炮from ->黑车(或黑将） -> 黑子to
        // 而下面这种情况则是安全的：
        //  帅 -> 其它子 -> 其它子 -> 红炮from ->黑炮 -> 黑子to
        //  帅 -> 其它子 -> 红炮from ->黑车（或黑将） -> 黑子to
        /// </summary>
        /// <param name="kingPos">红帅的位置</param>
        /// <param name="inc">方向增量</param>
        /// <param name="from">红炮的出发位置</param>
        /// <returns>如果红帅安全则返回true</returns>
        private bool CanRedPieceJumpOut(int kingPos, int inc, int from, int to)
        {
            int pos = FindNextPos(kingPos + inc, inc);  //这里找到一个其它子，有可能是边界

            // 要排除这种下面这种可能性
            //        黑子to
            //        黑将
            //         |       
            //        红炮from
            //         |       
            //   ------K---红炮from---黑车----黑子to----
            //   aaaaa
            if (pos == from)
            {
                //  帅 -> 红炮from ->黑车(或黑将） -> 黑子to
                pos = FindNextPos(pos + inc, inc);
                if (board.pieceData[pos] == Board.BLACK_ROOK) return false; //被黑车将军
                if (board.pieceData[pos] == Board.BLACK_KING) return false;  // 老将对脸！
                pos = FindNextPos(pos + inc, inc);
                //-----K---红炮from---子----黑炮to----
                if (pos == to) return true;
                if (board.pieceData[pos] == Board.BLACK_CANNON) return false;  // 被炮将军
                return true;
            }

            //  帅 -> 只有一个其它子 -> 红炮from ->黑炮 -> 黑子to
            //            pos当前在这里
            if (FindForward(pos, inc, from, Board.BLACK_CANNON)) return false;
            return true;
        }
        /// <summary>
        /// 从start位置开始（不包括start)以inc增量向前搜索，沿途要经过pos位置，最后找到pieceForFind
        /// 中间不能遇到其它任何棋子
        /// start ----> pos ----> [pieceForFind棋子]
        /// </summary>
        /// <param name="start"></param>
        /// <param name="inc"></param>
        /// <param name="pos"></param>
        /// <param name="pieceForFind"></param>
        /// <returns></returns>
        private bool FindForward(int start, int inc, int pos, int pieceForFind)
        {
            start = FindNextPos(start + inc, inc);
            if (start != pos) return false;

            start = FindNextPos(start + inc, inc);
            if (board.pieceData[start] == pieceForFind) return true;
            return false;
        }


        /// <summary>
        /// 从start位置开始（不包括start)以inc增量向前搜索，沿途要经过pos位置，最后找到pieceForFind
        /// 中间不能遇到其它任何棋子
        /// start ----> pos1 ---> pos2 ----> [pieceForFind棋子]
        /// start ----> pos2 ---> pos1 ----> [pieceForFind棋子]
        /// </summary>
        /// <param name="start"></param>
        /// <param name="inc"></param>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="pieceForFind"></param>
        /// <returns></returns>
        private bool FindForward(int start, int inc, int pos1, int pos2, int pieceForFind)
        {
            //  帅 -> 红车(或红兵)from ->黑子(to) ->黑炮 
            //  帅 ->黑子(to) -> 红车(或红兵)from  ->黑炮 
            int pos = FindNextPos(start + inc, inc);  //这里找到一个其它子，有可能是边界
            if (pos == pos1)
            {
                //  帅 -> 红车(或红兵)from ->黑子(to) ->黑炮 
                pos = FindNextPos(pos + inc, inc);
                if (pos != pos2) return false;
                pos = FindNextPos(pos + inc, inc);
                if (board.pieceData[pos] == pieceForFind) return true;
                return false;
            }
            else if (pos == pos2)
            {
                //  帅 ->黑子(to) -> 红车(或红兵)from  ->黑炮 
                pos = FindNextPos(pos + inc, inc);
                if (pos != pos1) return false;
                pos = FindNextPos(pos + inc, inc);
                if (board.pieceData[pos] == pieceForFind) return true;
                return false;
            }
            return true;

        }


        private bool ExistOnlyOneRedPiece(int from, int to, out int piece)
        {
            int inc = board.IncSliding(from, to);
            from = FindNextPos(from + inc, inc);
            piece = board.pieceData[from];
            if (Board.IsRedPiece(piece))
            {
                from = FindNextPos(from + inc, inc);
                if (from == to) return true;
            }
            return false;
        }

        private bool ExistOnlyOneBlackPiece(int from, int to, out int piece)
        {
            int inc = board.IncSliding(from, to);
            from = FindNextPos(from + inc, inc);
            piece = board.pieceData[from];
            if (Board.IsBlackPiece(piece))
            {
                from = FindNextPos(from + inc, inc);
                if (from == to) return true;
            }
            return false;
        }


        /// <summary>
        /// 判断马从from到to之间有没有蹩马腿的棋子
        /// </summary>
        /// <param name="from">一个棋子要从这里离开</param>
        /// <param name="to">准备到达的位置</param>
        /// <returns></returns>
        private bool IsBlockedForKnight(int from, int to)
        {
            // 如果当前的子从from处离开，并且*处是蹩着马腿的地方
            //      -17           -17
            //-1    .       *      .    +1     
            //      *     from     *
            //-1    .       *      .    +1
            //     +17            +17
            int incBlocker = board.IncBlockKnightFrom(from, to);
            if (incBlocker != 0)
            {
            }
            return false;

        }



    }
}
