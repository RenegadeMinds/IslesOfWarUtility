namespace IslesOfWarUtility
{
    partial class Form1
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
            this.btnGetGameState = new System.Windows.Forms.Button();
            this.srtbResults = new SearchableControls.SearchableRichTextBox();
            this.wbBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // btnGetGameState
            // 
            this.btnGetGameState.Location = new System.Drawing.Point(12, 12);
            this.btnGetGameState.Name = "btnGetGameState";
            this.btnGetGameState.Size = new System.Drawing.Size(156, 23);
            this.btnGetGameState.TabIndex = 0;
            this.btnGetGameState.Text = "Get game state";
            this.btnGetGameState.UseVisualStyleBackColor = true;
            this.btnGetGameState.Click += new System.EventHandler(this.btnGetGameState_Click);
            // 
            // srtbResults
            // 
            this.srtbResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.srtbResults.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.srtbResults.Location = new System.Drawing.Point(12, 41);
            this.srtbResults.Name = "srtbResults";
            this.srtbResults.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.srtbResults.Size = new System.Drawing.Size(156, 109);
            this.srtbResults.TabIndex = 2;
            this.srtbResults.Text = "";
            this.srtbResults.Visible = false;
            // 
            // wbBrowser
            // 
            this.wbBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wbBrowser.Location = new System.Drawing.Point(12, 41);
            this.wbBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbBrowser.Name = "wbBrowser";
            this.wbBrowser.Size = new System.Drawing.Size(832, 397);
            this.wbBrowser.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(856, 450);
            this.Controls.Add(this.wbBrowser);
            this.Controls.Add(this.srtbResults);
            this.Controls.Add(this.btnGetGameState);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Isles of War utility";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGetGameState;
        private SearchableControls.SearchableRichTextBox srtbResults;
        private System.Windows.Forms.WebBrowser wbBrowser;
    }
}

