using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            short locationX = 10;
            short locationY = 10;
            short width = 650;
            short height = 350;
            Graphics graphics = e.Graphics;
            Rectangle region = new Rectangle(locationX, locationY, width, height);
            Pen myPen = new Pen(Color.Black, 1);
            graphics.DrawRectangle(myPen, region);            
        }
    }
}
