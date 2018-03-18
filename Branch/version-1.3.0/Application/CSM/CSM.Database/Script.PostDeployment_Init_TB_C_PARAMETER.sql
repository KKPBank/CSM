/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
INSERT INTO TB_C_PARAMETER ([PARAMETER_NAME]
           ,[PARAMETER_VALUE]
           ,[PARAMETER_DESC]
           ,[UPDATE_USER]
           ,[UPDATE_DATE]
           ,[PARAMTER_TYPE])
VALUES ('NUM_DAYS_SR_REPORT','-90','',1,GETDATE(),null)

INSERT INTO TB_C_PARAMETER ([PARAMETER_NAME]
           ,[PARAMETER_VALUE]
           ,[PARAMETER_DESC]
           ,[UPDATE_USER]
           ,[UPDATE_DATE]
           ,[PARAMTER_TYPE])
VALUES ('TRIGGER_DAYS','1,16','',1,GETDATE(),null)

INSERT INTO TB_C_PARAMETER ([PARAMETER_NAME]
           ,[PARAMETER_VALUE]
           ,[PARAMETER_DESC]
           ,[UPDATE_USER]
           ,[UPDATE_DATE]
           ,[PARAMTER_TYPE])
VALUES ('REPORT_PATH','E:\CsmPath\Report','',1,GETDATE(),null)

