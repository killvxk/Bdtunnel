// -----------------------------------------------------------------------------
// BoutDuTunnel
// Sebastien LEBRETON
// sebastien.lebreton[-at-]free.fr
// -----------------------------------------------------------------------------

#region " Inclusions "
using System;
using System.Globalization;
using Bdt.Shared.Resources;
using Bdt.Shared.Logs;
using Bdt.Shared.Configuration;
using Bdt.Shared.Protocol;
#endregion

namespace Bdt.Shared.Runtime
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Une ébauche de programme
    /// </summary>
    /// -----------------------------------------------------------------------------
    public abstract class Program : Bdt.Shared.Logs.LoggedObject
    {

        #region " Constantes "
        protected const string CFG_LOG = SharedConfig.WORD_LOGS + SharedConfig.TAG_ELEMENT;
        protected const string CFG_CONSOLE = CFG_LOG + SharedConfig.WORD_CONSOLE;
        protected const string CFG_FILE = CFG_LOG + SharedConfig.WORD_FILE;
        #endregion

        #region " Attributs "
        protected ConfigPackage m_config;
        protected GenericProtocol m_protocol;
        protected BaseLogger m_consoleLogger;
        protected FileLogger m_fileLogger;
        protected string[] m_args;
        #endregion

        #region " Proprietes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Le fichier de configuration
        /// </summary>
        /// -----------------------------------------------------------------------------
        public virtual string ConfigFile
        {
            get
            {
                return string.Format("{0}Cfg.xml", this.GetType().Assembly.GetName().Name);
            }
        }
        #endregion

        #region " Méthodes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Chargement des données de configuration
        /// </summary>
        /// -----------------------------------------------------------------------------
        public void LoadConfiguration()
        {
            LoadConfiguration(m_args);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Initialisation des loggers
        /// </summary>
        /// <returns>un MultiLogger lié à une source fichier et console</returns>
        /// -----------------------------------------------------------------------------
        public virtual BaseLogger CreateLoggers ()
        {
            StringConfig ldcConfig = new StringConfig(m_args, 0);
            XMLConfig xmlConfig = new XMLConfig(ConfigFile, 1);
            m_config = new ConfigPackage();
            m_config.AddSource(ldcConfig);
            m_config.AddSource(xmlConfig);

            MultiLogger log = new MultiLogger();
            m_consoleLogger = new ConsoleLogger(CFG_CONSOLE, m_config);
            m_fileLogger = new Bdt.Shared.Logs.FileLogger(CFG_FILE, m_config);
            log.AddLogger(m_consoleLogger);
            log.AddLogger(m_fileLogger);

            return log;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Chargement des données de configuration
        /// </summary>
        /// <param name="args">Arguments de la ligne de commande</param>
        /// -----------------------------------------------------------------------------
        public virtual void LoadConfiguration(string[] args)
        {
            m_args = args;

            LoggedObject.GlobalLogger = CreateLoggers();
            Log(Strings.LOADING_CONFIGURATION, ESeverity.DEBUG);
            SharedConfig cfg = new SharedConfig(m_config);
            m_protocol = GenericProtocol.GetInstance(cfg);
            SetCulture(cfg.ServiceCulture);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Fixe la culture courante
        /// </summary>
        /// <param name="name">le nom de la culture</param>
        /// -----------------------------------------------------------------------------
        public virtual void SetCulture(String name)
        {
            if ((name != null) && (name != String.Empty))
            {
                Bdt.Shared.Resources.Strings.Culture = new CultureInfo(name);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Déchargement des données de configuration
        /// </summary>
        /// -----------------------------------------------------------------------------
        public virtual void UnLoadConfiguration()
        {
            Log(Strings.UNLOADING_CONFIGURATION, ESeverity.DEBUG);

            m_consoleLogger.Close();
            m_fileLogger.Close();

            m_consoleLogger = null;
            m_fileLogger = null;

            LoggedObject.GlobalLogger = null;
            m_config = null;
            m_protocol = null;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Affiche le nom et la version du framework utilisé
        /// </summary>
        /// -----------------------------------------------------------------------------
        public static string FrameworkVersion()
        {
            string plateform = (Type.GetType("Mono.Runtime", false) == null) ? ".NET" : "Mono";
            return string.Format(Strings.POWERED_BY, plateform, System.Environment.Version);
        }

        /*
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Encodeur simple par Xor pour un tableau d'octets
        /// </summary>
        /// <param name="bytes">Le tableau à encoder/décoder (xor réversible)</param>
        /// <param name="seed">La racine d'initialisation du générateur aléatoire</param>
        /// -----------------------------------------------------------------------------
        public static void RandomXorEncoder (ref byte[] bytes, int seed)
        {
            Random rnd = new Random(seed);
            if (bytes!=null)
            {
                for (int i = 0; i <= bytes.Length - 1; i++)
                {
                    bytes[i] = (byte) (bytes[i] ^ Convert.ToByte(Math.Abs(rnd.Next() % 256)));
                }
            }
        }
        */

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Encodeur simple par Xor pour un tableau d'octets
        /// </summary>
        /// <param name="bytes">Le tableau à encoder/décoder (xor réversible)</param>
        /// <param name="key">La clef de codage</param>
        /// -----------------------------------------------------------------------------
        public static void StaticXorEncoder(ref byte[] bytes, int key)
        {
            if (bytes != null)
            {
                for (int i = 0; i <= bytes.Length - 1; i++)
                {
                    bytes[i] = (byte)(bytes[i] ^ Convert.ToByte(key % 256));
                    key++;
                }
            }
        }
        #endregion

    }

}

