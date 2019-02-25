namespace SOS
{
    partial class BrowserTabUserControl
    {
        /// <summary> 
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        #region Código gerado pelo Designer de Componentes

        /// <summary> 
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowserTabUserControl));
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.findTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.findTextLabel = new System.Windows.Forms.ToolStripLabel();
            this.findPreviousButton = new System.Windows.Forms.ToolStripButton();
            this.findNextButton = new System.Windows.Forms.ToolStripButton();
            this.findCloseButton = new System.Windows.Forms.ToolStripButton();
            this.statusLabel = new System.Windows.Forms.Label();
            this.outputLabel = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.backButton = new System.Windows.Forms.ToolStripButton();
            this.forwardButton = new System.Windows.Forms.ToolStripButton();
            this.goButton = new System.Windows.Forms.ToolStripButton();
            this.urlTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.browserPanel = new System.Windows.Forms.Panel();
            this.downloadOutputLabel = new System.Windows.Forms.LinkLabel();
            this.toolStrip2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip2
            // 
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findTextBox,
            this.findTextLabel,
            this.findPreviousButton,
            this.findNextButton,
            this.findCloseButton});
            this.toolStrip2.Location = new System.Drawing.Point(0, 465);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(730, 25);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Visible = false;
            // 
            // findTextBox
            // 
            this.findTextBox.Name = "findTextBox";
            this.findTextBox.Size = new System.Drawing.Size(400, 25);
            this.findTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FindTextBoxKeyDown);
            // 
            // findTextLabel
            // 
            this.findTextLabel.Name = "findTextLabel";
            this.findTextLabel.Size = new System.Drawing.Size(0, 22);
            // 
            // findPreviousButton
            // 
            this.findPreviousButton.AutoToolTip = false;
            this.findPreviousButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.findPreviousButton.Image = ((System.Drawing.Image)(resources.GetObject("findPreviousButton.Image")));
            this.findPreviousButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.findPreviousButton.Name = "findPreviousButton";
            this.findPreviousButton.Size = new System.Drawing.Size(23, 22);
            this.findPreviousButton.Text = "Find Previous";
            this.findPreviousButton.Click += new System.EventHandler(this.FindPreviousButtonClick);
            // 
            // findNextButton
            // 
            this.findNextButton.AutoToolTip = false;
            this.findNextButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.findNextButton.Image = ((System.Drawing.Image)(resources.GetObject("findNextButton.Image")));
            this.findNextButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.findNextButton.Name = "findNextButton";
            this.findNextButton.Size = new System.Drawing.Size(23, 22);
            this.findNextButton.Text = "Find Next";
            this.findNextButton.Click += new System.EventHandler(this.FindNextButtonClick);
            // 
            // findCloseButton
            // 
            this.findCloseButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.findCloseButton.AutoToolTip = false;
            this.findCloseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.findCloseButton.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.findCloseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.findCloseButton.Name = "findCloseButton";
            this.findCloseButton.Size = new System.Drawing.Size(23, 22);
            this.findCloseButton.Text = "X";
            this.findCloseButton.Click += new System.EventHandler(this.FindCloseButtonClick);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusLabel.Location = new System.Drawing.Point(0, 477);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 13);
            this.statusLabel.TabIndex = 1;
            // 
            // outputLabel
            // 
            this.outputLabel.AutoSize = true;
            this.outputLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.outputLabel.Location = new System.Drawing.Point(0, 451);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(0, 13);
            this.outputLabel.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.backButton,
            this.forwardButton,
            this.goButton,
            this.urlTextBox});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip1.Size = new System.Drawing.Size(730, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Layout += new System.Windows.Forms.LayoutEventHandler(this.HandleToolStripLayout);
            // 
            // backButton
            // 
            this.backButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.backButton.Enabled = false;
            this.backButton.Image = global::SOS.Properties.Resources.arrow_left_solid;
            this.backButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(23, 22);
            this.backButton.Text = "Back";
            this.backButton.Click += new System.EventHandler(this.BackButtonClick);
            // 
            // forwardButton
            // 
            this.forwardButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.forwardButton.Enabled = false;
            this.forwardButton.Image = global::SOS.Properties.Resources.arrow_right_solid;
            this.forwardButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.forwardButton.Name = "forwardButton";
            this.forwardButton.Size = new System.Drawing.Size(23, 22);
            this.forwardButton.Text = "Forward";
            this.forwardButton.Click += new System.EventHandler(this.ForwardButtonClick);
            // 
            // goButton
            // 
            this.goButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.goButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(23, 22);
            this.goButton.Text = "Go";
            this.goButton.Click += new System.EventHandler(this.GoButtonClick);
            // 
            // urlTextBox
            // 
            this.urlTextBox.AutoSize = false;
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(500, 25);
            this.urlTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UrlTextBoxKeyUp);
            // 
            // browserPanel
            // 
            this.browserPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browserPanel.Location = new System.Drawing.Point(0, 25);
            this.browserPanel.Name = "browserPanel";
            this.browserPanel.Size = new System.Drawing.Size(730, 426);
            this.browserPanel.TabIndex = 2;
            // 
            // downloadOutputLabel
            // 
            this.downloadOutputLabel.AutoSize = true;
            this.downloadOutputLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.downloadOutputLabel.Location = new System.Drawing.Point(0, 464);
            this.downloadOutputLabel.Name = "downloadOutputLabel";
            this.downloadOutputLabel.Size = new System.Drawing.Size(0, 13);
            this.downloadOutputLabel.TabIndex = 0;
            this.downloadOutputLabel.Visible = false;
            this.downloadOutputLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DownloadLinkLabelClick);
            // 
            // BrowserTabUserControl
            // 
            this.Controls.Add(this.browserPanel);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.outputLabel);
            this.Controls.Add(this.downloadOutputLabel);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.toolStrip2);
            this.Name = "BrowserTabUserControl";
            this.Size = new System.Drawing.Size(730, 490);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton backButton;
        private System.Windows.Forms.ToolStripButton forwardButton;
        private System.Windows.Forms.ToolStripTextBox urlTextBox;
        private System.Windows.Forms.ToolStripButton goButton;

        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton findPreviousButton;
        private System.Windows.Forms.ToolStripButton findNextButton;
        private System.Windows.Forms.ToolStripButton findCloseButton;
        private System.Windows.Forms.Panel browserPanel;
        public System.Windows.Forms.Label statusLabel;
        public System.Windows.Forms.ToolStripLabel findTextLabel;
        public System.Windows.Forms.ToolStripTextBox findTextBox;
        public System.Windows.Forms.Label outputLabel;
        public System.Windows.Forms.LinkLabel downloadOutputLabel;
    }
}
