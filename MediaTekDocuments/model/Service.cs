using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe représentant les départements
    /// </summary>
    public class Service
    {        
        public int IdService { get; set; }
        
        public string NomService { get; set; }

        public Service(int idService, string nomService)
        {
            IdService = idService;
            NomService = nomService;
        }
    }
}
