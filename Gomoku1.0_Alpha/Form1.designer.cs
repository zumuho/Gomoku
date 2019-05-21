namespace Gomoku1._0_Alpha
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.radBtnBlack = new System.Windows.Forms.RadioButton();
            this.radBtnWhite = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.labIsStart = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Peru;
            this.pictureBox1.Location = new System.Drawing.Point(30, 30);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(561, 561);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            // 
            // radBtnBlack
            // 
            this.radBtnBlack.AutoSize = true;
            this.radBtnBlack.Checked = true;
            this.radBtnBlack.Font = new System.Drawing.Font("宋体", 12F);
            this.radBtnBlack.Location = new System.Drawing.Point(624, 73);
            this.radBtnBlack.Name = "radBtnBlack";
            this.radBtnBlack.Size = new System.Drawing.Size(58, 20);
            this.radBtnBlack.TabIndex = 0;
            this.radBtnBlack.TabStop = true;
            this.radBtnBlack.Text = "黑先";
            this.radBtnBlack.UseVisualStyleBackColor = true;
            // 
            // radBtnWhite
            // 
            this.radBtnWhite.AutoSize = true;
            this.radBtnWhite.Font = new System.Drawing.Font("宋体", 12F);
            this.radBtnWhite.Location = new System.Drawing.Point(624, 105);
            this.radBtnWhite.Name = "radBtnWhite";
            this.radBtnWhite.Size = new System.Drawing.Size(58, 20);
            this.radBtnWhite.TabIndex = 1;
            this.radBtnWhite.TabStop = true;
            this.radBtnWhite.Text = "白先";
            this.radBtnWhite.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.button1.Font = new System.Drawing.Font("宋体", 12F);
            this.button1.Location = new System.Drawing.Point(624, 144);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 43);
            this.button1.TabIndex = 2;
            this.button1.Text = "开始";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.IndianRed;
            this.button2.Font = new System.Drawing.Font("宋体", 12F);
            this.button2.Location = new System.Drawing.Point(624, 213);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(88, 43);
            this.button2.TabIndex = 3;
            this.button2.Text = "结束";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // labIsStart
            // 
            this.labIsStart.AutoSize = true;
            this.labIsStart.Font = new System.Drawing.Font("宋体", 12F);
            this.labIsStart.Location = new System.Drawing.Point(622, 285);
            this.labIsStart.Name = "labIsStart";
            this.labIsStart.Size = new System.Drawing.Size(0, 16);
            this.labIsStart.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 621);
            this.Controls.Add(this.labIsStart);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.radBtnWhite);
            this.Controls.Add(this.radBtnBlack);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RadioButton radBtnBlack;
        private System.Windows.Forms.RadioButton radBtnWhite;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label labIsStart;
    }
}

