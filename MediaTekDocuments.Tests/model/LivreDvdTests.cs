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
    /// Classe de test pour LivreDvd
    /// </summary>
    [TestClass]
    public class LivreDvdTests
    {
        /// <summary>
        /// On créer une fausse classe pour tester la classe abstraite LivreDvd
        /// </summary>
        private class FakeLivreDvd : LivreDvd
        {
            public FakeLivreDvd(string id, string titre, string image,
                                string idGenre, string genre,
                                string idPublic, string lePublic,
                                string idRayon, string rayon)
                : base(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon)
            {
            }
        }

        /// <summary>
        /// Doit initialiser correctement toutes les propriétés de LivreDvd
        /// </summary>
        [TestMethod]
        public void Constructeur_AssigneProprietes()
        {
            FakeLivreDvd fakeDvd = new FakeLivreDvd("LD01", "Titre test", "cover.jpg", "10002", "SF", "00003", "Tous publics", "DF001", "DVD films");

            Assert.AreEqual("LD01", fakeDvd.Id);
            Assert.AreEqual("Titre test", fakeDvd.Titre);
            Assert.AreEqual("cover.jpg", fakeDvd.Image);
            Assert.AreEqual("10002", fakeDvd.IdGenre);
            Assert.AreEqual("SF", fakeDvd.Genre);
            Assert.AreEqual("00003", fakeDvd.IdPublic);
            Assert.AreEqual("Tous publics", fakeDvd.Public);
            Assert.AreEqual("DF001", fakeDvd.IdRayon);
            Assert.AreEqual("DVD films", fakeDvd.Rayon);
        }
    }
}
