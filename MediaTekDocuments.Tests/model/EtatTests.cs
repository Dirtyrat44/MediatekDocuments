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
    /// Classe de test pour Etat
    /// </summary>
    [TestClass]
    public class EtatTests
    {
        /// <summary>
        /// Doit initialiser correctement toutes les propriétés de etat
        /// </summary>
        [TestMethod]
        public void Constructeur_AssigneProprietes()
        {
            Etat etat = new Etat("00001", "Neuf");

            Assert.AreEqual("00001", etat.Id);
            Assert.AreEqual("Neuf", etat.Libelle);
        }
    }
}
