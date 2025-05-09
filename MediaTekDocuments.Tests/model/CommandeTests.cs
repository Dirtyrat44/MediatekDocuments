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
    /// Classe de test pour Commande
    /// </summary>
    [TestClass]
    public class CommandeTests
    {
        /// <summary>
        /// Doit initialiser correctement toutes les propriétés de commande
        /// </summary>
        [TestMethod]
        public void Constructeur_AssigneProprietes()
        {
            DateTime date = new DateTime(2025, 03, 15, 0, 0, 0, DateTimeKind.Local);
            Commande cmd = new Commande("00001", date, 350.00m);

            Assert.AreEqual("00001", cmd.Id);
            Assert.AreEqual(date, cmd.DateCommande);
            Assert.AreEqual(350.00m, cmd.Montant);
        }
    }
}
