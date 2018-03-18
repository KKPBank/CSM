using System.Collections.Generic;

///<summary>
/// Class Name : Select2PagedResult
/// Purpose    : Extra classes to format the results the way the select2 dropdown wants them
/// Author     : Neda Peyrone
///</summary>
///<remarks>
/// Change History:
/// Date           Author           Description
/// ------         ------           -----------
///</remarks>
namespace CSM.Entity.Common
{
    public class Select2PagedResult
    {
        public int Total { get; set; }
        public List<Select2Result> Results { get; set; }
    }

    public class Select2Result
    {
        public object id { get; set; }
        public string text { get; set; }
    }
}
