using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ponder.ChineseChess;

namespace Ponder.Engine
{
    /// <summary>
    /// 简单搜索算法，不用置换表，这些算法用来理论试验，不会用于实战中
    /// </summary>
    public class SimpleSearch
    {
        /// <summary>
        /// 搜索开始时的初始盘面
        /// </summary>
        private Board board;

        private bool wantStop;

        private readonly PonderEngine engine;
        public  int MaxSearchDepth;
        private Move bestMove;

        public Move BestMove
        {
            get { return bestMove; }
        }

        /// <summary>
        /// 构造函数，需要指定搜索开始时的初始局面
        /// </summary>
        /// <param name="board">初始盘面</param>
        public SimpleSearch(Board board)
        {
            this.board = board;
            wantStop = false;
            //bestMove = null;
            engine = null;
        }

        public SimpleSearch(Board board, PonderEngine engine)
        {
            this.board = board;
            wantStop = false;
            //bestMove = null;
            this.engine = engine;
        }

        public int StartMinimax(int depth)
        {
            MaxSearchDepth = depth;
            return MiniMax(depth);
        }

        public int StartNegaMax(int depth)
        {
            MaxSearchDepth = depth;
            return NegaMax(depth);
        }

        public int StartAlphaBeta(int depth, int alpha, int beta)
        {
            MaxSearchDepth = depth;
            return AlphaBeta(depth, alpha, beta);
        }

        public int StartNegaAlphaBeta(int depth, int alpha, int beta)
        {
            MaxSearchDepth = depth;
            return NegaAlphaBeta(depth, alpha, beta);
        }

        /// <summary>
        /// 极小极大值搜索
        /// 这种搜索拿到实战中是不可行的，只是理论上的一个实验，用C++语言计算5层都要用42秒。
        /// </summary>
        /// <param name="depth">当前搜索深度</param>
        /// <param name="maxDepth">最大搜索深度</param>
        /// <returns>评估值</returns>
        private int MiniMax(int depth)
        {
            /* 要进行将死的判断
            if (board.BlackTurn && board.IsRedKingCheckmated())
            {
            return int.MinValue;
            }
            else if (board.RedTurn && board.IsBlackKingCheckmated())
            {
            return int.MaxValue;
            }
            */

            if (depth == 0) return Evaluator.EvaluateAll(board);

            int bestScore = board.IsRedTurn ?
                Evaluator.MIN_EVAL_VALUE + (MaxSearchDepth - depth) 
                : Evaluator.MAX_EVAL_VALUE - (MaxSearchDepth- depth);

            Move[] moveList = new Move[200];

            // 在终止局面时，即被将死的局面时，这个countMove返回0
            int countMove = MoveGenerator.GenAllMoveList(board,moveList); 

            for (int i = 0; i < countMove; i++)
            {
                board.MakeMove(moveList[i]);
                int score = MiniMax(depth - 1);
                board.UnmakeMove(moveList[i]);
                if (board.IsRedTurn)
                    bestScore = (score > bestScore) ? score : bestScore;
                else
                    bestScore = (score < bestScore) ? score : bestScore;
            }
            return bestScore;
        }




        /*! 负极大值搜索，与Minimax算法相比，算法描述更简洁一些
        * 这种搜索拿到实战中是不可行的，只是理论上的一个实验，计算5层都要用42秒。
        * \param depth 搜索深度
        * \return 评估值
        */
        private int NegaMax(int depth)
        {
            /* 在着法生成中有将军的判断，这里就不需要再进行判断了。否则还要进行终止局面的判断。
            * 是不是还有其它终止局面？现在还不得知。
            */

            if (depth == 0) return Evaluator.EvaluateAllWithSide(board);

            int bestScore = Evaluator.MIN_EVAL_VALUE + (MaxSearchDepth - depth);  //这种写法可以搜索深度最短的杀着

            // 着法生成中要进行将军的判断，也就是轮到红方走棋时，红方的走完后，帅不能被将军
            // 在终止局面时，即被将死的局面时，这个countMove返回0    
            Move[] moveList = new Move[200];
            int countMoves = MoveGenerator.GenAllMoveList(board,moveList);
            for (int i = 0; i < countMoves; i++)
            {
                board.MakeMove(moveList[i]);
                int score = -NegaMax(depth - 1);
                board.UnmakeMove(moveList[i]);
                bestScore = (score > bestScore) ? score : bestScore;
            }
            return bestScore;
        }


        /// <summary>
        /// 最简单的AlphaBeta剪枝搜索，没有用置换表。
        /// 比MinMax算法要好多了，C++64位计算5层只需0.8秒，6层为6.8秒，7层为151秒。
        /// C#版本5层要18秒
        /// 把搜索到的最好的着法保存在BestMove中。
        /// TODO: 这里没有检测胜利局面
        /// </summary>
        /// <param name="depth">当前搜索深度</param>
        /// <param name="maxDepth">最大搜索深度</param>
        /// <param name="alpha">初始值可以设置为MIN_EVAL_VALUE</param>
        /// <param name="beta">初始值可以设置为MAX_EVAL_VALUE</param>
        /// <returns>评估值</returns>
        private int AlphaBeta(int depth, int alpha, int beta)
        {
            /* 要进行被将死情况的检测
            if (board.BlackTurn && board.IsRedKingCheckmated())
            {
            return int.MinValue;
            }
            else if (board.RedTurn && board.IsBlackKingCheckmated())
            {
            return int.MaxValue;
            }
            */

            if (depth == 0) return Evaluator.EvaluateAll(board);
            Move[] moveList = new Move[200];
            int n_moves = MoveGenerator.GenAllMoveList(board,moveList);

            if (!board.IsRedTurn)
            {
                for (int i = 0; i < n_moves; i++)
                {
                    board.MakeMove(moveList[i]);
                    int score = AlphaBeta(depth - 1, alpha, beta);
                    board.UnmakeMove(moveList[i]);
                    if (score < beta)
                    {
                        beta = score;
                        if (depth == 0)
                            bestMove = moveList[i];
                        if (alpha >= beta)
                        {
                            return alpha;
                        }
                    }
                }
                return beta;
            }
            else
            {
                for (int i = 0; i < n_moves; i++)
                {
                    board.MakeMove(moveList[i]);
                    int score = AlphaBeta(depth - 1, alpha, beta);
                    board.UnmakeMove(moveList[i]);
                    if (score > alpha)
                    {
                        alpha = score;
                        if (depth == 0)
                            bestMove = moveList[i];
                        if (alpha >= beta)
                        {
                            return beta;
                        }
                    }
                }
                return alpha;
            }
        }

  
        /// <summary>
        /// 最简单的AlphaBeta剪枝搜索，没有用置换表。
        /// 比MinMax算法要好多了，C++64位计算5层只需0.8秒，6层为6.8秒，7层为151秒。
        /// C#版本5层要10秒
        /// 把搜索到的最好的着法保存在BestMove中。
        /// TODO: 如何能够结束计算？？？
        /// </summary>
        /// <param name="depth">当前搜索深度</param>
        /// <param name="alpha">初始值可以设置为MIN_EVAL_VALUE</param>
        /// <param name="beta">初始值可以设置为MAX_EVAL_VALUE</param>
        /// <returns>评估值</returns>      
        private int NegaAlphaBeta(int depth, int alpha, int beta)
        {
            /* 要进行被将死情况的检测
            if (board.BlackTurn && board.IsRedKingCheckmated())
            {
            return int.MinValue;
            }
            else if (board.RedTurn && board.IsBlackKingCheckmated())
            {
            return int.MaxValue;
            }
            */

            if (depth == 0) return Evaluator.EvaluateAllWithSide(board);
            Move[] moveList = new Move[200];

            int countMove = MoveGenerator.GenAllMoveList(board,moveList);
            if (countMove == 0 && depth == 0) bestMove = new Move(0,0,0,0);

            int bestScore = Evaluator.MIN_EVAL_VALUE + (MaxSearchDepth- depth);


            for (int i = 0; i < countMove; i++)
            {
                board.MakeMove(moveList[i]);
                int max_alpha_bestscore = (alpha > bestScore) ? alpha : bestScore;
                int score = -NegaAlphaBeta(depth - 1, -beta, -max_alpha_bestscore);
                board.UnmakeMove(moveList[i]);
                if (depth == 0 && engine != null)
                    engine.Output.WriteLine("info depth 0 score " + score + " pv " + moveList[i]);
                if (score > bestScore)
                {
                    bestScore = score;
                    if (depth == 0) 
                        bestMove = moveList[i];
                    if (bestScore >= beta)
                    {
                        return bestScore;
                    }
                }
            }
            return bestScore;
        }


        

    }
}
