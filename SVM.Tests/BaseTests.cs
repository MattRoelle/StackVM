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

        protected void AssertDeepEqual<T>(T[] o1, T[] o2)
        {
            Assert.AreEqual(o1.Length, o2.Length);
            for (var i = 0; i < o1.Length; i++)
            {
                Assert.AreEqual(o1[i], o2[i]);
            }
        }
        #endregion
    }
}
