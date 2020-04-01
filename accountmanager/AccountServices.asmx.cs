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
using System.Net;
using System.Net.Mail;

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
                Session["department"] = sqlDt.Rows[0]["department"];
                Session["position"] = sqlDt.Rows[0]["position"];
                Session["status"] = sqlDt.Rows[0]["status"];
                Session["admin"] = sqlDt.Rows[0]["admin"];
                Session["surveyComplete"] = sqlDt.Rows[0]["surveyComplete"];

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
        public string AddUser(string first, string last, string empID, string email, string position, string department, string pw )
        {

            ///webmethod to a newuser to the database


            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;

            //string sqlSelect = "INSERT INTO `abracadevs`.`Users_mentoring` (`firstName`, `lastName`, `employeeID`, `email`, `position`, `status`, `password`) " +
            // "VALUES('"+ first +"', '"+ last +"', '"+ empID + "', '" + email + "', '" + position + "', '" + status + "', '" + pw + "';";

            string sqlSelect = "INSERT INTO `abracadevs`.`Users_mentoring` (`firstName`, `lastName`, `employeeID`, `email`, `position`, `status`, `admin`, `department`, `password`) VALUES (@fnameValue, @lnameValue, @empIdValue, @emailValue, @positionValue, 'pending', '0', @departmentValue, SHA1(@passwordValue));";

            //"VALUES (@fnameValue, @lnameValue, @empIdValue, @emailValue, @positionValue, @statusValue, SHA1(@passwordValue);)";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);


           sqlCommand.Parameters.AddWithValue("@fnameValue", HttpUtility.UrlDecode(first));
            sqlCommand.Parameters.AddWithValue("@lnameValue", HttpUtility.UrlDecode(last));
            sqlCommand.Parameters.AddWithValue("@empIdValue", HttpUtility.UrlDecode(empID));
            sqlCommand.Parameters.AddWithValue("@emailValue", HttpUtility.UrlDecode(email));
            sqlCommand.Parameters.AddWithValue("@positionValue", HttpUtility.UrlDecode(position));
            sqlCommand.Parameters.AddWithValue("@departmentValue", HttpUtility.UrlDecode(department));
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
            var department = Session["department"];
            var position = Session["position"];

            DataTable sqlDt = new DataTable("user");

            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
                        
            // if the user is not an admin, only his/ her account info will be displayed 
            //string sqlSelect = "SELECT * FROM Users_mentoring WHERE email ='" + email + "';";

            // testing
            string sqlSelect = "SELECT * FROM Users_mentoring WHERE email ='eee@gmail.com';";

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
                    department = sqlDt.Rows[i]["department"].ToString(),
                    position = sqlDt.Rows[i]["position"].ToString()
                });
            }
            //convert the list of accounts to an array and return!
            return accountInfo.ToArray();
        }

        [WebMethod(EnableSession = true)]
        public Account[] GetAllAccountInfo()
        {
            DataTable sqlDt = new DataTable("users");

            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;

            // if the user is an admin, all users' account info will be displayed 
            //string sqlSelect = "SELECT * FROM Users_mentoring WHERE email ='" + email + "';";
            string sqlSelect = "SELECT * FROM Users_mentoring WHERE admin = '0';";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            //gonna use this to fill a data table
            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
            sqlDa.Fill(sqlDt);

            //loop through each row in the dataset
            List<Account> allAccountInfo = new List<Account>();
            for (int i = 0; i < sqlDt.Rows.Count; i++)
            {
                allAccountInfo.Add(new Account
                {
                    userID = Convert.ToInt32(sqlDt.Rows[i]["userID"]),
                    firstName = sqlDt.Rows[i]["firstName"].ToString(),
                    lastName = sqlDt.Rows[i]["lastName"].ToString(),
                    employeeID = sqlDt.Rows[i]["employeeID"].ToString(),
                    email = sqlDt.Rows[i]["email"].ToString(),
                    department = sqlDt.Rows[i]["department"].ToString(),
                    position = sqlDt.Rows[i]["position"].ToString(),
                    status = sqlDt.Rows[i]["status"].ToString()
                });
            }
            //convert the list of accounts to an array and return!
            return allAccountInfo.ToArray();
        }

        [WebMethod(EnableSession = true)]
        public string RecordSurvey(string q1, string q2, string q3, string q4, string q5)
        {



            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
            var user_id = Convert.ToInt32(Session["userID"]);
            

            string sqlSelect = "INSERT INTO `abracadevs`.`survey_mentoring` (`userID`, `q1`, `q2`, `q3`, `q4`, `q5`) VALUES ('" + user_id + "', '"+ q1 +"', '"+ q2 +"', '"+ q3 + "', '"+ q4 +"', '"+ q5 +"');";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

          

            sqlConnection.Open();
            try
            {
                sqlCommand.ExecuteNonQuery();
                var str = "Success";
                CompleteSurvey(user_id);

                //after the reservation is made, the SendReservationConfirmation function is called to notify the user that their spot has been reserved
                return str;
            }
            catch (Exception e)
            {
                var str = e.ToString();
                return str;
            }
            sqlConnection.Close();

        }

        public void CompleteSurvey(int user_id)
        {
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;

            string sqlSelect = "UPDATE `abracadevs`.`Users_mentoring` SET `surveyComplete` = '1' WHERE(`userID` = '"+ user_id + "');";


            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);



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

        [WebMethod(EnableSession = true)]
        public bool SurveyComplete()
        {
            var surveycomplete = Convert.ToInt32(Session["surveyComplete"]);

            if (surveycomplete == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [WebMethod]
        public void SendEmail(string recipient, string subject, string body)
        ///This webmethod will send out an email from cis440parking@asu.edu using googles
        ///SMTP server. 

        {
            var toAddress = recipient;



            using (MailMessage mail = new MailMessage())
            {

                var smtpAddr = "smtp.gmail.com";
                var portNumber = 587;
                var enableSSL = true;
                var fromAddress = "cis440parking@gmail.com";
                var password = "!!Abracadevs";


                mail.From = new MailAddress(fromAddress);
                mail.To.Add(toAddress);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient(smtpAddr, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(fromAddress, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);

                }
            }
        }


    }
}
