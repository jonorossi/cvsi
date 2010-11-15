namespace Castle.VisualStudio.MonoRailIntelliSenseProvider.Demo
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.binDirectoryTextBox = new System.Windows.Forms.TextBox();
			this.loadButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.elapsedTimeLabel = new System.Windows.Forms.Label();
			this.typesTreeView = new System.Windows.Forms.TreeView();
			this.treeViewImageList = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// binDirectoryTextBox
			// 
			this.binDirectoryTextBox.Location = new System.Drawing.Point(85, 15);
			this.binDirectoryTextBox.Name = "binDirectoryTextBox";
			this.binDirectoryTextBox.Size = new System.Drawing.Size(614, 20);
			this.binDirectoryTextBox.TabIndex = 0;
			this.binDirectoryTextBox.Text = "C:\\Documents and Settings\\Jonathon Rossi\\Desktop\\Uni-CheckInUniFromDesktop\\__Sync" +
				"\\CVSIDemo\\src\\bin";
			// 
			// loadButton
			// 
			this.loadButton.Location = new System.Drawing.Point(705, 12);
			this.loadButton.Name = "loadButton";
			this.loadButton.Size = new System.Drawing.Size(75, 23);
			this.loadButton.TabIndex = 1;
			this.loadButton.Text = "Load";
			this.loadButton.UseVisualStyleBackColor = true;
			this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(70, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Bin Directory:";
			// 
			// elapsedTimeLabel
			// 
			this.elapsedTimeLabel.AutoSize = true;
			this.elapsedTimeLabel.Location = new System.Drawing.Point(9, 544);
			this.elapsedTimeLabel.Name = "elapsedTimeLabel";
			this.elapsedTimeLabel.Size = new System.Drawing.Size(35, 13);
			this.elapsedTimeLabel.TabIndex = 5;
			this.elapsedTimeLabel.Text = "label3";
			// 
			// typesTreeView
			// 
			this.typesTreeView.Location = new System.Drawing.Point(12, 41);
			this.typesTreeView.Name = "typesTreeView";
			this.typesTreeView.Size = new System.Drawing.Size(768, 500);
			this.typesTreeView.TabIndex = 6;
			// 
			// treeViewImageList
			// 
			this.treeViewImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeViewImageList.ImageStream")));
			this.treeViewImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.treeViewImageList.Images.SetKeyName(0, "Assembly");
			this.treeViewImageList.Images.SetKeyName(1, "Class");
			this.treeViewImageList.Images.SetKeyName(2, "Error");
			this.treeViewImageList.Images.SetKeyName(3, "Field");
			this.treeViewImageList.Images.SetKeyName(4, "Information");
			this.treeViewImageList.Images.SetKeyName(5, "Macro");
			this.treeViewImageList.Images.SetKeyName(6, "Method");
			this.treeViewImageList.Images.SetKeyName(7, "Namespace");
			this.treeViewImageList.Images.SetKeyName(8, "Property");
			this.treeViewImageList.Images.SetKeyName(9, "Variable");
			this.treeViewImageList.Images.SetKeyName(10, "Warning");
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 566);
			this.Controls.Add(this.typesTreeView);
			this.Controls.Add(this.elapsedTimeLabel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.loadButton);
			this.Controls.Add(this.binDirectoryTextBox);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MonoRail IntelliSense Provider Demo";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox binDirectoryTextBox;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label elapsedTimeLabel;
        private System.Windows.Forms.TreeView typesTreeView;
        private System.Windows.Forms.ImageList treeViewImageList;
    }
}

