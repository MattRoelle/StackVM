using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVM.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var cli = new SVMCLI(message =>
            {
                Console.WriteLine(message);
            });
            cli.Execute(args);
        }
    }
}
