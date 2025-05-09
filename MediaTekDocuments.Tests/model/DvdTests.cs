using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocuments.Tests.model
{
    /// <summary>
    /// Classe de test pour Dvd
    /// </summary>
    [TestClass]
    public class DvdTests
    {
        /// <summary>
        /// Doit initialiser correctement toutes les propriétés du DVD
        /// </summary>
        [TestMethod]
        public void Constructeur_AssigneProprietes()
        {
            var dvd = new Dvd(
                id: "20001",
                titre: "Star Wars 5 L'empire contre‑attaque",
                image: "",
                duree: 124,
                realisateur: "Irvin Kershner",
                synopsis: "synopsis de test",
                idGenre: "10002", genre: "Science Fiction",
                idPublic: "00003", lePublic: "Tous publics",
                idRayon: "DF001", rayon: "DVD films"
            );

            // Document
            Assert.AreEqual("20001", dvd.Id);
            Assert.AreEqual("Star Wars 5 L'empire contre‑attaque", dvd.Titre);
            Assert.AreEqual("", dvd.Image);
            Assert.AreEqual("10002", dvd.IdGenre);
            Assert.AreEqual("Science Fiction", dvd.Genre);
            Assert.AreEqual("00003", dvd.IdPublic);
            Assert.AreEqual("Tous publics", dvd.Public);
            Assert.AreEqual("DF001", dvd.IdRayon);
            Assert.AreEqual("DVD films", dvd.Rayon);

            // Dvd
            Assert.AreEqual(124, dvd.Duree);
            Assert.AreEqual("Irvin Kershner", dvd.Realisateur);
            Assert.AreEqual("synopsis de test", dvd.Synopsis);
        }
    }
}

