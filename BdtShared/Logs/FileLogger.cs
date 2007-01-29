// -----------------------------------------------------------------------------
// BoutDuTunnel
// Sebastien LEBRETON
// sebastien.lebreton[-at-]free.fr
// -----------------------------------------------------------------------------

#region " Inclusions "
using System;
using System.IO;

using Bdt.Shared.Configuration;
#endregion

namespace Bdt.Shared.Logs
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Génération des logs dans un fichier
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class FileLogger : BaseLogger
    {

        #region " Constantes "
        public const string CONFIG_APPEND = "append";
        public const string CONFIG_FILENAME = "filename";
        #endregion

        #region " Attributs "
        protected string m_filename = null;
        protected bool m_append = false;
        #endregion

        #region " Propriétés "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Retourne le nom du fichier utilisé pour l'écriture des logs
        /// </summary>
        /// <returns>le nom du fichier utilisé pour l'écriture des logs</returns>
        /// -----------------------------------------------------------------------------
        public string Filename
        {
            get
            {
                return m_filename;
            }
            protected set
            {
                m_filename = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Retourne l'état indiquant si les données doivent être ajoutées au fichier
        /// </summary>
        /// <returns>l'état indiquant si les données doivent être ajoutées au fichier</returns>
        /// -----------------------------------------------------------------------------
        public bool Append
        {
            get
            {
                return m_append;
            }
            protected set
            {
                m_append = value;
            }
        }
        #endregion

        #region " Méthodes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Constructeur pour un log vierge
        /// </summary>
        /// -----------------------------------------------------------------------------
        protected FileLogger ()
        {
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Constructeur pour un log à partir des données fournies dans une configuration
        /// </summary>
        /// <param name="prefix">le prefixe dans la configuration ex: application/log</param>
        /// <param name="config">la configuration pour la lecture des parametres</param>
        /// -----------------------------------------------------------------------------
        public FileLogger(string prefix, ConfigPackage config)
            : base(null, prefix, config)
        {
            m_filename = config.Value(prefix + Bdt.Shared.Configuration.BaseConfig.SOURCE_ITEM_ATTRIBUTE + CONFIG_FILENAME, m_filename);
            m_append = config.ValueBool(prefix + Bdt.Shared.Configuration.BaseConfig.SOURCE_ITEM_ATTRIBUTE + CONFIG_APPEND, m_append);
            if (Enabled)
            {
                m_writer = new StreamWriter(m_filename, m_append, System.Text.Encoding.Default);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Constructeur pour un log
        /// </summary>
        /// <param name="filename">le nom du fichier dans lequel écrire</param>
        /// <param name="append">si false la fichier sera écrasé</param>
        /// <param name="dateFormat">le format des dates de timestamp</param>
        /// <param name="filter">le niveau de filtrage pour la sortie des logs</param>
        /// -----------------------------------------------------------------------------
        public FileLogger(string filename, bool append, string dateFormat, ESeverity filter)
            : base(new StreamWriter(filename, append, System.Text.Encoding.Default), dateFormat, filter)
        {
            m_filename = filename;
            m_append = append;
        }
        #endregion

    }

}

