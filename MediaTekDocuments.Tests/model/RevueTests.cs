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
    /// Class de test pour Revue
    /// </summary>
    [TestClass]
    public class RevueTests
    {
        /// <summary>
        /// Doit initialiser correctement toutes les propriétés de la Revue
        /// </summary>
        [TestMethod]
        public void Constructeur_AssigneProprietes()
        {
            var revue = new Revue(
                id: "10001",
                titre: "Arts Magazine",
                image: "",
                idGenre: "10016", genre: "Presse Culturelle",
                idPublic: "00002", lePublic: "Adultes",
                idRayon: "PR002", rayon: "Magazines",
                periodicite: "MS",
                delaiMiseADispo: 5
            );

            // Document
            Assert.AreEqual("10001", revue.Id);
            Assert.AreEqual("Arts Magazine", revue.Titre);
            Assert.AreEqual("10016", revue.IdGenre);
            Assert.AreEqual("Presse Culturelle", revue.Genre);

            // Revue
            Assert.AreEqual("MS", revue.Periodicite);
            Assert.AreEqual(5, revue.DelaiMiseADispo);
        }
    }    
}
