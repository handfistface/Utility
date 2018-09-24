using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// Class: SqlHelper
    /// Author: John Kirschner
    /// Date: 09-23-2018
    /// Class Purpose:
    ///     This class helps support SQL functionality
    /// </summary>
    public static class SqlHelper
    {
        #region public static bool DoesDBExist(SqlConnection sql_Conn, string s_DatabaseName)
        /// <summary>
        /// Determines if the passed database name actually exists
        /// The passed SqlConnection will be closed after this query
        /// </summary>
        /// <param name="sql_Conn">The sql connection to use to determine if the DB exists</param>
        /// <param name="s_DatabaseName">The name of the DB</param>
        /// <returns>True if the DB exists, false if the DB does not exist</returns>
        public static bool DoesDBExist(SqlConnection sql_Conn, string s_DatabaseName)
        {
            bool b_DoesExist = false;       //indicates whether the DB exists
            //determine if the database name exists
            string s_DBExistance = "SELECT database_id FROM sys.databases WHERE Name = '" + s_DatabaseName + "'";      //create the command used to determine if the DB exists
            //create the sql command used for DB interaction
            using (SqlCommand sql_DBInteration = new SqlCommand(s_DBExistance, sql_Conn))
            {
                object o_QueryRes = sql_DBInteration.ExecuteScalar();       //execute the command and get the response
                int i_DatabaseID = 0;       //used to hold the database's existance
                if (o_QueryRes != null)
                {
                    //then there was a response from the database
                    int.TryParse(o_QueryRes.ToString(), out i_DatabaseID);      //try to parse the data into the database ID
                }
                if (i_DatabaseID > 0)
                    b_DoesExist = true;     //set the return
                sql_DBInteration.Connection.Close();       //close the connectino
            }
            return b_DoesExist;     //return whether the DB exists
        }
        #endregion
    }
}
