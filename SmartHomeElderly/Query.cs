using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SmartHomeElderly
{
    internal class Query
    {
        public static string name,surname,AMKA,dateofbirth, region, address;
        public static int weight;
        public static string[] phoneNumbers;
        readonly SQLiteConnection conn = new SQLiteConnection("Data Source=DBElderly.db;Version=3;");

        public string GetAMKA()
        {
            return AMKA;
        }

        public string GetFullname()
        {
            return name + ' ' + surname;
        }

        public void Register(string name, string surname, string amka, string date, int weight, string region, string address, string phones)
        {
            conn.Open();
            string insertQuery = "Insert into Elderly(Name,Surname,AMKA,DateOfBirth,Weight,Region,Address,PhoneNumbers) values(@name,@surname,@amka,@date,@weight,@region,@address,@phoneNumbers)";
            SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@surname", surname);
            cmd.Parameters.AddWithValue("@amka", amka);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@weight", weight);
            cmd.Parameters.AddWithValue("@region", region);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@phoneNumbers", phones);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void UpdateData(int w, string r, string a, string phones)
        {
            weight = w;
            region = r;
            address = a;
            phoneNumbers = phones.Split(',');
            conn.Open();
            string updateQuery = "Update Elderly set Weight=@weight,Region=@region,Address=@address,PhoneNumbers=@phoneNumbers where AMKA=@amka";
            SQLiteCommand cmd = new SQLiteCommand(updateQuery, conn);
            cmd.Parameters.AddWithValue("@amka", AMKA);
            cmd.Parameters.AddWithValue("@weight", weight);
            cmd.Parameters.AddWithValue("@region", region);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@phoneNumbers", phones);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void InsertTask(string time, string task, string countdown)
        {
            conn.Open();
            string insertQuery = "Insert into Task(AMKA, Time, Todo, Countdown, Bool) values(@amka, @time, @todo, @countdown, @bool)";
            SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@amka", AMKA);
            cmd.Parameters.AddWithValue("@time", time);
            cmd.Parameters.AddWithValue("@todo", task);
            cmd.Parameters.AddWithValue("@countdown", int.Parse(countdown));
            cmd.Parameters.AddWithValue("@bool", 0);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public (string, string) AlreadyRegisteredTask(string time)
        {
            conn.Open();
            string selectQuery = "Select * from Task where AMKA=@amka order by Time asc";
            SQLiteCommand command = new SQLiteCommand(selectQuery, conn);
            command.Parameters.AddWithValue("@amka", AMKA);
            SQLiteDataReader reader = command.ExecuteReader();
            StringBuilder builder = new StringBuilder();
            int c = 0;
            bool flag = false;
            string id_tasks = string.Empty;
            while (reader.Read())
            {
                if (time.Equals(string.Empty))
                {
                    id_tasks += reader.GetInt32(0).ToString() + ',';
                    builder.Append(reader.GetInt32(0)).Append(" , ")
                        .Append(reader.GetString(2)).Append(" , ")
                        .Append(reader.GetString(3)).Append(" , ")
                        .Append(reader.GetInt16(4)).Append(Environment.NewLine);
                    flag = true;
                }
                else
                {
                    (double milli1, double milli2) = milliseconds(time,reader.GetString(2));
                    if ((milli1 - 600000 <= milli2) && (milli1 + 600000 >= milli2)) c++;
                }
            }
            conn.Close();
            if (flag)
            {
                return (builder.ToString(), id_tasks);
            }                
            return (c.ToString(), null) ;
        }

        public void DeleteTask(int id)
        {
            conn.Open();
            String delete = "Delete from Task where ID=@id";
            SQLiteCommand cmd = new SQLiteCommand(delete, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public string SelectElderly()
        {
            conn.Open();
            string selectQuery = "Select * from Elderly where AMKA=@amka";
            SQLiteCommand command = new SQLiteCommand(selectQuery, conn);
            command.Parameters.AddWithValue("@amka", AMKA);
            SQLiteDataReader reader = command.ExecuteReader();
            string message = string.Empty;
            if (reader.Read())
            {
                string year = reader.GetString(4).Split('/').Last();
                message = reader.GetString(1) + " " + reader.GetString(2) + Environment.NewLine + "ΑΜΚΑ: " + reader.GetString(3) + age(int.Parse(year)) + reader.GetString(7) + ", " + reader.GetString(6);
            }
            conn.Close();
            return message;
        }

        public bool Login(string amka)
        {
            bool flag = false;
            conn.Open();
            string selectQuery = "Select * from Elderly where AMKA=@amka";
            SQLiteCommand command = new SQLiteCommand(selectQuery, conn);
            command.Parameters.AddWithValue("@amka", amka);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                flag = true;
                name = reader.GetString(1);
                surname = reader.GetString(2);
                AMKA = reader.GetString(3);
                dateofbirth = reader.GetString(4);
                weight = int.Parse(reader.GetString(5));
                region = reader.GetString(6);
                address = reader.GetString(7);
                phoneNumbers = reader.GetString(8).Split(',');
            }
            conn.Close();
            return flag;
        }

        public string[] GetAllPhones()
        {
            List<string> allPhones = new List<string>();
            conn.Open();
            string selectQuery = "Select * from Elderly where AMKA=@amka";
            SQLiteCommand command = new SQLiteCommand(selectQuery, conn);
            command.Parameters.AddWithValue("@amka", AMKA);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                if (reader.GetString(8).Contains(',')) allPhones.AddRange(reader.GetString(8).Split(',').ToList());
                else allPhones.Add(reader.GetString(8));
            }
            reader.Dispose();
            conn.Close();
            return allPhones.ToArray();
        }

        public string GetFullName()
        {
            string fullname = "";
            conn.Open();
            string selectQuery = "Select Name,Surname from Elderly where AMKA=@amka";
            SQLiteCommand command = new SQLiteCommand(selectQuery, conn);
            command.Parameters.AddWithValue("@amka", AMKA);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                fullname = reader.GetString(0) + ' ' + reader.GetString(1);
                reader.Dispose();
            }
            conn.Close();
            return fullname;
        }

        private string age(int year)
        {
            int years = DateTime.Now.Year - year;
            return string.Format(Environment.NewLine + "{0} χρονών" + Environment.NewLine, years);
        }

        private (double, double) milliseconds(string time1,string time2)
        {
            string[] hour_min1 = time1.Split(':');
            TimeSpan milli1 = new TimeSpan(int.Parse(hour_min1[0]), int.Parse(hour_min1[1]), 0);
            string[] hour_min2 = time2.Split(':');
            TimeSpan milli2 = new TimeSpan(int.Parse(hour_min2[0]), int.Parse(hour_min2[1]), 0);
            return (milli1.TotalMilliseconds, milli2.TotalMilliseconds);
        }
    }
}
