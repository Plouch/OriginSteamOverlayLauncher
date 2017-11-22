﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

// Change this to match your program's normal namespace
namespace OriginSteamOverlayLauncher
{
    class IniFile   // revision 12
    {// INI Support, courtesy of: https://stackoverflow.com/questions/217902/reading-writing-an-ini-file
        public string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);
        
        public IniFile(string IniPath = null)
        {
            Path = new FileInfo(IniPath ?? EXE + ".ini").FullName.ToString();
        }

        public string ReadString(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        public bool ReadBool(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
            Boolean.TryParse(RetVal.ToString(), out bool _output);
            return _output;
        }

        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? EXE);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? EXE);
        }

        public bool KeyPopulated(string Key, string Section = null)
        {
            return ReadString(Key, Section).Length > 0;
        }

        public bool KeyExists(string Key)
        {
            var element = File.ReadLines(this.Path)
                        .SkipWhile(line => !line.Contains(Key + "="))
                        .TakeWhile(line => line.Contains(Key + "="))
                        .FirstOrDefault();

            if (element != null && element.Length > 0)
                return true;
            else
                return false;
        }
    }
}