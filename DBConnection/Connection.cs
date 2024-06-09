using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Npgsql;
using NpgsqlTypes;

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

        public static ObservableCollection<CLassCours> cours { get; set; } = new ObservableCollection<CLassCours>(); 
        public static ObservableCollection<FullInfoStudent> infoStudents { get; set; } = new ObservableCollection<FullInfoStudent>();
        public static ObservableCollection<FullInfoLecturer> infoLecturers { get; set; } = new ObservableCollection<FullInfoLecturer>();
        public static ObservableCollection<FullInfoInstructor> infoInstructors { get; set; } = new ObservableCollection<FullInfoInstructor>();
        public static ObservableCollection<CLassUser> classUsers { get; set; } = new ObservableCollection<CLassUser>();
        public static ObservableCollection<User> user { get; set; } = new ObservableCollection<User>();
        public static ObservableCollection<ClassGroup> classGroups { get; set; } = new ObservableCollection<ClassGroup>();
        public static ObservableCollection<ClassStudentGroup> studentGroups { get; set; } = new ObservableCollection<ClassStudentGroup>();
        public static string GetInstructorEmailById(int instructorId)
        {
            string email = null;
            try
            {
                NpgsqlCommand cmd = GetCommand("SELECT \"Email\" FROM \"User\" WHERE \"Id\" = @InstructorId");
                cmd.Parameters.AddWithValue("@InstructorId", instructorId);
                NpgsqlDataReader result = cmd.ExecuteReader();

                if (result.HasRows)
                {
                    result.Read();
                    email = result.GetString(result.GetOrdinal("Email"));
                }
                result.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении адреса электронной почты инструктора: " + ex.Message);
            }
            return email;
        }
        public static void SelectInfoStudent()
        {
            NpgsqlCommand cmd = GetCommand("SELECT \"User\".\"Id\", \"User\".\"Login\", \"User\".\"Password\", \"User\".\"FirstName\", " +
                "\"User\".\"LastName\" , \"User\".\"Patronymic\", \"User\".\"Phone\" , \"User\".\"Email\" , \"User\".\"DateBirth\" , \"User\".\"Role\"," +
                "\"Student\".\"Photo\" ,\"Cours\".\"Category\" ,\"Cours\".\"TheoryHours\" ,\"Cours\".\"DrivingHours\" ,\"Group\".\"NumberGroup\", \"StudentGroup\".\"Student\",  \"StudentGroup\".\"Group\", \"Student\".\"Cours\"" +
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
                            result.GetString(14),
                            result.GetInt32(15),
                            result.GetInt32(16),
                            result.GetInt32(17)
                        ));
                }
            }
            result.Close();
        }
        public static List<FullInfoStudent> GetFullInfoStudents()
        {
            List<FullInfoStudent> infoStudents = new List<FullInfoStudent>();

            try
            {
                NpgsqlCommand cmd = GetCommand("SELECT \"User\".\"Id\", \"User\".\"Login\", \"User\".\"Password\", \"User\".\"FirstName\", " +
                    "\"User\".\"LastName\" , \"User\".\"Patronymic\", \"User\".\"Phone\" , \"User\".\"Email\" , \"User\".\"DateBirth\" , \"User\".\"Role\"," +
                    "\"Student\".\"Photo\" ,\"Cours\".\"Category\" ,\"Cours\".\"TheoryHours\" ,\"Cours\".\"DrivingHours\" ,\"Group\".\"NumberGroup\", \"StudentGroup\".\"Student\",  \"StudentGroup\".\"Group\", \"Student\".\"Cours\"" +
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
                                result.GetString(14),
                                result.GetInt32(15),
                                result.GetInt32(16),
                                result.GetInt32(17)
                            ));
                    }
                }
                result.Close();
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки информации о студентах");
            }

            return infoStudents;
        }

        public static List<FullInfoInstructor> GetFullInfoInstructor()
        {
            List<FullInfoInstructor> infoInstructors = new List<FullInfoInstructor>();

            try
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
            catch
            {
                MessageBox.Show("Ошибка загрузки информации о студентах");
            }

            return infoInstructors;
        }

       
        public static void SelectGroupStudent()
        {
            NpgsqlCommand cmd = GetCommand("SELECT \"Student\", \"Group\" FROM \"StudentGroup\"");
            NpgsqlDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    studentGroups.Add(new ClassStudentGroup(
                            result.GetInt32(0),
                            result.GetInt32(1)
                        ));
                }
            }
            result.Close();
        }
        

        public static void SelectGroup()
        {
            NpgsqlCommand cmd = GetCommand("SELECT \"Group\".\"Id\", \"Group\".\"NumberGroup\", \"Group\".\"Lecturer\"," +
                "\"User\".\"FirstName\", \"User\".\"LastName\" , \"User\".\"Patronymic\"" +
                "FROM \"User\", \"Lecturer\", \"Group\"" +
                "WHERE \"User\".\"Id\" = \"Lecturer\".\"Id\" AND \"Group\".\"Lecturer\" = \"Lecturer\".\"Id\"");
            NpgsqlDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    classGroups.Add(new ClassGroup(
                            result.GetInt32(0),
                            result.GetString(1),
                            result.GetInt32(2),
                            result.GetString(3),
                            result.GetString(4),
                            result.GetString(5)
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
        public static void SelectCoursStudent()
        {
            NpgsqlCommand cmd = GetCommand("SELECT \"Id\", \"Category\", \"TheoryHours\", \"DrivingHours\" FROM \"Cours\"");
            NpgsqlDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    cours.Add(new CLassCours(result.GetInt32(0), result.GetString(1), result.GetString(2), result.GetString(3)));
                }
            }
            result.Close();
        }
        public static void InsertUsers(User users)
        {
            NpgsqlCommand cmd = GetCommand("insert into \"User\" (\"Login\", \"Password\", \"FirstName\", \"LastName\", \"Patronymic\", \"Phone\", \"Email\", \"DateBirth\", \"Role\")" +
                "values (@login, @password, @firstName, @lastName, @patronymic, @phone, @email, @dateBirth, @role)");
            cmd.Parameters.AddWithValue("@login", NpgsqlDbType.Varchar, users.Login);
            cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Varchar, users.Password);
            cmd.Parameters.AddWithValue("@firstName", NpgsqlDbType.Varchar, users.FirstName);
            cmd.Parameters.AddWithValue("@lastName", NpgsqlDbType.Varchar, users.LastName);
            cmd.Parameters.AddWithValue("@patronymic", NpgsqlDbType.Varchar, users.Patronymic);
            cmd.Parameters.AddWithValue("@phone", NpgsqlDbType.Varchar, users.Phone);
            cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, users.Email);
            cmd.Parameters.AddWithValue("@dateBirth", NpgsqlDbType.Date, users.DateBirth);
            cmd.Parameters.AddWithValue("@role", NpgsqlDbType.Varchar, users.Role);
            var result = cmd.ExecuteNonQuery();
        }
    }
}
