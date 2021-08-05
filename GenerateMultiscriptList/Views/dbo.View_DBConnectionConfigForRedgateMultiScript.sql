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
		AND SQLDialect = 'TSQL'
		AND DatabaseType NOT IN ('D', 'IM', 'S');	-- replace these values with any DatabaseTypes you have that you do not want Multiscript lists to be created for.  delete the line if there are none.

GO