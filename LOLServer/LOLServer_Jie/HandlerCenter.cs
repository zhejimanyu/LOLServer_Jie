using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetFrame;
using LOLServer_Jie.Logic;
using NetFrame.Auto;
using GameProtocol;

namespace LOLServer_Jie {
   public class HandlerCenter :AbsHandlerCenter{

       HandlerInterface login;
       public HandlerCenter() {
           login = new LoginHandler();
       }


       public override void ClientClose(UserToken token, string error) {
           Console.WriteLine("有客户端断开连接了");
       }

       public override void ClientConnect(UserToken token) {
           Console.WriteLine("有客户端连接了--{0}\n{1}", token, token.conn);
       }

       public override void MessageReceive(UserToken token, object obj) {
          // Console.WriteLine("接收到了消息");
           SocketModel model = obj as SocketModel;
           switch(model.type){
               case Protocol.TYPE_LOGIN:
                   login.MessageReceive(token, model);
                   break;           
           }      

       }
   }
}
