using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaTekDocuments.controller;
using MediaTekDocuments.model;

namespace MediaTekDocuments.view
{
    public partial class LoginForm : Form
    {
        private readonly FrmMediatekController controller;
        public Utilisateur userAuth { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            txtUsername.Focus();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string login = txtUsername.Text;
            string pwd = txtPwd.Text;

            if (string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Veuillez renseigner le nom d'utilisateur", "Nom d'utilisateur manquant", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrEmpty(pwd))
            {
                MessageBox.Show("Veuillez renseigner le mot de passe", "Mot de passe manquant", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPwd.Focus();
                return;
            }

            Utilisateur user = controller.getUser(login);

            if (user == null)
            {
                MessageBox.Show("Utilisateur inconnu.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                clearFields();
                return;
            }

            if (user.MotDePasse != pwd)
            {
                MessageBox.Show("Mot de passe incorrect.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPwd.Text = "";
                txtPwd.Focus();
                return;
            }

            this.userAuth = user;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void lblExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void clearFields()
        {
            txtPwd.Text = "";
            txtUsername.Text = "";
            txtUsername.Focus();
        }
    }
}
