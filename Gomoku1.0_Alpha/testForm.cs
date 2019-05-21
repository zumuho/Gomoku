using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gomoku1._0_Alpha
{
    public partial class testForm : Form
    {
        public testForm()
        {
            InitializeComponent();
        }
        private void SetAndFillClip(PaintEventArgs e)
        {

            // Set the Clip property to a new region.
            e.Graphics.Clip = new Region(new Rectangle(10, 10, 100, 200));

            // Fill the region.
            e.Graphics.FillRegion(Brushes.LightSalmon, e.Graphics.Clip);

            // Demonstrate the clip region by drawing a string
            // at the outer edge of the region.
            e.Graphics.DrawString("Outside of Clip", new Font("Arial",
                12.0F, FontStyle.Regular), Brushes.Black, 0.0F, 0.0F);

            e.Graphics.DrawEllipse(new Pen(Color.Black, 1), 10, 10, 20, 20);
        }

        private void testForm_Paint(object sender, PaintEventArgs e)
        {
            SetAndFillClip(e);
        }       
    }
}
