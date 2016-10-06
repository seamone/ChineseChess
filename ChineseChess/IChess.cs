using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ChineseChess
{
    public interface IChess
    {
        bool move(Point nextLocation);
        void remove();
        void InitChess();
    }
}
