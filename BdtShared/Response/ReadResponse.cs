// -----------------------------------------------------------------------------
// BoutDuTunnel
// Sebastien LEBRETON
// sebastien.lebreton[-at-]free.fr
// -----------------------------------------------------------------------------

#region " Inclusions "
using System;
#endregion

namespace Bdt.Shared.Response
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Une réponse de lecture
    /// </summary>
    /// -----------------------------------------------------------------------------
    [Serializable()]
    public struct ReadResponse : IGenericResponse
    {

        #region " Attributs "
        private bool m_success;
        private string m_message;
        private bool m_dataAvailable;
        private bool m_connected;
        private byte[] m_data;
        #endregion

        #region " Proprietes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Des données sont-elles disponibles?
        /// </summary>
        /// -----------------------------------------------------------------------------
        public bool DataAvailable
        {
            get
            {
                return m_dataAvailable;
            }
            set
            {
                m_dataAvailable = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// La connexion est-elle effective?
        /// </summary>
        /// -----------------------------------------------------------------------------
        public bool Connected
        {
            get
            {
                return m_connected;
            }
            set
            {
                m_connected = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// La requête a aboutie/échoué ?
        /// </summary>
        /// -----------------------------------------------------------------------------
        public bool Success
        {
            get
            {
                return m_success;
            }
            set
            {
                m_success = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Le message d'information
        /// </summary>
        /// -----------------------------------------------------------------------------
        public string Message
        {
            get
            {
                return m_message;
            }
            set
            {
                m_message = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Les données lues
        /// </summary>
        /// -----------------------------------------------------------------------------
        public byte[] Data
        {
            get
            {
                return m_data;
            }
            set
            {
                m_data = value;
            }
        }
        #endregion

    }

}


