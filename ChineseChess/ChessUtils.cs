using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChineseChess.ChessItems;
using System.Windows.Forms;

namespace ChineseChess
{
    public class ChessUtils
    {
        public static void getMinMax(int val1, int val2, out int min, out int max)
        {
            if (val1 < val2)
            {
                min = val1;
                max = val2;
            }
            else
            {
                min = val2;
                max = val1;
            }
        }
        public static AtackDirection ChessboardDirection = AtackDirection.BlackUpRedDown;

        public static string getMoveString(int gridX, int gridY, int previousGirdX, int previousGirdY)
        {
            string move = "";
            move += getConvertedX(previousGirdX) + getConvertedY(previousGirdY) + getConvertedX(gridX) + getConvertedY(gridY);
            return move;
        }
        public static string getConvertedX(int X)
        {
            switch (X)
            {
                case 0:
                    return "a";
                case 1:
                    return "b";
                case 2:
                    return "c";
                case 3:
                    return "d";
                case 4:
                    return "e";
                case 5:
                    return "f";
                case 6:
                    return "g";
                case 7:
                    return "h";
                case 8:
                    return "i";
                default:
                    return " ";
            }
        }
        public static string getConvertedY(int Y)
        {
            int convertedY = 9 - Y;
            return convertedY.ToString();
        }

        public static string getTypeForUcci(int theType)
        {
            if (theType == 1)
            {
                //红方
                return "w";
            }
            else if (theType == 0)
            {
                //黑方
                return "b";
            }
            return " ";
        }

        /// <summary>
        /// get gridX from converted X
        /// </summary>
        /// <param name="X"></param>
        /// <returns></returns>
        public static int getGridX(string X)
        {
            switch (X)
            {
                case "a":
                    return 0;
                case "b":
                    return 1;
                case "c":
                    return 2;
                case "d":
                    return 3;
                case "e":
                    return 4;
                case "f":
                    return 5;
                case "g":
                    return 6;
                case "h":
                    return 7;
                case "i":
                    return 8;
                default:
                    return -1;
            }
        }

        public static int getGridX(char X)
        {
            string sX = "" + X;
            return getGridX(sX);

        }
        /// <summary>
        /// get gridY from converted Y
        /// </summary>
        /// <param name="Y"></param>
        /// <returns></returns>
        public static int getGridY(string Y)
        {
            int convertedY = -1;
            int gridY = -1;
            try
            {
               convertedY = int.Parse(Y);
               gridY = (9 - convertedY); 
            }
            catch (System.Exception ex)
            {
            	
            }

            return gridY;
        }

        public static int getGridY(char Y)
        {
            string sY = "" + Y;
            return getGridY(sY);
        }

        public static bool getGridXY(string bestMove, out int currentGridX, out int currentGridY, out int nextGridX, out int nextGridY)
        {
            currentGridX = -1;
            currentGridY = -1;
            nextGridX = -1;
            nextGridY = -1;
            if (bestMove.Length == 4)
            {
                currentGridX = getGridX(bestMove[0]);
                currentGridY = getGridY(bestMove[1]);
                nextGridX = getGridX(bestMove[2]);
                nextGridY = getGridY(bestMove[3]);
                return true;
            }

            return false;
        }

        public static int charToNum(char sNum)
        {
            int num = sNum - 48;
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origFen"></param>
        /// <returns></returns>
        public static string convertFen(string origFen)
        {
            string fen;
            fen = origFen;
            //马
            fen.Replace('h', 'n');
            fen.Replace('H', 'N');
            //象/相
            fen.Replace('e', 'b');
            fen.Replace('E', 'B');
            //士
            fen.Replace('g', 'a');
            fen.Replace('G', 'A');

            return fen;
        }

    }
}
