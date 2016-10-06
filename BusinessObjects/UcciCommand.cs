using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessObjects.Ponder
{
    /// <summary>
    /// 封装Ucci命令
    /// </summary>
    public class UcciCommand
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        private string name;
        public string Name
        {
            get { return name; }
        }


        private string parameters;

        public string Paras
        {
            get { return parameters; }
        }

        public UcciCommand(string name)
        {
            this.name = name;
            this.parameters = "";
        }

        public UcciCommand(string name, string paras)
        {
            this.name = name;
            this.parameters = paras;
        }
        //private string positionFEN;
        //public string PositionFEN
        //{
        //    get { return positionFEN; }
        //}

        //private string positionMoves;
        //public string PositionMoves
        //{
        //    get { return positionMoves; }
        //}

        /// <summary>
        /// 解析ucci命令字符串
        /// </summary>
        /// <param name="strCommand">ucci命令字符串</param>
        /// <returns></returns>
        public static UcciCommand Parse(string strCommand)
        {
            int index = strCommand.IndexOf(' ');
            if (index < 0) return new UcciCommand(strCommand);

            string name = strCommand.Substring(0, index);
            string paras = strCommand.Substring(index + 1);
            return new UcciCommand(name, paras);


            /***
            strCommand = strCommand.Trim();
            UcciCommand cmd = new UcciCommand();

            if (strCommand == "ucci")
            {
                cmd.name = "ucci";
            }
            else if (strCommand == "quit")
            {
                cmd.name = "quit";
            }
            else if (strCommand.StartsWith("position "))
            {
                //position {fen <FEN串> | startpos} [moves <后续着法列表>]
                //空闲状态的指令。设置“内置棋盘”的局面，用fen来指定FEN格式串，moves后面跟的是随后走过的着法，例如：
                //position fen rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR w - - 0 1 moves h2e2 h9g7
                //FEN格式串的写法参阅《中国象棋电脑应用规范(三)：FEN文件格式》一文。
                //moves选项是为了防止引擎着出长打着法而设的，UCCI界面传递局面时，
                //通常fen选项为最后一个吃过子的局面(或开始局面)，然后moves选项列出该局面到当前局面的所有着法。
                //startpos表示开始局面，它等价于fen rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR w - - 0 1
                cmd.name = "position";

                const string SEARCH_FEN = "fen ";
                const string SEARCH_MOVES = "moves ";

                int start = strCommand.IndexOf(SEARCH_FEN);
                int idxMoves = strCommand.IndexOf("moves");

                if (idxMoves < 0)
                {
                    cmd.positionFEN = strCommand.Substring(start + SEARCH_FEN.Length);
                    if (cmd.positionFEN.Trim() == "startpos")
                        cmd.positionFEN = "rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR w - - 0 1";
                    cmd.positionMoves = null;
                }
                else
                {
                    cmd.positionFEN = strCommand.Substring(start + SEARCH_FEN.Length, idxMoves - start - SEARCH_FEN.Length);
                    cmd.positionMoves = strCommand.Substring(idxMoves + SEARCH_MOVES.Length);
                }
            }

            return cmd;
            */
        }
    }
}
