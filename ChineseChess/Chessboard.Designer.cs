namespace ChineseChess
{
    partial class Chessboard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.StartButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.XLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.XCoordinate = new System.Windows.Forms.Label();
            this.YCoordinate = new System.Windows.Forms.Label();
            this.TypeStatus = new System.Windows.Forms.Label();
            this.TypeStatusLabel = new System.Windows.Forms.Label();
            this.UndoButton = new System.Windows.Forms.Button();
            this.dangerLabel = new System.Windows.Forms.Label();
            this.TestButton = new System.Windows.Forms.Button();
            this.EngineInfoText = new System.Windows.Forms.RichTextBox();
            this.RedRadioButton = new System.Windows.Forms.RadioButton();
            this.BlackRadioButton = new System.Windows.Forms.RadioButton();
            this.PlayerTypeLabel = new System.Windows.Forms.Label();
            this.EngineOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.ChangeEngineButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.AntiqueWhite;
            this.panel1.Location = new System.Drawing.Point(12, 11);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(629, 607);
            this.panel1.TabIndex = 0;
            this.panel1.Click += new System.EventHandler(this.panel1_Click);
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(771, 306);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(75, 53);
            this.StartButton.TabIndex = 1;
            this.StartButton.Text = "开始";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // XLabel
            // 
            this.XLabel.AutoSize = true;
            this.XLabel.Location = new System.Drawing.Point(709, 480);
            this.XLabel.Name = "XLabel";
            this.XLabel.Size = new System.Drawing.Size(83, 12);
            this.XLabel.TabIndex = 2;
            this.XLabel.Text = "X coordinate:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(709, 506);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Y coordinate:";
            // 
            // XCoordinate
            // 
            this.XCoordinate.AutoSize = true;
            this.XCoordinate.Location = new System.Drawing.Point(793, 480);
            this.XCoordinate.Name = "XCoordinate";
            this.XCoordinate.Size = new System.Drawing.Size(41, 12);
            this.XCoordinate.TabIndex = 2;
            this.XCoordinate.Text = "label1";
            // 
            // YCoordinate
            // 
            this.YCoordinate.AutoSize = true;
            this.YCoordinate.Location = new System.Drawing.Point(793, 506);
            this.YCoordinate.Name = "YCoordinate";
            this.YCoordinate.Size = new System.Drawing.Size(41, 12);
            this.YCoordinate.TabIndex = 3;
            this.YCoordinate.Text = "label2";
            // 
            // TypeStatus
            // 
            this.TypeStatus.AutoSize = true;
            this.TypeStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TypeStatus.Location = new System.Drawing.Point(708, 109);
            this.TypeStatus.Name = "TypeStatus";
            this.TypeStatus.Size = new System.Drawing.Size(48, 24);
            this.TypeStatus.TabIndex = 4;
            this.TypeStatus.Text = "红方";
            // 
            // TypeStatusLabel
            // 
            this.TypeStatusLabel.AutoSize = true;
            this.TypeStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TypeStatusLabel.Location = new System.Drawing.Point(767, 109);
            this.TypeStatusLabel.Name = "TypeStatusLabel";
            this.TypeStatusLabel.Size = new System.Drawing.Size(48, 24);
            this.TypeStatusLabel.TabIndex = 2;
            this.TypeStatusLabel.Text = "着棋";
            // 
            // UndoButton
            // 
            this.UndoButton.Enabled = false;
            this.UndoButton.Location = new System.Drawing.Point(771, 388);
            this.UndoButton.Name = "UndoButton";
            this.UndoButton.Size = new System.Drawing.Size(75, 46);
            this.UndoButton.TabIndex = 5;
            this.UndoButton.Text = "悔棋";
            this.UndoButton.UseVisualStyleBackColor = true;
            this.UndoButton.Click += new System.EventHandler(this.UndoButton_Click);
            // 
            // dangerLabel
            // 
            this.dangerLabel.AutoSize = true;
            this.dangerLabel.BackColor = System.Drawing.Color.PeachPuff;
            this.dangerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dangerLabel.ForeColor = System.Drawing.Color.Red;
            this.dangerLabel.Location = new System.Drawing.Point(708, 72);
            this.dangerLabel.Name = "dangerLabel";
            this.dangerLabel.Size = new System.Drawing.Size(86, 24);
            this.dangerLabel.TabIndex = 6;
            this.dangerLabel.Text = "    将！  ";
            this.dangerLabel.Visible = false;
            // 
            // TestButton
            // 
            this.TestButton.Location = new System.Drawing.Point(923, 306);
            this.TestButton.Name = "TestButton";
            this.TestButton.Size = new System.Drawing.Size(75, 53);
            this.TestButton.TabIndex = 9;
            this.TestButton.Text = "Test";
            this.TestButton.UseVisualStyleBackColor = true;
            this.TestButton.Click += new System.EventHandler(this.TestButton_Click);
            // 
            // EngineInfoText
            // 
            this.EngineInfoText.Location = new System.Drawing.Point(857, 93);
            this.EngineInfoText.Name = "EngineInfoText";
            this.EngineInfoText.Size = new System.Drawing.Size(191, 171);
            this.EngineInfoText.TabIndex = 10;
            this.EngineInfoText.Text = "";
            // 
            // RedRadioButton
            // 
            this.RedRadioButton.AutoSize = true;
            this.RedRadioButton.Checked = true;
            this.RedRadioButton.Location = new System.Drawing.Point(872, 42);
            this.RedRadioButton.Name = "RedRadioButton";
            this.RedRadioButton.Size = new System.Drawing.Size(47, 16);
            this.RedRadioButton.TabIndex = 11;
            this.RedRadioButton.TabStop = true;
            this.RedRadioButton.Text = "红子";
            this.RedRadioButton.UseVisualStyleBackColor = true;
            this.RedRadioButton.CheckedChanged += new System.EventHandler(this.RedRadioButton_CheckedChanged);
            // 
            // BlackRadioButton
            // 
            this.BlackRadioButton.AutoSize = true;
            this.BlackRadioButton.Location = new System.Drawing.Point(927, 42);
            this.BlackRadioButton.Name = "BlackRadioButton";
            this.BlackRadioButton.Size = new System.Drawing.Size(47, 16);
            this.BlackRadioButton.TabIndex = 12;
            this.BlackRadioButton.Text = "黑棋";
            this.BlackRadioButton.UseVisualStyleBackColor = true;
            this.BlackRadioButton.CheckedChanged += new System.EventHandler(this.BlackRadioButton_CheckedChanged);
            // 
            // PlayerTypeLabel
            // 
            this.PlayerTypeLabel.AutoSize = true;
            this.PlayerTypeLabel.Location = new System.Drawing.Point(793, 43);
            this.PlayerTypeLabel.Name = "PlayerTypeLabel";
            this.PlayerTypeLabel.Size = new System.Drawing.Size(53, 12);
            this.PlayerTypeLabel.TabIndex = 13;
            this.PlayerTypeLabel.Text = "玩家类型";
            // 
            // EngineOpenFileDialog
            // 
            this.EngineOpenFileDialog.FileName = "openFileDialog1";
            // 
            // ChangeEngineButton
            // 
            this.ChangeEngineButton.Location = new System.Drawing.Point(681, 11);
            this.ChangeEngineButton.Name = "ChangeEngineButton";
            this.ChangeEngineButton.Size = new System.Drawing.Size(75, 44);
            this.ChangeEngineButton.TabIndex = 14;
            this.ChangeEngineButton.Text = "更换引擎";
            this.ChangeEngineButton.UseVisualStyleBackColor = true;
            this.ChangeEngineButton.Click += new System.EventHandler(this.ChangeEngineButton_Click);
            // 
            // Chessboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1114, 629);
            this.Controls.Add(this.ChangeEngineButton);
            this.Controls.Add(this.PlayerTypeLabel);
            this.Controls.Add(this.BlackRadioButton);
            this.Controls.Add(this.RedRadioButton);
            this.Controls.Add(this.EngineInfoText);
            this.Controls.Add(this.TestButton);
            this.Controls.Add(this.dangerLabel);
            this.Controls.Add(this.UndoButton);
            this.Controls.Add(this.TypeStatus);
            this.Controls.Add(this.YCoordinate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TypeStatusLabel);
            this.Controls.Add(this.XCoordinate);
            this.Controls.Add(this.XLabel);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.panel1);
            this.Name = "Chessboard";
            this.Text = "中国象棋";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Chessboard_FormClosing);
            this.Load += new System.EventHandler(this.Chessboard_Load);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Chessboard_MouseClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Chessboard_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label XLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label XCoordinate;
        private System.Windows.Forms.Label YCoordinate;
        private System.Windows.Forms.Label TypeStatus;
        private System.Windows.Forms.Label TypeStatusLabel;
        private System.Windows.Forms.Button UndoButton;
        private System.Windows.Forms.Label dangerLabel;
        private System.Windows.Forms.Button TestButton;
        private System.Windows.Forms.RichTextBox EngineInfoText;
        private System.Windows.Forms.RadioButton RedRadioButton;
        private System.Windows.Forms.RadioButton BlackRadioButton;
        private System.Windows.Forms.Label PlayerTypeLabel;
        private System.Windows.Forms.OpenFileDialog EngineOpenFileDialog;
        private System.Windows.Forms.Button ChangeEngineButton;




    }
}

