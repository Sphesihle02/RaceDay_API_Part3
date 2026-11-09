using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;

namespace API_template.backend
{//start of namespace
    public class register_backend
    {//start of class


        //creating method to store user's info inside a database table
        //called users
        public string store_user( string fullname , string email , string role, string password   )
        {//start of method

            //variable declaration
            string message = "Failed to register the user, try again later..";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

            //try and catch
            try
            {//start of try

                //get the pasword and hash it
                string hashed_password = BCrypt.Net.BCrypt.HashPassword(password);

                //creating an instance for the SqlConnection class
                //with an object name called connect
                SqlConnection connect = new SqlConnection(connection_string);


                //check if the email already have an account
                if (!check_email(email , connect))
                {




                    //open the connection
                    connect.Open();

                    //query to insert the user's info or Data
                    string query = @"insert into users values('" + fullname + "','" + email + "','" + role + "' ,'" + hashed_password + "' );";

                    //creating an instance for the class SqlCommand
                    //with an object name called run_query
                    SqlCommand run_query = new SqlCommand(query, connect);

                    //run or execute the query
                    //run_query.ExecuteNonQuery();
                    int results_affected = run_query.ExecuteNonQuery();

                    //check if successfull or not
                    if (results_affected > 0)
                    {
                        //assign success message
                        message = "User registered successfully..";

                    }//end of if statement

                    //close the connection
                    connect.Close();
                }
                else
                {
                    message = "Email already have an account....";
                }
            }//end of try
            catch ( Exception error  )
            {//start of catch

                //return error message 
                //assign first
                message = error.Message;


            }//end of catch

            //return the message
            return message;




        }//end of method store user

        //creating a method check if the email already have an account or not
        public Boolean check_email(string email,SqlConnection connect)
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

                


                found= true;
                   



                }//end of while


            //closing the connection
            connect.Close();


      return found;

        }//end of method login

  
    }//end of class
}//end of name space
