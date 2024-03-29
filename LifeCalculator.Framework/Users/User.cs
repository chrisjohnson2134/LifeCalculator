﻿using System;

namespace LifeCalculator.Framework.Users
{
    public class User
    {
        #region Properties

        public int Id { get; set; }
        public string Username { get; set; }

        public string PasswordHashed { get; set; }

        public string Email { get; set; }

        public string DateRegistered { get; set; }

        #endregion
    }
}
