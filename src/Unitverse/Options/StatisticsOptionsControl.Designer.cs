namespace Unitverse.Options
{
    partial class StatisticsOptionsControl
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
            System.Windows.Forms.Button resetStatsButton;
            this.EnableStatisticsCheckBox = new System.Windows.Forms.CheckBox();
            this.captionLabel1 = new System.Windows.Forms.Label();
            this.captionLabel2 = new System.Windows.Forms.Label();
            this.captionLabel3 = new System.Windows.Forms.Label();
            this.captionLabel4 = new System.Windows.Forms.Label();
            this.captionLabel5 = new System.Windows.Forms.Label();
            this.captionLabel6 = new System.Windows.Forms.Label();
            this.testClassesGeneratedLabel = new System.Windows.Forms.Label();
            this.testMethodsGeneratedLabel = new System.Windows.Forms.Label();
            this.testMethodsRegeneratedLabel = new System.Windows.Forms.Label();
            this.typesConstructedLabel = new System.Windows.Forms.Label();
            this.interfacesMockedLabel = new System.Windows.Forms.Label();
            this.valuesGeneratedLabel = new System.Windows.Forms.Label();
            this.infoLabel = new System.Windows.Forms.Label();
            tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            resetStatsButton = new System.Windows.Forms.Button();
            tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            tableLayoutPanel.ColumnCount = 3;
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(this.EnableStatisticsCheckBox, 0, 0);
            tableLayoutPanel.Controls.Add(this.captionLabel1, 1, 1);
            tableLayoutPanel.Controls.Add(this.captionLabel2, 1, 2);
            tableLayoutPanel.Controls.Add(this.captionLabel3, 1, 3);
            tableLayoutPanel.Controls.Add(this.captionLabel4, 1, 4);
            tableLayoutPanel.Controls.Add(this.captionLabel5, 1, 5);
            tableLayoutPanel.Controls.Add(this.captionLabel6, 1, 6);
            tableLayoutPanel.Controls.Add(this.testClassesGeneratedLabel, 2, 1);
            tableLayoutPanel.Controls.Add(this.testMethodsGeneratedLabel, 2, 2);
            tableLayoutPanel.Controls.Add(this.testMethodsRegeneratedLabel, 2, 3);
            tableLayoutPanel.Controls.Add(this.typesConstructedLabel, 2, 4);
            tableLayoutPanel.Controls.Add(this.interfacesMockedLabel, 2, 5);
            tableLayoutPanel.Controls.Add(this.valuesGeneratedLabel, 2, 6);
            tableLayoutPanel.Controls.Add(this.infoLabel, 0, 8);
            tableLayoutPanel.Controls.Add(resetStatsButton, 0, 7);
            tableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 9;
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel.Size = new System.Drawing.Size(281, 257);
            tableLayoutPanel.TabIndex = 0;
            // 
            // EnableStatisticsCheckBox
            // 
            this.EnableStatisticsCheckBox.AutoSize = true;
            tableLayoutPanel.SetColumnSpan(this.EnableStatisticsCheckBox, 3);
            this.EnableStatisticsCheckBox.Location = new System.Drawing.Point(3, 3);
            this.EnableStatisticsCheckBox.Margin = new System.Windows.Forms.Padding(3, 3, 3, 12);
            this.EnableStatisticsCheckBox.Name = "EnableStatisticsCheckBox";
            this.EnableStatisticsCheckBox.Size = new System.Drawing.Size(154, 17);
            this.EnableStatisticsCheckBox.TabIndex = 0;
            this.EnableStatisticsCheckBox.Text = "Collect generation statistics";
            this.EnableStatisticsCheckBox.UseVisualStyleBackColor = true;
            this.EnableStatisticsCheckBox.CheckedChanged += new System.EventHandler(this.EnableStatisticsCheckBox_CheckedChanged);
            // 
            // captionLabel1
            // 
            this.captionLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.captionLabel1.AutoSize = true;
            this.captionLabel1.Location = new System.Drawing.Point(37, 35);
            this.captionLabel1.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.captionLabel1.Name = "captionLabel1";
            this.captionLabel1.Size = new System.Drawing.Size(120, 13);
            this.captionLabel1.TabIndex = 1;
            this.captionLabel1.Text = "Test classes generated:";
            // 
            // captionLabel2
            // 
            this.captionLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.captionLabel2.AutoSize = true;
            this.captionLabel2.Location = new System.Drawing.Point(32, 54);
            this.captionLabel2.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.captionLabel2.Name = "captionLabel2";
            this.captionLabel2.Size = new System.Drawing.Size(125, 13);
            this.captionLabel2.TabIndex = 2;
            this.captionLabel2.Text = "Test methods generated:";
            // 
            // captionLabel3
            // 
            this.captionLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.captionLabel3.AutoSize = true;
            this.captionLabel3.Location = new System.Drawing.Point(23, 73);
            this.captionLabel3.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.captionLabel3.Name = "captionLabel3";
            this.captionLabel3.Size = new System.Drawing.Size(134, 13);
            this.captionLabel3.TabIndex = 3;
            this.captionLabel3.Text = "Test methods regenerated:";
            // 
            // captionLabel4
            // 
            this.captionLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.captionLabel4.AutoSize = true;
            this.captionLabel4.Location = new System.Drawing.Point(59, 92);
            this.captionLabel4.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.captionLabel4.Name = "captionLabel4";
            this.captionLabel4.Size = new System.Drawing.Size(98, 13);
            this.captionLabel4.TabIndex = 4;
            this.captionLabel4.Text = "Types constructed:";
            // 
            // captionLabel5
            // 
            this.captionLabel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.captionLabel5.AutoSize = true;
            this.captionLabel5.Location = new System.Drawing.Point(59, 111);
            this.captionLabel5.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.captionLabel5.Name = "captionLabel5";
            this.captionLabel5.Size = new System.Drawing.Size(98, 13);
            this.captionLabel5.TabIndex = 5;
            this.captionLabel5.Text = "Interfaces mocked:";
            // 
            // captionLabel6
            // 
            this.captionLabel6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.captionLabel6.AutoSize = true;
            this.captionLabel6.Location = new System.Drawing.Point(64, 130);
            this.captionLabel6.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.captionLabel6.Name = "captionLabel6";
            this.captionLabel6.Size = new System.Drawing.Size(93, 13);
            this.captionLabel6.TabIndex = 6;
            this.captionLabel6.Text = "Values generated:";
            // 
            // testClassesGeneratedLabel
            // 
            this.testClassesGeneratedLabel.AutoSize = true;
            this.testClassesGeneratedLabel.Location = new System.Drawing.Point(157, 35);
            this.testClassesGeneratedLabel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.testClassesGeneratedLabel.Name = "testClassesGeneratedLabel";
            this.testClassesGeneratedLabel.Size = new System.Drawing.Size(13, 13);
            this.testClassesGeneratedLabel.TabIndex = 7;
            this.testClassesGeneratedLabel.Text = "0";
            // 
            // testMethodsGeneratedLabel
            // 
            this.testMethodsGeneratedLabel.AutoSize = true;
            this.testMethodsGeneratedLabel.Location = new System.Drawing.Point(157, 54);
            this.testMethodsGeneratedLabel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.testMethodsGeneratedLabel.Name = "testMethodsGeneratedLabel";
            this.testMethodsGeneratedLabel.Size = new System.Drawing.Size(13, 13);
            this.testMethodsGeneratedLabel.TabIndex = 8;
            this.testMethodsGeneratedLabel.Text = "0";
            // 
            // testMethodsRegeneratedLabel
            // 
            this.testMethodsRegeneratedLabel.AutoSize = true;
            this.testMethodsRegeneratedLabel.Location = new System.Drawing.Point(157, 73);
            this.testMethodsRegeneratedLabel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.testMethodsRegeneratedLabel.Name = "testMethodsRegeneratedLabel";
            this.testMethodsRegeneratedLabel.Size = new System.Drawing.Size(13, 13);
            this.testMethodsRegeneratedLabel.TabIndex = 9;
            this.testMethodsRegeneratedLabel.Text = "0";
            // 
            // typesConstructedLabel
            // 
            this.typesConstructedLabel.AutoSize = true;
            this.typesConstructedLabel.Location = new System.Drawing.Point(157, 92);
            this.typesConstructedLabel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.typesConstructedLabel.Name = "typesConstructedLabel";
            this.typesConstructedLabel.Size = new System.Drawing.Size(13, 13);
            this.typesConstructedLabel.TabIndex = 10;
            this.typesConstructedLabel.Text = "0";
            // 
            // interfacesMockedLabel
            // 
            this.interfacesMockedLabel.AutoSize = true;
            this.interfacesMockedLabel.Location = new System.Drawing.Point(157, 111);
            this.interfacesMockedLabel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.interfacesMockedLabel.Name = "interfacesMockedLabel";
            this.interfacesMockedLabel.Size = new System.Drawing.Size(13, 13);
            this.interfacesMockedLabel.TabIndex = 11;
            this.interfacesMockedLabel.Text = "0";
            // 
            // valuesGeneratedLabel
            // 
            this.valuesGeneratedLabel.AutoSize = true;
            this.valuesGeneratedLabel.Location = new System.Drawing.Point(157, 130);
            this.valuesGeneratedLabel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.valuesGeneratedLabel.Name = "valuesGeneratedLabel";
            this.valuesGeneratedLabel.Size = new System.Drawing.Size(13, 13);
            this.valuesGeneratedLabel.TabIndex = 12;
            this.valuesGeneratedLabel.Text = "0";
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            tableLayoutPanel.SetColumnSpan(this.infoLabel, 3);
            this.infoLabel.Location = new System.Drawing.Point(3, 218);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(242, 39);
            this.infoLabel.TabIndex = 13;
            this.infoLabel.Text = "Note that statistics are stored in the current user\'s registry, and are not trans" +
    "mitted anywhere for any reason.";
            // 
            // resetStatsButton
            // 
            resetStatsButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            resetStatsButton.AutoSize = true;
            tableLayoutPanel.SetColumnSpan(resetStatsButton, 3);
            resetStatsButton.Location = new System.Drawing.Point(96, 158);
            resetStatsButton.Margin = new System.Windows.Forms.Padding(3, 12, 3, 3);
            resetStatsButton.Name = "resetStatsButton";
            resetStatsButton.Size = new System.Drawing.Size(88, 23);
            resetStatsButton.TabIndex = 14;
            resetStatsButton.Text = "Reset statistics";
            resetStatsButton.UseVisualStyleBackColor = true;
            resetStatsButton.Click += new System.EventHandler(this.resetStatsButton_Click);
            // 
            // StatisticsOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(tableLayoutPanel);
            this.Name = "StatisticsOptionsControl";
            this.Size = new System.Drawing.Size(287, 263);
            this.VisibleChanged += new System.EventHandler(this.StatisticsOptionsControl_VisibleChanged);
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox EnableStatisticsCheckBox;
        private System.Windows.Forms.Label testClassesGeneratedLabel;
        private System.Windows.Forms.Label testMethodsGeneratedLabel;
        private System.Windows.Forms.Label testMethodsRegeneratedLabel;
        private System.Windows.Forms.Label typesConstructedLabel;
        private System.Windows.Forms.Label interfacesMockedLabel;
        private System.Windows.Forms.Label valuesGeneratedLabel;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Label captionLabel1;
        private System.Windows.Forms.Label captionLabel2;
        private System.Windows.Forms.Label captionLabel3;
        private System.Windows.Forms.Label captionLabel4;
        private System.Windows.Forms.Label captionLabel5;
        private System.Windows.Forms.Label captionLabel6;
    }
}
