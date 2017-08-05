namespace AndroidExtension
{
    partial class AndroidUI
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.getContactDetailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addContactToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button5 = new System.Windows.Forms.Button();
            this.listView2 = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.listView3 = new System.Windows.Forms.ListView();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addEventToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeEventToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteEventToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.listView4 = new System.Windows.Forms.ListView();
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.listFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enterDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.contextMenuStrip3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 65);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(131, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Test Connection";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select Target";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(22, 25);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(263, 21);
            this.comboBox1.TabIndex = 2;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(302, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Current Client Disconnected";
            this.label2.Visible = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 94);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(131, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Get Location";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(12, 123);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(131, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Get Battery Information";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(12, 152);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(131, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "Get Contacts";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.Location = new System.Drawing.Point(149, 65);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(356, 110);
            this.listView1.TabIndex = 7;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "ID";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 282;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.getContactDetailsToolStripMenuItem,
            this.addContactToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(176, 48);
            // 
            // getContactDetailsToolStripMenuItem
            // 
            this.getContactDetailsToolStripMenuItem.Name = "getContactDetailsToolStripMenuItem";
            this.getContactDetailsToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.getContactDetailsToolStripMenuItem.Text = "Get Contact Details";
            this.getContactDetailsToolStripMenuItem.Click += new System.EventHandler(this.getContactDetailsToolStripMenuItem_Click);
            // 
            // addContactToolStripMenuItem
            // 
            this.addContactToolStripMenuItem.Name = "addContactToolStripMenuItem";
            this.addContactToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.addContactToolStripMenuItem.Text = "Add Contact";
            this.addContactToolStripMenuItem.Click += new System.EventHandler(this.addContactToolStripMenuItem_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(12, 181);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(131, 23);
            this.button5.TabIndex = 8;
            this.button5.Text = "Read Call Log";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.listView2.Location = new System.Drawing.Point(149, 181);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(356, 110);
            this.listView2.TabIndex = 9;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Phone Number";
            this.columnHeader3.Width = 130;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Call Type";
            this.columnHeader4.Width = 75;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Call Duration (s)";
            this.columnHeader5.Width = 97;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Call Date";
            this.columnHeader6.Width = 74;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(12, 210);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(131, 23);
            this.button6.TabIndex = 10;
            this.button6.Text = "Get SMS";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(12, 239);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(131, 23);
            this.button7.TabIndex = 11;
            this.button7.Text = "Send SMS";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(12, 268);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(131, 23);
            this.button8.TabIndex = 12;
            this.button8.Text = "Hide Application";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(12, 297);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(131, 23);
            this.button9.TabIndex = 13;
            this.button9.Text = "Show Application";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(12, 326);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(131, 23);
            this.button10.TabIndex = 14;
            this.button10.Text = "Get Calendar Events";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // listView3
            // 
            this.listView3.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8});
            this.listView3.ContextMenuStrip = this.contextMenuStrip2;
            this.listView3.Location = new System.Drawing.Point(149, 297);
            this.listView3.MultiSelect = false;
            this.listView3.Name = "listView3";
            this.listView3.Size = new System.Drawing.Size(356, 110);
            this.listView3.TabIndex = 15;
            this.listView3.UseCompatibleStateImageBehavior = false;
            this.listView3.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "ID";
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Title";
            this.columnHeader8.Width = 265;
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.addEventToolStripMenuItem,
            this.changeEventToolStripMenuItem,
            this.deleteEventToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(160, 92);
            // 
            // addEventToolStripMenuItem
            // 
            this.addEventToolStripMenuItem.Name = "addEventToolStripMenuItem";
            this.addEventToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.addEventToolStripMenuItem.Text = "Add event";
            this.addEventToolStripMenuItem.Click += new System.EventHandler(this.addEventToolStripMenuItem_Click);
            // 
            // changeEventToolStripMenuItem
            // 
            this.changeEventToolStripMenuItem.Name = "changeEventToolStripMenuItem";
            this.changeEventToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.changeEventToolStripMenuItem.Text = "Change event";
            this.changeEventToolStripMenuItem.Click += new System.EventHandler(this.changeEventToolStripMenuItem_Click);
            // 
            // deleteEventToolStripMenuItem
            // 
            this.deleteEventToolStripMenuItem.Name = "deleteEventToolStripMenuItem";
            this.deleteEventToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.deleteEventToolStripMenuItem.Text = "Delete event";
            this.deleteEventToolStripMenuItem.Click += new System.EventHandler(this.deleteEventToolStripMenuItem_Click);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(12, 356);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(131, 23);
            this.button11.TabIndex = 17;
            this.button11.Text = "Start Recording Mic";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(12, 385);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(131, 23);
            this.button12.TabIndex = 18;
            this.button12.Text = "Start Tapping Mic";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // button13
            // 
            this.button13.Location = new System.Drawing.Point(12, 414);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(131, 23);
            this.button13.TabIndex = 19;
            this.button13.Text = "Take Photo";
            this.button13.UseVisualStyleBackColor = true;
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // button14
            // 
            this.button14.Location = new System.Drawing.Point(12, 443);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(131, 23);
            this.button14.TabIndex = 20;
            this.button14.Text = "Start Recording Cam";
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.button14_Click);
            // 
            // button15
            // 
            this.button15.Location = new System.Drawing.Point(12, 473);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(131, 23);
            this.button15.TabIndex = 21;
            this.button15.Text = "Start Tapping Cam";
            this.button15.UseVisualStyleBackColor = true;
            this.button15.Click += new System.EventHandler(this.button15_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(159, 22);
            this.toolStripMenuItem1.Text = "Read Event Data";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(542, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "File System Manager";
            // 
            // listView4
            // 
            this.listView4.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11});
            this.listView4.ContextMenuStrip = this.contextMenuStrip3;
            this.listView4.Location = new System.Drawing.Point(557, 65);
            this.listView4.MultiSelect = false;
            this.listView4.Name = "listView4";
            this.listView4.Size = new System.Drawing.Size(624, 342);
            this.listView4.TabIndex = 23;
            this.listView4.UseCompatibleStateImageBehavior = false;
            this.listView4.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Name";
            this.columnHeader9.Width = 145;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Size";
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Full Path";
            this.columnHeader11.Width = 402;
            // 
            // contextMenuStrip3
            // 
            this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.listFilesToolStripMenuItem,
            this.enterDirectoryToolStripMenuItem,
            this.toolStripMenuItem2,
            this.moveFileToolStripMenuItem,
            this.copyFileToolStripMenuItem,
            this.pasteFileToolStripMenuItem,
            this.deleteFileToolStripMenuItem,
            this.renameFileToolStripMenuItem,
            this.downloadFileToolStripMenuItem,
            this.uploadFileToolStripMenuItem});
            this.contextMenuStrip3.Name = "contextMenuStrip3";
            this.contextMenuStrip3.Size = new System.Drawing.Size(153, 246);
            // 
            // listFilesToolStripMenuItem
            // 
            this.listFilesToolStripMenuItem.Name = "listFilesToolStripMenuItem";
            this.listFilesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.listFilesToolStripMenuItem.Text = "List Files";
            this.listFilesToolStripMenuItem.Click += new System.EventHandler(this.listFilesToolStripMenuItem_Click);
            // 
            // enterDirectoryToolStripMenuItem
            // 
            this.enterDirectoryToolStripMenuItem.Name = "enterDirectoryToolStripMenuItem";
            this.enterDirectoryToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.enterDirectoryToolStripMenuItem.Text = "Enter Directory";
            this.enterDirectoryToolStripMenuItem.Click += new System.EventHandler(this.enterDirectoryToolStripMenuItem_Click);
            // 
            // moveFileToolStripMenuItem
            // 
            this.moveFileToolStripMenuItem.Name = "moveFileToolStripMenuItem";
            this.moveFileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.moveFileToolStripMenuItem.Text = "Move File";
            this.moveFileToolStripMenuItem.Click += new System.EventHandler(this.moveFileToolStripMenuItem_Click);
            // 
            // copyFileToolStripMenuItem
            // 
            this.copyFileToolStripMenuItem.Name = "copyFileToolStripMenuItem";
            this.copyFileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.copyFileToolStripMenuItem.Text = "Copy File";
            this.copyFileToolStripMenuItem.Click += new System.EventHandler(this.copyFileToolStripMenuItem_Click);
            // 
            // pasteFileToolStripMenuItem
            // 
            this.pasteFileToolStripMenuItem.Name = "pasteFileToolStripMenuItem";
            this.pasteFileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.pasteFileToolStripMenuItem.Text = "Paste File";
            this.pasteFileToolStripMenuItem.Click += new System.EventHandler(this.pasteFileToolStripMenuItem_Click);
            // 
            // deleteFileToolStripMenuItem
            // 
            this.deleteFileToolStripMenuItem.Name = "deleteFileToolStripMenuItem";
            this.deleteFileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteFileToolStripMenuItem.Text = "Delete File";
            this.deleteFileToolStripMenuItem.Click += new System.EventHandler(this.deleteFileToolStripMenuItem_Click);
            // 
            // renameFileToolStripMenuItem
            // 
            this.renameFileToolStripMenuItem.Name = "renameFileToolStripMenuItem";
            this.renameFileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.renameFileToolStripMenuItem.Text = "Rename File";
            this.renameFileToolStripMenuItem.Click += new System.EventHandler(this.renameFileToolStripMenuItem_Click);
            // 
            // downloadFileToolStripMenuItem
            // 
            this.downloadFileToolStripMenuItem.Name = "downloadFileToolStripMenuItem";
            this.downloadFileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.downloadFileToolStripMenuItem.Text = "Download File";
            this.downloadFileToolStripMenuItem.Click += new System.EventHandler(this.downloadFileToolStripMenuItem_Click);
            // 
            // uploadFileToolStripMenuItem
            // 
            this.uploadFileToolStripMenuItem.Name = "uploadFileToolStripMenuItem";
            this.uploadFileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.uploadFileToolStripMenuItem.Text = "Upload File";
            this.uploadFileToolStripMenuItem.Click += new System.EventHandler(this.uploadFileToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem2.Text = "Up 1";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // AndroidUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1193, 565);
            this.Controls.Add(this.listView4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button15);
            this.Controls.Add(this.button14);
            this.Controls.Add(this.button13);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.listView3);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AndroidUI";
            this.Text = "Android Control Panel";
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.contextMenuStrip3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem getContactDetailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addContactToolStripMenuItem;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.ListView listView3;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem addEventToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeEventToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteEventToolStripMenuItem;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView listView4;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
        private System.Windows.Forms.ToolStripMenuItem listFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enterDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uploadFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
    }
}