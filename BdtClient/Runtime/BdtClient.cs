// -----------------------------------------------------------------------------
// BoutDuTunnel
// Sebastien LEBRETON
// sebastien.lebreton[-at-]free.fr
// -----------------------------------------------------------------------------

#region " Inclusions "
using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Globalization;

using Bdt.Client.Configuration;
using Bdt.Client.Resources;
using Bdt.Client.Sockets;
using Bdt.Client.Socks;
using Bdt.Shared.Runtime;
using Bdt.Shared.Service;
using Bdt.Shared.Protocol;
using Bdt.Shared.Logs;
using Bdt.Shared.Request;
using Bdt.Shared.Response;
#endregion

namespace Bdt.Client.Runtime
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Programme côté client du tunnel de communication
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class BdtClient : Program
    {

        #region " Attributs "
        // Les forwards de ports et socks, indexés par port local, en valeur le TcpServer associé au port
        protected Dictionary<int, Client.Sockets.TcpServer> m_servers = new Dictionary<int, Client.Sockets.TcpServer>();
        protected ClientConfig m_clientConfig;
        #endregion

        #region " Proprietes "
        public ClientConfig ClientConfig
        {
            get {
                return m_clientConfig;
            }
            set {
                m_clientConfig = value;
            }
        }
        #endregion

        #region " Méthodes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Point d'entrée du programme BdtClient
        /// </summary>
        /// <param name="args">les arguments de la ligne de commande</param>
        /// -----------------------------------------------------------------------------
        public static void Main(string[] args)
        {
            new BdtClient().Run(args);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Mise en place des ports à l'écoute pour les forwards via le tunnel
        /// </summary>
        /// <param name="tunnel">le tunnel crée avec le serveur</param>
        /// <param name="uid">le user id courant</param>
        /// -----------------------------------------------------------------------------
        protected void InitializeForwards(ITunnel tunnel, int uid)
        {
            foreach (PortForward forward in m_clientConfig.Forwards.Values)
            {
                if (forward.Enabled)
                {
                    int remotePort = forward.RemotePort;
                    int localPort = forward.LocalPort;
                    bool shared = forward.Shared;
                    string address = forward.Address;
                    if (m_servers.ContainsKey(localPort))
                    {
                        Log(string.Format(Strings.FORWARD_CANCELED, localPort, address, remotePort), ESeverity.WARN);
                    }
                    else
                    {
                        Bdt.Client.Sockets.GatewayServer server = new Bdt.Client.Sockets.GatewayServer(localPort, @shared, remotePort, address, tunnel, uid);
                        m_servers.Add(localPort, server);
                    }
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Mise en place du serveur Socks
        /// </summary>
        /// <param name="tunnel">le tunnel crée avec le serveur</param>
        /// <param name="uid">le user id courant</param>
        /// -----------------------------------------------------------------------------
        protected void InitializeSocks(ITunnel tunnel, int uid)
        {
            if (m_clientConfig.SocksEnabled)
            {
                int port = m_clientConfig.SocksPort;
                if (m_servers.ContainsKey(port))
                {
                    Log(string.Format(Strings.SOCKS_SERVER_DISABLED, port), ESeverity.WARN);
                }
                else
                {
                    Bdt.Client.Socks.SocksServer socks = new Bdt.Client.Socks.SocksServer(port, m_clientConfig.SocksShared, tunnel, uid);
                    m_servers.Add(port, socks);
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Destruction des ports à l'écoute pour les forwards via le tunnel
        /// </summary>
        /// -----------------------------------------------------------------------------
        protected void DisposeServers()
        {
            foreach (Client.Sockets.TcpServer server in m_servers.Values)
            {
                server.CloseServer();
            }
            m_servers.Clear();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Configuration du proxy http
        /// </summary>
        /// <param name="protocol">le protocole à configurer</param>
        /// -----------------------------------------------------------------------------
        protected void ConfigureProxy (GenericProtocol protocol)
        {
            if ((protocol) is IProxyCompatible)
            {
                IProxyCompatible proxyProtocol = ((GenericHttpRemoting)protocol);
                IWebProxy proxy;

                if (m_clientConfig.ProxyEnabled)
                {
                    // Configuration
                    if (m_clientConfig.ProxyAutoConfiguration)
                    {
#pragma warning disable 618
                        proxy = GlobalProxySelection.Select;
#pragma warning restore 618
                    }
                    else
                    {
                        proxy = new WebProxy(m_clientConfig.ProxyAddress, m_clientConfig.ProxyPort);
                    }

                    if (proxy != null)
                    {
                        // Authentification
                        if (m_clientConfig.ProxyAutoAuthentication)
                        {
                            proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                        }
                        else
                        {
                            NetworkCredential netCreds = new NetworkCredential();
                            netCreds.UserName = m_clientConfig.ProxyUserName;
                            netCreds.Password = m_clientConfig.ProxyPassword;
                            netCreds.Domain = m_clientConfig.ProxyDomain;
                            proxy.Credentials = netCreds;
                        }
                    }
                }
                else
                {
#pragma warning disable 618
                    proxy = GlobalProxySelection.GetEmptyWebProxy();
#pragma warning restore 618
                }

                if ((proxy) is WebProxy && (((WebProxy)proxy).Address == null))
                {
#pragma warning disable 618
                    proxy = GlobalProxySelection.GetEmptyWebProxy();
#pragma warning restore 618
                }

                proxyProtocol.Proxy = proxy;

                if ((proxy != null) && (proxy) is WebProxy)
                {
                    Log(string.Format(Strings.USING_PROXY, ((WebProxy)proxy).Address, proxy), ESeverity.INFO);
                }
                else
                {
                    Log(Strings.NOT_USING_PROXY, ESeverity.INFO);
                }
            }

        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Saisie d'une information sur stdin
        /// </summary>
        /// <param name="msg">le message à afficher</param>
        /// <param name="hide">pour masquer l'echo sur stdout (ex: mot de passe)</param>
        /// <returns>le chaine saisie</returns>
        /// -----------------------------------------------------------------------------
        protected string InputString(string msg, bool hide)
        {
            ConsoleKeyInfo cki;
            Console.Write(string.Format("INPUT {0}", msg));
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            Console.TreatControlCAsInput = false;
            StringBuilder result = new StringBuilder();
           
            do
            {
                cki = Console.ReadKey(true);
                switch(cki.Key) {
                    case ConsoleKey.Backspace:
                        if (result.Length > 0)
                        {
                            Console.SetCursorPosition(left + result.Length - 1, top);
                            Console.Write(" ");
                            result.Remove(result.Length - 1, 1);
                        }
                        Console.SetCursorPosition(left + result.Length, top);
                        break;
                    default:
                        if (Char.IsLetterOrDigit(cki.KeyChar) && (result.Length < 32))
                        {
                            result.Append(cki.KeyChar);
                            Console.Write((hide) ? '*' : cki.KeyChar);
                        }
                        break;
                }
            } while (cki.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return result.ToString();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Saisie des informations d'authentification sur le proxy
        /// </summary>
        /// <param name="proxyProtocol">le protocol IProxyCompatible à alterer</param>
        /// <param name="retry">pour permettre les essais multiples</param>
        /// -----------------------------------------------------------------------------
        protected virtual void InputProxyCredentials (IProxyCompatible proxyProtocol, ref bool retry)
        {
            Log(Strings.PROXY_AUTH_REQUESTED, ESeverity.INFO);
            string username = InputString(Strings.INPUT_PROXY_USERNAME, false);
            string password = InputString(Strings.INPUT_PROXY_PASSWORD, true);
            string domain = InputString(Strings.INPUT_PROXY_DOMAIN, false);

            proxyProtocol.Proxy.Credentials = new NetworkCredential(username, password, domain);
            retry = true;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Démarrage du client
        /// </summary>
        /// -----------------------------------------------------------------------------
        public void StartClient()
        {
            Log(string.Format(Strings.CLIENT_TITLE, this.GetType().Assembly.GetName().Version.ToString(3)), ESeverity.INFO);
            Log(FrameworkVersion(), ESeverity.INFO);

            m_protocol.ConfigureClient();
            ConfigureProxy(m_protocol);

            ITunnel tunnel = null;
            IGenericResponse response = null;
            bool retry;

            // Communication avec le tunnel. Détection éventuelle d'une authentification incorrecte proxy
            do
            {
                retry = false;
                try
                {
                    tunnel = m_protocol.GetTunnel();
                    response = tunnel.Version();
                }
                catch (WebException ex)
                {
                    if ((ex.Response) is HttpWebResponse && (m_protocol is IProxyCompatible))
                    {
                        IProxyCompatible proxyProtocol = (IProxyCompatible)m_protocol;
                        HttpWebResponse httpres = (HttpWebResponse)ex.Response;
                        if (httpres.StatusCode == HttpStatusCode.ProxyAuthenticationRequired)
                        {
                            retry = true;
                            InputProxyCredentials(proxyProtocol, ref retry);
                        }
                    }
                    else
                    {
                        throw (ex);
                    }
                }
            } while (retry);

            // Démarrage effectif du client, récuperation de l'user-id
            if ((response != null) && (tunnel != null))
            {
                if (response.Message.IndexOf(this.GetType().Assembly.GetName().Version.ToString(3)) < 0)
                {
                    Log(Strings.VERSION_MISMATCH, ESeverity.WARN);
                }
                Log(response.Message, ESeverity.INFO);
                if (response.Success)
                {
                    AuthenticateResponse authResponse = tunnel.Authenticate(new AuthenticateRequest(m_clientConfig.ServiceUserName, m_clientConfig.ServicePassword));
                    if (authResponse.Success)
                    {
                        Log(authResponse.Message, ESeverity.INFO);

                        // Mise En place des forwards
                        InitializeForwards(tunnel, authResponse.Uid);
                        // Puis du serveur Socks
                        InitializeSocks(tunnel, authResponse.Uid);
                    }
                    else {
                        throw new Exception(authResponse.Message);
                    }
                }
            }
            else
            {
                throw new Exception(Strings.CONNECTION_FAILED);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Arrêt du client
        /// </summary>
        /// -----------------------------------------------------------------------------
        public void StopClient()
        {
            // Nettoyage des serveurs de forwards et socks
            DisposeServers();

            // Nettoyage du tunnel
            m_protocol.UnConfigureClient();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Chargement des données de configuration
        /// </summary>
        /// <param name="args">Arguments de la ligne de commande</param>
        /// -----------------------------------------------------------------------------
        public override void LoadConfiguration (string[] args)
        {
            base.LoadConfiguration(args);
            m_clientConfig = new ClientConfig(m_config, m_consoleLogger, m_fileLogger);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Déchargement des données de configuration
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void UnLoadConfiguration ()
        {
            base.UnLoadConfiguration();
            m_clientConfig = null;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Fixe la culture courante
        /// </summary>
        /// <param name="name">le nom de la culture</param>
        /// -----------------------------------------------------------------------------
        public override void SetCulture(String name)
        {
            base.SetCulture(name);
            if ((name != null) && (name != String.Empty))
            {
                Bdt.Client.Resources.Strings.Culture = new CultureInfo(name);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Traitement principal
        /// </summary>
        /// -----------------------------------------------------------------------------
        protected virtual void Run(string[] args)
        {
            try
            {
                LoadConfiguration(args);

                StartClient();

                Log(Strings.CLIENT_STARTED, ESeverity.INFO);
                Console.ReadLine();

                StopClient();

                UnLoadConfiguration();
            }
            catch (Exception ex)
            {
                if (LoggedObject.GlobalLogger != null)
                {
                    Log(ex.Message, ESeverity.ERROR);
                    Log(ex.ToString(), ESeverity.DEBUG);
                }
                else
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        #endregion

    }

}

