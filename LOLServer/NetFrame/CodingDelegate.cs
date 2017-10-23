using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFrame {
    
    public delegate byte[] LengthEncode(byte[] buff);
    public delegate byte[] LengthDecode(ref List<byte> cache);

    public delegate byte[] Encode(object obj);
    public delegate object Decode(byte[] buff);





}
