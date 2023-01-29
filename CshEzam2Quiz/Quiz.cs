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

        private void Register()
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

        private bool LogIn()
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

        private void ShowUserMenu()
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

        private void StartNewQuiz()
        {

            int userChoise;
            int points = 0;
            string fileName;
            string userQuizAnswer = "";

            Console.WriteLine("Выберете викторину:");
            Console.WriteLine("1. География.");
            Console.WriteLine("2. Наука.");
            Console.WriteLine("3. История.");

            userChoise = Int32.Parse(Console.ReadLine());
            
            switch(userChoise)
            {
                case 1:
                    fileName = @"Quiz/QuizGeography.txt";
                    break;
                case 2:
                    fileName = @"Quiz/QuizScience.txt";
                    break;
                case 3:
                    fileName = @"Quiz/QuizHistory.txt";
                    break;
                default:
                    fileName = @"Quiz/QuizGeography.txt";
                    break;
            }

            //проводим викторину, читаем вопросы из файла и сравниваем ответ пользователя с верным ответом
            using (StreamReader sr = File.OpenText(fileName))
            {
                string temp = sr.ReadToEnd();
                string[] questions = temp.Split('\n');

                foreach (string question in questions)
                {
                    string[] data = { };
                    data = question.Split(':');
                    try
                    {
                        Console.WriteLine(data[0]);
                        Console.WriteLine(data[1]);
                        Console.WriteLine(data[2]);
                        Console.WriteLine(data[3]);
                        Console.WriteLine(data[4]);
                        userQuizAnswer = Console.ReadLine();
                        data[5] = data[5].Substring(0, data[5].Length - 1); //была проблема с символом /n в конце, по этому equals работал некорректно
                        if (String.Equals(userQuizAnswer.ToUpper(), data[5].ToUpper()))
                        {
                            Console.Clear();
                            Console.WriteLine("Верно!");
                            points++;
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine($"Нет, правильный ответ:{data[5]}");
                        }
                    }
                    catch (Exception ex) {
                        Console.WriteLine("Конец");
                    }
                }
                Console.WriteLine($"Количество набранных баллов: {points}");
                Console.ReadKey();

                //запись результатов в файл
                using(StreamWriter sw = File.AppendText("Results.txt"))
                {
                    sw.WriteLine($"{_user.ShowName()}:{points}:{fileName}");
                }

            }
        }
        private void ShowMyResults()
        {
            string quizName = "";
            using(StreamReader sr = File.OpenText("Results.txt")) // формат файла имя_пользователя:баллы:название_викторины
            {
                string temp = sr.ReadToEnd();
                string[] users = temp.Split('\n');
                foreach(string user in users)
                {
                    string[] data = user.Split(':');
                   
                    try // not working
                    {
                        data[2] = data[2].Substring(0, data[2].Length - 1);

                        if (data[2] == @"Quiz/QuizGeography.txt")
                            quizName = "География";
                        else if (data[2] == @"Quiz/QuizScience.txt")
                            quizName = "Наука";
                        else if (data[2] == @"Quiz/QuizHistory.txt")
                            quizName = "История";

                        if (data[0] == _user.ShowName())
                        {
                            Console.WriteLine($"{_user.ShowName()}, Количество баллов: {data[1]}, по викторине {quizName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ReadKey();
                    }

                    
                }
                Console.ReadKey();
            }
        }

        private void ShowTop20()
        {
            int userChoise = 0;
            //словари для хранения результатов по каждой викторине
            Dictionary<string, int> ResultsGeography = new Dictionary<string, int>();
            Dictionary<string, int> ResultsHistory = new Dictionary<string, int>();
            Dictionary<string, int> ResultsScience = new Dictionary<string, int>();

            using (StreamReader sr = File.OpenText("Results.txt")) // формат файла имя_пользователя:баллы:название_викторины
            {
                string temp = sr.ReadToEnd();
                string[] users = temp.Split('\n');

                foreach (string user in users)
                {
                    string[] data = user.Split(':');
                    //заполняем словари
                    try
                    {
                        data[2] = data[2].Substring(0, data[2].Length - 1);

                        if (data[2] == @"Quiz/QuizGeography.txt")
                            ResultsGeography.Add(data[0], Int32.Parse(data[1]));
                        else if (data[2] == @"Quiz/QuizScience.txt")
                            ResultsScience.Add(data[0], Int32.Parse(data[1]));
                        else if (data[2] == @"Quiz/QuizHistory.txt")
                            ResultsHistory.Add(data[0], Int32.Parse(data[1]));
                    }
                    catch (Exception ex)
                    {
                    }
                }
                //сортируем словари
                var sortedDictHistory = from entry in ResultsHistory orderby entry.Value ascending select entry;
                var sortedDictGeography = from entry in ResultsGeography orderby entry.Value ascending select entry;
                var sortedDictScience = from entry in ResultsScience orderby entry.Value ascending select entry;

                Console.Clear();
                Console.WriteLine("По какой викторине вы хотите увидеть таблицу лидеров?");
                Console.WriteLine("1. История");
                Console.WriteLine("2. География");
                Console.WriteLine("3. Наука");
                userChoise = Int32.Parse(Console.ReadLine());

                switch(userChoise)
                {
                    case 1:
                        if(sortedDictHistory != null)
                        {
                            foreach(KeyValuePair<string, int> entry in sortedDictHistory)
                            {
                                Console.WriteLine($"{entry.Key}\t{entry.Value} баллов.");
                            }
                            Console.ReadKey();
                        }else
                        {
                            Console.WriteLine("Список пуст.");
                            Console.ReadKey();
                        }
                        break;
                    case 2:
                        if (sortedDictGeography != null)
                        {
                            foreach (KeyValuePair<string, int> entry in sortedDictGeography)
                            {
                                Console.WriteLine($"{entry.Key}\t{entry.Value} баллов.");
                            }
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine("Список пуст.");
                            Console.ReadKey();
                        }
                        break;
                    case 3:
                        if (sortedDictScience != null)
                        {
                            foreach (KeyValuePair<string, int> entry in sortedDictScience)
                            {
                                Console.WriteLine($"{entry.Key}\t{entry.Value} баллов.");
                            }
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine("Список пуст.");
                            Console.ReadKey();
                        }
                        break;
                    default:
                        break;
                }

            }
        }


        private void ChangeUserSettings()
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
