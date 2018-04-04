using Granda.ATTS.CIMModule.Data;
using Secs4Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Granda.ATTS.CIMModule.Extension
{
    internal static class ExtensionHelper
    {

        public static string GetName(this SecsFormat format)
        {
            switch (format)
            {
                case SecsFormat.List: return nameof(SecsFormat.List);
                case SecsFormat.ASCII: return nameof(SecsFormat.ASCII);
                case SecsFormat.JIS8: return nameof(SecsFormat.JIS8);
                case SecsFormat.Boolean: return nameof(SecsFormat.Boolean);
                case SecsFormat.Binary: return nameof(SecsFormat.Binary);
                case SecsFormat.U1: return nameof(SecsFormat.U1);
                case SecsFormat.U2: return nameof(SecsFormat.U2);
                case SecsFormat.U4: return nameof(SecsFormat.U4);
                case SecsFormat.U8: return nameof(SecsFormat.U8);
                case SecsFormat.I1: return nameof(SecsFormat.I1);
                case SecsFormat.I2: return nameof(SecsFormat.I2);
                case SecsFormat.I4: return nameof(SecsFormat.I4);
                case SecsFormat.I8: return nameof(SecsFormat.I8);
                case SecsFormat.F4: return nameof(SecsFormat.F4);
                case SecsFormat.F8: return nameof(SecsFormat.F8);
                default: throw new ArgumentOutOfRangeException(nameof(format), (int)format, @"Invalid enum value");
            }
        }
        public static string ToHexString(this byte[] value)
        {
            if (value.Length == 0) return string.Empty;
            int length = value.Length * 3;
            char[] chs = new char[length];
            for (int ci = 0, i = 0; ci < length; ci += 3)
            {
                byte num = value[i++];
                chs[ci] = GetHexValue(num / 0x10);
                chs[ci + 1] = GetHexValue(num % 0x10);
                chs[ci + 2] = ' ';
            }
            return new string(chs, 0, length - 1);

            char GetHexValue(int i) => (i < 10) ? (char)(i + 0x30) : (char)((i - 10) + 0x41);
        }
        public static void Reverse(this byte[] bytes, int begin, int end, int offSet)
        {
            if (offSet <= 1) return;
            for (var i = begin; i < end; i += offSet)
                Array.Reverse(bytes, i, offSet);
        }

        public static string GetFunctionName(byte s, byte f)
        {
            var streamFunctions = StreamFunction.GetStreamFunctionArray();
            return GetFunctionName(streamFunctions, s, f, "", 0);
        }
        public static string GetFunctionName(byte s, byte f, string key, int value)
        {
            var streamFunctions = StreamFunction.GetStreamFunctionArray();
            return GetFunctionName(streamFunctions, s, f, key, value);
        }
        public static string GetFunctionName(StreamFunction[] streamFunctions, byte s, byte f, string key, int ceid)
        {
            foreach (var item in streamFunctions)
            {
                if (s == item.S && (f == item.F_Pri || f == item.F_Sec))
                {
                    if (ceid == 0 && key.Equals(String.Empty)) return f % 2 == 0 ? item.FunctionName[ceid] + " Ack" : item.FunctionName[ceid];
                    else
                    {
                        foreach (var function in item.FunctionName)
                        {
                            if (function.Contains($"{key}={ceid}"))
                                return f % 2 == 0 ? function + " Ack" : function;
                        }
                    }
                }
            }
            return null;
        }


        public static SecsMessage SendMessage(this SecsGem secsGem, short deviceId, byte s, byte f, bool replyExpected, int systemBytes, Item item = null, string key = "", int value = 0)
        {
            SecsMessage secsMessage = new SecsMessage(deviceId, s, f, GetFunctionName(s, f, key, value), replyExpected, systemBytes, item);
            return secsGem.Send(secsMessage);
        }
        public static String GetSFString(this SecsMessage secsMessage)
        {
            return $"S{secsMessage.S}F{secsMessage.F}";
        }
        /// <summary>
        /// 获取secsMessage中的command位
        /// </summary>
        /// <param name="secsMessage"></param>
        /// <returns></returns>
        public static int GetCommandValue(this SecsMessage secsMessage)
        {
            var commandItem = secsMessage.SecsItem.Count >= 1 ? secsMessage.SecsItem.Items[0] : null;
            if (commandItem != null)
            {
                Int32.TryParse(commandItem.GetString(), out int result);
                return result;
            }
            return -1;
        }
    }
}
