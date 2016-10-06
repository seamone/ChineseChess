using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Ponder.ChineseChess
{
    /// <summary>
    /// 与预置表有关的，主要为了着法生成的效率
    /// </summary>
    public  partial class MoveGenerator
    {
        public static readonly int[][] PRESET_KING = new int[Board.TOTAL_POS][];
        public static readonly int[][] PRESET_BISHOP_ADVISOR = new int[Board.TOTAL_POS][];
        public static readonly int[][] PRESET_RED_PAWN = new int[Board.TOTAL_POS][];
        public static readonly int[][] PRESET_BLACK_PAWN = new int[Board.TOTAL_POS][];
        public static readonly int[][] PRESET_KNIGHT = new int[Board.TOTAL_POS][];

        private static void InitAllPreset()
        {
            InitPresetKing();
            InitPresetBishopAdvisor();
            InitPresetRedPawn();
            InitPresetBlackPawn();
            InitPresetKnight();
        }

        private static void InitPresetKnight()
        {
            int[] inc = { -35, -33, -19, -15, 15, 19, +33, +35 };
            for (int from = 0; from < Board.TOTAL_POS; from++)
            {
                if (Board.IsInBoard(from))
                {
                    ArrayList toList = new ArrayList();
                    for (int j = 0; j < inc.Length; j++)
                        if (Board.IsInBoard(from + inc[j]))
                        {
                            toList.Add(from + inc[j]);
                        }
                    PRESET_KNIGHT[from] = (int[])toList.ToArray(typeof(int));
                }
            }
        }

        private static void InitPresetRedPawn()
        {
            int[] inc = { -17, -1, +1 };
            for (int from = 0; from < Board.TOTAL_POS; from++)
            {

                if (Board.IsInRedSide(from)) // 未过河，只能前进
                {
                    PRESET_RED_PAWN[from] = new int[] { from + inc[0] };
                }
                else if (Board.IsInBlackSide(from)) //过河了
                {
                    ArrayList toList = new ArrayList();
                    for (int j = 0; j < inc.Length; j++)
                        if (Board.IsInBoard(from + inc[j]))
                        {
                            toList.Add(from + inc[j]);
                        }
                    PRESET_RED_PAWN[from] = (int[])toList.ToArray(typeof(int));
                }
            }
        }


        private static void InitPresetBlackPawn()
        {
            int[] inc = { +17, -1, +1 };
            for (int from = 0; from < Board.TOTAL_POS; from++)
            {

                if (Board.IsInBlackSide(from)) // 未过河，只能前进
                {
                    PRESET_BLACK_PAWN[from] = new int[] { from + inc[0] };
                }
                else if (Board.IsInRedSide(from)) //过河了
                {
                    ArrayList toList = new ArrayList();
                    for (int j = 0; j < inc.Length; j++)
                        if (Board.IsInBoard(from + inc[j]))
                        {
                            toList.Add(from + inc[j]);
                        }
                    PRESET_BLACK_PAWN[from] = (int[])toList.ToArray(typeof(int));
                }
            }
        }
        private static void InitPresetKing()
        {
            int[] inc = { -17, -1, +1, +17 };
            for (int from = 0; from < Board.TOTAL_POS; from++)
            {
                if (Board.IsInRedPalace(from))
                {
                    ArrayList toList = new ArrayList();
                    for (int j = 0; j < inc.Length; j++)
                        if (Board.IsInRedPalace(from + inc[j]))
                        {
                            toList.Add(from + inc[j]);
                        }
                    PRESET_KING[from] = (int[])toList.ToArray(typeof(int));
                }
                if (Board.IsInBlackPalace(from))
                {
                    ArrayList toList = new ArrayList();
                    for (int j = 0; j < inc.Length; j++)
                        if (Board.IsInBlackPalace(from + inc[j]))
                        {
                            toList.Add(from + inc[j]);
                        }
                    PRESET_KING[from] = (int[])toList.ToArray(typeof(int));
                }
            }
        }

        private static void InitPresetBishopAdvisor()
        {
            for (int from = 0; from < Board.TOTAL_POS; from++)
            {
                int[] incBishop = { -36, -32, +32, +36 };
                if (Board.IsInRedBishopPlace(from))  // 红相
                {
                    ArrayList toList = new ArrayList();
                    for (int j = 0; j < incBishop.Length; j++)
                        if (Board.IsInRedSide(from + incBishop[j]))
                        {
                            toList.Add(from + incBishop[j]);
                        }
                    PRESET_BISHOP_ADVISOR[from] = (int[])toList.ToArray(typeof(int));
                }

                if (Board.IsInBlackBishopPlace(from))  // 黑象
                {
                    ArrayList toList = new ArrayList();
                    for (int j = 0; j < incBishop.Length; j++)
                        if (Board.IsInBlackSide(from + incBishop[j]))
                        {
                            toList.Add(from + incBishop[j]);
                        }
                    PRESET_BISHOP_ADVISOR[from] = (int[])toList.ToArray(typeof(int));
                }

                int[] incAdvisor = { -18, -16, +16, +18 };
                if (Board.IsInRedAdvisorPlace(from))  // 红仕
                {
                    ArrayList toList = new ArrayList();
                    for (int j = 0; j < incAdvisor.Length; j++)
                        if (Board.IsInRedAdvisorPlace(from + incAdvisor[j]))
                        {
                            toList.Add(from + incAdvisor[j]);
                        }
                    PRESET_BISHOP_ADVISOR[from] = (int[])toList.ToArray(typeof(int));
                }
                if (Board.IsInBlackAdvisorPlace(from))  // 黑士
                {
                    ArrayList toList = new ArrayList();
                    for (int j = 0; j < incAdvisor.Length; j++)
                        if (Board.IsInBlackAdvisorPlace(from + incAdvisor[j]))
                        {
                            toList.Add(from + incAdvisor[j]);
                        }
                    PRESET_BISHOP_ADVISOR[from] = (int[])toList.ToArray(typeof(int));
                }

            }
        }

        

    }
}
