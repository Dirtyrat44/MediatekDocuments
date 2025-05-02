using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    public class CommandeDto
    {
        public string Id { get; set; }
        public DateTime DateCommande { get; set; }
        public decimal Montant { get; set; }
        public int NbExemplaire { get; set; }
        public string IdLivreDvd { get; set; }
        public string IdSuivi { get; set; }
        public string Libelle { get; set; }

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
