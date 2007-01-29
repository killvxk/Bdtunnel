// -----------------------------------------------------------------------------
// BoutDuTunnel
// Sebastien LEBRETON
// sebastien.lebreton[-at-]free.fr
// -----------------------------------------------------------------------------

#region " Inclusions "
using System;
using System.Collections.Generic;
#endregion

namespace Bdt.Shared.Configuration
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Regroupe un ensemble de sources de configurations priorisées. Permet de rechercher des
    /// valeurs suivant un code.
    /// Les sources peuvent être différentes: base de donnée, fichier de configuration,
    /// ligne de commande, etc
    /// </summary>
    /// -----------------------------------------------------------------------------
    public sealed class ConfigPackage
    {

        #region " Evenements "
        public delegate void ReadStringEventHandler(ConfigPackage sender, ref string value);
        private ReadStringEventHandler ReadStringEvent;

        public event ReadStringEventHandler ReadString
        {
            add
            {
                ReadStringEvent = (ReadStringEventHandler)System.Delegate.Combine(ReadStringEvent, value);
            }
            remove
            {
                ReadStringEvent = (ReadStringEventHandler)System.Delegate.Remove(ReadStringEvent, value);
            }
        }

        public delegate void ReadIntEventHandler(ConfigPackage sender, ref int value);
        private ReadIntEventHandler ReadIntEvent;

        public event ReadIntEventHandler ReadInt
        {
            add
            {
                ReadIntEvent = (ReadIntEventHandler)System.Delegate.Combine(ReadIntEvent, value);
            }
            remove
            {
                ReadIntEvent = (ReadIntEventHandler)System.Delegate.Remove(ReadIntEvent, value);
            }
        }

        public delegate void ReadBoolEventHandler(ConfigPackage sender, ref bool value);
        private ReadBoolEventHandler ReadBoolEvent;

        public event ReadBoolEventHandler ReadBool
        {
            add
            {
                ReadBoolEvent = (ReadBoolEventHandler)System.Delegate.Combine(ReadBoolEvent, value);
            }
            remove
            {
                ReadBoolEvent = (ReadBoolEventHandler)System.Delegate.Remove(ReadBoolEvent, value);
            }
        }

        #endregion

        #region " Attributs "
        //Les sources sont triées par priorité grâce au compareTo de SourceConfiguration (IComparable)
        private List<BaseConfig> m_sources = new List<BaseConfig>();
        #endregion

        #region " Propriétés "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Retourne la valeur d'un élément suivant son code
        /// </summary>
        /// <param name="code">le code de l'élément</param>
        /// <param name="defaultValue">la valeur par défaut si l'élément est introuvable</param>
        /// <returns>La valeur de l'élément s'il existe ou defaultValue sinon</returns>
        /// -----------------------------------------------------------------------------
        public string Value(string code, string defaultValue)
        {
            foreach (BaseConfig source in m_sources)
            {
                string result = source.Value(code, null);
                if (result != null)
                {
                    if (ReadStringEvent != null)
                        ReadStringEvent(this, ref result);
                    return result;
                }
            }
            return defaultValue;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Fixe/Retourne la valeur entière d'un élément suivant son code
        /// </summary>
        /// <param name="code">le code de l'élément</param>
        /// <param name="defaultValue">la valeur par défaut si l'élément est introuvable</param>
        /// <returns>La valeur de l'élément s'il existe et s'il représente un entier ou defaultValue sinon</returns>
        /// -----------------------------------------------------------------------------
        public int ValueInt(string code, int defaultValue)
        {
            try
            {
                int result = int.Parse(Value(code, defaultValue.ToString()));
                if (ReadIntEvent != null)
                    ReadIntEvent(this, ref result);
                return result;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Fixe/Retourne la valeur booléenne d'un élément suivant son code
        /// </summary>
        /// <param name="code">le code de l'élément</param>
        /// <param name="defaultValue">la valeur par défaut si l'élément est introuvable</param>
        /// <returns>La valeur de l'élément s'il existe et s'il représente un booléen (true/false) ou defaultValue sinon</returns>
        /// -----------------------------------------------------------------------------
        public bool ValueBool(string code, bool defaultValue)
        {
            try
            {
                bool result = bool.Parse(Value(code, defaultValue.ToString()));
                if (ReadBoolEvent != null)
                    ReadBoolEvent(this, ref result);
                return result;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
        #endregion

        #region " Méthodes "
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Ajoute une source à ce contexte de configuration
        /// </summary>
        /// <param name="source">la source à ajouter, qui sera classée par SourceConfiguration.Priority()</param>
        /// -----------------------------------------------------------------------------
        public void AddSource(BaseConfig source)
        {
            m_sources.Add(source);
            m_sources.Sort();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Enleve une source de ce contexte de configuration
        /// </summary>
        /// <param name="source">la source à supprimer</param>
        /// -----------------------------------------------------------------------------
        public void RemoveSource(BaseConfig source)
        {
            m_sources.Remove(source);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Force un rechargement de toutes les sources de données liées
        /// </summary>
        /// -----------------------------------------------------------------------------
        public void RehashAll()
        {
            foreach (BaseConfig source in m_sources)
            {
                source.Rehash();
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Concatène tous les éléments depuis toutes les sources
        /// </summary>
        /// <returns>le format de chaque ligne est classe,priorité,code,valeur</returns>
        /// -----------------------------------------------------------------------------
        public override string ToString()
        {
            string returnValue;
            returnValue = string.Empty;
            foreach (BaseConfig source in m_sources)
            {
                returnValue += source.ToString();
            }
            return returnValue;
        }
        #endregion

    }

}

