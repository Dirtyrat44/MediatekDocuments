using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.manager;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.Tests.manager
{

    /// <summary>
    /// Classe de tests pour AbonnementService
    /// </summary>
    [TestClass]
    public class AbonnementServiceTests
    {
        [TestMethod]
        public void ParutionDansAbonnement_DansIntervalle_ReturnTrue()
        {
            var dCommande = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local);
            var dFin = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Local);
            var dParution = new DateTime(2025, 3, 15, 0, 0, 0, DateTimeKind.Local);

            bool result = AbonnementService.ParutionDansAbonnement(dCommande, dFin, dParution);

            Assert.IsTrue(result, "Une parution en mars 2025 doit être valide dans l'intervalle.");
        }

        [TestMethod]
        public void ParutionDansAbonnement_AvantIntervalle_ReturnFalse()
        {
            var dCommande = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local);
            var dFin = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Local);
            var dParution = dCommande.AddDays(-1);

            bool result = AbonnementService.ParutionDansAbonnement(dCommande, dFin, dParution);

            Assert.IsFalse(result, "Une parution avant la date de commande doit être invalide.");
        }

        [TestMethod]
        public void ParutionDansAbonnement_ApresIntervalle_ReturnFalse()
        {
            var dCommande = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local);
            var dFin = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Local);
            var dParution = dFin.AddDays(1);

            bool result = AbonnementService.ParutionDansAbonnement(dCommande, dFin, dParution);

            Assert.IsFalse(result, "Une parution après la date de fin doit être invalide.");
        }
    }
}

