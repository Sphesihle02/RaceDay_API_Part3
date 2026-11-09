using System.Data.SqlClient;

namespace API_template.backend
{//start of namespace
    public class login_backend
    {//start of class

        //global variables
        private string fullname, role, id,email;
       
        //creating a method to allow users to login , using their email and Password
        public string login( string email , string password , HttpContext create_session)
        {//start of methodlogin
            //variable declaration
            string message = "Incorrect email or password....";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";



            //try and catch
            try
            {


                //creating an instance for the class SqlConnection
                //with an object name connect
                SqlConnection connect = new SqlConnection(connection_string);

                //open the connection
                connect.Open();

                //query to check if the user is found or not
                string query = @"select * from users where email='"+email+"'";

                //creating an instance for the class SqlCommand with an object name
                //run_query
                SqlCommand run_query = new SqlCommand( query ,connect  );

                //creating an instance for the class SqlDataReader 
                //with an object name get_data
                SqlDataReader get_data = run_query.ExecuteReader();

                //read the data if found
                //using a while loop
                while ( get_data.Read()  )
                {

                    //get the password column and validate with the hashed one
                    //inside the table
                    string? hashed_password = get_data["password"].ToString();

                    //then validate with the one the user entered
                    // Verify the entered password
                    bool correct = BCrypt.Net.BCrypt.Verify(password, hashed_password);

                    //if statement to check for the correct password 
                    if ( correct  )
                    {
                        //then collect the user's info
                        //store the info in the sessions
                       create_session.Session.SetString("user_email",email  );
                        //temp variables
                        this.fullname = get_data["fullname"].ToString();
                        this.role = get_data["role"].ToString();
                        this.id = get_data["id"].ToString();
                        this.email = email;
                        /*store all in session  */
                        create_session.Session.SetString("user_name", fullname  );
                        create_session.Session.SetString("user_role" , role );
                        create_session.Session.SetString("user_id", id);
                      

                        //then user successfull signed in
                        message = "Login successfully...";



                    }//end of if



                }//end of while


                get_data.Close();
                connect.Close();





            }//end of try
            catch (Exception error)
            {
                //assign the error message to temp variable
                message = error.Message;

            }//end of catch








            //return the message
            return message;


        }//end of method login

        //method to return the details
        public string return_data()
        {
            //return
            return this.fullname + "," + this.role + "," + this.id+","+email;
        }



    }//end of class
}//end of namespace
