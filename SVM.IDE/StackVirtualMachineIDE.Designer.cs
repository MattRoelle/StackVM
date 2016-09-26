namespace SVM.IDE
{
    partial class StackVirtualMachineIDE
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
            this.CodePanel = new System.Windows.Forms.GroupBox();
            this.ResetAndLoadButton = new System.Windows.Forms.Button();
            this.CodeInput = new System.Windows.Forms.TextBox();
            this.Debugging = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.StepButton = new System.Windows.Forms.Button();
            this.SPTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.IPTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.StackListBox = new System.Windows.Forms.ListBox();
            this.OutputLabel = new System.Windows.Forms.GroupBox();
            this.Output = new System.Windows.Forms.TextBox();
            this.CodePanel.SuspendLayout();
            this.Debugging.SuspendLayout();
            this.OutputLabel.SuspendLayout();
            this.SuspendLayout();
            // 
            // CodePanel
            // 
            this.CodePanel.Controls.Add(this.ResetAndLoadButton);
            this.CodePanel.Controls.Add(this.CodeInput);
            this.CodePanel.Location = new System.Drawing.Point(13, 13);
            this.CodePanel.Name = "CodePanel";
            this.CodePanel.Size = new System.Drawing.Size(263, 463);
            this.CodePanel.TabIndex = 0;
            this.CodePanel.TabStop = false;
            this.CodePanel.Text = "Input";
            // 
            // ResetAndLoadButton
            // 
            this.ResetAndLoadButton.Location = new System.Drawing.Point(7, 434);
            this.ResetAndLoadButton.Name = "ResetAndLoadButton";
            this.ResetAndLoadButton.Size = new System.Drawing.Size(250, 23);
            this.ResetAndLoadButton.TabIndex = 1;
            this.ResetAndLoadButton.Text = "Reset and Load";
            this.ResetAndLoadButton.UseVisualStyleBackColor = true;
            this.ResetAndLoadButton.Click += new System.EventHandler(this.ResetAndLoadButton_Click);
            // 
            // CodeInput
            // 
            this.CodeInput.Location = new System.Drawing.Point(7, 20);
            this.CodeInput.Multiline = true;
            this.CodeInput.Name = "CodeInput";
            this.CodeInput.Size = new System.Drawing.Size(250, 408);
            this.CodeInput.TabIndex = 0;
            this.CodeInput.Text = "PSH $0000\r\nPSH $0001\r\nADD\r\nJMP $1003";
            // 
            // Debugging
            // 
            this.Debugging.Controls.Add(this.label3);
            this.Debugging.Controls.Add(this.StepButton);
            this.Debugging.Controls.Add(this.SPTextBox);
            this.Debugging.Controls.Add(this.label2);
            this.Debugging.Controls.Add(this.IPTextBox);
            this.Debugging.Controls.Add(this.label1);
            this.Debugging.Controls.Add(this.StackListBox);
            this.Debugging.Location = new System.Drawing.Point(283, 13);
            this.Debugging.Name = "Debugging";
            this.Debugging.Size = new System.Drawing.Size(373, 323);
            this.Debugging.TabIndex = 1;
            this.Debugging.TabStop = false;
            this.Debugging.Text = "Debugging";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Stack";
            // 
            // StepButton
            // 
            this.StepButton.Location = new System.Drawing.Point(222, 45);
            this.StepButton.Name = "StepButton";
            this.StepButton.Size = new System.Drawing.Size(145, 23);
            this.StepButton.TabIndex = 5;
            this.StepButton.Text = "Step";
            this.StepButton.UseVisualStyleBackColor = true;
            this.StepButton.Click += new System.EventHandler(this.StepButton_Click);
            // 
            // SPTextBox
            // 
            this.SPTextBox.Location = new System.Drawing.Point(115, 71);
            this.SPTextBox.Name = "SPTextBox";
            this.SPTextBox.ReadOnly = true;
            this.SPTextBox.Size = new System.Drawing.Size(100, 20);
            this.SPTextBox.TabIndex = 4;
            this.SPTextBox.TextChanged += new System.EventHandler(this.SPTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(91, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "SP";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // IPTextBox
            // 
            this.IPTextBox.Location = new System.Drawing.Point(115, 45);
            this.IPTextBox.Name = "IPTextBox";
            this.IPTextBox.ReadOnly = true;
            this.IPTextBox.Size = new System.Drawing.Size(100, 20);
            this.IPTextBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(91, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "IP";
            // 
            // StackListBox
            // 
            this.StackListBox.FormattingEnabled = true;
            this.StackListBox.Location = new System.Drawing.Point(6, 45);
            this.StackListBox.Name = "StackListBox";
            this.StackListBox.Size = new System.Drawing.Size(78, 264);
            this.StackListBox.TabIndex = 0;
            // 
            // OutputLabel
            // 
            this.OutputLabel.Controls.Add(this.Output);
            this.OutputLabel.Location = new System.Drawing.Point(282, 342);
            this.OutputLabel.Name = "OutputLabel";
            this.OutputLabel.Size = new System.Drawing.Size(367, 134);
            this.OutputLabel.TabIndex = 2;
            this.OutputLabel.TabStop = false;
            this.OutputLabel.Text = "Output";
            // 
            // Output
            // 
            this.Output.Location = new System.Drawing.Point(9, 20);
            this.Output.Multiline = true;
            this.Output.Name = "Output";
            this.Output.ReadOnly = true;
            this.Output.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.Output.Size = new System.Drawing.Size(352, 108);
            this.Output.TabIndex = 0;
            // 
            // StackVirtualMachineIDE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 488);
            this.Controls.Add(this.OutputLabel);
            this.Controls.Add(this.Debugging);
            this.Controls.Add(this.CodePanel);
            this.Name = "StackVirtualMachineIDE";
            this.Text = "StackVirtualMachine IDE";
            this.CodePanel.ResumeLayout(false);
            this.CodePanel.PerformLayout();
            this.Debugging.ResumeLayout(false);
            this.Debugging.PerformLayout();
            this.OutputLabel.ResumeLayout(false);
            this.OutputLabel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox CodePanel;
        private System.Windows.Forms.TextBox CodeInput;
        private System.Windows.Forms.GroupBox Debugging;
        private System.Windows.Forms.Button ResetAndLoadButton;
        private System.Windows.Forms.Button StepButton;
        private System.Windows.Forms.TextBox SPTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox IPTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox StackListBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox OutputLabel;
        private System.Windows.Forms.TextBox Output;
    }
}

