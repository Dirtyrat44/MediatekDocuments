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
    /// Classe de test pour Public
    /// </summary>
    [TestClass]
    public class PublicTests
    {
        /// <summary>
        /// Doit initialiser correctement toutes les propriétés de
        /// </summary>
        [TestMethod]
        public void Constructeur_AssigneProprietes()
        {
            Public p = new Public("00003", "Tous publics");

            Assert.AreEqual("00003", p.Id);
            Assert.AreEqual("Tous publics", p.Libelle);
        }

        /// <summary>
        /// Doit retourner le libelle
        /// </summary>
        [TestMethod]
        public void ToString_RetourneLibelle()
        {
            Public p = new Public("00001", "Jeunesse");
            Assert.AreEqual("Jeunesse", p.ToString());
        }
    }
}
