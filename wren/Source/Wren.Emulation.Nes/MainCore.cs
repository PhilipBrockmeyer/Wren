//////////////////////////////////////////////////////////////////////////////
//This file is part of My Nes                                               //
//A Nintendo Entertainment System Emulator.                                 //
//                                                                          //
//Copyright © 2009 - 2010 Ala Hadid (AHD)                                   //
//                                                                          //
//My Nes is free software; you can redistribute it and/or modify            //
//it under the terms of the GNU General Public License as published by      //
//the Free Software Foundation; either version 2 of the License, or         //
//(at your option) any later version.                                       //
//                                                                          //
//My Nes is distributed in the hope that it will be useful,                 //
//but WITHOUT ANY WARRANTY; without even the implied warranty of            //
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the             //
//GNU General Public License for more details.                              //
//                                                                          //
//You should have received a copy of the GNU General Public License         //
//along with this program; if not, write to the Free Software               //
//Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA//
//////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace AHD.MyNes.Core
{
    /// <summary>
    /// The main class of this library
    /// </summary>
    public static class MainCore
    {
        static string _SizeLable = "";
        static Settings _Settings;
        #region Assembly Attribute Accessors
        /// <summary>
        /// Get the assemply title
        /// </summary>
        public static string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }
        /// <summary>
        /// Get the version
        /// </summary>
        public static string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
        /// <summary>
        /// get the Description
        /// </summary>
        public static string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }
        /// <summary>
        /// Get the Product
        /// </summary>
        public static string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }
        /// <summary>
        /// get the copyright
        /// </summary>
        public static string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }
        /// <summary>
        /// Get the company
        /// </summary>
        public static string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion
        /// <summary>
        /// Handle an exception
        /// </summary>
        /// <param name="ex">Exception to handle</param>
        public static void HandleException(Exception ex)
        {
            if (ex.GetType() == typeof(DllNotFoundException))
            {
                MessageBox.Show(ex.Message, "An necessary dll library is missing, reinstall the program will solve this problem.");
            }
            else
            {
                MessageBox.Show("Exception message : " + ex.Message + "\n\nData :\n" + ex.ToString(), "UNEXPACTED ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Close every thing in the program immediatly
        /// </summary>
        public static void CloseProgram()
        { Application.Exit(); }
        /// <summary>
        /// Get file size
        /// </summary>
        /// <param name="FilePath">Full path of the file</param>
        /// <returns>Return file size + unit lable</returns>
        public static string GetFileSize(string FilePath)
        {
            if (File.Exists(Path.GetFullPath(FilePath)) == true)
            {
                FileInfo Info = new FileInfo(FilePath);
                string Unit = " Byte";
                double Len = Info.Length;
                if (Info.Length >= 1024)
                {
                    Len = Info.Length / 1024.00;
                    Unit = " KB";
                }
                if (Len >= 1024)
                {
                    Len /= 1024.00;
                    Unit = " MB";
                }
                if (Len >= 1024)
                {
                    Len /= 1024.00;
                    Unit = " GB";
                }
                return Len.ToString("F2") + Unit;
            }
            return "";
        }
        /// <summary>
        /// Get size
        /// </summary>
        /// <param name="SizeInByte"></param>
        /// <returns></returns>
        public static double GetSize(double SizeInByte)
        {
            string Unit = " Byte";
            double Len = SizeInByte;
            if (SizeInByte >= 1024)
            {
                Len = SizeInByte / 1024.00;
                Unit = " KB";
            }
            if (Len >= 1024)
            {
                Len /= 1024.00;
                Unit = " MB";
            }
            if (Len >= 1024)
            {
                Len /= 1024.00;
                Unit = " GB";
            }
            _SizeLable = Unit;
            return Len;
        }
        /// <summary>
        /// Get the lable size
        /// </summary>
        /// <returns></returns>
        public static string GetSizeLable()
        { return _SizeLable; }
        /// <summary>
        /// Get the application startup directory
        /// </summary>
        public static string ApplicationStartupDirectory
        { get { return Application.StartupPath; } }
        /// <summary>
        /// Get the main help file's path
        /// </summary>
        public static string HelpPath
        {
            get
            {
                return Application.StartupPath + "\\Help.chm";
            }
        }
        /// <summary>
        /// Get or set the settings class
        /// </summary>
        public static Settings Settings
        { get { return _Settings; } set { _Settings = value; } }
        /// <summary>
        /// Get the rom path that extracted from archive
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns>The first .nes file detected in the archive</returns>
        public static string GetRomPath(string FileName)
        {
            { return FileName; }
        }
    }
}
