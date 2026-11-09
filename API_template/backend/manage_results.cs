using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace API_template.backend
{
    public class manage_results
    {


        //method to view all enrolments in an event owned by this organizer
        public string view_enrolments_by_event(int? event_id, string organizer_id, HttpContext get_session)
        {
            //temp variables
            string message = "Failed to load enrolments !!!!!!!!";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";
            bool data_found = false;
            int count_enrolments = 0;

            //try and catch
            try
            {
                //temp for query
                int owner_id = Int32.Parse(organizer_id);

                //join Enrolments with users, and only for events this organizer owns
                string query = @"select e.enrolmentID, e.enrolmentDate, u.fullname, u.email, r.resultID, r.finishTime, r.finishPosition from Enrolments e inner join Events ev on e.eventID = ev.eventID inner join users u on e.user_id = u.id left join Results r on r.enrolmentID = e.enrolmentID where ev.eventID=" + event_id + " and ev.user_id=" + owner_id + ";";

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
                               "Participant Name: " + read_enrolments["fullname"].ToString() + "\n" +
                               "Participant Email: " + read_enrolments["email"].ToString() + "\n" +
                               "====================================================================\n";

                    //check if a result exists for this enrolment
                    if (read_enrolments["resultID"] == DBNull.Value)
                    {
                        message += "Result: NOT YET RECORDED\n" +
                                   "=====================================================================\n";
                    }
                    else
                    {
                        message += "Result      ID: " + read_enrolments["resultID"].ToString() + "\n" +
                                   "Finish    Time: " + ((TimeSpan)read_enrolments["finishTime"]).ToString(@"hh\:mm\:ss") + "\n" +
                                   "Finish Position: " + read_enrolments["finishPosition"].ToString() + "\n" +
                                   "=====================================================================\n";
                    }
                }//end of while loop

                //close the reader
                read_enrolments.Close();

                //if no data was found, send a friendly message
                if (!data_found)
                {
                    message = "No enrolments found for this event, or you do not own it.....";
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

        }//end of view_enrolments_by_event method

        //method to view all enrolments in an event owned by this organizer
        public SqlDataReader view_enrolments_by_eventSite(int event_id, int organizer_id)
        {
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

            //join Enrolments with users, and only for events this organizer owns
            string query = @"select e.enrolmentID, e.enrolmentDate, u.fullname, u.email, r.resultID, r.finishTime, r.finishPosition 
                     from Enrolments e 
                     inner join Events ev on e.eventID = ev.eventID 
                     inner join users u on e.user_id = u.id 
                     left join Results r on r.enrolmentID = e.enrolmentID 
                     where ev.eventID=" + event_id + " and ev.user_id=" + organizer_id + ";";

            //connecting to the server and database
            SqlConnection connection = new SqlConnection(connection_string);
            //open the connection
            connection.Open();

            //Execute the command
            SqlCommand run_query = new SqlCommand(query, connection);
            //then execute
            SqlDataReader read_enrolments = run_query.ExecuteReader();

            //return the reader so the caller can loop through it
            return read_enrolments;
        }


        public SqlDataReader view_my_enrolmentsSite(int participant_id)
        {
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

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

            //return the reader so the caller can loop through it
            return read_enrolments;
        }

        //method to record a result for an enrolment
        public string store_result(int? enrolment_id, TimeSpan? finishTime, int? finishPosition, string organizer_id, HttpContext get_session)
        {
            //temp variables
            string message = "Failed to record result !!!!!!!!";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

            //try and catch
            try
            {
                //temp for query
                int owner_id = Int32.Parse(organizer_id);

                //connecting to the server and database
                SqlConnection connection = new SqlConnection(connection_string);
                //open the connection
                connection.Open();

                //first check that this enrolment belongs to an event this organizer owns
                string check_query = @"select e.enrolmentID from Enrolments e inner join Events ev on e.eventID = ev.eventID where e.enrolmentID=" + enrolment_id + " and ev.user_id=" + owner_id + ";";

                SqlCommand check_command = new SqlCommand(check_query, connection);
                SqlDataReader check_reader = check_command.ExecuteReader();
                bool owned = check_reader.Read();
                check_reader.Close();

                if (!owned)
                {
                    //close the connection
                    connection.Close();
                    //return the error message
                    return "Failed to record result, either the enrolment does not exist or you do not own the event !!!!!!!!";
                }

                //check if a result already exists for this enrolment
                string dup_query = @"select resultID from Results where enrolmentID=" + enrolment_id + ";";

                SqlCommand dup_command = new SqlCommand(dup_query, connection);
                SqlDataReader dup_reader = dup_command.ExecuteReader();
                bool already_exists = dup_reader.Read();
                dup_reader.Close();

                if (already_exists)
                {
                    //close the connection
                    connection.Close();
                    //return the error message
                    return "A result is already recorded for this enrolment, use update instead.....";
                }

                //insert the result
                string query = @"insert into Results (finishTime, finishPosition, enrolmentID) values ('" + finishTime.Value.ToString(@"hh\:mm\:ss") + "', " + finishPosition + ", " + enrolment_id + ");";

                SqlCommand run_query = new SqlCommand(query, connection);
                int affected_rows = run_query.ExecuteNonQuery();

                if (affected_rows == 0)
                {
                    message = "Failed to record result !!!!!!!!";
                }
                else
                {
                    message = "Result is successfully recorded..";
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

        }//end of store_result method


        //method to update a result
        public string updating_results(int? result_id, TimeSpan? finishTime, int? finishPosition, string organizer_id, HttpContext get_session)
        {
            //temp variables
            string message = "Failed to update result !!!!!!!!";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

            //try and catch
            try
            {
                //temp for query
                int owner_id = Int32.Parse(organizer_id);

                //build the SET clause dynamically based on which fields were provided
                List<string> sets = new List<string>();

                if (finishTime != null)
                {
                    sets.Add("finishTime='" + finishTime.Value.ToString(@"hh\:mm\:ss") + "'");
                }

                if (finishPosition != null)
                {
                    sets.Add("finishPosition=" + finishPosition.Value);
                }

                if (sets.Count == 0)
                {
                    return "Nothing to update, all fields were empty.....";
                }

                //update only if this organizer owns the event of this result
                string query = "update Results set " + string.Join(", ", sets) + " where resultID=" + result_id + " and enrolmentID in (select e.enrolmentID from Enrolments e inner join Events ev on e.eventID = ev.eventID where ev.user_id=" + owner_id + ");";

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
                    message = "Failed to update result, either it does not exist or you do not own it !!!!!!!!";
                }
                else
                {
                    message = "Result with id " + result_id + " is successfully updated..";
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

        }//end of updating_results method


        //method to delete a result
        public string deleting_results(int? result_id, string organizer_id, HttpContext get_session)
        {
            //temp variables
            string message = "Failed to delete result !!!!!!!!";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

            //try and catch
            try
            {
                //temp for query
                int owner_id = Int32.Parse(organizer_id);

                //delete only if this organizer owns the event of this result
                string query = @"delete from Results where resultID=" + result_id + " and enrolmentID in (select e.enrolmentID from Enrolments e inner join Events ev on e.eventID = ev.eventID where ev.user_id=" + owner_id + ");";

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
                    message = "Failed to delete result, either it does not exist or you do not own it !!!!!!!!";
                }
                else
                {
                    message = "Result with id " + result_id + " is successfully deleted..";
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

        }//end of deleting_results method


    }//end of class

}//end of namespace