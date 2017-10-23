using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace NetFrame {
    /// <summary>
    /// 用户连接信息对象
    /// </summary>
    public class UserToken {
        //用户连接
        public Socket conn;
        //用户异步接收网络数据对象
        public SocketAsyncEventArgs receiveSAEA;
        //用户异步发送网络数据对象
        public SocketAsyncEventArgs sendSAEA;

        //四种编码解码委托
        public LengthEncode LE;
        public LengthDecode LD;
        public Encode encode;
        public Decode decode;

        //防止异步乱码
        private bool isReading = false;
        private bool isWriting = false;

        public delegate void SendProcess(SocketAsyncEventArgs e);
        public SendProcess sendProcess;
        public delegate void CloseProcess(UserToken token, string error);
        public CloseProcess closeProcess;
        public AbsHandlerCenter center;

        //数据缓存，粘包时使用
        List<byte> cache = new List<byte>();
        Queue<byte[]> writeQueue = new Queue<byte[]>();

        public UserToken() {
            receiveSAEA = new SocketAsyncEventArgs();
            sendSAEA = new SocketAsyncEventArgs();
            receiveSAEA.UserToken = this;
            sendSAEA.UserToken = this;
            //设置接收对象的缓冲区大小
            receiveSAEA.SetBuffer(new byte[1024], 0, 1024);
        }

        //网络消息到达
        public void Receive(byte[] buffer) {
            cache.AddRange(buffer);
            if (!isReading) {
                isReading = true;
                OnData();
            }
        }

        //缓存中有数据时的处理
        void OnData() {
            //解码消息存储对象
            byte[] buff=null;
            //当粘包解码器存在时进行粘包处理
            if (LD != null) {
                buff = LD(ref cache);
                if (buff == null) { isReading = false; return; }
            } else {
               //buff=cache.ToArray();
                //缓存区中没有数据 直接跳出数据处理 等待下次消息到达
                if (cache.Count == 0) { isReading = false; return; }

                buff = cache.ToArray();
                cache.Clear();
            }
            if (decode == null) { throw new Exception("Message Decode process is null."); }
            //进行消息反序列化
            object message = decode(buff);
            //TODO 通知应用层 有消息到达
            center.MessageReceive(this, message);
            //尾递归，防止在消息处理过程中 有其他消息到达而未经过处理
            OnData();
        }

        //发送消息
        public void Write(byte[] buff) {
            if (conn == null) { 
            //此连接断开
                closeProcess(this, "调用已断开的连接");
                return; 
            }
            writeQueue.Enqueue(buff);
            if (!isWriting) {
                isWriting = true;
                OnWrite();
            }
        }

        public void OnWrite() {
            //判断发送消息队列是否有消息
            if (writeQueue.Count == 0) { isWriting = false; return; }
            //取出第一条待发的消息
            byte[] buff = writeQueue.Dequeue();
            //设置消息发送异步对象的发送数据缓冲区数据
            sendSAEA.SetBuffer(buff, 0, buff.Length);
            //开启异步发送
            bool result = conn.SendAsync(sendSAEA);
            //是否挂起
            if (!result) {
                sendProcess(sendSAEA);
            }
        }

        //消息发送成功的回调
        public void Writed() {
            //与OnData同理的尾递归
            OnWrite();
        }

        public void Close() {
            try {
                isReading = false;
                isWriting = false;
                cache.Clear();
                writeQueue.Clear();
                conn.Shutdown(SocketShutdown.Both);
                conn.Close();
                conn = null;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message.ToString());
            }
        }

    }
}
