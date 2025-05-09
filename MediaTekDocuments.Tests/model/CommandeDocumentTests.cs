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
    /// Classe de test pour commandedocument
    /// </summary>
    [TestClass]
    public class CommandeDocumentTests
    {
        /// <summary>
        /// Doit initialiser correctement toutes les propriétés de commandedocument
        /// </summary>
        [TestMethod]
        public void Constructeur_AssigneProprietes()
        {
            CommandeDocument cd = new CommandeDocument("00001", "10001", 3, "0001");

            Assert.AreEqual("00001", cd.Id);
            Assert.AreEqual("10001", cd.IdLivreDvd);
            Assert.AreEqual(3, cd.NbExemplaire);
            Assert.AreEqual("0001", cd.IdSuivi);
        }
    }
}
