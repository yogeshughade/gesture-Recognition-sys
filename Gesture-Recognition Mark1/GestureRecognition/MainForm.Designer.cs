namespace GestureRecognition
{
    partial class MainForm
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
            this.cameraComboBox = new System.Windows.Forms.ComboBox();
            this.startButton = new System.Windows.Forms.Button();
            this.capturedImage = new System.Windows.Forms.PictureBox();
            this.stopButton = new System.Windows.Forms.Button();
            this.thresholdNumeric = new System.Windows.Forms.NumericUpDown();
            this.webcamImage = new System.Windows.Forms.PictureBox();
            this.textBox = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.recognizedGesture = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.capturedImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.thresholdNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.webcamImage)).BeginInit();
            this.SuspendLayout();
            // 
            // cameraComboBox
            // 
            this.cameraComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cameraComboBox.FormattingEnabled = true;
            this.cameraComboBox.Location = new System.Drawing.Point(24, 30);
            this.cameraComboBox.Name = "cameraComboBox";
            this.cameraComboBox.Size = new System.Drawing.Size(121, 21);
            this.cameraComboBox.TabIndex = 0;
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(151, 29);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(91, 21);
            this.startButton.TabIndex = 1;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // capturedImage
            // 
            this.capturedImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.capturedImage.Location = new System.Drawing.Point(24, 77);
            this.capturedImage.Name = "capturedImage";
            this.capturedImage.Size = new System.Drawing.Size(640, 480);
            this.capturedImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.capturedImage.TabIndex = 2;
            this.capturedImage.TabStop = false;
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(248, 29);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(91, 21);
            this.stopButton.TabIndex = 3;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // thresholdNumeric
            // 
            this.thresholdNumeric.Location = new System.Drawing.Point(345, 31);
            this.thresholdNumeric.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.thresholdNumeric.Name = "thresholdNumeric";
            this.thresholdNumeric.ReadOnly = true;
            this.thresholdNumeric.Size = new System.Drawing.Size(46, 20);
            this.thresholdNumeric.TabIndex = 5;
            this.thresholdNumeric.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
            // 
            // webcamImage
            // 
            this.webcamImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.webcamImage.Location = new System.Drawing.Point(687, 77);
            this.webcamImage.Name = "webcamImage";
            this.webcamImage.Size = new System.Drawing.Size(320, 240);
            this.webcamImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.webcamImage.TabIndex = 7;
            this.webcamImage.TabStop = false;
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(687, 323);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox.Size = new System.Drawing.Size(320, 234);
            this.textBox.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Webcams list";
            // 
            // recognizedGesture
            // 
            this.recognizedGesture.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.recognizedGesture.Location = new System.Drawing.Point(413, 31);
            this.recognizedGesture.Name = "recognizedGesture";
            this.recognizedGesture.ReadOnly = true;
            this.recognizedGesture.Size = new System.Drawing.Size(251, 20);
            this.recognizedGesture.TabIndex = 14;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(340, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Threshold";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(684, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Webcam";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(24, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(168, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Captured and proccesed frame";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(413, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Recognized gesture";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1019, 586);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.recognizedGesture);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.webcamImage);
            this.Controls.Add(this.thresholdNumeric);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.capturedImage);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.cameraComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.Text = "GestureRecognition";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.capturedImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.thresholdNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.webcamImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cameraComboBox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.PictureBox capturedImage;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.NumericUpDown thresholdNumeric;
        private System.Windows.Forms.PictureBox webcamImage;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox recognizedGesture;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}

