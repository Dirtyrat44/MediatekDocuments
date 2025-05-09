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
    /// Class de test pour document
    /// </summary>
    [TestClass]
    public class DocumentTests
    {
        /// <summary>
        ///  Doit initialiser correctement toutes les propriétés du document
        /// </summary>
        [TestMethod]
        public void Constructeur_AssigneProprietes()
        {            
            string id = "00001";
            string titre = "Les tests unitaires";
            string image = "cover.jpg";
            string idGenre = "10000";
            string genre = "Humour";
            string idPublic = "00002";
            string lePublic = "Adultes";
            string idRayon = "BD001";
            string rayon = "BD Adultes";
            
            Document doc = new Document(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon);
            
            Assert.AreEqual(id, doc.Id);
            Assert.AreEqual(titre, doc.Titre);
            Assert.AreEqual(image, doc.Image);
            Assert.AreEqual(idGenre, doc.IdGenre);
            Assert.AreEqual(genre, doc.Genre);
            Assert.AreEqual(idPublic, doc.IdPublic);
            Assert.AreEqual(lePublic, doc.Public);
            Assert.AreEqual(idRayon, doc.IdRayon);
            Assert.AreEqual(rayon, doc.Rayon);
        }

        /// <summary>
        /// Vérifie que le constructeur stocke bien n’importe quelle chaîne passée en paramètre        
        /// </summary>
        [DataTestMethod]
        [DataRow("00001", "", null, "10000", "Humour", "00002", "Adultes", "BD001", "BD Adultes")]
        [DataRow("00002", null, "", "", "", "", "", "", "")]
        [DataRow("", "Titre", "img", "10003", "Biographie", "00002", "Adultes", "LV002", "Littérature française")]
        public void Constructeur_ConserveChaines(string id, string titre, string image, string idGenre, string genre, string idPublic, string lePublic, string idRayon, string rayon)
        {            
            Document doc = new Document(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon);
           
            Assert.AreEqual(id, doc.Id);
            Assert.AreEqual(titre, doc.Titre);
            Assert.AreEqual(image, doc.Image);
            Assert.AreEqual(idGenre, doc.IdGenre);
            Assert.AreEqual(genre, doc.Genre);
            Assert.AreEqual(idPublic, doc.IdPublic);
            Assert.AreEqual(lePublic, doc.Public);
            Assert.AreEqual(idRayon, doc.IdRayon);
            Assert.AreEqual(rayon, doc.Rayon);
        }
    }
}
