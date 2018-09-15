
//
// Console app to create the database
// (or any Database) to be used in Coursemo
// by Daniel Boboc
// 
// Reads the file in the Debug folder named "DDL.sql"
// and creates an MSSQL Database named "DDL.mdf"
// After making the database, it populates it with the
// data from the .csv files also in the debug folder
//
//*
//  Instructions: Edit DDL.txt, store it in DDL.sql
//  Run the program. In the directory, a DDL.mdf should
//  be created. Finally, move the "DDL.mdf" file into
//  Coursemo's Debug folder before running it.
//*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateDBApp
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("** Create Database Console App **");
            Console.WriteLine();

            string baseDatabaseName = "DDL";
            string sql;

            try
            {
                //
                // 1. Make a copy of empty MDF file to get us started:
                //
                Console.WriteLine("Copying empty database to {0}.mdf and {0}_log.ldf...", baseDatabaseName);

                CopyEmptyFile("__EmptyDB", baseDatabaseName);

                Console.WriteLine();

                //
                // 2. Now let's make sure we can connect to SQL Server on local machine:
                //
                DataAccessTier.Data data = new DataAccessTier.Data(baseDatabaseName + ".mdf");

                Console.Write("Testing access to database: ");

                if (data.TestConnection())
                    Console.WriteLine("success");
                else
                    Console.WriteLine("failure?!");

                Console.WriteLine();

                //
                // 3. Create tables by reading from .sql file and executing DDL queries:
                //
                Console.WriteLine("Creating tables by executing {0}.sql file...", baseDatabaseName);

                string[] lines = System.IO.File.ReadAllLines(baseDatabaseName + ".sql");

                sql = "";

                for (int i = 0; i < lines.Length; ++i)
                {
                    string next = lines[i];

                    if (next.Trim() == "")  // empty line, ignore...
                    {
                    }
                    else if (next.Contains(";"))  // we have found the end of the query:
                    {
                        sql = sql + next + System.Environment.NewLine;

                        Console.WriteLine("** Executing '{0}'...", sql.Substring(0, 32));

                        data.ExecuteActionQuery(sql);

                        sql = "";  // reset:
                    }
                    else  // add to existing query:
                    {
                        sql = sql + next + System.Environment.NewLine;
                    }
                }

                Console.WriteLine();

                //
                // 4. Insert data by parsing data from .csv files: (WHAT I ACTUALLY DID)
                //
                Console.WriteLine("Inserting data...");

                // NOTE, i DONT HAVE TO INSERT THE PRIMARY KEY VALUES, IGNORE IT.
                //
                // first parse the courses

                using (var file = new System.IO.StreamReader("courses.csv"))
                {
                    while (!file.EndOfStream)
                    {
                        string   line = file.ReadLine();
                        string[] values = line.Split(',');
                        string   Dept = values[0];
                        string   CourseNumber = values[1];
                        string   Semester = values[2];
                        string   Year = values[3];
                        string   CRN = values[4];
                        string   Type = values[5];
                        string   Day = values[6];
                        string   Time = values[7];
                        string   Size = values[8];
                        sql = string.Format(@"Insert Into
                                              COURSE_INFO(Dept,CourseNumber, Semester, Year, CRN, Type, Day, Time, Size, Enrolled)
                                              Values('{0}',{1}, '{2}', {3}, '{4}', '{5}', '{6}', '{7}', {8}, 0);", Dept, CourseNumber, Semester, Year, CRN, Type, Day, Time, Size);
                        // letting the database handle CID
                        data.ExecuteActionQuery(sql);
                    }
                }
                // next the students
                using (var file = new System.IO.StreamReader("students.csv"))
                {
                    while (!file.EndOfStream)
                    {
                        string line = file.ReadLine();
                        string[] values = line.Split(',');
                        string LAST = values[0];
                        string FIRST = values[1];
                        string NetID = values[2];
                        LAST = LAST.Replace("'", @"''");
                        FIRST = FIRST.Replace("'", @"''");
                        sql = string.Format(@"INSERT INTO 
                                              STUDENT(FirstName,LastName, NetID)
                                              Values('{0}', '{1}', '{2}');", FIRST, LAST, NetID);
                        data.ExecuteActionQuery(sql);
                    }
                }

                Console.WriteLine();

                //
                // Done
                //
            }
            catch (Exception ex)
            {
                Console.WriteLine("**Exception: '{0}'", ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine("** Done **");
            Console.WriteLine();
        }//Main


        /// <summary>
        /// Makes a copy of an existing Microsoft SQL Server database file 
        /// and log file.  Throws an exception if an error occurs, otherwise
        /// returns normally upon successful copying.  Assumes files are in
        /// sub-folder bin\Debug or bin\Release --- i.e. same folder as .exe.
        /// </summary>
        /// <param name="basenameFrom">base file name to copy from</param>
        /// <param name="basenameTo">base file name to copy to</param>
        static void CopyEmptyFile(string basenameFrom, string basenameTo)
        {
            string from_file, to_file;

            //
            // copy .mdf:
            //
            from_file = basenameFrom + ".mdf";
            to_file = basenameTo + ".mdf";

            if (System.IO.File.Exists(to_file)) // check if the .mdf (database) already exists
            {
                System.IO.File.Delete(to_file); // if it does exist, delete it.
            }

            System.IO.File.Copy(from_file, to_file);

            // 
            // now copy .ldf:
            //
            from_file = basenameFrom + "_log.ldf";
            to_file = basenameTo + "_log.ldf";

            if (System.IO.File.Exists(to_file))
            {
                System.IO.File.Delete(to_file);
            }

            System.IO.File.Copy(from_file, to_file);
        }

    }//class
}//namespace


