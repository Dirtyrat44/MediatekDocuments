using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaTekDocuments.Tests.model
{
    /// <summary>
    /// Classe de test pour AbonnementAlerteDtoTests
    /// </summary>
    [TestClass]
    public class AbonnementAlerteDtoTests
    {
        /// <summary>
        /// Doit initialiser correctement toutes les propriétés de AbonnementAlerteDto
        /// </summary>
        [TestMethod]
        public void Constructeur_AssigneProprietes()
        {
            DateTime dateFin = new DateTime(2025, 12, 31, 0, 0, 0, DateTimeKind.Local);
            AbonnementAlerteDto dto = new AbonnementAlerteDto("10001", "Arts Magazine", dateFin);

            Assert.AreEqual("10001", dto.IdRevue);
            Assert.AreEqual("Arts Magazine", dto.TitreRevue);
            Assert.AreEqual(dateFin, dto.DateFinAbonnement);
        }
    }
}
