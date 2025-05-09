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
    /// Classe de test pour AbonnementDto
    /// </summary>
    [TestClass]
    public class AbonnementDtoTests
    {
        /// <summary>
        /// Doit initialiser correctement toutes les propriétés de AbonnementDto
        /// </summary>
        [TestMethod]
        public void Constructeur_AssigneProprietes()
        {
            DateTime cmd = new DateTime(2025, 05, 01, 0, 0, 0, DateTimeKind.Local);
            DateTime fin = new DateTime(2026, 05, 01, 0, 0, 0, DateTimeKind.Local);
            AbonnementDto abo = new AbonnementDto("00001", cmd, 120.50m, fin, "10005");

            Assert.AreEqual("00001", abo.Id);
            Assert.AreEqual(cmd, abo.DateCommande);
            Assert.AreEqual(120.50m, abo.Montant);
            Assert.AreEqual(fin, abo.DateFinAbonnement);
            Assert.AreEqual("10005", abo.IdRevue);
        }
    }
}
