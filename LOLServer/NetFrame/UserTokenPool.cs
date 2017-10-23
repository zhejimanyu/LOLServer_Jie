using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFrame {
    public class UserTokenPool {
        private Stack<UserToken> pool = new Stack<UserToken>();

        public UserTokenPool(int max) {

        }

        public UserToken Pop() {
            return pool.Pop();
        }

        public void Push(UserToken token) {
            if (token != null)
                pool.Push(token);
        }
        public int Size {
            get { return pool.Count; }
        }

    }
}
