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
    /// Classe de test pour Rayon
    /// </summary>
    [TestClass]
    public class RayonTests
    {
        /// <summary>
        /// Doit initialiser correctement toutes les propriétés de Rayon
        /// </summary>
        [TestMethod]
        public void Constructeur_AssigneProprietes()
        {
            Rayon rayon = new Rayon("LV001", "Littérature étrangère");

            Assert.AreEqual("LV001", rayon.Id);
            Assert.AreEqual("Littérature étrangère", rayon.Libelle);
        }

        /// <summary>
        /// Doit renvoyer le libelle
        /// </summary>
        [TestMethod]
        public void ToString_RetourneLibelle()
        {
            Rayon rayon = new Rayon("DV005", "Voyages");

            Assert.AreEqual("Voyages", rayon.ToString());
        }
    }
}
