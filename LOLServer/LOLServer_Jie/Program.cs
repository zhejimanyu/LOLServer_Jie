using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using NetFrame;
using NetFrame.Auto;

namespace LOLServer_Jie {
    class Program {
        static void Main(string[] args) {
            //服务器初始化
            ServerStart ss = new ServerStart(8000);
            ss.encode = MessageEncoding.Encode;
            ss.decode = MessageEncoding.Decode;
            ss.LE = LengthEncoding.Encode;
            ss.LD = LengthEncoding.Decode;
            ss.center = new HandlerCenter();
            ss.Start(6650);
            Console.WriteLine("服务器启动成功");
            while (true) {
            }
            

            ////同步接受和发送操作
            //Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //server.Bind(new IPEndPoint(IPAddress.Any, 123));
            ////置于监听状态
            //server.Listen(10);
            //Socket client = server.Accept();
            //byte[] buff = new byte[1024];
            //client.Receive(buff);
            //client.Send(buff);
 

        }
    }
}
