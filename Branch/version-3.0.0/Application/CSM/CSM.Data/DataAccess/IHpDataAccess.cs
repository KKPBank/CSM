using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Data.DataAccess
{
    public interface IHpDataAccess
    {
        bool SaveHpActivity(List<HpActivityEntity> hpActivitys);
        bool SaveHpActivityComplete(ref int numOfComplete, ref int numOfError, ref string messageError);
        List<HpActivityEntity> GetHpActivityExport();
        List<ServiceRequestForSaveEntity> GetSrWithHpActivity();
    }
}
