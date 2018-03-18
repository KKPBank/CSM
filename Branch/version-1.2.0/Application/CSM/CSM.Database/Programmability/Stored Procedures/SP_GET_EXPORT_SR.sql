-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SP_GET_EXPORT_SR]
	-- Add the parameters for the stored procedure here
	@SR_ID varchar(50),
	@productGroupId int,
	@productId int,
	@campaignId int,
	@typeId int,
	@areaId int,
	@subareaId int,
	@slaId int,
	@srstatusId int,
	@channelId int,
	@ownerId int,
	@ownerBranchId int,
	@creatorId int,
	@creatorBranchId int,
	@srMinDate datetime,
	@srMaxDate datetime,
	@firstName varchar(255),
	@lastName varchar(255),
	@cardNo varchar(50),
	@subject varchar(255),
	@description varchar(255),
	@accountNo varchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	--CASE WHEN ISNULL(TB_M_ACCOUNT.STATUS,'') = 'A' THEN 'Active' 
	--	ELSE 'Inactive' END AS ACCOUNT_STATUS,	
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
	TypeName = ty.TYPE_NAME,
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
	ContactName = ct.FIRST_NAME_TH,
	ContactSurname = ct.LAST_NAME_TH,
	Relationship = rp.RELATIONSHIP_NAME,
	MediaSourceName = me.MEDIA_SOURCE_NAME,
	AttachFile = CASE WHEN sm.SR_ATTACHMENT_ID IS NULL THEN 'No' ELSE 'Yes' END,
	JobType = jc.CHANNEL_NAME,
	ContactNo = co.PHONE_NO
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
				       LEFT OUTER JOIN TB_T_JOB AS jb ON jb.SR_ID = sr.SR_ID
				       LEFT OUTER JOIN TB_R_CHANNEL AS jc ON jc.CHANNEL_ID = jb.CHANNEL_ID
				       LEFT OUTER JOIN TB_R_BRANCH AS cb ON cb.BRANCH_ID = sr.CREATE_BRANCH_ID
				       LEFT OUTER JOIN TB_R_BRANCH AS ob ON ob.BRANCH_ID = sr.OWNER_BRANCH_ID
				       LEFT OUTER JOIN TB_M_RELATIONSHIP AS rp ON rp.RELATIONSHIP_ID = sr.CONTACT_RELATIONSHIP_ID
					   LEFT OUTER JOIN TB_M_CONTACT_PHONE AS co ON co.CONTACT_ID = ct.CONTACT_ID
					   LEFT OUTER JOIN TB_M_PHONE_TYPE AS pt ON pt.PHONE_TYPE_ID = co.PHONE_TYPE_ID
					   LEFT OUTER JOIN TB_T_SR_ACTIVITY AS sa ON sa.SR_ID = sr.SR_ID
					   LEFT OUTER JOIN TB_T_SR_ATTACHMENT AS sm ON sm.SR_ACTIVITY_ID = sa.SR_ACTIVITY_ID
WHERE pt.PHONE_TYPE_CODE = '02' AND (sr.SR_PAGE_ID in (null,1,2) OR cu.ROLE_ID IN (3,4,5,6))
                          AND (@ownerId IS NULL OR sr.OWNER_USER_ID = @ownerId)
                           AND (@ownerBranchId IS NULL OR sr.OWNER_BRANCH_ID = @ownerBranchId)
                           AND (@creatorId IS NULL OR sr.CREATE_USER = @creatorId)
                           AND (@creatorBranchId IS NULL OR sr.CREATE_BRANCH_ID = @creatorBranchId)
                           AND (@firstName IS NULL OR cs.FIRST_NAME_TH LIKE '%'+@firstName+'%' OR cs.FIRST_NAME_EN LIKE '%'+@firstName+'%')
						   AND (@lastName IS NULL OR cs.LAST_NAME_TH LIKE '%'+@lastName+'%' OR cs.LAST_NAME_EN LIKE '%'+@lastName+'%')
						   AND (@cardNo IS NULL OR cs.CARD_NO LIKE '%'+@cardNo+'%' OR cs.CARD_NO LIKE '%'+@cardNo+'%')
							AND (@productGroupId IS NULL OR pg.PRODUCTGROUP_ID = @productGroupId)
							AND (@productId IS NULL OR pr.PRODUCT_ID =@productId)
							AND (@campaignId IS NULL OR cp.CAMPAIGNSERVICE_ID =@campaignId)
							AND (@typeId IS NULL OR ty.TYPE_ID =@typeId)
							AND (@areaId IS NULL OR ar.AREA_ID =@areaId)
							AND (@subareaId IS NULL OR sb.SUBAREA_ID =@subareaId)
--                         AND (slaId == null OR slaId == Constants.ApplicationStatus.All
--                                    OR (slaId == 1 AND (sr.RULE_THIS_ALERT == null OR sr.RULE_THIS_ALERT == 0))
--                                    OR (slaId == 2 AND (sr.RULE_THIS_ALERT.HasValue AND sr.RULE_THIS_ALERT > 0))
--                               )
                         AND (@srstatusId IS NULL OR @srstatusId=-1 OR st.SR_STATUS_ID = @srstatusId)
                         AND (@channelId IS NULL OR @channelId=-1 OR ch.CHANNEL_ID = @channelId)
						 --AND (sr.CREATE_DATE BETWEEN @srMinDate AND @srMaxDate)
                         --AND sr.CREATE_DATE >= @srMinDate
                         --AND sr.CREATE_DATE <= @srMaxDate
                         AND (@subject IS NULL OR sr.SR_SUBJECT LIKE '%'+@subject+'%')
                         AND (@description IS NULL OR sr.SR_REMARK LIKE '%'+@description+'%')
                         AND (@SR_ID IS NULL OR sr.SR_NO LIKE '%'+@SR_ID+'%')
                         AND (@accountNo IS NULL OR ac.ACCOUNT_NO = @accountNo)
END

