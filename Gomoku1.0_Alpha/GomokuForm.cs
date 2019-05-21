using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gomoku1._0_Alpha
{
    public partial class GomokuForm : Form
    {
        public GomokuForm()
        {
            InitializeComponent();
        }
        #region 私有变量        
        //判断下黑棋还是白棋（默认偶数是下黑棋）
        private short count = 0;
        //记录已下棋子的位置
        private List<Point> recPos = new List<Point> { };//黑白棋子位置
        private List<Point> bRecPos = new List<Point> { };//黑棋位置
        private List<Point> wRecPos = new List<Point> { };//白棋位置
        //棋子类型
        private enum pieceType
        { bPiece = 1, wPiece = 2, empty = 0 };
        //记录棋盘状态
        private short[,] checkerboardStatus = new short[15, 15];
        //获胜标志位
        //不直接加减鼠标点击事件，有bug
        private bool flagWin = false;
        //人机或人人对奕标志位
        private bool isHumanVSHuman = false;
        #endregion 私有变量

        #region 私有类
        private class judgement
        {
            //AI落子事件委托
            public delegate void AITurnEventHandler(out int x, out int y);
            //发布事件
            public event AITurnEventHandler AITurn;

            protected virtual void OnAITurn(int x, int y)
            {
                if (AITurn != null)
                    AITurn(out x, out y);
            }
        }
        private class AIturn
        {

        }
        #endregion 私有类

        #region 方法
        //画棋盘
        private void drawCheckerboard(PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 30, 30, 560, 560);
            for (short i = 1; i < 14; i++)
            {
                e.Graphics.DrawLine(new Pen(Color.Black, 1), new Point(40 * i + 30, 30), new Point(40 * i + 30, 590));
                e.Graphics.DrawLine(new Pen(Color.Black, 1), new Point(30, 40 * i + 30), new Point(590, 40 * i + 30));
            }

        }
        //画棋子
        private void drawPiece(PaintEventArgs e, List<Point> recPos, Brush brush)
        {
            foreach (Point Pos in recPos)
                e.Graphics.FillEllipse(brush, new Rectangle(Pos.X, Pos.Y, 33, 33));
        }
        //画棋子方法重载
        private void drawPiece(Point pos, Brush brush)
        {
            Graphics g = pictureBox1.CreateGraphics();
            g.FillEllipse(brush, new Rectangle(pos.X, pos.Y, 33, 33));
        }
        //统计连子数
        private short countConsecNum(int x, int y, short[] pos, short pieceType)
        {
            short n = 0;//不计刚下的棋子
            for (short i = 1; i < 5; i++)
            {
                //不能用x - i < 0 || x + i > 14
                //因为(pos[0],pos[1])不同！！！！
                if (pos[0] != 0 && (x + pos[0] * i < 0 || x + pos[0] * i > 14))
                    break;
                if (pos[1] != 0 && (y + pos[1] * i < 0 || y + pos[1] * i > 14))
                    break;
                else if (checkerboardStatus[x + pos[0] * i, y + pos[1] * i] == pieceType)
                    n++;
                else
                    break;
            }
            return n;
        }
        //countConsecNum重构
        private short getPieceType(int x, int y, short[] pos, short offset)
        {
            if (pos[0] != 0 && (x + pos[0] * offset < 0 || x + pos[0] * offset > 14))
                return -1;
            if (pos[1] != 0 && (y + pos[1] * offset < 0 || y + pos[1] * offset > 14))
                return -1;
            return checkerboardStatus[x + pos[0] * offset, y + pos[1] * offset];
        }
        //判断是否获胜
        private bool isVictory(int x, int y, short pieceType)
        {
            int n = 1;//在这里计上刚下的棋子
            List<ArrayList> direction = new List<ArrayList> 
            { 
                new ArrayList { new short[2]{-1, 0}, new short[2]{1, 0} }, 
                new ArrayList { new short[2]{0, -1}, new short[2]{0, 1} }, 
                new ArrayList { new short[2]{-1, 1}, new short[2]{1, -1} }, 
                new ArrayList { new short[2]{-1, -1}, new short[2]{1, 1} } 
            };
            foreach (ArrayList list in direction)
            {
                n = 1;
                foreach (short[] pos in list)
                    n = n + countConsecNum(x, y, pos, pieceType);
                if (n >= 5)
                    return true;
            }
            return false;
        }
        //价值评估函数
        private int evaluate(int x, int y)
        {
            int value = 0;
            short turn = getPieceType(x, y, new short[2] { 0, 0 }, 0);
            short empty = (short)pieceType.empty;
            List<ArrayList> direction = new List<ArrayList> 
            { 
                new ArrayList { new short[2]{-1, 0}, new short[2]{1, 0} }, 
                new ArrayList { new short[2]{0, -1}, new short[2]{0, 1} }, 
                new ArrayList { new short[2]{-1, 1}, new short[2]{1, -1} }, 
                new ArrayList { new short[2]{-1, -1}, new short[2]{1, 1} } 
            };
            #region 评价
            foreach (ArrayList list in direction)
            {
                foreach (short[] pos in list)
                {
                    //（1）OOOOO
                    if (getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == turn
                        && getPieceType(x, y, pos, 4) == turn)
                        value += 100000;
                    if (getPieceType(x, y, pos, -1) == turn
                        && getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == turn)
                        value += 100000;
                    if (getPieceType(x, y, pos, -2) == turn
                        && getPieceType(x, y, pos, -1) == turn
                        && getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == turn)
                        value += 100000;
                    //（2）+OOOO+
                    if (getPieceType(x, y, pos, -1) == empty
                        && getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == turn
                        && getPieceType(x, y, pos, 4) == empty)
                        value += 4320;//累加所以+OOOO的价值会包含(实际会>4320)
                    if (getPieceType(x, y, pos, -2) == empty
                        && getPieceType(x, y, pos, -1) == turn
                        && getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == empty)
                        value += 4320;
                    //（3）+OOO++ || ++OOO+
                    if (getPieceType(x, y, pos, -1) == empty
                        && getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == empty
                        && getPieceType(x, y, pos, 4) == empty)
                        value += 720;
                    if (getPieceType(x, y, pos, -2) == empty
                        && getPieceType(x, y, pos, -1) == turn
                        && getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == empty
                        && getPieceType(x, y, pos, 3) == empty)
                        value += 720;
                    if (getPieceType(x, y, pos, -3) == empty
                        && getPieceType(x, y, pos, -2) == turn
                        && getPieceType(x, y, pos, -1) == turn
                        && getPieceType(x, y, pos, 1) == empty
                        && getPieceType(x, y, pos, 2) == empty)
                        value += 720;
                    //（4）+OO+O+ || +O+OO+
                    if (getPieceType(x, y, pos, -1) == empty
                        && getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == empty
                        && getPieceType(x, y, pos, 3) == turn
                        && getPieceType(x, y, pos, 4) == empty)
                        value += 720;
                    if (getPieceType(x, y, pos, -2) == empty
                        && getPieceType(x, y, pos, -1) == turn
                        && getPieceType(x, y, pos, 1) == empty
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == empty)
                        value += 720;
                    if (getPieceType(x, y, pos, -4) == empty
                        && getPieceType(x, y, pos, -3) == turn
                        && getPieceType(x, y, pos, -2) == turn
                        && getPieceType(x, y, pos, -1) == empty
                        && getPieceType(x, y, pos, 1) == empty)
                        value += 720;
                    //（5）+OOOO || 0000+
                    if (getPieceType(x, y, pos, -1) == empty
                        && getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == turn)
                        value += 720;
                    if (getPieceType(x, y, pos, -2) == empty
                        && getPieceType(x, y, pos, -1) == turn
                        && getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == turn)
                        value += 720;
                    if (getPieceType(x, y, pos, -3) == empty
                        && getPieceType(x, y, pos, -2) == turn
                        && getPieceType(x, y, pos, -1) == turn
                        && getPieceType(x, y, pos, 1) == turn)
                        value += 720;
                    if (getPieceType(x, y, pos, -4) == empty
                        && getPieceType(x, y, pos, -3) == turn
                        && getPieceType(x, y, pos, -2) == turn
                        && getPieceType(x, y, pos, -1) == turn)
                        value += 720;
                    //（6）OO+OO
                    if (getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == empty
                        && getPieceType(x, y, pos, 3) == turn
                        && getPieceType(x, y, pos, 4) == turn)
                        value += 720;
                    if (getPieceType(x, y, pos, -1) == turn
                        && getPieceType(x, y, pos, 1) == empty
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == turn)
                        value += 720;
                    //（7）O+OOO || OOO+O
                    if (getPieceType(x, y, pos, 1) == empty
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == turn
                        && getPieceType(x, y, pos, 4) == turn)
                        value += 720;
                    if (getPieceType(x, y, pos, -2) == turn
                        && getPieceType(x, y, pos, -1) == empty
                        && getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == turn)
                        value += 720;
                    if (getPieceType(x, y, pos, -3) == turn
                        && getPieceType(x, y, pos, -2) == empty
                        && getPieceType(x, y, pos, -1) == turn
                        && getPieceType(x, y, pos, 1) == turn)
                        value += 720;
                    if (getPieceType(x, y, pos, -4) == turn
                        && getPieceType(x, y, pos, -3) == empty
                        && getPieceType(x, y, pos, -2) == turn
                        && getPieceType(x, y, pos, -1) == turn)
                        value += 720;
                    //（8）++OO++
                    if (getPieceType(x, y, pos, -2) == empty
                        && getPieceType(x, y, pos, -1) == empty
                        && getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == empty
                        && getPieceType(x, y, pos, 3) == empty)
                        value += 120;
                    //（9）++O+O+ || +O+O++
                    if (getPieceType(x, y, pos, -2) == empty
                        && getPieceType(x, y, pos, -1) == empty
                        && getPieceType(x, y, pos, 1) == empty
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == empty)
                        value += 120;
                    if (getPieceType(x, y, pos, -4) == empty
                        && getPieceType(x, y, pos, -3) == empty
                        && getPieceType(x, y, pos, -2) == turn
                        && getPieceType(x, y, pos, -1) == empty
                        && getPieceType(x, y, pos, 1) == empty)
                        value += 120;
                    //（10）+++O++ || ++O+++
                    if (getPieceType(x, y, pos, -3) == empty
                        && getPieceType(x, y, pos, -2) == empty
                        && getPieceType(x, y, pos, -1) == empty
                        && getPieceType(x, y, pos, 1) == empty
                        && getPieceType(x, y, pos, 2) == empty)
                        value += 20;
                }
            }
            #endregion 评价
            return value;
        }
        //价值评估函数(重构)
        private int evaluate(int x, int y, short turn)
        {
            int value = 0;
            short opposite = (turn == (short)pieceType.bPiece) ? (short)pieceType.wPiece : (short)pieceType.bPiece;
            short empty = (short)pieceType.empty;
            List<ArrayList> direction = new List<ArrayList> 
            { 
                new ArrayList { new short[2]{-1, 0}, new short[2]{1, 0} }, 
                new ArrayList { new short[2]{0, -1}, new short[2]{0, 1} }, 
                new ArrayList { new short[2]{-1, 1}, new short[2]{1, -1} }, 
                new ArrayList { new short[2]{-1, -1}, new short[2]{1, 1} } 
            };
            #region 评价
            foreach (ArrayList list in direction)
            {
                foreach (short[] pos in list)
                {
                    //活四
                    if (getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == turn
                        && getPieceType(x, y, pos, 4) == turn
                        && getPieceType(x, y, pos, 5) == empty)
                        value += 1000000;
                    //死四
                    if (getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == turn
                        && getPieceType(x, y, pos, 4) == turn
                        && getPieceType(x, y, pos, 5) == opposite)
                        value += 100000;
                    if (getPieceType(x, y, pos, -1) == turn
                        && getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == turn)
                        value += 100000;
                    if (getPieceType(x, y, pos, -2) == turn
                        && getPieceType(x, y, pos, -1) == turn
                        && getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == turn)
                        value += 100000;
                    //活三
                    if (getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == turn
                        && getPieceType(x, y, pos, 4) == empty)
                        value += 99999;
                    if (getPieceType(x, y, pos, -2) == empty
                        && getPieceType(x, y, pos, -1) == turn
                        && getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == empty)
                        value += 99999;
                    //死三
                    if (getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == turn
                        && getPieceType(x, y, pos, 4) == opposite)
                        value += 10000;
                    if (getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == empty
                        && getPieceType(x, y, pos, 4) == turn
                        && getPieceType(x, y, pos, 5) == opposite)
                        value += 10000;
                    if (getPieceType(x, y, pos, -1) == turn
                        && getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == empty
                        && getPieceType(x, y, pos, 3) == turn)
                        value += 10000;
                    if (getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == empty
                        && getPieceType(x, y, pos, 3) == turn
                        && getPieceType(x, y, pos, 4) == turn
                        && getPieceType(x, y, pos, 5) == opposite)
                        value += 10000;
                    if (getPieceType(x, y, pos, -1) == turn
                        && getPieceType(x, y, pos, 1) == empty
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == turn)
                        value += 10000;
                    if (getPieceType(x, y, pos, -1) == opposite
                        && getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == turn
                        && getPieceType(x, y, pos, 4) == empty
                        && getPieceType(x, y, pos, 5) == opposite)
                        value += 10000;
                    //活二
                    if (getPieceType(x, y, pos, 1) == empty
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == turn
                        && getPieceType(x, y, pos, 4) == empty
                        && getPieceType(x, y, pos, 5) == empty)
                        value += 9999;
                    if (getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == empty
                        && getPieceType(x, y, pos, 3) == turn
                        && getPieceType(x, y, pos, 4) == empty)
                        value += 9999;
                    if (getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == empty
                        && getPieceType(x, y, pos, 3) == empty
                        && getPieceType(x, y, pos, 4) == turn
                        && getPieceType(x, y, pos, 5) == empty)
                        value += 9999;
                    //死二
                    if (getPieceType(x, y, pos, 1) == empty
                        && getPieceType(x, y, pos, 2) == empty
                        && getPieceType(x, y, pos, 3) == turn
                        && getPieceType(x, y, pos, 4) == turn
                        && getPieceType(x, y, pos, 5) == opposite)
                        value += 1000;
                    if (getPieceType(x, y, pos, 1) == turn
                        && getPieceType(x, y, pos, 2) == empty
                        && getPieceType(x, y, pos, 3) == empty
                        && getPieceType(x, y, pos, 4) == turn
                        && getPieceType(x, y, pos, 5) == opposite)
                        value += 1000;
                    if (getPieceType(x, y, pos, 1) == empty
                        && getPieceType(x, y, pos, 2) == turn
                        && getPieceType(x, y, pos, 3) == empty
                        && getPieceType(x, y, pos, 4) == turn
                        && getPieceType(x, y, pos, 5) == opposite)
                        value += 1000;
                    if (getPieceType(x, y, pos, -1) == turn
                        && getPieceType(x, y, pos, 1) == empty
                        && getPieceType(x, y, pos, 2) == empty
                        && getPieceType(x, y, pos, 3) == turn)
                        value += 1000;
                    //其他
                    if (getPieceType(x, y, pos, -1) == turn)
                        value += 10;
                    if (getPieceType(x, y, pos, -1) == opposite)
                        value += 10;
                }
            }
            #endregion 评价
            return value;
        }
        //轮到AI下
        private void turnToAI(out int x, out int y)
        {
            int offenseValue = 0, defenseValue = 0, tempValue = 0;
            int offenseTempX = 0, offenseTempY = 0, defenseTempX = 0, defenseTempY = 0;
            for (short i = 0; i < 15; i++)
                for (short j = 0; j < 15; j++)
                {
                    if (checkerboardStatus[i, j] == (short)pieceType.empty)
                    {
                        //tempValue = evaluate(i, j, (short)pieceType.wPiece);
                        //先下评估完价值再删，最后再找价值最大的那个
                        checkerboardStatus[i, j] = (short)pieceType.wPiece;
                        tempValue = evaluate(i, j);
                        checkerboardStatus[i, j] = (short)pieceType.empty;
                        if (offenseValue < tempValue)
                        {
                            offenseValue = tempValue;
                            offenseTempX = i;
                            offenseTempY = j;
                        }
                        //tempValue = evaluate(i, j, (short)pieceType.bPiece);
                        checkerboardStatus[i, j] = (short)pieceType.bPiece;
                        tempValue = evaluate(i, j);
                        checkerboardStatus[i, j] = (short)pieceType.empty;
                        if (defenseValue < tempValue)
                        {
                            defenseValue = tempValue;
                            defenseTempX = i;
                            defenseTempY = j;
                        }
                    }
                }
            if (offenseValue >= defenseValue || offenseValue >= 100000)
            {
                x = offenseTempX;
                y = offenseTempY;
            }
            else
            {
                x = defenseTempX;
                y = defenseTempY;
            }
        }
        private void turnToAI1(out int x, out int y)
        {
            int tempValue = 0;
            int value = 0;
            int tempX = 0, tempY = 0; 
            for (short i = 0; i < 15; i++)
                for (short j = 0; j < 15; j++)
                {
                    if (checkerboardStatus[i, j] == (short)pieceType.empty)
                    {
                        checkerboardStatus[i, j] = (short)pieceType.wPiece;
                        tempValue = alphaBeta(1, -100000000, 100000000, i, j);
                        checkerboardStatus[i, j] = (short)pieceType.empty;
                        if (value < tempValue)
                        {
                            value = tempValue;
                            tempX = i;
                            tempY = j;
                        }                      
                    }
                }
            x = tempX;
            y = tempY;
        }        
        //alphaBeta算法
        private int alphaBeta(int depth, int alpha, int beta,int x,int y)
        {
            int value = 0;
            if (depth == 0)
                return evaluate(x, y);
            //极大节点
            if (checkerboardStatus[x, y] == (short)pieceType.wPiece)
            {
                for (short i = 0; i < 15; i++)
                    for (short j = 0; j < 15; j++)
                    {
                        if (checkerboardStatus[i, j] == (short)pieceType.empty)
                        {
                            checkerboardStatus[i, j] = (short)pieceType.bPiece;
                            value = alphaBeta(depth - 1, alpha, beta, i, j);
                            checkerboardStatus[i, j] = (short)pieceType.empty;
                            if (value > alpha)
                                if (value > beta)
                                    return value;
                                else
                                    alpha = value;
                        }
                    }
                return alpha;
            }
            else
            {
                for (short i = 0; i < 15; i++)
                    for (short j = 0; j < 15; j++)
                    {
                        if (checkerboardStatus[i, j] == (short)pieceType.empty)
                        {
                            checkerboardStatus[i, j] = (short)pieceType.wPiece;
                            value = alphaBeta(depth - 1, alpha, beta, i, j);
                            checkerboardStatus[i, j] = (short)pieceType.empty;
                            if (value < beta)
                                if (value > alpha)
                                    return value;
                                else
                                    beta = value;
                        }
                    }
                return beta;
            }
        }
        #endregion 方法

        #region 事件
        //重绘
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            drawCheckerboard(e);
            drawPiece(e, bRecPos, new SolidBrush(Color.Black));
            drawPiece(e, wRecPos, new SolidBrush(Color.White));
        }
        //下棋
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int i = 0, j = 0;
            int x, y;
            Point pos = new Point();
            #region 对鼠标坐标的处理
            if (e.X - 30 - (e.X - 30) / 40 * 40 <= 20)
            {
                i = (e.X - 30) / 40;
                pos.X = i * 40 + 14;//14=-16 + 30
            }
            else
            {
                i = (e.X - 30) / 40 + 1;
                pos.X = i * 40 + 14;
            }
            if (e.Y - 30 - (e.Y - 30) / 40 * 40 <= 20)
            {
                j = (e.Y - 30) / 40;
                pos.Y = j * 40 + 14;
            }
            else
            {
                j = (e.Y - 30) / 40 + 1;
                pos.Y = j * 40 + 14;
            }
            #endregion 对鼠标坐标的处理
            if (recPos.Contains(pos))
            {
                //MessageBox.Show("棋子已存在，请落别处！");
                return;
            }
            if (flagWin)
                return;            
            if (isHumanVSHuman)
            {
                recPos.Add(pos);
                #region 人人
                if (count % 2 == 0)
                {
                    checkerboardStatus[i, j] = (short)pieceType.bPiece;
                    bRecPos.Add(pos);
                    drawPiece(pos, new SolidBrush(Color.Black));
                    if (isVictory(i, j, (short)pieceType.bPiece))
                    {
                        MessageBox.Show("黑方获胜！");
                        flagWin = true;
                    }
                }
                else
                {
                    checkerboardStatus[i, j] = (short)pieceType.wPiece;
                    wRecPos.Add(pos);
                    drawPiece(pos, new SolidBrush(Color.White));
                    if (isVictory(i, j, (short)pieceType.wPiece))
                    {
                        MessageBox.Show("白方获胜！");
                        flagWin = true;
                    }
                }
                count++;
                #endregion 人人
            }
            else
            {
                recPos.Add(pos);
                #region 人机
                checkerboardStatus[i, j] = (short)pieceType.bPiece;
                bRecPos.Add(pos);
                drawPiece(pos, new SolidBrush(Color.Black));
                if (isVictory(i, j, (short)pieceType.bPiece))
                {
                    MessageBox.Show("黑方获胜！");
                    flagWin = true;
                    return;
                }

                turnToAI(out x, out y);
                //turnToAI1(out x, out y);
                checkerboardStatus[x, y] = (short)pieceType.wPiece;
                #region 对鼠标坐标的处理
                if (e.X - 30 - (e.X - 30) / 40 * 40 <= 20)
                    pos.X = x * 40 + 14;//14=-16 + 30
                else
                    pos.X = x * 40 + 14;
                if (e.Y - 30 - (e.Y - 30) / 40 * 40 <= 20)
                    pos.Y = y * 40 + 14;
                else
                    pos.Y = y * 40 + 14;
                #endregion 对鼠标坐标的处理
                recPos.Add(pos);
                wRecPos.Add(pos);
                drawPiece(pos, new SolidBrush(Color.White));
                if (isVictory(x, y, (short)pieceType.wPiece))
                {
                    MessageBox.Show("白方获胜！");
                    flagWin = true;
                    return;
                }
                #endregion 人机
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            recPos.Clear();
            bRecPos.Clear();
            wRecPos.Clear();
            pictureBox1.Invalidate();
            checkerboardStatus = new short[15, 15];
            count = 0;
            flagWin = false;
        }
        #endregion 事件
    }
}
