using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CshEzam2Quiz
{
    internal class User
    {
        private string _login;
        private string _password;
        private DateTime _birthDay;

        public User(string login, string password, DateTime birthDay)
        {
            _login = login;
            _password = password;
            _birthDay = birthDay;
        }

        public void ChangePassword(string newPass)
        {
            _password = newPass;
            Queue<string> newUsers = new Queue<string>();

            using (StreamReader sr = File.OpenText("RegisteredUsers.txt"))
            {
                string temp = sr.ReadToEnd();
                string[] users = temp.Split('\n');
                
                foreach (string user in users)
                {
                    string[] data = user.Split(':');
                    if (data[0] != this._login && data[0].Length > 0)
                    {
                        newUsers.Enqueue(user);
                    }    
                }

            }

            if (newUsers.Count > 0)
            {
                using (StreamWriter sw = File.CreateText("RegisteredUsers.txt"))
                {
                    sw.Write($"{_login}:{_password}:{_birthDay.ToShortDateString()}");
                    foreach (string user in newUsers)
                    {
                        if(user.Length > 0 && !user.StartsWith(" "))
                            sw.Write($"\n{user}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Список затерт будет.");
                Console.ReadKey();
            }
        }

        public void ChangeBirthDay(DateTime newBirthDAy)
        {
            _birthDay=newBirthDAy;
            Queue<string> newUsers = new Queue<string>();

            using (StreamReader sr = File.OpenText("RegisteredUsers.txt"))
            {
                string temp = sr.ReadToEnd();
                string[] users = temp.Split('\n');

                foreach (string user in users)
                {
                    string[] data = user.Split(':');
                    if (data[0] != this._login && data[0].Length > 0)
                    {
                        newUsers.Enqueue(user);
                    }
                }

            }

            if (newUsers.Count > 0)
            {
                using (StreamWriter sw = File.CreateText("RegisteredUsers.txt"))
                {
                    sw.Write($"{_login}:{_password}:{_birthDay.ToShortDateString()}");
                    foreach (string user in newUsers)
                    {
                        if (user.Length > 0)
                            sw.WriteLine($"\n{user}");
                    }
                    
                }
            }
            else
            {
                Console.WriteLine("Список затерт будет.");
                Console.ReadKey();
            }

        }

        public override string ToString()
        {
            return $"{_login}, {_password}, {_birthDay.ToShortDateString()}";
        }

        public string ShowName()
        {
            return _login;
        }
    }
}
