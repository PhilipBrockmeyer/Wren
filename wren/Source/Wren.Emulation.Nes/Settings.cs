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
using System.Windows.Forms;
using System.Text;
using System.Xml;
using System.IO;
using SlimDX.DirectInput;
using AHD.MyNes.Nes;
using Wren.Emulation.Nes;
namespace AHD.MyNes.Core
{
    /// <summary>
    /// Settings class
    /// </summary>
    public partial class Settings
    {
        int _WindowX = 195;
        int _WindowY = 99;
        int _WindowW = 521;
        int _WindowH = 563;

        string _Player1_A = "Keyboard.X";
        string _Player1_B = "Keyboard.Z";
        string _Player1_Start = "Keyboard.V";
        string _Player1_Select = "Keyboard.C";
        string _Player1_Left = "Keyboard.LeftArrow";
        string _Player1_Right = "Keyboard.RightArrow";
        string _Player1_Up = "Keyboard.UpArrow";
        string _Player1_Down = "Keyboard.DownArrow";

        string _Player2_A = "Keyboard.K";
        string _Player2_B = "Keyboard.J";
        string _Player2_Start = "Keyboard.E";
        string _Player2_Select = "Keyboard.Q";
        string _Player2_Left = "Keyboard.A";
        string _Player2_Right = "Keyboard.D";
        string _Player2_Up = "Keyboard.W";
        string _Player2_Down = "Keyboard.S";

        string _ScreenShotsFolder = Path.GetFullPath(".\\Screenshots");
        string _ScreenshotFormat = ".bmp";
        string _PalettePath = Path.GetFullPath(".\\Pal.pal");
        string _StatesFolder = Path.GetFullPath(".\\State Saves");
        List<string> _Recents = new List<string>();
        string _DrawMode = "GDI 16-bit";

        int _MasterVolume = 7;
        bool _DMCEnabled = true;
        bool _SQR1enabled = true;
        bool _SQR2enabled = true;
        bool _TRIenabled = true;
        bool _NoiseEnabled = true;
        bool _SoundEnabled = true;
        bool _ShowBrowser = true;
        bool _ShowDebugger = true;

        NesSystem _System = NesSystem.NTSC;
        bool _SpeedThrottling = false;
        public bool SpeedThrottling { get { return _SpeedThrottling; } set { _SpeedThrottling = value; } }
        public NesSystem NesRegion { get { return _System; } set { _System = value; } }
        public bool NoiseEnabled { get { return _NoiseEnabled; } set { _NoiseEnabled = value; } }
        public bool TRIenabled { get { return _TRIenabled; } set { _TRIenabled = value; } }
        public bool SQR2enabled { get { return _SQR2enabled; } set { _SQR2enabled = value; } }
        public bool SQR1enabled { get { return _SQR1enabled; } set { _SQR1enabled = value; } }
        public bool DMCEnabled { get { return _DMCEnabled; } set { _DMCEnabled = value; } }
        public int MasterVolume { get { return _MasterVolume; } set { _MasterVolume = value; } }
        public bool SoundEnabled { get { return _SoundEnabled; } set { _SoundEnabled = value; } }
        public string StatesFolder { get { return _StatesFolder; } set { _StatesFolder = value; } }
        public bool ShowBrowser { get { return _ShowBrowser; } set { _ShowBrowser = value; } }
        public bool ShowDebugger { get { return _ShowDebugger; } set { _ShowDebugger = value; } }

        /// <summary>
        /// Get or set the main window position x
        /// </summary>
        public int WindowX { get { return _WindowX; } set { _WindowX = value; } }
        /// <summary>
        /// Get or set the main window position y
        /// </summary>
        public int WindowY { get { return _WindowY; } set { _WindowY = value; } }
        /// <summary>
        /// Get or set the main window width
        /// </summary>
        public int WindowW { get { return _WindowW; } set { _WindowW = value; } }
        /// <summary>
        /// Get or set the main window height
        /// </summary>
        public int WindowH { get { return _WindowH; } set { _WindowH = value; } }

        public string Player1_A { get { return _Player1_A; } set { _Player1_A = value; } }
        public string Player1_B { get { return _Player1_B; } set { _Player1_B = value; } }
        public string Player1_Start { get { return _Player1_Start; } set { _Player1_Start = value; } }
        public string Player1_Select { get { return _Player1_Select; } set { _Player1_Select = value; } }
        public string Player1_Left { get { return _Player1_Left; } set { _Player1_Left = value; } }
        public string Player1_Right { get { return _Player1_Right; } set { _Player1_Right = value; } }
        public string Player1_Up { get { return _Player1_Up; } set { _Player1_Up = value; } }
        public string Player1_Down { get { return _Player1_Down; } set { _Player1_Down = value; } }

        public string Player2_A { get { return _Player2_A; } set { _Player2_A = value; } }
        public string Player2_B { get { return _Player2_B; } set { _Player2_B = value; } }
        public string Player2_Start { get { return _Player2_Start; } set { _Player2_Start = value; } }
        public string Player2_Select { get { return _Player2_Select; } set { _Player2_Select = value; } }
        public string Player2_Left { get { return _Player2_Left; } set { _Player2_Left = value; } }
        public string Player2_Right { get { return _Player2_Right; } set { _Player2_Right = value; } }
        public string Player2_Up { get { return _Player2_Up; } set { _Player2_Up = value; } }
        public string Player2_Down { get { return _Player2_Down; } set { _Player2_Down = value; } }

        public List<string> RecentGames { get { return _Recents; } set { _Recents = value; } }
        public string ScreenShotsFolder { get { return _ScreenShotsFolder; } set { _ScreenShotsFolder = value; } }
        public string ScreenshotFormat { get { return _ScreenshotFormat; } set { _ScreenshotFormat = value; } }
        public string PalettePath { get { return _PalettePath; } set { _PalettePath = value; } }
        public string DrawModeName
        { get { return _DrawMode; } set { _DrawMode = value; } }

        /// <summary>
        /// Save settings
        /// </summary>
        public void Save()
        {
            try
            {
                //start saving the setting as xml file
                XmlWriter XMLwrt = XmlWriter.Create(MainCore.ApplicationStartupDirectory + "\\Settings.xml");
                XMLwrt.WriteStartElement("MyNesSettings");//header
                XMLwrt.WriteAttributeString("Version", MainCore.AssemblyVersion);//header version
                XMLwrt.WriteStartElement("Window");//window settings
                XMLwrt.WriteStartElement("Location");
                XMLwrt.WriteAttributeString("x", _WindowX.ToString());
                XMLwrt.WriteAttributeString("y", _WindowY.ToString());
                XMLwrt.WriteEndElement();//Location end
                XMLwrt.WriteStartElement("Size");
                XMLwrt.WriteAttributeString("w", _WindowW.ToString());
                XMLwrt.WriteAttributeString("h", _WindowH.ToString());
                XMLwrt.WriteEndElement();//size end
                XMLwrt.WriteEndElement();//window end

                XMLwrt.WriteStartElement("Controls");
                XMLwrt.WriteStartElement("Player1_A");
                XMLwrt.WriteAttributeString("A", _Player1_A);
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("Player1_B");
                XMLwrt.WriteAttributeString("B", _Player1_B);
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("Player1_Start");
                XMLwrt.WriteAttributeString("Start", _Player1_Start);
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("Player1_Select");
                XMLwrt.WriteAttributeString("Select", _Player1_Select);
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("Player1_Left");
                XMLwrt.WriteAttributeString("Left", _Player1_Left);
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("Player1_Right");
                XMLwrt.WriteAttributeString("Right", _Player1_Right);
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("Player1_Up");
                XMLwrt.WriteAttributeString("Up", _Player1_Up);
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("_Player1_Down");
                XMLwrt.WriteAttributeString("Down", _Player1_Down);
                XMLwrt.WriteEndElement();

                XMLwrt.WriteStartElement("Player2_A");
                XMLwrt.WriteAttributeString("A", _Player2_A);
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("Player2_B");
                XMLwrt.WriteAttributeString("B", _Player2_B);
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("Player2_Start");
                XMLwrt.WriteAttributeString("Start", _Player2_Start);
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("Player2_Select");
                XMLwrt.WriteAttributeString("Select", _Player2_Select);
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("Player2_Left");
                XMLwrt.WriteAttributeString("Left", _Player2_Left);
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("Player2_Right");
                XMLwrt.WriteAttributeString("Right", _Player2_Right);
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("Player2_Up");
                XMLwrt.WriteAttributeString("Up", _Player2_Up);
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("_Player2_Down");
                XMLwrt.WriteAttributeString("Down", _Player2_Down);
                XMLwrt.WriteEndElement();
                XMLwrt.WriteEndElement();//Controls end

                XMLwrt.WriteStartElement("ScreenShotsFolder");
                XMLwrt.WriteAttributeString("Val", _ScreenShotsFolder);
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("ScreenshotFormat");
                XMLwrt.WriteAttributeString("Val", _ScreenshotFormat);
                XMLwrt.WriteEndElement();

                XMLwrt.WriteStartElement("PalettePath");
                XMLwrt.WriteAttributeString("Val", _PalettePath);
                XMLwrt.WriteEndElement();

                XMLwrt.WriteStartElement("StatesFolder");
                XMLwrt.WriteAttributeString("Val", _StatesFolder);
                XMLwrt.WriteEndElement();

                XMLwrt.WriteStartElement("System");
                XMLwrt.WriteAttributeString("Val", _System.ToString());
                XMLwrt.WriteEndElement();

                XMLwrt.WriteStartElement("SpeedThrottling");
                XMLwrt.WriteAttributeString("Val", _SpeedThrottling.ToString());
                XMLwrt.WriteEndElement();

                XMLwrt.WriteStartElement("ShowBrowser");
                XMLwrt.WriteAttributeString("Val", _ShowBrowser.ToString());
                XMLwrt.WriteEndElement();

                XMLwrt.WriteStartElement("ShowDebugger");
                XMLwrt.WriteAttributeString("Val", _ShowDebugger.ToString());
                XMLwrt.WriteEndElement();

                XMLwrt.WriteStartElement("DrawMode");
                XMLwrt.WriteAttributeString("Val", _DrawMode);
                XMLwrt.WriteEndElement();

                XMLwrt.WriteStartElement("MasterVolume");
                XMLwrt.WriteAttributeString("Val", _MasterVolume.ToString());
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("DMCEnabled");
                XMLwrt.WriteAttributeString("Val", _DMCEnabled.ToString());
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("SQR1enabled");
                XMLwrt.WriteAttributeString("Val", _SQR1enabled.ToString());
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("SQR2enabled");
                XMLwrt.WriteAttributeString("Val", _SQR2enabled.ToString());
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("TRIenabled");
                XMLwrt.WriteAttributeString("Val", _TRIenabled.ToString());
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("NoiseEnabled");
                XMLwrt.WriteAttributeString("Val", _NoiseEnabled.ToString());
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("SoundEnabled");
                XMLwrt.WriteAttributeString("Val", _SoundEnabled.ToString());
                XMLwrt.WriteEndElement();
                XMLwrt.WriteStartElement("Recents");
                foreach (string Recc in _Recents)
                {
                    XMLwrt.WriteStartElement("Rec");
                    XMLwrt.WriteAttributeString("pat", Recc);
                    XMLwrt.WriteEndElement();
                }
                XMLwrt.WriteEndElement();//Recents end
                XMLwrt.WriteStartElement("Folders");
                XMLwrt.WriteEndElement();//Folders end
                XMLwrt.WriteEndElement();//header end
                XMLwrt.Flush();
                XMLwrt.Close();
            }
            catch (Exception EX)
            { MainCore.HandleException(EX); }
        }
        /// <summary>
        /// Load settings
        /// </summary>
        public void Load()
        {
            try
            {
                if (File.Exists(MainCore.ApplicationStartupDirectory + "\\Settings.xml") == true)
                {
                    XmlReader XMLread = XmlReader.Create(MainCore.ApplicationStartupDirectory + "\\Settings.xml");
                    XMLread.Read();//Reads the XML definition <XML>
                    XMLread.Read();//Reads the header
                    //check the header
                    if (XMLread.Name == "MyNesSettings")
                    {
                        //check the version
                        if (XMLread.HasAttributes == true)
                        {
                            XMLread.MoveToAttribute("Version");
                            if (XMLread.Value == MainCore.AssemblyVersion)
                            {
                                _Recents = new List<string>();
                                XMLread.Read();//Read the window lable
                                XMLread.Read();//Read the Location lable
                                XMLread.MoveToAttribute("x");
                                _WindowX = Convert.ToInt32(XMLread.Value);
                                XMLread.MoveToAttribute("y");
                                _WindowY = Convert.ToInt32(XMLread.Value);
                                XMLread.Read();//Read the Size lable
                                XMLread.MoveToAttribute("w");
                                _WindowW = Convert.ToInt32(XMLread.Value);
                                XMLread.MoveToAttribute("h");
                                _WindowH = Convert.ToInt32(XMLread.Value);
                                while (XMLread.Read())
                                {
                                    if (XMLread.Name == "DrawMode")
                                    {
                                        XMLread.MoveToAttribute("Val");
                                        _DrawMode = XMLread.Value.ToString();
                                    }
                                    if (XMLread.Name == "StatesFolder")
                                    {
                                        XMLread.MoveToAttribute("Val");
                                        _StatesFolder = XMLread.Value.ToString();
                                    }
                                    if (XMLread.Name == "ShowDebugger")
                                    {
                                        XMLread.MoveToAttribute("Val");
                                        _ShowDebugger = Convert.ToBoolean(XMLread.Value.ToString());
                                    }
                                    if (XMLread.Name == "ShowBrowser")
                                    {
                                        XMLread.MoveToAttribute("Val");
                                        _ShowBrowser = Convert.ToBoolean(XMLread.Value.ToString());
                                    }
                                    if (XMLread.Name == "SpeedThrottling")
                                    {
                                        XMLread.MoveToAttribute("Val");
                                        _SpeedThrottling = Convert.ToBoolean(XMLread.Value.ToString());
                                    }
                                    if (XMLread.Name == "System")
                                    {
                                        XMLread.MoveToAttribute("Val");
                                        _System = (NesSystem)Enum.Parse(typeof(NesSystem), XMLread.Value);
                                    }
                                    if (XMLread.Name == "SoundEnabled")
                                    {
                                        XMLread.MoveToAttribute("Val");
                                        _SoundEnabled = Convert.ToBoolean(XMLread.Value.ToString());
                                    }
                                    if (XMLread.Name == "NoiseEnabled")
                                    {
                                        XMLread.MoveToAttribute("Val");
                                        _NoiseEnabled = Convert.ToBoolean(XMLread.Value.ToString());
                                    }
                                    if (XMLread.Name == "TRIenabled")
                                    {
                                        XMLread.MoveToAttribute("Val");
                                        _TRIenabled = Convert.ToBoolean(XMLread.Value.ToString());
                                    }
                                    if (XMLread.Name == "SQR2enabled")
                                    {
                                        XMLread.MoveToAttribute("Val");
                                        _SQR2enabled = Convert.ToBoolean(XMLread.Value.ToString());
                                    }
                                    if (XMLread.Name == "SQR1enabled")
                                    {
                                        XMLread.MoveToAttribute("Val");
                                        _SQR1enabled = Convert.ToBoolean(XMLread.Value.ToString());
                                    }
                                    if (XMLread.Name == "DMCEnabled")
                                    {
                                        XMLread.MoveToAttribute("Val");
                                        _DMCEnabled = Convert.ToBoolean(XMLread.Value.ToString());
                                    }
                                    if (XMLread.Name == "MasterVolume")
                                    {
                                        XMLread.MoveToAttribute("Val");
                                        _MasterVolume = Convert.ToInt32(XMLread.Value.ToString());
                                    }
                                    if (XMLread.Name == "PalettePath")
                                    {
                                        XMLread.MoveToAttribute("Val");
                                        _PalettePath = XMLread.Value.ToString();
                                    }
                                    if (XMLread.Name == "ScreenshotFormat")
                                    {
                                        XMLread.MoveToAttribute("Val");
                                        _ScreenshotFormat = XMLread.Value.ToString();
                                    }
                                    if (XMLread.Name == "ScreenShotsFolder")
                                    {
                                        XMLread.MoveToAttribute("Val");
                                        _ScreenShotsFolder = XMLread.Value.ToString();
                                    }
                                    if (XMLread.Name == "Rec")
                                    {
                                        XMLread.MoveToAttribute("pat");
                                        _Recents.Add(XMLread.Value.ToString());
                                    }
                                    if (XMLread.Name == "Player1_Down")
                                    {
                                        XMLread.MoveToAttribute("Down");
                                        _Player1_Down = XMLread.Value;
                                    }
                                    if (XMLread.Name == "Player1_Up")
                                    {
                                        XMLread.MoveToAttribute("Up");
                                        _Player1_Up = XMLread.Value;
                                    }
                                    if (XMLread.Name == "Player1_Right")
                                    {
                                        XMLread.MoveToAttribute("Right");
                                        _Player1_Right = XMLread.Value;
                                    }
                                    if (XMLread.Name == "Player1_Left")
                                    {
                                        XMLread.MoveToAttribute("Left");
                                        _Player1_Left = XMLread.Value;
                                    }
                                    if (XMLread.Name == "Player1_Select")
                                    {
                                        XMLread.MoveToAttribute("Select");
                                        _Player1_Select = XMLread.Value;
                                    }
                                    if (XMLread.Name == "Player1_Start")
                                    {
                                        XMLread.MoveToAttribute("Start");
                                        _Player1_Start = XMLread.Value;
                                    }
                                    if (XMLread.Name == "Player1_B")
                                    {
                                        XMLread.MoveToAttribute("B");
                                        _Player1_B = XMLread.Value;
                                    }
                                    if (XMLread.Name == "Player1_A")
                                    {
                                        XMLread.MoveToAttribute("A");
                                        _Player1_A = XMLread.Value;
                                    }

                                    if (XMLread.Name == "Player2_Down")
                                    {
                                        XMLread.MoveToAttribute("Down");
                                        _Player2_Down = XMLread.Value;
                                    }
                                    if (XMLread.Name == "Player2_Up")
                                    {
                                        XMLread.MoveToAttribute("Up");
                                        _Player2_Up = XMLread.Value;
                                    }
                                    if (XMLread.Name == "Player2_Right")
                                    {
                                        XMLread.MoveToAttribute("Right");
                                        _Player2_Right = XMLread.Value;
                                    }
                                    if (XMLread.Name == "Player2_Left")
                                    {
                                        XMLread.MoveToAttribute("Left");
                                        _Player2_Left = XMLread.Value;
                                    }
                                    if (XMLread.Name == "Player2_Select")
                                    {
                                        XMLread.MoveToAttribute("Select");
                                        _Player2_Select = XMLread.Value;
                                    }
                                    if (XMLread.Name == "Player2_Start")
                                    {
                                        XMLread.MoveToAttribute("Start");
                                        _Player2_Start = XMLread.Value;
                                    }
                                    if (XMLread.Name == "Player2_B")
                                    {
                                        XMLread.MoveToAttribute("B");
                                        _Player2_B = XMLread.Value;
                                    }
                                    if (XMLread.Name == "Player2_A")
                                    {
                                        XMLread.MoveToAttribute("A");
                                        _Player2_A = XMLread.Value;
                                    }
                                }
                            }
                        }
                    }
                    XMLread.Close();
                }
            }
            catch (Exception EX) { MainCore.HandleException(EX); }
        }

        /// <summary>
        /// Add recent project path to the recent list
        /// </summary>
        /// <param name="RecentProjectToAdd"></param>
        public void AddRecent(string RecentGame)
        {
            //let's see if this recent file is exists
            for (int i = 0; i < _Recents.Count; i++)
            {
                if (_Recents[i] == RecentGame)
                { _Recents.Remove(RecentGame); i = -1; }
            }
            _Recents.Insert(0, RecentGame);
            //limit the recents to 6 elements
            if (_Recents.Count > 6)
            { _Recents.RemoveAt(6); }
        }
        /// <summary>
        /// Delete recent project from the lis
        /// </summary>
        /// <param name="RecentProjectToDelete">The project path to delete</param>
        public void DeleteRecent(string RecentGame)
        { _Recents.Remove(RecentGame); }
    }
}
