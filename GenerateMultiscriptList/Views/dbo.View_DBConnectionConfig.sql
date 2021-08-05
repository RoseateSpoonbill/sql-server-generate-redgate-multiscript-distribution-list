/** ==================================================================================================================
Description	:	List of databases across all environments
================================================================================================================== **/

CREATE VIEW [dbo].[View_DBConnectionConfig]

AS

SELECT
	[IsEnabled]				-- true if the application can access the database
	,[NewAccounts]			-- true if new accounts can be created in the database
	,[ConfigLevel]			-- 0 = Standard, 1 = Enterprise, 2 = Automation
	,[EnvironmentCode]		-- D = Development, T = Test, S = Stage, P = Production
	,[ControlPlane]			-- US, EU, AP
	,[DeploymentSubGroup]	-- Dev, Test, Stage, or Prod
	,[PortNumber]
	,[DatabaseType]			-- A, B, SSISDB
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
	 (1, 0, 0, 'D', 'US'	, 'Dev'		, '#'		, 'A'			, 'A'			, 'A-dev'			, 'A'				, NULL			, NULL			, 'dev.x.com'	, 'dev.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'D', 'GL'	, 'Dev'		, '#'		, 'AL'			, 'AL'			, 'AL-dev'			, 'AL'				, NULL			, NULL			, 'dev.x.com'	, 'dev.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'D', 'GL'	, 'Dev'		, '#'		, 'B'			, 'B'			, 'B-dev'			, 'B'				, NULL			, NULL			, 'dev.x.com'	, 'dev.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'D', 'GL'	, 'Dev'		, '#'		, 'C'			, 'C'			, 'C-dev'			, 'C'				, NULL			, NULL			, 'dev.x.com'	, 'dev.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'D', 'GL'	, 'Dev'		, '#'		, 'DBA'			, 'DBA'			, 'DBA-dev'			, 'DBA'				, NULL			, NULL			, 'dev.x.com'	, 'dev.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'D', 'GL'	, 'Dev'		, '#'		, 'E'			, 'E'			, 'E-dev'			, 'E'				, NULL			, NULL			, 'dev.x.com'	, 'dev.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'D', 'GL'	, 'Dev'		, '#'		, 'GA'			, 'GA'			, 'GA-dev'			, 'GA'				, NULL			, NULL			, 'dev.x.com'	, 'dev.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'D', 'GL'	, 'Dev'		, '#'		, 'I'			, 'I'			, 'I-dev'			, 'I'				, NULL			, NULL			, 'dev.x.com'	, 'dev.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'D', 'GL'	, 'Dev'		, '#'		, 'R'			, 'R'			, 'R-dev'			, 'R'				, NULL			, NULL			, 'dev.x.com'	, 'dev.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'D', 'US'	, 'Dev'		, '#'		, 'SSISDB'		, 'SSISDB'		, 'SSISDB-us-dev'	, 'usSSISDBdev'		, NULL			, NULL			, 'dev.x.com'	, 'dev.x.com'	, 0, NULL				, 'TSQL')
	,(1, 1, 0, 'D', 'GL'	, 'Dev'		, '#'		, 'V'			, 'V'			, 'V-dev'			, 'V'				, NULL			, NULL			, 'dev.x.com'	, 'dev.x.com'	, 0, NULL				, 'TSQL')

	--------------------------------------------------------------------------------------------------
	-- Test
	--------------------------------------------------------------------------------------------------
	--E,NA,CL, Env, CtlP	, DeployGrp		, PortNumber, DatabaseType	, DatabaseName	, CNamePrefix		, Node1Prefix		, Node2Prefix	, Node3Prefix	, InstanceSuffix, CNameSuffix	,ST, SingleTenantName	, SQLDialect
	,(1, 0, 0, 'T', 'US'	, 'Test'		, '#'		, 'A'			, 'A'			, 'A-test'			, 'A'				, NULL			, NULL			, 'test.x.com'	, 'test.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'T', 'GL'	, 'Test'		, '#'		, 'AL'			, 'AL'			, 'AL-test'			, 'AL'				, NULL			, NULL			, 'test.x.com'	, 'test.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'T', 'GL'	, 'Test'		, '#'		, 'B'			, 'B'			, 'B-test'			, 'B'				, NULL			, NULL			, 'test.x.com'	, 'test.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'T', 'GL'	, 'Test'		, '#'		, 'C'			, 'C'			, 'C-test'			, 'C'				, NULL			, NULL			, 'test.x.com'	, 'test.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'T', 'GL'	, 'Test'		, '#'		, 'DBA'			, 'DBA'			, 'DBA-test'		, 'DBA'				, NULL			, NULL			, 'test.x.com'	, 'test.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'T', 'GL'	, 'Test'		, '#'		, 'E'			, 'E'			, 'E-test'			, 'E'				, NULL			, NULL			, 'test.x.com'	, 'test.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'T', 'GL'	, 'Test'		, '#'		, 'GA'			, 'GA'			, 'GA-test'			, 'GA'				, NULL			, NULL			, 'test.x.com'	, 'test.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'T', 'GL'	, 'Test'		, '#'		, 'I'			, 'I'			, 'I-test'			, 'I'				, NULL			, NULL			, 'test.x.com'	, 'test.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'T', 'GL'	, 'Test'		, '#'		, 'R'			, 'R'			, 'R-test'			, 'R'				, NULL			, NULL			, 'test.x.com'	, 'test.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'T', 'US'	, 'Test'		, '#'		, 'SSISDB'		, 'SSISDB'		, 'SSISDB-us-test'	, 'usSSISDBtest'	, NULL			, NULL			, 'test.x.com'	, 'test.x.com'	, 0, NULL				, 'TSQL')
	,(1, 1, 0, 'T', 'GL'	, 'Test'		, '#'		, 'V'			, 'V'			, 'V-test'			, 'V'				, NULL			, NULL			, 'test.x.com'	, 'test.x.com'	, 0, NULL				, 'TSQL')

	--------------------------------------------------------------------------------------------------
	-- Stage
	--------------------------------------------------------------------------------------------------
	--E,NA,CL, Env, CtlP	, DeployGrp		, PortNumber, DatabaseType	, DatabaseName	, CNamePrefix		, Node1Prefix		, Node2Prefix	, Node3Prefix	, InstanceSuffix, CNameSuffix	,ST, SingleTenantName	, SQLDialect
	,(1, 0, 0, 'S', 'US'	, 'Stage'		, '#'		, 'A'			, 'A'			, 'A-stage'			, 'A'				, NULL			, NULL			, 'stage.x.com'	, 'stage.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'S', 'GL'	, 'Stage'		, '#'		, 'AL'			, 'AL'			, 'AL-stage'		, 'AL'				, NULL			, NULL			, 'stage.x.com'	, 'stage.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'S', 'GL'	, 'Stage'		, '#'		, 'B'			, 'B'			, 'B-stage'			, 'B'				, NULL			, NULL			, 'stage.x.com'	, 'stage.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'S', 'GL'	, 'Stage'		, '#'		, 'C'			, 'C'			, 'C-stage'			, 'C'				, NULL			, NULL			, 'stage.x.com'	, 'stage.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'S', 'GL'	, 'Stage'		, '#'		, 'DBA'			, 'DBA'			, 'DBA-stage'		, 'DBA'				, NULL			, NULL			, 'stage.x.com'	, 'stage.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'S', 'GL'	, 'Stage'		, '#'		, 'E'			, 'E'			, 'E-stage'			, 'E'				, NULL			, NULL			, 'stage.x.com'	, 'stage.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'S', 'GL'	, 'Stage'		, '#'		, 'GA'			, 'GA'			, 'GA-stage'		, 'GA'				, NULL			, NULL			, 'stage.x.com'	, 'stage.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'S', 'GL'	, 'Stage'		, '#'		, 'I'			, 'I'			, 'I-stage'			, 'I'				, NULL			, NULL			, 'stage.x.com'	, 'stage.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'S', 'GL'	, 'Stage'		, '#'		, 'R'			, 'R'			, 'R-stage'			, 'R'				, NULL			, NULL			, 'stage.x.com'	, 'stage.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'S', 'US'	, 'Stage'		, '#'		, 'SSISDB'		, 'SSISDB'		, 'SSISDB-us-stage'	, 'usSSISDBstage'	, NULL			, NULL			, 'stage.x.com'	, 'stage.x.com'	, 0, NULL				, 'TSQL')
	,(1, 1, 0, 'S', 'GL'	, 'Stage'		, '#'		, 'V'			, 'V'			, 'V-stage'			, 'V'				, NULL			, NULL			, 'stage.x.com'	, 'stage.x.com'	, 0, NULL				, 'TSQL')

	--------------------------------------------------------------------------------------------------
	-- Production
	--------------------------------------------------------------------------------------------------
	--E,NA,CL, Env, CtlP	, DeployGrp		, PortNumber, DatabaseType	, DatabaseName	, CNamePrefix		, Node1Prefix		, Node2Prefix	, Node3Prefix	, InstanceSuffix, CNameSuffix	,ST, SingleTenantName	, SQLDialect
	,(1, 0, 0, 'P', 'US'	, 'Prod'		, '#'		, 'A'			, 'A'			, 'A-prod'			, 'A'				, NULL			, NULL			, 'prod.x.com'	, 'prod.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'P', 'GL'	, 'Prod'		, '#'		, 'AL'			, 'AL'			, 'AL-prod'			, 'AL'				, NULL			, NULL			, 'prod.x.com'	, 'prod.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'P', 'GL'	, 'Prod'		, '#'		, 'B'			, 'B'			, 'B-prod'			, 'B'				, NULL			, NULL			, 'prod.x.com'	, 'prod.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'P', 'GL'	, 'Prod'		, '#'		, 'C'			, 'C'			, 'C-prod'			, 'C'				, NULL			, NULL			, 'prod.x.com'	, 'prod.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'P', 'GL'	, 'Prod'		, '#'		, 'DBA'			, 'DBA'			, 'DBA-prod'		, 'DBA'				, NULL			, NULL			, 'prod.x.com'	, 'prod.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'P', 'GL'	, 'Prod'		, '#'		, 'E'			, 'E'			, 'E-prod'			, 'E'				, NULL			, NULL			, 'prod.x.com'	, 'prod.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'P', 'GL'	, 'Prod'		, '#'		, 'GA'			, 'GA'			, 'GA-prod'			, 'GA'				, NULL			, NULL			, 'prod.x.com'	, 'prod.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'P', 'GL'	, 'Prod'		, '#'		, 'I'			, 'I'			, 'I-prod'			, 'I'				, NULL			, NULL			, 'prod.x.com'	, 'prod.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'P', 'GL'	, 'Prod'		, '#'		, 'R'			, 'R'			, 'R-prod'			, 'R'				, NULL			, NULL			, 'prod.x.com'	, 'prod.x.com'	, 0, NULL				, 'TSQL')
	,(1, 0, 0, 'P', 'US'	, 'Prod'		, '#'		, 'SSISDB'		, 'SSISDB'		, 'SSISDB-us-prod'	, 'usSSISDBprod'	, NULL			, NULL			, 'prod.x.com'	, 'prod.x.com'	, 0, NULL				, 'TSQL')
	,(1, 1, 0, 'P', 'GL'	, 'Prod'		, '#'		, 'V'			, 'V'			, 'V-prod'			, 'V'				, NULL			, NULL			, 'prod.x.com'	, 'prod.x.com'	, 0, NULL				, 'TSQL')

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