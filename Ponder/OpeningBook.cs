using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Ponder.ChineseChess;

namespace Ponder.Engine
{


    /// <summary>
    /// 开局库
    /// 当前的实现方式只是记录了盘面的zobrist值，并没有按树状的方式来保存每一着法里有几个子结点
    /// 这样在查找开局库时，需要对所有可行的着法要进行一次全面的搜索，效率稍微有点低
    /// </summary>
    public class OpeningBook : IDisposable
    {
        // 开局库的文件名
        const string OPENING_BOOK_FILENAME = @"..\..\..\OPENING.OB";

        // 最多只取前几十个着法放到开局库中，但对于一些流行的开局来说，可能40着还不够
        //const int MAX_MOVES = 40;

        // 一个盘面形势的存储占用10个字节：其中8字节的Zobrist值，2字节的相同盘面的统计数
        const int PER_BOARD_BYTES = 10;

        // 开局库的文件流
        private FileStream fs;


        /// <summary>
        /// 初始化开局库
        /// 主要是打开文件流，所以在使用本类时注意使用using语句，保证文件流被正常地关闭
        /// using(OpeningBook ob = new OpeningBook()) { ... };
        /// </summary>
        public OpeningBook(string openingFilename)
        {
            //如果文件不存在，要把空间分配好，全部写入0.
            if (!File.Exists(openingFilename))
            {
                fs = new FileStream(openingFilename, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

                // 256 * 256 * 256 对应于24位整数
                // 开局库的总大小为160M = 16M * 10，总共可以容纳1600万个盘面
                // 假设有10万局棋都保存前40步，最多会有400万个盘面，也只占用了整个空间的1/4(=400万/1600万）
                for (int i = 0; i < 256 * 256 * 256 * 10; i++)
                {
                    fs.WriteByte(0);
                }
            }
            else
            {
                fs = new FileStream(openingFilename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
        }

        /// <summary>
        /// 要保证文件流正常关闭
        /// </summary>
        public void Dispose()
        {
            fs.Close();
        }

        /// <summary>
        /// zobrist值是64位整数，想要放在160M的开局库里，必须把zobrist值再散列为24位整数
        /// 前面出现的256 * 256 * 256与这个散列函数有关
        /// </summary>
        /// <param name="zobristValue">一个盘面的zobrist值</param>
        /// <returns>24位整数值</returns>
        public int Hash(ulong zobristValue)
        {
            return (int)((zobristValue >> 40) ^ (zobristValue)) & 0x00FFFFFF;
        }

        /// <summary>
        /// 把一个盘面的zobrist值放入到开局库中
        /// 一个元素由｛zobrist值（8字节,ulong），出现次数（2字节, UInt16）｝组成
        /// </summary>
        /// <param name="zobristValue">一个盘面的zobrist值</param>
        public void InsertBoardZobrist(ulong zobristValue)
        {
            // 先要查找相同盘面是否已经在开局库中存在了
            UInt16 countSameBoard;
            int offset = FindBoardHash(zobristValue, out countSameBoard);

            // 2字节的整数的范围限制，一个盘面最多只记录65535次
            if (countSameBoard < UInt16.MaxValue)
                ++countSameBoard;

            // 写入zobrist值（8字节）和相同盘面统计数（2字节）
            fs.Seek(offset, SeekOrigin.Begin);
            byte[] bytes8 = BitConverter.GetBytes(zobristValue);
            //Console.WriteLine(fs.Position);
            fs.Write(bytes8, 0, 8);
            byte[] bytes2 = BitConverter.GetBytes(countSameBoard);
            fs.Write(bytes2, 0, 2);
        }




        /// <summary>
        /// 查找zobrist值是否已经在开局库中存在
        /// </summary>
        /// <param name="zobristValue">64位的zobrist值</param>
        /// <param name="countSameBoard">输出</param>
        /// <returns>在文件流中的偏移量</returns>
        public int FindBoardHash(ulong zobristValue, out UInt16 countSameBoard)
        {
            // 将zobrist值进行散列计算，用这个散列值当做文件流的位置
            int offset = Hash(zobristValue) * PER_BOARD_BYTES;
            fs.Seek(offset, SeekOrigin.Begin);

            for (; ; )
            {
                //从文件中取出10个字节，前8个字节是zobrist值，后2字节是相同盘面统计数
                byte[] bytes10 = new byte[10];
                fs.Read(bytes10, 0, 10);  
                
                ulong zobristInOpening = BitConverter.ToUInt64(bytes10, 0);   //前8字节
                countSameBoard = BitConverter.ToUInt16(bytes10, 8); //后2个字节

                // 如果这个盘面从来没有出现在开局库中，zobristInOpening应该为0，表示这个位置没有被占用，countInOpening应该也是0
                // 如果已经出现过此局面，表示查找到此项，把文件的索引位置返回，同时countSameBoard也被返回
                if (zobristInOpening == 0 || zobristInOpening == zobristValue)
                {
                    break;
                }
                else
                {
                    // 如果执行在这里，表示出现了散列冲突情况
                    // 这里采用简单的顺序查找办法来解决冲突
                    offset += PER_BOARD_BYTES;
                    if (offset > fs.Length)   // 当到达文件末尾时，要绕回到文件头
                        offset = 0;
                    // 如果插入元素的个数超过了Hash表的容量，这个offset会绕回到一开始的位置，说明元素溢出了
                    // 如果出现这种情况，那么这个hash表的查询效率将会非常低，也就是说搜索了整个hash表的长度才找到了这个元素
                    // 这种情况是不能容忍的，所以这里就没有进行这种判断

                }
            }  // end for
            return offset;
        }


        /// <summary>
        /// 输出开局库中元素的散列分布图，用图示的办法来查看散列的效果
        /// 2048行、2048列，2^11 * 2^11 * 4 = 16M 正好是16M个盘面情况
        /// 每4个元素为一组，未占用是白色，占用1个是黄色，占用2个是绿色，全部占用（3个）是蓝色
        /// 通过输出的位图可以看到散列效果还是不错的
        /// </summary>
        //public void SaveHashPicture(string filename)
        //{
        //    using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(2048, 2048, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
        //    {
        //        fs.Seek(0, SeekOrigin.Begin);
        //        for (int h = 0; h < 2048; h++)
        //            for (int w = 0; w < 2048; w++)
        //            {
        //                int count = 0;
        //                for (int i = 0; i < 4; i++)
        //                {
        //                    //从文件中取出6个字节，前4个字节是zobrist值，后2字节是相同盘面统计数
        //                    byte[] bytes6 = new byte[6];
        //                    fs.Read(bytes6, 0, 6);
        //                    int zobristInOpening = BitConverter.ToInt32(bytes6, 0);   //前4字节
        //                    //countSameBoard = BitConverter.ToUInt16(bytes6, 4); //后2个字节
        //                    if (zobristInOpening != 0)
        //                    {
        //                        ++count;
        //                    }
        //                }
        //                if (count == 0)
        //                    bmp.SetPixel(w, h, System.Drawing.Color.White);
        //                else if (count == 1)
        //                    bmp.SetPixel(w, h, System.Drawing.Color.Yellow);
        //                else if (count == 2)
        //                    bmp.SetPixel(w, h, System.Drawing.Color.Green);
        //                else
        //                    bmp.SetPixel(w, h, System.Drawing.Color.Blue);
        //            }
        //        bmp.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
        //    }

        //}


        public Move FindBestMove(Board board)
        {
            Move maxUsedMove = new Move(0,0,0,0);  // 使用率最高的着法
            int maxCount = 0;  // 保存使用得最多的次数

            // 在所有的可行的着法里查找一遍，可以发现这种办法效率有点低
            Move[] movelist = new Move[200];
            MoveGenerator.GenAllMoveList(board,movelist);
            foreach (Move m in movelist)
            {
                UInt16 countSameBoard;
                board.MakeMove(m);
                FindBoardHash(Zobrist.ZoristHash(board), out countSameBoard);

                //if (countSameBoard != 0)
                //    Console.WriteLine(m + ":  " + countSameBoard);
                if (countSameBoard > maxCount)
                {
                    maxCount = countSameBoard;
                    maxUsedMove = new Move(m);
                }
                board.UnmakeMove(m);  // 要恢复原始的盘面
            }
            return maxUsedMove;
        }

        /// <summary>
        /// 查找开局库，找到出现走的最多的着法，比如：board是红方走了炮二平五后的局面
        /// 在开局库里自动查找黑方的着法（因为zobrist值里有轮哪方走棋的信息），如果马8进7用了10000次，而其它着法都少于这个次数时
        /// 那么返回的最佳着法就是“马8进7”
        /// </summary>
        /// <param name="board">棋盘局面</param>
        /// <returns>在开局库里查找使用最多的着法，未找到时返回null</returns>
        public static Move BestMove(Board board)
        {
            return BestMove(OPENING_BOOK_FILENAME, board);
        }

        public static Move BestMove(string openingBookFile, Board board)
        {
            using (OpeningBook book = new OpeningBook(openingBookFile))
            {
                return book.FindBestMove(board);
            }
        }

        /// <summary>
        /// 用一堆PGN棋谱文件来建立一个开局库
        /// </summary>
        public static void CreateOpeningBook(string bookFilename, string pgnDir, int maxStep)
        {
            //这个目录里是从仲效卿那里找来的一堆棋谱
            DirectoryInfo dir = new DirectoryInfo(pgnDir);
            FileInfo[] files = dir.GetFiles("*.pgn", SearchOption.AllDirectories);  // 要搜索每一个子目录
            foreach (FileInfo pgnFileName in files)
            {
                Console.WriteLine(pgnFileName.Name);
                AddPgnFileToOpeningBook(bookFilename, pgnFileName.FullName, maxStep);
            }
        }

        /// <summary>
        /// 把一盘PGN对局里的前N个着法放入对局库中
        /// </summary>
        /// <param name="pgnFilename">PGN文件名</param>
        /// <returns>加入到开局库时,返回true
        /// 如果在解析PGN时遇到不规范的棋谱时，返回false，此时控制台会打印出错误信息</returns>
        public static bool AddPgnFileToOpeningBook(string bookFilename, string pgnFilename, int maxStep)
        {
            using (OpeningBook book = new OpeningBook(bookFilename))
            {
                // 从PGN文件里读出所有着法来，这里用的是纵列格式
                string[] allmoves = PgnUtil.GetAllMovesFromPgnFile(pgnFilename);

                Board board = new Board();
                int numMove = 0; // 记录已走了第几步了
                foreach (string strMove in allmoves)
                {
                    if (numMove >= maxStep) break;
                    try
                    {
                        // 走一步后，把盘面生成zobrist值，保存到开局库里
                        // TODO: board.CreateMoveFromString(NotationConverter.Convert(board, strMove));
                        Move move = MoveNotation.CreateMoveFromChineseNotation(board, strMove);                  
                        board.MakeMove(move);
                     //   Console.WriteLine("==== " + strMove + "\n" + board);
                        ++numMove;
                        ulong zobrist = Zobrist.ZoristHash(board);
                        book.InsertBoardZobrist(zobrist);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("--- " + strMove + " ---");
                        Console.WriteLine(e.Message);
                        Console.WriteLine(board);
                        return false;
                    }
                } // end 对每一着法循环结束
            }
            return true;
        }
    }
}
