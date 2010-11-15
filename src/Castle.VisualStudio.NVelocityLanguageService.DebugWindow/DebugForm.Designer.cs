// Copyright 2007-2008 Jonathon Rossi - http://www.jonorossi.com/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.VisualStudio.NVelocityLanguageService.DebugWindow
{
    partial class DebugForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebugForm));
            this.currentNodeLabel = new System.Windows.Forms.Label();
            this.currentPositionLabel = new System.Windows.Forms.Label();
            this.scopeListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.typeImageList = new System.Windows.Forms.ImageList(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.astTreeView = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // currentNodeLabel
            // 
            this.currentNodeLabel.AutoSize = true;
            this.currentNodeLabel.Location = new System.Drawing.Point(135, 35);
            this.currentNodeLabel.Name = "currentNodeLabel";
            this.currentNodeLabel.Size = new System.Drawing.Size(76, 13);
            this.currentNodeLabel.TabIndex = 0;
            this.currentNodeLabel.Text = "[Current Node]";
            // 
            // currentPositionLabel
            // 
            this.currentPositionLabel.AutoSize = true;
            this.currentPositionLabel.Location = new System.Drawing.Point(135, 9);
            this.currentPositionLabel.Name = "currentPositionLabel";
            this.currentPositionLabel.Size = new System.Drawing.Size(87, 13);
            this.currentPositionLabel.TabIndex = 1;
            this.currentPositionLabel.Text = "[Current Position]";
            // 
            // scopeListView
            // 
            this.scopeListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.scopeListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.scopeListView.FullRowSelect = true;
            this.scopeListView.Location = new System.Drawing.Point(6, 16);
            this.scopeListView.Name = "scopeListView";
            this.scopeListView.Size = new System.Drawing.Size(273, 571);
            this.scopeListView.SmallImageList = this.typeImageList;
            this.scopeListView.TabIndex = 2;
            this.scopeListView.UseCompatibleStateImageBehavior = false;
            this.scopeListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 140;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Type";
            this.columnHeader2.Width = 130;
            // 
            // typeImageList
            // 
            this.typeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("typeImageList.ImageStream")));
            this.typeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.typeImageList.Images.SetKeyName(0, "Assembly");
            this.typeImageList.Images.SetKeyName(1, "Class");
            this.typeImageList.Images.SetKeyName(2, "Error");
            this.typeImageList.Images.SetKeyName(3, "Field");
            this.typeImageList.Images.SetKeyName(4, "Information");
            this.typeImageList.Images.SetKeyName(5, "Macro");
            this.typeImageList.Images.SetKeyName(6, "Method");
            this.typeImageList.Images.SetKeyName(7, "Namespace");
            this.typeImageList.Images.SetKeyName(8, "Property");
            this.typeImageList.Images.SetKeyName(9, "Variable");
            this.typeImageList.Images.SetKeyName(10, "Warning");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Current Cursor Position:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Current AST Node:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Abstract Syntax Tree:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Current Scope:";
            // 
            // astTreeView
            // 
            this.astTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.astTreeView.Location = new System.Drawing.Point(6, 16);
            this.astTreeView.Name = "astTreeView";
            this.astTreeView.Size = new System.Drawing.Size(270, 571);
            this.astTreeView.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(15, 64);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.astTreeView);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Panel2.Controls.Add(this.scopeListView);
            this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer1.Size = new System.Drawing.Size(565, 590);
            this.splitContainer1.SplitterDistance = 279;
            this.splitContainer1.TabIndex = 13;
            // 
            // DebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 666);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.currentNodeLabel);
            this.Controls.Add(this.currentPositionLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Name = "DebugForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "CVSI Debug Window";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label currentNodeLabel;
        private System.Windows.Forms.Label currentPositionLabel;
        private System.Windows.Forms.ListView scopeListView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ImageList typeImageList;
        private System.Windows.Forms.TreeView astTreeView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}