// -----------------------------------------------------------------------------
// BoutDuTunnel
// Sebastien LEBRETON
// sebastien.lebreton[-at-]free.fr
// -----------------------------------------------------------------------------

#region " Inclusions "
using System;
using System.Xml;
#endregion

namespace Bdt.Shared.Configuration
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Représente une source de configuration basée sur un fichier XML
    /// </summary>
    /// -----------------------------------------------------------------------------
    public sealed class XMLConfig : BaseConfig
    {

        #region " Attributs "
        private string m_filename = ""; //Le nom du fichier XML associé à la source
        #endregion

        #region " Propriétés "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Retourne/Fixe le nom du fichier XML associé à la source
        /// </summary>
        /// <returns>le nom du fichier XML associé à la source</returns>
        /// -----------------------------------------------------------------------------
        public string FileName
        {
            get
            {
                return m_filename;
            }
            set
            {
                m_filename = value;
            }
        }
        #endregion

        #region " Méthodes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Création d'une source de donnée basée sur un fichier XML
        /// </summary>
        /// <param name="filename">le nom du fichier XML à lier</param>
        /// <param name="priority">la priorité de cette source (la plus basse=prioritaire)</param>
        /// -----------------------------------------------------------------------------
        public XMLConfig(string filename, int priority)
            : base(priority)
        {
            this.FileName = filename;
            Rehash();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Parsing des noeuds XML
        /// </summary>
        /// <param name="path">le chemin courant</param>
        /// <param name="node">le noeud en cours</param>
        /// -----------------------------------------------------------------------------
        private void ParseNode(string path, XmlNode node)
        {
            foreach (XmlNode subnode in node.ChildNodes)
            {

                if (subnode.Attributes != null)
                {
                    foreach (XmlAttribute attr in subnode.Attributes)
                    {
                        if (attr.Value != string.Empty)
                        {
                            this.SetValue(path + subnode.Name + SOURCE_ITEM_ATTRIBUTE + attr.Name, null, attr.Value);
                        }
                    }
                    if (subnode.InnerText != string.Empty)
                    {
                        this.SetValue(path + subnode.Name, null, subnode.InnerText);
                    }
                }

                if ((subnode.HasChildNodes) && (subnode.ChildNodes[0].NodeType == XmlNodeType.Element || subnode.ChildNodes[0].NodeType == XmlNodeType.Comment))
                {
                    // Chemin
                    ParseNode(path + subnode.Name + SOURCE_PATH_SEPARATOR, subnode);
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Force le rechargement de la source de donnée
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void Rehash()
        {
            XmlDocument docXML = new XmlDocument();
            docXML.Load(FileName);
            ParseNode(string.Empty, docXML.DocumentElement);
        }
        #endregion

    }

}


