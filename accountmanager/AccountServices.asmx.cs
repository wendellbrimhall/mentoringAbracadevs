using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

//we need these to talk to mysql
using MySql.Data;
using MySql.Data.MySqlClient;
//and we need this to manipulate data from a db
using System.Data;

namespace accountmanager
{
	/// <summary>
	/// Summary description for AccountServices
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	[System.Web.Script.Services.ScriptService]
	public class AccountServices : System.Web.Services.WebService
	{

		//EXAMPLE OF A SIMPLE SELECT QUERY (PARAMETERS PASSED IN FROM CLIENT)
		[WebMethod(EnableSession = true)]
		public bool LogOn(string email, string password)
		{
			//LOGIC: pass the parameters into the database to see if an account
			//with these credentials exist.  If it does, then return true.  If
			//it doesn't, then return false

			//we return this flag to tell them if they logged in or not
			bool success = false;

			//our connection string comes from our web.config file like we talked about earlier
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
			//here's our query.  A basic select with nothing fancy.  Note the parameters that begin with @
			string sqlSelect = "SELECT * FROM Users_mentoring WHERE email=@emailValue and password=SHA1(@passValue)";

			//set up our connection object to be ready to use our connection string
			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			//set up our command object to use our connection, and our query
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			//tell our command to replace the @parameters with real values
			//we decode them because they came to us via the web so they were encoded
			//for transmission (funky characters escaped, mostly)
			sqlCommand.Parameters.AddWithValue("@emailValue", HttpUtility.UrlDecode(email));
			sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(password));

			//a data adapter acts like a bridge between our command object and 
			//the data we are trying to get back and put in a table object
			MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
			//here's the table we want to fill with the results from our query
			DataTable sqlDt = new DataTable("accounts");
			//here we go filling it!
			sqlDa.Fill(sqlDt);
			//check to see if any rows were returned.  If they were, it means it's 
			//a legit account
			if (sqlDt.Rows.Count > 0)
			{
                // adding Session 
                Session["userID"] = sqlDt.Rows[0]["userID"];
                Session["first_name"] = sqlDt.Rows[0]["firstName"];
                Session["last_name"] = sqlDt.Rows[0]["lastName"];
                Session["employee_id"] = sqlDt.Rows[0]["employeeID"];
                Session["email"] = sqlDt.Rows[0]["email"];
                Session["position"] = sqlDt.Rows[0]["position"];
                Session["status"] = sqlDt.Rows[0]["status"];
                Session["admin"] = sqlDt.Rows[0]["admin"];
                                               
                //flip our flag to true so we return a value that lets them know they're logged in
                success = true;
			}
			//return the result!
			return success;
		}

		[WebMethod(EnableSession = true)]
		public bool LogOff()
		{
            //if they log off, we don't have much to do
            //right now.  Later, we'll see how we can
            //use this method to "forget" this user at
            //the server level.
            Session.Abandon();
            return true;
        }

      
        [WebMethod]
        public string AddUser(string first, string last, string empID, string email, string position, string pw)
        {

            ///webmethod to a newuser to the database


            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;

            //string sqlSelect = "INSERT INTO `abracadevs`.`Users_mentoring` (`firstName`, `lastName`, `employeeID`, `email`, `position`, `status`, `password`) " +
            // "VALUES('"+ first +"', '"+ last +"', '"+ empID + "', '" + email + "', '" + position + "', '" + status + "', '" + pw + "';";

            string sqlSelect = "INSERT INTO `abracadevs`.`Users_mentoring` (`firstName`, `lastName`, `employeeID`, `email`, `position`, `status`, `admin`, `password`) VALUES (@fnameValue, @lnameValue, @empIdValue, @emailValue, @positionValue, 'pending', '0', SHA1(@passwordValue));";

            //"VALUES (@fnameValue, @lnameValue, @empIdValue, @emailValue, @positionValue, @statusValue, SHA1(@passwordValue);)";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);


            sqlCommand.Parameters.AddWithValue("@fnameValue", HttpUtility.UrlDecode(first));
            sqlCommand.Parameters.AddWithValue("@lnameValue", HttpUtility.UrlDecode(last));
            sqlCommand.Parameters.AddWithValue("@empIdValue", HttpUtility.UrlDecode(empID));
            sqlCommand.Parameters.AddWithValue("@emailValue", HttpUtility.UrlDecode(email));
            sqlCommand.Parameters.AddWithValue("@positionValue", HttpUtility.UrlDecode(position));
            //sqlCommand.Parameters.AddWithValue("@statusValue", HttpUtility.UrlDecode(status));
            sqlCommand.Parameters.AddWithValue("@passwordValue", HttpUtility.UrlDecode(pw));

            sqlConnection.Open();
            try
            {
                sqlCommand.ExecuteNonQuery();
                return "Success!";

            }

            catch (Exception e)
            {
                var str = e.ToString();
                // Data base is setup to require a unique email and license plate. If a duplicate of either is put in the server will return an error. The follow code will
                // search the error returned from the server and return appropriate feedback.

                if (str.Contains("email_UNIQUE"))
                {
                    return "email";
                }

                else
                {
                    return str;
                }
            }
            sqlConnection.Close();

        }


        [WebMethod(EnableSession = true)]
        public Account[] ViewAccountInfo()
        {
            // get info of an account
            var user_id = Session["user_id"];
            var first_name = Session["first_name"];
            var last_name = Session["last_name"];
            var employee_id = Session["employee_id"];
            var email = Session["email"];
            var position = Session["position"];
            var admin = Convert.ToInt32(Session["admin"]);

            string sqlSelect;

            DataTable sqlDt = new DataTable("user");

            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;

            if (admin == 0)
            {
                // if the user is not a admin, only his/ her account info will be displayed 
                //sqlSelect = "SELECT * FROM Users_mentoring WHERE email ='" + email + "';";

                // testing
                sqlSelect = "SELECT * FROM Users_mentoring WHERE email ='eee@gmail.com';";
            }
            else
            {
                // if the user is a admin, all users' account info will be displayed 
                //string sqlSelect = "SELECT * FROM Users_mentoring WHERE email ='" + email + "';";
                sqlSelect = "SELECT * FROM Users_mentoring;";
            }

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            //gonna use this to fill a data table
            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
            sqlDa.Fill(sqlDt);

            //loop through each row in the dataset
            List<Account> accountInfo = new List<Account>();
            for (int i = 0; i < sqlDt.Rows.Count; i++)
            {
                accountInfo.Add(new Account
                {
                    userID = Convert.ToInt32(sqlDt.Rows[i]["userID"]),
                    firstName = sqlDt.Rows[i]["firstName"].ToString(),
                    lastName = sqlDt.Rows[i]["lastName"].ToString(),
                    employeeID = sqlDt.Rows[i]["employeeID"].ToString(),
                    email = sqlDt.Rows[i]["email"].ToString(),
                    position = sqlDt.Rows[i]["position"].ToString()
                });
            }
            //convert the list of accounts to an array and return!
            return accountInfo.ToArray();
        }


        //EXAMPLE OF AN INSERT QUERY WITH PARAMS PASSED IN.  BONUS GETTING THE INSERTED ID FROM THE DB!
        [WebMethod]
		public void RequestAccount(string uid, string pass, string firstName, string lastName, string email)
		{
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
			//the only thing fancy about this query is SELECT LAST_INSERT_ID() at the end.  All that
			//does is tell mySql server to return the primary key of the last inserted row.
			string sqlSelect = "insert into accounts (userid, pass, firstname, lastname, email) " +
				"values(@idValue, @passValue, @fnameValue, @lnameValue, @emailValue); SELECT LAST_INSERT_ID();";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(uid));
			sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(pass));
			sqlCommand.Parameters.AddWithValue("@fnameValue", HttpUtility.UrlDecode(firstName));
			sqlCommand.Parameters.AddWithValue("@lnameValue", HttpUtility.UrlDecode(lastName));
			sqlCommand.Parameters.AddWithValue("@emailValue", HttpUtility.UrlDecode(email));

			//this time, we're not using a data adapter to fill a data table.  We're just
			//opening the connection, telling our command to "executescalar" which says basically
			//execute the query and just hand me back the number the query returns (the ID, remember?).
			//don't forget to close the connection!
			sqlConnection.Open();
			//we're using a try/catch so that if the query errors out we can handle it gracefully
			//by closing the connection and moving on
			try
			{
				int accountID = Convert.ToInt32(sqlCommand.ExecuteScalar());
				//here, you could use this accountID for additional queries regarding
				//the requested account.  Really this is just an example to show you
				//a query where you get the primary key of the inserted row back from
				//the database!
			}
			catch (Exception e) {
			}
			sqlConnection.Close();
		}

		

		//EXAMPLE OF AN UPDATE QUERY WITH PARAMS PASSED IN
		[WebMethod]
		public void UpdateAccount(string id, string uid, string pass, string firstName, string lastName, string email)
		{
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
			//this is a simple update, with parameters to pass in values
			string sqlSelect = "update accounts set userid=@uidValue, pass=@passValue, firstname=@fnameValue, lastname=@lnameValue, " +
				"email=@emailValue where id=@idValue";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@uidValue", HttpUtility.UrlDecode(uid));
			sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(pass));
			sqlCommand.Parameters.AddWithValue("@fnameValue", HttpUtility.UrlDecode(firstName));
			sqlCommand.Parameters.AddWithValue("@lnameValue", HttpUtility.UrlDecode(lastName));
			sqlCommand.Parameters.AddWithValue("@emailValue", HttpUtility.UrlDecode(email));
			sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(id));

			sqlConnection.Open();
			//we're using a try/catch so that if the query errors out we can handle it gracefully
			//by closing the connection and moving on
			try
			{
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();
		}

		//EXAMPLE OF A SELECT, AND RETURNING "COMPLEX" DATA TYPES
		[WebMethod]
		public Account[] GetAccountRequests()
		{//LOGIC: get all account requests and return them!

			DataTable sqlDt = new DataTable("accountrequests");

			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
			//requests just have active set to 0
			string sqlSelect = "select id, userid, pass, firstname, lastname, email from accounts where active=0 order by lastname";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
			sqlDa.Fill(sqlDt);
			
			List<Account> accountRequests = new List<Account>();
			for (int i = 0; i < sqlDt.Rows.Count; i++)
			{
				accountRequests.Add(new Account
				{
					userID = Convert.ToInt32(sqlDt.Rows[i]["id"]),
					firstName = sqlDt.Rows[i]["firstname"].ToString(),
					lastName = sqlDt.Rows[i]["lastname"].ToString(),
					email = sqlDt.Rows[i]["email"].ToString()
				});
			}
			//convert the list of accounts to an array and return!
			return accountRequests.ToArray();
		}

		//EXAMPLE OF A DELETE QUERY
		[WebMethod]
		public void DeleteAccount(string id)
		{
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
			//this is a simple update, with parameters to pass in values
			string sqlSelect = "delete from accounts where id=@idValue";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);
			
			sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(id));

			sqlConnection.Open();
			try
			{
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();
		}

		//EXAMPLE OF AN UPDATE QUERY
		[WebMethod]
		public void ActivateAccount(string id)
		{
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
			//this is a simple update, with parameters to pass in values
			string sqlSelect = "update accounts set active=1 where id=@idValue";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(id));

			sqlConnection.Open();
			try
			{
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();
		}

		//EXAMPLE OF AN DELETE QUERY
		[WebMethod]
		public void RejectAccount(string id)
		{
			string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
			string sqlSelect = "delete from accounts where id=@idValue";

			MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
			MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

			sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(id));

			sqlConnection.Open();
			try
			{
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception e)
			{
			}
			sqlConnection.Close();
		}




    }
}
