using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//
// Mult-user Coursemo(inspired from Venmo) application for UIC course registrations.
// Written in MSSQL & C#
// Multi-user functionality brought by Transactions.
// by Daniel Boboc
// 

namespace Project2
{
    public partial class Form1 : Form
    {
        string connectionInfo;
        string localInfo;
        string remoteInfo;

        public Form1()
        {
            InitializeComponent();
            this.textBox1.Text = "DDL.mdf"; // SO THAT THERE IS A DEFAULT DATABASE, USER DOESNT HAVE TO ENTER IT EVERY TIME
        }

        private bool fileExists(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                string msg = string.Format("Input Database file not found: '{0}'",
                  filename);

                MessageBox.Show(msg);
                return false;
            }

            // exists!
            return true;
        }
        

        ///////////// BEGIN OLD PROGRAM 3 HERE.
        ///////////// BEGIN OLD PROGRAM 3 HERE.
        ///////////// BEGIN OLD PROGRAM 3 HERE.
        ///////////// BEGIN OLD PROGRAM 3 HERE.
        ///////////// BEGIN OLD PROGRAM 3 HERE.
        ///////////// BEGIN OLD PROGRAM 3 HERE.
        ///////////// BEGIN OLD PROGRAM 3 HERE.
        ///////////// BEGIN OLD PROGRAM 3 HERE.
        ///////////// BEGIN OLD PROGRAM 3 HERE.
        ///////////// BEGIN OLD PROGRAM 3 HERE.

        // #1
        private void button1_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear(); // clear the listbox

            string filename = this.textBox1.Text;  // GRABS THE INPUTTED DB FILENAME
            if (!fileExists(filename))
            {            // checks if the database file is valid
                return;
            }

            DataAccessTier.Data data = new DataAccessTier.Data(filename);
            string sql = @"SELECT FirstName, LastName " +
                          "FROM CUSTOMER WITH(INDEX(NAME_INDEX)) " +
                          "ORDER BY LastName, FirstName;";
            string name;
            DataSet ALLNAMES = data.ExecuteNonScalarQuery(sql);
            foreach (DataTable table in ALLNAMES.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (row != null)
                    {
                        name = row["FirstName"].ToString() + " " + row["LastName"].ToString();
                        this.listBox1.Items.Add(name);
                    }
                }
            }
        }

        // #2
        // grab a customer and display their info
        // (but check first if something is selected from the listbox)
        private void button2_Click(object sender, EventArgs e)
        {
            string filename = this.textBox1.Text;  // GRABS THE INPUTTED DB FILENAME
            if (!fileExists(filename))
            {            // checks if the database file is valid
                return;
            }
            DataAccessTier.Data data = new DataAccessTier.Data(filename);

            string userInput = this.textBox2.Text; // grab user input
            string FIRST, LAST, bikeCount, expected, sql, output;
            if (userInput != "") { // SOMETHING ENTERED 
                this.listBox1.Items.Clear(); // clear the listbox
                // split up the input into first and last name
                string[] NAME = this.textBox2.ToString().Split(' '); // ' ' is deliminator
                if ((NAME.Length) != 4) // 4 inputs grabbed from textbox for some reason
                {
                    MessageBox.Show("Enter a First and last name separated by a space please.");
                    return;
                }
                FIRST = NAME[2];
                LAST = NAME[3];
                FIRST = FIRST.Replace("'", @"''");
                LAST = LAST.Replace("'", @"''");
                sql = String.Format(@"SELECT CID, Email, WithRental
                                      FROM CUSTOMER WITH(INDEX(NAME_INDEX))
                                      WHERE FirstName = '{0}' AND
                                            LastName = '{1}';", FIRST, LAST);
                DataSet INFO = data.ExecuteNonScalarQuery(sql);
                // should only be one row result
                foreach (DataTable table in INFO.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (row != null)
                        {
                            output = String.Format(@"Customer: {0} {1}, email: ", FIRST, LAST);
                            output += row["Email"];
                            this.listBox1.Items.Add(output); // part 1
                            output = "Customer is ";
                            if (row["WithRental"].ToString() == "False") // check if not with a rental
                                output += "not with a rental\n";
                            else
                            {
                                output += "with a rental\n";
                                // renting a bike, so another query is necessary.
                                sql = String.Format(@"SELECT Count(*) AS count, MAX(ExpectedReturn) AS expected
                                                      FROM HISTORY
                                                      INNER JOIN CUSTOMER ON CUSTOMER.CID = HISTORY.CID
                                                      WHERE FirstName = '{0}' AND
                                                      LastName = '{1}';", FIRST, LAST);

                                DataSet INFO2 = data.ExecuteNonScalarQuery(sql);

                                // grab the amount of bikes and expected return time from table
                                foreach (DataTable table2 in INFO2.Tables)
                                {
                                    foreach (DataRow row2 in table2.Rows)
                                    {
                                        bikeCount = row2["count"].ToString();
                                        expected = row2["expected"].ToString();
                                        output += string.Format(@" this customer has {0} bikes rented", bikeCount);
                                        this.listBox1.Items.Add(output); // part 1
                                        output = "";
                                        output += string.Format(@"Expected return time is {0}", expected);
                                    }
                                }
                            }
                            this.listBox1.Items.Add(output);
                        }
                        else
                        { // if the row is null (person isn't found)
                            this.listBox1.Items.Add("Invalid name, customer not found.");
                        }
                    }
                }
                return; // this gets hit if it searches for a selected item
            }

            // IF THE USER SELECTS A PERSON INSTEAD OF SEARCHING FOR THEM
            if (this.listBox1.SelectedIndex != -1)
            {
                string[] NAME = this.listBox1.SelectedItem.ToString().Split(' '); // ' ' is deliminator

                this.listBox1.Items.Clear(); // clear the listbox

                if ((NAME.Length) != 2) // 2 inputs grabbed from listbox
                {
                    MessageBox.Show("Enter a First and last name separated by a space please.");
                    return;
                }
                FIRST = NAME[0];
                LAST = NAME[1];
                FIRST = FIRST.Replace("'", @"''"); 
                LAST = LAST.Replace("'", @"''"); 
                sql = String.Format(@"SELECT CID, Email, WithRental
                                      FROM CUSTOMER WITH(INDEX(NAME_INDEX))
                                      WHERE FirstName = '{0}' AND
                                            LastName = '{1}';", FIRST, LAST);
                DataSet INFO = data.ExecuteNonScalarQuery(sql);
                // should only be one row result
                foreach (DataTable table in INFO.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (row != null)
                        {
                            output = String.Format(@"Customer: {0} {1}, email: ", FIRST, LAST);
                            output += row["Email"];
                            this.listBox1.Items.Add(output); // part 1
                            output = "Customer is ";
                            if (row["WithRental"].ToString() == "False") // check if not with a rental
                                output += "not with a rental\n";
                            else
                            {
                                output += "with a rental\n";
                                // renting a bike, so another query is necessary.
                                sql = String.Format(@"SELECT Count(*) AS count, MAX(ExpectedReturn) AS expected
                                                      FROM HISTORY
                                                      INNER JOIN CUSTOMER ON CUSTOMER.CID = HISTORY.CID
                                                      WHERE FirstName = '{0}' AND
                                                      LastName = '{1}' AND ChargedYet = 0;", FIRST, LAST);

                                DataSet INFO2 = data.ExecuteNonScalarQuery(sql);

                                // grab the amount of bikes and expected return time from table
                                foreach (DataTable table2 in INFO2.Tables)
                                {
                                    foreach (DataRow row2 in table2.Rows)
                                    {
                                        bikeCount = row2["count"].ToString();
                                        expected = row2["expected"].ToString();
                                        output += string.Format(@" this customer has {0} bikes rented", bikeCount);
                                        this.listBox1.Items.Add(output); // part 1
                                        output = "";
                                        output += string.Format(@"Expected return time is {0}", expected);
                                    }
                                }
                            }
                            this.listBox1.Items.Add(output);
                        }
                        else
                        { // if the row is null (person isn't found)
                            this.listBox1.Items.Add("Invalid name, customer not found.");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("ENTER SOMETHING or SELECT from all NAMES and try again");
            }
            return;
        }

        // #3
        // display all bikes ordered by BID
        private void button3_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear(); // clear the listbox
            string filename = this.textBox1.Text;  // GRABS THE INPUTTED DB FILENAME
            if (!fileExists(filename))
            {            // checks if the database file is valid
                return;
            }

            string sql, BID, TID, YEAR, CHECKED, output = "";

            DataAccessTier.Data data = new DataAccessTier.Data(filename);
            sql = @"SELECT * FROM BIKE
                    ORDER BY BID;";

            DataSet INFO = data.ExecuteNonScalarQuery(sql);
            foreach (DataTable table in INFO.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (row != null)
                    {
                        BID = row["BID"].ToString();
                        TID = row["TID"].ToString();
                        YEAR = row["YearPub"].ToString();
                        CHECKED = row["IsItCheckedOut"].ToString();
                        output = string.Format("BID: {0}, TID: {1}, Year: {2}, CHECKED OUT: {3}\n", BID, TID, YEAR, CHECKED);
                        this.listBox1.Items.Add(output);
                    }
                    output = "";
                }
            }

        }

        // #4
        //
        //Select a bike from the list (or provide a search feature) to display bike’s year, type (the description, not the type
        // id), and the rental price per hour.Also display whether the bike is currently out on a rental; if so, display the
        //expected return date and time.
        private void button4_Click(object sender, EventArgs e)
        {
            string filename = this.textBox1.Text;  // GRABS THE INPUTTED DB FILENAME
            if (!fileExists(filename)){ // checks if the database file is valid
                return;
            }
            DataAccessTier.Data data = new DataAccessTier.Data(filename);

            string userInput = this.textBox2.Text; // grab user input
            string YEAR, DESCRIPT, PRICE, CHECKED, sql, output = "", input;
            int BID = -1, COUNT = 0;

            // IF THE USER SEARCHES FOR A BIKE
            if (userInput != ""){ // SOMETHING ENTERED 
                this.listBox1.Items.Clear(); // clear the listbox

                //check input to see if any letters are in it.
                input = this.textBox2.Text.ToString();
                if (input.Length != 4)
                {
                    MessageBox.Show("4 digit BID number please.");
                    return;
                }

                foreach (var letter in input)
                {
                    if (Char.IsLetter(letter))
                    {
                        MessageBox.Show("ENTER A BID, no letters please.");
                        return;
                    }
                }
                BID = Int32.Parse(input);  // grab the BID number from the input

                sql = String.Format(@"SELECT YearPub, Descript, Price, IsItCheckedOut
                                      FROM BIKE 
                                      INNER JOIN BIKE_TYPE ON BIKE.TID = BIKE_TYPE.TID
                                      WHERE BIKE.BID = {0} ", BID);
                DataSet INFO = data.ExecuteNonScalarQuery(sql);
                // should only be one row result
                foreach (DataTable table in INFO.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (row != null)
                        {
                            YEAR = row["YearPub"].ToString();
                            DESCRIPT = row["Descript"].ToString();
                            PRICE = row["Price"].ToString();
                            CHECKED = row["IsItCheckedOut"].ToString();
                            output = string.Format(@"#Bike: {0}", BID);
                            this.listBox1.Items.Add(output);
                            output = string.Format(@"Released in: {0}", YEAR);
                            this.listBox1.Items.Add(output);
                            output = string.Format(@"price: ${0}", PRICE);
                            this.listBox1.Items.Add(output);
                            output = string.Format(@"description: {0}", DESCRIPT);
                            this.listBox1.Items.Add(output);
                            if (CHECKED == "True")
                            {
                                sql = string.Format(@"SELECT ExpectedReturn FROM History WHERE BID = {0}", BID);
                                object INFO2 = data.ExecuteScalarQuery(sql);
                                if (INFO2 == null){ // should never happen
                                    MessageBox.Show("ERROR, EXPECTED RETURN NOT FOUND!");
                                    return;
                                }
                                output = @"This bike IS checked out and is expected to be back at:";
                                this.listBox1.Items.Add(output);
                                output = INFO2.ToString();
                                this.listBox1.Items.Add(output);
                            }
                            else
                            {
                                output = @"This bike is NOT checked out.";
                                this.listBox1.Items.Add(output);
                            }
                        }
                        COUNT++;
                    }
                }
                if (COUNT == 0)
                    this.listBox1.Items.Add("Bike not found.");
                return;
            }


            string []INPUT; // array to put the spit words into
            // IF THE USER SELECTS A PERSON INSTEAD OF SEARCHING FOR THEM
            if (this.listBox1.SelectedIndex != -1)
            {
                INPUT = this.listBox1.SelectedItem.ToString().Split(' '); // grab the selected item from the list
                this.listBox1.Items.Clear(); // clear the listbox


                BID = Int32.Parse((INPUT[1].TrimEnd(',')));  // grab the BID number from the input, trim the comma off the last char

                sql = String.Format(@"SELECT YearPub, Descript, Price, IsItCheckedOut
                                      FROM BIKE 
                                      INNER JOIN BIKE_TYPE ON BIKE.TID = BIKE_TYPE.TID
                                      WHERE BIKE.BID = {0};", BID);
                DataSet INFO = data.ExecuteNonScalarQuery(sql);
                // should only be one row result
                foreach (DataTable table in INFO.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (row != null)
                        {
                            YEAR = row["YearPub"].ToString();
                            DESCRIPT = row["Descript"].ToString();
                            PRICE = row["Price"].ToString();
                            CHECKED = row["IsItCheckedOut"].ToString();
                            output = string.Format(@"#Bike: {0}", BID);
                            this.listBox1.Items.Add(output);
                            output = string.Format(@"Released in: {0}", YEAR);
                            this.listBox1.Items.Add(output);
                            output = string.Format(@"price: ${0}", PRICE);
                            this.listBox1.Items.Add(output);
                            output = string.Format(@"description: {0}", DESCRIPT);
                            this.listBox1.Items.Add(output);
                            if (CHECKED == "True")
                            {
                                sql = string.Format(@"SELECT ExpectedReturn FROM History WHERE BID = {0}", BID);
                                object INFO2 = data.ExecuteScalarQuery(sql);
                                if (INFO2 == null)
                                { // should never happen
                                    MessageBox.Show("ERROR, EXPECTED RETURN NOT FOUND!");
                                    return;
                                }
                                output = @"This bike IS checked out and is expected to be back at:";
                                this.listBox1.Items.Add(output);
                                output = INFO2.ToString();
                                this.listBox1.Items.Add(output);
                            }
                            else
                            {
                                output = @"This bike is NOT checked out.";
                                this.listBox1.Items.Add(output);
                            }
                        }
                        COUNT++;
                    }
                }
                
                if (COUNT == 0) // this one should never happen, since we're literally searching for a selected bike.
                    this.listBox1.Items.Add("Bike not found.");

            }
            else { //  this gets executed if nothing is entered AND nothing is selected
                this.listBox1.Items.Clear(); // clear the listbox
                this.listBox1.Items.Add("Enter something in search bar OR select from ");
                this.listBox1.Items.Add("the list after clicking #3 then click #4 again.");
                return;
            }
            
        }

        // #5, display available bikes
        private void button5_Click(object sender, EventArgs e){
            this.listBox1.Items.Clear(); // clear the listbox

            string filename = this.textBox1.Text;  // GRABS THE DB FILENAME
            if (!fileExists(filename)){            // checks if the database file is valid
                return;
            }

            DataAccessTier.Data data = new DataAccessTier.Data(filename);
            string sql = @"SELECT BID, TID, YearPub, IsItCheckedOut " +
                          "FROM BIKE " +
                          "ORDER BY TID, YearPub DESC;";

            DataSet BIKES = data.ExecuteNonScalarQuery(sql);

            string BID, TID, YEAR, CHECKED, output;

            foreach (DataTable table in BIKES.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (row != null)
                    {
                        BID = row["BID"].ToString();
                        TID = row["TID"].ToString();
                        YEAR = row["YearPub"].ToString();
                        CHECKED = row["IsItCheckedOut"].ToString();
                        output = string.Format("BID: {0}, TID: {1}, Year: {2}, CHECKED OUT: {3}\n", BID, TID, YEAR, CHECKED);
                        this.listBox1.Items.Add(output);
                    }
                    output = "";
                }
            }

        }

        // #6
        // Allow customer C to rent N > 0 bikes for H hours; the hours can be a real number, e.g. 3.5 hours. The UI can be
        // as simple as selecting a customer and then inputting a set of bike ids into a text box, but the app must confirm
        // that(a) the customer exists, (b) the customer is not already out on a rental, and(c) each of the bikes is available
        // for rent.These conditions should be checked before the rental is entered into the database.If the rental is
        // successful, display the rental ID (MessageBox.Show is fine).
        // A CUSTOMER CAN ONLY HAVE ONE RENTAL AT A TIME (but multiple bikes is fine)
        private void button6_Click(object sender, EventArgs e)
        {
            string filename = this.textBox1.Text;  // GRABS THE INPUTTED DB FILENAME
            if (!fileExists(filename)){            // checks if the database file is valid
                return;
            }
            //DataAccessTier.Data data = new DataAccessTier.Data(filename);

            string userInput = this.textBox2.Text; // grab user-typed input
            string[] BIDS;
            string[] CNAME;
            RentalCart USER;
            try
            { // this try block attempts to parse input and checks to make sure everything is grabbed correctly
                CNAME = this.listBox1.SelectedItem.ToString().Split(' '); // grabs selected customer F&L names (falls to catch if none selected)
                // initialize rental cart
                USER = new RentalCart(CNAME[0], CNAME[1]);       // grabs entered rental time length 
                // GRAB RENTAL LENGTH
                USER.rentalLength = Convert.ToDouble(this.textBox3.Text);
                // GRAB BID VALUES
                BIDS = this.textBox2.Text.ToString().Split(',');         // puts entered BID values into an array
                if (BIDS[0] == "" || this.textBox3.Text == "" || CNAME.Length != 2) // 2nd conditional should never happen, 1st is if no BID's were entered
                    throw new Exception(); // forces into catch
            }
            catch{ // if input not done correctly
                MessageBox.Show("Please read instructions and try again");
                return;
            }

            ///////////////////////////////
            // set up the transaction
            connectionInfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\{0};Integrated Security=True;",
            filename);

            SqlConnection db = new SqlConnection(connectionInfo);
            db.Open();

            SqlTransaction tx = db.BeginTransaction(IsolationLevel.Serializable);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = db;
            cmd.Transaction = tx;
            SqlDataAdapter da = new SqlDataAdapter();
            // so onwards, before every sql command, i need to use
            // cmd.commandtext = sql;
            // cmd.ExecuteScalar();
            /////////////////////////////////////////

            for (int i = 0; i < 3; i++) // in case of deadlock, this for loop is to reattempt 3 times
            {
                /// MessageBox.Show(string.Format("Attempt: {0}", i));
                try // begin where the transaction code begins
                {
                    //
                    // so at this point, input is parsed, now I need to do a query to see if the user is with a rental currently.
                    //
                    string sql = String.Format(@"SELECT WithRental
                                                 FROM CUSTOMER WITH(INDEX(NAME_INDEX))
                                                 WHERE FirstName = '{0}' AND LastName = '{1}';", USER.FirstName, USER.LastName);
                    cmd.CommandText = sql;
                    object withRental = cmd.ExecuteScalar(); // should only be one row result
                    
                    if (withRental.ToString() == "True"){ // if the user already has a rental
                        MessageBox.Show("User is already with a rental, please return your bikes before making another rental :)");
                        return;
                    }


                    ////////////////////////////////
                    // at this point, we've confirmed, the selected customer is not withrentals
                    int timeInMS;
                    timeInMS = 500;    //  default 500ms delay

                    System.Threading.Thread.Sleep(timeInMS);
                    // so now we apply the sleep if the user wants it
                    ///////////////////////////////

                    //
                    // now, the user is confirmed not with a rental, but we must check if the bikes they are requesting are not already rented atm.
                    //

                    // ADD ALL THE DESIRED BIDS TO THE RENTAL CART
                    foreach (string bid in BIDS)
                    {
                        USER.AddBID(bid);
                    }

                    // create the set so we can grab all bike's info in 1 query
                    string set = USER.GetBIDS();

                    sql = String.Format(@"SELECT BID, IsitCheckedOut
                                          FROM BIKE 
                                          WHERE BID IN {0};", set);
                    // to get a dataset, the following code is used.
                    cmd.CommandText = sql;
                    da.SelectCommand = cmd;
                    DataSet INFO = new DataSet();
                    da.Fill(INFO);// = data.ExecuteNonScalarQuery(sql); // could be a multiple row result

                    string rented, BID;
                    foreach (DataTable table in INFO.Tables)
                    {  // go through all bikes and see if they're already checked out
                        foreach (DataRow row in table.Rows)
                        {
                            rented = row["IsitCheckedOut"].ToString();
                            BID = row["BID"].ToString();
                            if (rented == "True"){ // if the bike is already rented out
                                MessageBox.Show(string.Format(@"Bike {0} is already rented out, try another!", BID));
                                return;
                            }
                        }
                    }


                    ////////////////////////////////
                    // at this point, we've confirmed, the selected customer and bikes are not withrentals or rented
                    if (System.Int32.TryParse(this.textBox4.Text, out timeInMS) == true)
                        ;
                    else
                        timeInMS = 500;    //  default 300ms delay

                    System.Threading.Thread.Sleep(timeInMS);
                    // so now we apply the sleep if the user wants it
                    ///////////////////////////////
                    

                    //
                    // now, all inputted bikes are confirmed to not be checked out, time to finally being the insertion
                    //

                    DataSet ret = new DataSet();
                    object TIDO, CID;
                    string TID, cid;
                    //
                    // FIRST I will decrement the amount available for the type of each bike getting rented
                    // starting with needing to find the bike's type
                    foreach (string bid in BIDS)
                    {
                        sql = string.Format(@"SELECT TID FROM BIKE
                                              WHERE BID = {0};", bid);

                        cmd.CommandText = sql;
                        TIDO = cmd.ExecuteScalar(); // should only be one row result
                        if (TIDO == null)
                        {
                            MessageBox.Show("A nonexistent bike's BID was entered");
                            return;
                        }

                        cmd.CommandText = sql;
                        TIDO = cmd.ExecuteScalar();
                        TID  = TIDO.ToString();
                        sql  = string.Format(@"UPDATE BIKE_TYPE 
                                              SET AmountAvailable = ( (SELECT AmountAvailable FROM BIKE_TYPE WHERE TID = {0}) - 1 )
                                              WHERE TID = {1};", TID, TID);

                        cmd.CommandText = sql;
                        da.SelectCommand = cmd;
                        da.Fill(ret);
                    }


                    // THEN I am going to do SQL queries to UPDATE BIKE, and set them to "checked out"
                    // for this one, we can use the set I created earlier :)
                    sql = string.Format(@"UPDATE BIKE 
                                          SET IsItCheckedOut = 1
                                          WHERE BID IN {0};", set);
                    cmd.CommandText = sql;
                    da.SelectCommand = cmd;
                    da.Fill(ret);
                    //


                    //
                    // next I will set the Customer's rental status to true
                    sql = string.Format(@"UPDATE CUSTOMER 
                                          SET WithRental = 1
                                          WHERE FirstName = '{0}' AND LastName = '{1}';", USER.FirstName, USER.LastName);
                    cmd.CommandText = sql;
                    da.SelectCommand = cmd;
                    da.Fill(ret);
                    //
                    //


                    // NOW find the user's CID and INSERT into History table
                    sql = string.Format(@"SELECT CID 
                                          FROM CUSTOMER WITH(INDEX(NAME_INDEX))
                                          WHERE FirstName = '{0}' AND LastName = '{1}';", USER.FirstName, USER.LastName);
                    cmd.CommandText = sql;
                    CID = cmd.ExecuteScalar();
                    USER.CID = CID.ToString();

                    sql = "";
                    foreach (string bid in USER.OLDBIDS)
                    {
                        sql += string.Format(@"INSERT INTO 
                                               HISTORY(BID, CID, CheckedOut, ExpectedReturn, ChargedYet)
                                               Values({0}, {1}, GETDATE(), DATEADD(hour, {2}, GETDATE()), 0);", bid, USER.CID, USER.rentalLength.ToString());
                    }
                    // insert them all at once
                    cmd.CommandText = sql;
                    da.SelectCommand = cmd;
                    da.Fill(ret);

                    tx.Commit(); // NOW ATTEMPT TO COMMIT ALL CHANGES
                }
                catch (SqlException ex) when (ex.Number == 1205){ // if there's a deadlock, continue to next iteration of for-loop and try again
                    tx.Rollback(); // undo
                    continue; // retry!
                }
                catch (Exception ex) { // or if we catch a fatal error

                    MessageBox.Show(string.Format("Error: {0}", ex.ToString()));
                    tx.Rollback();
                }
                finally
                {
                    db.Close(); // and close the connnection.
                }
                break;
            } // end of transaction
            MessageBox.Show("Rental Processed!");
            return;

        }


        // #7
        // Allow customer C to return from a rental, in which case all N bikes are returned at the same time. The app
        // should display(MessageBox.Show is fine) the total cost of the rental based on the actual return time.
        private void button7_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex == -1){ // this is true when no customer is selected
                MessageBox.Show("Please select a user after clicking #1 and try again");
                return;
            }

            string filename = this.textBox1.Text;  // GRABS THE INPUTTED DB FILENAME
            if (!fileExists(filename)) {            // checks if the database file is valid
                return;
            }
            DataAccessTier.Data data = new DataAccessTier.Data(filename);
            string sql;

            string[] CNAME;
            CNAME = this.listBox1.SelectedItem.ToString().Split(' '); // grabs selected customer F&L names

            // now extract the Customer's First and last name.
            string FName = CNAME[0];
            string LName = CNAME[1];


            ///////////////////////////////
            // set up the transaction
            connectionInfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\{0};Integrated Security=True;",
            filename);

            SqlConnection db = new SqlConnection(connectionInfo);
            db.Open();

            SqlTransaction tx = db.BeginTransaction(IsolationLevel.Serializable);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = db;
            cmd.Transaction = tx;
            SqlDataAdapter da = new SqlDataAdapter();
            // so onwards, before every sql command, i need to use
            // cmd.CommandText = sql;
            // cmd.ExecuteScalar();
            /////////////////////////////////////////


            // begin attempting to execute the transaction
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    //
                    // First, now that we know the name, let's double check that they do have a rental
                    // (also grab their CID for future queries)
                    //
                    DataSet ret = new DataSet();
                    sql = String.Format(@"SELECT CID, WithRental FROM CUSTOMER
                                  WHERE FirstName = '{0}' AND LastName = '{1}';", FName, LName);


                    // insert them all at once
                    cmd.CommandText = sql;
                    da.SelectCommand = cmd;
                    da.Fill(ret);
                    //ret = data.ExecuteNonScalarQuery(sql);

                    string WithRental, CID = "";
                    foreach (DataTable table in ret.Tables)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            WithRental = row["WithRental"].ToString();
                            CID = row["CID"].ToString();
                            if (WithRental == "False")
                            { // if the bike is already rented out
                                MessageBox.Show("This user does not have a current rental!");
                                return;
                            }
                        }
                    }

                    //
                    // confirmed the customer does have a rental, now update all Tables 
                    //

                    // update Customer to say they are no longer with rental
                    sql = string.Format(@"UPDATE CUSTOMER 
                                  SET WithRental = 0
                                  WHERE FirstName = '{0}' AND LastName = '{1}';", FName, LName);


                    ////////////////////////////////
                    // at this point, we've done an update, a delay can be begun, hopefully it will last
                    // throughout the other updates and the entire transaction.
                    int timeInMS;
                    if (System.Int32.TryParse(this.textBox4.Text, out timeInMS) == true)
                        ;
                    else
                        timeInMS = 3000;    //  default 3000ms delay

                    System.Threading.Thread.Sleep(timeInMS);
                    // so now we apply the sleep if the user wants it
                    ///////////////////////////////


                    // insert them all at once
                    cmd.CommandText = sql;
                    da.SelectCommand = cmd;
                    da.Fill(ret);
                    //ret = data.ExecuteNonScalarQuery(sql);


                    // find the list of bikes rented by the customer
                    sql = string.Format(@"SELECT BID 
                                          FROM HISTORY WITH(INDEX(HISTORY_INDEX))
                                          WHERE CID = {0} AND ChargedYet = 0;", CID);

                    // insert them all at once
                    cmd.CommandText = sql;
                    da.SelectCommand = cmd;
                    da.Fill(ret);
                    //ret = data.ExecuteNonScalarQuery(sql);

                    // grab all the BID's and put it into a string, then split it into an array
                    string BIDSTRING = "";
                    string[] BIDS;
                    int j = 0;
                    foreach (DataTable table in ret.Tables)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            if (row != null)
                            {
                                if (j == 0){
                                    j++;
                                    continue; // skip first one
                                }

                                BIDSTRING += row["BID"].ToString();
                                BIDSTRING += ",";
                            }
                        }
                    }
                    BIDSTRING = BIDSTRING.TrimEnd(',');  // this approach i took leaves an extra comma at the end, let's trim it off.
                    BIDS = BIDSTRING.Split(',');        // puts collected BID values into an array
                                                        //

                    // create a string set containing all the BID's in order to check them out from BIKE table
                    string set = "(";
                    foreach (string bid in BIDS)
                    {
                        set += bid;
                        set += ",";
                    }
                    set = set.TrimEnd(',');  // this approach i took leaves an extra comma at the end, let's trim it off.
                    set += ")";
                    //


                    // Change the bikes to now make them available
                    sql = string.Format(@"UPDATE BIKE 
                                          SET IsItCheckedOut = 0
                                          WHERE BID IN {0};", set);

                    // insert them all at once
                    cmd.CommandText = sql;
                    da.SelectCommand = cmd;
                    da.Fill(ret);
                    //ret = data.ExecuteNonScalarQuery(sql);
                    // 


                    // increment the count of each bike's type for each bike type being returned

                    object TIDO;
                    string TID;

                    //
                    // increment the amount available for the type of each bike getting rented
                    foreach (string bid in BIDS)
                    {
                        sql = string.Format(@"SELECT TID FROM BIKE
                                              WHERE BID = {0};", bid);
                        cmd.CommandText = sql;
                        TIDO = cmd.ExecuteScalar(); // should only be one row result
                        TID = TIDO.ToString();
                        sql = string.Format(@"UPDATE BIKE_TYPE 
                                              SET AmountAvailable = ( (SELECT AmountAvailable FROM BIKE_TYPE WHERE TID = {0}) + 1 )
                                              WHERE TID = {1};", TID, TID);
                        cmd.CommandText = sql;
                        int rowsChanged = cmd.ExecuteNonQuery(); // should only be one row result
                        //ret = data.ExecuteNonScalarQuery(sql);
                    }
                    //


                    //
                    // finally, calculate the price of each bike's rental that is being returned
                    // to later be added, update the checked in time and "ChargedYet" attribute
                    // then add up all the prices and alert the user for how much they owe

                    // LONG QUERY, multiply the price of each bike, but we have to grab the type for each bike as well
                    // THEN update it so the HISTORY table has proper checked in time and reflects that customers are being charged
                    // calculates how much each minute costs, checks how many minutes the bike has been rented for then calcs total
                    sql = string.Format(@"SELECT SUM(CAST(DATEDIFF(minute, CheckedOut, GetDate()) AS DECIMAL(6,4))  *  BIKE_TYPE.Price/60.0) AS TOTAL
                                          FROM HISTORY WITH(INDEX(HISTORY_INDEX))
                                          INNER JOIN BIKE ON HISTORY.BID = BIKE.BID
                                          INNER JOIN BIKE_TYPE ON BIKE.TID = BIKE_TYPE.TID
                                          WHERE CID = {0} AND ChargedYet = 0;
                                          
                                          UPDATE HISTORY
                                          SET Checkedin = GetDate(), ChargedYet = 1
                                          WHERE CID = {0} AND ChargedYet = 0;
                                          ", CID); // works because both are {0}, not {0} {1}

                    // insert them all at once
                    cmd.CommandText = sql;
                    da.SelectCommand = cmd;
                    da.Fill(ret);
                    //ret = data.ExecuteNonScalarQuery(sql);

                    double TOTAL = 0;
                    int k = 0;
                    // for some reason, when using the data adapter, i need to grab the value from the 3rd row
                    foreach (DataTable table in ret.Tables){
                        foreach (DataRow row in table.Rows){
                            if (row != null){ // if the third row is not null, grab its value
                                object x = row["TOTAL"];
                                if(DBNull.Value.Equals(x)){
                                    continue;
                                }
                                TOTAL += Convert.ToDouble(row["TOTAL"]);//TOTAL += Convert.ToDouble(row["TOTAL"]);
                                break;
                            }
                            k++;
                        }
                        break;
                    }
                    TOTAL = Math.Round(TOTAL, 2); // round to the nearest penny
                    MessageBox.Show(String.Format(@"Your Amount Owed is ${0}, calculated by rounding to the nearest minute :)", TOTAL));
                    
                    tx.Commit(); // finally, commit.
                }
                catch (SqlException ex) when (ex.Number == 1205) { // if there's a deadlock, continue to next iteration of for-loop and try again
                    tx.Rollback(); // undo
                    continue; // retry! (upto 3 times)
                }
                catch (Exception ex){ // or if we catch a fatal error
                    tx.Rollback();
                    MessageBox.Show(string.Format("Error: {0}", ex.ToString()));
                }
                finally{
                    db.Close();
                }
            
            break; // if and exception isn't caught, then it was succesful and break out of the foor loop
            }
        }

        ////////////////////////////////////////////
        ////////////////////////////////////////////
        ////////////////////////////////////////////
        ////////////////////////////////////////////
        //
        // BEGIN EXTRA THINGS FOR PROJECT 3
        //
        ////////////////////////////////////////////
        ////////////////////////////////////////////
        ////////////////////////////////////////////
        ////////////////////////////////////////////
        ////////////////////////////////////////////
        
        
        //
        // This is button8 RESETS THE DATABASE, removing all rentals
        // #1 for proj3
        //
        private void button8_Click(object sender, EventArgs e){
            string filename = this.textBox1.Text;  // GRABS THE INPUTTED DB FILENAME
            if (!fileExists(filename)){            // checks if the database file is valid
                return;
            }
            //DataAccessTier.Data data = new DataAccessTier.Data(filename);
            string sql;
            connectionInfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\{0};Integrated Security=True;",
            filename);

            SqlConnection db = new SqlConnection(connectionInfo);
            db.Open();

            SqlTransaction tx = db.BeginTransaction(IsolationLevel.Serializable);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = db;
            cmd.Transaction = tx;

            sql = @"UPDATE COURSE_INFO
                    	SET Enrolled = 0
                    TRUNCATE TABLE WAITLIST
                    TRUNCATE TABLE ENROLLED
            ";

            cmd.CommandText = sql;

            // begin attempting to execute the transaction
            for (int i = 0; i < 3; i++) {
                try { // try to reset the database
                    var result = cmd.ExecuteScalar();
                }
                catch (SqlException ex) when (ex.Number == 1205) { // if there's a deadlock, continue to next iteration of for-loop and try again
                    tx.Rollback(); // undo
                    continue; // retry!
                }
                catch(Exception ex) { // or if we catch a fatal error
                    tx.Rollback();
                    MessageBox.Show(string.Format("Error: {0}", ex.ToString()));
                }
                finally{
                    tx.Commit();
                    db.Close();
                }
                break; // if it isn't "caught", then it was succesful and break out of the foor loop
            }
            MessageBox.Show("DATABASE RESET :)");
        }


        // #2 for Project 3, APPLIES INDEXES.
        private void button9_Click(object sender, EventArgs e)
        {
            string filename = this.textBox1.Text;  // GRABS THE INPUTTED DB FILENAME
            if (!fileExists(filename))
            {            // checks if the database file is valid
                return;
            }
            DataAccessTier.Data data = new DataAccessTier.Data(filename);
            string sql;

            // the first index is used to improve performance for when searching for people by 
            // first and last name, much faster than doing a linear scan each time.

            // the second index is to improve the functionality of #7 when I search for CID & ChargedYet
            // rather than doing a linear scan for those 2 values each time, this creates a tree
            sql = @"
                    CREATE PROCEDURE getSID
                    @FirstName  NVARCHAR(20),
                    @LastName   NVARCHAR(20),
                    @SID INT OUTPUT
                    AS
                        SET NOCOUNT ON;
                        SELECT @SID = SID 
                        FROM STUDENT WITH (INDEX(NAME_INDEX))
                        WHERE FirstName = '{0}' AND LastName = '{1}';
                    
                    
                    IF NOT EXISTS(SELECT * FROM sys.indexes WHERE Name = 'NAME_INDEX')
                        BEGIN
                        CREATE NONCLUSTERED INDEX NAME_INDEX
                        ON STUDENT(FirstName, Lastname)
                        
                        CREATE NONCLUSTERED INDEX NETID_INDEX
                        ON STUDENT(NetID)
                        
                        CREATE NONCLUSTERED INDEX COURSE_INDEX
                        ON COURSE_INFO(Dept, CourseNumber)
                        
                        CREATE NONCLUSTERED INDEX WAIT_INDEX
                        ON WAITLIST(SID, CID)
                        
                        CREATE NONCLUSTERED INDEX WAIT_INDEX2
                        ON WAITLIST(SID)
                        
                        CREATE NONCLUSTERED INDEX ENROLLED_INDEX
                        ON ENROLLED(SID);
                        END
                    ";
                    // notice the last is a STORED PROCEDURE
            data.ExecuteNonScalarQuery(sql);
            MessageBox.Show("INDEXES ADDED :)");
        }

        // this will contain data for shoppers
        public class RentalCart
        {   // default is public

            private List<string> BIDS = new List<string>();
            public  List<string> OLDBIDS = new List<string>();
            public string FirstName;
            public string LastName;
            public double rentalLength;
            public string CID;

            public RentalCart(string fname, string lname)
            {
                FirstName = fname;
                LastName = lname;
            }

            public void AddBID(string BID)
            {
                BIDS.Add(BID);
            }

            // creates a string containing a set of bid's, in sql notation.
            public string GetBIDS()
            {
                string set = "(";

                foreach (string bid in BIDS)
                {
                    set += bid;
                    set += ",";
                }
                set = set.TrimEnd(',');  // this approach i took leaves an extra comma at the end, let's trim it off.
                set += ")";              // and finally, append the closing parenthesis

                // clear old bids
                OLDBIDS.Clear();
                // populate the old 
                foreach (string bid in BIDS)
                    OLDBIDS.Add(bid);

                BIDS.Clear(); // upon returning the list, reset the list
                return set;
            }

        }

        ///////////// END OLD PROGRAM 3 HERE.
        ///////////// END OLD PROGRAM 3 HERE.
        ///////////// END OLD PROGRAM 3 HERE.
        ///////////// END OLD PROGRAM 3 HERE.
        ///////////// END OLD PROGRAM 3 HERE.
        ///////////// END OLD PROGRAM 3 HERE.
        ///////////// END OLD PROGRAM 3 HERE.
        ///////////// END OLD PROGRAM 3 HERE.
        ///////////// END OLD PROGRAM 3 HERE.
        ///////////// END OLD PROGRAM 3 HERE.

        // display's name and courses
        private void button10_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear(); // clear the listbox

            string filename = this.textBox1.Text;  // GRABS THE INPUTTED DB FILENAME
            if (!fileExists(filename))
            { // checks if the database file is valid
                return;
            }

            DataAccessTier.Data data = new DataAccessTier.Data(filename);
            string sql = @"SELECT FirstName, LastName, NetID " +
                          "FROM STUDENT  " +
                          "ORDER BY LastName, FirstName;";
            string name;
            DataSet ALLNAMES = null;
            try
            {
                ALLNAMES = data.ExecuteNonScalarQuery(sql);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Apply indexes before attempting to use the database please :)");
            }

            foreach (DataTable table in ALLNAMES.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (row != null)
                    {
                        name = row["FirstName"].ToString() + " " + row["LastName"].ToString() + " " + row["NetID"];// + " , Netid: " + row["NetID"].ToString();
                        this.listBox1.Items.Add(name);
                    }
                }
            }

            sql = @"SELECT Dept, CourseNumber, CRN
                   FROM COURSE_INFO
                   ORDER BY Dept DESC, CourseNumber;";

            DataSet ALLCOURSES = null;
            ALLCOURSES = data.ExecuteNonScalarQuery(sql);

            foreach (DataTable table in ALLCOURSES.Tables) {
                foreach (DataRow row in table.Rows) {
                    if (row != null) {
                        name = row["Dept"].ToString() + " " + row["CourseNumber"].ToString() + " " + row["CRN"];// + " , Netid: " + row["NetID"].ToString();
                        this.listBox2.Items.Add(name);
                    }
                }
            }

        }

        // #1
        private void button12_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex == -1){ // this is true when no customer is selected
                MessageBox.Show("Please select a user after clicking 'Display students' and try again");
                return;
            }
            
            string filename = this.textBox1.Text;  // GRABS THE INPUTTED DB FILENAME
            if (!fileExists(filename)) // checks if the database file is valid
                return;
            
            DataAccessTier.Data data = new DataAccessTier.Data(filename);
            string sql;

            string[] CNAME;
            CNAME = this.listBox1.SelectedItem.ToString().Split(' '); // grabs selected customer F&L names

            this.listBox1.Items.Clear(); // clear the listbox

            // now extract the Customer's First and last name.
            string FName = CNAME[0];
            string LName = CNAME[1];

            this.listBox1.Items.Add(String.Format("{0} {1} is Enrolled in the following classes:", FName, LName));
            sql = String.Format(@"
                    DECLARE @SID INT
                    EXEC getSID '{0}', '{1}', @SID output
                    
                    SELECT Dept, CourseNumber, CRN
                    FROM COURSE_INFO
                    WHERE CID IN (SELECT CID FROM ENROLLED WHERE SID = @SID)
                    ORDER BY CourseNumber;
                    ", FName, LName);

            string course;
            DataSet EnrolledCourses = data.ExecuteNonScalarQuery(sql);
            foreach (DataTable table in EnrolledCourses.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (row != null)
                    {
                        course = row["Dept"].ToString() + " " + row["CourseNumber"].ToString() + ", CRN: " + row["CRN"].ToString();
                        this.listBox1.Items.Add(course);
                    }
                }
                this.listBox1.Items.Add(" "); // blank line inbetween enrolled and waitlisted
            }

            this.listBox1.Items.Add(String.Format("{0} {1} is Waitlisted in the following classes:", FName, LName));
            sql = String.Format(@"
                    DECLARE @SID INT
                    EXEC getSID '{0}', '{1}', @SID output

                    SELECT Dept, CourseNumber, CRN
                    FROM COURSE_INFO
                    WHERE CID IN (SELECT CID FROM WAITLIST WHERE SID = @SID)
                    ORDER BY CourseNumber;
                    ", FName, LName);

            EnrolledCourses = data.ExecuteNonScalarQuery(sql);
            foreach (DataTable table in EnrolledCourses.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (row != null)
                    {
                        course = row["Dept"].ToString() + " " + row["CourseNumber"].ToString() + ", CRN: " + row["CRN"].ToString();
                        this.listBox1.Items.Add(course);
                    }
                }
            }
        }


        // #2
        // #2
        // #2 enrollment
        // #2
        private void button11_Click(object sender, EventArgs e)
        {
            int count = 0;
            string CID, SID;

            string filename = this.textBox1.Text;  // GRABS THE INPUTTED DB FILENAME
            if (!fileExists(filename)) // checks if the database file is valid
                return;

            if (this.listBox1.SelectedIndex == -1) // grabs selected user to enroll
            { // this is true when no customer is selected
                MessageBox.Show("Please select a user after clicking 'Display students' and try again");
                return;
            }
            DataAccessTier.Data data = new DataAccessTier.Data(filename);
            string sql;
            
            string []CNAME = this.listBox1.SelectedItem.ToString().Split(' '); // grabs selected customer F&L names

            // now extract the Customer's First and last name.
            string FName = CNAME[0];
            string LName = CNAME[1];
            FName = FName.Replace("'", @"''");
            LName = LName.Replace("'", @"''");
            string x = this.textBox2.Text;
            if (x == ""){ // to prevent no data from being inputted
                MessageBox.Show("Please enter a CRN");
                return;
            }
            string CRN = this.textBox2.Text;

            ///////////////////////////////
            // set up the transaction
            connectionInfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\{0};Integrated Security=True;",
            filename);

            SqlConnection db = new SqlConnection(connectionInfo);
            db.Open();

            SqlTransaction tx = db.BeginTransaction(IsolationLevel.Serializable);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = db;
            cmd.Transaction = tx;
            SqlDataAdapter da = new SqlDataAdapter();
            // so onwards, before every sql command, i need to use
            // cmd.CommandText = sql;
            // cmd.ExecuteScalar();
            /////////////////////////////////////////

            try
            {
                ////////////////////////////////

                int timeInMS;
                if (System.Int32.TryParse(this.textBox4.Text, out timeInMS) == true)
                    ;
                else
                    timeInMS = 3000;    //  default 3000ms delay

                System.Threading.Thread.Sleep(timeInMS);

                ///////////////////////////////

                //////////// first determine if they are already enrolled in the class
                // find SID and see if theyre Enrolled
                sql = string.Format(@"
                    DECLARE @SID INT
                    EXEC getSID '{0}', '{1}', @SID output

                    SELECT CID
                    FROM COURSE_INFO
                    WHERE CID IN (SELECT CID FROM ENROLLED WHERE SID = @SID) AND CRN = {2}
                    ORDER BY CourseNumber;
                    ", FName, LName, CRN);


                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                DataSet INFO = new DataSet();
                da.Fill(INFO);

                // count how many rows are found
                foreach (DataTable table in INFO.Tables)
                    foreach (DataRow row in table.Rows)
                        if (row != null)
                            count++;
                ///////////////

                /////////////// NOW SEE IF THE STUDENT IS ALREADY WAITLISTED FOR THIS CLASS
                sql = string.Format(@"
                    DECLARE @SID INT
                    EXEC getSID '{0}', '{1}', @SID output

                    SELECT CID
                    FROM COURSE_INFO
                    WHERE CID IN (SELECT CID FROM WAITLIST WHERE SID = @SID) AND CRN = {2}
                    ORDER BY CourseNumber
                    ", FName, LName, CRN);


                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                INFO = new DataSet();
                da.Fill(INFO);

                // count how many rows are found
                foreach (DataTable table in INFO.Tables)
                    foreach (DataRow row in table.Rows)
                        if (row != null)
                            count++;
                //////////////////////

                /////////////// NOW MAKE SURE THE CLASS EXISTS
                sql = string.Format(@"
                    SELECT CID
                    FROM COURSE_INFO
                    WHERE CRN = {0}
                    ", CRN);

                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                INFO = new DataSet();
                da.Fill(INFO);

                // count how many rows are found
                foreach (DataTable table in INFO.Tables)
                    if(table.Rows.Count == 0)
                        count++;
                //////////////////////

                if (count > 0) { // boot them out if they are already registerd
                    MessageBox.Show("Make sure you are not registering for a class you are currently already waiting for or enrolled in AND the class exists");
                    return;
                }

                ///////////////// NOW FIND OUT HOW MANY PEOPLE ARE IN THE CLASS AND ITS SIZE
                int Size, Enrolled;
                sql = string.Format(@"
                    SELECT Size
                    FROM COURSE_INFO
                    WHERE CRN = {0}
                    ", CRN);
                
                cmd.CommandText = sql;
                object ret = cmd.ExecuteScalar();
                Size = Convert.ToInt32(ret);

                sql = string.Format(@"
                    SELECT Enrolled
                    FROM COURSE_INFO
                    WHERE CRN = {0}
                    ", CRN);

                cmd.CommandText = sql;
                ret = cmd.ExecuteScalar();
                Enrolled = Convert.ToInt32(ret);
                /////////////


                ///////////// first we need the CID for the class and SID for the student
                sql = string.Format(@"
                            SELECT CID
                            FROM COURSE_INFO
                            WHERE CRN = {0}
                            ", CRN);

                cmd.CommandText = sql;
                ret = cmd.ExecuteScalar();
                CID = ret.ToString();

                sql = string.Format(@"
                            SELECT SID
                            FROM STUDENT
                            WHERE FirstName = '{0}' AND LastName = '{1}'
                            ", FName, LName);

                cmd.CommandText = sql;
                ret = cmd.ExecuteScalar();
                SID = ret.ToString();
                /////////////
                int z;
                // FINALLY, if the class is full, do a query to add the student to the waitlist
                if (Enrolled >= Size)
                {
                    MessageBox.Show("Sorry, this class is full! Adding you to the waitlist");
                    
                    sql = string.Format(@"
                            INSERT INTO WAITLIST(SID, CID, InList)
                            VALUES({0},{1}, 1)
                            ", SID, CID);
                    cmd.CommandText = sql;
                    z = cmd.ExecuteNonQuery();
                    tx.Commit();
                    MessageBox.Show(string.Format("{0} {1} has Succesfully been Waitlisted for class {2}", FName, LName, CRN));

                }
                else // OTHERWISE, IF THE CLASS ISN'T FULL, ENROLL THE STUDENT IN THE CLASS AND UPDATE ENROLLED
                {
                    sql = string.Format(@"
                            INSERT INTO ENROLLED(SID, CID)
                            VALUES({0},{1})
                            
                            UPDATE COURSE_INFO
                            SET Enrolled = (SELECT Enrolled FROM COURSE_INFO WHERE CRN = {2}) + 1
                            WHERE CRN = {2}
                            ", SID, CID, CRN);
                    cmd.CommandText = sql;
                    z = cmd.ExecuteNonQuery();
                }
                /////////////////
                tx.Commit();
                MessageBox.Show(string.Format("{0} {1} has Succesfully registered for class {2}", FName, LName, CRN));
                ////////////////
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error: {0}", ex));
                tx.Rollback();
            }
            finally {
                db.Close();
            }
            
        }

        //#3
        //#3
        //#3
        //#3  drop studnet from waitlist/course
        private void button13_Click(object sender, EventArgs e)
        {
            int count = 0, Enrolled;
            string CID, SID;

            string filename = this.textBox1.Text;  // GRABS THE INPUTTED DB FILENAME
            if (!fileExists(filename)) // checks if the database file is valid
                return;

            if (this.listBox1.SelectedIndex == -1) // grabs selected user to enroll
            { // this is true when no customer is selected
                MessageBox.Show("Please select a user after clicking 'Display students' and try again");
                return;
            }
            DataAccessTier.Data data = new DataAccessTier.Data(filename);
            string sql;

            string[] CNAME = this.listBox1.SelectedItem.ToString().Split(' '); // grabs selected customer F&L names

            // now extract the Customer's First and last name.
            string FName = CNAME[0];
            string LName = CNAME[1];
            string x = this.textBox2.Text;
            if (x == "") // to prevent no data from being inputted
                return;
            string CRN = this.textBox2.Text;

            ///////////////////////////////
            // set up the transaction
            connectionInfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\{0};Integrated Security=True;",
            filename);

            SqlConnection db = new SqlConnection(connectionInfo);
            db.Open();

            SqlTransaction tx = db.BeginTransaction(IsolationLevel.Serializable);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = db;
            cmd.Transaction = tx;
            SqlDataAdapter da = new SqlDataAdapter();
            // so onwards, before every sql command, i need to use
            // cmd.CommandText = sql;
            // cmd.ExecuteScalar();
            /////////////////////////////////////////  

            try
            {

                FName = FName.Replace("'", @"''");
                LName = LName.Replace("'", @"''");

                ////////////////////////////////

                int timeInMS;
                if (System.Int32.TryParse(this.textBox4.Text, out timeInMS) == true)
                    ;
                else
                    timeInMS = 3000;    //  default 3000ms delay

                System.Threading.Thread.Sleep(timeInMS);

                ///////////////////////////////

                //////////// first determine if they are already enrolled in the class
                sql = string.Format(@"
                    DECLARE @SID INT
                    EXEC getSID '{0}', '{1}', @SID output

                    SELECT CID
                    FROM COURSE_INFO
                    WHERE CID IN (SELECT CID FROM ENROLLED WHERE SID = @SID) AND CRN = {2}
                    ORDER BY CourseNumber;
                    ", FName, LName, CRN);


                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                DataSet INFO = new DataSet();
                da.Fill(INFO);

                // count how many rows are found, count becomes 1 if IN the class
                foreach (DataTable table in INFO.Tables)
                    if (table.Rows.Count != 0)
                        count = 1;
                
                if(count != 1){ // not in the class!
                    MessageBox.Show("You can not drop a class you are not in!");
                    return;
                }
                ///////////////
                
                /////////////// NOW SEE IF THE STUDENT IS ALREADY WAITLISTED FOR THIS CLASS
                sql = string.Format(@"
                    DECLARE @SID INT
                    
                    EXEC getSID '{0}', '{1}', @SID output

                    SELECT CID
                    FROM COURSE_INFO
                    WHERE CID IN (SELECT CID FROM WAITLIST WHERE SID = @SID) AND CRN = {2}
                    ORDER BY CourseNumber;
                    ", FName, LName, CRN);


                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                INFO = new DataSet();
                da.Fill(INFO);

                // count how many rows are found
                foreach (DataTable table in INFO.Tables)
                    if (table.Rows.Count == 0)
                        count = 2;
                if (count != 2){ 
                    MessageBox.Show("You ARE ALREADY waitlisted in this class!");
                    return;
                }
                //////////////////////

                /////////////// NOW MAKE SURE THE CLASS EXISTS 
                sql = string.Format(@"
                    SELECT CID
                    FROM COURSE_INFO
                    WHERE CRN = {0}
                    ", CRN);

                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                INFO = new DataSet(); 
                da.Fill(INFO); 

                // count how many rows are found
                foreach (DataTable table in INFO.Tables)
                    if (table.Rows.Count == 0) // if NO classes found for the given CRN
                        count = 3;
                //////////////////////

                if (count == 3) { // boot them out if the class doesn't exist 
                    MessageBox.Show("Make sure you are not registering for a class which exists");
                    return;
                }

                // find the count of how many are enrolled for the class
                sql = string.Format(@"
                    SELECT Enrolled
                    FROM COURSE_INFO
                    WHERE CRN = {0}
                    ", CRN);

                cmd.CommandText = sql;
                object ret = cmd.ExecuteScalar();
                Enrolled = Convert.ToInt32(ret);
                /////////////


                ///////////// we need the CID for the class and SID for the student
                sql = string.Format(@"
                            SELECT CID
                            FROM COURSE_INFO
                            WHERE CRN = {0}
                            ", CRN);

                cmd.CommandText = sql;
                ret = cmd.ExecuteScalar();
                CID = ret.ToString();

                sql = string.Format(@"
                            SELECT SID
                            FROM STUDENT
                            WHERE FirstName = '{0}' AND LastName = '{1}'
                            ", FName, LName);

                cmd.CommandText = sql;
                ret = cmd.ExecuteScalar();
                SID = ret.ToString();
                /////////////

                /////////////
                // DETERMINE IF THE USER IS ENROLLED OR WAITLISTED
                sql = string.Format(@"
                    SELECT SID
                    FROM ENROLLED
                    WHERE SID = {0} AND CID = {1}
                    ", SID, CID);

                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                INFO = new DataSet();
                da.Fill(INFO);

                // count how many rows are found
                foreach (DataTable table in INFO.Tables)
                    if (table.Rows.Count != 0) // if an enrollment is found
                        count = 1;
                    else
                        count = 2;
                /////////////

                int z;
                object OSID = null;
                if (count == 1) // if enrolled in >class< , remove and check waitlist
                {
                    // remove from class and update enrollment count
                    sql = string.Format(@"
                            DELETE FROM ENROLLED
                            WHERE SID = {0} AND CID = {1}
                            
                            UPDATE COURSE_INFO
                            SET Enrolled = {2}
                            WHERE CID = {1} 

                            ", SID, CID, Enrolled - 1);
                    cmd.CommandText = sql;
                    z = cmd.ExecuteNonQuery();

                    // NOW check if there is someone in the waitlist who can replace this drop
                    // grab the SID of this student if it exists.
                    sql = string.Format(@"
                            SELECT SID 
                            FROM WAITLIST
                            WHERE CID = {0} AND WID = (SELECT MIN(WID) FROM WAITLIST WHERE CID = {0} AND InList = 1)
                            ", CID);
                    cmd.CommandText = sql;
                    da.SelectCommand = cmd;
                    INFO = new DataSet();
                    da.Fill(INFO);

                    // count how many rows are found AND GRAB THE SID IF IT EXISTS
                    foreach (DataTable table in INFO.Tables){
                        if (table.Rows.Count == 0){
                            count = 1;
                            break;
                        }
                        foreach (DataRow row in table.Rows){
                            if (row != null) {
                                OSID = row["SID"];
                                count = 4; // trigger the insert of a new student
                            }
                        }
                    }

                    if (count == 4) // insert the new student
                    {
                        sql = string.Format(@"
                            INSERT INTO ENROLLED(SID, CID)
                            VALUES({0},{1})
                            
                            UPDATE COURSE_INFO
                            SET Enrolled = (SELECT Enrolled FROM COURSE_INFO WHERE CRN = {2}) + 1
                            WHERE CRN = {2}
                            
                            UPDATE WAITLIST
                            SET InList = 0
                            WHERE SID = {0} AND CID = {1}
                            ", OSID.ToString(), CID, CRN);
                        cmd.CommandText = sql;
                        z = cmd.ExecuteNonQuery();
                        MessageBox.Show(string.Format("UnEnrolled {0} {1} AND enrolled student with SID = {2} to class with CRN = {3}", FName, LName, OSID.ToString(), CRN));
                    }
                    
                }
                else if(count == 2) // if enrolled in >waitlist<   (don't update enrollment count)
                {
                    sql = string.Format(@"
                            UPDATE WAITLIST
                            SET InList = 0
                            WHERE SID = {0} AND CID = {1}
                            ", SID, CID);
                    cmd.CommandText = sql;
                    z = cmd.ExecuteNonQuery();
                    MessageBox.Show(string.Format("Unenrolled {0} {1} from the waitlist for Class with CRN = {2}", FName, LName, CRN));
                }
                ///////////////// // finished drop


                /////////////////
                tx.Commit();
                MessageBox.Show(string.Format("{0} {1} has successfully dropped class {2}", FName, LName, CRN));
                ////////////////
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error: {0}", ex));
                tx.Rollback();
            }
            finally
            {
                db.Close();
            }
            
        }


        //
        // when someone selects a course from the course list, display its info.
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filename = this.textBox1.Text;  // GRABS THE INPUTTED DB FILENAME
            if (!fileExists(filename)) // checks if the database file is valid
                return;

            object INDEX = this.listBox2.SelectedItem;
            DataAccessTier.Data data = new DataAccessTier.Data(filename);
            string sql;

            string[] CINFO = this.listBox2.SelectedItem.ToString().Split(' '); // grabs selected COURSE info that was selected
            this.listBox3.Items.Clear(); // CLEAR IT

            // now extract the CRN OF THE SELECTED COURSE
            string CRN = CINFO[2]; // 0 is dept, 1 is course_num, 2 is CRN


            /////////////////// COURSE INFO
            sql = string.Format(@"SELECT Semester, Year, Type, Day, Time, Size, Enrolled
                                  FROM COURSE_INFO
                                  WHERE CRN = {0}", CRN);

            DataSet COURSEINFO = null;
            COURSEINFO = data.ExecuteNonScalarQuery(sql);
            string output = "";
            foreach (DataTable table in COURSEINFO.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (row != null)
                    {
                        this.listBox3.Items.Add("Course Info:");
                        output = "Semester: " + row["Semester"].ToString() + ", Year: " + row["Year"].ToString();
                        this.listBox3.Items.Add(output);
                        output = "Type: " + row["Type"].ToString() + ", Days: " + row["Day"].ToString();
                        this.listBox3.Items.Add(output);
                        output = "Time: " + row["Time"].ToString() + ", Size: " + row["Size"].ToString();
                        this.listBox3.Items.Add(output);
                        output = "Enrolled: " + row["Enrolled"].ToString();
                        this.listBox3.Items.Add(output);
                    }
                }
            }
            this.listBox3.Items.Add("");
            /////////////////// COURSE INFO END


            /////////////////// NETID OF ALL ENROLLED STUDENTS
            // first we need to grab the CID, then we can find all students
            sql = string.Format(@"SELECT CID
                                  FROM COURSE_INFO
                                  WHERE CRN = {0}", CRN);

            object OCID = null;
            OCID = data.ExecuteScalarQuery(sql);
            string CID = OCID.ToString();

            // NOW, we can find all the NetID's
            sql = string.Format(@"SELECT NetID
                                  FROM STUDENT
                                  INNER JOIN ENROLLED ON ENROLLED.SID = STUDENT.SID
                                  WHERE CID = {0}", CID);

            DataSet NETIDS = null;
            NETIDS = data.ExecuteNonScalarQuery(sql);
            output = "";

            this.listBox3.Items.Add("Enrolled students' Netids:");
            foreach (DataTable table in NETIDS.Tables){
                foreach (DataRow row in table.Rows){
                    if (row != null) {
                        output = row["NetID"].ToString();
                        this.listBox3.Items.Add(output);
                        
                    }
                }
            }
            this.listBox3.Items.Add("");
            /////////////////// NETID END

            /////////////////// ALSO NEED TO ADD HOW MANY ON WAITLIST

            this.listBox3.Items.Add("All NetID's for waitlisted students:");

            sql = string.Format(@"SELECT NetID
                                  FROM STUDENT
                                  INNER JOIN WAITLIST ON WAITLIST.SID = STUDENT.SID
                                  WHERE CID = {0} AND WAITLIST.InList = 1
                                  ORDER BY WID ASC", CID);

            NETIDS = data.ExecuteNonScalarQuery(sql);
            output = "";

            foreach (DataTable table in NETIDS.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (row != null)
                    {
                        output = row["NetID"].ToString();
                        this.listBox3.Items.Add(output);

                    }
                }
            }
        }


        //
        //
        // #4, swap 2 students' enrollment positions.
        private void button14_Click(object sender, EventArgs e)
        {
            int count = 0, Enrolled;
            string CID, SID;

            string filename = this.textBox1.Text;  // GRABS THE INPUTTED DB FILENAME
            if (!fileExists(filename)) { // checks if the database file is valid
                MessageBox.Show("DB doesn't exist");
                return;
            }

            if (this.listBox1.SelectedIndex == -1) // grabs selected user to enroll
            { // this is true when no customer is selected
                MessageBox.Show("Please select a user after clicking 'Display students' and try again");
                return;
            }
            if (this.listBox3.SelectedIndex == -1) // grabs selected user to enroll
            { // this is true when no customer is selected
                MessageBox.Show("Please select a user after clicking 'Display students' button on the right and try again");
                return;
            }

            DataAccessTier.Data data = new DataAccessTier.Data(filename);
            string sql;

            string[] CNAME  = this.listBox1.SelectedItem.ToString().Split(' '); // grabs selected customer F&L names
            string[] CNAME2 = this.listBox3.SelectedItem.ToString().Split(' ');
            // now extract the Customer's First and last name.
            string FName1  = CNAME[0];
            string LName1  = CNAME[1];
            string FName2 = CNAME2[0];
            string LName2 = CNAME2[1];
            string x = this.textBox3.Text;
            string y = this.textBox5.Text;
            if (x == "" || y == "") // to prevent no data from being inputted
                return;
            //string CRN = this.textBox2.Text;
            string CRN1 = this.textBox3.Text;
            string CRN2 = this.textBox5.Text;
            string SID1, SID2, CID1, CID2;
            object ret;
            int z;

            ///////////////////////////////
            // set up the transaction
            connectionInfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\{0};Integrated Security=True;",
            filename);

            SqlConnection db = new SqlConnection(connectionInfo);
            db.Open();

            SqlTransaction tx = db.BeginTransaction(IsolationLevel.Serializable);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = db;
            cmd.Transaction = tx;
            SqlDataAdapter da = new SqlDataAdapter();
            // so onwards, before every sql command, i need to use
            // cmd.CommandText = sql;
            // cmd.ExecuteScalar();
            /////////////////////////////////////////  

            try
            {

                FName1 = FName1.Replace("'", @"''");
                LName1 = LName1.Replace("'", @"''");
                FName2 = FName2.Replace("'", @"''");
                LName2 = LName2.Replace("'", @"''");
                ////////////////////////////////

                int timeInMS;
                if (System.Int32.TryParse(this.textBox4.Text, out timeInMS) == true)
                    ;
                else
                    timeInMS = 3000;    //  default 3000ms delay

                System.Threading.Thread.Sleep(timeInMS);

                ///////////////////////////////

                //////////// first determine if they are already enrolled in the class
                sql = string.Format(@"
                    DECLARE @SID INT
                    EXEC getSID '{0}', '{1}', @SID output
                    
                    SELECT CID
                    FROM COURSE_INFO
                    WHERE CID IN (SELECT CID FROM ENROLLED WHERE SID = @SID) AND CRN = {2}
                    ORDER BY CourseNumber;
                    ", FName1, LName1, CRN1);


                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                DataSet INFO = new DataSet();
                da.Fill(INFO);

                // count how many rows are found, count becomes 1 if IN the class
                foreach (DataTable table in INFO.Tables)
                    if (table.Rows.Count != 0)
                        count = 1;

                if (count != 1)
                { // not in the class!
                    MessageBox.Show("You can not drop a class you are not in!");
                    return;
                }

                count = 0; // reset count
                // NOW CHECK IF THE SECOND STUDENT IS IN THE CLASS THEY SAID THEYRE IN
                sql = string.Format(@"
                    DECLARE @SID INT
                    EXEC getSID '{0}', '{1}', @SID output
                    
                    SELECT CID
                    FROM COURSE_INFO
                    WHERE CID IN (SELECT CID FROM ENROLLED WHERE SID = @SID) AND CRN = {2}
                    ORDER BY CourseNumber;
                    ", FName2, LName2, CRN2);


                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                INFO = new DataSet();
                da.Fill(INFO);

                // count how many rows are found, count becomes 1 if IN the class
                foreach (DataTable table in INFO.Tables)
                    if (table.Rows.Count != 0)
                        count = 1;

                if (count != 1){ // not in the class!
                    MessageBox.Show("You can not drop a class you are not in!");
                    return;
                }
                
                ///////////////
                

                /////////////// NOW MAKE SURE THE CLASSES EXISTS 
                sql = string.Format(@"
                    SELECT CID
                    FROM COURSE_INFO
                    WHERE CRN = {0}
                    ", CRN1);

                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                INFO = new DataSet();
                da.Fill(INFO);

                // count how many rows are found
                foreach (DataTable table in INFO.Tables)
                    if (table.Rows.Count == 0) // if NO classes found for the given CRN
                        count = 3;
                if (count == 3) { // boot them out if the class doesn't exist 
                    MessageBox.Show(string.Format("Make sure {0} {1} is not swapping for a class which exists", FName1, LName1));
                    return;
                }


                count = 0;
                // NOW FOR SECOND CLASS
                sql = string.Format(@"
                    SELECT CID
                    FROM COURSE_INFO
                    WHERE CRN = {0}
                    ", CRN2);

                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                INFO = new DataSet();
                da.Fill(INFO);

                // count how many rows are found
                foreach (DataTable table in INFO.Tables)
                    if (table.Rows.Count == 0) // if NO classes found for the given CRN
                        count = 3;

                if (count == 3){ // boot them out if the class doesn't exist 
                    MessageBox.Show(string.Format("Make sure {0} {1} is not swapping for a class which exists", FName2, LName2));
                    return;
                }
                /////////////// class exists



                ///////////// we need the CID for the class and SID for the students we are swapping
                sql = string.Format(@"
                            SELECT CID
                            FROM COURSE_INFO
                            WHERE CRN = {0}
                            ", CRN1);

                cmd.CommandText = sql;
                ret = cmd.ExecuteScalar();
                CID1 = ret.ToString();

                sql = string.Format(@"
                            SELECT SID
                            FROM STUDENT
                            WHERE FirstName = '{0}' AND LastName = '{1}'
                            ", FName1, LName1);

                cmd.CommandText = sql;
                ret = cmd.ExecuteScalar();
                SID1 = ret.ToString();

                //// now find the second student's info.
                sql = string.Format(@"
                            SELECT CID
                            FROM COURSE_INFO
                            WHERE CRN = {0}
                            ", CRN2);

                cmd.CommandText = sql;
                ret = cmd.ExecuteScalar();
                CID2 = ret.ToString();

                sql = string.Format(@"
                            SELECT SID
                            FROM STUDENT
                            WHERE FirstName = '{0}' AND LastName = '{1}'
                            ", FName2, LName2);

                cmd.CommandText = sql;
                ret = cmd.ExecuteScalar();
                SID2 = ret.ToString();
                /////////////


                ///////////// FINALLY, SWAP THEM
                // Swap the Enrollment values
                sql = String.Format(@"
                                      UPDATE ENROLLED
                                      SET CID = {0} 
                                      WHERE SID = {1} AND CID = {2}
                                      
                                      UPDATE ENROLLED
                                      SET CID = {2}
                                      WHERE SID = {3} AND CID = {0}
                                      ", CID2, SID1, CID1, SID2);
                
                cmd.CommandText = sql;
                z = cmd.ExecuteNonQuery();
                
                /////////////////
                tx.Commit();
                MessageBox.Show(string.Format("{0} {1} has successfully switched classes {2} and {3} with {4} {5}", FName1, LName1, CRN1, CRN2, FName2, LName2));
                ////////////////
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error: {0}", ex));
                tx.Rollback();
            }
            finally
            {
                db.Close();
            }
            
        } // finished swapping the two students



        //
        // button that puts students' names in the 3rd listbox, used for #4 
        private void button15_Click(object sender, EventArgs e)
        {
            this.listBox3.Items.Clear(); // clear the listbox

            string filename = this.textBox1.Text;  // GRABS THE INPUTTED DB FILENAME
            if (!fileExists(filename))
            { // checks if the database file is valid
                return;
            }

            DataAccessTier.Data data = new DataAccessTier.Data(filename);
            string sql = @"SELECT FirstName, LastName, NetID " +
                          "FROM STUDENT WITH (INDEX(NAME_INDEX)) " +
                          "ORDER BY LastName, FirstName;";
            string name;
            DataSet ALLNAMES = null;
            try
            {
                ALLNAMES = data.ExecuteNonScalarQuery(sql);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Apply indexes before attempting to use the database please :)");
            }

            foreach (DataTable table in ALLNAMES.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (row != null)
                    {
                        name = row["FirstName"].ToString() + " " + row["LastName"].ToString() + " " + row["NetID"];// + " , Netid: " + row["NetID"].ToString();
                        this.listBox3.Items.Add(name);
                    }
                }
            }
        }
    }
}
