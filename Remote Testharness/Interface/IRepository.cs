using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public interface IRepository
    {
        void uploadFile(string filename, string ToSendPath, string SavePath);
        void download(string filename, string SavePath);
    }
}
