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
    /// Class de test pour Livre
    /// </summary>
    [TestClass]
    public class LivreTests
    {
        /// <summary>
        /// Doit initialiser correctement toutes les propriétés du Livre
        /// </summary>
        [TestMethod]
        public void Constructeur_AssigneProprietes()
        {            
            Livre livre = new Livre(
                id: "00002",
                titre: "Un pays à l'aube",
                image: "",
                isbn: "978‑2264061921",
                auteur: "Dennis Lehane",
                collection: "Rivages/Thriller",
                idGenre: "10004", genre: "Historique",
                idPublic: "00002", lePublic: "Adultes",
                idRayon: "LV001", rayon: "Littérature étrangère"
            );
            
            // Document
            Assert.AreEqual("00002", livre.Id);
            Assert.AreEqual("Un pays à l'aube", livre.Titre);
            Assert.AreEqual("", livre.Image);
            Assert.AreEqual("10004", livre.IdGenre);
            Assert.AreEqual("Historique", livre.Genre);
            Assert.AreEqual("00002", livre.IdPublic);
            Assert.AreEqual("Adultes", livre.Public);
            Assert.AreEqual("LV001", livre.IdRayon);
            Assert.AreEqual("Littérature étrangère", livre.Rayon);

            // Livre
            Assert.AreEqual("978‑2264061921", livre.Isbn);
            Assert.AreEqual("Dennis Lehane", livre.Auteur);
            Assert.AreEqual("Rivages/Thriller", livre.Collection);
        }
    }
}
