/** ==================================================================================================================
Description	:	Return all RedGate Multiscript's Database Groups for this domain

Parameters
		@Domain				:	com
		@EnvironmentCode	:	If you only want results for an environment
								Valid values: null, D, T, S, P
		@DatabaseType		:	If you only want results for a specific DB type
								Valid values: null, A, AL, B, C, D, E, GA, I, R, SSISDB, V
		@AuthType			:	What kind of authentication will the DB connections use?
								Windows = Windows Authentication
								SQL	= SQL Server Authentication
		@SQLUserName		: If @AuthType = SQL, what user will this be for?
================================================================================================================== **/

CREATE PROCEDURE dbo.usp_RedGateMultiScriptDatabaseGroups_Get

	@Domain				VARCHAR(16)
	, @EnvironmentCode	VARCHAR(1)		= NULL
	, @DatabaseType		VARCHAR(32)		= NULL
	, @AuthType			VARCHAR(32)
	, @SQLUserName		NVARCHAR(256)	= NULL

AS

SET NOCOUNT ON;
 
BEGIN TRY

	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Error checks
	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		EXEC dbo.usp_RedGateMultiScriptDatabaseGroups_ErrorChecks
			@Domain				= @Domain
			, @EnvironmentCode	= @EnvironmentCode
			, @DatabaseType		= @DatabaseType
			, @AuthType			= @AuthType
			, @SQLUserName		= @SQLUserName;

	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Variables
	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		DECLARE
			@SQLUserNameToJoinOn NVARCHAR(256) = IIF(@AuthType = 'SQL' AND @SQLUserName IS NOT NULL AND @SQLUserName <> 'DeployUser', '', @SQLUserName);

	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Temp Tables
	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		DECLARE @GroupsToCreate TABLE (
			GroupID				INT				NOT NULL	IDENTITY(1,1)
			, EnvironmentCode	VARCHAR(1)		NOT NULL
			, DatabaseType		VARCHAR(32)		NOT NULL
			, NodeType			VARCHAR(32)		NOT NULL
			, AuthType			VARCHAR(32)		NOT NULL
			, SQLUserName		NVARCHAR(256)	NULL
			, GroupGuid			VARCHAR(36)		NULL
			, GroupName			VARCHAR(128)	NULL
			);

	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Groups
	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		INSERT INTO @GroupsToCreate (
			EnvironmentCode
			, DatabaseType
			, AuthType
			, SQLUserName
			, NodeType
			)
		SELECT 
			X.EnvironmentCode
			, X.DatabaseType
			, Y.AuthType
			, Y.SQLUserName
			, Y.NodeType
		FROM (
			SELECT DISTINCT
				EnvironmentCode
				, DatabaseType
			FROM dbo.View_DBConnectionConfigForRedgateMultiScript
			WHERE InstanceSuffix LIKE @Domain
			) AS X
		INNER JOIN (
				SELECT DISTINCT
					EnvironmentCode
					, DatabaseType
					, AuthType
					, SQLUserName
					, NodeType
				FROM dbo.View_RedgateMultiScriptDatabaseGroups
				WHERE AuthType = @AuthType
					AND (SQLUserName = @SQLUserNameToJoinOn
						OR (SQLUserName IS NULL
							AND @SQLUserNameToJoinOn IS NULL
							)
						)
				) AS Y
			ON X.EnvironmentCode = Y.EnvironmentCode
				AND X.DatabaseType = Y.DatabaseType
		WHERE 
			(@EnvironmentCode IS NULL
				OR X.EnvironmentCode = @EnvironmentCode
				)
			AND (@DatabaseType IS NULL
				OR X.DatabaseType = @DatabaseType
				);

	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Get additional info about groups
	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		UPDATE X
		SET GroupGuid	= Y.GroupGuid
			, GroupName = Y.GroupName
		FROM @GroupsToCreate AS X
		INNER JOIN dbo.View_RedgateMultiScriptDatabaseGroups AS Y
			ON X.EnvironmentCode = Y.EnvironmentCode
				AND X.DatabaseType = Y.DatabaseType
				AND X.AuthType = Y.AuthType
				AND X.NodeType = Y.NodeType
				AND (X.SQLUserName = Y.SQLUserName
					OR (X.SQLUserName IS NULL
						AND Y.SQLUserName IS NULL
						)
					);

	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Results
	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		SELECT
			GroupID
			, EnvironmentCode
			, DatabaseType
			, AuthType
			, @SQLUserName AS SQLUserName
			, NodeType
			, GroupGuid
			, GroupName
		FROM @GroupsToCreate;
END TRY
BEGIN CATCH
    DECLARE
        @ProcName 		NVARCHAR (257) 	= CONCAT(OBJECT_SCHEMA_NAME(@@PROCID), '.', OBJECT_NAME(@@PROCID))
        , @ErrMsg 		NVARCHAR (2048) = ERROR_MESSAGE()
        , @ErrorState 	INT 			= ERROR_STATE()
        , @ErrorLine 	INT 			= ERROR_LINE()
        , @ErrorMessage NVARCHAR(2048);
 
    -- Capture the procedure name, line number, and error message
    SET @ErrorMessage = CONCAT('Error in ', @ProcName, ' on line number ', @ErrorLine, ': ', @ErrMsg);
 
    THROW 50001, @ErrorMessage, @ErrorState;
END CATCH
GO
