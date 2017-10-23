using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetFrame.Auto {
    public class SerializeUtil {

        public static byte[] encode(object value) {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            //将obj对象序列化成二进制数据 写入流
            bf.Serialize(ms, value);
            byte[] result = new byte[ms.Length];
            //将流数据 拷贝到结果数组
            Buffer.BlockCopy(ms.GetBuffer(), 0, result, 0, (int)ms.Length);
            ms.Close();
            return result;
        }

        public static object decode(byte[] buff) {
            MemoryStream ms = new MemoryStream(buff);
            BinaryFormatter bf = new BinaryFormatter();
            //将流数据反序列化为obj
            object result = bf.Deserialize(ms);
            ms.Close();
            return result;
        }

    }
}
