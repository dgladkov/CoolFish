using System;
using System.Runtime.InteropServices;

namespace CoolFishNS.Management.CoolManager.HookingLua
{
    public static class Asm
    {
        [DllImport("FASM_1.71.21.DLL", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr fasm_Assemble(IntPtr source, IntPtr mem, int size, int passes, IntPtr displayPipe);

        /// <summary>
        /// Assembles an 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="memSize"></param>
        /// <param name="passLimit"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static byte[] Assemble(string a, int memSize = 0x1000, int passLimit = 500)
        {
            var str = Marshal.StringToHGlobalAnsi(a);

            var bytes = Marshal.AllocHGlobal(memSize);
            try
            {
                fasm_Assemble(str, bytes, memSize, passLimit, IntPtr.Zero);

                var state = (FasmState)Marshal.PtrToStructure(bytes, typeof(FasmState));

                if (state.condition == FasmResponse.FASM_OK)
                {
                    var b = new byte[state.output_length];

                    Marshal.Copy(state.output_data, b, 0, state.output_length);

                    return b;
                }
                var error = (FasmLineHeader)Marshal.PtrToStructure(state.error_line, typeof(FasmLineHeader));
                throw new Exception(string.Format("Assembling failed!{0}State:\t{1}{0}Error:\t{2}{0}line:\t{3} ",
                    Environment.NewLine, state.condition, state.error_code, error.line_number));
            }
            finally 
            {
                Marshal.FreeHGlobal(str);
                Marshal.FreeHGlobal(bytes);
            }

        }

        private enum FasmResponse
        {
            FASM_OK = 0,
            FASM_WORKING = 1,
            FASM_ERROR = 2,
            FASM_INVALID_PARAMETER = -1,
            FASM_OUT_OF_MEMORY = -2,
            FASM_STACK_OVERFLOW = -3,
            FASM_SOURCE_NOT_FOUND = -4,
            FASM_UNEXPECTED_END_OF_SOURCE = -5,
            FASM_CANNOT_GENERATE_CODE = -6,
            FASM_FORMAT_LIMITATIONS_EXCEDDED = -7,
            FASM_WRITE_FAILED = -8
        }

        private enum FasmError
        {
            FASMERR_FILE_NOT_FOUND = -101,
            FASMERR_ERROR_READING_FILE = -102,
            FASMERR_INVALID_FILE_FORMAT = -103,
            FASMERR_INVALID_MACRO_ARGUMENTS = -104,
            FASMERR_INCOMPLETE_MACRO = -105,
            FASMERR_UNEXPECTED_CHARACTERS = -106,
            FASMERR_INVALID_ARGUMENT = -107,
            FASMERR_ILLEGAL_INSTRUCTION = -108,
            FASMERR_INVALID_OPERAND = -109,
            FASMERR_INVALID_OPERAND_SIZE = -110,
            FASMERR_OPERAND_SIZE_NOT_SPECIFIED = -111,
            FASMERR_OPERAND_SIZES_DO_NOT_MATCH = -112,
            FASMERR_INVALID_ADDRESS_SIZE = -113,
            FASMERR_ADDRESS_SIZES_DO_NOT_AGREE = -114,
            FASMERR_DISALLOWED_COMBINATION_OF_REGISTERS = -115,
            FASMERR_LONG_IMMEDIATE_NOT_ENCODABLE = -116,
            FASMERR_RELATIVE_JUMP_OUT_OF_RANGE = -117,
            FASMERR_INVALID_EXPRESSION = -118,
            FASMERR_INVALID_ADDRESS = -119,
            FASMERR_INVALID_VALUE = -120,
            FASMERR_VALUE_OUT_OF_RANGE = -121,
            FASMERR_UNDEFINED_SYMBOL = -122,
            FASMERR_INVALID_USE_OF_SYMBOL = -123,
            FASMERR_NAME_TOO_LONG = -124,
            FASMERR_INVALID_NAME = -125,
            FASMERR_RESERVED_WORD_USED_AS_SYMBOL = -126,
            FASMERR_SYMBOL_ALREADY_DEFINED = -127,
            FASMERR_MISSING_END_QUOTE = -128,
            FASMERR_MISSING_END_DIRECTIVE = -129,
            FASMERR_UNEXPECTED_INSTRUCTION = -130,
            FASMERR_EXTRA_CHARACTERS_ON_LINE = -131,
            FASMERR_SECTION_NOT_ALIGNED_ENOUGH = -132,
            FASMERR_SETTING_ALREADY_SPECIFIED = -133,
            FASMERR_DATA_ALREADY_DEFINED = -134,
            FASMERR_TOO_MANY_REPEATS = -135,
            FASMERR_SYMBOL_OUT_OF_SCOPE = -136,
            FASMERR_USER_ERROR = -140,
            FASMERR_ASSERTION_FAILED = -141
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct FasmLineHeader
        {
            [FieldOffset(0)] public readonly IntPtr file_path;

            [FieldOffset(4)] public readonly uint line_number;

            [FieldOffset(8)] public readonly int file_offset;

            [FieldOffset(8)] public readonly int macro_calling_line;

            [FieldOffset(12)] public readonly IntPtr macro_line;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct FasmState
        {
            [FieldOffset(0)] public readonly FasmResponse condition;

            [FieldOffset(4)] public readonly int output_length;

            [FieldOffset(4)] public readonly FasmError error_code;

            [FieldOffset(8)] public readonly IntPtr output_data;

            [FieldOffset(8)] public readonly IntPtr error_line;
        }
    }
}