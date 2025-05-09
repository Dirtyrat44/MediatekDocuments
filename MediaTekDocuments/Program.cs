using MediaTekDocuments.view;
using MediaTekDocuments.model;
using System;
using System.Windows.Forms;

namespace MediaTekDocuments
{
    /// <summary>
    /// Point d'entrée principal de l'application.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LoginForm loginForm = new LoginForm();
            DialogResult result = loginForm.ShowDialog();

            if (result != DialogResult.OK)
            {
                return;
            }

            Utilisateur user = loginForm.userAuth;
            Application.Run(new FrmMediatek(user));
        }
    }
}
