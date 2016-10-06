using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace Ponder.Engine
{
    /// <summary>
    /// 置换表
    /// TODO: 以后这个改成unsafe代码，可以提高效率
    /// TODO: 这个字典没有进行总量控制，可能会溢出，要把ulong的值的范围减小
    /// </summary>
    public class TranspositionTable
    {
        /// <summary>
        /// size必须是2的整数次方
        /// </summary>
        int size;
        public NodeOfSearch []dict;

        public TranspositionTable()
            : this(2^20)
        {
        }

        public TranspositionTable(int _size)
        {
            this.size = _size;
            dict = new NodeOfSearch[_size];
            //int newSizeInBytes = Marshal.SizeOf(typeof(NodeOfSearch)) * size;
            //byte* newArrayPointer = (byte*)Marshal.AllocHGlobal(newSizeInBytes).ToPointer();

            //for (int i = 0; i < newSizeInBytes; i++)
            //    *(newArrayPointer + i) = 0;

            //dict = (void*)newArrayPointer;
        }


        //public void Free(void* pointerToUnmanagedMemory)
        //{
        //    Marshal.FreeHGlobal(new IntPtr(dict));
        //}


        /// <summary>
        /// 清空置换表中的所有项
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < size; i++)
                dict[i].NodeType = NodeOfSearch.EMPTY_NODES;
            //int newSizeInBytes = Marshal.SizeOf(typeof(NodeOfSearch)) * size;

            //byte* arrayPointer = (byte*)dict;
            //for (int i = 0; i < newSizeInBytes; i++)
            //    *(arrayPointer + i) = 0;
        }

        /// <summary>
        /// 要防止置换表空间的溢出
        /// </summary>
        /// <param name="boardKey"></param>
        /// <returns></returns>
        private int Rehash(ulong boardKey)
        {
            //int t = (int)((boardKey >> 44) ^ (boardKey & 0xFFFFF));
            int t = (int)boardKey & (size-1);
            return t;
        }


        /// <summary>
        /// 探查结点
        /// </summary>
        /// <param name="boardKey">要查找的键值，应该是盘面的zobrist值</param>
        /// <returns>找到指定键值的节点时返回该节点；找不到时返回null</returns>
        public NodeOfSearch Probe(ulong boardKey)
        {
            int pos = Rehash(boardKey);
            NodeOfSearch node = dict[pos];
            if (node.Key == boardKey) return node;
            else return NodeOfSearch.EmptyNode;
            //byte* arrayPointer = (byte*)dict + pos * Marshal.SizeOf(typeof(NodeOfSearch));

            //NodeOfSearch *node = (NodeOfSearch* ) arrayPointer;
            //if (node->Key == boardKey)
            //{
            //    node->Encode(out code1, out code2);
            //    return true;
            //}
            //code1 = 0;
            //code2 = 0;
            //return false;
        }


        /// <summary>
        /// 在置换表中记录该结点
        /// </summary>
        /// <param name="node">结点</param>
        public void RecordHash(NodeOfSearch node)
        {
            int pos = Rehash(node.Key);
            dict[pos] = node;
          //  byte* arrayPointer = (byte*)dict + pos * Marshal.SizeOf(typeof(NodeOfSearch));

          //  NodeOfSearch* ptrNode = (NodeOfSearch*)arrayPointer;
          //  ptrNode->Key = node.Key;
          //  ptrNode->Depth = node.Depth;
          //  ptrNode->Score = node.Score;
          //  ptrNode->NodeType = node.NodeType;

          //  // 置换表的替换策略。
          //  // 也可能是搜索到了更好的结果！！！
          //  // (depth=2)            a 
          //  //                    /   \
          //  // (depth=1)        b      c
          //  //                 / \      \
          //  // (depth=0)      d   e      f   (当前搜索的节点是f)
          //  // 假设b是以前搜索过的节点，已经保存在HASH表中，depth=1
          //  // 当前搜索的节点是f，depth=0，如果f与b的key在hash表中冲突，
          //  // 由于b的评估值是经过了更深层的搜索得到的，所以这时不替换b
          //  if (ptrNode->NodeType != NodeOfSearch.EMPTY_NODES)
          //  {
          //      //#ifdef _DEBUG
          //      //        if( node.Key != nodeInTable->Key) {
          //      //            totalCollision++;
          //      //            if(nodeInTable->Depth < node.Depth) 
          //      //                countCollision++;
          //      //        }
          //      //#endif
          //      return;
          //  }

          //  // 更新该节点，只能先删除，再加上了
          ////  dict.Remove(Rehash(node.Key));
          ////  dict.Add(Rehash(node.Key), node);
        }
    }
}
