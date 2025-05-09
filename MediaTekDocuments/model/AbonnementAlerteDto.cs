using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe pour afficher les infos dans la fenêtre d’alerte
    /// </summary>
    public class AbonnementAlerteDto
    {
        public string IdRevue { get; set; }
        public string TitreRevue { get; set; }
        public DateTime DateFinAbonnement { get; set; }

        /// <summary>
        /// Constructeur de la classe AbonnementAlerteDto
        /// </summary>
        /// <param name="idRevue"></param>
        /// <param name="titreRevue"></param>
        /// <param name="dateFin"></param>
        public AbonnementAlerteDto(string idRevue, string titreRevue, DateTime dateFin)
        {
            IdRevue = idRevue;
            TitreRevue = titreRevue;
            DateFinAbonnement = dateFin;
        }
    }
}
