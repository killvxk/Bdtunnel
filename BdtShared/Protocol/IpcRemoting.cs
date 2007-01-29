// -----------------------------------------------------------------------------
// BoutDuTunnel
// Sebastien LEBRETON
// sebastien.lebreton[-at-]free.fr
// -----------------------------------------------------------------------------

#region " Inclusions "
using System;
using System.Runtime.Remoting.Channels.Ipc;
#endregion

namespace Bdt.Shared.Protocol
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Protocole de communication basé sur le remoting .NET et sur le protocole IPC
    /// Exclusivement pour une communication sur la même machine (client/serveur)
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class IpcRemoting : GenericRemoting
    {

        #region " Proprietes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Le canal de communication côté client
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override System.Runtime.Remoting.Channels.IChannel ClientChannel
        {
            get
            {
                if (m_clientchannel == null)
                {
                    m_clientchannel = new IpcChannel();
                }
                return m_clientchannel;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Le canal de communication côté serveur
        /// </summary>
        /// -----------------------------------------------------------------------------
        protected override System.Runtime.Remoting.Channels.IChannel ServerChannel
        {
            get
            {
                if (m_serverchannel == null)
                {
                    m_serverchannel = new IpcChannel(Name);
                }
                return m_serverchannel;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// L'URL nécessaire pour se connecter au serveur
        /// </summary>
        /// -----------------------------------------------------------------------------
        protected override string ServerURL
        {
            get
            {
                return string.Format("ipc://{0}/{0}", Name, Name);
            }
        }
        #endregion

    }

}

