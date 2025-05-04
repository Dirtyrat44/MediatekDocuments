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

        public AbonnementAlerteDto(string idRevue, string titreRevue, DateTime dateFin)
        {
            IdRevue = idRevue;
            TitreRevue = titreRevue;
            DateFinAbonnement = dateFin;
        }
    }
}
