using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    public struct ProtocolPackage
    {
        public string Header;
        public int Cmd;
        public int Lenght;
        public string Data;

        public ProtocolPackage(string header, int cmd, int lenght, string data)
        {
            this.Header = header;
            this.Cmd = cmd;
            this.Lenght = lenght;
            this.Data = data;
        }
    }
}
