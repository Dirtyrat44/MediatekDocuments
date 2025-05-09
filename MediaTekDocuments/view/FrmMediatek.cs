using System;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;
using MediaTekDocuments.manager;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;

namespace MediaTekDocuments.view

{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmMediatek : Form
    {
        #region Commun
        private readonly FrmMediatekController controller;
        private readonly Utilisateur _user;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private List<Etat> lesEtats = new List<Etat>();

        /// <summary>
        /// Constructeur vide pour le designer
        /// </summary>
        internal FrmMediatek() : this(null)
        {

        }

        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        internal FrmMediatek(Utilisateur user)
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
            this._user = user;
        }

        /// <summary>
        /// Charge les données initiales et l'alerte si besoin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMediatek_Load(object sender, EventArgs e)
        {
            serviceRestriction();
            RefreshAllLists();
            if (_user.IdService == 3) return;
            Func<string, List<AbonnementDto>> getAbonnements = idRevue => controller.GetAllAbonnements(idRevue);

            List<AbonnementAlerteDto> alertes = AbonnementService.GetAlertesAbonnements(lesRevues, getAbonnements);

            if (alertes.Any())
            {
                Form frmAlerte = new FrmAlerteAbonnements(alertes);
                frmAlerte.ShowDialog(this);
            }
        }

        /// <summary>
        /// Gère les restrictions en fonction du service de l'utilisateur
        /// </summary>
        private void serviceRestriction()
        {
            tabCommandes.Enabled = false;
            tabOngletsApplication.Enabled = false;
            tabAdd.Enabled = false;

            switch (_user.IdService)
            {
                // Admin/Administratif
                case 1:
                case 2:
                    tabCommandes.Enabled = true;
                    tabOngletsApplication.Enabled = true;
                    break;  
                // Prêts
                case 3:
                    tabCommandes.Enabled = false;
                    tabAdd.Enabled = false;
                    tabOngletsApplication.Enabled = true;
                    tabOngletsApplication.TabPages.Remove(tabAdd);
                    tabOngletsApplication.TabPages.Remove(tabCommandes);                   
                    break;

                // Culture
                case 4:
                    MessageBox.Show("Votre service n'a pas les droits suffisants pour accéder à cette application.", "Accès refusé", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Application.Exit();
                    return;
            }
        }

        /// <summary>
        /// Cache les colonnes si mauvais service
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="columnName"></param>
        private void HideColumn(DataGridView dgv, string columnName)
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName].Visible = false;
            }
        }

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Ajoute deux colonnes modifier et supprimer à DataGridView si elles n'existent pas déjà
        /// Si IdService n'est pas admin ou administratif ne fait rien
        /// </summary>
        public void AjouterBoutons(DataGridView dgv)
        {
            if (_user.IdService != 3)
            {
                if (dgv.Columns["Modifier"] == null)
                {
                    DataGridViewButtonColumn btnModifier = new DataGridViewButtonColumn
                    {
                        HeaderText = "",
                        Name = "Modifier",
                        Text = " Modifier ",
                        UseColumnTextForButtonValue = true,
                        SortMode = DataGridViewColumnSortMode.NotSortable
                    };
                    dgv.Columns.Add(btnModifier);
                }

                if (dgv.Columns["Supprimer"] == null)
                {
                    DataGridViewButtonColumn btnSupprimer = new DataGridViewButtonColumn
                    {
                        HeaderText = "",
                        Name = "Supprimer",
                        Text = " X ",
                        UseColumnTextForButtonValue = true,
                        SortMode = DataGridViewColumnSortMode.NotSortable
                    };
                    dgv.Columns.Add(btnSupprimer);
                }

                int lastIndex = dgv.Columns.Count - 1;
                dgv.Columns["Supprimer"].DisplayIndex = lastIndex;
                dgv.Columns["Modifier"].DisplayIndex = lastIndex - 1;
            }
        }

        /// <summary>
        /// Recharge toutes les collections et rafraîchit les trois grilles.
        /// </summary>
        private void RefreshAllLists()
        {
            lesRevues = controller.GetAllRevues();
            lesLivres = controller.GetAllLivres();
            lesDvd = controller.GetAllDvd();
            lesEtats = controller.getEtat();

            RemplirRevuesListe(lesRevues);
            RemplirLivresListe(lesLivres);
            RemplirDvdListe(lesDvd);

            RemplirEtats(cBEtatLivre);
            RemplirEtats(cBEtatDvd);
            RemplirEtats(cBEtatRevue);
        }

        /// <summary>
        /// Intercepte la sélection de l'onglet nouveau document
        /// Annule la navigation vers l'onglet
        /// Ouvre la fenêtre de création d'un document
        /// Si tout s'est bien passé, rafraîchit la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabOngletsApplication_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == tabAdd)
            {
                e.Cancel = true;
                FrmDocEdit frmEdit = new FrmDocEdit(this.controller);

                if (frmEdit.ShowDialog() == DialogResult.OK)
                {
                    RefreshAllLists();
                }
            }
            else if (e.TabPage == tabCommandes)
            {
                tabCommandesRevue_Enter(null, EventArgs.Empty);
                tabCommandeLivres_Enter(null, EventArgs.Empty);
                tabCommandesDVD_Enter(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Modifie la taille de la fenêtre selon l'onglet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabOngletsApplication_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabOngletsApplication.SelectedTab.Name)
            {
                case "tabLivres":
                case "tabDvd":
                    this.Size = new Size(900, 820);
                    break;

                default:
                    this.Size = new Size(900, 700);
                    break;
            }
        }

        /// <summary>
        /// Rempli les combos etats
        /// </summary>
        /// <param name="cmb"></param>
        private void RemplirEtats(ComboBox cmb)
        {
            var sortedEtat = lesEtats.OrderBy(et => et.Id).ToList();
            cmb.DataSource = sortedEtat;
            cmb.DisplayMember = "Libelle";
            cmb.ValueMember = "Id";
            cmb.SelectedIndex = -1;
        }        
        #endregion

        #region Onglet Livres
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private readonly BindingSource bdgLivresExemplaires = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();
        private List<Exemplaire> lesExemplairesLivres = new List<Exemplaire>();

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
            dgvLivresListe.Columns["auteur"].DisplayIndex = 2;
            dgvLivresListe.Columns["collection"].DisplayIndex = 3;
            AjouterBoutons(dgvLivresListe);

        }

        /// <summary>
        /// Rempli le datagrid avec les exemplaires du livres
        /// </summary>
        /// <param name="id"></param>
        private void RemplirLivresExemplaires(string id)
        {
            bool userModif = _user.IdService != 3;
            lesExemplairesLivres = controller.GetExemplaires(id);

            var sortedList = lesExemplairesLivres.OrderByDescending(e => e.DateAchat).Select(e => new
            {
                Id = e.Id,
                Numero = e.Numero,
                IdEtat = e.IdEtat,
                DateAchat = e.DateAchat.ToShortDateString(),
                Etat = lesEtats.Find(et => et.Id == e.IdEtat).Libelle
            }).ToList();

            if (sortedList.Count == 0)
            {
                VideGridView(dgvLivreExemplaire);
                grpBxExLivre.Enabled = false;
                cBEtatLivre.SelectedIndex = -1;
                return;
            }

            bdgLivresExemplaires.DataSource = sortedList;
            dgvLivreExemplaire.Visible = false;
            dgvLivreExemplaire.DataSource = bdgLivresExemplaires;
            grpBxExLivre.Enabled = userModif;
            AjouterBoutons(dgvLivreExemplaire);

            dgvLivreExemplaire.Columns["IdEtat"].Visible = false;
            HideColumn(dgvLivreExemplaire, "Modifier");
            dgvLivreExemplaire.Columns["Id"].Visible = false;            

            dgvLivreExemplaire.Columns["DateAchat"].HeaderText = "Date d'achat";
            dgvLivreExemplaire.Columns["Numero"].HeaderText = "N° exemplaire";
            dgvLivreExemplaire.Visible = true;
            dgvLivreExemplaire.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }        

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            txbLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            RemplirLivresExemplaires(livre.Id);
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
        }

        /// <summary>
        /// Affiche l'état par défault de l'exemplaire selectionné ou rien
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivreExemplaire_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivreExemplaire.CurrentRow != null)
            {

                string idEtat = dgvLivreExemplaire.CurrentRow.Cells["IdEtat"].Value?.ToString();

                if (!string.IsNullOrEmpty(idEtat))
                {
                    cBEtatLivre.SelectedValue = idEtat;
                }
                else
                {
                    cBEtatLivre.SelectedIndex = -1;
                }
            }
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Boutton pour enregistrer le changement d'état
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveExLivre_Click(object sender, EventArgs e)
        {
            if (dgvLivreExemplaire.CurrentRow == null || cBEtatLivre.SelectedIndex < 0)
            {
                MessageBox.Show("Veuillez sélectionner un exemplaire et un état.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string idLivre = dgvLivresListe.CurrentRow?.Cells["Id"].Value.ToString();
            string numero = dgvLivreExemplaire.CurrentRow.Cells["Numero"].Value.ToString();
            string idEtat = cBEtatLivre.SelectedValue.ToString();

            var exemplaire = new
            {
                id = idLivre,
                numero = numero,
                idEtat = idEtat
            };

            bool success = controller.addUpdateDocument("exemplaire", exemplaire, false);

            if (!success)
            {
                MessageBox.Show("Échec de la mise à jour de l'état.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            RemplirLivresExemplaires(idLivre);
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                }
                catch
                {
                    VideLivresZones();
                    VideGridView(dgvLivreExemplaire);
                }
            }
            else
            {
                VideLivresInfos();
                VideGridView(dgvLivreExemplaire);
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList;
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
                default:
                    return;
            }
            RemplirLivresListe(sortedList);
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivreExemplaire_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvLivreExemplaire.Columns[e.ColumnIndex].HeaderText;

            IEnumerable<Exemplaire> sortedList;

            switch (titreColonne)
            {
                case "N° exemplaire":
                    sortedList = lesExemplairesLivres.OrderBy(ex => ex.Numero);
                    break;
                case "Date d'achat":
                    sortedList = lesExemplairesLivres.OrderBy(ex => ex.DateAchat);
                    break;
                case "Etat":
                    sortedList = lesExemplairesLivres.OrderBy(ex => lesEtats.Find(et => et.Id == ex.IdEtat).Libelle);
                    break;
                default:
                    return;
            }

            // Reconstruit la liste avec le format du grid
            var afficherListe = sortedList.Select(exemp => new {
                Id = exemp.Id,
                Numero = exemp.Numero,
                IdEtat = exemp.IdEtat,
                DateAchat = exemp.DateAchat.ToShortDateString(),
                Etat = lesEtats.Find(et => et.Id == exemp.IdEtat).Libelle
            })
            .ToList();

                bdgLivresExemplaires.DataSource = afficherListe;
                dgvLivreExemplaire.DataSource = bdgLivresExemplaires;
        }

        /// <summary>
        /// Gère les clics sur les boutons Modifier et Supprimer dans la grille des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresListe_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            DataGridView dgv = (DataGridView)sender;
            string colName = dgv.Columns[e.ColumnIndex].Name;
            Livre livreSelectionne = (Livre)dgv.Rows[e.RowIndex].DataBoundItem;

            if (colName == "Modifier")
            {
                FrmDocEdit frmEdit = new FrmDocEdit(this.controller, livreSelectionne);
                if (frmEdit.ShowDialog() == DialogResult.OK)
                {
                    RefreshAllLists();
                }
            }
            else if (colName == "Supprimer")
            {
                DialogResult res = MessageBox.Show("Êtes vous sûr de vouloir supprimer " + livreSelectionne.Titre + "?", "Confirmation de suppression", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (res == DialogResult.Yes)
                {
                    bool result = controller.DeleteDocument(livreSelectionne.Id, "livre");
                    if (!result)
                    {
                        MessageBox.Show("Impossible de supprimer " + livreSelectionne.Titre + ", il est lié à une commande ou un exemplaire.", "Erreur : suppression impossible", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    RefreshAllLists();
                }
            }
        }

        /// <summary>
        /// Gère la suppression d'un exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivreExemplaire_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string colName = dgvLivreExemplaire.Columns[e.ColumnIndex].Name;

            if (colName == "Supprimer")
            {
                string id = dgvLivreExemplaire.Rows[e.RowIndex].Cells["Id"].Value.ToString();
                string numero = dgvLivreExemplaire.Rows[e.RowIndex].Cells["Numero"].Value.ToString();
                DialogResult res = MessageBox.Show("Êtes vous sûr de vouloir supprimer l'exemplaire n°" + numero + "?", "Confirmation de suppression", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (res == DialogResult.OK)
                {
                    bool result = controller.DeleteDocument(id, "exemplaire", numero);
                    if (!result)
                    {
                        MessageBox.Show("Impossible de supprimer l'exemplaire n°" + numero, "Erreur : suppression impossible", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    RemplirLivresExemplaires(id);
                }
            }
        }

        #endregion

        #region Onglet Dvd
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private readonly BindingSource bdgDvdExemplaires = new BindingSource();
        private List<Dvd> lesDvd = new List<Dvd>();
        private List<Exemplaire> lesExemplairesDvd = new List<Exemplaire>();

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="Dvds">liste de dvd</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Boutton pour enregistrer le changement d'état
        /// </summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveExDvd_Click(object sender, EventArgs e)
        {

            if (dgvDvdExemplaire.CurrentRow == null || cBEtatDvd.SelectedIndex < 0)
            {
                MessageBox.Show("Veuillez sélectionner un exemplaire et un état.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string idDvd = dgvDvdExemplaire.CurrentRow?.Cells["Id"].Value.ToString();
            string numero = dgvDvdExemplaire.CurrentRow.Cells["Numero"].Value.ToString();
            string idEtat = cBEtatDvd.SelectedValue.ToString();

            var exemplaire = new
            {
                id = idDvd,
                numero = numero,
                idEtat = idEtat
            };

            bool success = controller.addUpdateDocument("exemplaire", exemplaire, false);

            if (!success)
            {
                MessageBox.Show("Échec de la mise à jour de l'état.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RemplirDvdExemplaires(idDvd);
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            RemplirDvdExemplaires(dvd.Id);
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Evénement pour le DataGridView DVD
        /// Masque les colonnes inutiles et ajout des boutons modifier et supprimer
        /// Permet d'organiser les colonnes dans le bon ordre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;

            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
            dgvDvdListe.Columns["realisateur"].DisplayIndex = 2;
            dgvDvdListe.Columns["duree"].DisplayIndex = 3;

            AjouterBoutons(dgvDvdListe);
        }

        /// <summary>
        /// Gère la suppression d'un exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdExemplaire_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string colName = dgvDvdExemplaire.Columns[e.ColumnIndex].Name;

            if (colName == "Supprimer")
            {
                string id = dgvDvdExemplaire.Rows[e.RowIndex].Cells["Id"].Value.ToString();
                string numero = dgvDvdExemplaire.Rows[e.RowIndex].Cells["Numero"].Value.ToString();
                DialogResult res = MessageBox.Show("Êtes vous sûr de vouloir supprimer l'exemplaire n°" + numero + "?", "Confirmation de suppression", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (res == DialogResult.OK)
                {
                    bool result = controller.DeleteDocument(id, "exemplaire", numero);
                    if (!result)
                    {
                        MessageBox.Show("Impossible de supprimer l'exemplaire n°" + numero, "Erreur : suppression impossible", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    RemplirDvdExemplaires(id);
                }
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Rempli le datagrid d'exemplaires du dvd
        /// </summary>
        /// <param name="id"></param>
        private void RemplirDvdExemplaires(string id)
        {
            bool userModif = _user.IdService != 3;
            lesExemplairesDvd = controller.GetExemplaires(id);

            var sortedList = lesExemplairesDvd.OrderByDescending(e => e.DateAchat).Select(e => new
                {
                    Id = e.Id,
                    Numero = e.Numero,
                    IdEtat = e.IdEtat,
                    DateAchat = e.DateAchat.ToShortDateString(),
                    Etat = lesEtats.Find(et => et.Id == e.IdEtat).Libelle
                }).ToList();

            if (sortedList.Count == 0)
            {
                VideGridView(dgvDvdExemplaire);
                gBExDvd.Enabled = false;
                cBEtatDvd.SelectedIndex = -1;
                return;
            }

            bdgDvdExemplaires.DataSource = sortedList;
            dgvDvdExemplaire.Visible = false;
            dgvDvdExemplaire.DataSource = bdgDvdExemplaires;
            gBExDvd.Enabled = userModif;
            AjouterBoutons(dgvDvdExemplaire);

            dgvDvdExemplaire.Columns["Id"].Visible = false;
            dgvDvdExemplaire.Columns["IdEtat"].Visible = false;
            HideColumn(dgvDvdExemplaire, "Modifier");

            dgvDvdExemplaire.Columns["DateAchat"].HeaderText = "Date d'achat";
            dgvDvdExemplaire.Columns["Numero"].HeaderText = "N° exemplaire";
            dgvDvdExemplaire.Visible = true;

            dgvDvdExemplaire.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        /// <summary>
        /// Affiche l'état par défault de l'exemplaire selectionné ou rien
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdExemplaire_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdExemplaire.CurrentRow != null)
            {

                string idEtat = dgvDvdExemplaire.CurrentRow.Cells["IdEtat"].Value?.ToString();

                if (!string.IsNullOrEmpty(idEtat))
                {
                    cBEtatDvd.SelectedValue = idEtat;
                }
                else
                {
                    cBEtatDvd.SelectedIndex = -1;
                }
            }
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList;
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
                default:
                    return;
            }
            RemplirDvdListe(sortedList);
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdExemplaire_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvDvdExemplaire.Columns[e.ColumnIndex].HeaderText;

            IEnumerable<Exemplaire> sortedList;

            switch (titreColonne)
            {
                case "N° exemplaire":
                    sortedList = lesExemplairesDvd.OrderBy(ex => ex.Numero);
                    break;
                case "Date d'achat":
                    sortedList = lesExemplairesDvd.OrderBy(ex => ex.DateAchat);
                    break;
                case "Etat":
                    sortedList = lesExemplairesDvd.OrderBy(ex => lesEtats.Find(et => et.Id == ex.IdEtat).Libelle);
                    break;
                default:
                    return;
            }

            // Reconstruit la liste avec le format du grid
            var afficherListe = sortedList.Select(exemp => new {
                Id = exemp.Id,
                Numero = exemp.Numero,
                IdEtat = exemp.IdEtat,
                DateAchat = exemp.DateAchat.ToShortDateString(),
                Etat = lesEtats.Find(et => et.Id == exemp.IdEtat).Libelle
            })
            .ToList();

            bdgDvdExemplaires.DataSource = afficherListe;
            dgvDvdExemplaire.DataSource = bdgDvdExemplaires;
        }

        /// <summary>
        /// Gère les clics sur les boutons Modifier et Supprimer dans la grille des DVD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            DataGridView dgv = (DataGridView)sender;
            string colName = dgv.Columns[e.ColumnIndex].Name;
            Dvd dvdSelectionne = (Dvd)dgv.Rows[e.RowIndex].DataBoundItem;

            if (colName == "Modifier")
            {
                FrmDocEdit frmEdit = new FrmDocEdit(this.controller, dvdSelectionne);
                if (frmEdit.ShowDialog() == DialogResult.OK)
                {
                    RefreshAllLists();
                }
            }
            else if (colName == "Supprimer")
            {
                DialogResult res = MessageBox.Show("Êtes vous sûr de vouloir supprimer " + dvdSelectionne.Titre + "?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (res == DialogResult.Yes)
                {
                    bool result = controller.DeleteDocument(dvdSelectionne.Id, "dvd");
                    if (!result)
                    {
                        MessageBox.Show("Impossible de supprimer " + dvdSelectionne.Titre + ", il est lié à une commande ou un exemplaire.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    RefreshAllLists();
                }
            }
        }

        #endregion

        #region Onglet Revues
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private List<Revue> lesRevues = new List<Revue>();

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="revues"></param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }

        /// <summary>
        /// Evénement pour le DataGridView Revue
        /// Masque les colonnes inutiles et ajout des boutons modifier et supprimer
        /// Permet d'organiser les colonnes dans le bon ordre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
            dgvRevuesListe.Columns["periodicite"].DisplayIndex = 2;
            dgvRevuesListe.Columns["delaiMiseADispo"].DisplayIndex = 3;
            AjouterBoutons(dgvRevuesListe);
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList;
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
                default:
                    return;
            }
            RemplirRevuesListe(sortedList);
        }

        /// <summary>
        /// Gère les clics sur les boutons Modifier et Supprimer dans la grille des Revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            DataGridView dgv = (DataGridView)sender;
            string colName = dgv.Columns[e.ColumnIndex].Name;
            Revue revueSelectionne = (Revue)dgv.Rows[e.RowIndex].DataBoundItem;

            if (colName == "Modifier")
            {
                FrmDocEdit frmEdit = new FrmDocEdit(this.controller, revueSelectionne);
                if (frmEdit.ShowDialog() == DialogResult.OK)
                {
                    RefreshAllLists();
                }
            }
            else if (colName == "Supprimer")
            {
                DialogResult res = MessageBox.Show("Êtes vous sûr de vouloir supprimer " + revueSelectionne.Titre + "?", "Confirmation de suppression", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (res == DialogResult.Yes)
                {
                    bool result = controller.DeleteDocument(revueSelectionne.Id, "revue");
                    if (!result)
                    {
                        MessageBox.Show("Impossible de supprimer " + revueSelectionne.Titre + ", il est lié à une commande ou un exemplaire.", "Erreur : suppression impossible", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    RefreshAllLists();
                }
            }
        }

        #endregion

        #region Onglet Paarutions
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        const string ETATNEUF = "00001";

        /// <summary>
        /// Ouverture de l'onglet : récupère le revues et vide tous les champs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            txbReceptionRevueNumero.Text = "";
            pEtatRevue.Enabled = false;
            grpReceptionExemplaire.Enabled = false;
        }

        /// <summary>
        /// Remplit le dategrid des exemplaires avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires">liste d'exemplaires</param>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            bool userModif = _user.IdService != 3;
            if (exemplaires != null && exemplaires.Count > 0)
            {
                var afficherListe = exemplaires.Select(e => new
                {
                    Id = e.Id,
                    Numero = e.Numero,
                    IdEtat = e.IdEtat,
                    DateAchat = e.DateAchat.ToShortDateString(),
                    Etat = lesEtats.Find(et => et.Id == e.IdEtat).Libelle,
                    Photo = e.Photo
                }).ToList();

                bdgExemplairesListe.DataSource = afficherListe;
                dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;

                dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
                dgvReceptionExemplairesListe.Columns["id"].Visible = false;
                dgvReceptionExemplairesListe.Columns["Photo"].Visible = false;
                dgvReceptionExemplairesListe.Columns["Numero"].HeaderText = "N° exemplaire";
                dgvReceptionExemplairesListe.Columns["DateAchat"].HeaderText = "Date d'achat";

                AjouterBoutons(dgvReceptionExemplairesListe);
                HideColumn(dgvReceptionExemplairesListe, "Modifier");

                dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                pEtatRevue.Enabled = userModif;
            }
            else
            {
                bdgExemplairesListe.DataSource = null;
                dgvReceptionExemplairesListe.Rows.Clear();
                dgvReceptionExemplairesListe.Columns.Clear();
                pEtatRevue.Enabled = false;
            }
        }

        /// <summary>
        /// Gère la suppression d'un exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string colName = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].Name;

            if (colName == "Supprimer")
            {
                string id = dgvReceptionExemplairesListe.Rows[e.RowIndex].Cells["Id"].Value.ToString();
                string numero = dgvReceptionExemplairesListe.Rows[e.RowIndex].Cells["Numero"].Value.ToString();
                DialogResult res = MessageBox.Show("Êtes vous sûr de vouloir supprimer l'exemplaire n°" + numero + "?", "Confirmation de suppression", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (res == DialogResult.OK)
                {
                    bool result = controller.DeleteDocument(id, "exemplaire", numero);
                    if (!result)
                    {
                        MessageBox.Show("Impossible de supprimer l'exemplaire n°" + numero, "Erreur : suppression impossible", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    string idRevue = txbReceptionRevueNumero.Text;
                    lesExemplaires = controller.GetExemplaires(idRevue);
                    RemplirReceptionExemplairesListe(lesExemplaires);
                }
            }
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
            pEtatRevue.Enabled = false;
            cBEtatRevue.SelectedIndex = -1;
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
        }

        /// <summary>
        /// Récupère et affiche les exemplaires d'une revue
        /// </summary>
        private void AfficheReceptionExemplairesRevue()
        {
            string idDocument = txbReceptionRevueNumero.Text;
            lesExemplaires = controller.GetExemplaires(idDocument);
            RemplirReceptionExemplairesListe(lesExemplaires);
            AccesReceptionExemplaireGroupBox(true);            
        }

        /// <summary>
        /// Gère le clic pour enregistrer le changement d'état de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveEtatRevue_Click(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentRow == null || cBEtatRevue.SelectedIndex < 0)
            {
                MessageBox.Show("Sélectionnez un exemplaire et un état.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string id = dgvReceptionExemplairesListe.CurrentRow.Cells["Id"].Value.ToString();
            string numero = dgvReceptionExemplairesListe.CurrentRow.Cells["Numero"].Value.ToString();
            string idEtat = cBEtatRevue.SelectedValue.ToString();

            var obj = new { id = id, numero = numero, idEtat = idEtat };
            bool result = controller.addUpdateDocument("exemplaire", obj, false);

            if (!result)
            {
                MessageBox.Show("Échec de la mise à jour de l'état.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } 
            
            string idRevue = txbReceptionRevueNumero.Text;
            lesExemplaires = controller.GetExemplaires(idRevue);
            RemplirReceptionExemplairesListe(lesExemplaires);            
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            bool userModif = _user.IdService != 3;
            grpReceptionExemplaire.Enabled = acces && userModif;
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire à insérer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = txbReceptionRevueNumero.Text;
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);
                    if (controller.CreerExemplaire(exemplaire))
                    {
                        AfficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "N° exemplaire":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).ToList();
                    break;
                case "Date d'achat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).ToList();
                    break;
                case "Etat":
                    sortedList = lesExemplaires.OrderBy(o => o.IdEtat).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// affichage de l'image de l'exemplaire suite à la sélection d'un exemplaire dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentRow != null)
            {
                string img = dgvReceptionExemplairesListe.CurrentRow.Cells["Photo"].Value?.ToString();
                string idEtat = dgvReceptionExemplairesListe.CurrentRow.Cells["IdEtat"].Value?.ToString();
                pcbReceptionExemplaireRevueImage.Image = File.Exists(img) ? Image.FromFile(img) : null;
                cBEtatRevue.SelectedValue = idEtat;
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
                cBEtatRevue.SelectedIndex = -1;
            }
        }
        #endregion

        #region Onglet Commandes
        private readonly BindingSource bdgLivreCommande = new BindingSource();
        private readonly BindingSource bdgDvdCommande = new BindingSource();
        private readonly BindingSource bdgRevueCommande = new BindingSource();

        private List<Suivi> lesSuivis = new List<Suivi>();
        private List<AbonnementDto> lesAbonnements = new List<AbonnementDto>();
        private List<CommandeDto> lesCommandes = new List<CommandeDto>();

        private EventHandler _handlerChangeCommitted; // handler pour combo Suivi

        /// <summary>
        /// Supprime le bouton supprimer et grise la celulle pour les commandes déjà Livrée ou Réglée
        /// </summary>
        private void DisableDeleteButtons(DataGridView dgv)
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                Color color = Color.DarkGray;
                var idSuivi = row.Cells["IdSuivi"].Value.ToString();
                if (idSuivi == "00003" || idSuivi == "00004")
                {
                    row.Cells["Supprimer"] = new DataGridViewTextBoxCell { Value = "" };
                    row.Cells["Supprimer"].Style = new DataGridViewCellStyle
                    {
                        BackColor = color,
                        ForeColor = color,
                        SelectionBackColor = color,
                        SelectionForeColor = color
                    };
                    row.Cells["Supprimer"].ReadOnly = true;
                }
            }
        }

        /// <summary>
        /// Vérifie que tous les champs nécessaires sont renseignés
        /// Retourne true si ok sinon false
        /// </summary>
        private bool ValidateEntry(DateTime dateChoisie, string txtMontant, string txtNbEx)
        {
            if (!decimal.TryParse(txtMontant, out _)) // Si montant != nombre = false
            {
                MessageBox.Show("Le montant doit être un nombre valide.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtNbEx, out _))
            {
                MessageBox.Show("Le nombre d'exemplaires doit être un nombre valide.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }


            if (string.IsNullOrWhiteSpace(txtMontant))
            {
                MessageBox.Show("Le montant est obligatoire.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNbEx))
            {
                MessageBox.Show("Le nombre d'exemplaires est obligatoire.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dateChoisie > DateTime.Today)
            {
                MessageBox.Show("La date de commande ne peut pas être supérieure à aujourd'hui.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Vide le datagrid
        /// </summary>
        private void VideGridView(DataGridView dgv)
        {
            dgv.DataSource = null;
            dgv.Rows.Clear();
            dgv.Columns.Clear();
        }
        #endregion

        #region Onglet Commandes Livres
        private Livre livreRecherche;

        /// <summary>
        /// Clique sur Rechercher   
        /// récupère le livre et ses commandes puis remplit le datagrid
        /// Message d'erreur si rien n'est trouvé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRechercheLivre_click(object sender, EventArgs e)
        {
            string idLivre = txtBoxIdLivreCmd.Text.Trim();
            if (idLivre == "") return;

            livreRecherche = lesLivres.Find(x => x.Id.Equals(idLivre));
            if (livreRecherche == null)
            {
                MessageBox.Show("numéro introuvable", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                VideLivresInfosCmd();
                VideGridView(dgvCommandeLivre);
                groupBox3.Enabled = false;
                return;
            }
            AfficheLivresInfosCmd(livreRecherche);

            lesCommandes = controller.GetAllCompleteCommandes(idLivre);
            if (lesCommandes.Count == 0)
            {
                VideGridView(dgvCommandeLivre);
                groupBox3.Enabled = true;
                return;
            }
            groupBox3.Enabled = true;
            var commandeRecherche = lesCommandes.Where(c => c.IdLivreDvd == idLivre).ToList();
            remplirLivreCommande(commandeRecherche);
        }

        /// <summary>
        /// Se déclenche quand la touche entrer est pressée sur la recherche   
        /// récupère le livre et ses commandes puis remplit le datagrid
        /// Message d'erreur si rien n'est trouvé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBoxIdLivreCmd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true; // Supprime le son windows

                btnRechercheLivre_click(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Recharge le datagrid pour le livre saisi
        /// garde le tri et les règles d’affichage
        /// </summary>
        private void refreshCommandeList()
        {
            string idLivre;
            if (livreRecherche != null)
            {
                idLivre = livreRecherche.Id;
            }
            else
            {
                idLivre = txtBoxIdLivreCmd.Text.Trim();
            }
            lesCommandes = controller.GetAllCompleteCommandes(idLivre);
            if (lesCommandes.Count == 0) return;
            var commandeRecherche = lesCommandes.Where(c => c.IdLivreDvd == idLivre).ToList();
            remplirLivreCommande(commandeRecherche);
        }

        /// <summary>
        /// Affiche les infos du livre dans l'onglet Commandes Livres
        /// </summary>
        /// <param name="livre"></param>
        private void AfficheLivresInfosCmd(Livre livre)
        {
            txbLivresAuteurCmd.Text = livre.Auteur;
            txbLivresCollectionCmd.Text = livre.Collection;
            txbLivresImageCmd.Text = livre.Image;
            txbLivresIsbnCmd.Text = livre.Isbn;
            txbLivresGenreCmd.Text = livre.Genre;
            txbLivresPublicCmd.Text = livre.Public;
            txbLivresRayonCmd.Text = livre.Rayon;
            txbLivresTitreCmd.Text = livre.Titre;

            try
            {
                pcbLivresImageCmd.Image = Image.FromFile(livre.Image);
            }
            catch
            {
                pcbLivresImageCmd.Image = null;
            }
        }

        /// <summary>
        /// Vide toutes les informations du livre
        /// </summary>
        private void VideLivresInfosCmd()
        {
            txtBoxIdLivreCmd.Text = "";
            txbLivresAuteurCmd.Text = "";
            txbLivresCollectionCmd.Text = "";
            txbLivresImageCmd.Text = "";
            txbLivresIsbnCmd.Text = "";
            txbLivresGenreCmd.Text = "";
            txbLivresPublicCmd.Text = "";
            txbLivresRayonCmd.Text = "";
            txbLivresTitreCmd.Text = "";
            pcbLivresImageCmd.Image = null;
        }

        /// <summary>
        /// Affiche la liste des commandes dans la grille  
        /// ajoute colonne Suivi + boutons
        /// </summary>
        /// <param name="commande"></param>
        private void remplirLivreCommande(List<CommandeDto> commande)
        {
            lesCommandes = commande;
            bdgLivreCommande.DataSource = commande;
            dgvCommandeLivre.DataSource = bdgLivreCommande;
            dgvCommandeLivre.Columns["DateCommande"].HeaderText = "Date de commande";
            dgvCommandeLivre.Columns["NbExemplaire"].HeaderText = "Nombre d’exemplaires";

            // colonne combo Suivi
            if (!dgvCommandeLivre.Columns.Contains("Suivi"))
            {
                var colStatut = new DataGridViewComboBoxColumn
                {
                    Name = "Suivi",
                    HeaderText = "Statut",
                    DataPropertyName = "IdSuivi",
                    DisplayMember = "Libelle",
                    ValueMember = "Id",
                    DataSource = lesSuivis,
                    MinimumWidth = 80,
                    SortMode = DataGridViewColumnSortMode.Programmatic
                };
                dgvCommandeLivre.Columns.Add(colStatut);
            }

            AjouterBoutons(dgvCommandeLivre);
            dgvCommandeLivre.Columns["Libelle"].Visible = false;
            dgvCommandeLivre.Columns["IdLivreDvd"].Visible = false;
            dgvCommandeLivre.Columns["IdSuivi"].Visible = false;
            dgvCommandeLivre.Columns["Modifier"].Visible = false;

            // Tout en readonly sauf Suivi
            foreach (DataGridViewColumn col in dgvCommandeLivre.Columns)
            {
                col.ReadOnly = true;
            }
            dgvCommandeLivre.Columns["Suivi"].ReadOnly = false;

            // Bloque les commandes déja réglées
            foreach (DataGridViewRow row in dgvCommandeLivre.Rows)
            {
                var idSuivi = row.Cells["IdSuivi"].Value?.ToString();
                if (idSuivi == "00004")
                {
                    var cell = (DataGridViewComboBoxCell)row.Cells["Suivi"];
                    cell.ReadOnly = true;
                    cell.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing; // Retire la flèche du combo
                }
            }

            DisableDeleteButtons(dgvCommandeLivre);
            dgvCommandeLivre.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dateCmd.MaxDate = DateTime.Today;
        }

        /// <summary>
        /// Récupère la liste des statuts quand on arrive dans l’onglet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandeLivres_Enter(object sender, EventArgs e)
        {
            lesSuivis = controller.GetAllSuivis();
            groupBox3.Enabled = false;
            VideGridView(dgvCommandeLivre);
            VideLivresInfosCmd();
        }

        /// <summary>
        /// Gère la colonne suivi
        /// Construit la liste filtrée
        /// Ouvre le combo
        /// Appel l'API quand on clique sur un nouveau statut
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandeLivre_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (dgvCommandeLivre.Columns[e.ColumnIndex].Name != "Suivi") return;

            // Cellule édition pour créer le combo
            if (!dgvCommandeLivre.IsCurrentCellInEditMode)
            {
                dgvCommandeLivre.BeginEdit(true);
            }

            var cmb = dgvCommandeLivre.EditingControl as ComboBox;
            if (cmb == null) return;

            // Filtre la liste pour la ligne selectionnée
            CommandeDto commande = (CommandeDto)dgvCommandeLivre.Rows[e.RowIndex].DataBoundItem;
            string id = commande.IdSuivi;
            int ordre = lesSuivis.First(s => s.Id == id).Ordre;
            var liste = lesSuivis.Where(s => s.Id == id || (s.Ordre > ordre && !(s.Id == "00004" && ordre < 3))).ToList();

            // Lie le combo
            cmb.DataSource = null;
            cmb.DisplayMember = "Libelle";
            cmb.ValueMember = "Id";
            cmb.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb.DataSource = liste;
            cmb.SelectedValue = id;

            cmb.DroppedDown = true;

            // Retire l'ancien handler
            if (_handlerChangeCommitted != null)
            {
                cmb.SelectionChangeCommitted -= _handlerChangeCommitted;
            }

            _handlerChangeCommitted = (s, ev) =>
            {
                ComboBox cb = (ComboBox)s;
                string nouveau = cb.SelectedValue?.ToString();
                if (nouveau == null || nouveau == commande.IdSuivi) return;

                var payload = new { id = commande.Id, idSuivi = nouveau };

                dgvCommandeLivre.EndEdit();
                bool result = controller.addUpdateDocument("commandedocument", payload, false);
                refreshCommandeList();

                if (!result)
                {
                    MessageBox.Show("Mise à jour impossible.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            cmb.SelectionChangeCommitted += _handlerChangeCommitted;
        }


        /// <summary>
        /// Supprime une commande sauf si déja Livrée ou Réglée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandeLivre_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvCommandeLivre.Columns[e.ColumnIndex].Name != "Supprimer") return;

            CommandeDto commande = (CommandeDto)dgvCommandeLivre.Rows[e.RowIndex].DataBoundItem;


            if (commande.IdSuivi == "00003" || commande.IdSuivi == "00004") return;

            DialogResult res = MessageBox.Show($"Êtes vous sûr de vouloir supprimer la commande n°{commande.Id}?", "Confirmation de suppression", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (res == DialogResult.OK)
            {
                bool result = controller.DeleteDocument(commande.Id, "commandedocument");
                if (!result)
                {
                    MessageBox.Show($"Impossible de supprimer la commande n°{commande.Id}.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                refreshCommandeList();
            }
        }

        /// <summary>
        /// Crée une nouvelle commande pour le livre selectionné
        /// Passe par une validation des champs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveLivreCommand_click(object sender, EventArgs e)
        {
            if (!ValidateEntry(dateCmd.Value, txtCommandeMontant.Text, txtNbExemplaireCommande.Text)) return;

            var payload = (object)new
            {
                dateCommande = dateCmd.Value.Date,
                montant = txtCommandeMontant.Text,
                nbExemplaire = txtNbExemplaireCommande.Text,
                idLivreDvd = livreRecherche.Id
            };

            bool result = controller.addUpdateDocument("commandedocument", payload, true);
            if (!result)
            {
                MessageBox.Show($"Erreur lors de l'ajout de la commande.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            refreshCommandeList();

            // On met le focus sur la nouvelle commande            
            DateTime dCible = dateCmd.Value.Date;
            string mCible = txtCommandeMontant.Text.Trim();
            string nCible = txtNbExemplaireCommande.Text.Trim();

            foreach (DataGridViewRow row in dgvCommandeLivre.Rows)
            {
                DateTime d = Convert.ToDateTime(row.Cells["DateCommande"].Value);
                string m = row.Cells["Montant"].Value?.ToString().Trim();
                string nb = row.Cells["NbExemplaire"].Value?.ToString().Trim();

                if (d.Date == dCible && m == mCible && nb == nCible)
                {
                    dgvCommandeLivre.CurrentCell = row.Cells["Id"];
                    dgvCommandeLivre.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        /// <summary>
        /// Tri sur les colonnes du DataGridView commandes livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandeLivre_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string colonne = dgvCommandeLivre.Columns[e.ColumnIndex].Name;

            List<CommandeDto> sortedList = null;
            switch (colonne)
            {
                case "Id":
                    sortedList = lesCommandes.OrderBy(o => o.Id).ToList();
                    break;
                case "DateCommande":
                    sortedList = lesCommandes.OrderBy(o => o.DateCommande).ToList();
                    break;
                case "Montant":
                    sortedList = lesCommandes.OrderBy(o => o.Montant).ToList();
                    break;
                case "NbExemplaire":
                    sortedList = lesCommandes.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "Suivi":
                    sortedList = lesCommandes.OrderBy(o => o.IdSuivi).ToList();
                    break;
                default:
                    // Colonne non traitée : on sort sans rien faire
                    return;
            }
            remplirLivreCommande(sortedList);
        }

        /// <summary>
        /// Vide le datagrid et bloque l'ajout de commande si le texte change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBoxIdLivreCmd_TextChanged(object sender, EventArgs e)
        {
            groupBox3.Enabled = false;
        }

        #endregion

        #region Onglet Commandes DVDs
        private Dvd DvdRecherche;
        /// <summary>
        /// Se déclenche quand la touche entrer est pressée sur la recherche   
        /// récupère le DVD et ses commandes puis remplit le datagrid
        /// Message d'erreur si rien n'est trouvé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBoxIdDvdCmd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true; // Supprime le son windows

                btnRechercheDvd_Click(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Se déclenche au clic sur la recherche   
        /// récupère le DVD et ses commandes puis remplit le datagrid
        /// Message d'erreur si rien n'est trouvé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRechercheDvd_Click(object sender, EventArgs e)
        {
            string idDvd = txtBoxIdDvdCmd.Text.Trim();
            if (idDvd == "") return;

            DvdRecherche = lesDvd.Find(x => x.Id.Equals(idDvd));
            if (DvdRecherche == null)
            {
                MessageBox.Show("numéro introuvable", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                VideDvdInfosCmd();
                VideGridView(dgvCommandeDvd);
                groupBox6.Enabled = false;
                return;
            }
            AfficheDvdInfosCmd(DvdRecherche);

            lesCommandes = controller.GetAllCompleteCommandes(idDvd);
            if (lesCommandes.Count == 0)
            {
                VideGridView(dgvCommandeDvd);
                groupBox6.Enabled = true;
                return;
            }
            groupBox6.Enabled = true;
            var commandeRecherche = lesCommandes.Where(c => c.IdLivreDvd == idDvd).ToList();
            remplirDvdCommande(commandeRecherche);
        }

        /// <summary>
        /// Affiche les infos du Dvd dans l'onglet Commandes Dvd
        /// </summary>
        /// <param name="Dvd"></param>
        private void AfficheDvdInfosCmd(Dvd dvd)
        {
            txbDvdDureeCmd.Text = dvd.Duree.ToString();
            txbDvdRealCmd.Text = dvd.Realisateur;
            txbDvdImageCmd.Text = dvd.Image;
            txbDvdSynopsisCmd.Text = dvd.Synopsis;
            txbDvdGenreCmd.Text = dvd.Genre;
            txbDvdPublicCmd.Text = dvd.Public;
            txbDvdRayonCmd.Text = dvd.Rayon;
            txbDvdTitreCmd.Text = dvd.Titre;

            try
            {
                pcbDvdImageCmd.Image = Image.FromFile(dvd.Image);
            }
            catch
            {
                pcbDvdImageCmd.Image = null;
            }
        }

        /// <summary>
        /// Vide toutes les informations du Dvd
        /// </summary>
        private void VideDvdInfosCmd()
        {
            txtBoxIdDvdCmd.Text = "";
            txbDvdDureeCmd.Text = "";
            txbDvdRealCmd.Text = "";
            txbDvdImageCmd.Text = "";
            txbDvdSynopsisCmd.Text = "";
            txbDvdGenreCmd.Text = "";
            txbDvdPublicCmd.Text = "";
            txbDvdRayonCmd.Text = "";
            txbDvdTitreCmd.Text = "";
            pcbDvdImageCmd.Image = null;
        }

        /// <summary>
        /// Ouvre l'onglet commande DVD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandesDVD_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            VideGridView(dgvCommandeDvd);
            VideDvdInfosCmd();
            groupBox6.Enabled = false;
        }

        /// <summary>
        /// Affiche la liste des commandes du DVD dans la grille  
        /// ajoute colonne Suivi + boutons
        /// </summary>
        /// <param name="commande"></param>
        private void remplirDvdCommande(List<CommandeDto> commande)
        {
            lesCommandes = commande;
            bdgDvdCommande.DataSource = commande;
            dgvCommandeDvd.DataSource = bdgDvdCommande;
            dgvCommandeDvd.Columns["DateCommande"].HeaderText = "Date de commande";
            dgvCommandeDvd.Columns["NbExemplaire"].HeaderText = "Nombre d’exemplaires";

            // colonne combo Suivi
            if (!dgvCommandeDvd.Columns.Contains("Suivi"))
            {
                var colStatut = new DataGridViewComboBoxColumn
                {
                    Name = "Suivi",
                    HeaderText = "Statut",
                    DataPropertyName = "IdSuivi",
                    DisplayMember = "Libelle",
                    ValueMember = "Id",
                    DataSource = lesSuivis,
                    MinimumWidth = 80,
                    SortMode = DataGridViewColumnSortMode.Programmatic
                };
                dgvCommandeDvd.Columns.Add(colStatut);
            }

            AjouterBoutons(dgvCommandeDvd);
            dgvCommandeDvd.Columns["Libelle"].Visible = false;
            dgvCommandeDvd.Columns["IdLivreDvd"].Visible = false;
            dgvCommandeDvd.Columns["IdSuivi"].Visible = false;
            dgvCommandeDvd.Columns["Modifier"].Visible = false;

            // Tout en readonly sauf Suivi
            foreach (DataGridViewColumn col in dgvCommandeDvd.Columns)
            {
                col.ReadOnly = true;
            }
            dgvCommandeDvd.Columns["Suivi"].ReadOnly = false;

            // Bloque les commandes déja réglées
            foreach (DataGridViewRow row in dgvCommandeDvd.Rows)
            {
                var idSuivi = row.Cells["IdSuivi"].Value?.ToString();
                if (idSuivi == "00004")
                {
                    var cell = (DataGridViewComboBoxCell)row.Cells["Suivi"];
                    cell.ReadOnly = true;
                    cell.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing; // Retire la flèche du combo
                }
            }

            DisableDeleteButtons(dgvCommandeDvd);
            dgvCommandeDvd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dateCmdDvd.MaxDate = DateTime.Today;
        }

        /// <summary>
        /// Recharge le datagrid pour le Dvd saisi
        /// garde le tri et les règles d’affichage
        /// </summary>
        private void refreshDvdCommandeList()
        {
            string idDvd;
            if (DvdRecherche != null)
            {
                idDvd = DvdRecherche.Id;
            }
            else
            {
                idDvd = txtBoxIdDvdCmd.Text.Trim();
            }
            lesCommandes = controller.GetAllCompleteCommandes(idDvd);
            if (lesCommandes.Count == 0) return;
            var commandeRecherche = lesCommandes.Where(c => c.IdLivreDvd == idDvd).ToList();
            remplirDvdCommande(commandeRecherche);
        }

        /// <summary>
        /// Tri sur les colonnes du DataGridView commandes Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandeDvd_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var commandes = bdgDvdCommande.DataSource as List<CommandeDto>;
            if (lesCommandes == null || lesCommandes.Count == 0) return;

            List<CommandeDto> sortedList;
            string colonne = dgvCommandeDvd.Columns[e.ColumnIndex].Name;

            switch (colonne)
            {
                case "Id":
                    sortedList = commandes.OrderBy(o => o.Id).ToList();
                    break;
                case "DateCommande":
                    sortedList = commandes.OrderBy(o => o.DateCommande).ToList();
                    break;
                case "Montant":
                    sortedList = commandes.OrderBy(o => o.Montant).ToList();
                    break;
                case "NbExemplaire":
                    sortedList = commandes.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "Suivi":
                    sortedList = commandes.OrderBy(o => o.IdSuivi).ToList();
                    break;
                default:
                    return;
            }
            bdgDvdCommande.DataSource = null;
            dgvCommandeDvd.DataSource = null;

            remplirDvdCommande(sortedList);
        }

        /// <summary>
        /// Crée une nouvelle commande pour le Dvd selectionné
        /// Passe par une validation des champs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveDvdCommande_Click(object sender, EventArgs e)
        {
            if (!ValidateEntry(dateCmdDvd.Value, txtCommandeMontantDvd.Text, txtNbExemplaireCommandeDvd.Text)) return;

            var payload = (object)new
            {
                dateCommande = dateCmdDvd.Value.Date,
                montant = txtCommandeMontantDvd.Text,
                nbExemplaire = txtNbExemplaireCommandeDvd.Text,
                idLivreDvd = DvdRecherche.Id
            };

            bool result = controller.addUpdateDocument("commandedocument", payload, true);
            if (!result)
            {
                MessageBox.Show($"Erreur lors de l'ajout de la commande.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            refreshDvdCommandeList();

            // On met le focus sur la nouvelle commande                
            DateTime dateFocus = dateCmdDvd.Value.Date;
            string montantFocus = txtCommandeMontantDvd.Text.Trim();
            string nbexFocus = txtNbExemplaireCommandeDvd.Text.Trim();

            foreach (DataGridViewRow row in dgvCommandeDvd.Rows)
            {
                DateTime d = Convert.ToDateTime(row.Cells["DateCommande"].Value);
                string m = row.Cells["Montant"].Value?.ToString().Trim();
                string nb = row.Cells["NbExemplaire"].Value?.ToString().Trim();

                if (d.Date == dateFocus && m == montantFocus && nb == nbexFocus)
                {
                    dgvCommandeDvd.CurrentCell = row.Cells["Id"];
                    dgvCommandeDvd.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        /// <summary>
        /// Supprime une commande sauf si déja Livrée ou Réglée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandeDvd_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvCommandeDvd.Columns[e.ColumnIndex].Name != "Supprimer") return;

            CommandeDto commande = (CommandeDto)dgvCommandeDvd.Rows[e.RowIndex].DataBoundItem;


            if (commande.IdSuivi == "00003" || commande.IdSuivi == "00004") return;

            DialogResult res = MessageBox.Show($"Êtes vous sûr de vouloir supprimer la commande n°{commande.Id}?", "Confirmation de suppression", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (res == DialogResult.OK)
            {
                bool result = controller.DeleteDocument(commande.Id, "commandedocument");
                if (!result)
                {
                    MessageBox.Show($"Impossible de supprimer la commande n°{commande.Id}.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                refreshDvdCommandeList();
            }
        }

        /// <summary>
        /// Construit la liste filtrée
        /// Ouvre le combo
        /// Appel l'API quand on clique sur un nouveau statut
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandeDvd_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (dgvCommandeDvd.Columns[e.ColumnIndex].Name != "Suivi") return;

            // Cellule édition pour créer le combo
            if (!dgvCommandeDvd.IsCurrentCellInEditMode)
            {
                dgvCommandeDvd.BeginEdit(true);
            }

            var cmb = dgvCommandeDvd.EditingControl as ComboBox;
            if (cmb == null) return;

            // Filtre la liste pour la ligne selectionnée
            CommandeDto commande = (CommandeDto)dgvCommandeDvd.Rows[e.RowIndex].DataBoundItem;
            string id = commande.IdSuivi;
            int ordre = lesSuivis.First(s => s.Id == id).Ordre;
            var liste = lesSuivis.Where(s => s.Id == id || (s.Ordre > ordre && !(s.Id == "00004" && ordre < 3))).ToList();

            // Lie le combo
            cmb.DataSource = null;
            cmb.DisplayMember = "Libelle";
            cmb.ValueMember = "Id";
            cmb.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb.DataSource = liste;
            cmb.SelectedValue = id;

            cmb.DroppedDown = true;

            // Retire l'ancien handler
            if (_handlerChangeCommitted != null)
            {
                cmb.SelectionChangeCommitted -= _handlerChangeCommitted;
            }

            _handlerChangeCommitted = (s, ev) =>
            {
                ComboBox cb = (ComboBox)s;
                string nouveau = cb.SelectedValue?.ToString();
                if (nouveau == null || nouveau == commande.IdSuivi) return;

                var payload = new { id = commande.Id, idSuivi = nouveau };

                dgvCommandeDvd.EndEdit();
                bool result = controller.addUpdateDocument("commandedocument", payload, false);
                refreshDvdCommandeList();

                if (!result)
                {
                    MessageBox.Show("Mise à jour impossible.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            cmb.SelectionChangeCommitted += _handlerChangeCommitted;
        }




        #endregion

        #region Onglet Commandes Revues
        private Revue revueRecherche;
        /// <summary>
        /// Se déclenche quand la touche entrer est pressée sur la recherche   
        /// récupère la Revue et ses abonnements puis remplit le datagrid
        /// Message d'erreur si rien n'est trouvé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBoxIdRevueCmd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true; // Supprime le son windows

                btnRechercheRevue_Click(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Se déclenche au clic sur la recherche 
        /// récupère la revue et ses abonnements puis remplit le datagrid
        /// Message d'erreur si rien n'est trouvé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRechercheRevue_Click(object sender, EventArgs e)
        {
            string idRevue = txtBoxIdRevueCmd.Text.Trim();
            if (idRevue == "") return;

            revueRecherche = lesRevues.Find(x => x.Id.Equals(idRevue));
            if (revueRecherche == null)
            {
                MessageBox.Show("numéro introuvable", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                VideRevueInfosCmd();
                VideGridView(dgvCommandeRevue);
                groupBox8.Enabled = false;
                return;
            }
            AfficheRevueInfosCmd(revueRecherche);

            lesAbonnements = controller.GetAllAbonnements(revueRecherche.Id);
            if (lesAbonnements.Count == 0)
            {
                VideGridView(dgvCommandeRevue);
                groupBox8.Enabled = true;
                return;
            }
            groupBox8.Enabled = true;
            var commandeRecherche = lesAbonnements.Where(c => c.IdRevue == revueRecherche.Id).ToList();
            remplirRevueCommande(commandeRecherche);
        }

        /// <summary>
        /// Vide toutes les informations de la revue
        /// </summary>
        private void VideRevueInfosCmd()
        {
            txtBoxIdRevueCmd.Text = "";
            txbRevueDelaiCmd.Text = "";
            txbRevueImageCmd.Text = "";
            txbRevuePeriodCmd.Text = "";
            txbRevueGenreCmd.Text = "";
            txbRevuePublicCmd.Text = "";
            txbRevueRayonCmd.Text = "";
            txbRevueTitreCmd.Text = "";
            pcbRevueImageCmd.Image = null;
        }

        /// <summary>
        /// Valide l’entrée pour un abonnement de revue
        /// </summary>
        /// <param name="dateCommande"></param>
        /// <param name="dateFinAbonnement"></param>
        /// <param name="txtMontant"></param>
        /// <returns></returns>
        private bool ValidateRevueEntry(DateTime dateCommande, DateTime dateFinAbonnement, string txtMontant)
        {
            if (string.IsNullOrWhiteSpace(txtMontant))
            {
                MessageBox.Show("Le montant est obligatoire.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!decimal.TryParse(txtMontant, out var montant) || montant <= 0)
            {
                MessageBox.Show("Le montant doit être un nombre positif.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dateCommande > DateTime.Today)
            {
                MessageBox.Show("La date de commande ne peut pas être dans le futur.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (dateFinAbonnement <= dateCommande)
            {
                MessageBox.Show("La date de fin d’abonnement doit être postérieure à la date de commande.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Affiche les infos de la revue dans l'onglet Commandes Revue
        /// </summary>
        /// <param name="Dvd"></param>
        private void AfficheRevueInfosCmd(Revue revue)
        {
            txbRevueDelaiCmd.Text = revue.DelaiMiseADispo.ToString();
            txbRevueImageCmd.Text = revue.Image;
            txbRevuePeriodCmd.Text = revue.Periodicite;
            txbRevueGenreCmd.Text = revue.Genre;
            txbRevuePublicCmd.Text = revue.Public;
            txbRevueRayonCmd.Text = revue.Rayon;
            txbRevueTitreCmd.Text = revue.Titre;

            try
            {
                pcbRevueImageCmd.Image = Image.FromFile(revue.Image);
            }
            catch
            {
                pcbRevueImageCmd.Image = null;
            }
        }

        /// <summary>
        /// Affiche la liste des abonnements de la revue dans la grille  
        /// ajoute le bouton
        /// </summary>
        /// <param name="commande"></param>
        private void remplirRevueCommande(List<AbonnementDto> abonnement)
        {
            lesAbonnements = abonnement;
            bdgRevueCommande.DataSource = abonnement;
            dgvCommandeRevue.DataSource = bdgRevueCommande;
            dgvCommandeRevue.Columns["DateCommande"].HeaderText = "Date de commande";
            dgvCommandeRevue.Columns["Montant"].HeaderText = "Montant";
            dgvCommandeRevue.Columns["DateFinAbonnement"].HeaderText = "Date de fin d'abonnement";

            AjouterBoutons(dgvCommandeRevue);
            dgvCommandeRevue.Columns["Modifier"].Visible = false;
            dgvCommandeRevue.Columns["IdRevue"].Visible = false;


            dgvCommandeRevue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dateCmdRevue.Value = DateTime.Today;
            dateCmdRevue.MaxDate = DateTime.Today;
            dateCmdFinAbo.Value = dateCmdFinAbo.Value.AddMonths(1);
            dateCmdFinAbo.MinDate = DateTime.Today;
            DisableDeleteButtonsAbonnement();
        }

        /// <summary>
        /// Ouvre l'onglet commande Revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandesRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            VideGridView(dgvCommandeRevue);
            VideRevueInfosCmd();
            groupBox8.Enabled = false;
        }

        /// <summary>
        /// Tri sur les colonnes du datagrid revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandeRevue_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string colonne = dgvCommandeRevue.Columns[e.ColumnIndex].Name;
            List<AbonnementDto> sortedList = null;

            switch (colonne)
            {
                case "Id":
                    sortedList = lesAbonnements.OrderBy(a => a.Id).ToList();
                    break;
                case "DateCommande":
                    sortedList = lesAbonnements.OrderBy(a => a.DateCommande).ToList();
                    break;
                case "Montant":
                    sortedList = lesAbonnements.OrderBy(a => a.Montant).ToList();
                    break;
                case "DateFinAbonnement":
                    sortedList = lesAbonnements.OrderBy(a => a.DateFinAbonnement).ToList();
                    break;
                default:
                    return;
            }
            remplirRevueCommande(sortedList);
        }

        /// <summary>
        /// Recharge le datagrid pour la revue saisi
        /// garde le tri et les règles d’affichage
        /// </summary>
        private void refreshRevueCommandeList()
        {
            string idRevue;
            if (revueRecherche != null)
            {
                idRevue = revueRecherche.Id;
            }
            else
            {
                idRevue = txtBoxIdRevueCmd.Text.Trim();
            }
            lesAbonnements = controller.GetAllAbonnements(idRevue);
            if (lesAbonnements.Count == 0) return;
            var abonnementRecherche = lesAbonnements.Where(c => c.IdRevue == idRevue).ToList();
            remplirRevueCommande(abonnementRecherche);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveRevueCommande_Click(object sender, EventArgs e)
        {
            if (!ValidateRevueEntry(dateCmdRevue.Value, dateCmdFinAbo.Value, txtCommandeMontantRevue.Text)) return;

            var payload = (object)new
            {
                dateCommande = dateCmdRevue.Value.Date,
                montant = txtCommandeMontantRevue.Text,
                dateFinAbonnement = dateCmdFinAbo.Value,
                idRevue = revueRecherche.Id
            };

            bool result = controller.addUpdateDocument("abonnement", payload, true);
            if (!result)
            {
                MessageBox.Show($"Erreur lors de l'ajout de l'abonnement.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            refreshRevueCommandeList();

            // On met le focus sur la nouvelle commande                
            DateTime dateFocus = dateCmdRevue.Value.Date;
            string montantFocus = txtCommandeMontantRevue.Text.Trim();
            DateTime finFocus = dateCmdFinAbo.Value.Date;

            foreach (DataGridViewRow row in dgvCommandeRevue.Rows)
            {
                DateTime d = Convert.ToDateTime(row.Cells["DateCommande"].Value);
                string m = row.Cells["Montant"].Value?.ToString().Trim();
                DateTime f = Convert.ToDateTime(row.Cells["DateFinAbonnement"].Value).Date;

                if (d.Date == dateFocus && m == montantFocus && f == finFocus)
                {
                    dgvCommandeRevue.CurrentCell = row.Cells["DateFinAbonnement"];
                    dgvCommandeRevue.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        /// <summary>
        /// Supprime un abonnement sauf si un exemplaire y est rattaché
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandeRevue_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvCommandeRevue.Columns[e.ColumnIndex].Name != "Supprimer") return;

            AbonnementDto abonnement = (AbonnementDto)dgvCommandeRevue.Rows[e.RowIndex].DataBoundItem;
            List<Exemplaire> exemplaires = controller.GetExemplaires(abonnement.IdRevue);

            bool hasExemplaire = exemplaires.Exists(ex => AbonnementService.ParutionDansAbonnement(abonnement.DateCommande, abonnement.DateFinAbonnement, ex.DateAchat));

            if (hasExemplaire)
            {
                MessageBox.Show("Suppression impossible : un ou plusieurs exemplaires sont rattachés à cet abonnement.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show($"Êtes-vous sûr de vouloir supprimer l’abonnement n°{abonnement.Id}?", "Confirmation de suppression", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
            {
                return;
            }

            bool result = controller.DeleteDocument(abonnement.Id, "abonnement");

            if (!result)
            {
                MessageBox.Show($"Impossible de supprimer l’abonnement n°{abonnement.Id}.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            refreshRevueCommandeList();
        }

        /// <summary>
        /// Retire le bouton Supprimer et grise la cellule si l'abonnement à au moins un exemplaire
        /// </summary>
        private void DisableDeleteButtonsAbonnement()
        {
            Color gris = Color.DarkGray;

            foreach (DataGridViewRow row in dgvCommandeRevue.Rows)
            {
                AbonnementDto abo = (AbonnementDto)row.DataBoundItem;
                List<Exemplaire> exemplaires = controller.GetExemplaires(abo.IdRevue);

                bool hasExemplaire = exemplaires.Exists(ex => AbonnementService.ParutionDansAbonnement(abo.DateCommande, abo.DateFinAbonnement, ex.DateAchat));

                if (hasExemplaire)
                {
                    var cell = new DataGridViewTextBoxCell { Value = "" };
                    cell.Style = new DataGridViewCellStyle
                    {
                        BackColor = gris,
                        ForeColor = gris,
                        SelectionBackColor = gris,
                        SelectionForeColor = gris
                    };
                    row.Cells["Supprimer"] = cell;
                    row.Cells["Supprimer"].ReadOnly = true;
                }
            }
        }











        #endregion

        
    }

}