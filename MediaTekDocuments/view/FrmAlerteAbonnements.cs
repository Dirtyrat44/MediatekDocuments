using System;
using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaTekDocuments.view
{
    public partial class FrmAlerteAbonnements : Form
    {
        public FrmAlerteAbonnements(List<AbonnementAlerteDto> alertes)
        {
            InitializeComponent();
            dgvAlerte.AutoGenerateColumns = false;
            dgvAlerte.DataSource = alertes;
            
        }

        private void btnAlerteOk_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
