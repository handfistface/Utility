using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// Class: TableEntry
    /// Author: John Kirschner
    /// Date: 09-24-2018
    /// Class Purpose:
    ///     This class describes a table entry for building a new table
    /// </summary>
    public class TableEntry
    {
        #region TableEntry Variables
        private string s_ColType;        //the type of colum, must be of the enum ColTypes when setting
        private string s_Colname;        //the name of the column to access it
        #endregion

        #region public TableEntry(ColTypes coltype, string colname)
        /// <summary>
        /// Constructor for the TableEntry class
        /// Sets up the column type and the column name
        /// </summary>
        /// <param name="coltype">The column type definition, must be the enum</param>
        /// <param name="colname">The name of the column which will be accessible through the DB</param>
        public TableEntry(ColTypes coltype, string colname)
        {
            SetColType(coltype);        //set the column type
            s_Colname = colname;        //set the column name
        }
        #endregion

        #region public SetColType(ColTypes coltype)
        /// <summary>
        /// Sets the column type
        /// Will setup the class variable based on the enum name
        /// </summary>
        /// <param name="coltype">The type of colum to setup this class with</param>
        public void SetColType(ColTypes coltype)
        {
            switch(coltype)
            {
                case ColTypes.VarChar:
                    s_ColType = "VARCHAR(255)";
                    break;
                case ColTypes.Int:
                    s_ColType = "INT(10)";
                    break;
                case ColTypes.Double:
                    s_ColType = "DOUBLE(10, 2)";
                    break;
                case ColTypes.Date:
                    s_ColType = "DATE()";
                    break;
            }
        }
        #endregion

        #region enum ColTypes
        /// <summary>
        /// Describes the different data types available for the sql server
        /// </summary>
        public enum ColTypes
        {
            VarChar, //variables length string, defaults to 255
            Int, //integer, 4 bytes
            Double, //double, max size and decimal point may be defined
            Date, //date in the format YYYY-MM-DD
        }
        #endregion
    }
}
