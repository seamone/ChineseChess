using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Ponder.ChineseChess
{
    public class BitHack64
    {

        // 在de Bruijn的位扫描算法中，用到这个神奇的数和一个索引表
        const ulong DEBRUIJN_MAGIC_NUMBER_64 = 0x07EDD5E59A4E28C2L;

        // 在de Bruijn的位扫描算法中，用到这个索引表
        private readonly static int[] INDEX_DEBRUIJN64 = {
           63,  0, 58,  1, 59, 47, 53,  2,
           60, 39, 48, 27, 54, 33, 42,  3,
           61, 51, 37, 40, 49, 18, 28, 20,
           55, 30, 34, 11, 43, 14, 22,  4,
           62, 57, 46, 52, 38, 26, 32, 41,
           50, 36, 17, 19, 29, 10, 13, 21,
           56, 45, 25, 31, 35, 16,  9, 12,
           44, 24, 15,  8, 23,  7,  6,  5
        };

        public static int Log2(ulong power2)
        {
            return INDEX_DEBRUIJN64[(power2 * DEBRUIJN_MAGIC_NUMBER_64) >> 58];
        }

        /// <summary>
        /// 向前扫描LS1B（Least Significant 1 Bit），意思是从b0向b63位的方向查找第一次出现1的位置
        /// 作者：Martin Lauter (1997), Charles E. Leiserson, Harald Prokop, Keith H. Randall
        /// 用de Bruijn方法
        /// 例如：在二进制数（高位）0001001100100（低位）中，调用此函数后返回2，因为从右边数，b0,b1位都是0，b2位是1，所以返回2
        /// 0001001100100  低位
        /// CBA9876543210  从低位向高位数0的个数
        ///           ^
        /// </summary>
        /// <param name="u64">一个64位整数，注意u64!=0</param>
        /// <returns>返回值从0到63, 最低的1所在的位置</returns>
        public static int BitScanForward(ulong u64)
        {
            long n = (long)u64;
            return INDEX_DEBRUIJN64[((ulong)(n & -n) * DEBRUIJN_MAGIC_NUMBER_64) >> 58];

            // 下面这段代码用到CPU相关的指令，试了一下，没有显著提高性能
            // _BitScanForward64只能在64位机器上运行，在32位机器上只能用_BitScanForward
            //unsigned long index;
            //_BitScanForward64(&index,u64);
            //return index;
        }


        /// <summary>
        ///  从低位向高位扫描，返回第一次出现1的位置，同时将该位置为0
        ///  例如：二进制数 （高位在右）0001001001100（低位在左），返回2，同时bb更改为0001001001000
        /// </summary>
        /// <param name="u64">一个64位整数，注意：不能为0</param>
        /// <returns>返回值从0到63, 最低的1所在的位置</returns>
        public static int BitScanForwardWithReset(ref ulong u64)
        { // also called dropForward
            int idx = BitHack64.BitScanForward(u64);
            u64 &= u64 - 1; // reset bit outside
            return idx;
        }

        /// <summary>
        /// 这个数组用于BitScanReverse方法
        /// </summary>
        private static readonly int[] index64 = {
            0, 47,  1, 56, 48, 27,  2, 60,
           57, 49, 41, 37, 28, 16,  3, 61,
           54, 58, 35, 52, 50, 42, 21, 44,
           38, 32, 29, 23, 17, 11,  4, 62,
           46, 55, 26, 59, 40, 36, 15, 53,
           34, 51, 20, 43, 31, 22, 10, 45,
           25, 39, 14, 33, 19, 30,  9, 24,
           13, 18,  8, 12,  7,  6,  5, 63
        };


        /// <summary>
        /// 从高位向低位扫描MS1B（most significant 1 bit），意思是从b63向b0位的方向查找第一次出现1的位置
        /// 这个方法比BitScanForward效率要低
        /// http://chessprogramming.wikispaces.com/BitScan
        /// 例如：以32位整数为例，二进制数（高位）0010 0010 0110 0100（低位）中
        /// 0010001001100100  低位
        /// FEDCBA9876543210  从高位向低位查找第一次出现1的位置
        ///   ^  返回13，注意这是以32位整数为例
        /// </summary>
        /// <param name="u64">一个64位整数，注意u64!=0</param>
        /// <returns>返回值从0到63, 最高的1所在的位置</returns>
        public static int BitScanReverse(ulong bb)
        {
            const ulong debruijn64 = 0x03f79d71b4cb0a89L;
            bb |= bb >> 1;
            bb |= bb >> 2;
            bb |= bb >> 4;
            bb |= bb >> 8;
            bb |= bb >> 16;
            bb |= bb >> 32;
            return index64[(bb * debruijn64) >> 58];
        }



        /// <summary>
        ///  从高位向低位扫描，查找1出现的位置，并将高位的1置为0
        /// </summary>
        /// <param name="u64">64位的整数，注意不能等于0</param>
        /// <returns>从0到63</returns>
        public static int BitScanReverseWithReset(ref ulong u64)
        {
            int idx = BitScanReverse(u64);
            u64 &= ~(1UL << idx); // 把这个位修改为0，效率不太高
            return idx;
        }


        /// <summary>
        /// 求一个64位整数中，低位连续出现的0的个数
        /// </summary>
        /// <param name="u64">64位的整数</param>
        /// <returns>从0到63，如果输入参数为0，则返回64</returns>
        public static int TrailingZeroCount(ulong u64)
        {
            if (u64 != 0)
                return BitScanForward(u64);
            return 64;
        }

        /// <summary>
        /// 求一个64位整数中，高位连续出现的0的个数。
        /// 这个方法比TrailingZeroCount效率要低
        /// </summary>
        /// <param name="u64">64位的整数</param>
        /// <returns>从0到63，如果输入参数为0，则返回64</returns>
        public static int LeadingZeroCount(ulong u64) {
            if ( u64 != 0 )
                return BitScanReverse(u64) ^ 63;
            return 64;
        }



        /// <summary>
        /// 求一个64位整数的二进制表示法中1的个数
        /// </summary>
        /// <param name="u64">64位整数</param>
        /// <returns>二进制1的个数</returns>
        public static int count_1s(ulong u64)
        {
            u64 -= ((u64 >> 1) & 0x5555555555555555UL);
            u64 = ((u64 >> 2) & 0x3333333333333333UL) + (u64 & 0x3333333333333333UL);
            u64 = ((u64 >> 4) + u64) & 0x0F0F0F0F0F0F0F0FUL;
            u64 *= 0x0101010101010101UL;
            return (int)(u64 >> 56);
        }


        /// <summary>
        /// 求一个整数中1的个数，最大为15个
        /// </summary>
        /// <param name="u64"></param>
        /// <returns></returns>
        public static int count_1s_max_15(ulong u64)
        {
            u64 -= (u64 >> 1) & 0x5555555555555555UL;
            u64 = ((u64 >> 2) & 0x3333333333333333UL) + (u64 & 0x3333333333333333UL);
            u64 *= 0x1111111111111111UL;
            return (int)(u64 >> 60);
        }

    }
}
