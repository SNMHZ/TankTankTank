namespace Tank_Practice
{
    partial class ConnectorForm
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
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label_port = new System.Windows.Forms.Label();
            this.label_addr = new System.Windows.Forms.Label();
            this.ConnectLoglistBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(179, 12);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 0;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(179, 41);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(73, 12);
            this.textBox1.MaxLength = 5;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 2;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(73, 39);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 21);
            this.textBox2.TabIndex = 3;
            // 
            // label_port
            // 
            this.label_port.AutoSize = true;
            this.label_port.Location = new System.Drawing.Point(9, 17);
            this.label_port.Name = "label_port";
            this.label_port.Size = new System.Drawing.Size(58, 12);
            this.label_port.TabIndex = 4;
            this.label_port.Text = "Port Num";
            // 
            // label_addr
            // 
            this.label_addr.AutoSize = true;
            this.label_addr.Location = new System.Drawing.Point(15, 42);
            this.label_addr.Name = "label_addr";
            this.label_addr.Size = new System.Drawing.Size(52, 12);
            this.label_addr.TabIndex = 5;
            this.label_addr.Text = "Address";
            // 
            // ConnectLoglistBox
            // 
            this.ConnectLoglistBox.FormattingEnabled = true;
            this.ConnectLoglistBox.ItemHeight = 12;
            this.ConnectLoglistBox.Location = new System.Drawing.Point(11, 71);
            this.ConnectLoglistBox.Name = "ConnectLoglistBox";
            this.ConnectLoglistBox.Size = new System.Drawing.Size(243, 76);
            this.ConnectLoglistBox.TabIndex = 6;
            // 
            // ConnectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 152);
            this.Controls.Add(this.ConnectLoglistBox);
            this.Controls.Add(this.label_addr);
            this.Controls.Add(this.label_port);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnOpen);
            this.Name = "ConnectorForm";
            this.Text = "ConnectorForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConnectorForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label label_port;
        private System.Windows.Forms.Label label_addr;
        public System.Windows.Forms.ListBox ConnectLoglistBox;
        public System.Windows.Forms.TextBox textBox1;
        public System.Windows.Forms.TextBox textBox2;
    }
}