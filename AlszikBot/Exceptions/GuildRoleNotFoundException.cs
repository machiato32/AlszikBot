using System;
using System.Collections.Generic;
using System.Text;

namespace AlszikBot.Exceptions
{
    class GuildRoleNotFoundException : Exception
    {
        public GuildRoleNotFoundException(string msg) : base(msg)
        {

        }
    }
}
