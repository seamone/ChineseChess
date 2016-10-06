using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BussinessObjects
{
    public enum LogTyeEnum
    {
        Info= 0,
        Ex =1,
        Debug =2
    }

    /// <summary>
    /// 
    /// </summary>
    public class Log
    {
        public Log(LogTyeEnum theLogType)
        {
            m_logType = theLogType;
        }

        public Log()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public LogTyeEnum LogType
        {
            get
            {
                return m_logType;
            }
            set
            {
                m_logType = value;
            }
        }
        private LogTyeEnum m_logType; 

    }


    public class LogHandler
    {
         //single instance pattern
        public static LogHandler DefaultLogHandler()
        {
            if (m_LogHandler == null)
            {
                m_LogHandler = new LogHandler();
            }
            return m_LogHandler;
        }
        private static LogHandler m_LogHandler;

        #region "Constructor"
        //constructor is private
        private LogHandler()
        {

        }
        //constructor is private
        public LogHandler(string path,string logFileName)
        {
            this._path = path;
            this._logFileName = logFileName;
            logging("Starting");
        }
        #endregion
       
        private string _path;

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
        private string _logFileName;

        public string LogFileName
        {
            get { return _logFileName; }
            set { _logFileName = value; }
        }

        public void logging(string text)
        {
            text = DateTime.Now.ToString() + 
                Environment.NewLine + 
                text;
           
            createOrUpdateLog(text);
        }
        private string getFullPathName()
        {
            string fullPathName = "";
            if (string.IsNullOrEmpty(Path))
            {
                Path = System.Environment.CurrentDirectory;
            }
            if (string.IsNullOrEmpty(LogFileName))
            {
                LogFileName = "Log";
            }
            string currentTime = DateTime.Now.ToString("yyyyMMddHH");
            fullPathName = Path + "\\" + LogFileName + currentTime + ".log";
            return fullPathName;
        }
        public void createOrUpdateLog(string text)
        {
            string FILE_NAME = getFullPathName();
            StreamWriter sr;
            if (File.Exists(FILE_NAME)) //如果文件存在,则创建File.AppendText对象
            {
                sr = File.AppendText(FILE_NAME);
            }
            else   //如果文件不存在,则创建File.CreateText对象
            {
                sr = File.CreateText(FILE_NAME);
            }
            sr.WriteLine(text);
            sr.Close();
        }

    }
}
