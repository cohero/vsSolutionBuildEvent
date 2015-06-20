﻿/*
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace net.r_eg.vsSBE
{
    public class GAC
    {
        /// <summary>
        /// The fusion API.
        /// ASM_CACHE_FLAGS Enumeration: https://msdn.microsoft.com/en-us/library/ms231621.aspx
        /// Global Assembly Cache (GAC) APIs: https://support.microsoft.com/en-us/kb/317540
        /// </summary>
        internal static class Fusion
        {
            /// <summary>
            /// Indicates that the GetCachePath function should return the path to the global assembly cache for CLR version 2.0. 
            /// </summary>
            public const uint ASM_CACHE_ROOT = 0x08;

            /// <summary>
            /// Indicates that the GetCachePath function should return the path to the global assembly cache for CLR version 4.
            /// </summary>
            public const uint ASM_CACHE_ROOT_EX = 0x80;

            /// <summary>
            /// Gets the path to the cached assembly, using the specified flags.
            /// https://msdn.microsoft.com/en-us/library/ms232964.aspx
            /// </summary>
            /// <param name="flags">[in] An ASM_CACHE_FLAGS value that indicates the source of the cached assembly.</param>
            /// <param name="path">[out] The returned pointer to the path.</param>
            /// <param name="size">[in, out] The requested maximum length of the path, and upon return, the actual length of the path.</param>
            /// <returns></returns>
            [DllImport("fusion.dll")]
            internal static extern int GetCachePath(uint flags, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder path, ref uint size);
        }

        /// <summary>
        /// Gets the path to the cached assembly.
        /// </summary>
        /// <param name="clrV4">Use global assembly cache for CLR version 4</param>
        /// <returns></returns>
        public string getGacPath(bool clrV4 = true)
        {
            // In the Windows API (with some exceptions), the maximum length for a path is MAX_PATH, which is defined as 260 characters.
            const int MAX_PATH  = 260;
            StringBuilder path  = new StringBuilder(MAX_PATH);
            uint size           = MAX_PATH;

            Fusion.GetCachePath((clrV4)? Fusion.ASM_CACHE_ROOT_EX : Fusion.ASM_CACHE_ROOT, path, ref size);
            return path.ToString();
        }

        /// <summary>
        /// 
        /// TODO: a better way use the - 
        ///       STDAPI CreateAssemblyEnum(IAssemblyEnum **pEnum, IUnknown *pUnkReserved, IAssemblyName *pName, DWORD dwFlags, LPVOID pvReserved);
        /// </summary>
        /// <param name="name">Assembly name.</param>
        /// <param name="version">Version of assembly or null value if not important.</param>
        /// <param name="publicKeyToken">PublicKeyToken of assembly or null value if not important.</param>
        /// <returns></returns>
        public string absolutePathToAssemblyName(string name, string version = null, string publicKeyToken = null)
        {
            // Where to look
            string[] paths = new string[] {
                Path.Combine(getGacPath(false), "GAC"),
                getGacPath(true),
            };
            string filename = String.Format("{0}.dll", name);

            foreach(string path in paths)
            {
                string absolute = Path.Combine(path, name);
                if(!Directory.Exists(absolute)) {
                    continue;
                }

                if(!String.IsNullOrEmpty(version) && !String.IsNullOrEmpty(publicKeyToken))
                {
                    absolute = Path.Combine(absolute, String.Format("{0}__{1}", version, publicKeyToken), filename);
                    return File.Exists(absolute)? absolute : null;
                }

                foreach(string dir in Directory.GetDirectories(absolute, (version != null)? String.Format("{0}__*", version) : "*"))
                {
                    absolute = Path.Combine(absolute, dir, filename);
                    if(File.Exists(absolute)) {
                        return absolute;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets path to assembly.
        /// Supports formats:
        ///     * EnvDTE.dll
        ///     * EnvDTE
        ///     * C:\WINDOWS\assembly\GAC\EnvDTE\8.0.0.0__b03f5f7f11d50a3a\EnvDTE.dll
        ///     * EnvDTE, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
        ///     * EnvDTE, Version=8.0.0.0, PublicKeyToken=b03f5f7f11d50a3a
        /// </summary>
        /// <param name="ident">Supported formats.</param>
        /// <returns></returns>
        public string getPathToAssembly(string ident)
        {
            if(Path.IsPathRooted(ident)) {
                return ident;
            }

            if(ident.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)) {
                return absolutePathToAssemblyName(ident.Substring(0, ident.Length - 4));
            }

            int comma       = ident.IndexOf(",");
            string name     = (comma == -1)? ident : ident.Substring(0, comma);
            string version  = extractKey("Version", ident);

            if(version == null) {
                return absolutePathToAssemblyName(name);
            }
            return absolutePathToAssemblyName(name, version, extractKey("PublicKeyToken", ident));
        }

        /// <summary>
        /// Extract value from keys of AssemblyInfo.
        /// </summary>
        /// <param name="key">Key name.</param>
        /// <param name="ident">AssemblyInfo data.</param>
        /// <returns></returns>
        protected virtual string extractKey(string key, string ident)
        {
            Match m = Regex.Match(ident, key + @"\s*=\s*([^,\s]+)", RegexOptions.IgnoreCase);
            if(!m.Success) {
                return null;
            }
            return m.Groups[1].Value;
        }
    }
}