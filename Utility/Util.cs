using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Utility
{
    /// <summary>
    /// Author: John Kirschner
    /// Creation Date: 09-17-2018
    /// Class Purpose:
    ///     The Util class is a static class which will host miscellaneous functionality
    /// </summary>
    public static class Util
    {
        #region Util Variables
        private static RichTextBox rtxt_Stat = null;      //used in combination with rtxtWriteLine, initialize through Init() before calling rtxtWriteLine(), default value of null for error detection
        #endregion Util Variables

        #region public static void Init(RichTextBox rtxt)
        /// <summary>
        /// This method initializes the static Util class
        /// It is advised to call this before calling any other methods
        /// </summary>
        /// <param name="rtxt">A rich text box that will be used in conjuction with rtxtWriteLine()</param>
        public static void Init(RichTextBox rtxt)
        {
            rtxt_Stat = rtxt;       //set the static rich text box
        }
        #endregion public static void Init(RichTextBox rtxt)

        #region public static void rtxtWriteLine(string s_Line)
        /// <summary>
        /// public static void rtxtWriteLine(string s_Line)
        /// This method prints s_Line to a rich text box, will print a new line at the end of the string automatically
        /// </summary>
        /// <param name="s_Line">The line that will be printed to the rich text box</param>
        public static void rtxtWriteLine(string s_Line)
        {
            string s_ClassMethod = "Util.rtxtWriteLine()";
            //first do a sanity check that the rtxtWriteLine is looking a valid text box
            if (rtxt_Stat == null)
            {
                //then an invalid text box is being accessed, this is an issue
                LogManager.WriteLine(s_ClassMethod + " -- rtxt_Stat is not assigned to a valid value. Please call Init() before calling rtxtWriteLine()");
                return;     //return from the method
            }
            //then its fine to print a line of text
            //first check if Invoke is needed (if we are cross thread accessing the text box)
            if (rtxt_Stat.InvokeRequired)
            {
                //then this is a cross thread access
                rtxt_Stat.Invoke((MethodInvoker)delegate
                {
                    WriteToTextBox(s_Line, rtxt_Stat);      //write the line to the text box
                });
            }
            else
            {
                //otherwise this is not a cross thread access, just access the text box normally
                WriteToTextBox(s_Line, rtxt_Stat);     //write the line to the text box
            }
        }
        #endregion public static void rtxtWriteLine(string s_Line)
        #region private static void WriteToTextBox(string s_Line, RichTextBox rtxt)
        /// <summary>
        /// private static void WriteToTextBox(string s_Line, RichTextBox rtxt)
        /// Used to write to a text box, used mainly in rtxtWriteLine() to write where invoking is required
        /// Only used in this class, not accessible outside
        /// </summary>
        /// <param name="s_Line">The line to write</param>
        /// <param name="rtxt">The rich text box to write to</param>
        private static void WriteToTextBox(string s_Line, RichTextBox rtxt_ToWrite)
        {
            rtxt_Stat.Text += s_Line + Environment.NewLine;     //tack on a new line character onto the end of the text box
        }
        #endregion

        #region public static uint CalcDateDifferenceDays(DateTime dt1, DateTime dt2)
        /// <summary>
        /// public static uint CalcDateDifferenceDays(DateTime dt1, DateTime dt2)
        /// Calculates the date difference between two datetimes
        /// Returns the difference in terms of days
        /// Aboslute difference so the return is unsigned and does not signify which DateTime is greater, for that use DateTime.Compare()
        /// </summary>
        /// <param name="dt1">The first date time to calculate the difference for</param>
        /// <param name="dt2">The second date time to calculate the difference for</param>
        /// <returns>An unsigned int indicating the difference in days, absolute value so there is no indication of which date time is greater</returns>
        public static uint CalcDateDifferenceDays(DateTime dt1, DateTime dt2)
        {
            int i_WeightedDays1 = dt1.Year * 365;       //calculate the weighted days for the first date time
            int i_WeightedDays2 = dt2.Year * 365;       //calculate the weighted days for the second date time
            int[] ia_DaysPerMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };     //indicates how many days in each year
            //count the days in the year up to the current month (not including the current month), for the first date time
            for (int i = dt1.Month - 2; i >= 0; i--)
                i_WeightedDays1 += ia_DaysPerMonth[i];      //count the days of the month being examined
            //count the days in the year up to the current month (not including the current month), for the second date time
            for (int i = dt2.Month - 2; i >= 0; i--)
                i_WeightedDays2 += ia_DaysPerMonth[i];      //count the dayso f hte month being examined
            i_WeightedDays1 += dt1.Day;     //add the day onto the end of weighted days 1
            i_WeightedDays2 += dt2.Day;     //add the day onto the end of the weighted days 2
            return ((uint)Math.Abs(i_WeightedDays1 - i_WeightedDays2));     //return the absolute difference between the two dates
        }
        #endregion

        #region public static List<string> DeleteOldFiles(string s_PathToDelete, uint ui_DateDifference)
        /// <summary>
        /// public static List<string> DeleteOldFiles(string s_PathToDelete, uint ui_DateDifference)
        /// Deletes old files in the specified directory
        /// Files older than the ui_DateDifference parameter will be deleted
        /// All of the deleted files will be returned in a string list
        /// Is not recursive, if needed, functionality will be added later
        /// If the target directory does not exist then this method will return
        /// The file last write time is used to determine if the file will be deleted
        /// </summary>
        /// <param name="s_PathToDelete">The path that will be examined to delete old files</param>
        /// <param name="ui_DayDifference">The day difference which indicates when a file should be deleted</param>
        /// <param name="s_SearchPattern">The search pattern of files that will be examined, example: "*.log", defaults to "*" indicating all files will be examined</param>
        /// <returns>A list of files that were deleted from the file system, only the file names, not the path</returns>
        /// <exception cref="NullReferenceException">Thrown whenever a null parameter is passed to this method</exception>
        /// <exception cref="IOException">Can occur when a file is attempted to be deleted, a number of other exceptions can occur, if curious, please review System.IO.FileInfo.Delete()</exception>
        /// <exception cref="ArgumentException">Can be thrown whenever the s_SearchPattern is not correct</exception>
        public static List<string> DeleteOldFiles(string s_PathToDelete, uint ui_DayDifference, string s_SearchPattern = "*")
        {
            List<string> sl_DeletedFiles = new List<string>();      //create a new list which will be returned to the user
            if (s_PathToDelete == null || s_SearchPattern == null)     //perform a null check
                throw new NullReferenceException("Null reference encountered when examining parameters of DeleteOldFiles method");     //then there was a null reference, return an empty path
            DirectoryInfo di_Examine = new DirectoryInfo(s_PathToDelete);       //get the directory info for the folder being examined
            if (!di_Examine.Exists)     //perform an existance check
                return sl_DeletedFiles;     //then nothing needs done, return
            FileInfo[] fia_Files = di_Examine.GetFiles(s_SearchPattern);       //get the files present in this directory
            //iterate through each file that was obtained from the search pattern
            foreach(FileInfo fi in fia_Files)
            {
                uint ui_Days = CalcDateDifferenceDays(fi.LastWriteTime, DateTime.Now);      //calculate the day difference between the last write time of the file and now
                //determine if the day difference has been exceeded
                if (ui_Days > ui_DayDifference)
                {
                    //then the days since the file was last written to are expired, the file may now be deleted
                    sl_DeletedFiles.Add(fi.Name);
                    fi.Delete();        //can throw a number of exceptions which are documented
                }
            }
            return sl_DeletedFiles;     //return the list of deleted files
        }
        #endregion
    }
}
