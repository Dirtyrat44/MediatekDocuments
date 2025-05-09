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
    /// <summary>
    /// Formulaire affichant la liste des abonnements qui se termine dans les 30 jours
    /// </summary>
    public partial class FrmAlerteAbonnements : Form
    {
        /// <summary>
        /// Constructeur du formulaire
        /// </summary>
        /// <param name="alertes"></param>
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
