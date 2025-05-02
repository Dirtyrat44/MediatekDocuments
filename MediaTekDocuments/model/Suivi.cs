
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
