using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Granda.ATTS.CIM.Data
{
    /// <summary>
    /// 
    /// </summary>
    internal class StreamFunction
    {
        /// <summary>
        /// 
        /// </summary>
        public byte S { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public byte F_Pri { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public byte F_Sec { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String FirstMessageDirection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String[] FunctionName { get; set; }

        private static StreamFunction[] streamFunctions = null;
        /// <summary>
        /// load stream function list under default path
        /// </summary>
        /// <returns></returns>
        public static StreamFunction[] GetStreamFunctionArray()
        {
            return streamFunctions ?? LoadStreamFunctionArray(@".\Configs\StreamFunctionList.json");
        }
        /// <summary>
        /// load stream function list under specified path.
        /// </summary>
        /// <param name="jsonPath">json file path</param>
        /// <returns></returns>
        public static StreamFunction[] LoadStreamFunctionArray(string jsonPath)
        {
            using (TextReader reader = File.OpenText(jsonPath))
            {
                streamFunctions = JArray.Load(new JsonTextReader(reader)).Values<JObject>().Select((JObject Object) =>
                    JsonConvert.DeserializeObject<StreamFunction>(Object.ToString())
                ).ToArray();
                return streamFunctions;
            }
        }

    }
}
