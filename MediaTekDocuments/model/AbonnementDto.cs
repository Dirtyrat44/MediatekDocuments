using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe qui pour afficher les abonnements lié à une revue
    /// </summary>
    public class AbonnementDto
    {
        public string Id { get; set; }
        public DateTime DateCommande { get; set; }
        public decimal Montant { get; set; }
        public DateTime DateFinAbonnement { get; set; }
        public string IdRevue { get; set; }

        /// <summary>
        /// Constructeur de la classe AbonnementDto
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dateCommande"></param>
        /// <param name="montant"></param>
        /// <param name="dateFinAbonnement"></param>
        /// <param name="idRevue"></param>
        public AbonnementDto(string id, DateTime dateCommande, decimal montant, DateTime dateFinAbonnement, string idRevue)
        {
            Id = id;
            DateCommande = dateCommande;
            Montant = montant;
            DateFinAbonnement = dateFinAbonnement;
            IdRevue = idRevue;
        }
    }
}

