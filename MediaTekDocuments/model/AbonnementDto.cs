using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    public class AbonnementDto
    {
        public string Id { get; set; }
        public DateTime DateCommande { get; set; }
        public decimal Montant { get; set; }
        public DateTime DateFinAbonnement { get; set; }
        public string IdRevue { get; set; }

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

