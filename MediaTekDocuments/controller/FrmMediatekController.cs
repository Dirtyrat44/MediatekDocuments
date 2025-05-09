using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;

namespace MediaTekDocuments.controller
{
    /// <summary>
    /// Contrôleur lié à FrmMediatek
    /// </summary>
    class FrmMediatekController
    {
        /// <summary>
        /// Objet d'accès aux données
        /// </summary>
        private readonly Access access;

        /// <summary>
        /// Récupération de l'instance unique d'accès aux données
        /// </summary>
        public FrmMediatekController()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// getter sur la liste des genres
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return access.GetAllGenres();
        }

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return access.GetAllLivres();
        }

        /// <summary>
        /// getter sur la liste des Dvd
        /// </summary>
        /// <returns>Liste d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return access.GetAllDvd();
        }

        /// <summary>
        /// getter sur la liste des revues
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return access.GetAllRevues();
        }

        /// <summary>
        /// getter sur les rayons
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return access.GetAllRayons();
        }

        /// <summary>
        /// getter sur suivis
        /// </summary>
        /// <returns>Liste d'objets Suivi</returns>
        public List<Suivi> GetAllSuivis()
        {
            return access.GetAllSuivis();
        }

        /// <summary>
        /// Getter sur les etats
        /// </summary>
        /// <returns>Liste d'objets Etat</returns>
        public List<Etat> getEtat()
        {
            return access.getEtat();
        }

        /// <summary>
        /// getter sur les publics
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return access.GetAllPublics();
        }

        /// <summary>
        /// getter sur la liste fusionnée de commande et commandedocument
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Liste d'objets CommandeDto</returns>
        public List<CommandeDto> GetAllCompleteCommandes(string id)
        {
            return access.GetAllCompleteCommandes(id);
        }

        /// <summary>
        /// getter sur la liste fusionnée de commande et abonnement
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Liste d'objets AbonnementDto</returns>
        public List<AbonnementDto> GetAllAbonnements(string id)
        {
            return access.GetAllAbonnements(id);
        }

        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocument">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplaires(string idDocument)
        {
            return access.GetExemplaires(idDocument);
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return access.CreerExemplaire(exemplaire);
        }

        /// <summary>
        /// Récupère un utilisateur par son login
        /// </summary>
        /// <param name="login">Identifiant de connexion</param>        
        /// <returns>Objet Utilisateur ou null si introuvable</returns>
        public Utilisateur getUser(string login)
        {
            return access.getUser(login);
        }

        /// <summary>
        /// Supprime une ressource selon son type, son identifiant et son numéro si c'est un exemplaire
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="ressource">Type de ressource</param>
        /// <param name="numero">Numéro de l'exemplaire</param>
        /// <returns>True si la suppression a réussi</returns>
        public bool DeleteDocument(string id, string ressource, string numero = null)
        {
            return access.DeleteDocument(id, ressource, numero);
        }

        /// <summary>
        /// Ajoute ou met à jour une ressource selon son type
        /// </summary>
        /// <param name="ressource">Type de ressource</param>
        /// <param name="payload">Données à insérer ou modifier</param>
        /// <param name="isNew">True pour création, false pour une MAJ</param>
        /// <returns>True si l’opération a réussi</returns>
        public bool addUpdateDocument(string ressource, object payload, bool isNew)
        {
            return access.addUpdateDocument(ressource, payload, isNew);
        }
    }
}
