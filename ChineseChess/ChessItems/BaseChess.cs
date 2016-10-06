using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ChineseChess.ChessItems
{
    public enum ChessType
    {
        Black = 0,
        Red = 1
    }

    public enum ChessPieceType
    {
        NONE = 0,
        KING = 1,
        ROOK = 2,
        KNIGHT = 3,
        CANNON = 4,
        BISHOP = 5,
        ADVISOR = 6,
        PAWN = 7
       
    }
    /// <summary>
    /// RedUpBlackDown means red piece is up of the board
    /// BlackUpRedDown means red piece is down of the board
    /// </summary>
    public enum AtackDirection
    {
        RedUpBlackDown = 0,
        BlackUpRedDown = 1
    }

    /// 棋子基类 
    /// </summary> 
    public abstract class BaseChess : Label, IChess,ICloneable
    {

        protected Chessboard _chessboard;

        /// <summary> 
        /// 构造函数 
        /// </summary> 
        public BaseChess(Chessboard theChessboard)
        {
            this._chessboard = theChessboard;
            
        }

        public BaseChess()
        {
            this._chessPieceSize = Chessboard.PieceWidth;
        }

        private ChessType _type;
        public ChessType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        protected byte _pieceType;
        public byte PieceType
        {
            get { return _pieceType; }
        }

        private bool _isChecked = false;
        public virtual bool IsChecked
        {
            get
            {
                // TODO:  添加 ChessItem.IsChecked getter 实现
                return _isChecked;
            }
            set
            {
                // TODO:  添加 ChessItem.IsChecked setter 实现
                _isChecked = value;
                if (_isChecked == true)
                    this.BorderStyle = BorderStyle.FixedSingle;
                else if (_isChecked == false)
                    this.BorderStyle = BorderStyle.None;
            }
        }

        
        private int _gridX;
        public int GridX
        {
            get { return _gridX; }
            set { _gridX = value; }
        }
        private int _gridY;
        public int GridY
        {
            get { return _gridY; }
            set { _gridY = value; }
        }
        //public ChineseChess.ChessItems.Chessboard Chessboard
        //{
        //    get { return _chessboard; }
        //    set { _chessboard = value; }
        //}
        private int _chessPieceSize = 50;
        public int ChessPieceSize
        {
            get { return _chessPieceSize; }
            set { _chessPieceSize = value; }
        }

        protected override void OnClick(EventArgs e)
        {
            if (!IsChecked)
            {
                IsChecked = true;
                this.BorderStyle = BorderStyle.FixedSingle;
            }
            base.OnClick(e);
        }



        public override System.Drawing.Image BackgroundImage
        {
            get
            {
                //绘制圆形
                Bitmap bitmap = new Bitmap(ChessPieceSize, ChessPieceSize);
                Graphics g = Graphics.FromImage(bitmap);
                g.DrawEllipse(new Pen(Color.Black, 1), 0, 0, ChessPieceSize, ChessPieceSize);
                if (_type == ChessType.Black)
                {
                    SolidBrush b = (SolidBrush)Brushes.Black;
                    g.FillEllipse(b, 0, 0, ChessPieceSize, ChessPieceSize);
                }
                else
                {
                    SolidBrush b = (SolidBrush)Brushes.Red;
                    g.FillEllipse(b, 0, 0, ChessPieceSize, ChessPieceSize);
                }
                //设置相关属性
                this.Width = ChessPieceSize+1;
                this.Height = ChessPieceSize+1;
                this.TextAlign = ContentAlignment.MiddleCenter;
                this.Font = new System.Drawing.Font("黑体", 30F, FontStyle.Bold);
                this.ForeColor = Color.White;

                g.Dispose();
                return bitmap;
            }
            set
            {
                base.BackgroundImage = value;
            }

        }

        private int _previousGridX;
        public int PreviousGridX
        {
            get { return _previousGridX; }
            set { _previousGridX = value; }
        }
        private int _previousGridY;
        public int PreviousGridY
        {
            get { return _previousGridY; }
            set { _previousGridY = value; }
        }

        /// <summary>
        /// This is for piece like King, Advisor
        /// </summary>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        /// <returns></returns>
        public bool isInSquareBox(int gridX, int gridY)
        {
            if (gridX < 3 || gridX > 5)
            {
                return false;
            }
            if (ChessUtils.ChessboardDirection == AtackDirection.RedUpBlackDown)
            {
                  if ((Type == ChessType.Red && gridY <= 2) ||
                (Type == ChessType.Black && gridY >= 7))
                  {
                      return true;
                  }
            }
            else if (ChessUtils.ChessboardDirection == AtackDirection.BlackUpRedDown)
            {
                if ((Type == ChessType.Red && gridY >= 7) ||
              (Type == ChessType.Black && gridY <= 2))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This is for piece like King, Advisor
        /// </summary>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        /// <returns></returns>
        public bool isInOwnSide(int gridX, int gridY)
        {
            if (ChessUtils.ChessboardDirection == AtackDirection.RedUpBlackDown)
            {
                if (Type == ChessType.Red && gridY <= 4)
                {
                    return true;
                }
                else if (Type == ChessType.Black && gridY >= 5)
                {
                    return true;
                }
            }
            else if (ChessUtils.ChessboardDirection == AtackDirection.BlackUpRedDown)
            {
                if (Type == ChessType.Red && gridY >= 5)
                {
                    return true;
                }
                else if (Type == ChessType.Black && gridY <= 4)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool obeyTheLimit(int gridX,int gridY)
        {
            if (gridX>=0 && gridX <=9
                && gridY>=0 && gridY<=8)
            {
                return true;
            }
            
            return false;

        }

        public bool hasChessOnPoint(int gridX, int gridY)
        {
            if (this.Parent is Panel)
            {
                foreach (Control curItem in this.Parent.Controls)
                {
                    if (curItem is BaseChess)
                    {
                        BaseChess ci = (BaseChess)curItem;
                        if (ci.GridX == gridX && ci.GridY == gridY)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;


        }

        public static BaseChess getChessOnPoint(Panel parent, int gridX, int gridY)
        {
            foreach (Control curItem in parent.Controls)
            {
                if (curItem is BaseChess)
                {
                    BaseChess ci = (BaseChess)curItem;
                    if (ci.GridX == gridX && ci.GridY == gridY)
                    {
                        return ci;
                    }
                }
            }
            return null;
        }

        public int PointConvertToGrid(int fromPoint)
        {
            int toGrid = (int)Math.Round((decimal)fromPoint / ChessPieceSize);
            return toGrid;
        }

        public virtual bool move(Point nextLocation)
        {
            int gridX = (int)Math.Floor((decimal)nextLocation.X / ChessPieceSize);
            int gridY = (int)Math.Floor((decimal)nextLocation.Y / ChessPieceSize);
            if (obeyTheLimit(gridX,gridY))
            {
                PreviousGridX = GridX;
                PreviousGridY = GridY;
                GridX = gridX;
                GridY = gridY;
                InitChess();
                return true;
            }
            else
            {
                return false;
            }
        }
        public virtual bool move(int gridX, int gridY)
        {
            if (obeyTheLimit(gridX, gridY))
            {
                PreviousGridX = GridX;
                PreviousGridY = GridY;
                GridX = gridX;
                GridY = gridY;
                InitChess();
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual void remove()
        {
            this.Dispose();
        }


        public void InitChess()
        {

            this.Location = new Point(GridX*ChessPieceSize,GridY*ChessPieceSize);

        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public BaseChess Clone()
        {
            Type theType = this.GetType();
            BaseChess theChess = (BaseChess)Activator.CreateInstance(theType);
            theChess.Type = this.Type;
            theChess.GridX = this.GridX;
            theChess.GridY = this.GridY;
            theChess.PreviousGridX = this.PreviousGridX;
            theChess.PreviousGridY = this.PreviousGridY;
            theChess.Text = this.Text;
            theChess.InitChess();
            return theChess;
        } 
    }
}
