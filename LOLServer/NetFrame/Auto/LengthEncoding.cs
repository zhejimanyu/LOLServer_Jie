using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NetFrame.Auto {
    public class LengthEncoding {

        //粘包长度编码 将长度和数据块拼在一起
        public static byte[] Encode(byte[] buff){
            MemoryStream ms=new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(buff.Length);
            bw.Write(buff);
            byte[] result = new byte[ms.Length];
            Buffer.BlockCopy(ms.GetBuffer(), 0, result, 0,(int)ms.Length);
            bw.Close();
            ms.Close();
            return result;
        }

        //粘包长度解码 解码后返回数据块
        public static byte[] Decode(ref List<byte> cache ) {
            if (cache.Count < 4) return null;

            MemoryStream ms = new MemoryStream(cache.ToArray());
            BinaryReader br = new BinaryReader(ms);
            int length = br.ReadInt32();
            //如果消息体的长度大于缓存中数据长度 说明数据没有读完 等待下次数据接收后再处理
            if (length > ms.Length - ms.Position) return null;
            //读取正确长度的数据
            byte[] result = br.ReadBytes(length);
            //清空缓存
            cache.Clear();
            //将读取后剩余数据写入缓存
            cache.AddRange(br.ReadBytes((int)(ms.Length - ms.Position)));
            br.Close();
            ms.Close();
            return result;
        }


    }
}
