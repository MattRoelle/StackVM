using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SVM.Assembler;

namespace SVM.IDE
{
    public partial class StackVirtualMachineIDE : Form
    {
        private StackVirtualMachine _vm;

        private BackgroundWorker _vmBackgroundThread;
        private BackgroundWorker _uiUpdateThread;

        public StackVirtualMachineIDE()
        {
            InitializeComponent();

            _uiUpdateThread = new BackgroundWorker();
            _uiUpdateThread.DoWork += (sender, args) =>
            {
                while(true)
                {
                    RenderVMState();
                }
            };
            _uiUpdateThread.RunWorkerAsync();
        }

        private void SPTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }


        private uint _lastStackPointer = 0;
        private uint _lastTopOfStackVal = 0;
        private void RenderVMState()
        {
            if (_vm != null)
            {
                IPTextBox.Invoke((Action) delegate { IPTextBox.Text = _vm.InstructionPointer.ToString("X"); });
                SPTextBox.Invoke((Action) delegate { SPTextBox.Text = _vm.StackPointer.ToString("X"); });

                if (_vm.StackPointer > 0)
                {
                    var topOfStackVal = _vm.Memory[_vm.StackPointer - 1];
                    if (_lastStackPointer != _vm.StackPointer || _lastTopOfStackVal != topOfStackVal)
                    {
                        _lastStackPointer = _vm.StackPointer;
                        _lastTopOfStackVal = topOfStackVal;

                        var stackList = new string[_vm.StackPointer];
                        for(var i = 0; i < _vm.StackPointer; i++)
                        {
                            stackList[i] = _vm.Memory[i + 1].ToString("X");
                        }

                        StackListBox.Invoke((Action) delegate { StackListBox.DataSource = stackList; });
                    }
                }
            }
        }

        private void ResetAndLoadButton_Click(object sender, EventArgs e)
        {
            try
            {
                _vm = new StackVirtualMachine(LogToOutput);
                _vm.State = SVMSStates.Idle;
                var assembler = new SVMAssembler();
                var program = assembler.Assemble(CodeInput.Text);

                _vm.Load(program);

                _vmBackgroundThread = new BackgroundWorker();
                _vmBackgroundThread.DoWork += (o, args) =>
                {
                    _vm.Start();
                };
                _vmBackgroundThread.RunWorkerAsync();

                LogToOutput("Loaded. VM Running in idle state");
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(AssemblerSyntaxErrorException))
                {
                    LogToOutput(ex.Message);
                }
                else
                {
                    LogToOutput(ex.ToString());
                }
            }
        }

        private void Tick()
        {

        }

        private void LogToOutput(string message)
        {
            Output.AppendText(message + "\n");
        }

        private void StepButton_Click(object sender, EventArgs e)
        {
            if (_vm != null)
            {
                _vm.State = SVMSStates.Step;
            }
        }
    }
}
