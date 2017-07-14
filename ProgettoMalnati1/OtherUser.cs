using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Net;

namespace ProgettoMalnati1
{
    public class OtherUser
    {
        IPAddress _address;
        string _name;

        public OtherUser(IPAddress addr, string n){
            _address = addr;
            _name = n;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public IPAddress Address
        {
            get { return _address; }
            set { _address = value; }
        }
    }
}
