using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        public void ChangeBirthDay(DateTime newBirthDAy)
        {
            _birthDay=newBirthDAy;
        }

        public override string ToString()
        {
            return $"{_login}, {_password}, {_birthDay.ToString()}";
        }

        // находит логин в файле и меняет его пароль и день рождения
        public void SaveToFile(string login)
        {

        }
    }
}
