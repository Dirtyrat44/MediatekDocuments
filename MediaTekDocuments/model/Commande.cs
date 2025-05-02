using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Commande (réunit les étapes de la commande)
    /// </summary>
    public class Commande
    {
        public string Id { get; }
        public DateTime DateCommande { get; }
        public decimal Montant { get; }

        public Commande(string id, DateTime dateCommande, decimal montant)
        { 
            Id = id;
            DateCommande = dateCommande;
            Montant = montant;
        }
    }
}

