namespace MiddlewarePcPos2080WinForms
{
    partial class fMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            listBox1 = new ListBox();
            timerWaitingPos = new System.Windows.Forms.Timer(components);
            lbState = new Label();
            timerPullRequests = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBox1.FormattingEnabled = true;
            listBox1.Location = new Point(12, 126);
            listBox1.Name = "listBox1";
            listBox1.RightToLeft = RightToLeft.No;
            listBox1.Size = new Size(850, 356);
            listBox1.TabIndex = 3;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // timerWaitingPos
            // 
            timerWaitingPos.Interval = 1000;
            timerWaitingPos.Tick += timerWaitingPos_Tick;
            // 
            // lbState
            // 
            lbState.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lbState.Font = new Font("Tahoma", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbState.Location = new Point(12, 3);
            lbState.Name = "lbState";
            lbState.RightToLeft = RightToLeft.No;
            lbState.Size = new Size(850, 105);
            lbState.TabIndex = 4;
            lbState.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // timerPullRequests
            // 
            timerPullRequests.Tick += timerPullRequests_Tick;
            // 
            // fMain
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(874, 486);
            Controls.Add(lbState);
            Controls.Add(listBox1);
            Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "fMain";
            RightToLeft = RightToLeft.Yes;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "رابط کاربری اتصال به دستگاه پوز  - 2080";
            Shown += fMain_Shown;
            ResumeLayout(false);
        }

        #endregion
        private ListBox listBox1;
        private System.Windows.Forms.Timer timerWaitingPos;
        private Label lbState;
        private System.Windows.Forms.Timer timerPullRequests;
    }
}
