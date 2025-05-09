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
    /// Classe de test de Categorie
    /// </summary>
    [TestClass]
    public class CategorieTests
    {
        /// <summary>
        ///  Doit initialiser correctement toutes les propriétés de catégorie
        /// </summary>
        [TestMethod]
        public void Constructeur_AssigneProprietes()
        {
            Categorie categorie = new Categorie("I001", "Informatique");

            Assert.AreEqual("I001", categorie.Id);
            Assert.AreEqual("Informatique", categorie.Libelle);
        }

        /// <summary>
        /// Doit renvoyer le libelle
        /// </summary>
        [TestMethod]
        public void ToString_RetourneLibelle()
        {
            Categorie categorie = new Categorie("I001", "Informatique");

            Assert.AreEqual("Informatique", categorie.ToString());
        }
    }
}
