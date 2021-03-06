﻿namespace SOS
{
    partial class BrowserInterface
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowserInterface));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printToPdfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.undoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.historicoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.atualizaçãoPorMáquinaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.acessoPorUsuárioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configuraçõesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exibirMsgConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exibirMensagensDeDownloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exibirFerramentasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.updateStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visualizarHistóricoDeAtualizaçãoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.forcarAtualizacaoCompletaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.browserTabControl = new System.Windows.Forms.TabControl();
            this.btnNewTab = new System.Windows.Forms.TabPage();
            this.statusOutputLinkLabel = new System.Windows.Forms.LinkLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeViewSearch = new System.Windows.Forms.TreeView();
            this.treeviewStatusLabel = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtBookmarkSearch = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.browserTabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.historicoToolStripMenuItem,
            this.configuraçõesToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(730, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newTabToolStripMenuItem,
            this.closeTabToolStripMenuItem,
            this.printToolStripMenuItem,
            this.printToPdfToolStripMenuItem,
            this.toolStripMenuItem3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.fileToolStripMenuItem.Text = "&Arquivos";
            // 
            // newTabToolStripMenuItem
            // 
            this.newTabToolStripMenuItem.Name = "newTabToolStripMenuItem";
            this.newTabToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.newTabToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.newTabToolStripMenuItem.Text = "&Nova guia";
            this.newTabToolStripMenuItem.Click += new System.EventHandler(this.NewTabToolStripMenuItemClick);
            // 
            // closeTabToolStripMenuItem
            // 
            this.closeTabToolStripMenuItem.Name = "closeTabToolStripMenuItem";
            this.closeTabToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.closeTabToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.closeTabToolStripMenuItem.Text = "&Fechar guia";
            this.closeTabToolStripMenuItem.Click += new System.EventHandler(this.CloseTabToolStripMenuItemClick);
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.printToolStripMenuItem.Text = "&Imprimir";
            this.printToolStripMenuItem.Click += new System.EventHandler(this.PrintToolStripMenuItemClick);
            // 
            // printToPdfToolStripMenuItem
            // 
            this.printToPdfToolStripMenuItem.Name = "printToPdfToolStripMenuItem";
            this.printToPdfToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.printToPdfToolStripMenuItem.Text = "Salvar como PDF";
            this.printToPdfToolStripMenuItem.Click += new System.EventHandler(this.PrintToPdfToolStripMenuItemClick);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(177, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "&Sair";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitMenuItemClick);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findMenuItem,
            this.toolStripSeparator2,
            this.zoomToolStripMenuItem,
            this.zoomToolStripMenuItem1,
            this.toolStripSeparator3,
            this.undoMenuItem,
            this.redoMenuItem,
            this.toolStripMenuItem2,
            this.selectAllMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.editToolStripMenuItem.Text = "Editar";
            // 
            // findMenuItem
            // 
            this.findMenuItem.Name = "findMenuItem";
            this.findMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findMenuItem.Size = new System.Drawing.Size(159, 22);
            this.findMenuItem.Text = "Procurar";
            this.findMenuItem.Click += new System.EventHandler(this.FindMenuItemClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(156, 6);
            // 
            // zoomToolStripMenuItem
            // 
            this.zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
            this.zoomToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Z)));
            this.zoomToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.zoomToolStripMenuItem.Text = "Zoom +";
            this.zoomToolStripMenuItem.Click += new System.EventHandler(this.ZoomInToolStripMenuItemClick);
            // 
            // zoomToolStripMenuItem1
            // 
            this.zoomToolStripMenuItem1.Name = "zoomToolStripMenuItem1";
            this.zoomToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.X)));
            this.zoomToolStripMenuItem1.Size = new System.Drawing.Size(159, 22);
            this.zoomToolStripMenuItem1.Text = "Zoom -";
            this.zoomToolStripMenuItem1.Click += new System.EventHandler(this.ZoomOutToolStripMenuItemClick);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(156, 6);
            // 
            // undoMenuItem
            // 
            this.undoMenuItem.Name = "undoMenuItem";
            this.undoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoMenuItem.Size = new System.Drawing.Size(159, 22);
            this.undoMenuItem.Text = "Desfazer";
            this.undoMenuItem.Click += new System.EventHandler(this.UndoMenuItemClick);
            // 
            // redoMenuItem
            // 
            this.redoMenuItem.Name = "redoMenuItem";
            this.redoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoMenuItem.Size = new System.Drawing.Size(159, 22);
            this.redoMenuItem.Text = "Refazer";
            this.redoMenuItem.Click += new System.EventHandler(this.RedoMenuItemClick);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(156, 6);
            // 
            // selectAllMenuItem
            // 
            this.selectAllMenuItem.Name = "selectAllMenuItem";
            this.selectAllMenuItem.Size = new System.Drawing.Size(159, 22);
            this.selectAllMenuItem.Text = "Selecionar tudo";
            this.selectAllMenuItem.Click += new System.EventHandler(this.SelectAllMenuItemClick);
            // 
            // historicoToolStripMenuItem
            // 
            this.historicoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.atualizaçãoPorMáquinaToolStripMenuItem,
            this.acessoPorUsuárioToolStripMenuItem});
            this.historicoToolStripMenuItem.Name = "historicoToolStripMenuItem";
            this.historicoToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.historicoToolStripMenuItem.Text = "Histórico";
            // 
            // atualizaçãoPorMáquinaToolStripMenuItem
            // 
            this.atualizaçãoPorMáquinaToolStripMenuItem.Name = "atualizaçãoPorMáquinaToolStripMenuItem";
            this.atualizaçãoPorMáquinaToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.atualizaçãoPorMáquinaToolStripMenuItem.Text = "Atualização por máquina";
            this.atualizaçãoPorMáquinaToolStripMenuItem.Click += new System.EventHandler(this.HistoricoAtualizacaoMaquinaItemClick);
            // 
            // acessoPorUsuárioToolStripMenuItem
            // 
            this.acessoPorUsuárioToolStripMenuItem.Name = "acessoPorUsuárioToolStripMenuItem";
            this.acessoPorUsuárioToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.acessoPorUsuárioToolStripMenuItem.Text = "Acesso por usuário";
            this.acessoPorUsuárioToolStripMenuItem.Click += new System.EventHandler(this.HistoricoAcessoItemClick);
            // 
            // configuraçõesToolStripMenuItem
            // 
            this.configuraçõesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateStatusToolStripMenuItem,
            this.exibirMsgConsoleToolStripMenuItem,
            this.exibirMensagensDeDownloadToolStripMenuItem,
            this.exibirFerramentasToolStripMenuItem,
            this.toolStripSeparator4,
            this.updateStartToolStripMenuItem,
            this.visualizarHistóricoDeAtualizaçãoToolStripMenuItem,
            this.toolStripSeparator1,
            this.forcarAtualizacaoCompletaToolStripMenuItem,
            this.toolStripSeparator5,
            this.aboutToolStripMenuItem});
            this.configuraçõesToolStripMenuItem.Name = "configuraçõesToolStripMenuItem";
            this.configuraçõesToolStripMenuItem.Size = new System.Drawing.Size(96, 20);
            this.configuraçõesToolStripMenuItem.Text = "Configurações";
            // 
            // updateStatusToolStripMenuItem
            // 
            this.updateStatusToolStripMenuItem.Name = "updateStatusToolStripMenuItem";
            this.updateStatusToolStripMenuItem.Size = new System.Drawing.Size(321, 22);
            this.updateStatusToolStripMenuItem.Text = "Exibir progresso de atualização de documentos";
            this.updateStatusToolStripMenuItem.Click += new System.EventHandler(this.UpdateStatusItemClick);
            // 
            // exibirMsgConsoleToolStripMenuItem
            // 
            this.exibirMsgConsoleToolStripMenuItem.Name = "exibirMsgConsoleToolStripMenuItem";
            this.exibirMsgConsoleToolStripMenuItem.Size = new System.Drawing.Size(321, 22);
            this.exibirMsgConsoleToolStripMenuItem.Text = "Exibir mensagens do console";
            this.exibirMsgConsoleToolStripMenuItem.Click += new System.EventHandler(this.DisplayOutputMessagesItemClick);
            // 
            // exibirMensagensDeDownloadToolStripMenuItem
            // 
            this.exibirMensagensDeDownloadToolStripMenuItem.Name = "exibirMensagensDeDownloadToolStripMenuItem";
            this.exibirMensagensDeDownloadToolStripMenuItem.Size = new System.Drawing.Size(321, 22);
            this.exibirMensagensDeDownloadToolStripMenuItem.Text = "Exibir estado de download";
            this.exibirMensagensDeDownloadToolStripMenuItem.Click += new System.EventHandler(this.DownloadMessagesItemClick);
            // 
            // exibirFerramentasToolStripMenuItem
            // 
            this.exibirFerramentasToolStripMenuItem.Name = "exibirFerramentasToolStripMenuItem";
            this.exibirFerramentasToolStripMenuItem.Size = new System.Drawing.Size(321, 22);
            this.exibirFerramentasToolStripMenuItem.Text = "Exibir ferramentas do desenvolvedor";
            this.exibirFerramentasToolStripMenuItem.Click += new System.EventHandler(this.ShowDevToolsMenuItemClick);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(318, 6);
            // 
            // updateStartToolStripMenuItem
            // 
            this.updateStartToolStripMenuItem.Name = "updateStartToolStripMenuItem";
            this.updateStartToolStripMenuItem.Size = new System.Drawing.Size(321, 22);
            this.updateStartToolStripMenuItem.Text = "Iniciar atualização de documentos";
            this.updateStartToolStripMenuItem.Click += new System.EventHandler(this.UpdateStartItemClick);
            // 
            // visualizarHistóricoDeAtualizaçãoToolStripMenuItem
            // 
            this.visualizarHistóricoDeAtualizaçãoToolStripMenuItem.Name = "visualizarHistóricoDeAtualizaçãoToolStripMenuItem";
            this.visualizarHistóricoDeAtualizaçãoToolStripMenuItem.Size = new System.Drawing.Size(321, 22);
            this.visualizarHistóricoDeAtualizaçãoToolStripMenuItem.Text = "Visualizar histórico de atualização";
            this.visualizarHistóricoDeAtualizaçãoToolStripMenuItem.Click += new System.EventHandler(this.VisualHistoricoItemClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(318, 6);
            // 
            // forcarAtualizacaoCompletaToolStripMenuItem
            // 
            this.forcarAtualizacaoCompletaToolStripMenuItem.Name = "forcarAtualizacaoCompletaToolStripMenuItem";
            this.forcarAtualizacaoCompletaToolStripMenuItem.Size = new System.Drawing.Size(321, 22);
            this.forcarAtualizacaoCompletaToolStripMenuItem.Text = "Forçar atualização completa";
            this.forcarAtualizacaoCompletaToolStripMenuItem.Click += new System.EventHandler(this.ForcarAtualizacaoCompletaItemClick);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(318, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(321, 22);
            this.aboutToolStripMenuItem.Text = "Sobre";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItemClick);
            // 
            // browserTabControl
            // 
            this.browserTabControl.Controls.Add(this.btnNewTab);
            this.browserTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browserTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.browserTabControl.Location = new System.Drawing.Point(0, 0);
            this.browserTabControl.Name = "browserTabControl";
            this.browserTabControl.SelectedIndex = 0;
            this.browserTabControl.ShowToolTips = true;
            this.browserTabControl.Size = new System.Drawing.Size(483, 453);
            this.browserTabControl.TabIndex = 2;
            this.browserTabControl.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.BrowserTabDrawItem);
            this.browserTabControl.SelectedIndexChanged += new System.EventHandler(this.BrowserTabSelectedIndexChanged);
            this.browserTabControl.MouseClick += new System.Windows.Forms.MouseEventHandler(this.BrowserTabMouseClick);
            // 
            // btnNewTab
            // 
            this.btnNewTab.Location = new System.Drawing.Point(4, 22);
            this.btnNewTab.Name = "btnNewTab";
            this.btnNewTab.Padding = new System.Windows.Forms.Padding(3);
            this.btnNewTab.Size = new System.Drawing.Size(475, 427);
            this.btnNewTab.TabIndex = 0;
            this.btnNewTab.ToolTipText = "Nova guia";
            this.btnNewTab.UseVisualStyleBackColor = true;
            // 
            // statusOutputLinkLabel
            // 
            this.statusOutputLinkLabel.AutoSize = true;
            this.statusOutputLinkLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusOutputLinkLabel.Location = new System.Drawing.Point(0, 477);
            this.statusOutputLinkLabel.Name = "statusOutputLinkLabel";
            this.statusOutputLinkLabel.Size = new System.Drawing.Size(0, 13);
            this.statusOutputLinkLabel.TabIndex = 0;
            this.statusOutputLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.StatusOutputLinkLabelClick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeViewSearch);
            this.splitContainer1.Panel1.Controls.Add(this.treeviewStatusLabel);
            this.splitContainer1.Panel1.Controls.Add(this.btnSearch);
            this.splitContainer1.Panel1.Controls.Add(this.txtBookmarkSearch);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.browserTabControl);
            this.splitContainer1.Size = new System.Drawing.Size(730, 453);
            this.splitContainer1.SplitterDistance = 243;
            this.splitContainer1.TabIndex = 0;
            // 
            // treeViewSearch
            // 
            this.treeViewSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewSearch.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeViewSearch.HideSelection = false;
            this.treeViewSearch.HotTracking = true;
            this.treeViewSearch.Location = new System.Drawing.Point(0, 56);
            this.treeViewSearch.Name = "treeViewSearch";
            this.treeViewSearch.ShowNodeToolTips = true;
            this.treeViewSearch.Size = new System.Drawing.Size(243, 397);
            this.treeViewSearch.TabIndex = 2;
            this.treeViewSearch.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.TreeViewSearchNodeDraw);
            this.treeViewSearch.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeViewSearchNodeClick);
            // 
            // treeviewStatusLabel
            // 
            this.treeviewStatusLabel.AutoSize = true;
            this.treeviewStatusLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.treeviewStatusLabel.Location = new System.Drawing.Point(0, 43);
            this.treeviewStatusLabel.Name = "treeviewStatusLabel";
            this.treeviewStatusLabel.Size = new System.Drawing.Size(0, 13);
            this.treeviewStatusLabel.TabIndex = 3;
            // 
            // btnSearch
            // 
            this.btnSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSearch.Location = new System.Drawing.Point(0, 20);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(243, 23);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Pesquisa SOS";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.SearchBookmarksButtonClick);
            // 
            // txtBookmarkSearch
            // 
            this.txtBookmarkSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtBookmarkSearch.Location = new System.Drawing.Point(0, 0);
            this.txtBookmarkSearch.Name = "txtBookmarkSearch";
            this.txtBookmarkSearch.Size = new System.Drawing.Size(243, 20);
            this.txtBookmarkSearch.TabIndex = 0;
            this.txtBookmarkSearch.TextChanged += new System.EventHandler(this.TxtBookmarkSearchTextChanged);
            this.txtBookmarkSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtBookmarkSearchKeyDown);
            // 
            // BrowserInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 490);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusOutputLinkLabel);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "BrowserInterface";
            this.Text = "BrowserForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.browserTabControl.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem findMenuItem;
        private System.Windows.Forms.TabControl browserTabControl;
        private System.Windows.Forms.ToolStripMenuItem newTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printToPdfToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configuraçõesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exibirMsgConsoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem exibirFerramentasToolStripMenuItem;
        private System.Windows.Forms.TabPage btnNewTab;
        private System.Windows.Forms.LinkLabel statusOutputLinkLabel;
        private System.Windows.Forms.ToolStripMenuItem exibirMensagensDeDownloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateStatusToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem updateStartToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtBookmarkSearch;
        private System.Windows.Forms.TreeView treeViewSearch;
        private System.Windows.Forms.Label treeviewStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem visualizarHistóricoDeAtualizaçãoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forcarAtualizacaoCompletaToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem historicoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem atualizaçãoPorMáquinaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem acessoPorUsuárioToolStripMenuItem;
    }
}

