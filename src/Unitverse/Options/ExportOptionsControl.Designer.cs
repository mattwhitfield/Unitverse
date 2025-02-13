namespace Unitverse.Options
{
    partial class ExportOptionsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
            System.Windows.Forms.Button Export;
            System.Windows.Forms.Label label;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportOptionsControl));
            System.Windows.Forms.Button Import;
            tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            Export = new System.Windows.Forms.Button();
            label = new System.Windows.Forms.Label();
            Import = new System.Windows.Forms.Button();
            tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel.Controls.Add(Export, 0, 1);
            tableLayoutPanel.Controls.Add(label, 0, 0);
            tableLayoutPanel.Controls.Add(Import, 1, 1);
            tableLayoutPanel.Location = new System.Drawing.Point(6, 6);
            tableLayoutPanel.Margin = new System.Windows.Forms.Padding(6);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel.Size = new System.Drawing.Size(472, 302);
            tableLayoutPanel.TabIndex = 0;
            // 
            // Export
            // 
            Export.Anchor = System.Windows.Forms.AnchorStyles.None;
            Export.AutoSize = true;
            Export.Location = new System.Drawing.Point(67, 276);
            Export.Name = "Export";
            Export.Size = new System.Drawing.Size(102, 23);
            Export.TabIndex = 0;
            Export.Text = "Export options file";
            Export.UseVisualStyleBackColor = true;
            Export.Click += new System.EventHandler(this.Export_Click);
            // 
            // label
            // 
            label.AutoSize = true;
            tableLayoutPanel.SetColumnSpan(label, 2);
            label.Location = new System.Drawing.Point(3, 0);
            label.Name = "label";
            label.Size = new System.Drawing.Size(456, 182);
            label.TabIndex = 1;
            label.Text = resources.GetString("label.Text");
            // 
            // Import
            // 
            Import.Anchor = System.Windows.Forms.AnchorStyles.None;
            Import.AutoSize = true;
            Import.Location = new System.Drawing.Point(304, 276);
            Import.Name = "Import";
            Import.Size = new System.Drawing.Size(100, 23);
            Import.TabIndex = 0;
            Import.Text = "Import options file";
            Import.UseVisualStyleBackColor = true;
            Import.Click += new System.EventHandler(this.Import_Click);
            // 
            // ExportOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(tableLayoutPanel);
            this.Name = "ExportOptionsControl";
            this.Size = new System.Drawing.Size(484, 314);
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
