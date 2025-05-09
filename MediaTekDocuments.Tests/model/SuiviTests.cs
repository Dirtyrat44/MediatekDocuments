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
    /// Classe de test de Suivi
    /// </summary>
    [TestClass]
    public class SuiviTests
    {
        /// <summary>
        /// Doit initialiser correctement toutes les propriétés de suivi
        /// </summary>
        [TestMethod]
        public void Constructeur_AssigneProprietes()
        {
            Suivi suivi = new Suivi("0001", "En cours", 1);

            Assert.AreEqual("0001", suivi.Id);
            Assert.AreEqual("En cours", suivi.Libelle);
            Assert.AreEqual(1, suivi.Ordre);
        }

        /// <summary>
        /// Doit renvoyer le libelle
        /// </summary>
        [TestMethod]
        public void ToString_RetourneLibelle()
        {
            Suivi suivi = new Suivi("0005", "Renvoyé", 5);

            Assert.AreEqual("Renvoyé", suivi.ToString());
        }
    }
}
