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

        User user;
        public void HelloMessage()
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
            
            //тут может быть старт игры
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
                    sw.WriteLine($"{tempLogin}:{tempPassword}:{birthDay.ToShortDateString()}");
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
                user = new User(tempLogin, tempPassword, birthDay2);
                Console.WriteLine($"{user.ToString()}");
                Console.ReadKey();
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
    }
}
