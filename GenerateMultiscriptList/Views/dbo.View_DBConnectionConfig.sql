/** ==================================================================================================================
Description	:	List of databases across all environments
================================================================================================================== **/

CREATE VIEW [dbo].[View_DBConnectionConfig]

AS

SELECT
	[IsEnabled]				-- true if the application can access the database
	,[NewAccounts]			-- true if new accounts can be created in the database
	,[ConfigLevel]			-- 0 = Standard, 1 = Enterprise, 2 = Automation
	,[EnvironmentCode]
	,[ControlPlane]			-- US, EU, AP, GL (Global)
	,[DeploymentSubGroup]
	,[PortNumber]
	,[DatabaseType]
	,[DatabaseName]
	,[CNamePrefix]
	,[Node1Prefix]
	,[Node2Prefix]
	,[Node3Prefix]
	,[InstanceSuffix]
	,[CNameSuffix]
	,[IsSingleTenantDB]		-- true if the database only hosts a single tenant
	,[SingleTenantName]		-- a description of the single tenant
	,[SQLDialect]			-- to denote the sql language varient (TSQL,MySQL,etc)
FROM (VALUES

	--------------------------------------------------------------------------------------------------
	-- Development
	--------------------------------------------------------------------------------------------------
	--E,NA,CL, Env, CtlP	, DeployGrp	, PortNumber, DatabaseType	, DatabaseName	, CNamePrefix		, Node1Prefix		, Node2Prefix	, Node3Prefix	, InstanceSuffix, CNameSuffix	,ST, SingleTenantName	, SQLDialect
	 (1, 1, 0, 'D', 'GL'	, 'Dev'		, '#'		, 'DB1'			, 'DB1'			, 'DB1-dev'			, 'DB1'				, NULL			, NULL			, 'dev.x.com'	, 'dev.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'D', 'GL'	, 'Dev'		, '#'		, 'DB2'			, 'DB2'			, 'DB2-dev'			, 'DB2'				, NULL			, NULL			, 'dev.x.com'	, 'dev.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'D', 'US'	, 'Dev'		, '#'		, 'DB3'			, 'DB3'			, 'DB3-us-dev'		, 'usDB3dev'		, NULL			, NULL			, 'dev.x.com'	, 'dev.x.com'	, 0, NULL				, 'TSQL')

	--------------------------------------------------------------------------------------------------
	-- Test
	--------------------------------------------------------------------------------------------------
	--E,NA,CL, Env, CtlP	, DeployGrp		, PortNumber, DatabaseType	, DatabaseName	, CNamePrefix		, Node1Prefix		, Node2Prefix	, Node3Prefix	, InstanceSuffix, CNameSuffix	,ST, SingleTenantName	, SQLDialect
	,(1, 0, 0, 'T', 'GL'	, 'Test'		, '#'		, 'DB1'			, 'DB1'			, 'DB1-test'			, 'DB1'			, NULL			, NULL			, 'test.x.com'	, 'test.x.com'	, 0, NULL				, 'TSQL')
	,(1, 1, 0, 'T', 'GL'	, 'Test'		, '#'		, 'DB2'			, 'DB2'			, 'DB2-test'			, 'DB2'			, NULL			, NULL			, 'test.x.com'	, 'test.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'T', 'US'	, 'Test'		, '#'		, 'DB3'			, 'DB3'			, 'DB3-us-test'			, 'usDB3test'	, NULL			, NULL			, 'test.x.com'	, 'test.x.com'	, 0, NULL				, 'TSQL')

	--------------------------------------------------------------------------------------------------
	-- Stage
	--------------------------------------------------------------------------------------------------
	--E,NA,CL, Env, CtlP	, DeployGrp		, PortNumber, DatabaseType	, DatabaseName	, CNamePrefix		, Node1Prefix		, Node2Prefix	, Node3Prefix	, InstanceSuffix, CNameSuffix	,ST, SingleTenantName	, SQLDialect
	,(1, 0, 0, 'S', 'GL'	, 'Stage'		, '#'		, 'DB1'			, 'DB1'			, 'DB1-stage'		, 'DB1'				, NULL			, NULL			, 'stage.x.com'	, 'stage.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'S', 'GL'	, 'Stage'		, '#'		, 'DB2'			, 'DB2'			, 'DB2-stage'		, 'DB2'				, NULL			, NULL			, 'stage.x.com'	, 'stage.x.com'	, 0, NULL				, 'TSQL')
	,(1, 1, 0, 'S', 'US'	, 'Stage'		, '#'		, 'DB3'			, 'DB3'		, 'DB3-us-stage'		, 'usDB3stage'		, NULL			, NULL			, 'stage.x.com'	, 'stage.x.com'	, 0, NULL				, 'TSQL')

	--------------------------------------------------------------------------------------------------
	-- Production
	--------------------------------------------------------------------------------------------------
	--E,NA,CL, Env, CtlP	, DeployGrp		, PortNumber, DatabaseType	, DatabaseName	, CNamePrefix		, Node1Prefix		, Node2Prefix	, Node3Prefix	, InstanceSuffix, CNameSuffix	,ST, SingleTenantName	, SQLDialect
	,(1, 1, 0, 'P', 'GL'	, 'Prod'		, '#'		, 'DB1'			, 'DB1'			, 'DB1-prod'		, 'DB1'				, NULL			, NULL			, 'prod.x.com'	, 'prod.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'P', 'GL'	, 'Prod'		, '#'		, 'DB2'			, 'DB2'			, 'DB2-prod'		, 'DB2'				, NULL			, NULL			, 'prod.x.com'	, 'prod.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'P', 'US'	, 'Prod'		, '#'		, 'DB3'			, 'DB3'			, 'DB3-us-prod'		, 'usDB3prod'		, NULL			, NULL			, 'prod.x.com'	, 'prod.x.com'	, 0, NULL				, 'TSQL')

) v (
	[IsEnabled]
	,[NewAccounts]
	,[ConfigLevel]
	,[EnvironmentCode]
	,[ControlPlane]
	,[DeploymentSubGroup]
	,[PortNumber]
	,[DatabaseType]
	,[DatabaseName]
	,[CNamePrefix]
	,[Node1Prefix]
	,[Node2Prefix]
	,[Node3Prefix]
	,[InstanceSuffix]
	,[CNameSuffix]
	,[IsSingleTenantDB]
	,[SingleTenantName]
	,[SQLDialect]
	)
GO