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
            Stack<string> newUsers = new Stack<string>();

            using (StreamReader sr = File.OpenText("RegisteredUsers.txt"))
            {
                string temp = sr.ReadToEnd();
                string[] users = temp.Split('\n');
                
                foreach (string user in users)
                {
                    string[] data = user.Split(':');
                    if (data[0] != this._login)
                    {
                        newUsers.Append($"{data[0]}:{data[1]}:{data[2]}");
                    }
                        
                }
            }

            using (StreamWriter sw = File.CreateText("RegisteredUsers.txt"))
            {
                sw.WriteLine($"{_login}:{_password}:{_birthDay.ToShortDateString()}");
                foreach (string user in newUsers)
                {
                    sw.WriteLine(user);
                }
            }
        }

        public void ChangeBirthDay(DateTime newBirthDAy)
        {
            _birthDay=newBirthDAy;
        }

        public override string ToString()
        {
            return $"{_login}, {_password}, {_birthDay.ToString()}";
        }

        public string ShowName()
        {
            return _login;
        }
    }
}
