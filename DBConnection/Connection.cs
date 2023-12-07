using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace DBConnection
{
    public class Connection
    {

        public static CLassUser users;
        public static NpgsqlConnection connection;

        public static void Connect(string host, string port, string user, string pass, string database)
        {
            string cs = string.Format("Server = {0}; Port = {1}; User Id = {2}; Password = {3}; Database = {4}", host, port, user, pass, database);

            connection = new NpgsqlConnection(cs);
            connection.Open();
        }

        public static NpgsqlCommand GetCommand(string sql)
        {
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = connection;
            command.CommandText = sql;
            return command;
        }
        public static ObservableCollection<FullInfoStudent> infoStudents { get; set; } = new ObservableCollection<FullInfoStudent>();
        public static ObservableCollection<FullInfoLecturer> infoLecturers { get; set; } = new ObservableCollection<FullInfoLecturer>();
        public static ObservableCollection<FullInfoInstructor> infoInstructors { get; set; } = new ObservableCollection<FullInfoInstructor>();
        public static ObservableCollection<CLassUser> classUsers { get; set; } = new ObservableCollection<CLassUser>();

        public static void SelectInfoStudent()
        {
            NpgsqlCommand cmd = GetCommand("SELECT \"User\".\"Id\", \"User\".\"Login\", \"User\".\"Password\", \"User\".\"FirstName\", " +
                "\"User\".\"LastName\" , \"User\".\"Patronymic\", \"User\".\"Phone\" , \"User\".\"Email\" , \"User\".\"DateBirth\" , \"User\".\"Role\"," +
                "\"Student\".\"Photo\" ,\"Cours\".\"Category\" ,\"Cours\".\"TheoryHours\" ,\"Cours\".\"DrivingHours\" ,\"Group\".\"NumberGroup\"" +
                "FROM \"User\", \"Student\", \"Cours\", \"StudentGroup\",\"Group\"" +
                "WHERE \"User\".\"Id\" = \"Student\".\"Id\" AND \"Student\".\"Cours\" = \"Cours\".\"Id\" AND \"Student\".\"Id\" = \"StudentGroup\".\"Student\"" +
                "AND \"StudentGroup\".\"Group\" = \"Group\".\"Id\"");
            NpgsqlDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    infoStudents.Add(new FullInfoStudent(
                            result.GetInt32(0),
                            result.GetString(1),
                            result.GetString(2),
                            result.GetString(3),
                            result.GetString(4),
                            result.GetString(5),
                            result.GetString(6),
                            result.GetString(7),
                            result.GetDateTime(8),
                            result.GetString(9),
                            result.GetString(10),
                            result.GetString(11),
                            result.GetString(12),
                            result.GetString(13),
                            result.GetString(14)
                        ));
                }
            }
            result.Close();
        }

        public static void SelectInfoLecturers()
        {
            NpgsqlCommand cmd = GetCommand("SELECT \"User\".\"Id\", \"User\".\"Login\", \"User\".\"Password\", \"User\".\"FirstName\", \"User\".\"LastName\"," +
                "\"User\".\"Patronymic\", \"User\".\"Phone\" , \"User\".\"Email\" , \"User\".\"DateBirth\" , \"User\".\"Role\", \"Lecturer\".\"DateEmployment\"" +
                "FROM \"User\", \"Lecturer\"" +
                "WHERE \"User\".\"Id\" = \"Lecturer\".\"Id\"");
            NpgsqlDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    infoLecturers.Add(new FullInfoLecturer(
                            result.GetInt32(0),
                            result.GetString(1),
                            result.GetString(2),
                            result.GetString(3),
                            result.GetString(4),
                            result.GetString(5),
                            result.GetString(6),
                            result.GetString(7),
                            result.GetDateTime(8),
                            result.GetString(9),
                            result.GetDateTime(10)
                        ));
                }

            }
            result.Close();
        }

        public static void SelectInfoInstructors()
        {
            NpgsqlCommand cmd = GetCommand("SELECT \"User\".\"Id\", \"User\".\"Login\", \"User\".\"Password\", \"User\".\"FirstName\", \"User\".\"LastName\"," +
                "\"User\".\"Patronymic\", \"User\".\"Phone\" , \"User\".\"Email\" , \"User\".\"DateBirth\" , \"User\".\"Role\", \"Instructor\".\"DateEmployment\", \"Instructor\".\"DrivingExperience\"" +
                "FROM \"User\", \"Instructor\"" +
                "WHERE \"User\".\"Id\" = \"Instructor\".\"Id\"");
            NpgsqlDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    infoInstructors.Add(new FullInfoInstructor(
                            result.GetInt32(0),
                            result.GetString(1),
                            result.GetString(2),
                            result.GetString(3),
                            result.GetString(4),
                            result.GetString(5),
                            result.GetString(6),
                            result.GetString(7),
                            result.GetDateTime(8),
                            result.GetString(9),
                            result.GetDateTime(10),
                            result.GetString(11)
                        ));
                }

            }
            result.Close();
        }

    }
}
