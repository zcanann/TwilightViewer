namespace WWActorEdit.Kazari.DZx
{
    partial class RPATControl
    {
        /// <summary> 
        /// Erforderliche Designervariable. bleh
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_numPoints = new System.Windows.Forms.TextBox();
            this.textBox_u1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_u2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_u3 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_offset = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Num Points:";
            // 
            // textBox_numPoints
            // 
            this.textBox_numPoints.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_numPoints.Location = new System.Drawing.Point(79, 29);
            this.textBox_numPoints.MaxLength = 4;
            this.textBox_numPoints.Name = "textBox_numPoints";
            this.textBox_numPoints.Size = new System.Drawing.Size(105, 20);
            this.textBox_numPoints.TabIndex = 3;
            // 
            // textBox_u1
            // 
            this.textBox_u1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_u1.Location = new System.Drawing.Point(79, 64);
            this.textBox_u1.MaxLength = 4;
            this.textBox_u1.Name = "textBox_u1";
            this.textBox_u1.Size = new System.Drawing.Size(105, 20);
            this.textBox_u1.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Index to Next:";
            // 
            // textBox_u2
            // 
            this.textBox_u2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_u2.Location = new System.Drawing.Point(79, 103);
            this.textBox_u2.MaxLength = 2;
            this.textBox_u2.Name = "textBox_u2";
            this.textBox_u2.Size = new System.Drawing.Size(105, 20);
            this.textBox_u2.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Unk1:";
            // 
            // textBox_u3
            // 
            this.textBox_u3.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_u3.Location = new System.Drawing.Point(79, 144);
            this.textBox_u3.MaxLength = 2;
            this.textBox_u3.Name = "textBox_u3";
            this.textBox_u3.Size = new System.Drawing.Size(105, 20);
            this.textBox_u3.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 147);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Is Loop:";
            // 
            // textBox_offset
            // 
            this.textBox_offset.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBox_offset.Location = new System.Drawing.Point(79, 184);
            this.textBox_offset.MaxLength = 8;
            this.textBox_offset.Name = "textBox_offset";
            this.textBox_offset.Size = new System.Drawing.Size(105, 20);
            this.textBox_offset.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 187);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "First Entry Off:";
            // 
            // RPATControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBox_offset);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox_u3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_u2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_u1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_numPoints);
            this.Controls.Add(this.label2);
            this.DoubleBuffered = true;
            this.Name = "RPATControl";
            this.Size = new System.Drawing.Size(188, 234);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_numPoints;
        private System.Windows.Forms.TextBox textBox_u1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_u2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_u3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_offset;
        private System.Windows.Forms.Label label5;
    }
}
