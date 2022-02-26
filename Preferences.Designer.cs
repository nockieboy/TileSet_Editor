namespace Tile_Set_Editor
{
    partial class Preferences
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
            this.button1 = new System.Windows.Forms.Button();
            this.txtPalAddr = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtEntrySize = new System.Windows.Forms.TextBox();
            this.txtPaletteSize = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(131, 180);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtPalAddr
            // 
            this.txtPalAddr.Location = new System.Drawing.Point(133, 50);
            this.txtPalAddr.MaxLength = 8;
            this.txtPalAddr.Name = "txtPalAddr";
            this.txtPalAddr.Size = new System.Drawing.Size(73, 23);
            this.txtPalAddr.TabIndex = 1;
            this.txtPalAddr.TextChanged += new System.EventHandler(this.txtPalAddr_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Palette Base Address";
            // 
            // txtEntrySize
            // 
            this.txtEntrySize.Location = new System.Drawing.Point(133, 91);
            this.txtEntrySize.MaxLength = 8;
            this.txtEntrySize.Name = "txtEntrySize";
            this.txtEntrySize.Size = new System.Drawing.Size(73, 23);
            this.txtEntrySize.TabIndex = 3;
            this.txtEntrySize.TextChanged += new System.EventHandler(this.txtEntrySize_TextChanged);
            // 
            // txtPaletteSize
            // 
            this.txtPaletteSize.Location = new System.Drawing.Point(133, 136);
            this.txtPaletteSize.MaxLength = 8;
            this.txtPaletteSize.Name = "txtPaletteSize";
            this.txtPaletteSize.Size = new System.Drawing.Size(73, 23);
            this.txtPaletteSize.TabIndex = 4;
            this.txtPaletteSize.TextChanged += new System.EventHandler(this.txtPaletteSize_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Palette Entry Size";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Palette Size";
            // 
            // Preferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(233, 219);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPaletteSize);
            this.Controls.Add(this.txtEntrySize);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPalAddr);
            this.Controls.Add(this.button1);
            this.Name = "Preferences";
            this.Text = "Preferences";
            this.Load += new System.EventHandler(this.Preferences_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button button1;
        private TextBox txtPalAddr;
        private Label label1;
        private TextBox txtEntrySize;
        private TextBox txtPaletteSize;
        private Label label2;
        private Label label3;
    }
}