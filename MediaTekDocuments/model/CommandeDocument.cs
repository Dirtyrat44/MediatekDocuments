using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    public class CommandeDocument
    {
        public string Id { get; }
        public string IdLivreDvd { get; }
        public int NbExemplaire { get; }
        public string IdSuivi { get; }

        public CommandeDocument(string id, string idLivreDvd, int nbExemplaire, string idSuivi)
        {
            Id = id;
            IdLivreDvd = idLivreDvd;
            NbExemplaire = nbExemplaire;
            IdSuivi = idSuivi;
        }
    }
}
