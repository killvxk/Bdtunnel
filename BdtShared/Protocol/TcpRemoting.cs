// -----------------------------------------------------------------------------
// BoutDuTunnel
// Sebastien LEBRETON
// sebastien.lebreton[-at-]free.fr
// -----------------------------------------------------------------------------

#region " Inclusions "
using System;
using System.Runtime.Remoting.Channels.Tcp;
#endregion

namespace Bdt.Shared.Protocol
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Protocole de communication basé sur le remoting .NET et sur le protocole TCP
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class TcpRemoting : GenericRemoting
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
                    m_clientchannel = new TcpChannel();
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
                    m_serverchannel = new TcpChannel(Port);
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
                return string.Format("tcp://{0}:{1}/{2}", Address, Port, Name);
            }
        }
        #endregion

    }

}

