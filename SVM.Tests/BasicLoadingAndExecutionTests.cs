using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SVM.Tests
{
    [TestClass]
    public class BasicLoadingAndExecutionTests : BaseTests
    {
        [TestMethod]
        [TestCategory("VM")]
        public void PushByValue()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.Push, OpModes.Value, 100u,
                OpCodes.Halt
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[1], 100u);
            Assert.AreEqual(vm.Flag, 0m);
        }

        [TestMethod]
        [TestCategory("VM")]
        public void AddWithoutCarry()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.Push, OpModes.Value, 100u,
                OpCodes.Push, OpModes.Value, 200u,
                OpCodes.Add,
                OpCodes.Halt
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[1], 300u);
            Assert.AreEqual(vm.Flag, 0m);
        }

        [TestMethod]
        [TestCategory("VM")]
        public void AddWithCarry()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.Push, OpModes.Value, 0xFFFFFFFFu,
                OpCodes.Push, OpModes.Value, 200u,
                OpCodes.Add,
                OpCodes.Halt
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[1], 199u);
            Assert.AreEqual(vm.Flag, (Flags.Carry));
        }

        [TestMethod]
        [TestCategory("VM")]
        public void AddToZero()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.Push, OpModes.Value, 1u,
                OpCodes.Push, OpModes.Value, 0xFFFFFFFFu,
                OpCodes.Add, 
                OpCodes.Halt
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[1], 0u);
            Assert.IsTrue((vm.Flag & (Flags.Zero | Flags.Carry)) != 0);
        }

        [TestMethod]
        [TestCategory("VM")]
        public void SubtractWithoutCarry()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.Push, OpModes.Value, 100u,
                OpCodes.Push, OpModes.Value, 200u,
                OpCodes.Subtract,
                OpCodes.Halt
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[1], 100u);
            Assert.AreEqual(vm.Flag, 0m);
        }

        [TestMethod]
        [TestCategory("VM")]
        public void SubtractWithCarry()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.Push, OpModes.Value, 0xFFFFFFFFu,
                OpCodes.Push, OpModes.Value, 200u,
                OpCodes.Subtract,
                OpCodes.Halt
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[1], 201u);
            Assert.AreEqual(vm.Flag, (Flags.Carry));
        }

        [TestMethod]
        [TestCategory("VM")]
        public void SubtractToZero()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.Push, OpModes.Value, 200u,
                OpCodes.Push, OpModes.Value, 200u,
                OpCodes.Subtract,
                OpCodes.Halt
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[1], 0u);
            Assert.IsTrue((vm.Flag & Flags.Zero) != 0);
        }

        [TestMethod]
        [TestCategory("VM")]
        public void StoreByValue()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.Store, OpModes.Value, 0x8000, OpModes.Value, 2000u,
                OpCodes.Halt
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[0x8000], 2000u);
            Assert.AreEqual(vm.Flag, 0m);
        }

        [TestMethod]
        [TestCategory("VM")]
        public void StoreByReference()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.Store, OpModes.Value, 0x8000, OpModes.Value, 0x3000,
                OpCodes.Store, OpModes.Reference, 0x8000, OpModes.Value, 2000u,
                OpCodes.Halt
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[0x3000], 2000u);
            Assert.AreEqual(vm.Flag, 0m);
        }

        [TestMethod]
        [TestCategory("VM")]
        public void PushByReference()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.Store, OpModes.Value, 0x8000, OpModes.Value, 2000u,
                OpCodes.Push, OpModes.Reference, 0x8000u,
                OpCodes.Halt
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[1], 2000u);
            Assert.AreEqual(vm.Flag, 0m);
        }

        [TestMethod]
        [TestCategory("VM")]
        public void StoreByStackValue()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.Push, OpModes.Value, 4000u,
                OpCodes.Store, OpModes.Value, 0x8000, OpModes.StackValue,
                OpCodes.Halt
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[0x8000], 4000u);
            Assert.AreEqual(vm.Flag, 0m);
        }

        [TestMethod]
        [TestCategory("VM")]
        public void StoreByStackReference()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.Store, OpModes.Value, 0x7000, OpModes.Value, 0x0020,
                OpCodes.Push, OpModes.Value, 5000u,
                OpCodes.Push, OpModes.Value, 0x7000,
                OpCodes.Store, OpModes.StackReference, OpModes.StackValue,
                OpCodes.Halt
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[0x0020], 5000u);
            Assert.AreEqual(vm.Flag, 0m);
        }

        [TestMethod]
        [TestCategory("VM")]
        public void Swap()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.Push, OpModes.Value, 5000u,
                OpCodes.Push, OpModes.Value, 6000u,
                OpCodes.Swap,
                OpCodes.Halt
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[0x0001], 6000u);
            Assert.AreEqual(vm.Memory[0x0002], 5000u);
            Assert.AreEqual(vm.Flag, 0m);
        }

        [TestMethod]
        [TestCategory("VM")]
        public void Subroutine()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.JumpToSubroutine, OpModes.Value, 0x1004,
                OpCodes.Halt,
                OpCodes.NoOperation,
                OpCodes.Push, OpModes.Value, 100m,
                OpCodes.Swap,
                OpCodes.Return
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[0x0001], 100u);
            Assert.AreEqual(vm.Flag, 0m);
        }

        [TestMethod]
        [TestCategory("VM")]
        public void Compare()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.Push, OpModes.Value, 1000u,
                OpCodes.Push, OpModes.Value, 1000u,
                OpCodes.Compare, OpModes.StackValue, OpModes.StackValue,
                OpCodes.Halt,
            });

            vm.Start();

            Assert.IsTrue((vm.Flag & Flags.Zero) != 0);
        }

        [TestMethod]
        [TestCategory("VM")]
        public void JumpIfEqual()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.Push, OpModes.Value, 1000u,
                OpCodes.Push, OpModes.Value, 1000u,
                OpCodes.Compare, OpModes.StackValue, OpModes.StackValue,
                OpCodes.JumpIfEqual, OpModes.Value, 0x100D,
                OpCodes.Halt,
                OpCodes.Push, OpModes.Value, 100u,
                OpCodes.Swap,
                OpCodes.Return
            });

            vm.Start();
            Assert.AreEqual(vm.Memory[0x0001], 100u);
        }

        [TestMethod]
        [TestCategory("VM")]
        public void JumpIfNotEqual()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.Push, OpModes.Value, 1000u,
                OpCodes.Push, OpModes.Value, 1001u,
                OpCodes.Compare, OpModes.StackValue, OpModes.StackValue,
                OpCodes.JumpIfNotEqual, OpModes.Value, 0x100D,
                OpCodes.Halt,
                OpCodes.Push, OpModes.Value, 100u,
                OpCodes.Swap,
                OpCodes.Return
            });

            vm.Start();
            Assert.AreEqual(vm.Memory[0x0001], 100u);
        }

        [TestMethod]
        [TestCategory("VM")]
        public void ClearFlags()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.Push, OpModes.Value, 1000u,
                OpCodes.Push, OpModes.Value, 1000u,
                OpCodes.Compare, OpModes.StackValue, OpModes.StackValue,
                OpCodes.ClearFlags,
                OpCodes.Halt,
            });

            vm.Start();

            Assert.AreEqual(vm.Flag, 0u);
        }

        [TestMethod]
        [TestCategory("VM")]
        public void Increment()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.Push, OpModes.Value, 100u,
                OpCodes.Increment,
                OpCodes.Halt,
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[1], 101u);
            Assert.AreEqual(vm.Flag, 0u);
        }

        [TestMethod]
        [TestCategory("VM")]
        public void Decrement()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.Push, OpModes.Value, 100u,
                OpCodes.Decrement,
                OpCodes.Halt,
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[1], 99u);
            Assert.AreEqual(vm.Flag, 0u);
        }

        [TestMethod]
        [TestCategory("VM")]
        public void DecrementToZero()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.Push, OpModes.Value, 1u,
                OpCodes.Decrement,
                OpCodes.Halt,
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[1], 0u);
            Assert.IsTrue((vm.Flag & Flags.Zero) != 0);
        }

        [TestMethod]
        [TestCategory("VM")]
        public void DebugErrorCallback()
        {
            string debugMsg = null;

            var vm = new StackVirtualMachine((string msg) =>
            {
                debugMsg = msg;
            });

            LoadVM(new object[]
            {
                0x12345678u
            }, vm);

            try
            {
                vm.Start();
            }
            finally
            {
                Assert.IsNotNull(debugMsg);
            }
        }
    }
}
