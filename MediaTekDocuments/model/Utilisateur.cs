using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe utilisateur
    /// </summary>
    public class Utilisateur
    {        
        public int IdUtilisateur { get; set; }
        
        public string Login { get; set; }
        
        public string MotDePasse { get; set; }
       
        public int IdService { get; set; }   
        
        /// <summary>
        /// Constructeur de la classe Utilisateur
        /// </summary>
        /// <param name="idUtilisateur"></param>
        /// <param name="login"></param>
        /// <param name="motDePasse"></param>
        /// <param name="idService"></param>
        public Utilisateur(int idUtilisateur, string login, string motDePasse, int idService)
        {
            IdUtilisateur = idUtilisateur;
            Login = login;
            MotDePasse = motDePasse;
            IdService = idService;
        }
    }
}

