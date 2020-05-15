using System;
using System.Collections.Generic;
using System.Text;

namespace AlszikBot
{
    public class Player
    {
        public string Username { get; private set; }
        private string nickname;
        public string Nickname { 
            get
            {
                if (nickname == null)
                    return Username;
                return nickname;
            }
            set
            {
                nickname = value;
            }
        }
        public List<string> GameRoles { get; set; }

        public Player(string username, string nickname)
        {
            Username = username;
            Nickname = nickname;
            GameRoles = new List<string>();
        }

        public Player(string username, string nickname, string gameRole) : this(username, nickname)
        {
            GameRoles.Add(gameRole);
        }
        public void AddRole(string gameRole)
        {
            if(!GameRoles.Contains(gameRole))
                GameRoles.Add(gameRole);
        }
        public override string ToString()
        {
            return Nickname + " szerepe(i): " + String.Join(", ", GameRoles);
        }
    }
}
