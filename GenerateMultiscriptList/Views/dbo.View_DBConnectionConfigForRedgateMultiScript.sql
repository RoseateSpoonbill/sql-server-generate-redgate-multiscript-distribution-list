/** ==================================================================================================================
Description	:	Contains the list of Databases for Redgate Multiscript
================================================================================================================== **/

CREATE VIEW dbo.View_DBConnectionConfigForRedgateMultiScript

AS

	SELECT 
		EnvironmentCode
		, DatabaseType
		, DatabaseName
		, CNamePrefix
		, CNameSuffix
		, Node1Prefix
		, Node2Prefix
		, Node3Prefix
		, InstanceSuffix
		, PortNumber
	FROM dbo.View_DBConnectionConfig
	WHERE IsEnabled = 1
		AND SQLDialect = 'TSQL';

GO