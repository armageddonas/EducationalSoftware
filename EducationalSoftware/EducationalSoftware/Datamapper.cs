﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace EducationalSoftware
{
    class Datamapper
    {
        OleDbConnection connection;

       public Datamapper()
        {
            GetConnection();
        }
        /// <summary>
        /// Gets the connection string. Must be initialized once before any other method from the DataMapper is used by a class.
        /// </summary>
        public void GetConnection()
        {
            string connetionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=../../Database2.accdb";
            connection=new OleDbConnection(connetionString);
        }
        /// <summary>
        /// Tries to open the connection and returns true if succeeded or false if not.
        /// </summary>
        /// <returns></returns>
        public bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
        /// <summary>
        /// Tries to close the connection and returns true if succeeded or false if not.
        /// </summary>
        public bool CloseConnection()
        {
            try
            {
                connection.Close();
            }catch(Exception e)
            {
                return false;
            }
            return true;
        }

        private byte[] GenerateHash(string pass,byte[] salt)
        {
            byte[] text = Encoding.ASCII.GetBytes(pass);

            HashAlgorithm alg = new SHA256Managed();
            byte[] txt_salt = new byte[text.Length + salt.Length];

            for (int i = 0; i < pass.Length; i++)
            {
                txt_salt[i] = text[i];
            }

            for (int i = 0; i < salt.Length; i++)
            {
                txt_salt[text.Length + i] = salt[i];
            }

            return alg.ComputeHash(txt_salt);
        }

        /// <summary>
        /// Creates new entry in database, with hash n salt for the password.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public bool Register(string user, string pass)
        {
            try
            {
                //password salt and hashing
                byte[] salt;

                new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
                byte[] hash = GenerateHash(pass, salt);

                //convert byte to equivelant string
                string str_salt = Convert.ToBase64String(salt);
                string str_hash = Convert.ToBase64String(hash);

                OpenConnection();
                string cmd = "INSERT INTO [students] ([username],[password],[salt]) VALUES (@user,@pass,@salt);";
                OleDbCommand command = new OleDbCommand(cmd, connection);
                command.Parameters.AddWithValue("@user", user);
                command.Parameters.AddWithValue("@pass", str_hash);
                command.Parameters.AddWithValue("@salt", str_salt);
                command.ExecuteNonQuery();
                cmd ="INSERT INTO [Stats]([Username],[Stat_1],[Stat_2],[Stat_3],[Stat_4],[Stat_5],[Stat_6],[Stat_7],[Stat_8],[Stat_9],[Stat_10]) VALUES(@username,10, 10, 10, 10, 10, 10, 10, 10, 10, 10)";
                command = new OleDbCommand(cmd, connection);
                command.Parameters.AddWithValue("@username", user);
                command.ExecuteNonQuery();
                cmd = "INSERT INTO [Multipliers]([Username],[multiplier_1],[multiplier_2],[multiplier_3],[multiplier_4],[multiplier_5],[multiplier_6],[multiplier_7],[multiplier_8],[multiplier_9],[multiplier_10]) VALUES(@username,1, 1, 1, 1, 1, 1, 1, 1, 1, 1)";
                command = new OleDbCommand(cmd, connection);
                command.Parameters.AddWithValue("@username", user);
                command.ExecuteNonQuery();
                CloseConnection();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Gets the multiplier for the stats or given user and returns them as a float array.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public float[] GetMultipliers(string username)
        {
            string cmd = "SELECT multiplier_1,multiplier_2,multiplier_3,multiplier_4,multiplier_5,multiplier_6,multiplier_7,multiplier_8,multiplier_9,multiplier_10 FROM Multipliers WHERE Username=?";
            float[] multipliers = new float[10];
            OleDbCommand command = new OleDbCommand(cmd, connection);
            command.Parameters.AddWithValue("@username", username);
            OpenConnection();
            using (OleDbDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    for (int i = 0; i < 10; i++)
                    {
                        multipliers[i] = float.Parse(reader[i].ToString());
                    }

                }
            }
            CloseConnection();
            return multipliers;

        }
        /// <summary>
        /// Gets stats of given user and returns them as a float array.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public float[] GetStats(string username)
        {
            string cmd = "SELECT [Stat_1],[Stat_2],[Stat_3],[Stat_4],[Stat_5],[Stat_6],[Stat_7],[Stat_8],[Stat_9],[Stat_10] FROM Stats WHERE [Username]=@username";
            float[] stats = new float[10];
            OleDbCommand command = new OleDbCommand(cmd,connection);
            command.Parameters.AddWithValue("@username",username);
            OpenConnection();
            using (OleDbDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    for(int i =0; i<10; i++)
                    {
                        stats[i] = float.Parse(reader[i].ToString());
                    }
                  
                }
            }
            CloseConnection();
            return stats;

        }
        /// <summary>
        /// Updates stats into the databse.
        /// </summary>
        /// <param name="stats"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool SaveStats(float[] stats,string username)
        {
            try
            {
                string cmd = "UPDATE Stats SET Stat_1 = ?,Stat_2= ?,Stat_3= ?,Stat_4= ?,Stat_5= ?,Stat_6= ?,Stat_7= ?,Stat_8= ?,Stat_9= ?,Stat_10= ? WHERE Username=?";
                OleDbCommand command = new OleDbCommand(cmd, connection);
                command.Parameters.AddWithValue("@Stat_1", stats[0]);
                command.Parameters.AddWithValue("@Stat_2", stats[1]);
                command.Parameters.AddWithValue("@Stat_3", stats[2]);
                command.Parameters.AddWithValue("@Stat_4", stats[3]);
                command.Parameters.AddWithValue("@Stat_5", stats[4]);
                command.Parameters.AddWithValue("@Stat_6", stats[5]);
                command.Parameters.AddWithValue("@Stat_7", stats[6]);
                command.Parameters.AddWithValue("@Stat_8", stats[7]);
                command.Parameters.AddWithValue("@Stat_9", stats[8]);
                command.Parameters.AddWithValue("@Stat_10", stats[9]);
                command.Parameters.AddWithValue("@username", username);
                OpenConnection();
                command.ExecuteNonQuery();
                CloseConnection();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }
        /// <summary>
        /// Updates multipliers into database.
        /// </summary>
        /// <param name="multipliers"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool SaveMultipliers(float[] multipliers, string username)
        {
            try
            {
                string cmd = "UPDATE [Multipliers] SET [multiplier_1] = @multiplier_1,[multiplier_2] = @multiplier_2,[multiplier_3] = @multiplier_3,[multiplier_4] = @multiplier_4,[multiplier_5] = @multiplier_5,[multiplier_6] = @multiplier_6,[multiplier_7] = @multiplier_7,[multiplier_8] = @multiplier_8,[multiplier_9] = @multiplier_9,[multiplier_10] = @multiplier_10 WHERE [Username]=@username";
                OleDbCommand command = new OleDbCommand(cmd, connection);
                command.Parameters.AddWithValue("@multiplier_1", multipliers[0]);
                command.Parameters.AddWithValue("@multiplier_2", multipliers[1]);
                command.Parameters.AddWithValue("@multiplier_3", multipliers[2]);
                command.Parameters.AddWithValue("@multiplier_4", multipliers[3]);
                command.Parameters.AddWithValue("@multiplier_5", multipliers[4]);
                command.Parameters.AddWithValue("@multiplier_6", multipliers[5]);
                command.Parameters.AddWithValue("@multiplier_7", multipliers[6]);
                command.Parameters.AddWithValue("@multiplier_8", multipliers[7]);
                command.Parameters.AddWithValue("@multiplier_9", multipliers[8]);
                command.Parameters.AddWithValue("@multiplier_10", multipliers[9]);
                command.Parameters.AddWithValue("@username", username);
                OpenConnection();
                command.ExecuteNonQuery();
                CloseConnection();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public int[] GetStatistics(string username, string year,string month)
        {
            try
            {
                string cmd = "SELECT * FROM Statistics WHERE [Username]=@username AND Year([Date]) >= @year AND Month([Date])>= @month";
                int[] statistics = new int[20];
                OleDbCommand command = new OleDbCommand(cmd, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@year",year);
                command.Parameters.AddWithValue("@month",month);
                OpenConnection();
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            statistics[i] += int.Parse(reader[i+3].ToString());
                        }

                    }
                }
                CloseConnection();
                return statistics;
            }
            catch(Exception e)
            {
                throw new Exception("Data error.");
            }
        }
        public bool LoginUser(string username, string password)
        {
            try
            {
                string fetcher = "Select [password],[salt] From students Where [username] = @user";
                OleDbCommand command = new OleDbCommand(fetcher, connection);
                command.Parameters.AddWithValue("@user", username);
                OpenConnection();
                string hashedpass = null;
                string salt = null;
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        hashedpass = reader[0].ToString();
                        salt = reader[1].ToString();
                    }
                }
                CloseConnection();
                //extracts the bytes.
                byte[] hashed = Convert.FromBase64String(hashedpass);
                //retrieving the salt.
                byte[] saltBytes = Convert.FromBase64String(salt);
                byte[] newhash = GenerateHash(password, saltBytes);

                //compare results.
                if (newhash.Length != hashed.Length)
                {
                    throw new UnauthorizedAccessException();
                }

                for(int i = 0; i < hashed.Length; i++)
                {
                    if (hashed[i] != newhash[i])
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

    }
}
