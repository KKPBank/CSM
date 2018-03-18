using CSM.Common.Utilities;
using CSM.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSM.Service.Messages.SchedTask;

namespace CSM.Data.DataAccess
{
    public class HRIDataAccess : BaseDA<HRIDataAccess>
    {
        public HRIDataAccess(CSMContext context) : base(context) { }

        private int Save()
        {
            return _context.SaveChanges();
        }

        public bool InsertHRTempTable(List<HRIEmployeeEntity> data)
        {
            SqlConnection con = null;
            SqlBulkCopy bc = null;
            try
            {
                if (data != null && data.Any())
                {
                    //_context.Database.ExecuteSqlCommand("TRUNCATE TABLE TB_I_HR_EMPLOYEE;");
                    _context.Database.ExecuteSqlCommand("DELETE FROM TB_I_HR_EMPLOYEE;");
                    Save();

                    int pageSize = 5000;
                    int totalPage = (data.Count() + pageSize - 1) / pageSize;

                    Task.Factory.StartNew(() => Parallel.For(0, totalPage, new ParallelOptions { MaxDegreeOfParallelism = WebConfig.GetTotalCountToProcess() },
                    k =>
                    {
                        var lst = from d in data.Skip(k * pageSize).Take(pageSize)
                                  select
                                  new TB_I_HR_EMPLOYEE
                                  {
                                      BRANCH = d.Branch,
                                      BRANCHDESC = d.BranchDesc,
                                      EMPLOYEEID = d.EmployeeId,
                                      TITLEID = d.TitleId,
                                      TTITLE = d.Title,
                                      TFNAME = d.FName,
                                      TLNAME = d.LName,
                                      NICKNAME = d.Nickname,
                                      XENGNAME = d.FullNameEng,
                                      ETITLE = d.ETitle,
                                      EFNAME = d.EFName,
                                      ELNAME = d.ELName,
                                      SEX = d.Sex,
                                      BIRTHDAY = d.BirthDay,
                                      EMPTYPE = d.EmpType,
                                      EMPTYPEDESC = d.EmpTypeDesc,
                                      POSITION = d.Position,
                                      POSITONDESC = d.PositionDesc,
                                      BU1 = d.BU1,
                                      BU1DESC = d.BU1Desc,
                                      BU2 = d.BU2,
                                      BU2DESC = d.BU2Desc,
                                      BU3 = d.BU3,
                                      BU3DESC = d.BU3Desc,
                                      BU4 = d.BU4,
                                      BU4DESC = d.BU4Desc,
                                      JOB = d.Job,
                                      JOBPOSITION = d.JobPosition,
                                      STARTDATE = d.StartDate,
                                      FIRSTHIREDATE = d.FirstHireDate,
                                      RESIGNDATE = d.ResignDate,
                                      STATUS = d.Status,
                                      EMAIL = d.Email,
                                      NOTESADDRESS = d.NotesAddress,
                                      WORKAREA = d.WorkArea,
                                      WORKAREADESC = d.WorkAreaDesc,
                                      COSTCENTER = d.CostCenter,
                                      COSTCENTERDESC = d.CostCenterDesc,
                                      TELEXT = d.TelExt,
                                      BOSS = d.Boss,
                                      BOSSNAME = d.BossName,
                                      ASSESSOR1 = d.Assessor1,
                                      ASSESSOR1NAME = d.Assessor1Name,
                                      ASSESSOR2 = d.Assessor2,
                                      ASSESSOR2NAME = d.Assessor2Name,
                                      ASSESSOR3 = d.Assessor3,
                                      ASSESSOR3NAME = d.Assessor3Name,
                                      TELNO = d.TelNo,
                                      MOBILENO = d.MobileNo,
                                      ADUSER = d.ADUser,
                                      OFFICER_ID = d.OfficerId,
                                      OFFICER_DESC = d.OfficerDesc,
                                      ADDITIONJOB = d.AdditionJob,
                                      UNITBOSS = d.UnitBoss,
                                      UNITBOSSNAME = d.UnitBossName,
                                      IDNO = d.IDNO,
                                      ERROR = d.Error
                                  };

                        DataTable dt = DataTableHelpers.ConvertTo(lst);

                        con = new SqlConnection(WebConfig.GetConnectionString("CSMConnectionString"));
                        bc = new SqlBulkCopy(con.ConnectionString, SqlBulkCopyOptions.TableLock);

                        bc.DestinationTableName = "TB_I_HR_EMPLOYEE";
                        bc.BatchSize = dt.Rows.Count;
                        con.Open();
                        bc.WriteToServer(dt);
                    })).Wait();

                    data = null; // clear
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
            }
            finally
            {
                if (bc != null)
                    bc.Close();
                if (con != null)
                    con.Close();
            }
            return false;
        }

        public bool UpdateHREmployee(out int empInsert, out int empUpdate, out int empMarkDelete
                                    , out int buInsert, out int buUpdate, out int buMarkDelete
                                    , out int brInsert, out int brUpdate, out int brMarkDelete, out string msg)
        {
            bool result = false;
            empInsert =
            empUpdate =
            empMarkDelete =
            buInsert =
            buUpdate =
            buMarkDelete =
            brInsert =
            brUpdate =
            brMarkDelete = 0;
            msg = string.Empty;

            try
            {
                System.Data.Entity.Core.Objects.ObjectParameter paraEmpInsert = new System.Data.Entity.Core.Objects.ObjectParameter("o_empInsert", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter paraEmpUpdate = new System.Data.Entity.Core.Objects.ObjectParameter("o_empUpdate", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter paraEmpMarkDelete = new System.Data.Entity.Core.Objects.ObjectParameter("o_empMarkDelete", typeof(int));

                System.Data.Entity.Core.Objects.ObjectParameter paraBUInsert = new System.Data.Entity.Core.Objects.ObjectParameter("o_buInsert", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter paraBUUpdate = new System.Data.Entity.Core.Objects.ObjectParameter("o_buUpdate", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter paraBUMarkDelete = new System.Data.Entity.Core.Objects.ObjectParameter("o_buMarkDelete", typeof(int));

                System.Data.Entity.Core.Objects.ObjectParameter paraBRInsert = new System.Data.Entity.Core.Objects.ObjectParameter("o_brInsert", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter paraBRUpdate = new System.Data.Entity.Core.Objects.ObjectParameter("o_brUpdate", typeof(int));
                System.Data.Entity.Core.Objects.ObjectParameter paraBRMarkDelete = new System.Data.Entity.Core.Objects.ObjectParameter("o_brMarkDelete", typeof(int));

                System.Data.Entity.Core.Objects.ObjectParameter paraSucc = new System.Data.Entity.Core.Objects.ObjectParameter("o_succ", typeof(bool));
                System.Data.Entity.Core.Objects.ObjectParameter paraMsg = new System.Data.Entity.Core.Objects.ObjectParameter("o_msg", typeof(string));

                _context.Database.CommandTimeout = Constants.BatchCommandTimeout;

                _context.SP_IMPORT_HRIS(paraEmpInsert, paraEmpUpdate, paraEmpMarkDelete, paraBUInsert, paraBUUpdate, paraBUMarkDelete
                                        , paraBRInsert, paraBRUpdate, paraBRMarkDelete, paraSucc, paraMsg);

                result = (bool)paraSucc.Value;

                empInsert = (int)paraEmpInsert.Value;
                empUpdate = (int)paraEmpUpdate.Value;
                empMarkDelete = (int)paraEmpMarkDelete.Value;
                buInsert = (int)paraBUInsert.Value;
                buUpdate = (int)paraBUUpdate.Value;
                buMarkDelete = (int)paraBUMarkDelete.Value;
                brInsert = (int)paraBRInsert.Value;
                brUpdate = (int)paraBRUpdate.Value;
                brMarkDelete = (int)paraBRMarkDelete.Value;
                msg = (string)paraMsg.Value;

            }
            catch (Exception ex)
            {
                Logger.Error("Exception occur:\n", ex);
                msg = ex.ToString();
            }
            return result;
        }
    }
}
