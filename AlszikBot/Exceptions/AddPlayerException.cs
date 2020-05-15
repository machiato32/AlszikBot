using System;
using System.Collections.Generic;
using System.Text;

namespace AlszikBot.Exceptions
{
    class AddPlayerException : Exception
    {
        public AddPlayerException(string message) : base(message)
        {

        }
    }
}
