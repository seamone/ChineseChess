using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace Ponder.ChineseChess
{
    /// <summary>
    /// 
    /// </summary>
    public class PgnUtil
    {
        /*
        static void Main(string[] args)
        {
            OpeningBook.CreateOpeningBook();

            Board board = Board.CreateStartingBoard();
            Move m = OpeningBook.BestMove(board);

            int ttt = 0;
            while(m!=null)
            {
                m.Make(board);
                Console.WriteLine("---- " + (++ttt) + ": Best Move: " + m + "\n" + board);
                m = OpeningBook.BestMove(board);
            }

           // AddPgnFileToOpeningBook(@"..\..\test.pgn");

            DirectoryInfo dir = new DirectoryInfo("e:\\11万精品真藏");
            FileInfo[] files = dir.GetFiles("*.pgn",SearchOption.AllDirectories);

            foreach (FileInfo f in files)
            {
                Console.WriteLine(f.Name);
                //AddPgnFileToOpeningBook(f.FullName);

                //if (!CheckPgnFile(f.FullName))
                //{
                //    Console.WriteLine("!!!FAIL!!!" + f.FullName);
                //    File.Delete(f.FullName);
                //    Console.ReadKey();
                //}
            }
            Console.ReadKey();
        }

         */


        private static bool CheckPgnFile(string pgnFilename)
        {
            string[] allmoves = GetAllMovesFromPgnFile(pgnFilename);

            Board board = new Board();
            foreach (string strMove in allmoves)
            {
                try
                {
                    //TODO:  board.CreateMoveFromString(NotationConverter.Convert(board, strMove));
                    Move move = BoardUtil.CreateMoveFromString(board, strMove); 
                    board.MakeMove(move);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString() + "\n" + board);
                    return false;
                }
                // Console.WriteLine("--- " + strMove + " ---");
                // Console.WriteLine(board);
            }
            return true;
        }

        public static string[] GetAllMovesFromPgnFile(string pgnFilename)
        {
            StreamReader infile = new StreamReader(pgnFilename, Encoding.GetEncoding("GBK"));
            string pgnText = infile.ReadToEnd();
            infile.Close();

            ArrayList chMoves = new ArrayList();

            Regex reg = new Regex(@"\d+\.\s+(\w{4})\s*(?:\{(?:.|[\r\n])*?\})?\s*(\w{4})\s*(?:\{(?:.|[\r\n])*?\})?");
            MatchCollection collection = reg.Matches(pgnText);
            GroupCollection groupCollection;

            foreach (Match match in collection)
            {
                //Console.WriteLine(match.ToString());
                groupCollection = match.Groups;
                string redMove = groupCollection[1].Value;
                if (!redMove.Equals(""))
                    chMoves.Add(redMove);
                string blackMove = groupCollection[2].Value;
                if (!blackMove.Equals(""))
                    chMoves.Add(blackMove);
            }

            string[] strs = new string[chMoves.Count];
            for (int i = 0; i < chMoves.Count; i++)
                strs[i] = (string)chMoves[i];

            return strs;
        }


    }
}
