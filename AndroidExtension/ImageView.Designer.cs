namespace AndroidExtension
{
    partial class ImageView
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rotateLeft90ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotateRight90ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(551, 328);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rotateLeft90ToolStripMenuItem,
            this.rotateRight90ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(155, 70);
            // 
            // rotateLeft90ToolStripMenuItem
            // 
            this.rotateLeft90ToolStripMenuItem.Name = "rotateLeft90ToolStripMenuItem";
            this.rotateLeft90ToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.rotateLeft90ToolStripMenuItem.Text = "Rotate Left 90";
            this.rotateLeft90ToolStripMenuItem.Click += new System.EventHandler(this.rotateLeft90ToolStripMenuItem_Click);
            // 
            // rotateRight90ToolStripMenuItem
            // 
            this.rotateRight90ToolStripMenuItem.Name = "rotateRight90ToolStripMenuItem";
            this.rotateRight90ToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.rotateRight90ToolStripMenuItem.Text = "Rotate Right 90";
            this.rotateRight90ToolStripMenuItem.Click += new System.EventHandler(this.rotateRight90ToolStripMenuItem_Click);
            // 
            // ImageView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 328);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ImageView";
            this.Text = "Image View";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem rotateLeft90ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotateRight90ToolStripMenuItem;
    }
}