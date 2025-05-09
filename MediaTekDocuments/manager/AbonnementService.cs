using System;
using System.Collections.Generic;
using System.Linq;
using MediaTekDocuments.model;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.manager
{
    /// <summary>
    /// Classe qui contient les méthodes pour les alertes et validité d'abonnement de revue
    /// </summary>
    public static class AbonnementService
    {
        /// <summary>
        /// Nombre de jours avant la fin d’un abonnement pour déclencher une alerte
        /// </summary>
        public const int JoursAlerte = 30;

        /// <summary>
        /// Retourne vrai si dateParution entre dateCommande et dateFinAbonnement
        /// </summary>
        /// <param name="dateCommande">Date de la commande</param>
        /// <param name="dateFinAbonnement">Date de fin de l’abonnement</param>
        /// <param name="dateParution">Date de parution</param>
        /// <returns>True si la parution est dans l’abonnement, sinon false</returns>
        public static bool ParutionDansAbonnement(DateTime dateCommande, DateTime dateFinAbonnement, DateTime dateParution)
        {
            return dateParution >= dateCommande && dateParution <= dateFinAbonnement;
        }

        /// <summary>
        /// Renvoie la liste des revues dont l'abonnement se termine dans moins de 30 jours
        /// </summary>
        /// <param name="revues">Collection de toutes les revues</param>
        /// <param name="aboParRevues">Fonction qui attend un id de revue et renvoie la liste de ses abonnements</param>
        /// <returns>Liste des abonnements de revues se terminant dans les 30 jours</returns>
        public static List<AbonnementAlerteDto> GetAlertesAbonnements(IEnumerable<Revue> revues, Func<string, List<AbonnementDto>> aboParRevues)
        {
            DateTime today = DateTime.Today;
            DateTime deadline = today.AddDays(JoursAlerte);

            // Récupération de la liste d'alertes abonnements
            var alertes = revues.SelectMany(
                r => aboParRevues(r.Id).Where(
                a => a.DateFinAbonnement >= today && a.DateFinAbonnement <= deadline).Select(
                a => new AbonnementAlerteDto(
                    r.Id,
                    r.Titre,
                    a.DateFinAbonnement
                ))
            ).OrderBy(a => a.DateFinAbonnement).ToList();

            return alertes;
        }
    }
}

