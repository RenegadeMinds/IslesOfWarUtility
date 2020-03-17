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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnGetGameState = new System.Windows.Forms.Button();
            this.srtbResults = new SearchableControls.SearchableRichTextBox();
            this.wbBrowser = new System.Windows.Forms.WebBrowser();
            this.cbxPlayers = new System.Windows.Forms.ComboBox();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.scBrowsers = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.scBrowsers)).BeginInit();
            this.scBrowsers.Panel1.SuspendLayout();
            this.scBrowsers.Panel2.SuspendLayout();
            this.scBrowsers.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGetGameState
            // 
            this.btnGetGameState.Location = new System.Drawing.Point(12, 12);
            this.btnGetGameState.Name = "btnGetGameState";
            this.btnGetGameState.Size = new System.Drawing.Size(200, 23);
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
            this.srtbResults.Size = new System.Drawing.Size(336, 109);
            this.srtbResults.TabIndex = 2;
            this.srtbResults.Text = "";
            this.srtbResults.Visible = false;
            // 
            // wbBrowser
            // 
            this.wbBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbBrowser.Location = new System.Drawing.Point(0, 0);
            this.wbBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbBrowser.Name = "wbBrowser";
            this.wbBrowser.ScriptErrorsSuppressed = true;
            this.wbBrowser.Size = new System.Drawing.Size(514, 397);
            this.wbBrowser.TabIndex = 3;
            // 
            // cbxPlayers
            // 
            this.cbxPlayers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxPlayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPlayers.FormattingEnabled = true;
            this.cbxPlayers.Location = new System.Drawing.Point(218, 12);
            this.cbxPlayers.Name = "cbxPlayers";
            this.cbxPlayers.Size = new System.Drawing.Size(806, 21);
            this.cbxPlayers.TabIndex = 5;
            this.cbxPlayers.SelectedIndexChanged += new System.EventHandler(this.cbxPlayers_SelectedIndexChanged);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(494, 397);
            this.webBrowser1.TabIndex = 6;
            // 
            // scBrowsers
            // 
            this.scBrowsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scBrowsers.Location = new System.Drawing.Point(12, 41);
            this.scBrowsers.Name = "scBrowsers";
            // 
            // scBrowsers.Panel1
            // 
            this.scBrowsers.Panel1.Controls.Add(this.wbBrowser);
            // 
            // scBrowsers.Panel2
            // 
            this.scBrowsers.Panel2.Controls.Add(this.webBrowser1);
            this.scBrowsers.Size = new System.Drawing.Size(1012, 397);
            this.scBrowsers.SplitterDistance = 514;
            this.scBrowsers.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1036, 450);
            this.Controls.Add(this.scBrowsers);
            this.Controls.Add(this.cbxPlayers);
            this.Controls.Add(this.srtbResults);
            this.Controls.Add(this.btnGetGameState);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Isles of War utility";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.scBrowsers.Panel1.ResumeLayout(false);
            this.scBrowsers.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scBrowsers)).EndInit();
            this.scBrowsers.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGetGameState;
        private SearchableControls.SearchableRichTextBox srtbResults;
        private System.Windows.Forms.WebBrowser wbBrowser;
        private System.Windows.Forms.ComboBox cbxPlayers;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.SplitContainer scBrowsers;
    }
}

