using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFrame {
    //消息中心
   public abstract class AbsHandlerCenter {
       /// <summary>
       /// 客户端连入
       /// </summary>
       /// <param name="token">连入的客户端对象</param>
       public abstract void ClientConnect(UserToken token);
       /// <summary>
       /// 收到客户端的消息
       /// </summary>
       /// <param name="token">发消息的客户端对象</param>
       /// <param name="obj">消息内容</param>
       public abstract void MessageReceive(UserToken token, object obj);
       /// <summary>
       /// 客户端断开连接
       /// </summary>
       /// <param name="token">断开的客户端对象</param>
       /// <param name="error">断开的错误信息</param>
       public abstract void ClientClose(UserToken token, string error);

    }
}
