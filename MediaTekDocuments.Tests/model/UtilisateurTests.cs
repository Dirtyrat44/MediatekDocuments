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
    /// Classe de test pour Utilisateur
    /// </summary>
    [TestClass]
    public class UtilisateurTests
    {
        /// <summary>
        /// Doit initialiser correctement toutes les propriétés de Utilisateur
        /// </summary>
        [TestMethod]
        public void Constructeur_AssigneProprietes()
        {
            Utilisateur user = new Utilisateur(0001, "login", "mdp", 0001);

            Assert.AreEqual(0001, user.IdUtilisateur);
            Assert.AreEqual("login", user.Login);
            Assert.AreEqual("mdp", user.MotDePasse);
            Assert.AreEqual(0001, user.IdService);            
        }
    }
}
