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
INSERT INTO  TB_R_SR_REPORT(TOTAL_SLA,
	CURRENT_ALERT,
	FIRST_NAME_TH,
	LAST_NAME_TH,
	CARD_NO,
	ACCOUNT_NO,
	CAR_REGIS_NO,
	SR_NO,
	CREATOR_BRANCH,
	CHANNEL_NAME,
	CALL_ID,
	ANO,
	PRODUCTGROUP_NAME,
	PRODUCT_NAME,
	CAMPAIGNSERVICE_NAME,
	[TYPE_NAME],
	AREA_NAME,
	SUBAREA_NAME,
	SR_STATUS_NAME,
	CLOSE_DATE,
	SR_IS_VERIFY_PASS,
	CREATOR_NAME,
	CREATE_DATE,
	OWNER_NAME,
	UPDATE_DATE_BY_OWNER,
	DELEGATOR_NAME,
	UPDATE_DATE_BY_DELEGATE,
	SR_SUBJECT,
	SR_REMARK,
	CONTACT_NAME,
	CONTACT_SURNAME,
	RELATIONSHIP,
	MEDIA_SOURCE_NAME,
	--ATTACH_FILE,
	--JOB_TYPE,
	CONTACT_NO
	)
SELECT 
	TotalSla = sr.RULE_TOTAL_ALERT,
	CurrentAlert = sr.RULE_THIS_ALERT,
	CustomerFirstname = CASE WHEN cs.FIRST_NAME_TH IS NULL THEN cs.FIRST_NAME_EN ELSE cs.FIRST_NAME_TH END,
	CustomerLastname = CASE WHEN cs.LAST_NAME_TH IS NULL THEN cs.LAST_NAME_EN ELSE cs.LAST_NAME_TH END,
	CardNo = cs.CARD_NO,
	AccountNo = ac.ACCOUNT_NO,
	CarRegisNo = ac.CAR_NO,
	SRNo = sr.SR_NO,
	CreatorBranch = cb.BRANCH_NAME,
	ChannelName = ch.CHANNEL_NAME,
	CallId =sr.SR_CALL_ID,
	ANo = sr.SR_ANO,
	ProductGroupName = pg.PRODUCTGROUP_NAME,
	ProductName = pr.PRODUCT_NAME,
	CampaignServiceName = cp.CAMPAIGNSERVICE_NAME,
	TypeName = ty.[TYPE_NAME],
	AreaName = ar.AREA_NAME,
	SubAreaName = sb.SUBAREA_NAME,
	SRStatusName = st.SR_STATUS_NAME,
	CloseDate = sr.CLOSE_DATE,
	SRIsverifyPass = sr.SR_IS_VERIFY_PASS,
	CreatorName = cu.POSITION_CODE + '-' + cu.FIRST_NAME + ' ' + cu.LAST_NAME,
	CreateDate = sr.CREATE_DATE,
	OwnerName = ou.POSITION_CODE + '-' + ou.FIRST_NAME + ' ' + ou.LAST_NAME,
	UpdateDateOwner = sr.UPDATE_DATE_BY_OWNER,
	DelegatorName = du.POSITION_CODE + '-' + du.FIRST_NAME + ' ' + du.LAST_NAME,
	UpdateDelegate = sr.UPDATE_DATE_BY_DELEGATE,
	SRSubject = sr.SR_SUBJECT,
	SRRemark = sr.SR_REMARK,
	CONTACT_NAME = ct.FIRST_NAME_TH,
	CONTACT_SURNAME = ct.LAST_NAME_TH,
	Relationship = rp.RELATIONSHIP_NAME,
	MediaSourceName = me.MEDIA_SOURCE_NAME,
	--AttachFile = CASE WHEN sm.SR_ATTACHMENT_ID IS NULL THEN 'No' ELSE 'Yes' END,
	--JobType = jc.CHANNEL_NAME,
	--CONTACT_NO = co.PHONE_NO
	CONTACT_NO = co.PHONE_NO
FROM TB_T_SR AS sr LEFT OUTER JOIN TB_M_CUSTOMER AS cs ON cs.CUSTOMER_ID = sr.CUSTOMER_ID
				   LEFT OUTER JOIN TB_M_ACCOUNT AS ac ON ac.ACCOUNT_ID = sr.ACCOUNT_ID
				   LEFT OUTER JOIN TB_R_CHANNEL AS ch ON ch.CHANNEL_ID = sr.CHANNEL_ID
				       LEFT OUTER JOIN TB_R_PRODUCTGROUP AS pg ON pg.PRODUCTGROUP_ID = sr.PRODUCTGROUP_ID
				       LEFT OUTER JOIN TB_R_PRODUCT AS pr ON pr.PRODUCT_ID = sr.PRODUCT_ID
				       LEFT OUTER JOIN TB_R_CAMPAIGNSERVICE AS cp ON cp.CAMPAIGNSERVICE_ID = sr.CAMPAIGNSERVICE_ID
				       LEFT OUTER JOIN TB_M_TYPE AS ty ON ty.TYPE_ID = sr.TYPE_ID
				       LEFT OUTER JOIN TB_M_AREA AS ar ON ar.AREA_ID = sr.AREA_ID
				       LEFT OUTER JOIN TB_M_SUBAREA AS sb ON sb.SUBAREA_ID = sr.SUBAREA_ID
				       LEFT OUTER JOIN TB_C_SR_STATUS AS st ON st.SR_STATUS_ID = sr.SR_STATUS_ID
				       LEFT OUTER JOIN TB_R_USER AS cu ON cu.USER_ID = sr.CREATE_USER
				       LEFT OUTER JOIN TB_R_USER AS ou ON ou.USER_ID = sr.OWNER_USER_ID
				       LEFT OUTER JOIN TB_R_USER AS du ON du.USER_ID = sr.DELEGATE_USER_ID
				       LEFT OUTER JOIN TB_M_CONTACT AS ct ON ct.CONTACT_ID = sr.CONTACT_ID
				       LEFT OUTER JOIN TB_M_MEDIA_SOURCE AS me ON me.MEDIA_SOURCE_ID = sr.MEDIA_SOURCE_ID
--				       LEFT OUTER JOIN TB_T_JOB AS jb ON jb.SR_ID = sr.SR_ID
--				       LEFT OUTER JOIN TB_R_CHANNEL AS jc ON jc.CHANNEL_ID = jb.CHANNEL_ID
				       LEFT OUTER JOIN TB_R_BRANCH AS cb ON cb.BRANCH_ID = sr.CREATE_BRANCH_ID
--				       LEFT OUTER JOIN TB_R_BRANCH AS ob ON ob.BRANCH_ID = sr.OWNER_BRANCH_ID
				       LEFT OUTER JOIN TB_M_RELATIONSHIP AS rp ON rp.RELATIONSHIP_ID = sr.CONTACT_RELATIONSHIP_ID
					   LEFT OUTER JOIN TB_M_CONTACT_PHONE AS co ON co.CONTACT_ID = ct.CONTACT_ID
--					   LEFT OUTER JOIN TB_M_CONTACT_PHONE AS co ON co.CONTACT_ID = ct.CONTACT_ID
--					   LEFT OUTER JOIN TB_M_PHONE_TYPE AS pt ON pt.PHONE_TYPE_ID = co.PHONE_TYPE_ID
--					   LEFT OUTER JOIN TB_T_SR_ACTIVITY AS sa ON sa.SR_ID = sr.SR_ID
--					   LEFT OUTER JOIN TB_T_SR_ATTACHMENT AS sm ON sm.SR_ACTIVITY_ID = sa.SR_ACTIVITY_ID