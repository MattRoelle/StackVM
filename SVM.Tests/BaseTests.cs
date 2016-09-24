using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SVM.Tests
{
    [TestClass]
    public class BaseTests
    {
        #region Helper Methods

        protected StackVirtualMachine LoadVM(object[] instructions, StackVirtualMachine vm = null)
        {
            vm = vm ?? new StackVirtualMachine();
            vm.Load(instructions.Select(o => Convert.ToUInt32(o)).ToArray());
            return vm;
        }

        #endregion
    }
}
