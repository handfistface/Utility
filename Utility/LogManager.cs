using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// Author: John Kirschner
    /// Date: 09-17-2018
    /// Class Purpose:
    ///     The LogManager class handles managing all of the logs of the application
    ///     Static is used because there can only ever be one instance of LogManager per application
    ///     Logging is thread safe
    ///     Logs are created in the AppData folder for the current user
    ///     Logs are also created based on the day
    ///     Whenever the Init() method is run old logs will be cleaned out
    ///     Not to be used on windows less than Vista (works off the AppData location), based off the C:\ drive
    /// </summary>
    public static class LogManager
    {
        #region LogManager Variables
        private static string s_RecordPath = null;     //the path which represents the recording path, null indicates that the class has not had Init() called
        private static object o_WriteLock = new object();       //used to lock the class while writing
        #endregion

        #region public static void Init(string s_AppDataLocalFolder)
        /// <summary>
        /// public static void Init(string s_AppDataLocalFolder)
        /// This is used to setup the static class
        /// Usually run on program start
        /// All logs will be recorded in the current user's AppData/Local/ folder
        /// If the folder does not exist then it will be created
        /// Deletes old txt files in the target directory older than 30 days
        /// </summary>
        /// <param name="s_AppDataLocalFolder">The folder which the logs will be created and written to, example: "UtilityProgram" will create the folder "UtilityProgram" in the 
        ///     current user's AppData\Local folder, forward and back slash characters will be removed</param>
        /// <exception cref="IOException">Creating a directory can throw an IOException</exception>
        public static void Init(string s_AppDataLocalFolder)
        {
            string s_ClassMethod = "LogManager.Init()";
            s_AppDataLocalFolder = s_AppDataLocalFolder.Replace("/", "");       //replace all forward slash characters
            s_AppDataLocalFolder = s_AppDataLocalFolder.Replace("\\", "");      //replace all back slash characters
            s_RecordPath = "C:\\Users\\" + Environment.UserName + "\\Local\\" + s_AppDataLocalFolder + "\\";       //set the record path
            DirectoryInfo di = new DirectoryInfo(s_RecordPath);     //get directory information about the directory that is going to be created
            //does the target directory exist?
            if(!di.Exists)
            {
                //then the directory does not exist
                di.Create();        //create the new directory
            }
            //delete any old files
            List<string> sl_FilesDeleted = Util.DeleteOldFiles(s_RecordPath, 30, "*.txt");     //delete old log files
            foreach (string s_File in sl_FilesDeleted)
                WriteLine(s_ClassMethod + " -- Deleted file: [" + s_File + "]");
            WriteLine(s_ClassMethod + " -- Successfully initialized the LogManager class");
        }
        #endregion

        #region public static void WriteLine(string s_Line)
        /// <summary>
        /// public static void WriteLine(string s_Line)
        /// Writes a line of text to an output file based on the record path and the current date
        /// The file name will be based off the date and the recording path, will be a .txt file
        /// Kind of inefficient since the file name has to be recalcuated everytime this method runs, performance hasn't been an issue as of yet
        /// Is thread safe
        /// </summary>
        /// <param name="s_Line">The line of text to write to the file, new line should not be added to the end of this line</param>
        /// <exception cref="NullReferenceException">Thrown whenever the recording path is set to null indicating that the user needs to call Init() prior to executing this method</exception>
        public static void WriteLine(string s_Line)
        {
            if (s_RecordPath == null)
                throw new NullReferenceException("Recording path is null. Please call Init() before trying to perform any logging");
            //lock the write to be thread safe
            lock(o_WriteLock)
            {
                string s_FileName = s_RecordPath + GetDate() + ".txt";      //get the file name of the file to record to
                FileInfo fi_ToWrite = new FileInfo(s_FileName);     //get the fileinfo object associated with the path
                                                                    //determine if the new incoming file exists
                if (!fi_ToWrite.Exists)
                {
                    fi_ToWrite.Create();        //create this new file to write to
                }
                //using the stream writer make sure to specify that appending to the file will be enabled
                using (StreamWriter sw = new StreamWriter(s_FileName, true))
                {
                    sw.WriteLine(s_Line);       //record the line
                    sw.Close();     //close the stream writer
                }
            }
        }
        #endregion

        #region public static string GetDate()
        /// <summary>
        /// public static string GetDate()
        /// Gets the current date and returns it in a string format
        /// The return format will be in the following format:
        ///     YYYY_MM_DD
        /// Both MM and DD will be padded with 0 if they are less than 9
        /// </summary>
        /// <returns>A string indicating the current date in the format YYYY_MM_DD</returns>
        public static string GetDate()
        {
            DateTime dt_Now = DateTime.Now;     //get the current date time
            string s_Date = dt_Now.Year + "_" + dt_Now.Month.ToString("00") + "_" + dt_Now.Day.ToString("00");
            return s_Date;      //return the date string that was just created
        }
        #endregion
        #region public static string GetTime()
        /// <summary>
        /// public static string GetTime()
        /// Gets the current time and returns it in a string format
        /// The return format will be in the following format:
        ///     HH_MM_SS
        /// All three values will be padded with 0 if necessary
        /// </summary>
        /// <returns>A string indicating the current time in the format HH_MM_SS</returns>
        public static string GetTime()
        {
            DateTime dt_Now = DateTime.Now;     //get the current date time
            string s_Time = dt_Now.Hour.ToString("00") + "_" + dt_Now.Minute.ToString("00") + "_" + dt_Now.Second.ToString("00");
            return s_Time;      //return the time string
        }
        #endregion
    }
}
