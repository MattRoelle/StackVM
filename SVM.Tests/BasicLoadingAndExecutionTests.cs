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
        public void PushByValue()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.PSH, OpModes.Value, 100u,
                OpCodes.HLT
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[1], 100u);
            Assert.AreEqual(vm.Flag, 0m);
        }

        [TestMethod]
        public void AddWithoutCarry()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.PSH, OpModes.Value, 100u,
                OpCodes.PSH, OpModes.Value, 200u,
                OpCodes.ADD,
                OpCodes.HLT
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[1], 300u);
            Assert.AreEqual(vm.Flag, 0m);
        }

        [TestMethod]
        public void AddWithCarry()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.PSH, OpModes.Value, 0xFFFFFFFFu,
                OpCodes.PSH, OpModes.Value, 200u,
                OpCodes.ADD,
                OpCodes.HLT
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[1], 199u);
            Assert.AreEqual(vm.Flag, (uint)(Flags.Carry));
        }

        [TestMethod]
        public void SubtractWithoutCarry()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.PSH, OpModes.Value, 100u,
                OpCodes.PSH, OpModes.Value, 200u,
                OpCodes.SUB,
                OpCodes.HLT
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[1], 100u);
            Assert.AreEqual(vm.Flag, 0m);
        }

        [TestMethod]
        public void SubtractWithCarry()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.PSH, OpModes.Value, 0xFFFFFFFFu,
                OpCodes.PSH, OpModes.Value, 200u,
                OpCodes.SUB,
                OpCodes.HLT
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[1], 201u);
            Assert.AreEqual(vm.Flag, (uint)(Flags.Carry));
        }

        [TestMethod]
        public void StoreByValue()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.STO, OpModes.Value, 0x8000, OpModes.Value, 2000u,
                OpCodes.HLT
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[0x8000], 2000u);
            Assert.AreEqual(vm.Flag, 0m);
        }

        [TestMethod]
        public void StoreByReference()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.STO, OpModes.Value, 0x8000, OpModes.Value, 0x3000,
                OpCodes.STO, OpModes.Reference, 0x8000, OpModes.Value, 2000u,
                OpCodes.HLT
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[0x3000], 2000u);
            Assert.AreEqual(vm.Flag, 0m);
        }

        [TestMethod]
        public void PushByReference()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.STO, OpModes.Value, 0x8000, OpModes.Value, 2000u,
                OpCodes.PSH, OpModes.Reference, 0x8000u,
                OpCodes.HLT
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[1], 2000u);
            Assert.AreEqual(vm.Flag, 0m);
        }

        [TestMethod]
        public void StoreByStackValue()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.PSH, OpModes.Value, 4000u,
                OpCodes.STO, OpModes.Value, 0x8000, OpModes.StackValue,
                OpCodes.HLT
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[0x8000], 4000u);
            Assert.AreEqual(vm.Flag, 0m);
        }

        [TestMethod]
        public void StoreByStackReference()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.STO, OpModes.Value, 0x7000, OpModes.Value, 0x0020,
                OpCodes.PSH, OpModes.Value, 5000u,
                OpCodes.PSH, OpModes.Value, 0x7000,
                OpCodes.STO, OpModes.StackReference, OpModes.StackValue,
                OpCodes.HLT
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[0x0020], 5000u);
            Assert.AreEqual(vm.Flag, 0m);
        }

        [TestMethod]
        public void Swap()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.PSH, OpModes.Value, 5000u,
                OpCodes.PSH, OpModes.Value, 6000u,
                OpCodes.SWP,
                OpCodes.HLT
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[0x0001], 6000u);
            Assert.AreEqual(vm.Memory[0x0002], 5000u);
            Assert.AreEqual(vm.Flag, 0m);
        }

        [TestMethod]
        public void Subroutine()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.JMP, OpModes.Value, 0x1004,
                OpCodes.HLT,
                OpCodes.NOP,
                OpCodes.PSH, OpModes.Value, 100m,
                OpCodes.SWP,
                OpCodes.RET
            });

            vm.Start();

            Assert.AreEqual(vm.Memory[0x0001], 100u);
            Assert.AreEqual(vm.Flag, 0m);
        }

        [TestMethod]
        public void Compare()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.PSH, OpModes.Value, 1000u,
                OpCodes.PSH, OpModes.Value, 1000u,
                OpCodes.CMP, OpModes.StackValue, OpModes.StackValue,
                OpCodes.HLT,
            });

            vm.Start();

            Assert.IsTrue((vm.Flag & (uint)Flags.Equal) != 0);
        }

        [TestMethod]
        public void JumpIfEqual()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.PSH, OpModes.Value, 1000u,
                OpCodes.PSH, OpModes.Value, 1000u,
                OpCodes.CMP, OpModes.StackValue, OpModes.StackValue,
                OpCodes.JEQ, OpModes.Value, 0x100D,
                OpCodes.HLT,
                OpCodes.PSH, OpModes.Value, 100u,
                OpCodes.SWP,
                OpCodes.RET
            });

            vm.Start();
            Assert.AreEqual(vm.Memory[0x0001], 100u);
        }

        [TestMethod]
        public void JumpIfNotEqual()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.PSH, OpModes.Value, 1000u,
                OpCodes.PSH, OpModes.Value, 1001u,
                OpCodes.CMP, OpModes.StackValue, OpModes.StackValue,
                OpCodes.JNE, OpModes.Value, 0x100D,
                OpCodes.HLT,
                OpCodes.PSH, OpModes.Value, 100u,
                OpCodes.SWP,
                OpCodes.RET
            });

            vm.Start();
            Assert.AreEqual(vm.Memory[0x0001], 100u);
        }

        [TestMethod]
        public void ClearFlags()
        {
            var vm = LoadVM(new object[]
            {
                OpCodes.PSH, OpModes.Value, 1000u,
                OpCodes.PSH, OpModes.Value, 1000u,
                OpCodes.CMP, OpModes.StackValue, OpModes.StackValue,
                OpCodes.CLR,
                OpCodes.HLT,
            });

            vm.Start();

            Assert.AreEqual(vm.Flag, 0u);
        }

        [TestMethod]
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
