using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace API_template.backend
{
    public class manage_join_events
    {


        //method to view all categories so a user can choose one
        public string view_all_categories(HttpContext get_session)
        {
            //temp variables
            string message = "Failed to load categories !!!!!!!!";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";
            bool data_found = false;
            int count_categories = 0;

            //try and catch
            try
            {
                //query to select all categories
                string query = @"select id, name, description, organizer_id from category;";

                //connecting to the server and database
                SqlConnection connection = new SqlConnection(connection_string);
                //open the connection
                connection.Open();

                //Execute the command
                SqlCommand run_query = new SqlCommand(query, connection);
                //then execute
                SqlDataReader read_categories = run_query.ExecuteReader();

                //clear the message so we start fresh
                message = "";

                //loop through the rows
                while (read_categories.Read())
                {
                    data_found = true;
                    //get categories by column names and append
                    message += "----------------------------------------------" +
                               "\n    [ Category no " + (count_categories += 1) + " ]      \n" +
                               "----------------------------------------------\n";
                    //appending so message variable does not lose the last data
                    message += "Category   ID: " + read_categories["id"].ToString() + "\n" +
                               "Category NAME: " + read_categories["name"].ToString() + "\n" +
                               "Category Desc: " + read_categories["description"].ToString() + "\n" +
                               "====================================================================\n" +
                               "Category Creator ID: " + read_categories["organizer_id"].ToString() + "\n" +
                               "=====================================================================\n";
                }//end of while loop

                //close the reader
                read_categories.Close();

                //if no data was found, send a friendly message
                if (!data_found)
                {
                    message = "No categories available yet.....";
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

        }//end of view_all_categories method


        //method to view all events inside a chosen category
        public string view_events_by_category(int? category_id, HttpContext get_session)
        {
            //temp variables
            string message = "Failed to load events !!!!!!!!";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";
            bool data_found = false;
            int count_events = 0;

            //try and catch
            try
            {
                //query to select events that belong to this category
                string query = @"select eventID, eventName, description, date, location, distance, eventType, age, startTime, user_id, category_id from Events where category_id=" + category_id + ";";

                //connecting to the server and database
                SqlConnection connection = new SqlConnection(connection_string);
                //open the connection
                connection.Open();

                //Execute the command
                SqlCommand run_query = new SqlCommand(query, connection);
                //then execute
                SqlDataReader read_events = run_query.ExecuteReader();

                //clear the message so we start fresh
                message = "";

                //loop through the rows
                while (read_events.Read())
                {
                    data_found = true;
                    //get events by column names and append
                    message += "----------------------------------------------" +
                               "\n    [ Event no " + (count_events += 1) + " ]      \n" +
                               "----------------------------------------------\n";
                    //appending so message variable does not lose the last data
                    message += "Event   ID: " + read_events["eventID"].ToString() + "\n" +
                               "Event NAME: " + read_events["eventName"].ToString() + "\n" +
                               "Event Desc: " + read_events["description"].ToString() + "\n" +
                               "Event Date: " + Convert.ToDateTime(read_events["date"]).ToString("yyyy-MM-dd") + "\n" +
                               "Event Loca: " + read_events["location"].ToString() + "\n" +
                               "Event Dist: " + read_events["distance"].ToString() + "\n" +
                               "Event Type: " + read_events["eventType"].ToString() + "\n" +
                               "Event  Age: " + read_events["age"].ToString() + "\n" +
                               "Event Start: " + (read_events["startTime"] == DBNull.Value ? "N/A" : ((TimeSpan)read_events["startTime"]).ToString(@"hh\:mm\:ss")) +
                               "\n" + "====================================================================\n" +
                               "Event Category ID: " + read_events["category_id"].ToString() + "\n" +
                               "Event Organizer  ID: " + read_events["user_id"].ToString() + "\n" +
                               "=====================================================================\n";
                }//end of while loop

                //close the reader
                read_events.Close();

                //if no data was found, send a friendly message
                if (!data_found)
                {
                    message = "No events found in this category.....";
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

        }//end of view_events_by_category method


        //method to enroll in an event
        public string store_enrolment(int? event_id, HttpContext get_session)
        {
            //temp variables
            string message = "Failed to enroll in event !!!!!!!!";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

            //try and catch
            try
            {
                //get the user id from the session
                string? user_id = get_session.Session.GetString("user_id");
                int participant_id = Int32.Parse(user_id);

                //check if this user is already enrolled in this event
                string check_query = @"select enrolmentID from Enrolments where eventID=" + event_id + " and user_id=" + participant_id + ";";

                //connecting to the server and database
                SqlConnection connection = new SqlConnection(connection_string);
                //open the connection
                connection.Open();

                //check if already enrolled
                SqlCommand check_command = new SqlCommand(check_query, connection);
                SqlDataReader check_reader = check_command.ExecuteReader();
                bool already_enrolled = check_reader.Read();
                check_reader.Close();

                if (already_enrolled)
                {
                    //close the connection
                    connection.Close();
                    //return the error message
                    return "You are already enrolled in this event.....";
                }

                //insert the enrolment
                string date_today = DateTime.Now.ToString("yyyy-MM-dd");
                string query = @"insert into Enrolments (enrolmentDate, eventID, user_id) values ('" + date_today + "', " + event_id + ", " + participant_id + ");";

                //Execute the command
                SqlCommand run_query = new SqlCommand(query, connection);
                //then execute
                int affected_rows = run_query.ExecuteNonQuery();

                if (affected_rows == 0)
                {
                    message = "Failed to enroll in event !!!!!!!!";
                }
                else
                {
                    message = "successfully enrolled in event.";
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

        }//end of store_enrolment method







        //method to view all of this user's own results
        public string view_my_results(HttpContext get_session)
        {
            //temp variables
            string message = "Failed to load results !!!!!!!!";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";
            bool data_found = false;
            int count_results = 0;

            //try and catch
            try
            {
                //get the user id from the session
                string? user_id = get_session.Session.GetString("user_id");
                int participant_id = Int32.Parse(user_id);

                //join Results → Enrolments → Events, only for this user
                string query = @"select r.resultID, r.finishTime, r.finishPosition, ev.eventName, ev.date from Results r inner join Enrolments e on r.enrolmentID = e.enrolmentID inner join Events ev on e.eventID = ev.eventID where e.user_id=" + participant_id + ";";

                //connecting to the server and database
                SqlConnection connection = new SqlConnection(connection_string);
                //open the connection
                connection.Open();

                //Execute the command
                SqlCommand run_query = new SqlCommand(query, connection);
                //then execute
                SqlDataReader read_results = run_query.ExecuteReader();

                //clear the message so we start fresh
                message = "";

                //loop through the rows
                while (read_results.Read())
                {
                    data_found = true;
                    //get results by column names and append
                    message += "----------------------------------------------" +
                               "\n    [ Result no " + (count_results += 1) + " ]      \n" +
                               "----------------------------------------------\n";
                    //appending so message variable does not lose the last data
                    message += "Result      ID: " + read_results["resultID"].ToString() + "\n" +
                               "Event     Name: " + read_results["eventName"].ToString() + "\n" +
                               "Event     Date: " + Convert.ToDateTime(read_results["date"]).ToString("yyyy-MM-dd") + "\n" +
                               "====================================================================\n" +
                               "Finish    Time: " + ((TimeSpan)read_results["finishTime"]).ToString(@"hh\:mm\:ss") + "\n" +
                               "Finish Position: " + read_results["finishPosition"].ToString() + "\n" +
                               "=====================================================================\n";
                }//end of while loop

                //close the reader
                read_results.Close();

                //if no data was found, send a friendly message
                if (!data_found)
                {
                    message = "You have no recorded results yet.....";
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

        }//end of view_my_results method








        //method to view all events the user has enrolled in
        public string view_my_enrolments(HttpContext get_session)
        {
            //temp variables
            string message = "Failed to load enrolments !!!!!!!!";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";
            bool data_found = false;
            int count_enrolments = 0;

            //try and catch
            try
            {
                //get the user id from the session
                string? user_id = get_session.Session.GetString("user_id");
                int participant_id = Int32.Parse(user_id);

                //query to join Enrolments with Events for this user
                string query = @"select e.enrolmentID, e.enrolmentDate, ev.eventID, ev.eventName, ev.date, ev.location, ev.eventType from Enrolments e inner join Events ev on e.eventID = ev.eventID where e.user_id=" + participant_id + ";";

                //connecting to the server and database
                SqlConnection connection = new SqlConnection(connection_string);
                //open the connection
                connection.Open();

                //Execute the command
                SqlCommand run_query = new SqlCommand(query, connection);
                //then execute
                SqlDataReader read_enrolments = run_query.ExecuteReader();

                //clear the message so we start fresh
                message = "";

                //loop through the rows
                while (read_enrolments.Read())
                {
                    data_found = true;
                    //get enrolments by column names and append
                    message += "----------------------------------------------" +
                               "\n    [ Enrolment no " + (count_enrolments += 1) + " ]      \n" +
                               "----------------------------------------------\n";
                    //appending so message variable does not lose the last data
                    message += "Enrolment   ID: " + read_enrolments["enrolmentID"].ToString() + "\n" +
                               "Enrolment Date: " + Convert.ToDateTime(read_enrolments["enrolmentDate"]).ToString("yyyy-MM-dd") + "\n" +
                               "====================================================================\n" +
                               "Event     ID: " + read_enrolments["eventID"].ToString() + "\n" +
                               "Event   NAME: " + read_enrolments["eventName"].ToString() + "\n" +
                               "Event   Date: " + Convert.ToDateTime(read_enrolments["date"]).ToString("yyyy-MM-dd") + "\n" +
                               "Event   Loca: " + read_enrolments["location"].ToString() + "\n" +
                               "Event   Type: " + read_enrolments["eventType"].ToString() + "\n" +
                               "=====================================================================\n";
                }//end of while loop

                //close the reader
                read_enrolments.Close();

                //if no data was found, send a friendly message
                if (!data_found)
                {
                    message = "You have not enrolled in any events yet.....";
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

        }//end of view_my_enrolments method


        //method to cancel an enrolment
        public string cancel_enrolment(int? enrolment_id, HttpContext get_session)
        {
            //temp variables
            string message = "Failed to cancel enrolment !!!!!!!!";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

            //try and catch
            try
            {
                //get the user id from the session
                string? user_id = get_session.Session.GetString("user_id");
                int participant_id = Int32.Parse(user_id);

                //delete only if this user owns the enrolment
                string query = @"delete from Enrolments where enrolmentID=" + enrolment_id + " and user_id=" + participant_id + ";";

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
                    message = "Failed to cancel enrolment, either it does not exist or you do not own it !!!!!!!!";
                }
                else
                {
                    message = "Enrolment with id " + enrolment_id + " is successfully cancelled..";
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

        }//end of cancel_enrolment method


        //all events from the database (every organizer's)
        public SqlDataReader view_all_eventsSite()
        {
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";
            string query = @"select eventID, eventName, description, date, location, distance, eventType, age, startTime, user_id, category_id, image 
                     from Events order by date asc;";

            SqlConnection connection = new SqlConnection(connection_string);
            connection.Open();

            SqlCommand run_query = new SqlCommand(query, connection);
            return run_query.ExecuteReader();
        }
        public SqlDataReader view_my_resultsSite(int participant_id)
        {
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

            string query = @"select r.resultID, r.finishTime, r.finishPosition, ev.eventName, ev.date 
                     from Results r 
                     inner join Enrolments e on r.enrolmentID = e.enrolmentID 
                     inner join Events ev on e.eventID = ev.eventID 
                     where e.user_id = " + participant_id + ";";

            SqlConnection connection = new SqlConnection(connection_string);
            connection.Open();

            SqlCommand run_query = new SqlCommand(query, connection);
            return run_query.ExecuteReader();
        }

        public SqlDataReader view_my_enrolmentsSite(int participant_id)
        {
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

            //query to join Enrolments with Events for this user — now includes image
            string query = @"select e.enrolmentID, e.enrolmentDate, ev.eventID, ev.eventName, ev.date, ev.location, ev.eventType, ev.image 
                     from Enrolments e 
                     inner join Events ev on e.eventID = ev.eventID 
                     where e.user_id=" + participant_id + ";";

            SqlConnection connection = new SqlConnection(connection_string);
            connection.Open();

            SqlCommand run_query = new SqlCommand(query, connection);
            SqlDataReader read_enrolments = run_query.ExecuteReader();

            return read_enrolments;
        }




        //reader version — all categories
        public SqlDataReader view_all_categoriesSite()
        {
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";
            string query = "select id, name, description, organizer_id from category order by name;";

            SqlConnection connection = new SqlConnection(connection_string);
            connection.Open();

            SqlCommand run_query = new SqlCommand(query, connection);
            return run_query.ExecuteReader();
        }

        //reader version — events by category
        public SqlDataReader view_events_by_categorySite(int category_id)
        {
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";
            string query = @"select eventID, eventName, description, date, location, distance, eventType, age, startTime, user_id, category_id, image 
                     from Events where category_id = " + category_id + " order by date asc;";

            SqlConnection connection = new SqlConnection(connection_string);
            connection.Open();

            SqlCommand run_query = new SqlCommand(query, connection);
            return run_query.ExecuteReader();
        }

    }//end of class

}//end of namespace