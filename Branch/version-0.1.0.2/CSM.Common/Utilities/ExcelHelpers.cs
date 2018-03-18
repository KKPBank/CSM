using System.Drawing;
using OfficeOpenXml;
using System;
using System.Data;
using System.IO;
using System.Linq;
using OfficeOpenXml.Style;

namespace CSM.Common.Utilities
{
    public static class ExcelHelpers
    {
        public static Byte[] WriteToExcelSR(string path, DateTime reportDate, DataSet ds)
        {
            Byte[] fileBytes = null;
            ExcelNamedRange namedRange = null;
            FileInfo newFile = new FileInfo(path);
            using(ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                //Check if worksheet with name "Export data" exists and retrieve that instance or null if it doesn't exist       
                ExcelWorksheet ws = xlPackage.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Export data");

                //If worksheet "Export data" was not found, add it
                if (ws == null)
                {
                    ws = xlPackage.Workbook.Worksheets.Add("Export data");
                }

                DataTable dt = ds.Tables[0];

                // lbl_ReportDate
                namedRange = xlPackage.Workbook.Names["lbl_ReportDate"];
                namedRange.Value = reportDate.FormatDateTime(Constants.DateTimeFormat.ReportDateTime);

                int nCol = 38;
                int startRow = 6;
                int iRow = startRow;

                if (dt.Rows.Count > 0)
                {
                    for (int i = 1; i <= dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i - 1];

                        ws.Cells[iRow, 2].Value = i.ConvertToString();
                        ws.Cells[iRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws.Cells[iRow, 3].Value = row["TotalSla"];
                        ws.Cells[iRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        ws.Cells[iRow, 4].Value = row["CurrentAlert"];
                        ws.Cells[iRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        ws.Cells[iRow, 5].Value = row["CustomerFistname"];
                        ws.Cells[iRow, 6].Value = row["CustomerLastname"];
                        ws.Cells[iRow, 7].Value = row["CardNo"];
                        ws.Cells[iRow, 8].Value = row["AccountNo"];
                        ws.Cells[iRow, 9].Value = row["CarRegisNo"];
                        ws.Cells[iRow, 10].Value = row["SRNo"];
                        ws.Cells[iRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[iRow, 11].Value = row["CreatorBranch"];
                        ws.Cells[iRow, 12].Value = row["ChannelName"];
                        ws.Cells[iRow, 13].Value = row["CallId"];
                        ws.Cells[iRow, 14].Value = row["ANo"];
                        ws.Cells[iRow, 15].Value = row["ProductGroupName"];
                        ws.Cells[iRow, 16].Value = row["ProductName"];
                        ws.Cells[iRow, 17].Value = row["CampaignServiceName"];
                        ws.Cells[iRow, 18].Value = row["TypeName"];
                        ws.Cells[iRow, 19].Value = row["AreaName"];
                        ws.Cells[iRow, 20].Value = row["SubAreaName"];
                        ws.Cells[iRow, 21].Value = row["SRStatusName"];
                        ws.Cells[iRow, 21].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[iRow, 22].Value = row["CloseDateDisplay"];
                        ws.Cells[iRow, 22].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[iRow, 23].Value = row["SRIsverifyPassDisplay"];
                        ws.Cells[iRow, 23].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws.Cells[iRow, 24].Value = row["CreatorName"];
                        ws.Cells[iRow, 25].Value = row["CreateDateDisplay"];
                        ws.Cells[iRow, 26].Value = row["OwnerName"];
                        ws.Cells[iRow, 27].Value = row["UpdateDateOwnerDisplay"];
                        ws.Cells[iRow, 27].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[iRow, 28].Value = row["DelegatorName"];                      
                        ws.Cells[iRow, 29].Value = row["UpdateDelegateDisplay"];
                        ws.Cells[iRow, 29].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[iRow, 30].Value = row["SRSubject"];
                        ws.Cells[iRow, 31].Value = row["SRRemark"];
                        ws.Cells[iRow, 32].Value = row["ContactName"];
                        ws.Cells[iRow, 33].Value = row["ContactSurname"];
                        ws.Cells[iRow, 34].Value = row["Relationship"];
                        ws.Cells[iRow, 35].Value = row["ContactNo"];
                        ws.Cells[iRow, 36].Value = row["MediaSourceName"];

                        ws.Cells[iRow, 37].Value = row["AttachFile"];
                        ws.Cells[iRow, 37].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws.Cells[iRow, 38].Value = row["JobType"];
                        ws.Cells[iRow, 38].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        iRow++;
                    }

                    int nRow = iRow - 1;
                    SetCellStyle(ws.Cells[startRow, 2, nRow, nCol]);
                    ws.Cells[nRow, 2, nRow, nCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                //Read the Excel file in a byte array
                fileBytes = xlPackage.GetAsByteArray();
            }

            return fileBytes;
        }
        public static Byte[] WriteToExcelJobs(string path, DateTime reportDate, DataSet ds)
        {
            Byte[] fileBytes = null;
            ExcelNamedRange namedRange = null;
            FileInfo newFile = new FileInfo(path);

            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                //Check if worksheet with name "Export data" exists and retrieve that instance or null if it doesn't exist       
                ExcelWorksheet ws = xlPackage.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Export data");

                //If worksheet "Export data" was not found, add it
                if (ws == null)
                {
                    ws = xlPackage.Workbook.Worksheets.Add("Export data");
                }

                DataTable dt = ds.Tables[0];

                // lbl_ReportDate
                namedRange = xlPackage.Workbook.Names["lbl_ReportDate"];
                namedRange.Value = reportDate.FormatDateTime(Constants.DateTimeFormat.ReportDateTime);

                int nCol = 16;
                int startRow = 6;
                int iRow = startRow;

                if (dt.Rows.Count > 0)
                {

                    for (int i = 1; i <= dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i - 1];

                        ws.Cells[iRow, 2].Value = i.ConvertToString();
                        ws.Cells[iRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws.Cells[iRow, 3].Value = row["FirstName"];
                        ws.Cells[iRow, 4].Value = row["LastName"];
                        ws.Cells[iRow, 5].Value = row["JobType"];
                        ws.Cells[iRow, 6].Value = row["JobStatus"];
                        ws.Cells[iRow, 7].Value = row["JobDateDisplay"];
                        ws.Cells[iRow, 8].Value = row["From"];
                        ws.Cells[iRow, 9].Value = row["Subject"];
                        ws.Cells[iRow, 10].Value = row["SRID"];
                        ws.Cells[iRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws.Cells[iRow, 11].Value = row["SRCreator"];
                        ws.Cells[iRow, 12].Value = row["SROwner"];
                        ws.Cells[iRow, 13].Value = row["SRStatus"];
                        ws.Cells[iRow, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[iRow, 14].Value = row["AttachFile"];
                        ws.Cells[iRow, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[iRow, 15].Value = row["Remark"];
                        ws.Cells[iRow, 16].Value = row["PoolName"];

                        iRow++;
                    }

                    int nRow = iRow - 1;
                    SetCellStyle(ws.Cells[startRow, 2, nRow, nCol]);
                    ws.Cells[nRow, 2, nRow, nCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }              

                //Read the Excel file in a byte array
                fileBytes = xlPackage.GetAsByteArray();
            }

            return fileBytes;
        }
        public static Byte[] WriteToExcelVerify(string path, DateTime reportDate, DataSet ds)
        {
            Byte[] fileBytes = null;
            ExcelNamedRange namedRange = null;
            FileInfo newFile = new FileInfo(path);

            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                //Check if worksheet with name "Export data" exists and retrieve that instance or null if it doesn't exist       
                ExcelWorksheet ws = xlPackage.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Export data");

                //If worksheet "Export data" was not found, add it
                if (ws == null)
                {
                    ws = xlPackage.Workbook.Worksheets.Add("Export data");
                }

                DataTable dt = ds.Tables[0];

                // lbl_ReportDate
                namedRange = xlPackage.Workbook.Names["lbl_ReportDate"];
                namedRange.Value = reportDate.FormatDateTime(Constants.DateTimeFormat.ReportDateTime);

                int nCol = 23;
                int startRow = 6;
                int iRow = startRow;

                if (dt.Rows.Count > 0)
                {
                    for (int i = 1; i <= dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i - 1];

                        ws.Cells[iRow, 2].Value = i.ConvertToString();
                        ws.Cells[iRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws.Cells[iRow, 3].Value = row["SRId"];
                        ws.Cells[iRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws.Cells[iRow, 4].Value = row["AccountNo"];
                        ws.Cells[iRow, 5].Value = row["CustomerFistname"];
                        ws.Cells[iRow, 6].Value = row["CustomerLastname"];
                        ws.Cells[iRow, 7].Value = row["SROwnerName"];
                        ws.Cells[iRow, 8].Value = row["SRCreateDateDisplay"];
                        ws.Cells[iRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws.Cells[iRow, 9].Value = row["SRCreatorBranch"];
                        ws.Cells[iRow, 10].Value = row["ProductGroupName"];
                        ws.Cells[iRow, 11].Value = row["ProductName"];
                        ws.Cells[iRow, 12].Value = row["CampaignServiceName"];
                        ws.Cells[iRow, 13].Value = row["TypeName"];
                        ws.Cells[iRow, 14].Value = row["AreaName"];
                        ws.Cells[iRow, 15].Value = row["SubAreaName"];
                        ws.Cells[iRow, 16].Value = row["SRSubject"];
                        ws.Cells[iRow, 17].Value = row["SRDescription"];

                        ws.Cells[iRow, 18].Value = row["SRStatus"];
                        ws.Cells[iRow, 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws.Cells[iRow, 19].Value = row["IsVerifyResultDisplay"];
                        ws.Cells[iRow, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws.Cells[iRow, 20].Value = row["TotalQuestion"];
                        ws.Cells[iRow, 20].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        ws.Cells[iRow, 21].Value = row["TotalPass"];
                        ws.Cells[iRow, 21].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        ws.Cells[iRow, 22].Value = row["TotalFailed"];
                        ws.Cells[iRow, 22].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        ws.Cells[iRow, 23].Value = row["TotalDisregard"];
                        ws.Cells[iRow, 23].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        iRow++;
                    }

                    int nRow = iRow - 1;
                    SetCellStyle(ws.Cells[startRow, 2, nRow, nCol]);
                    ws.Cells[nRow, 2, nRow, nCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                //Read the Excel file in a byte array
                fileBytes = xlPackage.GetAsByteArray();
            }

            return fileBytes;
        }
        public static Byte[] WriteToExcelVerifyDetail(string path, DateTime reportDate, DataSet ds)
        {
            Byte[] fileBytes = null;
            ExcelNamedRange namedRange = null;
            FileInfo newFile = new FileInfo(path);

            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                //Check if worksheet with name "Export data" exists and retrieve that instance or null if it doesn't exist       
                ExcelWorksheet ws = xlPackage.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Export data");

                //If worksheet "Export data" was not found, add it
                if (ws == null)
                {
                    ws = xlPackage.Workbook.Worksheets.Add("Export data");
                }

                DataTable dt = ds.Tables[0];

                // lbl_ReportDate
                namedRange = xlPackage.Workbook.Names["lbl_ReportDate"];
                namedRange.Value = reportDate.FormatDateTime(Constants.DateTimeFormat.ReportDateTime);

                int nCol = 17;
                int startRow = 6;
                int iRow = startRow;

                if (dt.Rows.Count > 0)
                {
                    for (int i = 1; i <= dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i - 1];

                        ws.Cells[iRow, 2].Value = i.ConvertToString();
                        ws.Cells[iRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws.Cells[iRow, 3].Value = row["SRId"];
                        ws.Cells[iRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws.Cells[iRow, 4].Value = row["AccountNo"];
                        ws.Cells[iRow, 5].Value = row["CustomerFistname"];
                        ws.Cells[iRow, 6].Value = row["CustomerLastname"];
                        ws.Cells[iRow, 7].Value = row["ProductGroupName"];
                        ws.Cells[iRow, 8].Value = row["ProductName"];
                        ws.Cells[iRow, 9].Value = row["CampaignServiceName"];
                        ws.Cells[iRow, 10].Value = row["TypeName"];
                        ws.Cells[iRow, 11].Value = row["AreaName"];
                        ws.Cells[iRow, 12].Value = row["SubAreaName"];
                        ws.Cells[iRow, 13].Value = row["SROwnerName"];
                        ws.Cells[iRow, 14].Value = row["IsVerifyPassDisplay"];
                        ws.Cells[iRow, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws.Cells[iRow, 15].Value = row["QuestionGroupName"];
                        ws.Cells[iRow, 16].Value = row["QuestionName"];
  
                        ws.Cells[iRow, 17].Value = row["VerifyResultDisplay"];
                        ws.Cells[iRow, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        iRow++;
                    }

                    int nRow = iRow - 1;
                    SetCellStyle(ws.Cells[startRow, 2, nRow, nCol]);
                    ws.Cells[nRow, 2, nRow, nCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                //Read the Excel file in a byte array
                fileBytes = xlPackage.GetAsByteArray();
            }

            return fileBytes;
        }

        public static Byte[] WriteToExcelNcb(string path, DateTime reportDate, DataSet ds)
        {
            Byte[] fileBytes = null;
            ExcelNamedRange namedRange = null;
            FileInfo newFile = new FileInfo(path);

            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                //Check if worksheet with name "Export data" exists and retrieve that instance or null if it doesn't exist       
                ExcelWorksheet ws = xlPackage.Workbook.Worksheets.FirstOrDefault(x => x.Name == "Export data");

                //If worksheet "Export data" was not found, add it
                if (ws == null)
                {
                    ws = xlPackage.Workbook.Worksheets.Add("Export data");
                }

                DataTable dt = ds.Tables[0];

                // lbl_ReportDate
                namedRange = xlPackage.Workbook.Names["lbl_ReportDate"];
                namedRange.Value = reportDate.FormatDateTime(Constants.DateTimeFormat.ReportDateTime);

                int nCol = 26;
                int startRow = 6;
                int iRow = startRow;

                if (dt.Rows.Count > 0)
                {
                    for (int i = 1; i <= dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i - 1];

                        ws.Cells[iRow, 2].Value = i.ConvertToString();
                        ws.Cells[iRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws.Cells[iRow, 3].Value = row["Sla"];
                        ws.Cells[iRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws.Cells[iRow, 4].Value = row["CustomerFistname"];
                        ws.Cells[iRow, 5].Value = row["CustomerLastname"];
                        ws.Cells[iRow, 6].Value = row["CardNo"];
                        ws.Cells[iRow, 7].Value = row["CustomerBirthDateDisplay"];
                        ws.Cells[iRow, 8].Value = row["NcbCheckStatus"];
                        ws.Cells[iRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[iRow, 9].Value = row["SRId"];
                        ws.Cells[iRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[iRow, 10].Value = row["SRStatus"];
                        ws.Cells[iRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws.Cells[iRow, 11].Value = row["ProductGroupName"];
                        ws.Cells[iRow, 12].Value = row["ProductName"];
                        ws.Cells[iRow, 13].Value = row["CampaignName"];
                        ws.Cells[iRow, 14].Value = row["TypeName"];
                        ws.Cells[iRow, 15].Value = row["AreaName"];
                        ws.Cells[iRow, 16].Value = row["SubAreaName"];
                        ws.Cells[iRow, 17].Value = row["SRCreator"];
                        ws.Cells[iRow, 18].Value = row["SRCreateDateDisplay"];
                        ws.Cells[iRow, 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ws.Cells[iRow, 19].Value = row["SROwner"];
                        ws.Cells[iRow, 20].Value = row["OwnerUpdateDisplay"];
                        ws.Cells[iRow, 20].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[iRow, 21].Value = row["SRDelegate"];
                        ws.Cells[iRow, 22].Value = row["SRDelegateUpdateDisplay"];
                        ws.Cells[iRow, 23].Value = row["MKTUpperBranch1"];
                        ws.Cells[iRow, 24].Value = row["MKTUpperBranch2"];
                        ws.Cells[iRow, 25].Value = row["MKTEmployeeBranch"];
                        ws.Cells[iRow, 26].Value = row["MKTEmployeeName"];

                        iRow++;
                    }

                    int nRow = iRow - 1;
                    SetCellStyle(ws.Cells[startRow, 2, nRow, nCol]);
                    ws.Cells[nRow, 2, nRow, nCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                //Read the Excel file in a byte array
                fileBytes = xlPackage.GetAsByteArray();
            }

            return fileBytes;
        }
        private static void SetCellStyle(ExcelRange cell)
        {
            // Assign borders
            cell.Style.Border.Top.Style = ExcelBorderStyle.None;
            cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            cell.Style.Border.Bottom.Style = ExcelBorderStyle.Dotted;

            cell.Style.Border.Left.Color.SetColor(Color.CornflowerBlue);
            cell.Style.Border.Right.Color.SetColor(Color.CornflowerBlue);
            cell.Style.Border.Bottom.Color.SetColor(Color.CornflowerBlue);
        }
    }
}
