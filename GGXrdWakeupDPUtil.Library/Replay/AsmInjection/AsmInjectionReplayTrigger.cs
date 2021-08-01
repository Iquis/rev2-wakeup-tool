using System;
using System.Configuration;
using System.Diagnostics;
using Binarysharp.Assemblers.Fasm;
using GGXrdWakeupDPUtil.Library.Memory;

namespace GGXrdWakeupDPUtil.Library.Replay.AsmInjection
{
    public class AsmInjectionReplayTrigger : ReplayTrigger
    {
        private readonly Process _process;
        private readonly MemoryReader _memoryReader;
        private readonly IntPtr _scriptOffset = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("ScriptOffset"), 16));
        
        private readonly MemoryAllocator _memoryAllocator = new MemoryAllocator();
        private IntPtr _newMemoryAllocationBase;
        private IntPtr _flagMemoryAllocationBase;
        private IntPtr _nonRelativeScriptOffset;

        public AsmInjectionReplayTrigger(Process process, MemoryReader memoryReader)
        {
            this._process = process;
            this._memoryReader = memoryReader;
        }

        public override void InitTrigger()
        {
            LogManager.Instance.WriteLine("Initialization Asm Injection Replay Trigger");
            var baseAddress = this._process.MainModule.BaseAddress;


            _nonRelativeScriptOffset = IntPtr.Add(baseAddress, (int)_scriptOffset);

            _newMemoryAllocationBase = _memoryAllocator.AddAllocation(_process.Handle, 128);

            _flagMemoryAllocationBase = _memoryAllocator.AddAllocation(_process.Handle, 128);

            string remoteAsm = "mov ebp,[eax+0x40]\n" + "mov ebp,[ebp+0x0C]\n" + "cmp edi,3\n" +
                               $"jne 0x{(_nonRelativeScriptOffset.ToInt32() + 6):X8}\n" +
                               $"cmp BYTE [0x{IntPtr.Add(_flagMemoryAllocationBase, 1).ToString("X8")}], 1\n" +
                               $"je 0x{IntPtr.Add(_newMemoryAllocationBase, 0x49).ToString("X8")}\n" +
                               $"mov DWORD [0x{IntPtr.Add(_flagMemoryAllocationBase, 4).ToString("X8")}], 0x200\n" +
                               $"and DWORD [0x{IntPtr.Add(_flagMemoryAllocationBase, 4).ToString("X8")}], eax\n" +
                               $"cmp DWORD [0x{IntPtr.Add(_flagMemoryAllocationBase, 4).ToString("X8")}], 0x200\n" +
                               $"jne 0x{(_nonRelativeScriptOffset.ToInt32() + 6):X8}\n" +
                               $"mov DWORD [0x{IntPtr.Add(_flagMemoryAllocationBase, 4).ToString("X8")}], eax\n" +
                               $"mov BYTE [0x{IntPtr.Add(_flagMemoryAllocationBase, 1).ToString("X8")}], 1\n" +
                               $"jmp 0x{(_nonRelativeScriptOffset.ToInt32() + 6):X8}\n" +
                               $"cmp DWORD [0x{IntPtr.Add(_flagMemoryAllocationBase, 4).ToString("X8")}], eax\n" +
                               $"jne 0x{(_nonRelativeScriptOffset.ToInt32() + 6):X8}\n" +
                               $"cmp BYTE [0x{_flagMemoryAllocationBase.ToString("X8")}],0\n" +
                               $"jne 0x{(_nonRelativeScriptOffset.ToInt32() + 6):X8}\n" + "mov ebp,[edx]\n" +
                               $"mov BYTE [0x{_flagMemoryAllocationBase.ToString("X8")}], 1\n" +
                               $"jmp 0x{(_nonRelativeScriptOffset.ToInt32() + 6):X8}";
            
            byte[] remoteCode = Assemble(remoteAsm, _newMemoryAllocationBase);

            this._memoryReader.Write(_newMemoryAllocationBase, remoteCode);
        }

        public override void TriggerReplay()
        {
            InitializeReplayFeature();

            LogManager.Instance.WriteLine("Trigger Replay by Asm Injection");
            this._memoryReader.Write(_flagMemoryAllocationBase, new byte[] { 0 });
        }


        private void InitializeReplayFeature()
        {
            this._memoryReader.Write(_flagMemoryAllocationBase, new byte[] { 1 });

            string mnemonic = $"jmp 0x{_newMemoryAllocationBase.ToString("X8")}\nnop";

            var bytes = Assemble(mnemonic, _nonRelativeScriptOffset);

            this._memoryReader.Write(_nonRelativeScriptOffset, bytes);
        }

        public override void Dispose()
        {
            string mnemonics = "mov ebp,[eax+0x40]\nmov ebp,[ebp+0x0C]\n";

            var bytes = Assemble(mnemonics, _nonRelativeScriptOffset);

            this._memoryReader.Write(_nonRelativeScriptOffset, bytes);

        }



        private byte[] Assemble(string mnemonic, IntPtr baseAddress)
        {
            mnemonic = $"use32\norg 0x{(object)baseAddress.ToInt64():X8}\n" + mnemonic;

            return FasmNet.Assemble(mnemonic);
        }
    }
}
