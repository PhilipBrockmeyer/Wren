using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Wren.Transport.DataObjects;
using Wren.Data;
using NHibernate.Linq;
using Wren.Data.Entities;

namespace Wren.Services.Core
{
    public class AccountService
    {
        ISessionManager _sessionManager;

        public AccountService(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public LogOnResultDto LogOn(String username, String password)
        {
            LogOnResultDto result = new LogOnResultDto();
            result.IsSuccessful = false;

            username = username.Trim().ToLower();
            password = password.Trim();

            using (var session = _sessionManager.GetSession())
            {
                if (!String.IsNullOrEmpty(username))
                {
                    var results = session.Linq<User>()
                        .Where(u => u.LoweredUsername == username);

                    var user = results.FirstOrDefault();

                    if (user != null)
                    {
                        if (user.Password == password)
                        {
                            result.IsSuccessful = true;
                            result.UserId = user.Id;
                        }
                    }
                }
            }

            if (!result.IsSuccessful)
                result.Errors = new String[] { "The username or password is incorrect." };

            return result;
        }

        public RegistrationValidationResultDto IsRegistrationValid(String username, String password, String email)
        {
            var result = new RegistrationValidationResultDto();
            result.UserId = Guid.NewGuid().ToString();
            var errors = new List<String>();

            username = username.Trim();
            password = password.Trim();
            email = email.Trim();

            User user = new User() { Username = username.Trim(), EmailAddress = email.Trim(), Password = password.Trim(), Id = Guid.NewGuid().ToString(), LoweredUsername = username.Trim().ToLower() };

            result.IsValid = IsUserValid(user, errors);
            result.Errors = errors.ToArray();
            return result;
        }

        public Boolean Register(User user)
        {
            user.Username = user.Username.Trim();
            user.LoweredUsername = user.Username.ToLower();
            user.Password = user.Password.Trim();
            user.EmailAddress = user.EmailAddress.Trim();

            if (!IsUserValid(user, new List<String>()))
                return false;

            try
            {
                using (var session = _sessionManager.GetSession())
                {
                    using (var tx = session.BeginTransaction())
                    {

                        session.SaveOrUpdate(user);
                        tx.Commit();
                    }
                }
            }
            catch
            {
                return true;
            }

            return true;
        }

        private Boolean IsUserValid(User user, List<String> errors)
        {
            if (user.Username.Length <= 4 || user.Username.Length > 20)
            {
                errors.Add("Username must be 5 to 20 characters long.");
            }

            if (user.Password.Length <= 4 || user.Password.Length > 20)
            {
                errors.Add("Password must be 5 to 20 characters long.");
            }

            if (!Regex.IsMatch(user.EmailAddress, @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?"))
            {
                errors.Add("Please enter a valid email address.");
            }

            using (var session = _sessionManager.GetSession())
            {
                if (!String.IsNullOrEmpty(user.Username))
                {
                    var results = session.Linq<User>()
                        .Where(u => u.LoweredUsername == user.LoweredUsername);

                    if (results.ToList().Count() > 0)
                        errors.Add("Username is already in use.");
                }
            }

            if (errors.Count > 0)
                return false;

            return true;
        }
    }
}