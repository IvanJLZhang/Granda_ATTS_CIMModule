using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Granda.ATTS.CIMModule.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class StreamFunction
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

        /// <summary>
        /// load stream function list under default path
        /// </summary>
        /// <returns></returns>
        public static StreamFunction[] LoadStreamFunctionArray()
        {
            return LoadStreamFunctionArray("./Data/StreamFunctionList.json");
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
                return JArray.Load(new JsonTextReader(reader)).Values<StreamFunction>().ToArray();
            }
        }

    }
}
