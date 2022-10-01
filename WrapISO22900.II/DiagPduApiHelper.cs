﻿#region License

// /*
// MIT License
// 
// Copyright (c) 2022 Joerg Frank
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// */

#endregion

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.XPath;

namespace ISO22900.II
{
    /// <summary>
    /// Static methods for accessing the registry and reading the XML (root.xml)
    /// I'm a helper to keep reading the registry and the XML out of the actual API
    /// I should be replaced if, for example, another XML reader is preferred
    /// </summary>
    public static class DiagPduApiHelper
    {
        private static string FullyQualifiedRootFilePath()
        {
            var path = string.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //On Windows path is stored in registry
                using (var key = Registry.LocalMachine.OpenSubKey("Software")?.OpenSubKey("D-PDU API"))
                {
                    if (key != null)
                    {
                        path = key.GetValue("Root File") as string;
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                //ToDo: (return a path)
                //ISO 22900-2:2017(E)  8.7.3.1 Locating the Linux D-PDU API shared library
                //a) The root file is stored as the file  “pdu_api_root.xml” in the directory “/etc”.
                //b) The pdu_api_root.xml file contains all the installed MVCI protocol  modules’ shared library information from each vendor.
            }

            return path;
        }

        /// <summary>
        /// API Shortnames
        /// </summary>
        /// <returns>List of Shortnames</returns>
        public static IEnumerable<string> MvciPduApiShortNameList()
        {
            var path = FullyQualifiedRootFilePath();
            if (!path.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                yield break;
            }

            //Load the file and create a navigator object. 
            var xPath = new XPathDocument(path);
            var navigator = xPath.CreateNavigator();
            var nodeIterator = navigator.Select("MVCI_PDU_API_ROOT/MVCI_PDU_API/SHORT_NAME");
            foreach (XPathNavigator node in nodeIterator)
            {
                yield return node.Value;
            }
        }

        /// <summary>
        /// Get the dll-Path (or so-Path) from API-ShortName
        /// </summary>
        /// <param name="shortName"></param>
        /// <returns></returns>
        [Obsolete("Method is deprecated, please use FullLibraryPathFormApiShortName instead.")]
        public static string FullyQualifiedLibraryFileNameFormShortName(string shortName)
        {
            return FullLibraryPathFormApiShortName(shortName);
        }

        /// <summary>
        /// Get the dll-Path (or so-Path) from API-ShortName
        /// </summary>
        /// <param name="apiShortName"></param>
        /// <returns></returns>
        public static string FullLibraryPathFormApiShortName(string apiShortName)
        {
            var libraryFile = string.Empty;
            var path = FullyQualifiedRootFilePath();
            if (!path.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                return libraryFile;
            }

            //Load the file and create a navigator object. 
            var xPath = new XPathDocument(path);
            var navigator = xPath.CreateNavigator();
            var nodeIterator = navigator.SelectSingleNode(
                "MVCI_PDU_API_ROOT/MVCI_PDU_API/SHORT_NAME[normalize-space(text()) = '" + apiShortName + "']/.." +
                "/LIBRARY_FILE/@URI");
            if (nodeIterator != null)
                libraryFile = new Uri(nodeIterator.Value, UriKind.Absolute).LocalPath;

            return libraryFile;
        }

        /// <summary>
        /// Get the module description file (MDF)-Path from API-ShortName
        /// This API wrapper is made to work without this file.
        /// But sometimes it can be useful to read in the file for additional information (descriptions).
        /// In this case this function is useful to get the file path
        /// </summary>
        /// <param name="apiShortName"></param>
        /// <returns></returns>
        public static string FullMdfPathFormApiShortName(string apiShortName)
        {
            var moduleDescriptionFile = string.Empty;
            var path = FullyQualifiedRootFilePath();
            if (!path.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                return moduleDescriptionFile;
            }

            //Load the file and create a navigator object. 
            var xPath = new XPathDocument(path);
            var navigator = xPath.CreateNavigator();
            var nodeIterator = navigator.SelectSingleNode(
                "MVCI_PDU_API_ROOT/MVCI_PDU_API/SHORT_NAME[normalize-space(text()) = '" + apiShortName + "']/.." +
                "/MODULE_DESCRIPTION_FILE/@URI");
            if (nodeIterator != null)
                moduleDescriptionFile = new Uri(nodeIterator.Value, UriKind.Absolute).LocalPath;

            return moduleDescriptionFile;
        }

        /// <summary>
        /// Get the cable description file (CDF)-Path from API-ShortName
        /// This API wrapper is made to work without this file.
        /// But sometimes it can be useful to read in the file for additional information (descriptions).
        /// In this case this function is useful to get the file path
        /// </summary>
        /// <param name="apiShortName"></param>
        /// <returns></returns>
        public static string FullCdfPathFormApiShortName(string apiShortName)
        {
            var cableDescriptionFile = string.Empty;
            var path = FullyQualifiedRootFilePath();
            if (!path.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                return cableDescriptionFile;
            }

            //Load the file and create a navigator object. 
            var xPath = new XPathDocument(path);
            var navigator = xPath.CreateNavigator();
            var nodeIterator = navigator.SelectSingleNode(
                "MVCI_PDU_API_ROOT/MVCI_PDU_API/SHORT_NAME[normalize-space(text()) = '" + apiShortName + "']/.." +
                "/CABLE_DESCRIPTION_FILE/@URI");
            if (nodeIterator != null)
                cableDescriptionFile = new Uri(nodeIterator.Value, UriKind.Absolute).LocalPath;

            return cableDescriptionFile;
        }


        public struct MvciPduApiDetail
        {
            public string ShortName;
            public string Description;
            public string SupplierName;
            public string LibraryFile;
            public string ModuleDescriptionFile;
            public string CableDescriptionFile;
        }

        /// <summary>
        /// Infos about the MVCI API
        /// </summary>
        /// <returns>IEnumerable of MvciPduApiDetail</returns>
        public static IEnumerable<MvciPduApiDetail> InstalledMvciPduApiDetails()
        {
            var path = FullyQualifiedRootFilePath();

            if (!path.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)) yield break;

            //Load the file and create a navigator object. 
            var xPath = new XPathDocument(path);
            var navigator = xPath.CreateNavigator();
            var nodeIterator = navigator.Select("MVCI_PDU_API_ROOT/MVCI_PDU_API");
            
            foreach (XPathNavigator node in nodeIterator)
            {
                yield return new MvciPduApiDetail
                {
                    ShortName = node.SelectSingleNode("./SHORT_NAME")?.Value ?? string.Empty,
                    Description = node.SelectSingleNode("./DESCRIPTION")?.Value ?? string.Empty,
                    SupplierName = node.SelectSingleNode("./SUPPLIER_NAME")?.Value ?? string.Empty,
                    LibraryFile = new Uri(node.SelectSingleNode("./LIBRARY_FILE/@URI")?.Value ?? string.Empty).LocalPath,
                    ModuleDescriptionFile = new Uri(node.SelectSingleNode("./MODULE_DESCRIPTION_FILE/@URI")?.Value ?? string.Empty).LocalPath,
                    CableDescriptionFile = new Uri(node.SelectSingleNode("./CABLE_DESCRIPTION_FILE/@URI")?.Value ?? string.Empty).LocalPath
                };
            }
        }

    }
}