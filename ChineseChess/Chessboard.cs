using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ChineseChess.ChessItems;
using System.Diagnostics;
using BusinessObjects.Events;

namespace ChineseChess
{
    public enum PostionLineType
    {
        Full = 0,
        UpPart = 1,
        DownPart = 2,
        RightPart = 3,
        LeftPart = 4
    }

    public enum InfoType
    {
        ShowDangerInfo,
        ShowEngineInfo,
        InitializeChessBoard,
        BestMove
    }
    //|------------------------------------------------------------------------------|
    //|                                                                              |
    //|------------------------------------------------------------------------------|
    //|
    //|
    //|
    public partial class Chessboard : Form
    {
       
        
        public Chessboard()
        {
            InitializeComponent();
            //Control.CheckForIllegalCrossThreadCalls = false;
        }
      

        
#region "Private Field"
        private Point leftTopPoint;
        private Point leftBottomPoint;
        private Point rightTopPoint;
        private Point rightBottomPoint;
        Graphics g;
        Pen p;
        private int iniX = 60;
        private int iniY = 60;
        private const int chessRow = 10;
        private const int chessCol = 9;
        private static int _pieceWidth = 60;
        private ChessType _currentActionType = ChessType.Red;
        private BaseChess _selectChess;
        private bool canUndo = false;

        private BaseChess _previousChess;
        private BaseChess _previousOppositeChess;

        private King jiang;
        private King shuai;
        //Button startButton;

        //private List<BaseChess> _chessPieces = new List<BaseChess>();

        private EngineClient _theEngineClient = null;
        public byte[,] chessArray = new byte[chessRow, chessCol];
        private string fen = "rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR w - - 0 1";
        private string moves = "";
        private int m_nNoOfMoves = 0;
        private bool m_bMoveStep = false;
        private InfoType m_InfoType;

        private ChessType m_PlayerType;
        private ChessType m_EngineType;

        private int m_IndexX = 0;
        private int m_IndexY = 0;

        private string bestMove;
#endregion
  

#region "Public Property"
        public static int PieceWidth
        {
            get { return _pieceWidth; }
        }
#endregion
        
     

#region "Private method"

        /// <summary>
        /// update the info about moves
        /// the total of move
        /// 是否是一个回合还是半个回合
        /// 生成moves的string
        /// </summary>
        /// <param name="move"></param>
        /// <param name="isRemove"></param>
        private void _updateMovesStep(string move, bool isRemove = false)
        {

            if (!m_bMoveStep)
            {
                if (isRemove)
                {
                    moves = "";
                }
                else
                {
                    moves = "moves ";
                    moves += move;
                }
                m_bMoveStep = true;

                m_nNoOfMoves++;
            }
            else
            {
                if (!isRemove)
                {
                    moves += " " + move;
                }
                m_bMoveStep = false;

            }
        }

        /// <summary>
        /// it's for multi thread, access the controls
        /// it's common, update control by the info type 
        /// </summary>
        private void threadPro()
        {
            int id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            MethodInvoker MethInvo = new MethodInvoker(updateMessage);
            BeginInvoke(MethInvo);
        }
        /// <summary>
        /// combine with threadPro
        /// </summary>
        private void updateMessage()
        {
            if (m_InfoType == InfoType.ShowDangerInfo)
            {
                showDangerInfo();
            }
            else if (m_InfoType == InfoType.ShowEngineInfo)
            {
                showEngineInfo();
            }
            else if (m_InfoType == InfoType.InitializeChessBoard)
            {
                initializeChessboard();
            }
            else if (m_InfoType == InfoType.BestMove)
            {
                _executeBestMove();
            }


        }

        /// <summary>
        /// show the engine info
        /// </summary>
        private void showEngineInfo()
        {
            EngineInfoText.Text = _theEngineClient.EngineInfo;
            //if (!string.IsNullOrEmpty(m_engineInfos.ToString()))
            //{
            //    EngineInfoText.Text = m_engineInfos.ToString() + "\n";
            //}
        }

        /// <summary>
        /// 显示“将”
        /// </summary>
        private void showDangerInfo()
        {
            dangerLabel.Visible = true;
        }

        /// <summary>
        /// reset the chess board, 
        /// including to create the chess pieces at chess board
        /// and the action type
        /// and disable the start button
        /// </summary>
        private void initializeChessboard()
        {
            _changeType(true);
            loadChessPieces(fen);
            StartButton.Enabled = false;
        }

        /// <summary>
        /// create the chess pieces
        /// </summary>
        /// <param name="fen"></param>
        private void loadChessPieces(string fen)
        {
            string chessBoard;
            string[] chessArr = fen.Split(' ');
            m_IndexX = 0;
            m_IndexY = 0;
            if (chessArr.Length < 1)
            {
                return;
            }
            else
            {
                chessBoard = chessArr[0];
            }
            foreach (char curWord in chessBoard)
            {
                if (curWord >= 49 && curWord <= 57)
                {
                    m_IndexX += ChessUtils.charToNum(curWord);
                }
                else if ((curWord >= 65 && curWord <= 90) || (curWord >= 97 && curWord <= 122))
                {
                    //red
                    createPiece((byte)curWord);
                }
                else if (curWord == 47)
                {
                    m_IndexX = 0;
                    m_IndexY++;
                }
            }

        }
        /// <summary>
        /// the chess type
        /// r/R:Rook
        /// n/N:Knight
        /// b/B:Bishop
        /// a/A:Advisor
        /// k/K:King
        /// c/C:Cannon
        /// p/P:Pawn
        /// </summary>
        private void createPiece(byte theChessType)
        {
            switch (theChessType)
            {
                case 82:
                case 114:
                    //车
                    Rook ju1 = new Rook(theChessType);

                    ju1.GridX = m_IndexX;
                    ju1.GridY = m_IndexY;
                    ju1.PreviousGridX = m_IndexX;
                    ju1.PreviousGridY = m_IndexY;
                    ju1.InitChess();
                    this.panel1.Controls.Add(ju1);
                    ju1.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                    chessArray[m_IndexY, m_IndexX] = (byte)ju1.PieceType;
                    break;
                case 78:
                case 110:
                    //马
                    Knight ma = new Knight(theChessType);
                    ma.GridX = m_IndexX;
                    ma.GridY = m_IndexY;
                    ma.PreviousGridX = m_IndexX;
                    ma.PreviousGridY = m_IndexY;
                    ma.InitChess();
                    this.panel1.Controls.Add(ma);
                    ma.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                    chessArray[m_IndexY, m_IndexX] = (byte)ma.PieceType;
                    break;
                case 66:
                case 98:
                    //象
                    Bishop xiang = new Bishop(theChessType);
                    xiang.GridX = m_IndexX;
                    xiang.GridY = m_IndexY;
                    xiang.PreviousGridX = m_IndexX;
                    xiang.PreviousGridY = m_IndexY;

                    xiang.InitChess();
                    this.panel1.Controls.Add(xiang);
                    xiang.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                    chessArray[m_IndexY, m_IndexX] = (byte)xiang.PieceType;
                    break;
                case 65:
                case 97:
                    //士
                    Advisor shi = new Advisor(theChessType);
                    shi.GridX = m_IndexX;
                    shi.GridY = m_IndexY;
                    shi.PreviousGridX = m_IndexX;
                    shi.PreviousGridY = m_IndexY;
                    shi.InitChess();
                    this.panel1.Controls.Add(shi);
                    shi.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                    chessArray[m_IndexY, m_IndexX] = (byte)shi.PieceType;
                    break;
                case 75:
                    //将
                    jiang = new King(theChessType);

                    jiang.GridX = m_IndexX;
                    jiang.GridY = m_IndexY;
                    jiang.PreviousGridX = m_IndexX;
                    jiang.PreviousGridY = m_IndexY;

                    jiang.InitChess();
                    this.panel1.Controls.Add(jiang);
                    jiang.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                    jiang.BeRemoved += new EventHandler(_beRemovedEventHandler);
                    //jiang.IsMoved += new EventHandler(checkKingFaceToFace);
                    chessArray[m_IndexY, m_IndexX] = (byte)jiang.PieceType;
                    break;
                case 107:
                    //帅
                    shuai = new King(theChessType);

                    shuai.GridX = m_IndexX;
                    shuai.GridY = m_IndexY;
                    shuai.PreviousGridX = m_IndexX;
                    shuai.PreviousGridY = m_IndexY;
                    shuai.InitChess();
                    this.panel1.Controls.Add(shuai);
                    shuai.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                    shuai.BeRemoved += new EventHandler(_beRemovedEventHandler);
                    //shuai.IsMoved += new EventHandler(checkKingFaceToFace);
                    chessArray[m_IndexY, m_IndexX] = (byte)shuai.PieceType;
                    break;
                case 67:
                case 99:
                    //炮
                    Cannon pao = new Cannon(theChessType);

                    pao.GridX = m_IndexX;
                    pao.GridY = m_IndexY;
                    pao.PreviousGridX = m_IndexX;
                    pao.PreviousGridY = m_IndexY;

                    pao.InitChess();
                    this.panel1.Controls.Add(pao);
                    pao.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                    chessArray[m_IndexY, m_IndexX] = (byte)pao.PieceType;
                    break;
                case 80:
                case 112:
                    //兵
                    Pawn bing = new Pawn(theChessType);

                    bing.GridX = m_IndexX;
                    bing.GridY = m_IndexY;
                    bing.PreviousGridX = m_IndexX;
                    bing.PreviousGridY = m_IndexY;
                    bing.InitChess();
                    this.panel1.Controls.Add(bing);
                    bing.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                    chessArray[m_IndexY, m_IndexX] = (byte)bing.PieceType;
                    break;

            }
            m_IndexX++;

        }
        /// <summary>
        /// static load chess pieces
        /// </summary>
        private void loadChessPieces()
        {
            #region 放置棋子,红方
            for (int i = 0; i <= 8; i += 8)
            {
                Rook ju1 = new Rook();
                ju1.Type = ChessType.Red;
                ju1.GridX = i;
                ju1.GridY = 0;
                ju1.PreviousGridX = i;
                ju1.PreviousGridY = 0;
                ju1.Text = "车";
                ju1.InitChess();
                this.panel1.Controls.Add(ju1);
                ju1.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                chessArray[0, i] = (byte)ju1.PieceType;

            }

            for (int i = 1; i <= 7; i += 6)
            {
                Knight ma = new Knight();
                ma.Type = ChessType.Red;
                ma.GridX = i;
                ma.GridY = 0;
                ma.PreviousGridX = i;
                ma.PreviousGridY = 0;
                ma.Text = "马";
                ma.InitChess();
                this.panel1.Controls.Add(ma);
                ma.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                chessArray[0, i] = (byte)ma.PieceType;
                //因为“炮”与“马”的位置都类似，循环次数也一样
                Cannon pao = new Cannon();
                pao.Type = ChessType.Red;
                pao.GridX = i;
                pao.GridY = 2;
                pao.PreviousGridX = i;
                pao.PreviousGridY = 2;
                pao.Text = "炮";
                pao.InitChess();
                this.panel1.Controls.Add(pao);
                pao.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                chessArray[2, i] = (byte)pao.PieceType;
            }

            for (int i = 2; i <= 6; i += 4)
            {
                Bishop xiang = new Bishop();
                xiang.Type = ChessType.Red;
                xiang.GridX = i;
                xiang.GridY = 0;
                xiang.PreviousGridX = i;
                xiang.PreviousGridY = 0;
                xiang.Text = "相";
                xiang.InitChess();
                this.panel1.Controls.Add(xiang);
                xiang.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                chessArray[0, i] = (byte)xiang.PieceType;
            }

            for (int i = 3; i <= 6; i += 2)
            {
                Advisor shi = new Advisor();
                shi.Type = ChessType.Red;
                shi.GridX = i;
                shi.GridY = 0;
                shi.PreviousGridX = i;
                shi.PreviousGridY = 0;
                shi.Text = "士";
                shi.InitChess();
                this.panel1.Controls.Add(shi);
                shi.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                chessArray[0, i] = (byte)shi.PieceType;
            }

            jiang = new King();
            jiang.Type = ChessType.Red;
            jiang.GridX = 4;
            jiang.GridY = 0;
            jiang.PreviousGridX = 4;
            jiang.PreviousGridY = 0;
            jiang.Text = "将";
            jiang.InitChess();
            this.panel1.Controls.Add(jiang);
            jiang.MouseClick += new MouseEventHandler(chessItem_MouseClick);
            jiang.BeRemoved += new EventHandler(_beRemovedEventHandler);
            //jiang.IsMoved += new EventHandler(checkKingFaceToFace);
            chessArray[0, 4] = (byte)jiang.PieceType;

            for (int i = 0; i <= 8; i += 2)
            {
                Pawn bing = new Pawn();
                bing.Type = ChessType.Red;
                bing.GridX = i;
                bing.GridY = 3;
                bing.PreviousGridX = i;
                bing.PreviousGridY = 3;
                bing.Text = "兵";
                bing.InitChess();
                this.panel1.Controls.Add(bing);
                bing.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                chessArray[3, i] = (byte)bing.PieceType;
            }
            #endregion

            #region 放置棋子,黑方
            for (int i = 0; i <= 8; i += 8)
            {
                Rook ju1 = new Rook();
                ju1.Type = ChessType.Black;
                ju1.GridX = i;
                ju1.GridY = 9;
                ju1.PreviousGridX = i;
                ju1.PreviousGridY = 9;
                ju1.Text = "車";
                ju1.InitChess();
                this.panel1.Controls.Add(ju1);
                ju1.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                chessArray[9, i] = (byte)ju1.PieceType;
            }

            for (int i = 1; i <= 7; i += 6)
            {
                Knight ma = new Knight();
                ma.Type = ChessType.Black;
                ma.GridX = i;
                ma.GridY = 9;
                ma.PreviousGridX = i;
                ma.PreviousGridY = 9;
                ma.Text = "馬";
                ma.InitChess();
                this.panel1.Controls.Add(ma);
                ma.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                chessArray[9, i] = (byte)ma.PieceType;

                //因为“炮”与“马”的位置都类似，循环次数也一样
                Cannon pao = new Cannon();
                pao.Type = ChessType.Black;
                pao.GridX = i;
                pao.GridY = 7;
                pao.PreviousGridX = i;
                pao.PreviousGridY = 7;
                pao.Text = "炮";
                pao.InitChess();
                this.panel1.Controls.Add(pao);
                pao.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                chessArray[7, i] = (byte)pao.PieceType;
            }

            for (int i = 2; i <= 6; i += 4)
            {
                Bishop xiang = new Bishop();
                xiang.Type = ChessType.Black;
                xiang.GridX = i;
                xiang.GridY = 9;
                xiang.PreviousGridX = i;
                xiang.PreviousGridY = 9;
                xiang.Text = "象";
                xiang.InitChess();
                this.panel1.Controls.Add(xiang);
                xiang.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                chessArray[9, i] = (byte)xiang.PieceType;
            }

            for (int i = 3; i <= 6; i += 2)
            {
                Advisor shi = new Advisor();
                shi.Type = ChessType.Black;
                shi.GridX = i;
                shi.GridY = 9;
                shi.PreviousGridX = i;
                shi.PreviousGridY = 9;
                shi.Text = "仕";
                shi.InitChess();
                this.panel1.Controls.Add(shi);
                shi.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                chessArray[9, i] = (byte)shi.PieceType;
            }

            shuai = new King();
            shuai.Type = ChessType.Black;
            shuai.GridX = 4;
            shuai.GridY = 9;
            shuai.PreviousGridX = 4;
            shuai.PreviousGridY = 9;
            shuai.Text = "帅";
            shuai.InitChess();
            this.panel1.Controls.Add(shuai);
            shuai.MouseClick += new MouseEventHandler(chessItem_MouseClick);
            shuai.BeRemoved += new EventHandler(_beRemovedEventHandler);
            //shuai.IsMoved += new EventHandler(checkKingFaceToFace);
            chessArray[9, 4] = (byte)shuai.PieceType;

            for (int i = 0; i <= 8; i += 2)
            {
                Pawn bing = new Pawn();
                bing.Type = ChessType.Black;
                bing.GridX = i;
                bing.GridY = 6;
                bing.PreviousGridX = i;
                bing.PreviousGridY = 6;
                bing.Text = "卒";
                bing.InitChess();
                this.panel1.Controls.Add(bing);
                bing.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                chessArray[6, i] = (byte)bing.PieceType;
            }
            #endregion
        }

        /// <summary>
        /// get the best move from engine
        /// </summary>
        private void _executeBestMove()
        {
            int currentGridX, currentGridY, nextGridX, nextGridY;
            BaseChess _attackedChess = null;
            if (ChessUtils.getGridXY(bestMove, out currentGridX, out currentGridY, out nextGridX, out nextGridY))
            {
                _selectChess = BaseChess.getChessOnPoint(panel1, currentGridX, currentGridY);
                _attackedChess = BaseChess.getChessOnPoint(panel1, nextGridX, nextGridY);
                if (_selectChess!=null)
                {
                    chessItemSelected(_selectChess);
                    if (_attackedChess!=null)
                    {
                        chessItemSelected(_attackedChess);
                    } 
                    else
                    {
                        chessMove(nextGridX, nextGridY);
                    }
                }
            }

        }

        /// <summary>
        /// it's common from select chess piece, or attack another chess piece
        /// </summary>
        /// <param name="theChessPiece"></param>
        private void chessItemSelected(BaseChess theChessPiece)
        {
            if (_selectChess == null)
            {
                //第一次选择棋子
                if (theChessPiece.Type == _currentActionType)
                {
                    _selectChess = theChessPiece;
                    //_previousChess = _selectChess;
                }
                else
                {
                    //选择错了棋子
                    theChessPiece.IsChecked = false;
                }


            }
            else
            {
                //已经选择了一个棋子，再次选择时是对方棋子
                if (theChessPiece.Type != _currentActionType)
                {
                    BaseChess beAttackChess = theChessPiece;
                    if (_selectChess.move(beAttackChess.Location))
                    {
                        chessArray[_selectChess.GridY, _selectChess.GridX] = (byte)_selectChess.PieceType;
                        chessArray[beAttackChess.GridY, beAttackChess.GridX] = 0;
                        _previousOppositeChess = beAttackChess.Clone();
                        beAttackChess.remove();
                        string move = ChessUtils.getMoveString(_selectChess.GridX, _selectChess.GridY, _selectChess.PreviousGridX, _selectChess.PreviousGridY);
                        _updateMovesStep(move, true);
                        _updateFenStr();
                        doSomeAfterMove();
                    }
                    else
                    {
                        //不遵循着法
                        beAttackChess.IsChecked = false;
                    }

                }
                else
                {
                    //如果再次选择时还是己方棋子,更改check状态
                    _selectChess.IsChecked = false;
                    //更换当前引用
                    _selectChess = theChessPiece;

                }
            }
        }

        /// <summary>
        /// it's only move the chess piece, after select it
        /// </summary>
        /// <param name="newLocation"></param>
        private void chessMove(Point newLocation)
        {
            if (_selectChess != null && _selectChess.Type == _currentActionType)
            {
                if (_selectChess.move(newLocation))
                {
                    //_selectChess.IsChecked = false;
                    chessArray[_selectChess.GridY, _selectChess.GridX] = (byte)_selectChess.PieceType;
                    chessArray[_selectChess.PreviousGridY, _selectChess.PreviousGridX] = (byte)0;
                    string move = ChessUtils.getMoveString(_selectChess.GridX, _selectChess.GridY, _selectChess.PreviousGridX, _selectChess.PreviousGridY);
                    _updateMovesStep(move);                   
                    doSomeAfterMove();
                }
            }
        }
        /// <summary>
        /// it's only move the chess piece, after select it
        /// </summary>
        /// <param name="newLocation"></param>
        private void chessMove(int gridX, int gridY)
        {
            if (_selectChess != null && _selectChess.Type == _currentActionType)
            {
                if (_selectChess.move(gridX, gridY))
                {
                    //_selectChess.IsChecked = false;
                    chessArray[_selectChess.GridY, _selectChess.GridX] = (byte)_selectChess.PieceType;
                    chessArray[_selectChess.PreviousGridY, _selectChess.PreviousGridX] = (byte)0;
                    string move = ChessUtils.getMoveString(_selectChess.GridX, _selectChess.GridY, _selectChess.PreviousGridX, _selectChess.PreviousGridY);
                    _updateMovesStep(move);
                    doSomeAfterMove();
                }
            }
        }
        /// <summary>
        /// 着子后，轮询着方
        /// </summary>
        /// <param name="isReset"></param>
        private void _changeType(bool isReset = false)
        {
            if (isReset)
            {
                _currentActionType = ChessType.Red;
            }
            else
            {
                _currentActionType = _getOppositeType(_currentActionType);
            }
            TypeStatus.Text = Enum.GetName(typeof(ChessType), _currentActionType);
            if (_currentActionType == ChessType.Red)
            {
                TypeStatus.ForeColor = Color.Red;
            }
            else
            {
                TypeStatus.ForeColor = Color.Black;
            }
        }

        /// <summary>
        /// 查询对方的类型,红方还是黑方
        /// </summary>
        /// <param name="currentType"></param>
        /// <returns></returns>
        private ChessType _getOppositeType(ChessType currentType)
        {
            if (currentType == ChessType.Black)
            {
                return ChessType.Red;
            }
            else if (currentType == ChessType.Red)
            {
                return ChessType.Black;
            }
            return ChessType.Red;
        }

        /// <summary>
        /// do something after chess move or attack
        /// </summary>
        private void doSomeAfterMove()
        {
            if (_currentActionType == m_PlayerType)
            {
                _sendPositionCommand();
                _theEngineClient.SendGoCommand(5);
                _theEngineClient.BestMoveReceived += new BestMoveReceivedEventHandler(_theEngineClient_BestMoveReceived);
                
            }
            _updateFenStr();
            //the type should not change before sending the position
            _changeType();
            //两种将的方式都会显示将
            checkKingFaceToFace(null, null);
            if (!isKingDangerFromOtherChess(jiang) && !isKingDangerFromOtherChess(shuai))
            {
                dangerLabel.Visible = false;
            }
            string move = "";

            UndoButton.Enabled = true;
            _previousChess = _selectChess;
            _selectChess.IsChecked = false;
            _selectChess = null;
        }

        /// <summary>
        /// 查询对方的将或帅
        /// </summary>
        /// <param name="theType"></param>
        /// <returns></returns>
        private King getOppoisteKing(ChessType theType)
        {
            if (theType == ChessType.Red)
            {
                return shuai;
            }
            else
            {
                return jiang;
            }

        }

        /// <summary>
        /// 查询己方的将或帅
        /// </summary>
        /// <param name="theType"></param>
        /// <returns></returns>
        private King getOwnKing(ChessType theType)
        {
            if (theType == ChessType.Red)
            {
                return jiang;
            }
            else
            {
                return shuai;
            }

        }

        /// <summary>
        /// abandon
        /// </summary>
        /// <returns></returns>
        private bool isKingDanger()
        {
            //to do
            int gridX, gridY;
            gridX = gridY = -1;
            if (_currentActionType == ChessType.Red)
            {
                gridX = jiang.GridX;
                gridY = jiang.GridY;

            }
            else if (_currentActionType == ChessType.Black)
            {
                gridX = shuai.GridX;
                gridY = shuai.GridY;
            }

            if (_selectChess != null && _selectChess.obeyTheLimit(gridX, gridY))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// check whether the two king can face to face
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkKingFaceToFace(object sender, EventArgs e)
        {
            bool hasChessBetweenKing = false;
            if (jiang.GridY == shuai.GridY)
            {
                foreach (Control curCtr in this.panel1.Controls)
                {
                    if (curCtr is BaseChess)
                    {
                        BaseChess tempChess = (BaseChess)curCtr;
                        if (tempChess.GridY == jiang.GridY
                            && tempChess != jiang
                            && tempChess != shuai)
                        {
                            hasChessBetweenKing = true;
                            break;
                        }
                    }
                }
                if (!hasChessBetweenKing)
                {
                    showDangerInfo();
                }
            }
        }
        /// <summary>
        /// check if the king can be defeat by other chess piece
        /// </summary>
        /// <param name="theKing"></param>
        /// <returns></returns>
        private bool isKingDangerFromOtherChess(King theKing)
        {

            foreach (Control curCtr in this.panel1.Controls)
            {
                if (curCtr is BaseChess)
                {
                    BaseChess tempChess = (BaseChess)curCtr;
                    ChessType oppositeType = _getOppositeType(theKing.Type);
                    if (tempChess.Type == oppositeType)
                    {
                        if (tempChess.obeyTheLimit(theKing.GridX, theKing.GridY))
                        {

                            showDangerInfo();
                            return true;

                        }

                    }
                }
            }
            return false;
        }

        /// <summary>
        /// to do
        /// </summary>
        private void _updatePositionToEngine()
        {

        }

        /// <summary>
        /// send position command to engine
        /// and update the fen
        /// </summary>
        private void _sendPositionCommand()
        {
            StringBuilder cmd = new StringBuilder();
            string[] strArr = fen.Split(' ');
            if (strArr.Length < 1)
            {
                cmd.Append(fen);
            }
            else
            {
                cmd.Append(strArr[0]);
            }
            string theType = ChessUtils.getTypeForUcci((int)_currentActionType);
            cmd.Append(" " + theType + " - - 0 ");
            cmd.Append(m_nNoOfMoves.ToString() + " ");
            cmd.Append(moves);
            fen = cmd.ToString();
            _theEngineClient.SendPositionCommand(fen);

        }

        /// <summary>
        /// update the fen string
        /// </summary>
        private void _updateFenStr()
        {
            char theChar;
            string temp = "";
            int countOfEmpty = 0;
            //行
            for (m_IndexY = 0; m_IndexY < chessRow; m_IndexY++)
            {
                //列
                for (m_IndexX = 0; m_IndexX < chessCol; m_IndexX++)
                {
                    byte curWord = chessArray[m_IndexY, m_IndexX];
                    if (curWord == 0)
                    {
                        countOfEmpty++;
                    }
                    else if ((curWord >= 65 && curWord <= 90) || (curWord >= 97 && curWord <= 122))
                    {
                        if (countOfEmpty > 0)
                        {
                            temp += countOfEmpty;
                            countOfEmpty = 0;
                        }
                        theChar = (char)curWord;
                        temp += theChar;
                    }
                }
                if (countOfEmpty > 0)
                {
                    temp += countOfEmpty;
                    countOfEmpty = 0;
                }
                if (m_IndexY < chessRow)
                {
                    theChar = '/';
                    temp += theChar;
                }
               
            }
            fen = temp;
        }

        
        /// <summary>
        /// 画棋盘
        /// </summary>
        private void drawChessBoard()
        {

            int screenWidth = this.panel1.Width;
            int screenHeight = this.panel1.Height;

            // int wei = 50;
            //Point point = new Point(25, 25);
            //_pieceWidth = (int)Math.Round((decimal)(screenHeight) / chessRow) -10;
            iniX = 30;
            iniY = 30;

            leftTopPoint = new Point(iniX, iniY);
            leftBottomPoint = new Point(iniX, iniY + _pieceWidth * (chessRow - 1));
            rightTopPoint = new Point(iniX + _pieceWidth * (chessCol - 1), iniY);
            rightBottomPoint = new Point(iniX + _pieceWidth * (chessCol - 1), iniY + _pieceWidth * (chessRow - 1));
            //this.panel1.Width = iniY + _pieceWidth * chessRow + 50;
            //this.panel1.Height = screenHeight;

            g = this.panel1.CreateGraphics();
            p = new Pen(Color.Black, 2);
            int x1, x2, y1, y2;

            //Horizontal line
            x1 = leftTopPoint.X;
            y1 = leftTopPoint.Y;
            x2 = rightTopPoint.X;
            y2 = rightTopPoint.Y;
            for (int i = 0; i < chessRow; i++)
            {
                g.DrawLine(p, x1, y1, x2, y2);
                y1 += _pieceWidth;
                y2 = y1;
            }
            g.DrawLine(p, leftTopPoint, leftBottomPoint);
            g.DrawLine(p, rightTopPoint, rightBottomPoint);

            //upper vertical  line
            x1 = leftTopPoint.X + _pieceWidth;
            y1 = leftTopPoint.Y;
            x2 = x1;
            y2 = y1 + _pieceWidth * 4;
            for (int i = 1; i < chessCol - 1; i++)
            {
                g.DrawLine(p, x1, y1, x2, y2);
                x1 += _pieceWidth;
                x2 = x1;
            }
            //lower vertical line
            x1 = leftBottomPoint.X;
            y1 = leftBottomPoint.Y;
            x2 = leftBottomPoint.X;
            y2 = leftBottomPoint.Y - _pieceWidth * 4;
            for (int i = 1; i < chessRow - 1; i++)
            {
                g.DrawLine(p, x1, y1, x2, y2);
                x1 += _pieceWidth;
                x2 = x1;
            }

            //upper cross line
            x1 = leftTopPoint.X + _pieceWidth * 3;
            y1 = leftTopPoint.Y;
            x2 = x1 + _pieceWidth * 2;
            y2 = y1 + _pieceWidth * 2;
            g.DrawLine(p, x1, y1, x2, y2);

            x1 = x1 + _pieceWidth * 2;
            x2 = x1 - _pieceWidth * 2;
            g.DrawLine(p, x1, y1, x2, y2);

            //lower cross line
            x1 = leftBottomPoint.X + _pieceWidth * 3;
            y1 = leftBottomPoint.Y;
            x2 = x1 + _pieceWidth * 2;
            y2 = y1 - _pieceWidth * 2;
            g.DrawLine(p, x1, y1, x2, y2);

            x1 = x1 + _pieceWidth * 2;
            x2 = x1 - _pieceWidth * 2;
            g.DrawLine(p, x1, y1, x2, y2);

            //draw position line for special chess pieces
            //upper left Cannon
            x1 = leftTopPoint.X + _pieceWidth;
            y1 = leftTopPoint.Y + _pieceWidth * 2;
            drawPostion(x1, y1);
            //left Pawn 1
            x1 = leftTopPoint.X;
            y1 = leftTopPoint.Y + _pieceWidth * 3;
            drawPostion(x1, y1, PostionLineType.RightPart);
            //left Pawn 2
            x1 += (_pieceWidth * 2);
            drawPostion(x1, y1);
            //left Pawn 3
            x1 += (_pieceWidth * 2);
            drawPostion(x1, y1);
            //left Pawn 4
            x1 += (_pieceWidth * 2);
            drawPostion(x1, y1);
            //left Pawn 5
            x1 += (_pieceWidth * 2);
            drawPostion(x1, y1, PostionLineType.LeftPart);
            //upper right Cannon's position
            x1 = rightTopPoint.X - _pieceWidth;
            y1 = rightTopPoint.Y + _pieceWidth * 2;
            drawPostion(x1, y1);

            //lower left Cannon
            x1 = leftBottomPoint.X + _pieceWidth;
            y1 = leftBottomPoint.Y - _pieceWidth * 2;
            drawPostion(x1, y1);
            //lower Pawn 1
            x1 = leftBottomPoint.X;
            y1 = leftBottomPoint.Y - _pieceWidth * 3;
            drawPostion(x1, y1, PostionLineType.RightPart);
            //lower Pawn 2
            x1 += (_pieceWidth * 2);
            drawPostion(x1, y1);
            //lower Pawn 3
            x1 += (_pieceWidth * 2);
            drawPostion(x1, y1);
            //lower Pawn 4
            x1 += (_pieceWidth * 2);
            drawPostion(x1, y1);
            //lower Pawn 5
            x1 += (_pieceWidth * 2);
            drawPostion(x1, y1, PostionLineType.LeftPart);
            //lower right Cannon
            x1 = rightBottomPoint.X - _pieceWidth;
            y1 = rightBottomPoint.Y - _pieceWidth * 2;
            drawPostion(x1, y1);

            //"楚河","汉界"字
            FontFamily fm = new FontFamily("黑体");
            Font f = new Font(fm, 30);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            g.DrawString("楚河", f, Brushes.Black, (float)(leftTopPoint.X + _pieceWidth * 1.5), (float)(leftTopPoint.Y + _pieceWidth * 4.1), sf);
            g.DrawString("汉界", f, Brushes.Black, (float)(leftTopPoint.X + _pieceWidth * 5.1), (float)(leftTopPoint.Y + _pieceWidth * 4.1), sf);
            f.Dispose();

            //draw the frame
            //p = new Pen(Color.Black, 10);
            //g.DrawLine(p, leftTopPoint.X - 15, leftTopPoint.Y - 10, rightTopPoint.X + 15, rightTopPoint.Y - 10);
            //g.DrawLine(p, leftBottomPoint.X - 15, leftBottomPoint.Y + 10, rightBottomPoint.X + 15, rightBottomPoint.Y + 10);
            //g.DrawLine(p, leftTopPoint.X - 10, leftTopPoint.Y - 15, leftBottomPoint.X - 10, leftBottomPoint.Y + 15);
            //g.DrawLine(p, rightTopPoint.X + 10, rightTopPoint.Y - 15, rightBottomPoint.X + 10, rightBottomPoint.Y + 15);
            //------------------------
            g.Dispose();

        }

        /// <summary>
        /// draw the cross for the position
        /// </summary>
        /// <param name="thePoint"></param>
        private void drawPostion(Point thePoint)
        {
            drawPostion(thePoint.X, thePoint.Y);
        }
        /// <summary>
        /// draw the cross for the position
        /// _||_
        /// -||-
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void drawPostion(int x, int y, PostionLineType theType = PostionLineType.Full)
        {
            int offset = 5;
            int lineWidth = 20;
            //if (theType == PostionLineType.Full || theType == PostionLineType.UpPart)
            //{
            //    //left top
            //    Point temp11 = new Point(x - offset, y - offset);
            //    Point temp12 = new Point(x - offset - lineWidth, y - offset);
            //    Point temp13 = new Point(x - offset, y - offset - lineWidth);
            //    g.DrawLine(p, temp11, temp12);
            //    g.DrawLine(p, temp11, temp13);
            //    //right top
            //    Point temp21 = new Point(x + offset, y - offset);
            //    Point temp22 = new Point(x + offset, y - offset - lineWidth);
            //    Point temp23 = new Point(x + offset + lineWidth, y - offset);
            //    g.DrawLine(p, temp21, temp22);
            //    g.DrawLine(p, temp21, temp23);
            //}
            //if (theType == PostionLineType.Full || theType == PostionLineType.DownPart)
            //{
            //    //left bottom
            //    Point temp31 = new Point(x - offset, y + offset);
            //    Point temp32 = new Point(x - offset, y + offset + lineWidth);
            //    Point temp33 = new Point(x - offset - lineWidth, y + offset);
            //    g.DrawLine(p, temp31, temp32);
            //    g.DrawLine(p, temp31, temp33);
            //    //right bottom
            //    Point temp41 = new Point(x + offset, y + offset);
            //    Point temp42 = new Point(x + offset + lineWidth, y + offset);
            //    Point temp43 = new Point(x + offset, y + offset + lineWidth);
            //    g.DrawLine(p, temp41, temp42);
            //    g.DrawLine(p, temp41, temp43);
            //}

            if (theType == PostionLineType.Full || theType == PostionLineType.RightPart)
            {

                //right top
                Point temp21 = new Point(x + offset, y - offset);
                Point temp22 = new Point(x + offset, y - offset - lineWidth);
                Point temp23 = new Point(x + offset + lineWidth, y - offset);
                g.DrawLine(p, temp21, temp22);
                g.DrawLine(p, temp21, temp23);
                //right bottom
                Point temp41 = new Point(x + offset, y + offset);
                Point temp42 = new Point(x + offset + lineWidth, y + offset);
                Point temp43 = new Point(x + offset, y + offset + lineWidth);
                g.DrawLine(p, temp41, temp42);
                g.DrawLine(p, temp41, temp43);
            }
            if (theType == PostionLineType.Full || theType == PostionLineType.LeftPart)
            {
                //left top
                Point temp11 = new Point(x - offset, y - offset);
                Point temp12 = new Point(x - offset - lineWidth, y - offset);
                Point temp13 = new Point(x - offset, y - offset - lineWidth);
                g.DrawLine(p, temp11, temp12);
                g.DrawLine(p, temp11, temp13);
                //left bottom
                Point temp31 = new Point(x - offset, y + offset);
                Point temp32 = new Point(x - offset, y + offset + lineWidth);
                Point temp33 = new Point(x - offset - lineWidth, y + offset);
                g.DrawLine(p, temp31, temp32);
                g.DrawLine(p, temp31, temp33);

            }




        }

     


#endregion

#region "Event Handler"

        /// <summary>
        /// when king is killed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _beRemovedEventHandler(object sender, EventArgs e)
        {
            King theloseKing = (King)sender;
            ChessType theWinnerType = _getOppositeType(theloseKing.Type);
            string type = Enum.GetName(typeof(ChessType), theWinnerType);
            if (MessageBox.Show(type + " side Win!!! Do you want new game?", "Game over", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                //重新开具
                StartButton_Click(null, null);
            }
            else
            {
                StartButton.Enabled = true;
            }
        }

        private void Chessboard_Load(object sender, EventArgs e)
        {
            //drawChessBoard();
            //startButton = new Button();
            //startButton.Location = new Point(rightBottomPoint.X+100,rightBottomPoint.Y-100);
            //startButton.Text = "Start";
            //startButton.Size = new System.Drawing.Size(100, 100);
            //startButton.Visible = true;
            //this.Controls.Add(startButton);
            //TypeStatus.Text = Enum.GetName(typeof(ChessType), _currentActionType);
            //createDangerLabel();
            //_previousChess = new Pawn();
            //_previousChess.Disposed += new EventHandler(_previousChess_Disposed);

            _theEngineClient = EngineClient.DefaultEngineClient;
            //_theEngineClient.ReceviedEngineData += new DataReceivedEventHandler(_theEngineClient_ReceviedEngineData);

            //default
            m_PlayerType = ChessType.Red;
            m_EngineType = ChessType.Black;
            //
        }

        private void _previousChess_Disposed(object sender, EventArgs e)
        {

        }


        //private void _theEngineClient_ReceviedEngineData(object sender, DataReceivedEventArgs e)
        //{
        //    m_UcciInfo = e.Data;

        //    _theEngineClient.log(m_UcciInfo);

        //    //m_InfoType = InfoType.ShowEngineInfo;
        //    //threadPro();

        //}

        private void _theEngineClient_EngineIsOK(object sender, EventArgs e)
        {
            m_InfoType = InfoType.InitializeChessBoard;
            threadPro();
            _theEngineClient.SendStartPostion();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            drawChessBoard();
        }

        private void panel1_Click(object sender, EventArgs e)
        {

        }

        private void Chessboard_MouseClick(object sender, MouseEventArgs e)
        {
            XCoordinate.Text = e.X.ToString();
            YCoordinate.Text = e.Y.ToString();
        }

        private void Chessboard_MouseMove(object sender, MouseEventArgs e)
        {
            XCoordinate.Text = e.X.ToString();
            YCoordinate.Text = e.Y.ToString();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            XCoordinate.Text = e.X.ToString();
            YCoordinate.Text = e.Y.ToString();
        }



        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (_selectChess != null && _selectChess.Type != m_EngineType)
            {
                chessMove(e.Location);
            }
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            try
            {
                _changeType();
                if (_previousChess != null)
                {
                    _previousChess.GridX = _previousChess.PreviousGridX;
                    _previousChess.GridY = _previousChess.PreviousGridY;
                    _previousChess.InitChess();
                }
                if (_previousOppositeChess != null)
                {
                    //Type theType = _previousOppositeChess.GetType();
                    //object theChess = Activator.CreateInstance(theType);

                    //BaseChess newChess = _previousOppositeChess.Clone();
                    //int index = this.panel1.Controls.GetChildIndex(_previousOppositeChess, false);
                    //Pawn theBing = new Pawn();
                    //theBing.GridX = 6;
                    //theBing.GridY = 4;
                    //theBing.Type = ChessType.Black;
                    //theBing.Text = "卒";
                    _previousOppositeChess.MouseClick += new MouseEventHandler(chessItem_MouseClick);
                    this.panel1.Controls.Add(_previousOppositeChess);
                    _previousOppositeChess = null;
                }
                if (dangerLabel.Visible)
                {
                    dangerLabel.Visible = false;
                }
                UndoButton.Enabled = false;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            showEngineInfo();
        }

        private void Chessboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            _theEngineClient.DisposeEngine();
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            _theEngineClient.SendUcciCommand();
        }

        private void RedRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            m_PlayerType = ChessType.Red;
            m_EngineType = ChessType.Black;
        }

        private void BlackRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            m_PlayerType = ChessType.Black;
            m_EngineType = ChessType.Red;
        }

        private void chessItem_MouseClick(object sender, EventArgs e)
        {
            BaseChess _tempChess = (BaseChess)sender;
            //if (_selectChess != null && _tempChess != _selectChess)
            //{
            //    _selectChess.IsChecked = false;
            //}
            if (_tempChess.Type == m_EngineType)
            {
                return;
            }
            chessItemSelected(_tempChess);


            //else
            //{
            //    _selectChess.IsChecked = false;
            //    // _tempChess.IsChecked = false;
            //}

        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            //_previousChess.remove();
            this.panel1.Controls.Clear();
            timer1.Enabled = true;
            timer1.Start();
            _theEngineClient.SendUcciCommand();
            _theEngineClient.EngineIsOK += new EventHandler(_theEngineClient_EngineIsOK);

        }

        private void _theEngineClient_BestMoveReceived(object sender, BestMoveReceivedEventArgs e)
        {
            m_InfoType = InfoType.BestMove;
            bestMove = e.BestMove;
            threadPro();
            _theEngineClient.BestMoveReceived -= new BestMoveReceivedEventHandler(_theEngineClient_BestMoveReceived);
        }
#endregion

      

        private void ChangeEngineButton_Click(object sender, EventArgs e)
        {
            if (EngineOpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string enginePath = EngineOpenFileDialog.FileName.ToString();
                //outputFileName = RefLgfPath.Substring(RefLgfPath.LastIndexOf("\\") + 1);
                string log = "the target file path is " + enginePath;
                _theEngineClient.log(log);
                _theEngineClient.changeEngine(enginePath);

            }
        }

       


       
       






    }
}
