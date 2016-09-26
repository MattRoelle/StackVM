using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SVM.Assembler;

namespace SVM.CLI
{
    public class SVMCLI
    {
        private Action<string> _errorCallback;

        public SVMCLI(Action<string> errorCallback = null)
        {
            _errorCallback = errorCallback;
        }

        public void Execute(string[] args)
        {
            switch(args[0].ToLowerInvariant())
            {
                case "assemble":
                    Assemble(args[1], args[2]);
                    break;
                default:
                    // If no command is passed, assume execution
                    Run(args[0]);
                    break;
            }
        }

        private void Run(string input)
        {
            byte[] buffer;
            using (var fs = new FileStream(input, FileMode.Open, FileAccess.Read))
            {
                buffer = new byte[fs.Length];
                fs.Seek(0, SeekOrigin.Begin);
                fs.Read(buffer, 0, input.Length);
            }

            var vm = new StackVirtualMachine(_errorCallback);
            vm.Load(buffer);
            vm.Start();
        }

        private void Assemble(string input, string dest)
        {
            uint[] output;

            using (var fs = new FileStream(input, FileMode.Open, FileAccess.Read))
            {
                var assembler = new SVMAssembler();

                byte[] buffer = new byte[input.Length];
                fs.Seek(0, SeekOrigin.Begin);
                fs.Read(buffer, 0, input.Length);

                output = assembler.Assemble(Encoding.ASCII.GetString(buffer));
            }

            using (var fs = new FileStream(dest, FileMode.CreateNew, FileAccess.Write))
            {
                fs.Write(SVMAssembler.ToByteArray(output), 0, output.Length * 4);
            }
        }
    }
}
