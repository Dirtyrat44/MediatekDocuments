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
    /// Classe de test pour Genre
    /// </summary>
    [TestClass]
    public class GenreTests
    {
        /// <summary>
        /// Doit initialiser correctement toutes les propriétés de genre
        /// </summary>
        [TestMethod]
        public void Constructeur_AssigneProprietes()
        {
            Genre genre = new Genre("10014", "Horreur");

            Assert.AreEqual("10014", genre.Id);
            Assert.AreEqual("Horreur", genre.Libelle);
        }

        /// <summary>
        /// Doit retourner le libelle
        /// </summary>
        [TestMethod]
        public void ToString_RetourneLibelle()
        {
            Genre genre = new Genre("10014", "Horreur");
            Assert.AreEqual("Horreur", genre.ToString());
        }
    }
}
