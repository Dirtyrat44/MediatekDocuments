using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier CommandeDocument (Livre ou Dvd)
    /// </summary>
    public class CommandeDocument
    {
        public string Id { get; }
        public string IdLivreDvd { get; }
        public int NbExemplaire { get; }
        public string IdSuivi { get; }

        /// <summary>
        /// Constructeur de la classe CommandeDocument
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idLivreDvd"></param>
        /// <param name="nbExemplaire"></param>
        /// <param name="idSuivi"></param>
        public CommandeDocument(string id, string idLivreDvd, int nbExemplaire, string idSuivi)
        {
            Id = id;
            IdLivreDvd = idLivreDvd;
            NbExemplaire = nbExemplaire;
            IdSuivi = idSuivi;
        }
    }
}
