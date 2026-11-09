using Org.BouncyCastle.Crypto.Generators;
using System.Data.SqlClient;

namespace API_template.backend
{//start of namespace
    public class profile_info
    {//start of class

        //creating a method to get the users info
        //from the sessions
        public string get_profile_info(HttpContext get_details)
        {

            //temp variable declaration
            string details = "Please login firs...";

            //get each and assign to temp variables
            string? user_email = get_details.Session.GetString("user_email");
            string? user_name = get_details.Session.GetString("user_name");
            string? user_role = get_details.Session.GetString("user_role");
            string? user_id = get_details.Session.GetString("user_id");

            //check if the session is empty or not
            if ( user_email!=null ) {

                //then append
                details = "----------Profile Details for " + user_name + "----------\n" +
                    "[ Fullname       ] :  " + user_name + "  \n" +
                    "[ Emaill address ] :  " + user_email + " \n" +
                    "[ Role           ] :  " + user_role + "\n" +
                    "[ ------------------------------------------------------------------\n" +
                    "           [  user id : " + user_id + "  ]";
            }

            //then return the details
            return details;

        }//end of get profile info method



        //update the user's details
        public string update_info(string fullname , string email,string choice , HttpContext get_sessions)
        {

            //temp variable declaration
            string message = "Failed to update the user's details, try again later....";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";


            //try and catch
            try
            {

                //connect to the database and server
                //using SqlConnection
                //creating an instance for the class SqlConnection with an object name connect
                SqlConnection connect = new SqlConnection(connection_string);

                //check the email the user is trying to update
                if (check_email(email, connect) && (choice.Equals("email") || choice.Equals("both")))
                {

                   return  "Please update or change something first ...";

                }
               


                    //open the connection
                    connect.Open();


                    //query to update
                    int? user_id = Int32.Parse(get_sessions.Session.GetString("user_id").ToString());


                    string query = @"update users set fullname='" + fullname + "',email='" + email + "' where id=" + user_id + "";


                    //check choice
                    if (choice.Equals("fullname"))
                    {
                        query = @"update users set fullname='" + fullname + "' where id=" + user_id + "";
                    }
                    else if (choice.Equals("email"))
                    {
                        query = @"update users set email='" + email + "' where id=" + user_id + "";
                    }

                    //then use SqlCommand to execute the query
                    //creating the instance for the class SqlCommand with an object name run_query
                    SqlCommand run_query = new SqlCommand(query, connect);

                    int affected_rows = run_query.ExecuteNonQuery();

                    //check how many rows are affected
                    if (affected_rows > 0)
                    {


                        //then assign the new details to the session
                        if (choice.Equals("fullname"))
                        {
                            get_sessions.Session.SetString("user_name", fullname);
                        }
                        else if (choice.Equals("email"))
                        {
                            get_sessions.Session.SetString("user_email", email);
                        }
                        else
                        {
                            get_sessions.Session.SetString("user_email", email);
                            get_sessions.Session.SetString("user_name", fullname);
                        }

                        //then assign succes message
                        message = "Profile details successfully updated.....";
                    }



                    //then close the connection
                    connect.Close();




            }
            catch (Exception error)
            {
                //assign the error message
                message = error.Message;
            }







            //then return the message
            return message;



        }//end of method update info


        //creating a method check if the email already have an account or not
        public Boolean check_email(string email, SqlConnection connect)
        {//start of methodlogin

            bool found = false;
            //open the connection
            connect.Open();

            //query to check if the user is found or not
            string query = @"select * from users where email='" + email + "'";

            //creating an instance for the class SqlCommand with an object name
            //run_query
            SqlCommand run_query = new SqlCommand(query, connect);

            //creating an instance for the class SqlDataReader 
            //with an object name get_data
            SqlDataReader get_data = run_query.ExecuteReader();

            //read the data if found
            //using a while loop
            while (get_data.Read())
            {




                found = true;




            }//end of while


            //closing the connection
            get_data.Close();
            connect.Close();


            return found;

        }//end of method check email & login sessions






        //method to update the password
        public string update_password(string old_password , string new_password ,HttpContext get_sessions)
        {

            //temp variables
            string message = "Failed to update password , please try again later....";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";


            //try and catch
            try
            {
                //connect to the server using SqlConnection class
                SqlConnection connect = new SqlConnection(connection_string);

                //open the connection
                connect.Open();

                //getting the id of the user and also do the query
                int user_id = Int32.Parse( get_sessions.Session.GetString("user_id").ToString()  );
                //first check if the old password is not the same as the new one
                string old_query = @"select password from users where id="+user_id+";";


                //run the query using the SqlCommand
                SqlCommand run_query = new SqlCommand(old_query,connect);

                //read the password using SqlDataReader
                SqlDataReader old_passwordHashed = run_query.ExecuteReader();
                
                //using while loop to read the data
                while ( old_passwordHashed.Read()  )
                {

                    // the the password by column name
                    string? hashed_password = old_passwordHashed["password"].ToString();

                    //then verify before
                    bool found_match = BCrypt.Net.BCrypt.Verify(old_password,hashed_password);

                    //then return
                    if (found_match)
                    {

                     if (BCrypt.Net.BCrypt.Verify(new_password, hashed_password))
                    {
                            //close the connection
                            old_passwordHashed.Close();
                            connect.Close();
                            return "Old password must not be used as new password";
                    }
                    else
                    {
                            old_passwordHashed.Close();
                            //hash the password
                            string hash_password = BCrypt.Net.BCrypt.HashPassword(new_password);

                        string query = @"update users set password='" + hash_password + "' where id=" + user_id + ";";

                        //then execute the query using the SqlCommand
                        SqlCommand run_newQuery = new SqlCommand(query, connect);
                        run_newQuery.ExecuteNonQuery();

                            //close the connection
                            old_passwordHashed.Close();
                            connect.Close();
                            return "Password updated successfully.....";

                    }

                    }
                    else
                    {
                        //close the connection
                        old_passwordHashed.Close();
                        connect.Close();
                        return "Incorrect current or old password entered......";
                    }



                }//end of while loop



                //close the connection
                old_passwordHashed.Close();
                connect.Close();

            }
            catch (Exception error)
            {
                //assign error message
                message = error.Message;
            }


            //return the message
            return message;
        }//end of update password method












    }//end of class
}//end of namespace
