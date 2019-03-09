using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOS
{
    public partial class Historico : Form
    {
        private object DataSource { get; set; }
        private string DisplayConfig { get; set; }
        public Historico(object list, string displayConfig)
        {
            InitializeComponent();
            DataSource = list;
            DisplayConfig = displayConfig;
            Load += Historico_Load;
            //Only perform layout when control has completly finished resizing
            ResizeBegin += (s, e) => SuspendLayout();
            ResizeEnd += (s, e) => ResumeLayout(true);
        }

        private void Historico_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = DataSource;
            if (DisplayConfig == "maquina")
            {
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                //Data

                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                //PcMachineName
                dataGridView1.Columns[1].HeaderText = "Máquina";

                dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                //PCUsername
                dataGridView1.Columns[2].HeaderText = "Usuário";

                dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                //Concluido
                dataGridView1.Columns[3].HeaderText = "0 Erros";

                dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells; //Msg
                dataGridView1.Columns[4].HeaderText = "Mensagem";
            }
            else if (DisplayConfig == "acesso")
            {
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dataGridView1.Columns[1].HeaderText = "Máquina";

                dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dataGridView1.Columns[2].HeaderText = "Usuário";

                dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dataGridView1.Columns[3].HeaderText = "Origem SSC";

                dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dataGridView1.Columns[4].HeaderText = "Índice";

                dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells; //Url
                dataGridView1.Columns[5].HeaderText = "Arquivo";
            }


        }
    }
}
