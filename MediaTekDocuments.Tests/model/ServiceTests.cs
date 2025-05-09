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
    /// Classe de test pour Service
    /// </summary>
    [TestClass]
    public class ServiceTests
    {
        /// <summary>
        /// Doit initialiser correctement toutes les propriétés du service
        /// </summary>
        [TestMethod]
        public void Constructeur_AssigneProprietes()
        {
            Service service = new Service(5, "BTS");

            Assert.AreEqual(5, service.IdService);
            Assert.AreEqual("BTS", service.NomService);
        }
    }
}
