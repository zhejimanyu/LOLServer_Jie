using NetFrame;
using NetFrame.Auto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOLServer_Jie {
    public interface HandlerInterface {

        void ClientClose(UserToken token, string error);

        void ClientConnect(UserToken token);

        void MessageReceive(UserToken token, SocketModel obj);

    }
}