using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ponder.ChineseChess;
using System.Diagnostics;

namespace Ponder.Engine
{
    /// <summary>
    /// 记录搜索节点的信息
    ///   (64)      | (16)  |  (8)   |   (3)      | (37) 
    /// board key   | score |  depth |  node type |  unused
    /// 还有BUG！！！不能用
    /// </summary>
    public struct NodeOfSearch
    {
        /// <summary>
        /// 结点在置换表中初始时的状态
        /// </summary>
        public const int EMPTY_NODES = 0;  

        /// <summary>
        /// 也叫EXACT结点，有准确的评估值，比较过这个结点下的所有子结点的值
        /// 这里的type 1, type 2和type 3是与这个文献的说法保持一致的
        /// http://chessprogramming.wikispaces.com/Node+Types#PV
        /// </summary>
        public const int PV_NODES = 1;     

        /// <summary>
        /// 也叫BETA结点，也叫Fail-High结点，该结点的子结点没有评估，直接被剪枝了
        /// 它的子结点的返回值肯定比beta要大
        /// </summary>
        public const int CUT_NODES = 2;   
 
        /// <summary>
        /// 也叫ALPHA结点，也叫Fail-Low结点
        /// 它的子结点的评估值比alpha要小
        /// </summary>
        public const int ALL_NODES = 3;    

        private static string[] NODE_TYPE_STRING = { "Empty-Nodes", "PV-Nodes", "Cut-Nodes", "All-Nodes" };

        public static readonly NodeOfSearch EmptyNode = new NodeOfSearch(0, 0, 0, 0);
        
        public ulong Key;
        public int Depth;
        public int NodeType;
        public int Score;


        public Move HashMove;



        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="depth"></param>
        /// <param name="type"></param>
        /// <param name="score"></param>
        public NodeOfSearch(ulong boardKey, int depth, int nodeType, int score)
            : this(boardKey, depth, nodeType, score, null)
        {
        }

        public NodeOfSearch(ulong boardKey, int depth, int nodeType, int score, Move hashMove)
        {
            this.Key = boardKey;
            this.Depth = depth;
            this.NodeType = nodeType;
            this.Score = score;
            this.HashMove = hashMove;
        }

        public void Encode(out UInt64 code1, out UInt64 code2)
        {
            code1 = Key;
            code2 = ((UInt64)Score << 48) | ((UInt64)Depth << 40) | ((UInt64)NodeType << 37);
        }

        public static NodeOfSearch Decode(UInt64 code1, UInt64 code2)
        {
            ulong key = code1;
            int score = (int)(code2 >> 48);
            int depth = (int)(code2>>40) & 0xFF;
            int nodeType = (int) (code2 >>37) & 0x7;
            return new NodeOfSearch(key, depth, nodeType, score);
        }

        public override string ToString()
        {
            return   Key + ", depth=" + Depth + ", type=" + NODE_TYPE_STRING[NodeType] + ", score=" + Score;
        }

    };


    /// <summary>
    /// 带转换表的搜索算法
    /// </summary>
    public class SearchTT
    {
        /// <summary>
        /// 可以给引擎发思考的细节
        /// </summary>
        private readonly PonderEngine engine;

        Stopwatch stopWatch;

        // 2013.10.09 想实现repetition detection
        public int Ply;
        public int []HalfmoveClock;
        public ulong []BoardHistory;
        public Move[] MoveHistory;

        /// <summary>
        /// 思考的盘面，初始化搜索时是根结点，在搜索时，这个变量记录的一直是正在搜索的局面
        /// </summary>
        private readonly Board board;

        /// <summary>
        /// 置换表
        /// </summary>
        private readonly TranspositionTable transpositionTable;

        /// <summary>
        /// 记录最好的着法
        /// </summary>
        //private Move bestMove;
        public Move BestMove
        {
            get
            {
                ulong boardHash = Zobrist.ZoristHash(board); 
                NodeOfSearch entry = transpositionTable.Probe(boardHash);
                if (entry.NodeType == NodeOfSearch.PV_NODES)
                {
                    return entry.HashMove;
                }
                return null;
            }
        }


        public long ElapseTime;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="board">盘面</param>
        /// <param name="transpositionTable">置换表</param>
        public SearchTT(Board board, TranspositionTable transpositionTable)
            : this(board, transpositionTable, null)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="board">盘面</param>
        /// <param name="transpositionTable">置换表</param>
        /// <param name="engine">引擎</param>
        public SearchTT(Board board, TranspositionTable transpositionTable, PonderEngine engine)
        {
            this.Ply = 0;
            this.board = board;
            this.transpositionTable = transpositionTable;
            //this.bestMove = null;
            this.engine = engine;
        }

        /// <summary>
        /// 打印调试信息
        /// </summary>
        /// <param name="info">要输出给引擎的信息串</param>
        private void PrintDebugInfo(string info)
        {
            if (engine != null)
                engine.PrintDebugInfo(info);
        }

        public int StartAlphaBetaTT(int depth, int alpha, int beta)
        {
            Ply = 0;
            HalfmoveClock = new int[1000];
            BoardHistory = new ulong[1000];
            MoveHistory = new Move[1000];
            stopWatch = new Stopwatch();
            stopWatch.Start();

            int score =  NegaAlphaBetaTT(depth, alpha, beta);
                
            stopWatch.Stop();
            ElapseTime = stopWatch.ElapsedMilliseconds;
            return score;
        }

        /**********************************************************
         带置换表的Alphabeta搜索算法的示例代码供参考： 
           int AlphaBeta(int depth, int alpha, int beta) { 
　             int hashf = hashfALPHA; 
　             if ((val = ProbeHash(depth, alpha, beta)) != valUNKNOWN) { 
　　                // 【valUNKNOWN必须小于-INFINITY或大于INFINITY，否则会跟评价值混淆。】 
　　                return val; 
　             } 
　             if (depth == 0) { 
　　                val = Evaluate(); 
　　                RecordHash(depth, val, hashfEXACT); 
　　                return val; 
　             } 
　             GenerateLegalMoves(); 
　             while (MovesLeft()) { 
　　                MakeNextMove(); 
　　                val = -AlphaBeta(depth - 1, -beta, -alpha); 
　　                UnmakeMove(); 
　　                if (val >= beta) { 
　　　                   RecordHash(depth, beta, hashfBETA); 
　　　                   return beta; 
　　                } 
　　                if (val > alpha) { 
　　　                   hashf = hashfEXACT; 
　　　                   alpha = val; 
　　                } 
　              } 
　              RecordHash(depth, alpha, hashf); 
　              return alpha; 
           } 
        *****************************************************************/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depth">最底部的叶子结点为第0层</param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        private int NegaAlphaBetaTT(int depth, int alpha, int beta)
        {
            ulong boardKey = board.ZobristKey;   // Zobrist.ZoristHash(board);
            BoardHistory[Ply] = boardKey;

            if (IsRepetition(boardKey, Ply, BoardHistory, HalfmoveClock[Ply]))
            {
                bool redRepCheck = true;
                bool blackRepCheck = true;
                Board b = board;
                for (int d = 0; d < HalfmoveClock[Ply]; d++)
                {
                    // TODO: 逻辑不合理的代码：关于将军的判断应该放在Board类中
                    MoveGenerator g = new MoveGenerator(b);
                    if (b.IsRedTurn)
                    {
                        if (redRepCheck && g.IsBlackKingSafe()) redRepCheck = false;
                    }
                    else
                    {
                        if (blackRepCheck && g.IsRedKingSafe()) blackRepCheck = false;
                    }
                    b.UnmakeMove(MoveHistory[Ply - d]);
                }

                if (redRepCheck && blackRepCheck) return 0; //双方都长将，平局分数
                if (redRepCheck) return -30000 + 100;
                if (blackRepCheck) return 30000 - 100;
                return 0; // 双方都是允许着法时
            }


            // 探查置换表
            //UInt64 code1, code2;
            NodeOfSearch entry = transpositionTable.Probe(boardKey);

            // (depth=2)            a 
            //                    /   \
            // (depth=1)   entry<b>     c       这里的b是已经搜索过的节点，已经保存在置换表中，entry.Depth = 1
            //                 / \      \
            // (depth=0)      d   e     [f]   (当前搜索的节点是f) depth = 0
            //              ..................
            // 假设b是以前搜索过的节点，已经保存在置换表中，entry.depth=1
            // 当前搜索的节点是f，depth=0，如果f与b的局面相同，由于b的评估值是经过了更深层的搜索得到的，
            // 所以f可以直接用b的评估值，这个结果只会好，不会差，所以应该判断entry.Depth >= depth
            if (entry.NodeType != NodeOfSearch.EMPTY_NODES && entry.Depth > depth)
            {
                //PrintDebugInfo("Hash表命中！" + entry);
                switch (entry.NodeType)
                {
                    case NodeOfSearch.PV_NODES:
                        return entry.Score;
                    case NodeOfSearch.CUT_NODES:
                        // -------------\         /-----------------\
                        // 评估值的范围 |         |  当前搜索窗口   |   
                        //--------------+---------+-----------------+--
                        //      entry.Score  <=  alpha            beta  
                        if (entry.Score <= alpha)  // 剪枝！
                        {  
                            return alpha;
                        }
                        //-------------------------\                
                        //        评估值的范围     |
                        //       /-----------------+---------------\       
                        //       |           当前搜| 索窗口        |   
                        //-------+-----------------+---------------+-------
                        //      alpha          entry.Score   <    beta
                        if (entry.Score < beta)  //调整beta即可
                        {  
                            beta = entry.Score;
                        }
                        break;
                    case NodeOfSearch.ALL_NODES:
                        //      /-----------------\      /-------------
                        //      |  当前搜索窗口   |      |评估值的范围     
                        //------+-----------------+------+--------------
                        //    alpha            beta <= entry.Score  
                        if (beta <= entry.Score)  // 剪枝！
                        { 
                            return beta;
                        }
                        //                         /-----------------------
                        //                         |     评估值的范围
                        //       /-----------------+---------------\       
                        //       |           当前搜| 索窗口        |   
                        //-------+-----------------+---------------+-------
                        //      alpha    <     entry.Score       beta
                        if (alpha < entry.Score)  // 此时只要调整alpha即可
                        { 
                            alpha = entry.Score;
                        }
                        break;
                }
            }

            // 到达叶子节点
            if (depth == 0)
            {
                int valueLeaf = Evaluator.EvaluateAllWithSide(board);
                // 应该肯定是EXACT节点吧？
                /* if(v0 <= alpha)
                    node.Type = NODE_ALPHA;
                else if(v0 >= beta)
                    node.Type = NODE_BETA;
                else  */
                NodeOfSearch nodeLeaf = new NodeOfSearch(boardKey, depth, NodeOfSearch.PV_NODES, valueLeaf);
                transpositionTable.RecordHash(nodeLeaf);
                //DEBUG(DBG_DEBUG, SPACES[depth] << "到达最大深度:" << " return score: "<< v0);

                return valueLeaf;
            }


            int nodeType = NodeOfSearch.ALL_NODES;
            Move[] moveList = new Move[200];
            int countMove = MoveGenerator.GenAllMoveList(board, moveList);

            // 无着可走，说明是终止局面，即被将死
            if (countMove == 0)
            {
                // TODO: 杀棋分数调整 http://www.xqbase.com/computer/stepbystep5.htm
                // (1) 对于RecordHash：置换表项记录的杀棋步数 = 实际杀棋步数 - 置换表项距离根节点的步数；
　　            // (2) 对于ProbeHash：实际杀棋步数 = 置换表项记录的杀棋步数 + 置换表项距离根节点的步数。
                int scoreEndStatus = Evaluator.MIN_EVAL_VALUE + Ply;
                NodeOfSearch nodeEnd = new NodeOfSearch(boardKey, depth, NodeOfSearch.PV_NODES, scoreEndStatus);
                transpositionTable.RecordHash(nodeEnd);
                return scoreEndStatus;
            }

            // 利用了置换表中的历史评估数据，进行着法排序
            //            局面"9/4a4/3k5/3N5/3N5/r8/9/9/9/4K4 w"
            // 用迭代加深算法来测试效果：迭代加深计算到第8层
            // DEBUG 
            // 不排序时1.7秒，探查并对着法排序时：17秒，代价很大
            // Release
            // 不排序时0.7秒，探查并对着法排序时:7秒

            // if(depth == 0)   
            //     SortMovelist(moveList, countMove);


            //int bestScore = Evaluator.MIN_EVAL_VALUE + depth;
            Move bestMove = null;
            for (int i = 0; i < countMove; i++)
            {
                ++Ply; // 胜利局面中需要这个变量， -MAX + ply
                MoveHistory[Ply] = moveList[i];
                HalfmoveClock[Ply] = moveList[i].Irreversible ? 0 : HalfmoveClock[Ply - 1] + 1;
                board.MakeMove(moveList[i]);
                int score = -NegaAlphaBetaTT(depth - 1, -beta, -alpha);
                board.UnmakeMove(moveList[i]);
                
                --Ply;
                //HalfmoveClock[Ply] = moveList[i].Irreversible ? 0 : HalfmoveClock[Ply - 1] + 1;

                // 这里负责记录最佳着法
                //if (score > bestScore)
                //{
                //    bestScore = score;
                //    if(depth == 0) bestMove = moveList[i];
                //}

                if (score >= beta)
                {
                    //PrintDebugInfo("发生剪枝！bestScore >= beta: " + bestScore + " >= " + beta);
                    // 这里记录refutation move
                    NodeOfSearch nodeBeta = new NodeOfSearch(boardKey, depth, NodeOfSearch.CUT_NODES, beta, moveList[i]);
                    transpositionTable.RecordHash(nodeBeta);
                    return beta;
                }
                if (score > alpha)
                {
                    // alpha = bestScore;  // alpha = score????
                    // 这时只是记录alpha值的变化情况，并不写置换表
                    nodeType = NodeOfSearch.PV_NODES;
                    alpha = score;
                    bestMove = moveList[i];
                    //PrintDebugInfo("修改alpha: " + alpha);
                }

            }

            NodeOfSearch node = new NodeOfSearch(boardKey, depth, nodeType, alpha, bestMove);
            transpositionTable.RecordHash(node);
            return alpha;
        }

        private static bool IsRepetition(ulong boardKey, int ply, ulong[] BoardHistory, int halfmoveClock)
        {
            if (halfmoveClock < 4) return false;
            for (int d = 4; d <= halfmoveClock; d += 2)
                if (boardKey == BoardHistory[ply - d])
                {
                    // TODO: 还要判断重复的具体情况，这里会涉及亚洲规则
                    return true;
                }
            return false;
        }

        public int StartPVS(int depth, int alpha, int beta)
        {
            Ply = 0;
            HalfmoveClock = new int[1000];
            BoardHistory = new ulong[1000];
            MoveHistory = new Move[1000];

            stopWatch = new Stopwatch();
            stopWatch.Start();
            int score = PVS(depth, alpha, beta);
            stopWatch.Stop();
            ElapseTime = stopWatch.ElapsedMilliseconds;
            return score;
        }

         /// <summary>
        /// 主要变例搜索
        /// </summary>
        /// <param name="depth">最底部的叶子结点为第0层</param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        private int PVS(int depth, int alpha, int beta)
        {

            bool haveFoundPV = false;
            

            ulong boardKey = board.ZobristKey;   // Zobrist.ZoristHash(board);
            BoardHistory[Ply] = boardKey;

            if (IsRepetition(boardKey, Ply, BoardHistory, HalfmoveClock[Ply]))
            {
                bool redRepCheck = true;
                bool blackRepCheck = true;
                Board b = new Board(board);
                for (int d = 0; d < HalfmoveClock[Ply]; d++)
                {
                    if (Ply - d == 0) break;
                    // TODO: 逻辑不合理的代码：关于将军的判断应该放在Board类中
                    MoveGenerator g = new MoveGenerator(b);
                    if (b.IsRedTurn)
                    {
                        if (redRepCheck && g.IsBlackKingSafe()) redRepCheck = false;
                    }
                    else
                    {
                        if (blackRepCheck && g.IsRedKingSafe()) blackRepCheck = false;
                    }
                    b.UnmakeMove(MoveHistory[Ply - d]);
                    if (board.ZobristKey == b.ZobristKey) break;
                }

                if (redRepCheck && blackRepCheck) return 0; //双方都长将，平局分数
                if (redRepCheck)
                {
                    return -30000 + 100;
                }
                if (blackRepCheck)
                {
                    return 30000 - 100;
                }
                return 0; // 双方都是允许着法时
            }


            // 探查置换表
            NodeOfSearch entry = transpositionTable.Probe(boardKey);

            // (depth=2)            a 
            //                    /   \
            // (depth=1)   entry<b>     c       这里的b是已经搜索过的节点，已经保存在置换表中，entry.Depth = 1
            //                 / \      \
            // (depth=0)      d   e     [f]   (当前搜索的节点是f) depth = 0
            //              ..................
            // 假设b是以前搜索过的节点，已经保存在置换表中，entry.depth=1
            // 当前搜索的节点是f，depth=0，如果f与b的局面相同，由于b的评估值是经过了更深层的搜索得到的，
            // 所以f可以直接用b的评估值，这个结果只会好，不会差，所以应该判断entry.Depth >= depth
            if (entry.NodeType != NodeOfSearch.EMPTY_NODES && entry.Depth > depth)
            {
                switch (entry.NodeType)
                {
                    case NodeOfSearch.PV_NODES:
                        return entry.Score;
                    case NodeOfSearch.CUT_NODES:
                        // -------------\         /-----------------\
                        // 评估值的范围 |         |  当前搜索窗口   |   
                        //--------------+---------+-----------------+--
                        //      entry.Score  <=  alpha            beta  
                        if (entry.Score <= alpha)  // 剪枝！
                        {  
                            return alpha;
                        }
                        //-------------------------\                
                        //        评估值的范围     |
                        //       /-----------------+---------------\       
                        //       |           当前搜| 索窗口        |   
                        //-------+-----------------+---------------+-------
                        //      alpha          entry.Score   <    beta
                        if (entry.Score < beta)  //调整beta即可
                        {  
                            beta = entry.Score;
                        }
                        break;
                    case NodeOfSearch.ALL_NODES:
                        //      /-----------------\      /-------------
                        //      |  当前搜索窗口   |      |评估值的范围     
                        //------+-----------------+------+--------------
                        //    alpha            beta <= entry.Score  
                        if (beta <= entry.Score)  // 剪枝！
                        { 
                            return beta;
                        }
                        //                         /-----------------------
                        //                         |     评估值的范围
                        //       /-----------------+---------------\       
                        //       |           当前搜| 索窗口        |   
                        //-------+-----------------+---------------+-------
                        //      alpha    <     entry.Score       beta
                        if (alpha < entry.Score)  // 此时只要调整alpha即可
                        { 
                            alpha = entry.Score;
                        }
                        break;
                }
            }

            // 到达叶子节点
            if (depth == 0)
            {
                int valueLeaf = Evaluator.EvaluateAllWithSide(board);
                NodeOfSearch nodeLeaf = new NodeOfSearch(boardKey, depth, NodeOfSearch.PV_NODES, valueLeaf);
                transpositionTable.RecordHash(nodeLeaf);
                return valueLeaf;
            }


            int nodeType = NodeOfSearch.ALL_NODES;
            Move[] moveList = new Move[200];
            int countMove = MoveGenerator.GenAllMoveList(board, moveList);

            // 无着可走，说明是终止局面，即被将死
            if (countMove == 0)
            {
                int scoreEndStatus = Evaluator.MIN_EVAL_VALUE + Ply;
                NodeOfSearch nodeEnd = new NodeOfSearch(boardKey, depth, NodeOfSearch.PV_NODES, scoreEndStatus);
                transpositionTable.RecordHash(nodeEnd);
                return scoreEndStatus;
            }

            // 利用了置换表中的历史评估数据，进行着法排序
            //            局面"9/4a4/3k5/3N5/3N5/r8/9/9/9/4K4 w"
            // 用迭代加深算法来测试效果：迭代加深计算到第8层
            // DEBUG 
            // 不排序时1.7秒，探查并对着法排序时：17秒，代价很大
            // Release
            // 不排序时0.7秒，探查并对着法排序时:7秒

            // if(depth == 0)   
            //     SortMovelist(moveList, countMove);

            Move bestMove = null;
            for (int i = 0; i < countMove; i++)
            {
                ++Ply; // 胜利局面中需要这个变量， -MAX + ply
                MoveHistory[Ply] = moveList[i];
                HalfmoveClock[Ply] = moveList[i].Irreversible ? 0 : HalfmoveClock[Ply - 1] + 1;
                board.MakeMove(moveList[i]);

                int score;
                if (haveFoundPV)
                {
                    score = -PVS(depth - 1, -alpha - 1, -alpha);
                    if ((score > alpha) && (score < beta))
                    { // 检查失败 
                        score = -PVS(depth - 1, -beta, -alpha);
                    }
                }
                else
                {
                    score = -PVS(depth - 1, -beta, -alpha);
                }
                board.UnmakeMove(moveList[i]);
                --Ply;

                if (score >= beta)
                {
                    //PrintDebugInfo("发生剪枝！bestScore >= beta: " + bestScore + " >= " + beta);
                    NodeOfSearch nodeBeta = new NodeOfSearch(boardKey, depth, NodeOfSearch.CUT_NODES, beta, moveList[i]);
                    transpositionTable.RecordHash(nodeBeta);
                    return beta;
                }
                if (score > alpha)
                {
                    // 这时只是记录alpha值的变化情况，并不写置换表
                    nodeType = NodeOfSearch.PV_NODES;
                    alpha = score;
                    bestMove = moveList[i];
                    //PrintDebugInfo("修改alpha: " + alpha);
                }

                if (engine != null && stopWatch.ElapsedMilliseconds > engine.timeLimit) break;

            }

            NodeOfSearch node = new NodeOfSearch(boardKey, depth, nodeType, alpha, bestMove);
            transpositionTable.RecordHash(node);
            return alpha;
        }
    
 

        ///// <summary>
        ///// 使用了置换表的AlphaBeta剪枝搜索
        ///// </summary>
        ///// <param name="depth">当前搜索的深度，0代表树根</param>
        ///// <param name="maxDepth">最大搜索深度</param>
        ///// <param name="alpha">初始值可以设置为MIN_EVAL_VALUE</param>
        ///// <param name="beta">初始值可以设置为MAX_EVAL_VALUE</param>
        ///// <returns></returns>
        //public int NegaAlphaBetaTT_old(int depth, int maxDepth, int alpha, int beta)
        //{
        //    //if (depth == 0) bestMove = new Move(0,0,0,0);

        //    ulong boardHash = Zobrist.ZoristHash(board);

        //    // 探查置换表
        //    NodeOfSearch entry = transpositionTable[boardHash]; //ProbeHash(boardHash);

        //    // (depth=0)            a 
        //    //                    /   \
        //    // (depth=1)        b      c       这里的b是已经搜索过的节点，已经保存在置换表中，entry.Depth = 1
        //    //                 / \      \
        //    // (depth=2)      d   e      f   (当前搜索的节点是f) depth = 2
        //    //              ..................
        //    // 假设b是以前搜索过的节点，已经保存在置换表中，entry.depth=1
        //    // 当前搜索的节点是f，depth=2，如果f与b的局面相同，由于b的评估值是经过了更深层的搜索得到的，
        //    // 所以f可以直接用b的评估值，这个结果只会好，不会差，所以应该判断entry.Depth <= depth
        //    if (entry != null && (entry.MaxDepth - entry.Depth) >= (maxDepth - depth)  )
        //    //if (entry != null && entry.Depth <= depth)
        //    {
        //        //PrintDebugInfo("Hash表命中！" + entry);

        //        if (entry.Type == NodeOfSearch.PV_NODES)
        //        {
        //            return entry.Score;
        //        }

        //        if (entry.Type == NodeOfSearch.ALL_NODES && entry.Score <= alpha)
        //        {
        //            //PrintDebugInfo("Hash表命中！修改alpha：" + entry.Score + " -> " + alpha);
        //            alpha = entry.Score;
        //            //return alpha;
        //        }
        //        if (entry.Type == NodeOfSearch.CUT_NODES && entry.Score >= beta)
        //        {
        //            //PrintDebugInfo("Hash表命中！修改beta：" + entry.Score + " -> " + beta);
        //            beta = entry.Score;
        //            //return beta;
        //        }

        //        //if (alpha >= beta)
        //        //{
        //        //    PrintDebugInfo("***************这里还会发生alpha>=beta？");
        //        //    //return entry.Score;
        //        //}

        //    }

        //    // 到达叶子节点，搜索到最大深度了
        //    if (depth == maxDepth)
        //    {
        //        int valueLeaf = Evaluator.EvaluateAllWithSide(board);
        //        // 应该肯定是EXACT节点吧？
        //        /* if(v0 <= alpha)
        //            node.Type = NODE_ALPHA;
        //        else if(v0 >= beta)
        //            node.Type = NODE_BETA;
        //        else  */
        //        NodeOfSearch nodeLeaf = new NodeOfSearch(boardHash, depth, maxDepth, NodeOfSearch.PV_NODES, valueLeaf);
        //        transpositionTable.RecordHash(nodeLeaf);
        //        //DEBUG(DBG_DEBUG, SPACES[depth] << "到达最大深度:" << " return score: "<< v0);

        //        return valueLeaf;
        //    }


        //    int nodeType = NodeOfSearch.ALL_NODES;
        //    Move[] moveList = new Move[200];
        //    int countMove = MoveGenerator.GenAllMoveList(board, moveList); 

        //    // 无着可走，说明是终止局面，即被将死
        //    if (countMove == 0)
        //    {
        //        int scoreEndStatus = Evaluator.MIN_EVAL_VALUE + depth;
        //        NodeOfSearch nodeEnd = new NodeOfSearch(boardHash, depth, maxDepth, NodeOfSearch.PV_NODES, scoreEndStatus);
        //        transpositionTable.RecordHash(nodeEnd);
        //        return scoreEndStatus;
        //    }

        //    // 利用了置换表中的历史评估数据，进行着法排序
        //    //            局面"9/4a4/3k5/3N5/3N5/r8/9/9/9/4K4 w"
        //    // 用迭代加深算法来测试效果：迭代加深计算到第8层
        //    // DEBUG 
        //    // 不排序时1.7秒，探查并对着法排序时：17秒，代价很大
        //    // Release
        //    // 不排序时0.7秒，探查并对着法排序时:7秒

        //    // if(depth == 0)   
        //    //     SortMovelist(moveList, countMove);


        //    //int bestScore = Evaluator.MIN_EVAL_VALUE + depth;

        //    for (int i = 0; i < countMove; i++)
        //    {
        //        board.MakeMove(moveList[i]);
        //        int score = -NegaAlphaBetaTT_old(depth + 1, maxDepth, -beta, -alpha);
        //        board.UnmakeMove(moveList[i]);

        //        // 这里负责记录最佳着法
        //        //if (score > bestScore)
        //        //{
        //        //    bestScore = score;
        //        //    if(depth == 0) bestMove = moveList[i];
        //        //}

        //        if (score >= beta)
        //        {
        //            //PrintDebugInfo("发生剪枝！bestScore >= beta: " + bestScore + " >= " + beta);
        //            // 应该记录该节点！！
        //            NodeOfSearch nodeBeta = new NodeOfSearch(boardHash, depth, maxDepth, NodeOfSearch.CUT_NODES, beta);
        //            transpositionTable.RecordHash(nodeBeta);
        //            return beta;// score;
        //        }
        //        if (score > alpha)
        //        {
        //           // alpha = bestScore;  // alpha = score????
        //            // 这时只是记录alpha值的变化情况，并不写置换表
        //            nodeType = NodeOfSearch.PV_NODES;
        //            alpha = score;
        //            //PrintDebugInfo("修改alpha: " + alpha);
        //        }

        //    }

        //    //int type = 
        //    //    (bestScore <= alpha) ?  NodeOfSearch.NODE_TYPE_ALPHA 
        //    //    : (bestScore >= beta) ? NodeOfSearch.NODE_TYPE_BETA 
        //    //    : NodeOfSearch.NODE_TYPE_EXACT;
        //    //NodeOfSearch node = new NodeOfSearch(boardHash, depth, maxDepth, type, bestScore);
        //    NodeOfSearch node = new NodeOfSearch(boardHash, depth, maxDepth, nodeType, alpha);
        //    transpositionTable.RecordHash(node);
        //    //return bestScore;
        //    return alpha;
        //}

        /// <summary>
        /// 试验数据，对于局面"9/4a4/3k5/3N5/3N5/r8/9/9/9/4K4 w"
        // 用迭代加深算法来测试效果：迭代加深计算到第8层
        // DEBUG 
        // 不排序时1.7秒，探查并对着法排序时：17秒，代价很大
        // Release
        // 不排序时0.7秒，探查并对着法排序时:7秒
        /// </summary>
        /// <param name="moveList"></param>
        /// <param name="countMove"></param>
        private void SortMovelist(Move[] moveList, int countMove)
        {
            for (int i = 0; i < countMove; i++)
            {
                board.MakeMove(moveList[i]);
                ulong boardHash = Zobrist.ZoristHash(board);

                // 探查置换表
                //NodeOfSearch entry = transpositionTable.Probe(boardHash); 
                //if (entry != null)
                //{
                //    moveList[i].Score = entry.Score;
                //}
                //else
                //    moveList[i].Score = 0;

                board.UnmakeMove(moveList[i]);
            }
            Array.Sort(moveList, 0, countMove);
        }


        /*! 迭代加深搜索，调用了带置换表的AlphaBeta搜索算法。
        * \param maxDepth 最大搜索深度
        * \return 评估值
        */
        public int IterativeDeepeningSearch(int maxDepth)
        {
            int v = 0;
            HalfmoveClock = new int[1000];
            BoardHistory = new ulong[1000];
            MoveHistory = new Move[1000];

            stopWatch = new Stopwatch();
            stopWatch.Start();

            for (int d = 1; d <= maxDepth; d++)
            {
                Ply = 0;
                //v = NegaAlphaBetaTT(d, Evaluator.MIN_EVAL_VALUE, Evaluator.MAX_EVAL_VALUE);
                v = PVS(d, Evaluator.MIN_EVAL_VALUE, Evaluator.MAX_EVAL_VALUE);
                //在置换表记录了depth和maxDepth，可以准确地更新置换表中的结点了，就不需要在每次搜索前清空置换表了！
                //transpositionTable.Reset();
            }
            stopWatch.Stop();
            ElapseTime = stopWatch.ElapsedMilliseconds;
            return v;
        }







    }
}
