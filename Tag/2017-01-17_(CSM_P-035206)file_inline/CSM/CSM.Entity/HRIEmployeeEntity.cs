using CSM.Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSM.Entity
{
    public class HRIEmployeeEntity
    {
        public int? HREmployeeId { get; set; }
        [Display(Name = "รหัสบริษัท")]
        [LocalizedStringLength(Constants.HRIMaxLength.Branch, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Branch { get; set; }
        [Display(Name = "ชื่อบริษัท")]
        [LocalizedStringLength(Constants.HRIMaxLength.BranchDesc, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string BranchDesc { get; set; }
        [Display(Name = "รหัสพนักงาน")]
        [LocalizedStringLength(Constants.HRIMaxLength.EmployeeId, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string EmployeeId { get; set; }
        [Display(Name = "รหัสคำนำหน้าชื่อ")]
        [LocalizedStringLength(Constants.HRIMaxLength.TitleId, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TitleId { get; set; }
        [Display(Name = "คำนำหน้าชื่อ")]
        [LocalizedStringLength(Constants.HRIMaxLength.Title, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Title { get; set; }
        [Display(Name = "ชื่อ")]
        [LocalizedStringLength(Constants.HRIMaxLength.FName, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FName { get; set; }
        [Display(Name = "นามสกุล")]
        [LocalizedStringLength(Constants.HRIMaxLength.LName, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string LName { get; set; }
        [Display(Name = "ชื่อเล่น")]
        [LocalizedStringLength(Constants.HRIMaxLength.Nickname, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Nickname { get; set; }
        [Display(Name = "ชื่อ นามสกุลภาษาอังกฤษ")]
        [LocalizedStringLength(Constants.HRIMaxLength.FullNameEng, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FullNameEng { get; set; }
        [Display(Name = "คำนำหน้าชื่อภาษาอังกฤษ")]
        [LocalizedStringLength(Constants.HRIMaxLength.ETitle, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ETitle { get; set; }
        [Display(Name = "ชื่อภาษาอังกฤษ")]
        [LocalizedStringLength(Constants.HRIMaxLength.EFName, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string EFName { get; set; }
        [Display(Name = "นามสกุลภาษาอังกฤษ")]
        [LocalizedStringLength(Constants.HRIMaxLength.ELName, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ELName { get; set; }
        [Display(Name = "เพศ")]
        [LocalizedStringLength(Constants.HRIMaxLength.Sex, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Sex { get; set; }
        [Display(Name = "วันเดือนปีเกิด")]
        [LocalizedStringLength(Constants.HRIMaxLength.BirthDay, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string BirthDay { get; set; }
        [Display(Name = "รหัสประเภทพนักงาน")]
        [LocalizedStringLength(Constants.HRIMaxLength.EmpType, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string EmpType { get; set; }
        [Display(Name = "ชื่อประเภทพนักงาน")]
        [LocalizedStringLength(Constants.HRIMaxLength.EmpTypeDesc, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string EmpTypeDesc { get; set; }
        [Display(Name = "รหัสระดับพนักงาน")]
        [LocalizedStringLength(Constants.HRIMaxLength.Position, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Position { get; set; }
        [Display(Name = "ชื่อระดับพนักงาน")]
        [LocalizedStringLength(Constants.HRIMaxLength.PositionDesc, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string PositionDesc { get; set; }
        [Display(Name = "รหัสสาย")]
        [LocalizedStringLength(Constants.HRIMaxLength.BU1, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string BU1 { get; set; }
        [Display(Name = "ชื่อสาย")]
        [LocalizedStringLength(Constants.HRIMaxLength.BU1Desc, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string BU1Desc { get; set; }
        [Display(Name = "รหัสฝ่าย")]
        [LocalizedStringLength(Constants.HRIMaxLength.BU2, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string BU2 { get; set; }
        [Display(Name = "ชื่อฝ่าย")]
        [LocalizedStringLength(Constants.HRIMaxLength.BU2Desc, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string BU2Desc { get; set; }
        [Display(Name = "รหัสทีม")]
        [LocalizedStringLength(Constants.HRIMaxLength.BU3, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string BU3 { get; set; }
        [Display(Name = "ชื่อทีม")]
        [LocalizedStringLength(Constants.HRIMaxLength.BU3Desc, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string BU3Desc { get; set; }
        [Display(Name = "รหัสทีมย่อย")]
        [LocalizedStringLength(Constants.HRIMaxLength.BU4, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string BU4 { get; set; }
        [Display(Name = "ชื่อทีมย่อย")]
        [LocalizedStringLength(Constants.HRIMaxLength.BU4Desc, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string BU4Desc { get; set; }
        [Display(Name = "รหัสตำแหน่ง")]
        [LocalizedStringLength(Constants.HRIMaxLength.Job, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Job { get; set; }
        [Display(Name = "ชื่อตำแหน่งงาน")]
        [LocalizedStringLength(Constants.HRIMaxLength.JobPosition, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string JobPosition { get; set; }
        [Display(Name = "วันที่ทำสัญญา")]
        [LocalizedStringLength(Constants.HRIMaxLength.StartDate, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string StartDate { get; set; }
        [Display(Name = "วันที่เริ่มงาน")]
        [LocalizedStringLength(Constants.HRIMaxLength.FirstHireDate, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string FirstHireDate { get; set; }
        [Display(Name = "วันพ้นสภาพ")]
        [LocalizedStringLength(Constants.HRIMaxLength.ResignDate, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ResignDate { get; set; }
        [Display(Name = "สถานะ")]
        [LocalizedStringLength(Constants.HRIMaxLength.Status, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Status { get; set; }
        [Display(Name = "คำอธิบายสถานะ")]
        [LocalizedStringLength(Constants.HRIMaxLength.EmpStatus, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string EmpStatus { get; set; }
        [Display(Name = "KK Email")]
        [LocalizedStringLength(Constants.HRIMaxLength.Email, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Email { get; set; }
        [Display(Name = "Lotus notes address")]
        [LocalizedStringLength(Constants.HRIMaxLength.NotesAddress, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string NotesAddress { get; set; }
        [Display(Name = "ที่ตั้ง(ชั้น)")]
        [LocalizedStringLength(Constants.HRIMaxLength.WorkArea, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string WorkArea { get; set; }
        [Display(Name = "ชื่อที่ตั้ง(ชั้น)")]
        [LocalizedStringLength(Constants.HRIMaxLength.WorkAreaDesc, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string WorkAreaDesc { get; set; }
        [Display(Name = "Cost Center")]
        [LocalizedStringLength(Constants.HRIMaxLength.CostCenter, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CostCenter { get; set; }
        [Display(Name = "ชื่อ Cost Center")]
        [LocalizedStringLength(Constants.HRIMaxLength.CostCenterDesc, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string CostCenterDesc { get; set; }
        [Display(Name = "เบอร์โทรศัพท์ภายใน")]
        [LocalizedStringLength(Constants.HRIMaxLength.TelExt, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TelExt { get; set; }
        [Display(Name = "รหัสผู้บังคับบัญชาเบื้องต้น")]
        [LocalizedStringLength(Constants.HRIMaxLength.Boss, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Boss { get; set; }
        [Display(Name = "ชื่อผู้บังคับบัญชาเบื้องต้น")]
        [LocalizedStringLength(Constants.HRIMaxLength.BossName, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string BossName { get; set; }
        [Display(Name = "รหัสผู้ประเมินลำดับ 1")]
        [LocalizedStringLength(Constants.HRIMaxLength.Assessor1, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Assessor1 { get; set; }
        [Display(Name = "ชื่อผู้ประเมินลำดับ 1")]
        [LocalizedStringLength(Constants.HRIMaxLength.Assessor1Name, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Assessor1Name { get; set; }
        [Display(Name = "รหัสผู้ประเมินลำดับ 2")]
        [LocalizedStringLength(Constants.HRIMaxLength.Assessor2, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Assessor2 { get; set; }
        [Display(Name = "ชื่อผู้ประเมินลำดับ 2")]
        [LocalizedStringLength(Constants.HRIMaxLength.Assessor2Name, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Assessor2Name { get; set; }
        [Display(Name = "รหัสประธานสาย")]
        [LocalizedStringLength(Constants.HRIMaxLength.Assessor3, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Assessor3 { get; set; }
        [Display(Name = "ชื่อประธานสาย")]
        [LocalizedStringLength(Constants.HRIMaxLength.Assessor3Name, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string Assessor3Name { get; set; }
        [Display(Name = "เบอร์ โทรศัพท์สายตรง")]
        [LocalizedStringLength(Constants.HRIMaxLength.TelNo, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string TelNo { get; set; }
        [Display(Name = "เบอร์โทรศัพท์มือถือ")]
        [LocalizedStringLength(Constants.HRIMaxLength.MobileNo, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string MobileNo { get; set; }
        [Display(Name = "aduser")]
        [LocalizedStringLength(Constants.HRIMaxLength.ADUser, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string ADUser { get; set; }
        [Display(Name = "รหัสกลุ่มพนักงาน")]
        [LocalizedStringLength(Constants.HRIMaxLength.OfficerId, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string OfficerId { get; set; }
        [Display(Name = "ชื่อกลุ่มพนักงาน")]
        [LocalizedStringLength(Constants.HRIMaxLength.OfficerDesc, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string OfficerDesc { get; set; }
        [Display(Name = "ประเภทตำแหน่งงาน")]
        [LocalizedStringLength(Constants.HRIMaxLength.AdditionJob, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string AdditionJob { get; set; }
        [Display(Name = "รหัสลักษณะงาน")]
        [LocalizedStringLength(Constants.HRIMaxLength.UnitBoss, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UnitBoss { get; set; }
        [Display(Name = "ชื่อลักษณะงาน")]
        [LocalizedStringLength(Constants.HRIMaxLength.UnitBossName, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string UnitBossName { get; set; }
        [Display(Name = "เลขที่ประจำตัวประชาชน")]
        [LocalizedStringLength(Constants.HRIMaxLength.IDNO, ErrorMessageResourceName = "ValErr_StringLength", ErrorMessageResourceType = typeof(CSM.Common.Resources.Resource))]
        public string IDNO { get; set; }

        public string Error { get; set; }
    }
}
