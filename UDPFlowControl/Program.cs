using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPFlowControl
{
    class Program
    {
        static void Main(string[] args)
        {
            UdpClient mclient = new UdpClient(new IPEndPoint(IPAddress.Any, 3801));
            mclient.Client.ReceiveBufferSize = 1024 * 1024 * 5;
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            Dictionary<IPEndPoint, byte> sequences = new Dictionary<IPEndPoint, byte>(); 
            while (true)
            {
                byte[] me = mclient.Receive(ref ep);
                if(!sequences.ContainsKey(ep))
                {
                    sequences.Add(ep, me[0]);
                }
                if(me[0] != sequences[ep])
                {
                    mclient.Send(new byte[] { sequences[ep] },1,ep);
                }
                sequences[ep] = (byte)(me[0]+1);
            }
        }
    }
}
