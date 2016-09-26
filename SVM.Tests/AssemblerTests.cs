using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SVM.Assembler;

namespace SVM.Tests
{
    [TestClass]
    public class AssemblerTests : BaseTests
    {
        [TestCategory("Assembler")]
        [TestMethod]
        public void PushByValue()
        {
            var assembler = new SVMAssembler();
            var output = assembler.Assemble(
                "PSH $FF00\n" +
                "HLT");

            AssertDeepEqual<uint>(output, new uint[]
            {
                OpCodes.Push, OpModes.Value, 0xFF00u,
                OpCodes.Halt
            });
        }

        [TestCategory("Assembler")]
        [TestMethod]
        public void PushByReference()
        {
            var assembler = new SVMAssembler();
            var output = assembler.Assemble(
                "PSH #FF00\n" +
                "HLT");

            AssertDeepEqual<uint>(output, new uint[]
            {
                OpCodes.Push, OpModes.Reference, 0xFF00u,
                OpCodes.Halt
            });
        }

        [TestCategory("Assembler")]
        [TestMethod]
        public void PushByStackValue()
        {
            var assembler = new SVMAssembler();
            var output = assembler.Assemble(
                "PSH $^\n" +
                "HLT");

            AssertDeepEqual<uint>(output, new uint[]
            {
                OpCodes.Push, OpModes.StackValue,
                OpCodes.Halt
            });
        }

        [TestCategory("Assembler")]
        [TestMethod]
        public void PushByStackReference()
        {
            var assembler = new SVMAssembler();
            var output = assembler.Assemble(
                "PSH $^\n" +
                "HLT");

            AssertDeepEqual<uint>(output, new uint[]
            {
                OpCodes.Push, OpModes.StackValue,
                OpCodes.Halt
            });
        }

        [TestCategory("Assembler")]
        [TestMethod]
        public void InvalidInstructionSyntaxError()
        {
            Exception ex = null;

            var assembler = new SVMAssembler();

            try
            {
                var output = assembler.Assemble(
                    "aaaPSH $^\n" +
                    "HLT");
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.IsInstanceOfType(ex, typeof(AssemblerSyntaxErrorException));
        }

        [TestCategory("Assembler")]
        [TestMethod]
        public void InvalidOperandSyntaxError()
        {
            Exception ex = null;

            var assembler = new SVMAssembler();

            try
            {
                var output = assembler.Assemble(
                    "PSH F___F00\n" +
                    "HLT");
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.IsInstanceOfType(ex, typeof(AssemblerSyntaxErrorException));
        }
    }
}
