using System.Data.SqlClient;

namespace API_template.backend
{
    public class manage_categories
    {


        //creating method to store categories
        public string store_category(string name , string description , HttpContext get_orgID)
        {
            //temp variable declaration
            string message = "Failed to create category....";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";


            //try and catch
            try
            {
                //connecting to the server
                SqlConnection connect = new SqlConnection(connection_string);

                //open the connection
                connect.Open();

                //query
                int user_id = Int32.Parse(get_orgID.Session.GetString("user_id") );
                string query = @"insert into category values('"+name+"','"+description+"',"+user_id+");";

                //run the command or execute the query
                SqlCommand run_query = new SqlCommand(query , connect);
                int affected_rows = run_query.ExecuteNonQuery();

                //check if a row is affected 
                if (affected_rows>0)
                {
                    message = "Category successfully created.....";
                }


                //close the connection
                connect.Close();


            }
            catch (Exception error)
            {
                //assign error message
                message = error.Message;
            }



            //return the message
            return message;
        }//end of store category method

        //creating method to store categories
        public string store_categorySite(string name, string description, int user_id)
        {
            //temp variable declaration
            string message = "Failed to create category....";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";


            //try and catch
            try
            {
                //connecting to the server
                SqlConnection connect = new SqlConnection(connection_string);

                //open the connection
                connect.Open();

                //query
                string query = @"insert into category values('" + name + "','" + description + "'," + user_id + ");";

                //run the command or execute the query
                SqlCommand run_query = new SqlCommand(query, connect);
                int affected_rows = run_query.ExecuteNonQuery();

                //check if a row is affected 
                if (affected_rows > 0)
                {
                    message = "Category successfully created.....";
                }


                //close the connection
                connect.Close();


            }
            catch (Exception error)
            {
                //assign error message
                message = error.Message;
            }



            //return the message
            return message;
        }//end of store category method


        //method to display all the categories
        public SqlDataReader view_categoriesSite(int user_id)
        {
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

            //query to select only this organizer's categories
            string query = @"select * from category where organizer_id=" + user_id + ";";

            //connect to the database and server
            SqlConnection connect = new SqlConnection(connection_string);

            //open the connection
            connect.Open();

            //execute the query
            SqlCommand run_query = new SqlCommand(query, connect);

            //then read data as it execute and return the reader
            SqlDataReader read_categories = run_query.ExecuteReader();

            return read_categories;
        }

        //method to display all the categories
        public string view_categories(HttpContext get_id)
        {
            //temp variables
            string message = "You have no categories added, please add them first..";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

            //try and catch
            try
            {

                //connect to the database and server
                SqlConnection connect = new SqlConnection(connection_string);

                //open the connection
                connect.Open();

                //temp variables for query
                int user_id = Int32.Parse(get_id.Session.GetString("user_id") );
                string query = @"select * from category where organizer_id="+user_id+";";

                //execute the query
                SqlCommand run_query = new SqlCommand(query , connect);
                //then read data as it execute
                SqlDataReader read_categories = run_query.ExecuteReader();

                //then temp and while loop to read the data
                int count_categories = 0;
                bool data_found = false;
                message = "";
                while ( read_categories.Read() )
                {
                    data_found = true;
                    //get categories by column names and append
                    message += "----------------------------------------------" +
                               "\n    [ Category no "+(count_categories +=1 )+" ]      \n" +
                               "----------------------------------------------\n";
                    //appending so message variable does not lose the last data
                    message += "Category   ID: "+read_categories["id"].ToString()+"\n" +
                               "Category NAME: "+read_categories["name"].ToString()+"\n" +
                               "Category Desc: "+read_categories["description"].ToString()+"\n" +
                               "====================================================================\n" +
                               "Category Creator ID "+read_categories["organizer_id"].ToString()+"\n" +
                               "=====================================================================\n";

                }//end of while loop

                //then check if any was found or not
                if (!data_found)
                {
                    message = "You have no categories added, please add them first..";

                }


                //closing the connection
                connect.Close();





            }
            catch (Exception error)
            {
                //assign error message
                message = error.Message;
            }



            ///return the error message or success
            return message;

        }//end of method view categories







        //method for updating_categories
        public string updating_categories(int? category_id, string name, string description, string organizer_id, string choice, HttpContext get_session)
        {

            //temp variables
            string message = "Failed to update " + choice + " !!!!!!!!";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";



            //try and catch
            try
            {

                //temp for query
                int owner_id = Int32.Parse(organizer_id);
                string query = @"update category set name='" + name + "' ,description='" + description + "' where id=" + category_id + " and organizer_id=" + owner_id + ";";

                //check choice
                if (choice == "name")
                {
                    query = @"update category set name='" + name + "'  where id=" + category_id + " and organizer_id=" + owner_id + ";";
                    message = "Category name is successfully updated..";
                }//end of if for name

                //check for the desc
                if (choice == "description")
                {
                    query = @"update category set description='" + description + "' where id=" + category_id + " and organizer_id=" + owner_id + ";";
                    message = "Category description is successfully updated..";

                }
                if (choice == "both")
                {
                    message = "Category name and description are successfully updated..";

                }



                //connecting to the server and database
                SqlConnection connection = new SqlConnection(connection_string);
                //open the connection
                connection.Open();

                //Execute the command
                SqlCommand run_query = new SqlCommand(query, connection);
                //then execute
                int affected_rows = run_query.ExecuteNonQuery();

                if (affected_rows == 0)
                {
                    message = "Failed to update " + choice + " !!!!!!!!";
                }

                //close the connection
                connection.Close();



            }
            catch (Exception error)
            {

                //assign the error message from catch
                message = error.Message;
            }//end of catch





            //then return
            return message;

        }//end of updating_categories method

        //method for updating_categories
        public string updating_categoriesSite(int? category_id, string name, string description, string organizer_id, string choice)
        {

            //temp variables
            string message = "Failed to update " + choice + " !!!!!!!!";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";



            //try and catch
            try
            {

                //temp for query
                int owner_id = Int32.Parse(organizer_id);
                string query = @"update category set name='" + name + "' ,description='" + description + "' where id=" + category_id + " and organizer_id=" + owner_id + ";";


                    message = "Category successfully updated.....";

                



                //connecting to the server and database
                SqlConnection connection = new SqlConnection(connection_string);
                //open the connection
                connection.Open();

                //Execute the command
                SqlCommand run_query = new SqlCommand(query, connection);
                //then execute
                int affected_rows = run_query.ExecuteNonQuery();

                if (affected_rows == 0)
                {
                    message = "Failed to update " + choice + " !!!!!!!!";
                }

                //close the connection
                connection.Close();



            }
            catch (Exception error)
            {

                //assign the error message from catch
                message = error.Message;
            }//end of catch





            //then return
            return message;

        }//end of updating_categories method



        //method for deleting_categories
        public string deleting_categories(int? category_id, string organizer_id, HttpContext get_session)
        {

            //temp variables
            string message = "Failed to delete category !!!!!!!!";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

            //try and catch
            try
            {

                //temp for query
                int owner_id = Int32.Parse(organizer_id);

                //query to delete only if this organizer owns the category
                string query = @"delete from category where id=" + category_id + " and organizer_id=" + owner_id + ";";

                //connecting to the server and database
                SqlConnection connection = new SqlConnection(connection_string);
                //open the connection
                connection.Open();

                //Execute the command
                SqlCommand run_query = new SqlCommand(query, connection);
                //then execute
                int affected_rows = run_query.ExecuteNonQuery();

                if (affected_rows == 0)
                {
                    message = "Failed to delete category, either it does not exist or you do not own it !!!!!!!!";
                }
                else
                {
                    message = "Category with id " + category_id + " is successfully deleted..";
                }

                //close the connection
                connection.Close();

            }
            catch (Exception error)
            {

                //assign the error message from catch
                message = error.Message;
            }//end of catch

            //then return
            return message;

        }//end of deleting_categories method


        //method for deleting_categories
        public string deleting_categoriesSite(int? category_id, string organizer_id)
        {

            //temp variables
            string message = "Failed to delete category !!!!!!!!";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

            //try and catch
            try
            {

                //temp for query
                int owner_id = Int32.Parse(organizer_id);

                //query to delete only if this organizer owns the category
                string query = @"delete from category where id=" + category_id + " and organizer_id=" + owner_id + ";";

                //connecting to the server and database
                SqlConnection connection = new SqlConnection(connection_string);
                //open the connection
                connection.Open();

                //Execute the command
                SqlCommand run_query = new SqlCommand(query, connection);
                //then execute
                int affected_rows = run_query.ExecuteNonQuery();

                if (affected_rows == 0)
                {
                    message = "Failed to delete category, either it does not exist or you do not own it !!!!!!!!";
                }
                else
                {
                    message = "Category is successfully deleted..";
                }

                //close the connection
                connection.Close();

            }
            catch (Exception error)
            {

                //assign the error message from catch
                message = error.Message;
            }//end of catch

            //then return
            return message;

        }//end of deleting_categories method






    }
}
