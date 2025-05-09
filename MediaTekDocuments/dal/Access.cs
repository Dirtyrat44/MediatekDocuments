using System;
using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Linq;
using Serilog;
using System.IO;
using System.Diagnostics;

namespace MediaTekDocuments.dal
{
    /// <summary>
    /// Classe d'accès aux données
    /// </summary>
    public class Access
    {
        /// <summary>
        /// Configuration unique de Serilog
        /// </summary>
        static Access()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            // On remonte jusqu'à MediatekDocuments
            var projectDir = Path.GetFullPath(Path.Combine(baseDir, "..", ".."));
            var logDir = Path.Combine(projectDir, "logs");
            Directory.CreateDirectory(logDir);

            var logPath = Path.Combine(logDir, "mediatek_access-.log");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug().WriteTo.File(
                    path: logPath,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}").CreateLogger();
            Log.Information("Journalisation Serilog initialisée pour MediaTekDocuments.Access");
        }
        /// <summary>
        /// instance unique de la classe
        /// </summary>
        private static Access instance = null;
        /// <summary>
        /// instance de ApiRest pour envoyer des demandes vers l'api et recevoir la réponse
        /// </summary>
        private readonly ApiRest api = null;
        /// <summary>
        /// méthode HTTP pour select
        /// </summary>
        private const string GET = "GET";
        /// <summary>
        /// méthode HTTP pour insert
        /// </summary>
        private const string POST = "POST";
        /// <summary>
        /// méthode HTTP pour update
        /// </summary>
        private const string PUT = "PUT";
        /// <summary>
        /// méthode HTTP pour DELETE
        /// </summary>
        private const string DELETE = "DELETE";

        /// <summary>
        /// Méthode privée pour créer un singleton
        /// initialise l'accès à l'API
        /// </summary>
        private Access()
        {
            String authenticationString;
            try
            {
                string login = ConfigurationManager.AppSettings["ApiLogin"];
                string password = ConfigurationManager.AppSettings["ApiPassword"];
                string uriApi = ConfigurationManager.AppSettings["uriApi"];
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    throw new ConfigurationErrorsException("Login et password manquants");
                }

                authenticationString = $"{login}:{password}";
                api = ApiRest.GetInstance(uriApi, authenticationString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Log.Error(e, "Erreur lors de l'initialisation de l'accès à l'API");
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Création et retour de l'instance unique de la classe
        /// </summary>
        /// <returns>instance unique de la classe</returns>
        public static Access GetInstance()
        {
            if (instance == null)
            {
                instance = new Access();
            }
            return instance;
        }

        /// <summary>
        /// Retourne tous les genres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            IEnumerable<Genre> lesGenres = TraitementRecup<Genre>(GET, "genre", null);
            return new List<Categorie>(lesGenres);
        }

        /// <summary>
        /// Retourne tous les rayons à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            IEnumerable<Rayon> lesRayons = TraitementRecup<Rayon>(GET, "rayon", null);
            return new List<Categorie>(lesRayons);
        }

        /// <summary>
        /// Retourne toutes les catégories de public à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            IEnumerable<Public> lesPublics = TraitementRecup<Public>(GET, "public", null);
            return new List<Categorie>(lesPublics);
        }

        /// <summary>
        /// Retourne toutes les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            List<Livre> lesLivres = TraitementRecup<Livre>(GET, "livre", null);
            return lesLivres;
        }

        /// <summary>
        /// Retourne toutes les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            List<Dvd> lesDvd = TraitementRecup<Dvd>(GET, "dvd", null);
            return lesDvd;
        }

        /// <summary>
        /// Retourne toutes les revues à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            List<Revue> lesRevues = TraitementRecup<Revue>(GET, "revue", null);
            return lesRevues;
        }

        /// <summary>
        /// Retourne les différents status suivi
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Suivi> GetAllSuivis()
        {
            List<Suivi> lesSuivis = TraitementRecup<Suivi>(GET, "suivi", null);
            return lesSuivis;
        }

        /// <summary>
        /// Retourne les différents status etat
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Etat> getEtat()
        {
            List<Etat> lesEtats = TraitementRecup<Etat>(GET, "etat", null);
            return lesEtats;
        }

        /// <summary>
        /// Retourne la liste fusionnée de commande et commande document à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets commandeDto</returns>
        public List<CommandeDto> GetAllCompleteCommandes(string id)
        {
            String jsonId = convertToJson("id", id);
            List<CommandeDto> lesCommandes = TraitementRecup<CommandeDto>(GET, "commandedocument/" + jsonId, null);
            return lesCommandes;
        }

        /// <summary> 
        /// Retourne la liste fusionnée de commande et abonnement à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets commandeDto</returns>
        public List<AbonnementDto> GetAllAbonnements(string id)
        {
            String jsonId = convertToJson("id", id);
            List<AbonnementDto> lesAbonnements = TraitementRecup<AbonnementDto>(GET, "abonnement/" + jsonId, null);
            return lesAbonnements;
        }

        /// <summary>
        /// Récupère un utilisateur par son login
        /// </summary>
        /// <param name="login"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public Utilisateur getUser(string login)
        {
            String jsonBody = convertToJson("login", login);
            List<Utilisateur> users = TraitementRecup<Utilisateur>(GET, "utilisateur/" + jsonBody, null);
            Utilisateur user = users.FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Retourne les exemplaires
        /// </summary>
        /// <param name="idDocument">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplaires(string idDocument)
        {
            String jsonIdDocument = convertToJson("id", idDocument);
            List<Exemplaire> lesExemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + jsonIdDocument, null);
            return lesExemplaires;
        }

        /// <summary>
        /// ecriture d'un exemplaire en base de données
        /// </summary>
        /// <param name="exemplaire">exemplaire à insérer</param>
        /// <returns>true si l'insertion a pu se faire (retour != null)</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            String jsonExemplaire = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());
            try
            {
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(POST, "exemplaire", "champs=" + jsonExemplaire);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error(ex, "Erreur lors de la création de l'exemplaire : {Message}", ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Traitement de la récupération du retour de l'api, avec conversion du json en liste pour les select (GET)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methode">verbe HTTP (GET, POST, PUT, DELETE)</param>
        /// <param name="message">information envoyée dans l'url</param>
        /// <param name="parametres">paramètres à envoyer dans le body, au format "chp1=val1&chp2=val2&..."</param>
        /// <returns>liste d'objets récupérés (ou liste vide)</returns>
        private List<T> TraitementRecup<T>(String methode, String message, String parametres)
        {
            // trans
            List<T> liste = new List<T>();
            try
            {
                JObject retour = api.RecupDistant(methode, message, parametres);
                // extraction du code retourné
                String code = (String)retour["code"];
                if (code.Equals("200"))
                {
                    // dans le cas du GET (select), récupération de la liste d'objets
                    if (methode.Equals(GET))
                    {
                        String resultString = JsonConvert.SerializeObject(retour["result"]);
                        // construction de la liste d'objets à partir du retour de l'api
                        liste = JsonConvert.DeserializeObject<List<T>>(resultString, new CustomBooleanJsonConverter());
                    }
                }
                else
                {
                    Console.WriteLine("code erreur = " + code + " message = " + (String)retour["message"]);
                    Log.Warning("API renvoie code={Code} message={Msg}", code, (String)retour["message"]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur lors de l'accès à l'API : " + e.Message);
                Log.Error(e, "Erreur lors de l'accès distant à l'API");
                Environment.Exit(0);
            }
            return liste;
        }

        /// <summary>
        /// Convertit en json un couple nom/valeur
        /// </summary>
        /// <param name="nom"></param>
        /// <param name="valeur"></param>
        /// <returns>couple au format json</returns>
        private String convertToJson(Object nom, Object valeur)
        {
            Dictionary<Object, Object> dictionary = new Dictionary<Object, Object>();
            dictionary.Add(nom, valeur);
            return JsonConvert.SerializeObject(dictionary);
        }

        /// <summary>
        /// Modification du convertisseur Json pour gérer le format de date
        /// </summary>
        private sealed class CustomDateTimeConverter : IsoDateTimeConverter
        {
            public CustomDateTimeConverter()
            {
                base.DateTimeFormat = "yyyy-MM-dd";
            }
        }

        /// <summary>
        /// Modification du convertisseur Json pour prendre en compte les booléens
        /// classe trouvée sur le site :
        /// https://www.thecodebuzz.com/newtonsoft-jsonreaderexception-could-not-convert-string-to-boolean/
        /// </summary>
        private sealed class CustomBooleanJsonConverter : JsonConverter<bool>
        {
            public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return Convert.ToBoolean(reader.ValueType == typeof(string) ? Convert.ToByte(reader.Value) : reader.Value);
            }

            public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }

        /// <summary>
        /// Ajoute ou modifie un document
        /// </summary>
        /// <param name="doc">L'objet (Livre, dvd, revue)</param>
        /// <param name="isNew">True = POST, false = PUT</param>
        /// <returns>Code 200 si true</returns>
        public bool addUpdateDocument(string ressource, object doc, bool isNew)
        {
            string json = JsonConvert.SerializeObject(doc);
            string parameters = "champs=" + json;
            string methode = isNew ? POST : PUT;

            try
            {
                JObject retour = api.RecupDistant(methode, ressource, parameters);
                string code = (string)retour["code"];
                return code == "200";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Log.Error(e, "Erreur dans addUpdateDocument pour ressource={Ressource}", ressource);
                return false;
            }
        }

        /// <summary>
        /// Supprime un document dans la BDD via l'API
        /// Envoie une requête DELETE avec le champ "id" dans le body 
        /// True si la suppression marche sinon false
        /// </summary>
        /// <param name="id">Identifiant du document à supprimer</param>
        /// <param name="document"></param>
        /// <returns>True si la suppression marche sinon false</returns>
        public bool DeleteDocument(string id, string document, string numero = null)
        {
            try
            {
                object body;

                if (numero == null)
                {
                    body = new { id = id };
                }
                else // Exemplaire
                {
                    body = new { id = id, numero = numero };
                }

                string json = JsonConvert.SerializeObject(body);
                string parametres = "champs=" + json;

                JObject retour = api.RecupDistant(DELETE, document, parametres);

                string code = (string)retour["code"];
                return code == "200";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Log.Error(e, "Erreur dans DeleteDocument id={Id} doc={Doc} num={Numero}", id, document, numero);
                return false;
            }
        }
    }
}
