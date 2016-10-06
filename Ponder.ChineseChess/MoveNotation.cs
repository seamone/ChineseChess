using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ponder.ChineseChess
{
    /// <summary>
    /// 有关中文棋谱与内部坐标格式棋谱Move的转换
    /// </summary>
    public class MoveNotation
    {
        /// <summary>
        /// 红方棋子名称，用于棋谱表示
        /// </summary>
        private static readonly string[] RED_PIECE_NAMES = { "　", "车", "马", "炮", "相", "仕", "兵", "帅" };

        /// <summary>
        /// 黑方棋子名称，用于棋谱表示
        /// </summary>
        private static readonly string[] BLACK_PIECE_NAMES = { "　", "车", "马", "炮", "象", "士", "卒", "将" };

        // 红方棋谱文本中用大写的数字
        private static readonly string[] RED_DIGITS = { "", "一", "二", "三", "四", "五", "六", "七", "八", "九" };

        // 黑方棋谱文本中用全角的小写数字
        private static readonly string[] BLACK_DIGITS = { "", "１", "２", "３", "４", "５", "６", "７", "８", "９" };

        //// 是否走斜线的棋子
        private static readonly bool[] IS_TILT = 
        { 
                false, // 0
                false,  
                true,  /* Knight*/ 
                false, 
                true,  /* bishop*/ 
                true,  /* advisor*/ 
                false, 
                false
        };

        public static bool IsTiltPiece(int piece)
        {
            return IS_TILT[Board.PieceType(piece)];
        }

        /// <summary>
        /// 把大写数字（一到九）和全角数字（１到９）转换为整型数
        /// </summary>
        /// <param name="c">大写数字或全角数字</param>
        /// <returns>转换成功时返回1-9，如果不在此范围内返回0</returns>
        public static int ToInt(char c)
        {
            int index = "一二三四五六七八九".IndexOf(c);
            if (index >= 0)
                return index + 1;
            index = "１２３４５６７８９".IndexOf(c);
            if (index >= 0)
                return index + 1;
            return 0;
        }


        /// <summary>
        /// 把纵列表示中的大写数字（一到九）和全角数字（１到９）转换为实际的列号
        /// 红方是从右向左数的，所以“九”到“一”分别对应于0-8
        /// 黑方是从左向右数的，所以“１”到“９”分别对应于0-8
        /// </summary>
        /// <param name="c">大写数字或全角数字</param>
        /// <returns>转换为列号，从0到8；否则返回-1</returns>
        public static int ToFileNum(char c)
        {
            int index = "一二三四五六七八九".IndexOf(c);
            if (index >= 0)
                return 8 - index;
            index = "１２３４５６７８９".IndexOf(c);
            if (index >= 0) return index;
            return -1;
        }

        /// <summary>
        ///  数字转换为大写的中文数字，1->"一"
        /// </summary>
        /// <param name="num">从1到9的整数</param>
        /// <returns>大写中文数字，如果数字不在1到9之间，返回""</returns>
        public static string RedDigitString(int num)
        {
            if (num < 1 || num > 9) return "";
            return RED_DIGITS[num];
        }

        /// <summary>
        ///  数字转换为全角的中文数字，1->"１"
        /// </summary>
        /// <param name="num">从1到9的整数</param>
        /// <returns>全角中文数字，如果数字不在1到9之间，返回""</returns>
        public static string BlackDigitString(int num)
        {
            if (num < 1 || num > 9) return "";
            return BLACK_DIGITS[num];
        }


        /// <summary>
        /// 得到棋谱表示中规范的棋子名称
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public static string PieceName(int piece)
        {
            if (Board.IsRedPiece(piece))
                return RED_PIECE_NAMES[Board.PieceType(piece)];
            else
                return BLACK_PIECE_NAMES[Board.PieceType(piece)];
        }

     

        #region  把中文纵列格式的棋谱转换为Move

        /// <summary>
        /// 把中文纵列格式的棋谱转换成Move，例如：炮二平五-->h2e2
        /// </summary>
        /// <param name="board">在走该着法之前的盘面情况</param>
        /// <param name="chNotation">中文纵列标记</param>
        /// <returns>Move。转换过程中出现的错误，都会抛出异常</returns>
        public static Move CreateMoveFromChineseNotation(Board board, string chNotation)
        {
            if (chNotation.Length != 4)
            {
                throw new Exception("着法必须是4个汉字:" + chNotation);
            }

            char first = chNotation[0];
            char second = chNotation[1];
            char third = chNotation[2];
            char fourth = chNotation[3];

            int piece = BoardUtil.PieceFromString(chNotation, board.IsRedTurn);

            int startPos = 0;

            // 先要根据前2个汉字找到棋子的初始位置，会有两大类情况，（1）车二；（2）前车
            // 先处理第一种情况：类似这样的“炮二平五”，“马８进７”
            if ("车马炮相仕兵帅象士卒将".Contains(first))
            {
                // 第2个汉字就可以得到起始点的纵列坐标, from 0 to 8
                int startFile = ToFileNum(second);

                // 对于相（象）、士（仕）来说，得找到前面或后面的象、士，因为“相七退五”时，七路上可能会有2个象
                // “仕六进五”也是同样的道理
                if ("相仕象士".Contains(first))
                {
                    // 如果一条线上有2个象，则需要用第三个字符是“进”还是“退”来判断是移动了哪个象
                    if (first == '相' && second == '三' && third == '进') startPos = Board.G0;
                    else if (first == '相' && second == '三' && third == '退') startPos = Board.G4;
                    else if (first == '相' && second == '七' && third == '进') startPos = Board.C0;
                    else if (first == '相' && second == '七' && third == '退') startPos = Board.C4;
                    else if (first == '仕' && second == '四' && third == '进') startPos = Board.F0;
                    else if (first == '仕' && second == '四' && third == '退') startPos = Board.F2;
                    else if (first == '仕' && second == '六' && third == '进') startPos = Board.D0;
                    else if (first == '仕' && second == '六' && third == '退') startPos = Board.D2;
                    else if (first == '象' && second == '３' && third == '进') startPos = Board.C9;
                    else if (first == '象' && second == '３' && third == '退') startPos = Board.C5;
                    else if (first == '象' && second == '７' && third == '进') startPos = Board.G9;
                    else if (first == '象' && second == '７' && third == '退') startPos = Board.G5;
                    else if (first == '士' && second == '４' && third == '进') startPos = Board.D9;
                    else if (first == '士' && second == '４' && third == '退') startPos = Board.D7;
                    else if (first == '士' && second == '６' && third == '进') startPos = Board.F9;
                    else if (first == '士' && second == '６' && third == '退') startPos = Board.F7;
                    else
                    {
                        // throw new Exception("error of the position of bishop or advisor");
                        startPos = BoardUtil.FindPosition(board, piece, startFile);
                    }
                }
                else
                {
                    // 按理说，正规的棋谱里这里应该同一纵线上只有一个车、马、炮、兵、卒，但有些不正规的棋谱里会遇到问题
                    // TODO: 以后有时间的，可以多做一些容错性的判断，如果一条纵线上有2个相同类的棋子，但一个棋子无法走动时，则以另一个棋子的着法为准
                    startPos = BoardUtil.FindPosition(board, piece, startFile);
                }
            }
            else // 在这里处理第二大类情况，类似“前炮进二”这样的情况
            {
                // 兵、卒有些特殊，有可能一列中3个兵，这样会有“中兵”或“中卒”的情况出现
                // 2011年无极棋谱\华山棋谱\2011-01-09 青山豹(天罡) 和 一把家族(无极).pgn
                // TODO:　这里面极少可能出现的棋谱，“前１平２”的着法
                if (piece == Board.RED_PAWN || piece == Board.BLACK_PAWN)
                {
                    int col = BoardUtil.Find3PawnFileNum(board, piece);
                    if (first == '中')
                    {
                        startPos = BoardUtil.FindMiddlePawn(board, piece, col);
                    }
                    else if ((first == '前' && board.IsRedTurn) || (first == '后' && !board.IsRedTurn))
                        startPos = BoardUtil.FindPositionFromBlackToRed(board, piece, col);
                    else
                        startPos = BoardUtil.FindPositionFromRedToBlack(board, piece, col);
                }
                else
                {
                    // 扫描整个棋盘，找到类似“前炮”这样的棋子的位置
                    if ((first == '前' && board.IsRedTurn) || (first == '后' && !board.IsRedTurn))
                        startPos = BoardUtil.FindPositionFromBlackToRed(board, piece);
                    else
                        startPos = BoardUtil.FindPositionFromRedToBlack(board, piece);
                }
            }

            if (startPos == 0)
            {
                throw new Exception("在盘面上找不到" + first + second + "这枚棋子，" + chNotation);
            }

            /////////////////////////////////////////////////////////////////////
            ////////////////////////// 以后都是为了找到终点的坐标了
            /////////////////////////////////////////////////////////////////////

            int startPosFile = Board.File(startPos);
            int startPosRank = Board.Rank(startPos);

            // 根据第3个字符“平、进、退”分别进行相应的处理
            int endFile = -1;
            int endRank = -1;
            if (third == '平')
            {
                if (!IsTiltPiece(piece))
                {
                    endFile = ToFileNum(fourth);
                    if (endFile < 0)
                    {
                        throw new Exception(chNotation + "中的第4个字符错误，应该为大写数字或全角数字");
                    }
                    // 对于“平”着法，起点和终点的行号坐标是一样的
                    endRank = startPosRank;
                }
                else  //士、象、马 
                {
                    throw new Exception("士象马不能有平的移动：" + chNotation);
                }
            }
            else if (third == '退')
            {
                if (piece == Board.RED_KNIGHT || piece == Board.BLACK_KNIGHT)
                {
                    endFile = ToFileNum(fourth);
                    if (endFile < 0)
                    {
                        throw new Exception(chNotation + "中的第4个字符错误，应该为大写数字或全角数字");
                    }
                    int diff = Math.Abs(endFile - startPosFile);
                    if (diff == 1)
                    {
                        if (!board.IsRedTurn) endRank = startPosRank - 2;
                        else endRank = startPosRank + 2;
                    }
                    else if (diff == 2)
                    {
                        if (!board.IsRedTurn) endRank = startPosRank - 1;
                        else endRank = startPosRank + 1;
                    }
                    else
                    {
                        throw new Exception("马的走法有误：" + chNotation);
                    }
                }
                else if (piece == Board.RED_BISHOP || piece == Board.BLACK_BISHOP)
                {
                    endFile = ToFileNum(fourth);
                    if (endFile < 0)
                    {
                        throw new Exception(chNotation + "中的第4个字符错误，应该为大写数字或全角数字");
                    }
                    int diff = Math.Abs(endFile - startPosFile);
                    if (diff == 2)
                    {
                        if (!board.IsRedTurn) endRank = startPosRank - 2;
                        else endRank = startPosRank + 2;
                    }
                    else
                    {
                        throw new Exception("相象的走法有误：" + chNotation);
                    }
                }
                else if (piece == Board.RED_ADVISOR || piece == Board.BLACK_ADVISOR)
                {
                    endFile = ToFileNum(fourth);
                    if (endFile < 0)
                    {
                        throw new Exception(chNotation + "中的第4个字符错误，应该为大写数字或全角数字");
                    }
                    int diff = Math.Abs(endFile - startPosFile);
                    if (diff == 1)
                    {
                        if (!board.IsRedTurn) endRank = startPosRank - 1;
                        else endRank = startPosRank + 1;
                    }
                    else
                    {
                        throw new Exception("仕士的走法有误：" + chNotation);
                    }
                }
                else if (  // 炮、车、帅、兵在纵线上移动时规则都是一样的
                    piece == Board.RED_CANNON || piece == Board.BLACK_CANNON ||
                    piece == Board.RED_ROOK || piece == Board.BLACK_ROOK ||
                    piece == Board.RED_KING || piece == Board.BLACK_KING ||
                    piece == Board.RED_PAWN || piece == Board.BLACK_PAWN)
                {
                    endFile = startPosFile;

                    int diff = ToInt(fourth);
                    if (diff < 0)
                    {
                        throw new Exception(chNotation + "中的第4个字符错误，应该为大写数字或全角数字");
                    }
                    if (!board.IsRedTurn)
                        endRank = startPosRank - diff;
                    else
                        endRank = startPosRank + diff;
                }
                else
                {
                    throw new Exception("不认识的着法：" + chNotation);
                }
            }
            else // 进
            {
                if (third != '进')
                {
                    throw new Exception(chNotation + "中的第3个字符错误，应该进、退、平");
                }
                if (piece == Board.RED_KNIGHT || piece == Board.BLACK_KNIGHT)
                {
                    endFile = ToFileNum(fourth);
                    if (endFile < 0)
                    {
                        throw new Exception(chNotation + "中的第4个字符错误，应该为大写数字或全角数字");
                    }
                    int diff = Math.Abs(endFile - startPosFile);
                    if (diff == 1)
                    {
                        if (!board.IsRedTurn) endRank = startPosRank + 2;
                        else endRank = startPosRank - 2;
                    }
                    else if (diff == 2)
                    {
                        if (!board.IsRedTurn) endRank = startPosRank + 1;
                        else endRank = startPosRank - 1;
                    }
                    else
                    {
                        throw new Exception("马的走法有误：" + chNotation);
                    }
                }
                else if (piece == Board.RED_BISHOP || piece == Board.BLACK_BISHOP)
                {
                    endFile = ToFileNum(fourth);
                    if (endFile < 0)
                    {
                        throw new Exception(chNotation + "中的第4个字符错误，应该为大写数字或全角数字");
                    }
                    int diff = Math.Abs(endFile - startPosFile);
                    if (diff == 2)
                    {
                        if (!board.IsRedTurn) endRank = startPosRank + 2;
                        else endRank = startPosRank - 2;
                    }
                    else
                    {
                        throw new Exception("相象的走法有误：" + chNotation);
                    }
                }
                else if (piece == Board.RED_ADVISOR || piece == Board.BLACK_ADVISOR)
                {
                    endFile = ToFileNum(fourth);
                    if (endFile < 0)
                    {
                        throw new Exception(chNotation + "中的第4个字符错误，应该为大写数字或全角数字");
                    }
                    int diff = Math.Abs(endFile - startPosFile);
                    if (diff == 1)
                    {
                        if (!board.IsRedTurn) endRank = startPosRank + 1;
                        else endRank = startPosRank - 1;
                    }
                    else
                    {
                        throw new Exception("士仕的走法有误：" + chNotation);
                    }
                }
                else if (
                    piece == Board.RED_CANNON || piece == Board.BLACK_CANNON ||
                    piece == Board.RED_ROOK || piece == Board.BLACK_ROOK ||
                    piece == Board.RED_KING || piece == Board.BLACK_KING ||
                    piece == Board.RED_PAWN || piece == Board.BLACK_PAWN)
                {
                    endFile = startPosFile;  // !!!!!
                    int diff = ToInt(fourth);
                    if (diff < 0)
                    {
                        throw new Exception(chNotation + "中的第4个字符错误，应该为大写数字或全角数字");
                    }
                    if (!board.IsRedTurn)
                        endRank = startPosRank + diff;
                    else
                        endRank = startPosRank - diff;
                }
                else
                {
                    throw new Exception("不认识的着法：" + chNotation);
                }
            }
            if (endFile < 0 || endFile >= 9 || endRank < 0 || endRank >= 10)
            {
                throw new Exception("棋子走到棋盘之外了：" + chNotation + "\n" + board);
            }


            int endPos = BoardUtil.Pos(endFile, endRank);
            Move move = board.CreateMove(startPos, endPos);
            return move;
        }
        #endregion

        #region 把Move转换为中文纵列格式的棋谱文本

        /// <summary>
        /// 把Move转换为“炮二平五”中文纵列格式的棋谱文本
        /// </summary>
        /// <param name="move">着法</param>
        /// <returns>中文棋谱字符串</returns>
        public static string NotationConvertedFromMove(Board board, Move move)
        {
            string pieceName = PieceName(move.Piece);
            int fileFrom = Board.File(move.From);
            int rankFrom = Board.Rank(move.From);
            int fileTo = Board.File(move.To);
            int rankTo = Board.Rank(move.To);
            string strNum = board.IsRedTurn ? RedDigitString(9 - fileFrom) : BlackDigitString(1 + fileFrom);

            //思路:累计在同一条纵线的上方up或下方down是否有相同棋子,
            //如果都有则为"中",否则根据上下判断"前后".黑棋与红旗"前后"颠倒
            int up = 0;
            int down = 0;
            for (int rank = 0; rank <= 9; rank++)
            {
                if (rank == rankFrom) continue;
                if (board.pieceData[BoardUtil.Pos(fileFrom, rank)] == move.Piece)
                {
                    if (rank > rankFrom) ++down;
                    if (rank < rankFrom) ++up;
                }
            }

            // 前2个汉字，这种名字会产生“前炮”、“中兵”、“后马”的效果
            string notation1;

            if (up > 0 && down > 0)
            {
                notation1 = "中";
            }
            else if (up > 0)
            {
                notation1 = board.IsRedTurn ? "前" : notation1 = "后";
            }
            else if (down > 0)
            {
                notation1 = board.IsRedTurn ? "后" : notation1 = "前";
            }
            else
            {
                notation1 = pieceName + strNum;
            }


            // 后2个汉字
            string notation2;
            if (board.IsRedTurn)
            {
                //逻辑说明:首先判断纵向y轴是否改变,负则后退,否则前进.不变则平移.
                int num = rankFrom - rankTo;
                if (num < 0)
                {
                    if (IsTiltPiece(move.Piece))
                        notation2 = "退" + RedDigitString(Math.Abs(9 - fileTo));
                    else
                        notation2 = "退" + RedDigitString(Math.Abs(rankFrom - rankTo));
                }
                else if (num > 0)
                {
                    if (IsTiltPiece(move.Piece))
                        notation2 = "进" + RedDigitString(Math.Abs(9 - fileTo));
                    else
                        notation2 = "进" + RedDigitString(Math.Abs(rankFrom - rankTo));
                }
                else // (num == 0)
                {
                    notation2 = "平" + RedDigitString(Math.Abs(9 - fileTo));
                }

            }


            else // 黑方的着法
            {
                //逻辑说明:首先判断纵向y轴是否改变,负则后退,否则前进.不变则平移.
                int num = rankFrom - rankTo;
                if (num > 0)
                {
                    if (IsTiltPiece(move.Piece))
                        notation2 = "退" + BlackDigitString(Math.Abs(1 + fileTo));
                    else
                        notation2 = "退" + BlackDigitString(Math.Abs(rankFrom - rankTo));
                }
                else if (num < 0)
                {
                    if (IsTiltPiece(move.Piece))
                        notation2 = "进" + BlackDigitString(Math.Abs(1 + fileTo));
                    else
                        notation2 = "进" + BlackDigitString(Math.Abs(rankFrom - rankTo));
                }
                else //(num == 0)
                    notation2 = "平" + BlackDigitString(Math.Abs(1 + fileTo));

            }
            return notation1 + notation2;
        }

        #endregion

    }
}
