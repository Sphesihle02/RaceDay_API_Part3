using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace API_template.backend
{
    public class manage_events
    {


        //method for storing/inserting a new event
        public string store_event(string eventName, string description, DateTime? date, string location, decimal? distance, string eventType, int? age, TimeSpan? startTime, int? category_id, HttpContext get_session)
        {
            //temp variables
            string message = "Failed to create event !!!!!!!!";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

            //try and catch
            try
            {
                //get the organizer id from the session
                string? organizer_id = get_session.Session.GetString("user_id");
                int owner_id = Int32.Parse(organizer_id);

                //query to insert a new event
                string dateFormated = ""+date?.ToString("yyyy-MM-dd");
                string starterTime = "" + startTime?.ToString(@"hh\:mm\:ss");
                string query = @"insert into Events (eventName, description, date, location, distance, eventType, age, startTime, user_id, category_id) values ('" + eventName + "', '" + description + "', '" + dateFormated+ "', '" + location + "', " + distance + ", '" + eventType + "', " + age + ", '" + starterTime + "', " + owner_id + ", " + category_id + ");";

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
                    message = "Failed to create event !!!!!!!!";
                }
                else
                {
                    message = "Event is successfully created..";
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

        }//end of store_event method


        //method for storing/inserting a new event
        public string store_eventSite(string eventName, string description, DateTime? date, string location, decimal? distance, string eventType, int? age, TimeSpan? startTime, int? category_id, string image, int owner_id)
        {
            //temp variables
            string message = "Failed to create event !!!!!!!!";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

            //try and catch
            try
            {
                //get the organizer id from the session
                //string? organizer_id = get_session.Session.GetString("user_id");
               // int owner_id = Int32.Parse(organizer_id);

                //query to insert a new event
                string dateFormated = "" + date?.ToString("yyyy-MM-dd");
                string starterTime = "" + startTime?.ToString(@"hh\:mm\:ss");
                string query = @"insert into Events (eventName, description, date, location, distance, eventType, age, startTime, user_id, category_id, image) values ('" + eventName + "', '" + description + "', '" + dateFormated + "', '" + location + "', " + distance + ", '" + eventType + "', " + age + ", '" + starterTime + "', " + owner_id + ", " + category_id + ", '" + image + "');";

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
                    message = "Failed to create event !!!!!!!!";
                }
                else
                {
                    message = "Event is successfully created..";
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

        }//end of store_event method






        //method for viewing all events that belong to this organizer
        public string view_events(HttpContext get_session)
        {
            //temp variables
            string message = "Failed to load events !!!!!!!!";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";
            bool data_found = false;
            int count_events = 0;

            //try and catch
            try
            {
                //get the organizer id from the session
                string? organizer_id = get_session.Session.GetString("user_id");
                int owner_id = Int32.Parse(organizer_id);

                //query to select only this organizer's events
                string query = @"select eventID, eventName, description, date, location, distance, eventType, age, startTime, category_id, user_id from Events where user_id=" + owner_id + ";";

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
                               //"Event Loca: " + read_events["location"].ToString() + "\n" +
                               "Event Dist: " + read_events["distance"].ToString() + "\n" +
                               "Event Type: " + read_events["eventType"].ToString() + "\n" +
                               "Event  Age: " + read_events["age"].ToString() + "\n" +
                               "Event Start: " + read_events["startTime"].ToString() + "\n" +
                               "====================================================================\n" +
                               "Event Category ID: " + read_events["category_id"].ToString() + "\n" +
                               "Event Creator  ID: " + read_events["user_id"].ToString() + "\n" +
                               "=====================================================================\n";

                }//end of while loop

                //close the reader
                read_events.Close();

                //if no data was found, send a friendly message
                if (!data_found)
                {
                    message = "No events found for this organizer.....";
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

        }//end of view_events method



        //method for viewing all events that belong to this organizer
        public SqlDataReader view_eventsSite(int owner_id)
        {
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

            //query to select only this organizer's events
            string query = @"select eventID, eventName, description, date, location, distance, eventType, age, startTime, category_id, user_id,image from Events where user_id=" + owner_id + ";";

            //connecting to the server and database
            SqlConnection connection = new SqlConnection(connection_string);
            //open the connection
            connection.Open();

            //Execute the command
            SqlCommand run_query = new SqlCommand(query, connection);
            //then execute
            SqlDataReader read_events = run_query.ExecuteReader();

            //return the reader so the caller can loop through it
            return read_events;
        }

        //method for updating_events
        public string updating_events(int? event_id, string? eventName, string? description, DateTime? date, string? location, decimal? distance, string? eventType, int? age, TimeSpan? startTime, int? category_id, string organizer_id, HttpContext get_session)
        {
            //temp variables
            string message = "Failed to update event !!!!!!!!";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

            //try and catch
            try
            {
                //temp for query
                int owner_id = Int32.Parse(organizer_id);

                //building a SET of clause  based on which fields were provided
                List<string> sets = new List<string>();

                if (!string.IsNullOrWhiteSpace(eventName))
                {
                    sets.Add("eventName='" + eventName + "'");
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    sets.Add("description='" + description + "'");
                }

                if (date != null)
                {
                    sets.Add("date='" + date.Value.ToString("yyyy-MM-dd") + "'");
                }

                if (!string.IsNullOrWhiteSpace(location))
                {
                    sets.Add("location='" + location + "'");
                }

                if (distance != null)
                {
                    sets.Add("distance=" + distance.Value);
                }

                if (!string.IsNullOrWhiteSpace(eventType))
                {
                    sets.Add("eventType='" + eventType + "'");
                }

                if (age != null)
                {
                    sets.Add("age=" + age.Value);
                }

                if (startTime != null)
                {
                    sets.Add("startTime='" + startTime.Value.ToString(@"hh\:mm\:ss") + "'");
                }

                if (category_id != null)
                {
                    sets.Add("category_id=" + category_id.Value);
                }

                if (sets.Count == 0)
                {
                    return "Nothing to update, all fields were empty.....";
                }

                //query to update the event only if this organizer owns it
                string query = "update Events set " + string.Join(", ", sets) + " where eventID=" + event_id + " and user_id=" + owner_id + ";";

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
                    message = "Failed to update event, either it does not exist or you do not own it !!!!!!!!";
                }
                else
                {
                    message = "Event with id " + event_id + " is successfully updated..";
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

        }//end of updating_events method


        //method for deleting_events
        public string deleting_events(int? event_id, string organizer_id, HttpContext get_session)
        {
            //temp variables
            string message = "Failed to delete event !!!!!!!!";
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

            //try and catch
            try
            {
                //temp for query
                int owner_id = Int32.Parse(organizer_id);

                //query to delete only if this organizer owns the event
                string query = @"delete from Events where eventID=" + event_id + " and user_id=" + owner_id + ";";

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
                    message = "Failed to delete event, either it does not exist or you do not own it !!!!!!!!";
                }
                else
                {
                    message = "Event with id " + event_id + " is successfully deleted..";
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

        }//end of deleting_events method




        public int count_participants(int evId)
        {
            int count = 0;
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();

                    using (SqlCommand run_query = new SqlCommand("select count(*) from Enrolments where eventID = @id;", connection))
                    {
                        run_query.Parameters.AddWithValue("@id", evId);

                        object result = run_query.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            count = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return 0;
            }

            return count;
        }




        public string update_event(int event_id, string eventName, string description, DateTime date, string location, decimal distance, string eventType, int age, TimeSpan startTime, int category_id, int organizer_id, string image)
        {
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();

                    string query = @"update Events set 
                eventName = @name, description = @desc, date = @date, location = @location, 
                distance = @distance, eventType = @type, age = @age, startTime = @start, 
                category_id = @cid, image = @image 
                where eventID = @id and user_id = @uid;";

                    using (SqlCommand run = new SqlCommand(query, connection))
                    {
                        run.Parameters.AddWithValue("@name", eventName);
                        run.Parameters.AddWithValue("@desc", description ?? "");
                        run.Parameters.AddWithValue("@date", date.Date);
                        run.Parameters.AddWithValue("@location", location);
                        run.Parameters.AddWithValue("@distance", distance);
                        run.Parameters.AddWithValue("@type", eventType);
                        run.Parameters.AddWithValue("@age", age);
                        run.Parameters.AddWithValue("@start", startTime);
                        run.Parameters.AddWithValue("@cid", category_id);
                        run.Parameters.AddWithValue("@image", string.IsNullOrEmpty(image) ? (object)DBNull.Value : image);
                        run.Parameters.AddWithValue("@id", event_id);
                        run.Parameters.AddWithValue("@uid", organizer_id);

                        int affected = run.ExecuteNonQuery();

                        if (affected > 0)
                        {
                            return "Event is successfully updated..";
                        }

                        return "Event not found or you do not own it.";
                    }
                }
            }
            catch (Exception error)
            {
                return error.Message;
            }
        }

        public SqlDataReader view_event_by_id(int event_id, int organizer_id)
        {
            string connection_string = @"Data source=(localdb)\part_3Api;database=part_3;";
            string query = @"select eventID, eventName, description, date, location, distance, eventType, age, startTime, category_id, user_id, image 
                     from Events where eventID = " + event_id + " and user_id = " + organizer_id + ";";

            SqlConnection connection = new SqlConnection(connection_string);
            connection.Open();

            SqlCommand run_query = new SqlCommand(query, connection);
            return run_query.ExecuteReader();
        }

        //delete an event
        public string delete_event(int event_id, int organizer_id)
        {
            string connection_string = @"Data source=(localdb)\part_2_api;database=part_2;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    connection.Open();

                    using (SqlCommand checkUse = new SqlCommand("select count(*) from Enrolments where eventID = @id;", connection))
                    {
                        checkUse.Parameters.AddWithValue("@id", event_id);
                        int inUse = Convert.ToInt32(checkUse.ExecuteScalar());

                        if (inUse > 0)
                        {
                            return "Cannot delete this event - participants have joined.";
                        }
                    }

                    using (SqlCommand run = new SqlCommand("delete from Events where eventID = @id and user_id = @uid;", connection))
                    {
                        run.Parameters.AddWithValue("@id", event_id);
                        run.Parameters.AddWithValue("@uid", organizer_id);

                        int affected = run.ExecuteNonQuery();

                        if (affected > 0)
                        {
                            return "Event is successfully deleted..";
                        }

                        return "Event not found or you do not own it.";
                    }
                }
            }
            catch (Exception error)
            {
                return error.Message;
            }
        }



    }//end of class

}//end of namespace