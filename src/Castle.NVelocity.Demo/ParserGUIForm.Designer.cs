namespace Castle.NVelocity.Demo
{
    partial class ParserGUIForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.parseTreeView = new System.Windows.Forms.TreeView();
            this.label2 = new System.Windows.Forms.Label();
            this.errorsListView = new System.Windows.Forms.ListView();
            this.errorsDescriptionColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.label3 = new System.Windows.Forms.Label();
            this.templateTextBox = new System.Windows.Forms.TextBox();
            this.astNodeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Template:";
            // 
            // parseTreeView
            // 
            this.parseTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.parseTreeView.Location = new System.Drawing.Point(408, 25);
            this.parseTreeView.Name = "parseTreeView";
            this.parseTreeView.Size = new System.Drawing.Size(372, 398);
            this.parseTreeView.TabIndex = 2;
            this.parseTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.parseTreeView_AfterSelect);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(405, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Parse Tree:";
            // 
            // errorsListView
            // 
            this.errorsListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.errorsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.errorsDescriptionColumnHeader});
            this.errorsListView.Location = new System.Drawing.Point(15, 458);
            this.errorsListView.Name = "errorsListView";
            this.errorsListView.Size = new System.Drawing.Size(765, 96);
            this.errorsListView.TabIndex = 4;
            this.errorsListView.UseCompatibleStateImageBehavior = false;
            this.errorsListView.View = System.Windows.Forms.View.Details;
            // 
            // errorsDescriptionColumnHeader
            // 
            this.errorsDescriptionColumnHeader.Text = "Description";
            this.errorsDescriptionColumnHeader.Width = 500;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 442);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Errors:";
            // 
            // templateTextBox
            // 
            this.templateTextBox.AcceptsTab = true;
            this.templateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.templateTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.templateTextBox.Location = new System.Drawing.Point(15, 25);
            this.templateTextBox.Multiline = true;
            this.templateTextBox.Name = "templateTextBox";
            this.templateTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.templateTextBox.Size = new System.Drawing.Size(387, 414);
            this.templateTextBox.TabIndex = 7;
            this.templateTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.templateTextBox_KeyUp);
            this.templateTextBox.TextChanged += new System.EventHandler(this.templateTextBox_TextChanged);
            // 
            // astNodeLabel
            // 
            this.astNodeLabel.Location = new System.Drawing.Point(408, 426);
            this.astNodeLabel.Name = "astNodeLabel";
            this.astNodeLabel.Size = new System.Drawing.Size(372, 13);
            this.astNodeLabel.TabIndex = 8;
            this.astNodeLabel.Text = "No node selected.";
            // 
            // ParserGUIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 566);
            this.Controls.Add(this.astNodeLabel);
            this.Controls.Add(this.templateTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.errorsListView);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.parseTreeView);
            this.Controls.Add(this.label1);
            this.Name = "ParserGUIForm";
            this.Text = "Parser GUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ParserGuiForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView parseTreeView;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView errorsListView;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ColumnHeader errorsDescriptionColumnHeader;
        private System.Windows.Forms.TextBox templateTextBox;
        private System.Windows.Forms.Label astNodeLabel;
    }
}
