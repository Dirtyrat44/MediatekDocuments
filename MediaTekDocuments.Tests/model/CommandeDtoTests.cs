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
    /// Classe de test pour CommandeDto
    /// </summary>
    [TestClass]
    public class CommandeDtoTests
    {
        /// <summary>
        /// Doit initialiser correctement toutes les propriétés de commandeDto
        /// </summary>
        [TestMethod]
        public void Constructeur_AssigneProprietes()
        {
            DateTime date = new DateTime(2025, 04, 10, 0, 0, 0, DateTimeKind.Local);
            CommandeDto dto = new CommandeDto("00005", date, 89.90m, 2, "00006", "2", "En cours");

            Assert.AreEqual("00005", dto.Id);
            Assert.AreEqual(date, dto.DateCommande);
            Assert.AreEqual(89.90m, dto.Montant);
            Assert.AreEqual(2, dto.NbExemplaire);
            Assert.AreEqual("00006", dto.IdLivreDvd);
            Assert.AreEqual("2", dto.IdSuivi);
            Assert.AreEqual("En cours", dto.Libelle);
        }
    }
}
