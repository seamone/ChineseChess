using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ponder.ChineseChess
{
    /// <summary>
    /// 与输入和输出FEN有关的内容
    /// </summary>
    public class Fen
    {
        /// <summary>
        /// 根据FEN字符串初始化棋盘
        /// http://chessprogramming.wikispaces.com/Forsyth-Edwards+Notation
        /// FEN表示法从第9行到第0行扫描棋子，在每一行中，从第0列（第A列）到第8列（第I列）扫描棋子，每一行以字符'/'结束。
        /// 例如：初始状态的FEN字符串是  "rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR w"
        /// 在描述完棋子位置后，根一个空格，然后是一个表示轮到哪方走棋的字符，如果是b，表示轮黑方走棋，r或其它字符都表示红方走棋
        /// </summary>
        /// <param name="fen">FEN格式字符串</param>
        public static void InitFromFEN(Board board, string fen)
        {
            int piece;
            int index = 0;
            int pos90 = 0;

            while (fen[index] != ' ')
            {
                piece = BoardUtil.PieceID(fen[index]);  //把字符转换为棋子的编号;
                if (piece > 0)
                {
                    int pos238 = BoardUtil.Pos(pos90);
                    AddPiece(board, piece, pos238);
                    ++pos90;
                }
                else if (fen[index] >= '1' && fen[index] <= '9')
                {
                    pos90 += fen[index] - '0';        // skip empty squares
                }
                else if (fen[index] != '/')
                {  // if there another char than '/' throw exception
                    // throw new ArgumentException("fen");
                    // 如果程序执行到这里表示FEN字符串有错
                    throw new Exception("Fen代码出错，缺少终止符'/'");
                }

                ++index;
            }

            ++index;

            // 由于国际象棋中是白方先行，这里会记录一个w字母，但在中国象棋中通常会记录r，代表红方
            // 这里把不是b字母的都认为是轮红方走棋
            board.IsRedTurn = (fen[index] != 'b');

            // 最后更新另外几个特殊的位棋盘的状态
            board.TotalRedPiece = board.RedPieceNum[1] + board.RedPieceNum[2] + board.RedPieceNum[3] + board.RedPieceNum[4]
                + board.RedPieceNum[5] + board.RedPieceNum[6] + 1;  // 最后的1代表红帅
            board.TotalBlackPiece = board.BlackPieceNum[1] + board.BlackPieceNum[2] + board.BlackPieceNum[3] + board.BlackPieceNum[4]
                + board.BlackPieceNum[5] + board.BlackPieceNum[6] + 1;
            board.ZobristKey = Zobrist.ZoristHash(board);
        }



        private static void AddPiece(Board board, int piece, int pos238)
        {
            board.pieceData[pos238] = piece;

            int idx = Board.PieceType(piece);

            if (Board.IsRedPiece(piece))
            {
                int num = board.RedPieceNum[idx];
                board.RedPieceIndex[pos238] = num;
                board.RedPiecePos[idx, num] = pos238;
                ++(board.RedPieceNum[idx]);
            }
            else
            {
                int num = board.BlackPieceNum[idx];
                board.BlackPieceIndex[pos238] = num;
                board.BlackPiecePos[idx, num] = pos238;
                ++(board.BlackPieceNum[idx]);
            }
        }
            //switch (piece)
            //{
            //    case RED_ROOK:
            //        redIndex[pos238] = RedPieces[IDX_ROOK];
            //        RedPieceList[IDX_ROOK, RedPieces[IDX_ROOK]++] = pos238; break;
            //    case RED_KNIGHT:
            //        redIndex[pos238] = RedPieces[IDX_KNIGHT];
            //        RedPieceList[IDX_KNIGHT, RedPieces[IDX_KNIGHT]++] = pos238; break;
            //    case RED_CANNON:
            //        redIndex[pos238] = RedPieces[IDX_CANNON];
            //        RedPieceList[IDX_CANNON, RedPieces[IDX_CANNON]++] = pos238; break;
            //    case RED_BISHOP:
            //        redIndex[pos238] = RedPieces[IDX_BISHOP];
            //        RedPieceList[IDX_BISHOP, RedPieces[IDX_BISHOP]++] = pos238; break;
            //    case RED_ADVISOR:
            //        redIndex[pos238] = RedPieces[IDX_ADVISOR];
            //        RedPieceList[IDX_ADVISOR, RedPieces[IDX_ADVISOR]++] = pos238; break;
            //    case RED_PAWN:
            //        redIndex[pos238] = RedPieces[IDX_PAWN];
            //        RedPieceList[IDX_PAWN, RedPieces[IDX_PAWN]++] = pos238; break;
            //    case RED_KING:
            //        //RedKingPos = pos238; break;
            //        redIndex[pos238] = RedPieces[IDX_KING];
            //        RedPieceList[IDX_KING, RedPieces[IDX_KING]++] = pos238; break;

            //    case BLACK_ROOK:
            //        blackIndex[pos238] = BlackPieces[IDX_ROOK];
            //        BlackPieceList[IDX_ROOK, BlackPieces[IDX_ROOK]++] = pos238; break;
            //    case BLACK_KNIGHT:
            //        blackIndex[pos238] = BlackPieces[IDX_KNIGHT];
            //        BlackPieceList[IDX_KNIGHT, BlackPieces[IDX_KNIGHT]++] = pos238; break;
            //    case BLACK_CANNON:
            //        blackIndex[pos238] = BlackPieces[IDX_CANNON];
            //        BlackPieceList[IDX_CANNON, BlackPieces[IDX_CANNON]++] = pos238; break;
            //    case BLACK_BISHOP:
            //        blackIndex[pos238] = BlackPieces[IDX_BISHOP];
            //        BlackPieceList[IDX_BISHOP, BlackPieces[IDX_BISHOP]++] = pos238; break;
            //    case BLACK_ADVISOR:
            //        blackIndex[pos238] = BlackPieces[IDX_ADVISOR];
            //        BlackPieceList[IDX_ADVISOR, BlackPieces[IDX_ADVISOR]++] = pos238; break;
            //    case BLACK_PAWN:
            //        blackIndex[pos238] = BlackPieces[IDX_PAWN];
            //        BlackPieceList[IDX_PAWN, BlackPieces[IDX_PAWN]++] = pos238; break;
            //    case BLACK_KING:
            //        BlackKingPos = pos238; break;
            //}
        



        /// <summary>
        /// TODO: 得到一个局面的FEN字符串
        /// </summary>
        /// <returns>FEN字符串</returns>
        public static string ToFEN()
        {
            //char[] fen = new char[200];

            //int[] pieces = new int[90];

            //for (int i = 0; i < 90; i++) pieces[i] = 0;

            //for (int i = 1; i <= 15; i++)
            //{
            //    if (i == 8) continue;
            //    BitBoard bb = BB[i];
            //    while (bb.H != 0 || bb.L != 0)
            //    {
            //        int pos = bb.GetLowestBitWithReset();
            //        pieces[(pos % 9) + (9 - (pos / 9)) * 9] = i;
            //    }
            //}

            //// 用于统计空位置
            //int emptySqNo = 0;
            //int indexFEN = 0;
            //for (int sqIndex = 0; sqIndex < 90; sqIndex++)
            //{
            //    // if the end of row is reached
            //    if (sqIndex % 9 == 0 && sqIndex > 0)
            //    {
            //        // write the number of empty squares (if any) and reset it
            //        if (emptySqNo != 0)
            //        {
            //            fen[indexFEN++] = DIGITS[emptySqNo];
            //            emptySqNo = 0;
            //        }
            //        fen[indexFEN++] = '/';
            //    }

            //    //if there is a piece on this square
            //    if (pieces[sqIndex] != 0)
            //    {
            //        // write the number of empty squares (if any) and reset it
            //        if (emptySqNo != 0)
            //        {
            //            fen[indexFEN++] = DIGITS[emptySqNo];
            //            emptySqNo = 0;
            //        }

            //        fen[indexFEN++] = PIECE_CHARS[pieces[sqIndex]];// write piece char representation

            //    }
            //    // if the square is empty
            //    else
            //    {
            //        emptySqNo++;// increment the number of empty squares
            //    }
            //}

            //// write the number of empty squares (if any)
            //if (emptySqNo != 0)
            //{
            //    fen[indexFEN++] = DIGITS[emptySqNo];
            //}
            //fen[indexFEN++] = ' ';
            //fen[indexFEN++] = (WhichTurn == TURN_RED) ? 'r' : 'b';// write the side to move char
            //fen[indexFEN] = '\0';

            return "";// new string(fen, 0, indexFEN);
        }

    }
}
