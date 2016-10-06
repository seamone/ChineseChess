using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Ponder.ChineseChess;
using BusinessObjects.Ponder;
using BussinessObjects;

namespace Ponder.Engine
{
    /// <summary>
    /// UCCI引擎在启动后，有三种状态
    /// </summary>
    public enum EngineStatus
    {
        /// <summary>
        /// 为了编程一致性，加了这个Quit状态，方便退出引擎
        /// </summary>
        Quit,

        /// <summary>
        /// (1) 引导状态。
        ///引擎启动时，即进入引导状态。此时引擎只是等待和捕捉界面的输入，
        ///而界面必须用ucci指令让引擎进入接收其他UCCI指令的空闲状态。
        ///当然，引擎也可以保留使用其他协议的权利，
        ///例如引擎允许第一条有效指令是cxboard，这样引擎就转而进入CXBoard状态。
        ///收到ucci只后，引擎要完成一系列初始化工作，以输出ucciok的反馈作为初始化结束的标志，进入空闲状态。
        ///如果引导状态下UCCI引擎收到其他指令，则可以退出。
        /// </summary>
        Boot,

        /// <summary>
        /// (2) 空闲状态。
        ///该状态下引擎没有思考(即几乎不占用CPU资源)，而只是等待和捕捉界面的输入(和引导状态类似)，
        ///接收这样几类指令：
        ///A. 设置引擎选项(setoption指令)，
        ///B. 设置引擎的内置局面(即让引擎思考的局面)及其禁止着法(position和banmoves指令)，
        ///C. 让引擎思考(go指令)，
        ///D. 退出(quit指令)。
        /// </summary>
        Idle,

        /// <summary>
        /// (3) 思考状态。
        ///引擎收到go指令后，即进入思考状态，以输出bestmove或nobestmove的反馈作为思考状态结束的标志(回到空闲状态)。
        ///该状态下引擎将满负荷运转(CPU资源占用率接近100%)，但仍旧需要捕捉界面的输入(只有在批处理模式下不会捕捉界面的输入)，
        ///接收两类指令：
        ///A. 中止思考(stop指令)，
        ///B. 改变思考方式(ponderhit指令)。
        ///go指令只决定了引擎将按照什么思考方式来思考(即限定思考的深度，或限定思考的局面个数，
        ///或限定思考的时间)，而思考的局面则必须通过前面输入的position指令来告诉引擎。
        /// </summary>
        Thinking
    }

    /// <summary>
    /// 接收ucci命令，处理后反馈
    /// </summary>
    public class PonderEngine
    {
        private string openingBookFile;

        public string OpeningBookFile
        {
            get { return openingBookFile; }
        }

        private EngineStatus status;

        private TranspositionTable transpositionTable;

        private bool useBook;
        private bool isDebug;
        private string fen;
        private string moves;
        private LogHandler m_LogHandler;
        public long timeLimit;
        private string info;
        private string m_OutputMessage;
        private string inputMesaage = "";

          /// <summary>
        /// 构造函数
        /// 各种信息都输出到Console.Out
        /// </summary>
        public PonderEngine() : this(Console.Out)
        {
            string sPath = System.Environment.CurrentDirectory;
            string name = "EngineLog";
            m_LogHandler = new LogHandler(sPath, name);
        }


        public PonderEngine(System.IO.TextWriter _output)
        {
            this._output = _output;
            useBook = false;
#if DEBUG
            isDebug = true;
#else
            isDebug = false;
#endif
            openingBookFile = null;
            status = EngineStatus.Boot;
            //TODO: ！！！时间限制
            timeLimit = 60000;
            
        }

        private TextWriter _output;
        public System.IO.TextWriter Output
        {
            get { return _output; }
            set { _output = value; }
        }

        public void PrintDebugInfo(string strInfo)
        {
            // 调试模式中打印更多的信息
            if (this.isDebug)
                _output.WriteLine("info message " + strInfo);
        }

        public EngineStatus ExecuteCommand(string strCommand)
        {
            PrintDebugInfo("receive command：" + strCommand);

            UcciCommand cmd = UcciCommand.Parse(strCommand);

            if (status == EngineStatus.Boot)
            {
                if (cmd.Name == "ucci")
                {
                    //引导状态的反馈。显示引擎的版本号、版权、作者和授权用户，例如：
                    //id name ElephantEye 1.6 Beta，说明引擎的版本号
                    //id copyright 2004-2006 www.xqbase.com，说明引擎的版权
                    //id author Morning Yellow，说明引擎的作者
                    //id user ElephantEye Test Team，说明引擎授权给用户
                    info = "id name PonderEngine " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                    m_LogHandler.logging(info);
                    Output.WriteLine(info);

                    info = "id copyright 2013 by slb";
                    m_LogHandler.logging(info);
                    Output.WriteLine(info);

                    info = "id author shen longbin";
                    m_LogHandler.logging(info);
                    Output.WriteLine(info);

                    info = "id user slb test";
                    m_LogHandler.logging(info);
                    Output.WriteLine(info);

                    // TODO: 给出支持option选项
                    // option <选项> type <类型> [min <最小值>] [max <最大值>] 
                    // [var <可选项> [var <可选项> [...]]] [default <默认值>]
                    //引导状态的反馈。显示引擎所支持的选项，<option>指选项的名称，
                    //选项的类型是label(标签，非选项)、button(指令)、check(是或非)、combo(多选项)、
                    //spin(整数)、string(字符串)中的一种。
                    //option usemillisec type check default false 
                    //option usebook type check default true 
                    //引导状态的反馈，此后引擎进入空闲状态。
                    info = "option usebook type check default true";
                    m_LogHandler.logging(info);
                    Output.WriteLine(info);


                    info = "option bookfiles type string default " + "opening.ob";
                    m_LogHandler.logging(info);
                    Output.WriteLine(info);

                    // 这里完成一些初始化的任务
                    transpositionTable = new TranspositionTable();

                    info = "ucciok";
                    m_LogHandler.logging(info);
                    Output.WriteLine(info);

                    status = EngineStatus.Idle;
                }
                else
                {
                    //如果引导状态下UCCI引擎收到其他指令，则可以退出。
                    info = "quited directly";
                    m_LogHandler.logging(info);
                    Output.WriteLine(info);
                    return EngineStatus.Quit;
                }
            }
            else if (status == EngineStatus.Idle)
            {
                //isready
                //空闲状态和思考状态的指令。检测引擎是否处于就绪状态，其反馈总是readyok，该指令仅仅用来检测引擎是否能够正常接收指令。
                if (cmd.Name == "isready")
                {
                    //空闲状态和思考状态的反馈。表明引擎处于就绪状态(可正常接收指令)。
                    Output.WriteLine("readyok");
                }
                else if (cmd.Name == "setoption")
                {
                    ExecuteSetoptionCommand(cmd);
                }
                else if (cmd.Name == "position")
                {
                    ExecutePositionCommand(cmd);
                }
                else if (cmd.Name == "banmoves")
                {
                    // banmoves <禁止着法列表>
                    // 空闲状态的指令。为当前局面设置禁手，以解决引擎无法处理的长打问题。当出现长打局面时，
                    // 棋手可以操控界面向引擎发出禁手指令。例如：
                    // position fen 1r2kab1r/2c1a4/n1c1b1n2/4p2N1/p1p6/1C4P2/P1P1P4/2N1B3C/4A4/1RBAK2R1 w - - 0 1 moves h6i4 i9h9 i4h6 h9i9
                    // banmoves h6i4
                    // 本例取自《象棋竞赛规则》(1999年版)棋例图三，由于大多数象棋引擎无法识别红方这种方式的长捉，
                    // 所以在采用中国象棋协会的比赛规则时，遇到这种局面就必须给引擎发出禁手指令。
                    // 下一次发送position指令后，前面设置过的禁止着法就取消了，需要重新设置禁止着法。
                    // 目前UCCI界面《象棋巫师》不识别长打禁手，所以不会向引擎发送banmoves指令。
                    /******************   估计永远也不会实现了  ************************/
                }
                else if (cmd.Name == "go")
                {
                    // go [ponder | draw] <思考模式>
                    // 空闲状态的指令，此后引擎进入思考状态。让引擎根据position指令设定的棋盘来思考，
                    // 各选项为思考方式，有三种模式可供选择：
                    // (1) depth <深度> | infinite：
                    // 限定搜索深度，infinite表示无限制思考(直到找到杀棋或用stop指令中止)。
                    // 如果深度设定为0，那么引擎可以只列出当前局面静态评价的分数，并且反馈nobestmove。
                    // (2) nodes <结点数>：限定搜索结点数。
                    // (3) time <时间> [movestogo <剩余步数> | increment <每步加时>] 
                    // [opptime <对方时间> [oppmovestogo <对方剩余步数> | oppincrement <对方每步加时>]]
                    // 限定时间，时间单位是秒(默认)或毫秒(启用毫秒制时)，movestogo适用于时段制，increment适用于加时制。
                    // opptime、oppmovestogo和oppincrement可以让界面把对方的用时情况告诉引擎。
                    // 例如：go time 120 movestogo 120 opptime 120 oppmovestogo 120
                    // -------
                    // 如果指定ponder选项，则引擎思考时时钟不走，直到接受到ponderhit指令后才计时，该选项用于后台思考，
                    // 它只对限定时间的思考模式有效。
                    // 指定draw选项表示向引擎提和，引擎以bestmove提供的选项作为反馈，参阅bestmove指令。
                    // 注意：ponder和draw选项不能同时使用，如果界面向正在后台思考中的引擎求和，
                    // 则使用ponderhit draw指令。

                    status = EngineStatus.Thinking;


                    Board board = new Board(fen);
                    if (moves != null && moves != "")
                        board.MakeMoves(moves);

                    if (useBook)
                    {
                        Move bestInOpening = OpeningBook.BestMove(openingBookFile, board);
                        if (bestInOpening.From != 0)
                        {
                            PrintDebugInfo("命中开局库！");
                            Output.WriteLine("bestmove " + bestInOpening);
                            status = EngineStatus.Idle;
                            return status;
                        }
                    }

                    int idxDepth = cmd.Paras.IndexOf("depth");
                    int searchDepth = 4;
                    if (idxDepth >= 0)
                        searchDepth = int.Parse(cmd.Paras.Substring(idxDepth + 5));

                    //SimpleSearch search = new SimpleSearch(board, this);
                    //search.NegaAlphaBeta(0, searchDepth, Evaluator.MIN_EVAL_VALUE, Evaluator.MAX_EVAL_VALUE);
                    SearchTT search = new SearchTT(board, transpositionTable, this);

                    //search.NegaAlphaBetaTT_old(0, searchDepth, Evaluator.MIN_EVAL_VALUE, Evaluator.MAX_EVAL_VALUE);
                    //search.StartPVS(searchDepth, Evaluator.MIN_EVAL_VALUE, Evaluator.MAX_EVAL_VALUE);
                    search.IterativeDeepeningSearch(searchDepth);
                    //search.StartAlphaBetaTT(searchDepth, Evaluator.MIN_EVAL_VALUE, Evaluator.MAX_EVAL_VALUE);
                    /*************
                     * bestmove <最佳着法> [ponder <后台思考的猜测着法>] [draw | resign]
                     * 思考状态的反馈，此后引擎返回空闲状态。显示思考结果，即引擎认为在当前局面下的最佳着法，
                     * 以及猜测在这个着法后对手会有怎样的应对(即后台思考的猜测着法)。
                     * 通常，最佳着法是思考路线(主要变例)中的第一个着法，而后台思考的猜测着法
                     * 则是第二个着法。
                     * 在对手尚未落子时，可以根据该着法来设定局面，并作后台思考。
                     * 当对手走出的着法和后台思考的猜测着法吻合时，称为“后台思考命中”。
                     * draw选项表示引擎提和或者接受界面向引擎发送的提和请求，参阅go draw和ponderhit draw指令。
                     * resign选项表示引擎认输。UCCI界面在人机对弈方式下，根据不同情况，
                     * 可以对引擎的bestmove反馈中的draw和resign选项作出相应的处理：
                     * (1) 如果用户提和，界面向引擎发出go draw或ponderhit draw指令，而引擎反馈带draw的bestmove，
                     * 那么界面可终止对局并判议和；
                     * (2) 如果用户没有提和，而引擎反馈带draw的bestmove，那么界面可向用户提和，
                     * 用户接受提和则可终止对局并判议和；
                     * (3) 如果引擎反馈带resign的bestmove，那么界面可终止对局并判引擎认输。
                     * 引擎应该根据当前局面的情况(由position指令给出)，以及界面是否发送了带draw的go或ponderhit指令，
                     * 来考虑是否反馈带draw或resign的bestmove。
                    *************/
                    //TODO: bestmove
                    Move bestMove = search.BestMove;
                    if (bestMove == null || bestMove.From == 0)
                        Output.WriteLine("bestmove resign");
                    else
                        Output.WriteLine("bestmove " + bestMove);
                    status = EngineStatus.Idle;
                }
                else if (cmd.Name == "quit")
                {
                    //接收到quit指令后的反馈"bye"。引擎完成了退出运转前的准备工作，通知界面，
                    //引擎将在瞬间正常退出运转。界面收到该指令后，即可关闭输入输出通道。
                    info = "bye";
                    m_LogHandler.logging(info);
                    Output.WriteLine(info);
                    status = EngineStatus.Quit;
                }

            }

            else   // Thinking状态时
            {
                //isready
                //空闲状态和思考状态的指令。检测引擎是否处于就绪状态，其反馈总是readyok，该指令仅仅用来检测引擎是否能够正常接收指令。
                if (cmd.Name == "isready")
                {
                    //空闲状态和思考状态的反馈。表明引擎处于就绪状态(可正常接收指令)。
                    Output.WriteLine("readyok");
                }
                else if (cmd.Name == "ponderhit")
                {
                    // ponderhit [draw]
                    // 思考状态的指令。告诉引擎后台思考命中，现在转入正常思考模式(引擎继续处于思考状态，
                    // 此时go指令设定的时限开始起作用)。
                    // 指定draw选项表示向引擎提和，引擎以bestmove提供的选项作为反馈，参阅bestmove指令。
                }
                else if (cmd.Name == "stop")
                {
                    // stop
                    // 思考状态的指令。中止引擎的思考。另外，后台思考没有命中时，就用该指令来中止思考，然后重新输入局面。
                    // 注意：发出该指令并不意味着引擎将立即回到空闲状态，而是要等到引擎反馈
                    // bestmove或nobestmove后才表示回到空闲状态，引擎应尽可能快地作出这样的反馈。
                }

                else
                {
                }
                


            }
            return status;


        }

       
        /// <summary>
        /// 处理setoption命令。
        /// 空闲状态的指令。
        /// 
        /// setoption <选项> [<值>]
        /// 设置引擎参数，这些参数都应该是option反馈的参数，例如：
        /// setoption usebook false，不让引擎使用开局库；
        /// setoption selectivity large，把选择性设成最大；
        /// setoption style risky，指定冒进的走棋风格；
        /// setoption loadbook，初始化开局库。
        /// 但是，设置option反馈没有给出的参数，并不会出错。
        /// 例如UCCI界面《象棋巫师》就从不识别option反馈，而直接根据用户的设置发送setoption指令。
        /// </summary>
        /// <param name="cmd">Ucci命令</param>
        private void ExecuteSetoptionCommand(UcciCommand cmd)
        {
            string[] options = cmd.Paras.Split(' ');

            if (options[0] == "debug")
            {
                if (!bool.TryParse(options[1], out this.isDebug)) this.isDebug = false;
                PrintDebugInfo("debug模式：" + isDebug);
            }
            else if (options[0] == "usebook")
            {
                // 是否用开局库
                // setoption usebook true
                // setoption usebook false
                if (!bool.TryParse(options[1], out this.useBook)) this.useBook = false;
                PrintDebugInfo("usebook " + useBook);
            }
            else if (options[0] == "bookfiles")
            {
                // 设置开局库文件名，当前只支持一个开局库文件
                openingBookFile = options[1];
                PrintDebugInfo("开局库文件：" + openingBookFile);
                if (!File.Exists(openingBookFile))
                {
                    useBook = false;
                    PrintDebugInfo("开局库文件不存在: " + openingBookFile);
                    PrintDebugInfo("setoption usebook false");
                }
            }
            else if (options[0] == "newgame")
            {
                // TODO: 在用置换表时，要将置换表清空
            }

        }

        /// <summary>
        /// 处理position命令
        /// 空闲状态的指令。
        /// position {fen <FEN串> | startpos} [moves <后续着法列表>]
        /// 设置“内置棋盘”的局面，用fen来指定FEN格式串，moves后面跟的是随后走过的着法，例如：
        /// position fen rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR w - - 0 1 moves h2e2 h9g7
        /// FEN格式串的写法参阅《中国象棋电脑应用规范(三)：FEN文件格式》一文。
        /// moves选项是为了防止引擎着出长打着法而设的，UCCI界面传递局面时，
        /// 通常fen选项为最后一个吃过子的局面(或开始局面)，然后moves选项列出该局面到当前局面的所有着法。
        /// startpos表示开始局面，它等价于fen rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR w - - 0 1
        /// 
        /// 执行position命令后，会设置引擎的fen和moves变量
        /// </summary>
        /// <param name="cmd">Ucci命令</param>
        private void ExecutePositionCommand(UcciCommand cmd)
        {
            // 先看看是不是有moves字符串
            const string STR_MOVES = "moves ";
            int idxMoves = cmd.Paras.IndexOf(STR_MOVES);
            string strFen = cmd.Paras;
            if (idxMoves >= 0)
            {
                this.moves = cmd.Paras.Substring(idxMoves + STR_MOVES.Length);
                strFen = cmd.Paras.Substring(0, idxMoves - 1);
            }
            else
            {
                this.moves = "";
            }

            // 此时可能用startpos来表示开始局面
            strFen = strFen.Replace("startpos", "fen rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR w - - 0 1");

            const string STR_FEN = "fen ";
            int start = strFen.IndexOf(STR_FEN);
            if (start < 0) this.fen = "";  // position后面的FEN串错误
            this.fen = strFen.Substring(start + STR_FEN.Length);
        }

    }
}
