using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Net.UI.Utils
{
    public interface IConfigFile
    {
        Stream GetStream();
    }
}
