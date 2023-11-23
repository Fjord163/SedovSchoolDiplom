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
        public static ObservableCollection<CLassUser> classUsers { get; set; } = new ObservableCollection<CLassUser>();
        public static ObservableCollection<ClassCar> classCars { get; set; } = new ObservableCollection<ClassCar>();
        public static ObservableCollection<ClassCategory> classCategories { get; set; } = new ObservableCollection<ClassCategory>();
        public static ObservableCollection<ClassColorCar> classColorCars { get; set; } = new ObservableCollection<ClassColorCar>();
        public static ObservableCollection<CLassCours> classCours { get; set; } = new ObservableCollection<CLassCours>();
        public static ObservableCollection<ClassDrivingLesson> classDrivingLessons { get; set; } = new ObservableCollection<ClassDrivingLesson>();
        public static ObservableCollection<ClassExam> classExams { get; set; } = new ObservableCollection<ClassExam>();
        public static ObservableCollection<ClassGroup>  classGroups { get; set; } = new ObservableCollection<ClassGroup>();
        public static ObservableCollection<ClassModelCar> classModelCars { get; set; } = new ObservableCollection<ClassModelCar>();
        public static ObservableCollection<ClassNameWeek> classNameWeeks { get; set; } = new ObservableCollection<ClassNameWeek>();
        public static ObservableCollection<ClassNumberGroup> classNumberGroups { get; set; } = new ObservableCollection<ClassNumberGroup>();
        public static ObservableCollection<ClassNumberWeek> classNumberWeeks { get; set; } = new ObservableCollection<ClassNumberWeek>();
        public static ObservableCollection<ClassOrder>  classOrders { get; set; } = new ObservableCollection<ClassOrder>();
        public static ObservableCollection<ClassResultExam> classResultExams { get; set; } = new ObservableCollection<ClassResultExam>();
        public static ObservableCollection<ClassService> classService { get; set; } = new ObservableCollection<ClassService>();
        public static ObservableCollection<ClassServices> classServices { get; set; } = new ObservableCollection<ClassServices>();
        public static ObservableCollection<ClassStampCar> classStampCars { get; set; } = new ObservableCollection<ClassStampCar>();
        public static ObservableCollection<ClassStatusPayment> classStatusPayments { get; set; } = new ObservableCollection<ClassStatusPayment>();
        public static ObservableCollection<ClassTimetableLecture> classTimetableLectures { get; set; } = new ObservableCollection<ClassTimetableLecture>();
        public static ObservableCollection<ClassWeekday> classWeekdays { get; set; } = new ObservableCollection<ClassWeekday>();
       
        
        /*
        public void SelectTableDrivingLesson()
        {
            NpgsqlCommand cmd = GetCommand("SELECT \"Id\",\"DateTime\",\"Student\",\"Car\" FROM \"DrivingLesson\"");
            NpgsqlDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    classDrivingLessons.Add(new ClassDrivingLesson(result.GetInt32(0), result.GetDateTime(1), result.GetInt32(2), result.GetString(3)));
                }

            }
            result.Close();
        }

        public static void Select()
        {
            NpgsqlCommand cmd = GetCommand("SELECT \"Student\".\"Id\", \"Student\".\"FirstName\", \"Student\".\"LastName\" , \"Student\".\"Patronymic\" ," +
                " \"Student\".\"Password\",\"Student\".\"Phone\" , \"Student\".\"Email\" , \"Student\".\"Photo\" , \"Student\".\"Cours\" , \"Student\".\"Login\"," +
                "\"StudentInstructor\".\"Student\" ,\"StudentInstructor\".\"Instructor\"" +
                " FROM \"Student\", \"StudentInstructor\" " +
                "WHERE \"Student\".\"Id\" = \"StudentInstructor\".\"Student\"");
            NpgsqlDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    studentInstructors.Add(new ClassStudentInstructor(result.GetInt32(0),
                        result.GetString(1),
                        result.GetString(2),
                        result.GetString(3),
                        result.GetString(4),
                        result.GetString(5),
                        result.GetString(6),
                        result.GetString(7),
                        result.GetInt32(8),
                        result.GetString(9),
                        result.GetInt32(10),
                        result.GetInt32(11)
                        ));
                }

            }
            result.Close();
        }
        */
    }
}
