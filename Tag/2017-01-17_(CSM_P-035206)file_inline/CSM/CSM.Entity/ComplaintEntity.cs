namespace CSM.Entity
{
    public class ComplaintBUGroupEntity
    {
        public int? ComplaintBUGroupId { get; set; }
        public string ComplaintBUGroupName { get; set; }
        public short? ComplaintBUGroupStatus { get; set; }
    }

    public class ComplaintCauseSummaryEntity
    {
        public int? ComplaintCauseSummaryId { get; set; }
        public string ComplaintCauseSummaryName { get; set; }
        public short? ComplaintCauseSummaryStatus { get; set; }
    }

    public class ComplaintIssuesEntity
    {
        public int? ComplaintIssuesId { get; set; }
        public string ComplaintIssuesName { get; set; }
        public short? ComplaintIssuesStatus { get; set; }
    }

    public class ComplaintMappingEntity
    {
        public int? ComplaintMappingId { get; set; }
        public int? ComplaintSubjectId { get; set; }
        public int? ComplaintTypetId { get; set; }
        public int? RootCauseId { get; set; }
        public short? ComplaintMappingStatus { get; set; }
    }

    public class RootCauseEntity
    {
        public int? RootCauseId { get; set; }
        public string RootCauseName { get; set; }
        public short? RootCauseStatus { get; set; }
    }

    public class ComplaintSubjectEntity
    {
        public int? ComplaintSubjectId { get; set; }
        public string ComplaintSubjectName { get; set; }
        public short? ComplaintSubjectStatus { get; set; }
    }

    public class ComplaintSummaryEntity
    {
        public int? ComplaintSummaryId { get; set; }
        public string ComplaintSummaryName { get; set; }
        public short? ComplaintSummaryStatus { get; set; }
    }

    public class ComplaintTypeEntity
    {
        public int? ComplaintTypeId { get; set; }
        public string ComplaintTypeName { get; set; }
        public short? ComplaintTypeStatus { get; set; }
    }

    public class ComplaintHRBUEntity
    {
        public string BU1 { get; set; }
        public string BU1Desc { get; set; }
        public string BU2 { get; set; }
        public string BU2Desc { get; set; }
        public string BU3 { get; set; }
        public string BU3Desc { get; set; }
        public bool? BU_Status { get; set; }
    }

    public class ComplaintBUEntity
    {
        public string BU_Code { get; set; }
        public string BU_Desc { get; set; }
        public bool? BU_Status { get; set; }
    }

    public class MSHBranchEntity
    {
        public int? Branch_Id { get; set; }
        public string Comp_Code { get; set; }
        public string Branch_Code { get; set; }
        public string Branch_Name { get; set; }
        public string Status { get; set; }
    }
}
