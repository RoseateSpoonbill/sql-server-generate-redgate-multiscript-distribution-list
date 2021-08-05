/** ==================================================================================================================
Description	:	List of valid environments
================================================================================================================== **/

CREATE VIEW [dbo].[View_Environment]

AS

SELECT
	[EnvironmentCode]
	, [EnvironmentShortName]
FROM (VALUES
		( 'D', 'Dev'),
		( 'T', 'Test'),
		( 'S', 'Stage'),
		( 'P', 'Prod')
	) AS  v (
		[EnvironmentCode]
		, [EnvironmentShortName]
		);
GO