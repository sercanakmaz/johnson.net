using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JohnsonNet.Operation;
using JohnsonNet.Config;
using System.Configuration;
using System.Web;
using JohnsonNet.Data;
using JohnsonNet.Serialization;

namespace JohnsonNet
{
    public static class JohnsonManager
    {
        private static MultiThreadOperation p_MultiThread = null;
        public static MultiThreadOperation MultiThread
        {
            get
            {
                if (p_MultiThread == null)
                    p_MultiThread = new MultiThreadOperation();

                return p_MultiThread;
            }
            set
            {
                p_MultiThread = value;
            }
        }

        private static ReflectionOperation p_Reflection = null;
        public static ReflectionOperation Reflection
        {
            get
            {
                if (p_Reflection == null)
                    p_Reflection = new ReflectionOperation();

                return p_Reflection;
            }
            set
            {
                p_Reflection = value;
            }
        }

        private static ConvertOperation p_Convert = null;
        public static ConvertOperation Convert
        {
            get
            {
                if (p_Convert == null)
                    p_Convert = new ConvertOperation();

                return p_Convert;
            }
            set
            {
                p_Convert = value;
            }
        }

        private static LogOperation p_Logger = null;
        public static LogOperation Logger
        {
            get
            {
                if (p_Logger == null)
                    p_Logger = new LogOperation();

                return p_Logger;
            }
            set
            {
                p_Logger = value;
            }
        }

        private static MailOperation p_Mail = null;
        public static MailOperation Mail
        {
            get
            {
                if (p_Mail == null)
                    p_Mail = new MailOperation();

                return p_Mail;
            }
            set
            {
                p_Mail = value;
            }
        }

        private static IOOperation p_IO = null;
        public static IOOperation IO
        {
            get
            {
                if (p_IO == null)
                    p_IO = new IOOperation();

                return p_IO;
            }
            set
            {
                p_IO = value;
            }
        }

        private static DataOperation p_Data = null;
        public static DataOperation Data
        {
            get
            {
                if (p_Data == null)
                    p_Data = new DataOperation();

                return p_Data;
            }
        }

        #region Serialization
        private static JsonSerializer p_Json = null;
        public static JsonSerializer Json
        {
            get
            {
                if (p_Json == null)
                {
                    p_Json = new JsonSerializer
                    {
                        ConvertDateToUnixTimeStamp = true,
                        ConvertEnumToString = true,
                        ToLowerCase = true
                    };

                    p_Json.Settings.Converters.Add(new AbsoluteUriConverter());
                }
                return p_Json;
            }
        }

        private static XmlSerializer p_Xml = null;
        public static XmlSerializer Xml
        {
            get
            {
                if (p_Xml == null) p_Xml = new XmlSerializer();
                return p_Xml;
            }
        }
        #endregion

        #region Config

        private static ConfigOperation p_Config = null;
        public static ConfigOperation Config
        {
            get
            {
                if (p_Config == null)
                    p_Config = new ConfigOperation();

                return p_Config;
            }
        }

        #endregion
    }
}
