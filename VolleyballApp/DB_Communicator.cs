﻿using System;
using System.IO;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace VolleyballApp
{
	public class DB_Communicator
	{
		string connectionString;

		public DB_Communicator ()
		{
			connectionString = "SERVER=localhost;" +
				"DATABASE=Volleyball_App_DB;" +
				"UID=root;" +
				"PASSWORD=root;"+
				"Initial Catalog= volleyballapp";
		}

		public void test() {
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				connection.Open();
				try
				{
					MySqlCommand authentification = new MySqlCommand("SELECT U_NAME FROM User WHERE U_Name == user");
				}
				catch (Exception )
				{

				}
			}
		}
	}
}