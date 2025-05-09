using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe qui représente une commande avec son état de suivi
    /// </summary>
    public class CommandeDto
    {
        public string Id { get; set; }
        public DateTime DateCommande { get; set; }
        public decimal Montant { get; set; }
        public int NbExemplaire { get; set; }
        public string IdLivreDvd { get; set; }
        public string IdSuivi { get; set; }
        public string Libelle { get; set; }

        /// <summary>
        /// Constructeur de la classe CommandeDto
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dateCommande"></param>
        /// <param name="montant"></param>
        /// <param name="nbExemplaire"></param>
        /// <param name="idLivreDvd"></param>
        /// <param name="idSuivi"></param>
        /// <param name="libelle"></param>
        public CommandeDto(string id, DateTime dateCommande, decimal montant, int nbExemplaire, string idLivreDvd, string idSuivi, string libelle)
        {
            Id = id;
            DateCommande = dateCommande;
            Montant = montant;
            NbExemplaire = nbExemplaire;
            IdLivreDvd = idLivreDvd;
            IdSuivi = idSuivi;
            Libelle = libelle;
        }
    }
}
