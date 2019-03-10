using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOS
{
    public partial class Login : Form
    {
        static HttpClient client = new HttpClient();

        public Login()
        {
            InitializeComponent();
            lblLinkEsqueceuSenha.Visible = false;
            lblLoginResult.Visible = false;
        }


        private void btnEntrar_Click(object sender, EventArgs e)
        {
            LoginAsync();
        }
        private void txtPasswordKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }
            LoginAsync();
        }
        private async void LoginAsync()
        {
            btnEntrar.Enabled = false;
            try
            {
                await WebScrap.DiagramasAuthCDRE(txtUsername.Text, txtPassword.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Não foi possível concluir a ação. Cheque se está conectado à Intranet e Internet. Caso o problema persista comunique o administrador da aplicação");
            }
            if (WebScrap.IsCDREAuthenticated)
            {
                const bool multiThreadedMessageLoop = true;
                var browser = new BrowserInterface(multiThreadedMessageLoop);
                Hide();
                browser.Closed += (s, args) =>
                {
                    Close();
                };
                browser.Show();
            }
            else
            {
                lblLinkEsqueceuSenha.Visible = true;
                lblLoginResult.Visible = true;
                lblLoginResult.ForeColor = Color.Red;
            }
            btnEntrar.Enabled = true;

        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOffline_Click(object sender, EventArgs e)
        {
            const bool multiThreadedMessageLoop = true;
            var browser = new BrowserInterface(multiThreadedMessageLoop, true);
            Hide();
            browser.Closed += (s, args) => Close();
            browser.Show();
        }

        private void lbllinkSolicitarCadastro_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://cdreweb.ons.org.br/CDRE/Views/SolicitarCadastro/SolicitarCadastro.aspx");
        }

        private void lblLinkEsqueceuSenha_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://pops.ons.org.br/ons.pop.federation/passwordrecovery/");
        }
    }
}
