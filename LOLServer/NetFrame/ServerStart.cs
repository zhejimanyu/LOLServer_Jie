using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace NetFrame {
    public class ServerStart {
        Socket server;//服务器Socket监听对象
        int maxClient;//最大客户端连接数
        Semaphore acceptClient;
        UserTokenPool pool;
        
        public LengthEncode LE;
        public LengthDecode LD;
        public Encode encode;
        public Decode decode;
        /// <summary>
        /// 消息处理中心，必须外部传入
        /// </summary>
        public AbsHandlerCenter center;

        public ServerStart(int max) {
            //实例化监听对象
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //设定服务器最大连接人数
            maxClient = max;
        }

        public void Start(int port) {
            //创建连接池
            pool = new UserTokenPool(maxClient);
            //连接信号量
            acceptClient = new Semaphore(maxClient, maxClient);
            for (int i = 0; i < maxClient; i++) {
                UserToken token = new UserToken();

                token.receiveSAEA.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                token.sendSAEA.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                token.LD = LD;
                token.LE = LE;
                token.encode = encode;
                token.decode = decode;
                token.sendProcess = ProcessSend;
                token.closeProcess = ClientClose;
                token.center = center;//在ServerStart和Start之间赋值

        

                //初始化token信息
                pool.Push(token);
            }

            try {
                //监听服务器当前网卡的所有可用IP和port端口
                server.Bind(new IPEndPoint(IPAddress.Any, port));
                //置于监听状态
                server.Listen(10);
                StartAccept(null);
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 开启客户端连接监听
        /// </summary>
        /// <param name="e"></param>
        public void StartAccept(SocketAsyncEventArgs e) {
            //如果当前传入为空 说明调用新的客户端连接监听事件 否则的话 移除当前客户端连接
            if (e == null) {
                e = new SocketAsyncEventArgs();
                e.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completed);
            } else {
                e.AcceptSocket = null;
            }
            //信号量-1
            acceptClient.WaitOne();
            bool result = server.AcceptAsync(e);
            //判断异步事件是否挂起 没挂起说明立即执行完成 直接处理事件 否则会在处理完成后触发
            if (!result) {//没有挂起
                ProcessAccept(e);
            }

        }

        public void ProcessAccept(SocketAsyncEventArgs e) {
            //从连接对象池取出连接对象，供新用户使用
            UserToken token = pool.Pop();
            token.conn = e.AcceptSocket;
            //TODO 通知应用层 有客户端连接
            center.ClientConnect(token);
            //开启消息到达监听
            StartReceive(token);
            //释放当前异步对象
            StartAccept(e);
        }

        //客户端连入事件完成后调用
        public void Accept_Completed(object sender,SocketAsyncEventArgs e){
            ProcessAccept(e);
        }

        public void StartReceive(UserToken token) {
            try {
                bool result = token.conn.ReceiveAsync(token.receiveSAEA);
                if (!result) {
                    //lock (token) {
                        ProcessReceive(token.receiveSAEA);
                    //}
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        public void IO_Completed(object sender, SocketAsyncEventArgs e) {
            if (e.LastOperation == SocketAsyncOperation.Receive) {
                ProcessReceive(e);
            } else {
                ProcessSend(e);
            }
        }

        public void ProcessReceive(SocketAsyncEventArgs e) {
            UserToken token = e.UserToken as UserToken;
            //判断网络消息接收是否成功
            if (token.receiveSAEA.BytesTransferred > 0 && token.receiveSAEA.SocketError == SocketError.Success) {
                //处理接收到的消息
                byte[] message = new byte[token.receiveSAEA.BytesTransferred];
                Buffer.BlockCopy(token.receiveSAEA.Buffer, 0, message, 0, token.receiveSAEA.BytesTransferred);

                token.Receive(message);

                //无限递归
                StartReceive(token);
            } else {
                if (token.receiveSAEA.SocketError != SocketError.Success) {
                    ClientClose(token, e.SocketError.ToString());
                } else {
                    ClientClose(token, "客户端主动断开连接");
                }
            }

        }
        public void ProcessSend(SocketAsyncEventArgs e) {
            UserToken token = e.UserToken as UserToken;
            if (e.SocketError != SocketError.Success) {
                ClientClose(token, e.SocketError.ToString());
            } else {
                //消息发送成功 回调成功
                token.Writed();
            }
        }

        /// <summary>
        /// 客户端断开连接
        /// </summary>
        /// <param name="token">断开连接的用户</param>
        public void ClientClose(UserToken token, string error) {
            if (token.conn != null) {
                lock (token) {
                    //TODO 通知应用层 客户端断开连接
                    center.ClientClose(token, error);

                    token.Close();
                    //加回一个信号量 供其他用户使用
                    pool.Push(token);
                    acceptClient.Release();
                }
            }
        }


    }
}
