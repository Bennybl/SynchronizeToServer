using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SynchronizeToServer
{
    class Program
    {
        static void Main(string[] args)
        {

            AppManger appManger = new AppManger(args[1], args[2], args[3]);

        }
    }
}
