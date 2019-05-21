using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;

//目前解决的一个Bug
//运行程序鼠标单击落子后，若将窗体缩放或者移动至任务栏
//则棋子会被抹掉
//version1.0用PictureBox控件作为棋盘，且处理了边界只画一半棋子的bug
namespace Gomoku1._0_Alpha
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region 私有变量
        //格子大小
        private short gridSize = 40;
        //棋盘规模（如15*15）
        private short checkerboardSize = 15;
        //当前棋子位置
        private Point pieceLocation;
        //棋盘状态（黑棋、白棋、空白）
        private enum checkerboardStatus
        { whitePiece = 0, blackPiece = 1, empty = -1 };
        //棋权（默认黑先）
        private bool moveFirst = true;
        //判断下黑棋还是白棋（默认偶数是下黑棋）
        private short count = 0;
        //判断是否开始
        private bool isBegin = false;
        //记录已下棋子的位置
        List<Point> recordPieceLocation = new List<Point> { };//黑白棋子位置
        List<Point> bRecordPieceLocation = new List<Point> { };//黑棋位置
        List<Point> wRecordPieceLocation = new List<Point> { };//白棋位置
        //黑棋获胜标志
        private bool isbWin = false;
        //白棋获胜标志
        private bool iswWin = false;
        #endregion 私有变量

        #region 方法
        //判断是否获胜
        private bool isWin(List<Point> recordPieceLocation)
        {
            //判断横向五连            
            if (consecNum(recordPieceLocation, gridSize, 0) >= 5)
                return true;
            //判断竖向五连 
            else if (consecNum(recordPieceLocation, 0, gridSize) >= 5)
                return true;
            //判断正斜五连 
            else if (consecNum(recordPieceLocation, gridSize, gridSize) >= 5)
                return true;
            //判断反斜五连 
            else if (consecNum(recordPieceLocation, -gridSize, gridSize) >= 5)
                return true;
            else
                return false;
        }
        //判断连子数
        private short consecNum(List<Point> recordPieceLocation, int offsetX, int offsetY)
        {
            int X = recordPieceLocation.Last().X;
            int Y = recordPieceLocation.Last().Y;
            short n = 1;
            for (int i = 0; i < 5; i++)
            {
                if (recordPieceLocation.Contains(new Point(X + offsetX, Y + offsetY)))
                {
                    n++;
                    X = X + offsetX;
                    Y = Y + offsetY;
                }
                else
                    break;
            }
            X = recordPieceLocation.Last().X;
            Y = recordPieceLocation.Last().Y;
            for (int i = 0; i < 5; i++)
            {
                if (recordPieceLocation.Contains(new Point(X - offsetX, Y - offsetY)))
                {
                    n++;
                    X = X - offsetX;
                    Y = Y - offsetY;
                }
                else
                    break;
            }
            return n;
        }
        private void redraw//在窗体发生变化时，抹去了已经画好的棋子需要对其重绘
            (PictureBox picPaint, List<Point> recordPieceLocation, Brush myBrush)
        {
            Graphics graphics = picPaint.CreateGraphics();
            Graphics gBound = this.CreateGraphics();//窗体和控件边界
            foreach (Point element in recordPieceLocation)
            {
                Rectangle piece = new Rectangle
                        (element.X, element.Y, gridSize * 5 / 6, gridSize * 5 / 6);
                //gBound.FillEllipse(myBrush, piece);屏蔽掉！！！！醉了！！！智商捉急啊！！！
                graphics.FillEllipse(myBrush, piece);
                //边界画棋子判断处理
                if (element.X == -gridSize * 5 / 12 || element.Y == -gridSize * 5 / 12
                    || element.X == picPaint.Width - gridSize * 5 / 12 +1
                    || element.Y == picPaint.Height - gridSize * 5 / 12+1)
                {
                    piece = new Rectangle
                        (element.X + picPaint.Location.X, element.Y + picPaint.Location.Y, gridSize * 5 / 6, gridSize * 5 / 6);
                    //graphics.FillEllipse(myBrush, piece);屏蔽掉！！！！醉了！！！智商捉急啊！！！
                    gBound.FillEllipse(myBrush, piece);
                }
            }
        }
        #endregion 方法

        #region 事件        
        //画棋盘
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Pen myPen = new Pen(Color.Black, 1);
            //画横竖线
            Point pointStart = new Point();
            Point pointEnd = new Point();
            for (short i = 0; i <= checkerboardSize; i++)
            {
                pointStart.X = gridSize * i;
                pointStart.Y = 0;
                pointEnd.X = gridSize * i;
                pointEnd.Y = pictureBox1.Height;
                graphics.DrawLine(myPen, pointStart, pointEnd);
            }
            for (short i = 0; i <= checkerboardSize; i++)
            {
                pointStart.X = 0;
                pointStart.Y = gridSize * i;
                pointEnd.X = pictureBox1.Width;
                pointEnd.Y = gridSize * i;
                graphics.DrawLine(myPen, pointStart, pointEnd);
            }
            //画星位
            //            
            Brush myBrushBlack = new SolidBrush(Color.Black);
            Brush myBrushWhite = new SolidBrush(Color.White);
            redraw(pictureBox1, bRecordPieceLocation, myBrushBlack);
            redraw(pictureBox1, wRecordPieceLocation, myBrushWhite);
        }
        //画棋子
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //未按下开始按钮或已经取得胜利后不准下棋            
            if (!isBegin || isbWin || iswWin)
                return;
            Graphics graphics = pictureBox1.CreateGraphics();
            Graphics gBound = this.CreateGraphics();//窗体和控件边界
            //画棋子的颜色
            Brush myBrushBlack = new SolidBrush(Color.Black);
            Brush myBrushWhite = new SolidBrush(Color.White);
            //画棋子的位置
            if (e.X - e.X / gridSize * gridSize <= gridSize / 2)
                pieceLocation.X = e.X / gridSize * gridSize - gridSize * 5 / 12;
            else
                pieceLocation.X = (e.X / gridSize + 1) * gridSize - gridSize * 5 / 12;
            if (e.Y - e.Y / gridSize * gridSize <= gridSize / 2)
                pieceLocation.Y = e.Y / gridSize * gridSize - gridSize * 5 / 12;
            else
                pieceLocation.Y = (e.Y / gridSize + 1) * gridSize - gridSize * 5 / 12;
            if (!recordPieceLocation.Contains(pieceLocation))
            {
                recordPieceLocation.Add(pieceLocation);
                Rectangle piece = new Rectangle(pieceLocation.X, pieceLocation.Y, gridSize * 5 / 6, gridSize * 5 / 6);
                //判断落子颜色
                if (count % 2 == 0)
                {
                    bRecordPieceLocation.Add(pieceLocation);
                    graphics.FillEllipse(myBrushBlack, piece);
                    //边界画棋子判断处理
                    if (pieceLocation.X == -gridSize * 5 / 12 || pieceLocation.Y == -gridSize * 5 / 12
                        || pieceLocation.X == pictureBox1.Width - gridSize * 5 / 12 - 1
                        || pieceLocation.Y == pictureBox1.Height - gridSize * 5 / 12 - 1)
                    {
                        piece = new Rectangle
                            (pieceLocation.X + pictureBox1.Location.X, pieceLocation.Y + pictureBox1.Location.Y, gridSize * 5 / 6, gridSize * 5 / 6);
                        gBound.FillEllipse(myBrushBlack, piece);
                    }
                    if (isWin(bRecordPieceLocation))
                    {
                        MessageBox.Show("黑方获胜！");
                        isbWin = true;
                        //return;注意这里用return没用，应该在最开始处返回才不会有接下来的操作                        
                    }
                }
                else
                {
                    wRecordPieceLocation.Add(pieceLocation);
                    graphics.FillEllipse(myBrushWhite, piece);
                    //边界画棋子判断处理
                    if (pieceLocation.X == -gridSize * 5 / 12 || pieceLocation.Y == -gridSize * 5 / 12
                        || pieceLocation.X == pictureBox1.Width - gridSize * 5 / 12 - 1
                        || pieceLocation.Y == pictureBox1.Height - gridSize * 5 / 12 - 1)
                    {
                        piece = new Rectangle
                            (pieceLocation.X + pictureBox1.Location.X, pieceLocation.Y + pictureBox1.Location.Y, gridSize * 5 / 6, gridSize * 5 / 6);
                        gBound.FillEllipse(myBrushWhite, piece);
                    }
                    if (isWin(wRecordPieceLocation))
                    {
                        MessageBox.Show("白方获胜！");
                        iswWin = true;
                    }
                }
                count++;//轮到另一方落子
            }
            else
                MessageBox.Show("棋子已存在，请落别处！");
        }
        //开始按钮
        private void button1_Click(object sender, EventArgs e)
        {
            labIsStart.Text = "游戏开始";
            isBegin = true;
            isbWin = false;
            iswWin = false;
            //pictureBox1.Invalidate();
            recordPieceLocation.Clear();
            bRecordPieceLocation.Clear();
            wRecordPieceLocation.Clear();
            this.Refresh();//直接清除画的棋子，但需要先清空存棋子的List不然没办法把边界的Form上的棋子清空
            if (radBtnWhite.Checked)
                moveFirst = false;
            if (moveFirst)
                count = 0;
            else
                count = 1;
        }
        //结束按钮
        private void button2_Click(object sender, EventArgs e)
        {
            //可增加保存棋谱到数据库 
            if (isBegin)
            {
                labIsStart.Text = "游戏结束";
                DialogResult dialogResult = MessageBox.Show("是否保存棋谱？", "保存消息框", MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.OK)
                    MessageBox.Show("保存成功！");
                recordPieceLocation.Clear();
                bRecordPieceLocation.Clear();
                wRecordPieceLocation.Clear();
            }
            else
                MessageBox.Show("请先开始游戏！");
            isBegin = false;//可以在没赢的情况下结束            
        }
        #endregion 事件        
    }
}
