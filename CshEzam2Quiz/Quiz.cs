using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Xml.Linq;
using System.Security.Cryptography;

namespace CshEzam2Quiz
{
    internal class Quiz
    {

        User _user;
        public void ShowLogInMenu()
        {
            int userChoise = -1;
            bool isLogIn = false;
            try
            {
                do
                {
                    Console.Clear();
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
            }catch(Exception ex) {
                Console.WriteLine(ex);
            }

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
            tempPassword = MakeMeMD5(tempPassword);
            Console.WriteLine("Введите дату рождения в формате дд.мм.гггг: ");
            birthDayString = Console.ReadLine();
            string[] birthDayArr = birthDayString.Split('.');
            DateTime birthDay = new DateTime(Int32.Parse(birthDayArr[2]), Int32.Parse(birthDayArr[1]), Int32.Parse(birthDayArr[0]));

            if (File.Exists("RegisteredUsers.txt"))
            {
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
            }


            if (alreadyExists == false)
            {
                using (StreamWriter sw = File.AppendText("RegisteredUsers.txt"))
                {
                    sw.Write($"{tempLogin}:{tempPassword}:{birthDay.ToShortDateString()}\n");
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

            if (File.Exists("RegisteredUsers.txt"))
            {
                using (StreamReader sr = File.OpenText("RegisteredUsers.txt"))
                {
                    string temp = sr.ReadToEnd();
                    string[] users = temp.Split('\n');

                    foreach (string user in users)
                    {
                        string[] data = user.Split(':');
                        if (data[0] == tempLogin && IsGoodPass(tempPassword, data[1]))
                        {
                            logIn = true;
                            birthDay = data[2].Split('.');
                            birthDay2 = new DateTime(Int32.Parse(birthDay[2]), Int32.Parse(birthDay[1]), Int32.Parse(birthDay[0]));
                        }

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
            int userChoise = -1;
            do
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine($"Добрый день, {_user.ShowName()}");
                    Console.WriteLine("1. Старт новой викторины.");
                    Console.WriteLine("2. Посмотреть результаты прошлых викторин.");
                    Console.WriteLine("3. Посмотреть топ 20 по конкретной викторине.");
                    Console.WriteLine("4. Изменить пользовательские настойки.");
                    if (_user.ShowName() == "admin")
                    {
                        Console.WriteLine("5. Создание викторин и редактирование старых.");
                    }
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
                        case 5:
                            if (_user.ShowName() == "admin")
                            {
                                //редактирование викторин
                                ShowAdminMenu();
                            }
                            break;
                        default:
                            break;
                    }
                }catch(Exception ex) { Console.WriteLine(ex.ToString()); }
            } while (userChoise != 0);

        }

        private void StartNewQuiz()
        {
            int points = 0;
            string fileName;
            string userQuizAnswer = "";

            Console.WriteLine("Выберете викторину:");

            foreach (string file in Directory.EnumerateFiles("Quiz", "*.txt"))
            {
                Console.WriteLine(file);
            }

            fileName = Console.ReadLine();

            //если выбраны рандомные вопросы, читаем все файлы и создаем новый со случайными вопросами
            if(fileName == "Random")
            {
                string temp = "";
                foreach (string file in Directory.EnumerateFiles("Quiz", "*.txt"))
                {
                    using (StreamReader sr = File.OpenText(file))
                    {
                        temp += sr.ReadToEnd();
                    }
                }

                // массив со случайно выбранными вопросами
                List<string> questionsForNEwFile = new List<string>();
                //массив со всеми вопросами, взятыми из файлов
                string[] allQuestions = temp.Split('\n');
                //рандомная переменная
                var rand = new Random();
                //массив для проверки, что вопрос еще не включен 
                int[] check = { };
                while(questionsForNEwFile.Count < 10)
                {
                    int i;
                    i = rand.Next(0, allQuestions.Length-1);
                    if(!check.Contains(i))
                    {
                        check.Append(i);
                        questionsForNEwFile.Add(allQuestions[i]);
                    }
                }

                //создаем файл с рандомными вопросами

                using (StreamWriter sw = File.CreateText(@"Quiz\QuizRandom.txt"))
                {
                    foreach(string str in questionsForNEwFile)
                    {
                        sw.Write($"{str}:\n") ;
                    }
                }
            }

            if (fileName == "Random")
                fileName = @"Quiz\QuizRandom.txt";

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
                    sw.WriteLine($"{_user.ShowName()}:{points}:{fileName}:\n");
                }

            }
        }
        private void ShowMyResults()
        {
            if (File.Exists("Results.txt"))
            {
                using (StreamReader sr = File.OpenText("Results.txt")) // формат файла имя_пользователя:баллы:название_викторины
                {
                    string temp = sr.ReadToEnd();
                    string[] users = temp.Split('\n');
                    foreach (string user in users)
                    {
                        string[] data = user.Split(':');

                        try
                        {
                            if (data[0] == _user.ShowName())
                            {
                                Console.WriteLine($"{_user.ShowName()}, Количество баллов: {data[1]}, по викторине {data[2]}");
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
            else
            {
                Console.WriteLine("Файл с результатами не существует.");
            }
        }

        private void ShowTop20()
        {
            int userChoise = 0;
            //словари для хранения результатов по каждой викторине
            Dictionary<string, int> ResultsGeography = new Dictionary<string, int>();
            Dictionary<string, int> ResultsHistory = new Dictionary<string, int>();
            Dictionary<string, int> ResultsScience = new Dictionary<string, int>();
            Dictionary<string, int> ResultsRandom = new Dictionary<string, int>();

            if (File.Exists("Results.txt"))
            {
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
                            else if (data[2] == @"Quiz/QuizRandom.txt")
                                ResultsRandom.Add(data[0], Int32.Parse(data[1]));
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    //сортируем словари
                    var sortedDictHistory = from entry in ResultsHistory orderby entry.Value descending select entry;
                    var sortedDictGeography = from entry in ResultsGeography orderby entry.Value descending select entry;
                    var sortedDictScience = from entry in ResultsScience orderby entry.Value descending select entry;
                    var sortedDictRandom = from entry in ResultsRandom orderby entry.Value descending select entry;

                    Console.Clear();
                    Console.WriteLine("По какой викторине вы хотите увидеть таблицу лидеров?");
                    Console.WriteLine("1. История");
                    Console.WriteLine("2. География");
                    Console.WriteLine("3. Наука");
                    Console.WriteLine("4. Рандом");
                    userChoise = Int32.Parse(Console.ReadLine());

                    switch (userChoise)
                    {
                        case 1:
                            if (sortedDictHistory != null)
                            {
                                foreach (KeyValuePair<string, int> entry in sortedDictHistory)
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
                        case 4:
                            if (sortedDictRandom != null)
                            {
                                foreach (KeyValuePair<string, int> entry in sortedDictRandom)
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
            else
            {
                Console.WriteLine("Файл с результатами не существует.");
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

        private void MakeNewQuiz()
        {
            string name = ""; //название викторины на английском
            
            List<string> questions = new List<string>();
            Console.WriteLine("Напишите название викторины на английском:");
            name = Console.ReadLine();

            for (int i = 0; i < 10; i++)
            {
                string q = ""; // вопрос с ответами
                string tmp = "";
                Console.WriteLine($"Введите {i+1} вопрос:");
                q = Console.ReadLine();
                q += ":";
                Console.WriteLine("Введите первый предложенный ответ:");
                tmp = Console.ReadLine();
                q += tmp + ":";
                Console.WriteLine("Введите второй предложенный ответ:");
                tmp = Console.ReadLine();
                q += tmp + ":";
                Console.WriteLine("Введите третий предложенный ответ:");
                tmp = Console.ReadLine();
                q += tmp + ":";
                Console.WriteLine("Введите четвертый предложенный ответ:");
                tmp = Console.ReadLine();
                q += tmp + ":";
                Console.WriteLine("Введите правильный ответ:");
                tmp = Console.ReadLine();
                q += tmp+":";
                questions.Add(q);
            }

            using (StreamWriter sw = File.AppendText($"Quiz/Quiz{name}.txt"))
            {
                foreach(string s in questions)
                {
                    sw.Write($"{s}\n");
                }
            }


        }

        private void AddQuestionsInQuiz()
        {

            string choosenFile = "";
            int numberOfQ = 0;
            Console.WriteLine("Выберете файл для добавления вопроса:");
            foreach (string file in Directory.EnumerateFiles("Quiz", "*.txt"))
            {
                Console.WriteLine(file);
            }
            choosenFile = Console.ReadLine();

            Console.WriteLine("Сколько вопросов вы хотите добавить?:");

            numberOfQ = Int32.Parse(Console.ReadLine());

            List<string> questions = new List<string>();

            for (int i = 0; i < numberOfQ; i++)
            {
                string q = ""; // вопрос с ответами
                string tmp = "";
                Console.WriteLine($"Введите {i+1} вопрос:");
                q = Console.ReadLine();
                q += ":";
                Console.WriteLine("Введите первый предложенный ответ:");
                tmp = Console.ReadLine();
                q += tmp + ":";
                Console.WriteLine("Введите второй предложенный ответ:");
                tmp = Console.ReadLine();
                q += tmp + ":";
                Console.WriteLine("Введите третий предложенный ответ:");
                tmp = Console.ReadLine();
                q += tmp + ":";
                Console.WriteLine("Введите четвертый предложенный ответ:");
                tmp = Console.ReadLine();
                q += tmp + ":";
                Console.WriteLine("Введите правильный ответ:");
                tmp = Console.ReadLine();
                q += tmp+":";
                questions.Add(q);
            }

            using (StreamWriter sw = File.AppendText(@choosenFile))
            {
                foreach (string s in questions)
                {
                    sw.Write($"{s}\n");
                }
            }
        }

        private void ShowAdminMenu()
        {
            int choise;
            Console.Clear();
            Console.WriteLine("1. Создать новую викторину.");
            Console.WriteLine("2. Добавить вопросы в существующую викторину.");
            Console.WriteLine("0. Выход.");

            try
            {
                choise = Int32.Parse(Console.ReadLine());

                switch (choise)
                {
                    case 1:
                        MakeNewQuiz();
                        break;
                    case 2:
                        AddQuestionsInQuiz();
                        break;
                    case 0:
                        ShowUserMenu();
                        break;
                    default:
                        ShowUserMenu();
                        break;
                }
            }catch(Exception ex) { }
        }

        private string MakeMeMD5(string password)
        {
            byte[] byteHashedPassword;
            byte[] bytePassword = Encoding.UTF8.GetBytes(password);

            using (MD5 md5 = MD5.Create())
            {
                byteHashedPassword = md5.ComputeHash(bytePassword);
            }
            string hashPassword = BitConverter.ToString(byteHashedPassword).Replace("-", "");

            return hashPassword;
        }

        private bool IsGoodPass(string password, string md5)
        {
            if (MakeMeMD5(password) == md5)
                return true;
            else
                return false;
        }
    }
}
