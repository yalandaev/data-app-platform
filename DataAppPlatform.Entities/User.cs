﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DataAppPlatform.Entities
{
    public class User: Entity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
