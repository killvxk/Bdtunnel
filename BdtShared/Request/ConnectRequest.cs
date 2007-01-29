// -----------------------------------------------------------------------------
// BoutDuTunnel
// Sebastien LEBRETON
// sebastien.lebreton[-at-]free.fr
// -----------------------------------------------------------------------------

#region " Inclusions "
using System;
#endregion

namespace Bdt.Shared.Request
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Une demande de connexion
    /// </summary>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public struct ConnectRequest : IGenericRequest 
    {

        #region " Attributs "
        private string m_address;
        private int m_port;
        private int m_cid;
        private int m_uid;
        #endregion

        #region " Proprietes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Le jeton de connexion
        /// </summary>
        /// -----------------------------------------------------------------------------
        public int Cid
        {
            get
            {
                return m_cid;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Le jeton utilisateur
        /// </summary>
        /// -----------------------------------------------------------------------------
        public int Uid
        {
            get
            {
                return m_uid;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// L'adresse distante
        /// </summary>
        /// -----------------------------------------------------------------------------
        public string Address
        {
            get
            {
                return m_address;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Le port distant
        /// </summary>
        /// -----------------------------------------------------------------------------
        public int Port
        {
            get
            {
                return m_port;
            }
        }
        #endregion

        #region " Methodes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="uid">Le jeton utilisateur</param>
        /// <param name="address">L'adresse distante</param>
        /// <param name="port">Le port distant</param>
        /// -----------------------------------------------------------------------------
        public ConnectRequest(int uid, string address, int port)
        {
            this.m_uid = uid;
            this.m_cid = -1;
            this.m_address = address;
            this.m_port = port;
        }
        #endregion

    }

}

