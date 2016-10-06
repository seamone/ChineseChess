using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ponder.ChineseChess
{
    /// <summary>
    /// 这些方法效率并不高，不要在着法生成和评估中频繁调用，所以单独放在一个类中
    /// </summary>
    public static class BoardUtil
    {
        /// <summary>
        /// 把9x10的棋盘位置映射到17x14的棋盘位置上
        /// </summary>
        private static readonly int[] POS_90_238 = 
        {
            38,   39,  40,  41,  42,  43,  44,  45,  46,
            55,   56,  57,  58,  59,  60,  61,  62,  63,
            72,   73,  74,  75,  76,  77,  78,  79,  80,
            89,   90,  91,  92,  93,  94,  95,  96,  97,
            106, 107, 108, 109, 110, 111, 112, 113, 114,
            123, 124, 125, 126, 127, 128, 129, 130, 131,
            140, 141, 142, 143, 144, 145, 146, 147, 148,
            157, 158, 159, 160, 161, 162, 163, 164, 165,
            174, 175, 176, 177, 178, 179, 180, 181, 182,
            191, 192, 193, 194, 195, 196, 197, 198, 199
        };


        /// <summary>
        /// 把0-89的整数映射到棋盘的内部表示坐标
        /// </summary>
        /// <param name="i90">0-89的整数</param>
        /// <returns></returns>
        public static int Pos(int i90)
        {
            return POS_90_238[i90];
        }

        /// <summary>
        /// 根据一个字符串生成棋盘的交叉点位置，如果参数无效时，返回0
        /// </summary>
        /// <param name="s">必须是2个字符，
        /// 第一个字符从'a'到'i'，从左到右
        /// 第二个字符是'0'-'9'，从下（红方）到上（黑方）
        /// </param>
        public static int Pos(string s)
        {
            if (s == null || s.Length != 2)
            {
                return 0;
            }

            s = s.ToLower();
            int file = s[0] - 'a';
            int rank = 9 - (s[1] - '0');  // 注意是颠倒的！
            return Pos(file, rank);
        }

        /// <summary>
        /// 根据行号（0-8）、列号（0-9）得到内部的位置编号
        /// </summary>
        /// <param name="file">列号，0-8</param>
        /// <param name="rank">行号，0-9</param>
        /// <returns>返回内部的编号，从0-238，如果不满足条件，则抛出异常</returns>
        public static int Pos(int file, int rank)
        {
            if (file >= 0 && file < 9 && rank >= 0 && rank < 10)
                return BoardUtil.Pos(rank * 9 + file);
            throw new Exception("行号、列号越界");
        }

        /// <summary>
        /// 在棋盘内查找三个兵或卒出现在同一列时的列号
        /// 在纵列表示法中会出现“中兵平二”这样的记录
        /// 要全盘查找哪一列上有3个兵，才可以准确定位这个兵
        /// </summary>
        /// <param name="board"></param>
        /// <param name="piece"></param>
        /// <returns>找到3兵或卒在同一列时,返回0-8的列号；否则,返回-1</returns>
        public static int Find3PawnFileNum(Board board, int piece)
        {
            // 这里用了一个比较懒的办法，全部找一遍，统计每一列上兵的出现个数
            // 哪一列上兵出现的次数最多，就返回哪个列号了
            int countMax = 0;
            int file = -1;

            for (int i = 0; i < 9; i++)
            {
                int numFound = 0;

                for (int rank = 0; rank < 10; rank++)
                {
                    int pos = BoardUtil.Pos(i, rank);
                    if (board.pieceData[pos] == piece)
                    {
                        numFound++;
                    }

                    if (numFound > countMax)
                    {
                        file = i;
                        countMax = numFound;
                    }
                }
            }
            return file;
        }


        ///<summary>
        /// 在某一列上找中兵或中卒的位置编号
        ///</summary>
        ///<param name="piece"></param>
        ///<param name="file">列号</param>
        ///<returns></returns>
        public static int FindMiddlePawn(Board board, int piece, int file)
        {
            int foundPos = -1;
            int numFound = 0;

            for (int rank = 0; rank < 10; rank++)
            {
                int pos = BoardUtil.Pos(file, rank);
                if (board.pieceData[pos] == piece)
                {
                    foundPos = pos;
                    // 如果之前发现过一个兵，又在同一列上找到一个兵，就是“中兵”了
                    if (numFound == 1) break;

                    ++numFound;
                }
            }

            return foundPos;
        }




        public static int FindPosition(Board board, int piece, int file)
        {
            return FindPositionFromRedToBlack(board, piece, file);
        }

        /// <summary>
        /// 在棋盘的某一列上查找某个棋子的准确位置，从下边（红方）到上边（黑方）搜索
        /// </summary>
        /// <param name="board">棋盘</param>
        /// <param name="piece">棋子编号</param>
        /// <param name="file">列号</param>
        /// <returns>返回找到的棋子位置的编号，找不到时返回0</returns>
        public static int FindPositionFromRedToBlack(Board board, int piece, int file)
        {
            for (int rank = 9; rank >= 0; rank--)
            {
                int pos = BoardUtil.Pos(file, rank);
                if (board.pieceData[pos] == piece)
                {
                    return pos;
                }
            }
            return 0;
        }

        /// <summary>
        /// 在棋盘的某一列上查找某个棋子的准确位置，从下边（红方）到上边（黑方）搜索
        /// </summary>
        /// <param name="board">棋盘</param>
        /// <param name="piece">棋子编号</param>
        /// <returns>返回找到的棋子位置的编号，找不到时返回0</returns>
        public static int FindPositionFromRedToBlack(Board board, int piece)
        {
            for (int file = 0; file <= 8; file++)
                for (int rank = 9; rank >= 0; rank--)
                {
                    int pos = BoardUtil.Pos(file, rank);
                    if (board.pieceData[pos] == piece)
                    {
                        return pos;
                    }
                }
            return 0;
        }

        /// <summary>
        /// 在棋盘的某一列上查找某个棋子的准确位置，从上边（黑方）到下边（红方）的方向搜索
        /// </summary>
        /// <param name="board">棋盘</param>
        /// <param name="piece">棋子编号</param>
        /// <param name="file">列号</param>
        /// <returns>返回找到的棋子位置的编号，找不到时返回0</returns>
        public static int FindPositionFromBlackToRed(Board board, int piece, int file)
        {
            for (int rank = 0; rank <= 9; rank++)
            {
                int pos = BoardUtil.Pos(file, rank);
                if (board.pieceData[pos] == piece)
                {
                    return pos;
                }
            }
            return 0;
        }


        /// <summary>
        /// 在棋盘的某一列上查找某个棋子的准确位置，从上边（黑方）到下边（红方）的方向搜索
        /// </summary>
        /// <param name="board">棋盘</param>
        /// <param name="piece">棋子编号</param>
        /// <returns>返回找到的棋子位置的编号，找不到时返回0</returns>
        public static int FindPositionFromBlackToRed(Board board, int piece)
        {
            for (int file = 0; file <= 8; file++)
                for (int rank = 0; rank <= 9; rank++)
                {
                    int pos = BoardUtil.Pos(file, rank);
                    if (board.pieceData[pos] == piece)
                    {
                        return pos;
                    }
                }
            return 0;
        }




        /// <summary>
        /// 把棋子的字符代号转换为棋子的内部表示（一个整数）
        /// </summary>
        /// <param name="p">棋子字符，只能是RrNnCcBbAaPpKk中的一个，大写是红方棋子，小写是黑方棋子</param>
        /// <returns>RED_ROOK, BLACK_ROOK, ...等14个棋子类型之一。如果不是14个字符之一，返回0</returns>
        public static int PieceID(char p)
        {
            switch (p)
            {
                case 'R': return Board.RED_ROOK;
                case 'r': return Board.BLACK_ROOK;
                case 'N': return Board.RED_KNIGHT;
                case 'n': return Board.BLACK_KNIGHT;
                case 'C': return Board.RED_CANNON;
                case 'c': return Board.BLACK_CANNON;
                case 'B': return Board.RED_BISHOP;
                case 'b': return Board.BLACK_BISHOP;
                case 'A': return Board.RED_ADVISOR;
                case 'a': return Board.BLACK_ADVISOR;
                case 'K': return Board.RED_KING;
                case 'k': return Board.BLACK_KING;
                case 'P': return Board.RED_PAWN;
                case 'p': return Board.BLACK_PAWN;
                default: return 0;
            }
        }

        /// <summary>
        /// 根据字符串里含着车、马、炮等字符来得到棋子的编号
        /// </summary>
        /// <param name="notation">例如：炮二平五</param>
        /// <param name="isRed">是不是红方的棋子编号</param>
        /// <returns></returns>
        public static int PieceFromString(string notation, bool isRed)
        {
            int idxPiece;

            if (isRed)
            {
                if (notation.Contains('车'))
                    idxPiece = Board.RED_ROOK;
                else if (notation.Contains('马'))
                    idxPiece = Board.RED_KNIGHT;
                else if (notation.Contains('炮'))
                    idxPiece = Board.RED_CANNON;
                else if (notation.Contains('相'))
                    idxPiece = Board.RED_BISHOP;
                else if (notation.Contains('仕'))
                    idxPiece = Board.RED_ADVISOR;
                else if (notation.Contains('帅'))
                    idxPiece = Board.RED_KING;
                else
                {
                    // 在中国象棋中有一种特别少见的情况就是某2列上都有2个兵出现，这时有这样的“前一平二”的记法
                    // 表示移动一路上的前面的兵
                    // 在这种棋谱中是不会出现棋子的名称的，所以默认都返回“兵”了
                    idxPiece = Board.RED_PAWN;
                }
            }
            else
            {
                if (notation.Contains('车'))
                    idxPiece = Board.BLACK_ROOK;
                else if (notation.Contains('马'))
                    idxPiece = Board.BLACK_KNIGHT;
                else if (notation.Contains('炮'))
                    idxPiece = Board.BLACK_CANNON;
                else if (notation.Contains('象'))
                    idxPiece = Board.BLACK_BISHOP;
                else if (notation.Contains('士'))
                    idxPiece = Board.BLACK_ADVISOR;
                else if (notation.Contains('将'))
                    idxPiece = Board.BLACK_KING;
                else
                {
                    // 在中国象棋中有一种特别少见的情况就是某2列上都有2个兵出现，这时有这样的“前１平２”的记法
                    // 表示移动一路上的前面的兵
                    // 在这种棋谱中是不会出现棋子的名称的，所以默认都返回“卒”了
                    idxPiece = Board.BLACK_PAWN;
                }

            }
            return idxPiece;
        }

        /// <summary>
        /// 用于打印输出时的棋子名称，通过名称就可以把红黑区别出来
        /// 例如：红方的“车”写成“伡”
        /// </summary>
        /// <param name="piece">棋子编号</param>
        /// <returns>用于打印输出的棋子中文名称</returns>
        public static string PieceChineseNameForPrint(int piece)
        {
            switch (piece)
            {
                case Board.RED_KING: return "帅";
                case Board.RED_ROOK: return "伡";
                case Board.RED_KNIGHT: return "傌";
                case Board.RED_CANNON: return "炮";
                case Board.RED_BISHOP: return "相";
                case Board.RED_ADVISOR: return "仕";
                case Board.RED_PAWN: return "兵";

                case Board.BLACK_KING: return "将";
                case Board.BLACK_ROOK: return "车";
                case Board.BLACK_KNIGHT: return "马";
                case Board.BLACK_CANNON: return "包";
                case Board.BLACK_BISHOP: return "象";
                case Board.BLACK_ADVISOR: return "士";
                case Board.BLACK_PAWN: return "卒";

                default: return "";
            }
        }

        /// <summary>
        /// 得到用字符表示的棋子名称
        /// 七种棋子对应的字符，大写是红方，小写是黑方
        /// R车 N马 C炮 B相 A仕 P兵 K帅
        /// r车 n马 c炮 b象 a士 p卒 k将
        /// </summary>
        /// <param name="piece">棋子编号</param>
        /// <returns>用于打印输出的棋子字符</returns>
        public static char PieceCharForPrint(int piece)
        {
            switch (piece)
            {
                case Board.RED_KING: return 'K';
                case Board.RED_ROOK: return 'R';
                case Board.RED_KNIGHT: return 'N';
                case Board.RED_CANNON: return 'C';
                case Board.RED_BISHOP: return 'B';
                case Board.RED_ADVISOR: return 'A';
                case Board.RED_PAWN: return 'P';

                case Board.BLACK_KING: return 'k';
                case Board.BLACK_ROOK: return 'r';
                case Board.BLACK_KNIGHT: return 'n';
                case Board.BLACK_CANNON: return 'c';
                case Board.BLACK_BISHOP: return 'b';
                case Board.BLACK_ADVISOR: return 'a';
                case Board.BLACK_PAWN: return 'p';

                default: return ' ';
            }
        }

        /// <summary>
        /// 用字符串表示的着法
        /// </summary>
        /// <param name="strMove">例如："a2c2"</param>
        /// <returns></returns>
        public static Move CreateMoveFromString(Board board, string strMove)
        {
            int from = BoardUtil.Pos(strMove.Substring(0, 2));
            int to = BoardUtil.Pos(strMove.Substring(2, 2));
            return board.CreateMove(from, to);
        }


    }


 
}
