
namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Suivi (réunit les étapes de la commande)
    /// </summary>
    public class Suivi
    {
        public string Id { get; }
        public string Libelle { get; }
        public byte Ordre { get; }

        /// <summary>
        /// Constructeur de la classe Suivi
        /// </summary>
        /// <param name="id"></param>
        /// <param name="libelle"></param>
        /// <param name="ordre"></param>
        public Suivi(string id, string libelle, byte ordre)
        {
            Id = id;
            Libelle = libelle;
            Ordre = ordre;
        }

        /// <summary>
        /// Récupération du libellé pour l'affichage dans le combo
        /// </summary>
        /// <returns>Libelle</returns>
        public override string ToString()
        {
            return this.Libelle;
        }
    }
}
