using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFrame.Auto {
    public class SocketModel {
        //一级协议 用于区分所属模块
        public byte type { get; set; }
        //二级协议 用于区分所属模块下的子模块
        public int area { get; set; }
        //三级协议 用于区分当前处理的逻辑功能
        public int command { get; set; }
        //消息体 当前需要处理的主体数据
        public object message { get; set; }

        public SocketModel() {
        }

        public SocketModel(byte t, int a, int c, object m) {
            this.type = type;
            this.area = a;
            this.command = c;
            this.message = m;
        }

        public T GetMessage<T>() {
            return (T)message;
        }
        

    }
}
