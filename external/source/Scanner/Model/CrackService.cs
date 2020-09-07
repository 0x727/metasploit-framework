using System;
using System.Text;
//using Tools;

namespace SNETCracker.Model
{
    public abstract class CrackService
    {
        public abstract Server crack(String ip, int port, String username, String password, int timeOut);
    }
}
