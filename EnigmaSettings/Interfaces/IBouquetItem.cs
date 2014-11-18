// Copyright (c) 2013 Krkadoni.com - Released under The MIT License.
// Full license text can be found at http://opensource.org/licenses/MIT

using System;
using System.ComponentModel;

namespace Krkadoni.EnigmaSettings.Interfaces
{
    public interface IBouquetItem : INotifyPropertyChanged, IEditableObject, ICloneable
    {
        /// <summary>
        ///     Type of bouquet item
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        Enums.BouquetItemType BouquetItemType { get; }

        /// <summary>
        ///     First number in SERVICE line inside one of the bouquets files
        /// </summary>
        Enums.FavoritesType FavoritesType { get; }

        /// <summary>
        ///     Second number in SERVICE line inside one of the bouquets file
        /// </summary>
        /// <remarks></remarks>
        Enums.LineSpecifier LineSpecifierType { get; }

        /// <summary>
        ///     First number in SERVICE line inside one of the bouquets files
        /// </summary>
        string FavoritesTypeFlag { get; set; }

        /// <summary>
        ///     Second number in SERVICE line inside one of the bouquets file
        /// </summary>
        /// <remarks></remarks>
        string LineSpecifierFlag { get; set; }

        /// <summary>
        /// Performs MemberwiseClone on current object
        /// </summary>
        /// <returns></returns>
        object ShallowCopy();
    }
}