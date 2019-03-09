namespace SOS
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.label1 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnEntrar = new System.Windows.Forms.Button();
            this.btnSair = new System.Windows.Forms.Button();
            this.lblLoginResult = new System.Windows.Forms.Label();
            this.btnOffline = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lbllinkSolicitarCadastro = new System.Windows.Forms.LinkLabel();
            this.lblLinkEsqueceuSenha = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(62, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Usuário:";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(114, 62);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(109, 20);
            this.txtUsername.TabIndex = 3;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(114, 88);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(109, 20);
            this.txtPassword.TabIndex = 5;
            this.txtPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPasswordKeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(67, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Senha:";
            // 
            // btnEntrar
            // 
            this.btnEntrar.Location = new System.Drawing.Point(171, 150);
            this.btnEntrar.Name = "btnEntrar";
            this.btnEntrar.Size = new System.Drawing.Size(82, 23);
            this.btnEntrar.TabIndex = 7;
            this.btnEntrar.Text = "Entrar";
            this.btnEntrar.UseVisualStyleBackColor = true;
            this.btnEntrar.Click += new System.EventHandler(this.btnEntrar_Click);
            // 
            // btnSair
            // 
            this.btnSair.Location = new System.Drawing.Point(259, 150);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(82, 23);
            this.btnSair.TabIndex = 8;
            this.btnSair.Text = "Sair";
            this.btnSair.UseVisualStyleBackColor = true;
            this.btnSair.Click += new System.EventHandler(this.btnSair_Click);
            // 
            // lblLoginResult
            // 
            this.lblLoginResult.AutoSize = true;
            this.lblLoginResult.Location = new System.Drawing.Point(62, 111);
            this.lblLoginResult.Name = "lblLoginResult";
            this.lblLoginResult.Size = new System.Drawing.Size(224, 13);
            this.lblLoginResult.TabIndex = 9;
            this.lblLoginResult.Text = "Sua tentativa de acesso não obteve sucesso.";
            // 
            // btnOffline
            // 
            this.btnOffline.Location = new System.Drawing.Point(12, 150);
            this.btnOffline.Name = "btnOffline";
            this.btnOffline.Size = new System.Drawing.Size(82, 23);
            this.btnOffline.TabIndex = 11;
            this.btnOffline.Text = "Entrar Offline";
            this.btnOffline.UseVisualStyleBackColor = true;
            this.btnOffline.Click += new System.EventHandler(this.btnOffline_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SOS.Properties.Resources.logo_cdre;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(329, 43);
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // lbllinkSolicitarCadastro
            // 
            this.lbllinkSolicitarCadastro.AutoSize = true;
            this.lbllinkSolicitarCadastro.Location = new System.Drawing.Point(226, 65);
            this.lbllinkSolicitarCadastro.Name = "lbllinkSolicitarCadastro";
            this.lbllinkSolicitarCadastro.Size = new System.Drawing.Size(115, 13);
            this.lbllinkSolicitarCadastro.TabIndex = 13;
            this.lbllinkSolicitarCadastro.TabStop = true;
            this.lbllinkSolicitarCadastro.Text = "Solicitar novo cadastro";
            this.lbllinkSolicitarCadastro.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbllinkSolicitarCadastro_LinkClicked);
            // 
            // lblLinkEsqueceuSenha
            // 
            this.lblLinkEsqueceuSenha.AutoSize = true;
            this.lblLinkEsqueceuSenha.Location = new System.Drawing.Point(62, 124);
            this.lblLinkEsqueceuSenha.Name = "lblLinkEsqueceuSenha";
            this.lblLinkEsqueceuSenha.Size = new System.Drawing.Size(113, 13);
            this.lblLinkEsqueceuSenha.TabIndex = 14;
            this.lblLinkEsqueceuSenha.TabStop = true;
            this.lblLinkEsqueceuSenha.Text = "Esqueceu sua senha?";
            this.lblLinkEsqueceuSenha.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblLinkEsqueceuSenha_LinkClicked);
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 185);
            this.Controls.Add(this.lblLinkEsqueceuSenha);
            this.Controls.Add(this.lbllinkSolicitarCadastro);
            this.Controls.Add(this.btnOffline);
            this.Controls.Add(this.lblLoginResult);
            this.Controls.Add(this.btnSair);
            this.Controls.Add(this.btnEntrar);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SOS - Autenticação pelo CDRE ONS";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnEntrar;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.Label lblLoginResult;
        private System.Windows.Forms.Button btnOffline;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel lbllinkSolicitarCadastro;
        private System.Windows.Forms.LinkLabel lblLinkEsqueceuSenha;
    }
}