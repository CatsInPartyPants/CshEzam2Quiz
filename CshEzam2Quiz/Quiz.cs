using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CshEzam2Quiz
{
    internal class Quiz
    {

        User _user;
        public void ShowLogInMenu()
        {
            int userChoise = -1;
            bool isLogIn = false;
            do
            {
                Console.WriteLine("Добрый день, Вас приветствует игра \"Викторина\"!");
                Console.WriteLine("Если вы зарегистрированы, нажмите 1 для ввода логина и пароля.");
                Console.WriteLine("Если вы желаете зарегистрироваться, нажмите 2.");
                Console.WriteLine("Нажмите 0 для выхода.");

                userChoise = Int32.Parse(Console.ReadLine());

                switch (userChoise)
                {
                    case 1:
                        isLogIn = LogIn();
                        break;
                    case 2:
                        Register();
                        break;
                    default:
                        break;
                }
            } while (userChoise != 0 && isLogIn == false);
            
            if(isLogIn == true)
            {
                ShowUserMenu();
            }
            else
            {
                Console.WriteLine("До свидания.");
                Console.ReadKey();
            }
        }

        public void Register()
        {
            string tempLogin;
            string tempPassword;
            string birthDayString;
            bool alreadyExists = false;

            Console.WriteLine("Введите логин:");
            tempLogin = Console.ReadLine();
            Console.WriteLine("Введите пароль:");
            tempPassword = Console.ReadLine();
            Console.WriteLine("Введите дату рождения в формате дд.мм.гггг: ");
            birthDayString = Console.ReadLine();
            string[] birthDayArr = birthDayString.Split('.');
            DateTime birthDay = new DateTime(Int32.Parse(birthDayArr[2]), Int32.Parse(birthDayArr[1]), Int32.Parse(birthDayArr[0]));

            using (StreamReader sr = File.OpenText("RegisteredUsers.txt"))
            {
                string temp = sr.ReadToEnd();
                string[] users = temp.Split('\n');

                foreach (string user in users)
                {
                    string[] data = user.Split(':');
                    if (data[0] == tempLogin)
                        alreadyExists = true;
                }
            }


            if (alreadyExists == false)
            {
                using (StreamWriter sw = File.AppendText("RegisteredUsers.txt"))
                {
                    sw.Write($"\n{tempLogin}:{tempPassword}:{birthDay.ToShortDateString()}");
                }
                Console.WriteLine("Вы успешно зарегистрировались, пожалуйста пройдите процесс авторизации.");
                Console.ReadKey();
                Console.Clear();
            }
            else
            {
                Console.WriteLine("Указанный пользователь уже существует. Попробуйте ввести логин или пароль или выберете иное имя пользователя.");
                Console.ReadKey();
                Console.Clear();
            }
        }

        public bool LogIn()
        {
            bool logIn = false;
            string tempLogin;
            string tempPassword;
            string[] birthDay;
            DateTime birthDay2 = new DateTime();
            Console.WriteLine("Введите логин:");
            tempLogin = Console.ReadLine();
            Console.WriteLine("Введите пароль:");
            tempPassword = Console.ReadLine();

            using(StreamReader sr = File.OpenText("RegisteredUsers.txt"))
            {
                string temp = sr.ReadToEnd();
                string[] users = temp.Split('\n');

                foreach(string user in users)
                {
                    string[] data = user.Split(':');
                    if (data[0] == tempLogin && data[1] == tempPassword)
                    {
                        logIn = true;
                        birthDay = data[2].Split('.');
                        birthDay2 = new DateTime(Int32.Parse(birthDay[2]), Int32.Parse(birthDay[1]), Int32.Parse(birthDay[0]));
                    }
                    
                }
            }
           if(logIn)
            {
                Console.Clear();
                Console.WriteLine($"Добро пожаловать, {tempLogin}.");
                //создать пользователя
                _user = new User(tempLogin, tempPassword, birthDay2);
                return true;
            }
           else
            {
                Console.WriteLine("Данные не верны.");
                Console.ReadKey();
                Console.Clear();
                return false;
            }

        }

        public void ShowUserMenu()
        {
            int userChoise;
            do
            {
                Console.Clear();
                Console.WriteLine($"Добрый день, {_user.ShowName()}");
                Console.WriteLine("1. Старт новой викторины.");
                Console.WriteLine("2. Посмотреть результаты прошлых викторин.");
                Console.WriteLine("3. Посмотреть топ 20 по конкретной викторине.");
                Console.WriteLine("4. Изменить пользовательские настойки.");
                Console.WriteLine("0. Выход.");
                
                userChoise = Int32.Parse(Console.ReadLine());

                switch (userChoise)
                {
                    case 1:
                        StartNewQuiz();
                        break;
                    case 2:
                        ShowMyResults();
                        break;
                    case 3:
                        ShowTop20();
                        break;
                    case 4:
                        ChangeUserSettings();
                        break;
                    default:
                        break;
                }
            } while (userChoise != 0);
        }

        public void StartNewQuiz()
        {

        }
        public void ShowMyResults()
        {

        }

        public void ShowTop20()
        {

        }

        public void ChangeUserSettings()
        {
            int userSettingsChoise;
            
            Console.WriteLine("1. Изменить пароль.\n2. Изменить дату рождения");
            userSettingsChoise = Int32.Parse(Console.ReadLine());
            
            if(userSettingsChoise == 1)
            {
                string newPassword;
                Console.WriteLine("Введите новый пароль:");
                newPassword = Console.ReadLine();
                _user.ChangePassword(newPassword);
            }
            else if(userSettingsChoise == 2)
            {
                Console.WriteLine("Введите новую дату рождения в формате дд.мм.гггг");
                DateTime newBD = DateTime.Parse(Console.ReadLine());
                _user.ChangeBirthDay(newBD);
            }
        }
    }
}
