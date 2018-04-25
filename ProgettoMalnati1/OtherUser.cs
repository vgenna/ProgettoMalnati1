using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Net;

using System.Drawing;

namespace ProgettoMalnati1
{
    public class OtherUser
    {
        IPAddress _address;
        string _name;
        Image _image;

        public OtherUser(IPAddress addr, string n, Image image){
            _address = addr;
            _name = n;
            _image = image;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Image Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public IPAddress Address
        {
            get { return _address; }
            set { _address = value; }
        }
    }
}
