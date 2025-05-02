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
    public partial class FrmDocEdit : Form
    {
        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private Document document;

        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        internal FrmDocEdit(FrmMediatekController controller)
        {            
            InitializeComponent();

            Enregistrer.Text = "Ajouter";
            this.controller = controller;
            this.Text = "Ajout d'un nouveau document";
        }

        /// <summary>
        ///  Constructeur: modification d'un document
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="document"></param>
        internal FrmDocEdit(FrmMediatekController controller, Document document)
            : this(controller)
        {
            this.document = document;
            this.Text = "Édition du document : " + document.Titre;
            Enregistrer.Text = "Enregistrer";
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
        ///  Chargement du formulaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmDocEdit_Load(object sender, EventArgs e)
        {            
            List<Categorie> listeGenres = controller.GetAllGenres();
            List<Categorie> listePublics = controller.GetAllPublics();
            List<Categorie> listeRayons = controller.GetAllRayons();            

            RemplirComboCategorie(listeGenres, bdgGenres, cbxGenre);
            RemplirComboCategorie(listePublics, bdgPublics, cbxPublic);
            RemplirComboCategorie(listeRayons, bdgRayons, cbxRayon);

            if (document != null)
            {
                // Champs générique document
                txtId.Text = document.Id;
                txtId.ReadOnly = true;
                cbxPublic.Text = document.Public;
                cbxRayon.Text = document.Rayon;
                cbxGenre.Text = document.Genre;
                txtTitre.Text = document.Titre;
                comboBoxType.Visible = false;
                lblType.Visible = false;                

                // Champs spécifique en fonction du type de doc
                switch (this.document)
                {
                    case Livre livre:                        
                        lblFirstSpecific.Text = "Code ISBN";
                        lblSecondSpecific.Text = "Auteur(e)";
                        lblThirdSpecific.Text = "Collection";
                        txtFirstSpecific.Text = livre.Isbn;
                        txtSecondSpecific.Text = livre.Auteur;
                        txtThirdSpecific.Text = livre.Collection;
                        break;
                    case Dvd dvd:
                        lblFirstSpecific.Text = "Duree";
                        lblSecondSpecific.Text = "Realisateur";
                        lblThirdSpecific.Text = "Synopsis";
                        txtFirstSpecific.Text = dvd.Duree.ToString();
                        txtSecondSpecific.Text = dvd.Realisateur;
                        txtThirdSpecific.Text = dvd.Synopsis;
                        break;
                    case Revue revue:
                        lblFirstSpecific.Text = "Périodicité";
                        lblSecondSpecific.Text = "Délai de mise à dispo";
                        lblThirdSpecific.Visible = false;
                        txtThirdSpecific.Visible = false;
                        txtFirstSpecific.Text = revue.Periodicite;
                        txtSecondSpecific.Text = revue.DelaiMiseADispo.ToString();
                        break;
                    default:
                        return;
                }
            }
            else
            {
                // Formulaire d'ajout
                comboBoxType.Items.AddRange(new[] { "Livre", "DVD", "Revue" });
                comboBoxType.SelectedIndex = 0;
                lblFirstSpecific.Text = "Code ISBN";
                lblSecondSpecific.Text = "Auteur(e)";
                lblThirdSpecific.Text = "Collection";
                txtId.ReadOnly = false;
            }
           
        }

        /// <summary>
        /// Ajuste les labels et champs spécifiques quand l'utilisateur change le type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string type = comboBoxType.SelectedItem?.ToString();

            switch (type)
            {
                case "Livre":
                    lblFirstSpecific.Text = "Code ISBN";
                    lblSecondSpecific.Text = "Auteur(e)";
                    lblThirdSpecific.Text = "Collection";
                    txtFirstSpecific.Text = "";
                    txtSecondSpecific.Text = "";
                    txtThirdSpecific.Text = "";
                    lblThirdSpecific.Visible = true;
                    txtThirdSpecific.Visible = true;
                    break;
                case "DVD":
                    lblFirstSpecific.Text = "Duree";
                    lblSecondSpecific.Text = "Realisateur";
                    lblThirdSpecific.Text = "Synopsis";
                    txtFirstSpecific.Text = "";
                    txtSecondSpecific.Text = "";
                    txtThirdSpecific.Text = "";
                    lblThirdSpecific.Visible = true;
                    txtThirdSpecific.Visible = true;
                    break;
                case "Revue":
                    lblFirstSpecific.Text = "Périodicité";
                    lblSecondSpecific.Text = "Délai de mise à dispo";
                    lblThirdSpecific.Visible = false;
                    txtThirdSpecific.Visible = false;
                    txtFirstSpecific.Text = "";
                    txtSecondSpecific.Text = "";
                    break;
                default:                    
                    return;
            }
        }
        /// <summary>
        /// Vérifie que tous les champs nécessaires sont renseignés
        /// Retourne true si ok sinon false
        /// </summary>
        private bool ValidateForm()
        {
            if (!int.TryParse(txtId.Text, out _)) // Si Id != nombre = false
            {
                MessageBox.Show("Le numéro du document doit être un nombre.", "Attention",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtTitre.Text))
            {
                MessageBox.Show("Le titre est obligatoire.", "Attention",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show("Le numéro de document est obligatoire.", "Attention",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Choix du type en création
            if (document == null && comboBoxType.SelectedIndex < 0)
            {
                MessageBox.Show("Choisissez un type de document.", "Attention",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cbxGenre.SelectedIndex < 0)
            {
                MessageBox.Show("Choisissez le genre.", "Attention",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (cbxRayon.SelectedIndex < 0)
            {
                MessageBox.Show("Choisissez le rayon.", "Attention",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
            }
            if (cbxPublic.SelectedIndex < 0)
            {
                MessageBox.Show("Choisissez le public.", "Attention",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            bool isDvd = (document == null && comboBoxType.SelectedItem?.ToString() == "DVD")
              || document is Dvd;
            if (isDvd)
            {
                if (!int.TryParse(txtFirstSpecific.Text, out _)) // Si duree != nombre = false
                {
                    MessageBox.Show("La durée doit être un nombre.", "Attention",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            bool isRevue = (document == null && comboBoxType.SelectedItem?.ToString() == "Revue")
                || document is Revue;
            if (isRevue)
            {
                if (!int.TryParse(txtSecondSpecific.Text, out _)) // Si delaiMiseADispo != nombre = false
                {
                    MessageBox.Show("Le délai de mise à disposition doit être un nombre.", "Attention",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Prépare le type et le payload pour l'enregistrement ou la modification d'un document
        /// </summary>
        /// <returns>Un tuple de type string : type  et un objet anonyme : payload</returns>
        private (string type, object payload) prepareSaveOrEdit()
        {
            bool isNew = (document == null);
            string type = isNew ? comboBoxType.SelectedItem.ToString().ToLower() : document.GetType().Name.ToLower();

            // Récupère les champs communs aux documents
            string id = txtId.Text;
            string titre = txtTitre.Text;
            string image = txtImage.Text;
            string idGenre = ((Categorie)cbxGenre.SelectedItem).Id;
            string idPublic = ((Categorie)cbxPublic.SelectedItem).Id;
            string idRayon = ((Categorie)cbxRayon.SelectedItem).Id;

            // Construit le payload pour matcher avec l'API
            switch (type)
            {
                case "livre":
                    return (type, (object)new
                    {
                        id,
                        titre,
                        image,
                        idGenre,
                        idPublic,
                        idRayon,
                        ISBN = txtFirstSpecific.Text,
                        auteur = txtSecondSpecific.Text,
                        collection = txtThirdSpecific.Text
                    });

                case "dvd":
                    return (type, (object)new
                    {
                        id,
                        titre,
                        image,
                        idGenre,
                        idPublic,
                        idRayon,
                        duree = int.Parse(txtFirstSpecific.Text),
                        realisateur = txtSecondSpecific.Text,
                        synopsis = txtThirdSpecific.Text
                    });

                case "revue":
                    return (type, (object)new
                    {
                        id,
                        titre,
                        image,
                        idGenre,
                        idPublic,
                        idRayon,
                        periodicite = txtFirstSpecific.Text,
                        delaiMiseADispo = int.Parse(txtSecondSpecific.Text)
                    });

                default:
                    throw new InvalidOperationException($"Type de document inconnu : {type}");
            }
        }

        /// <summary>
        /// Enregistre ou met à jour le document via l'API
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Enregistrer_Click(object sender, EventArgs e)
        {            
            if (!ValidateForm())
            { 
                return;
            }

            bool isNew = (document == null);

            // Prepare le type et le payload
            var documentInfo = prepareSaveOrEdit();
            string type = documentInfo.type;
            object payload = documentInfo.payload;

            // Appel API
            bool success = controller.addUpdateDocument(type, payload, isNew);

            if (!success)
            {
                MessageBox.Show(
                    isNew
                      ? "Échec de la création."
                      : "Échec de la mise à jour.",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Fermeture du forulaire sans enregistrement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
