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
    /// Classe de test pour exemplaire
    /// </summary>
    [TestClass]
    public class ExemplaireTests
    {
        /// <summary>
        /// Doit initialiser correctement toutes les propriétés de exemplaire
        /// </summary>
        [TestMethod]
        public void Constructeur_AssigneProprietes()
        {
            DateTime dAchat = new DateTime(2025, 02, 20, 0, 0, 0, DateTimeKind.Local);
            Exemplaire ex = new Exemplaire(42, dAchat, "photo.jpg", "00001", "10001");

            Assert.AreEqual(42, ex.Numero);
            Assert.AreEqual(dAchat, ex.DateAchat);
            Assert.AreEqual("photo.jpg", ex.Photo);
            Assert.AreEqual("00001", ex.IdEtat);
            Assert.AreEqual("10001", ex.Id);
        }
    }
}
