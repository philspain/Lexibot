using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace LexibotData.DataAccess
{
    public class Logger : LexibotData.DataAccess.ILogger
    {
        // Path for directory that will contain logs
        static readonly string _dirName;

        // Path for file to log errors
        static readonly string _errorFile;


        static Logger()
        {
            string baseDir = Path.GetDirectoryName(
                Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory));
            _dirName = Path.Combine(baseDir, "logs");
            _errorFile = Path.Combine(_dirName, "error-log.txt");
        }

        /// <summary>
        /// Attempt to append error message to log file.
        /// </summary>
        /// <param name="error">Error message to be added.</param>
        public void LogError(string error)
        {
            CheckDirectoryExists();
            CheckFileExists();

            using (FileStream fs = File.Open(_errorFile, FileMode.Append))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    string message = "\r\n------ Begin Error " + DateTime.Now +  ": ------\r\n" +
                        error +
                        "\r\n------ End Error ------\r\n";

                    sw.Write(message);
                }
            }
        }

        /// <summary>
        /// Check if directory for logs existts.
        /// </summary>
        private void CheckDirectoryExists()
        {
            if (!Directory.Exists(_dirName))
            {
                try
                {
                    Directory.CreateDirectory(_dirName);
                }
                catch { }
            }
        }

        /// <summary>
        /// Check if log file exists, create if it doesn't
        /// </summary>
        private void CheckFileExists()
        {
            if (!File.Exists(_errorFile))
            {
                    FileStream fs = File.Create(_errorFile);
                    fs.Close();
            }
        }
    }
}
