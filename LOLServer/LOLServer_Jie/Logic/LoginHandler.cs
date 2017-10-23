using NetFrame.Auto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetFrame;
using GameProtocol.dto;

namespace LOLServer_Jie.Logic {
    public class LoginHandler:HandlerInterface {
        public void ClientClose(UserToken token, string error) {
        }

        public void MessageReceive(UserToken token, SocketModel obj) {
            switch (obj.command ) {
                case  LoginProtocol.LOGIN_CREQ:
                    login(token, obj.GetMessage<AccountInfoDTO>());
                    break;
                case LoginProtocol.REG_CREQ:
                    break;
            }

        }
        private void login(UserToken token,AccountInfoDTO value) {
            AccountInfoDTO info = value;
        }
        private void reg(UserToken token, AccountInfoDTO value) {

        }


        public void ClientConnect(UserToken token) {

        }
    }
}
