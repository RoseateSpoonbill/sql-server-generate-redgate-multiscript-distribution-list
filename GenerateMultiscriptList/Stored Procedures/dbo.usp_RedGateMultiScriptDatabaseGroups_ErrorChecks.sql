/** ==================================================================================================================
Description	:	Check RedGate Multiscript parameters for errors

Parameters
		@Domain				:	com
		@EnvironmentCode	:	If you only want results for an environment
								Valid values: null, D, T, S, P
		@DatabaseType		:	If you only want results for a specific DB type
								Valid values: null, A, AL, B, C, DBA, E, GA, I, R, SSISDB, V
		@AuthType			:	What kind of authentication will the DB connections use?
								Windows = Windows Authentication
								SQL	= SQL Server Authentication
		@SQLUserName		: If @AuthType = SQL, what user will this be for?
================================================================================================================== **/

CREATE PROCEDURE dbo.usp_RedGateMultiScriptDatabaseGroups_ErrorChecks

	@Domain				VARCHAR(16)		= NULL
	, @EnvironmentCode	VARCHAR(1)		= NULL
	, @DatabaseType		VARCHAR(32)		= NULL
	, @AuthType			VARCHAR(32)		= NULL
	, @SQLUserName		NVARCHAR(128)	= NULL

AS

SET NOCOUNT ON;
 
BEGIN TRY
		IF (@Domain IS NULL OR @Domain NOT IN ('%.com'))
		BEGIN
			; THROW 50001, 'Invalid @Domain', 1;
		END

		IF (@EnvironmentCode IS NOT NULL AND @EnvironmentCode NOT IN ('D', 'T', 'S', 'P'))
		BEGIN
			; THROW 50001, 'Invalid @EnvironmentCode', 1;
		END

		IF (@DatabaseType IS NOT NULL AND @DatabaseType NOT IN ('A', 'AL', 'B', 'C', 'DBA', 'E', 'GA', 'I', 'R', 'SSISDB', 'V'))
		BEGIN
			; THROW 50001, 'Invalid @DatabaseType', 1;
		END

		IF (@AuthType IS NULL OR @AuthType NOT IN ('Windows', 'SQL'))
		BEGIN
			; THROW 50001, 'Invalid @AuthType', 1;
		END

		IF (@AuthType = 'Windows' AND @SQLUserName IS NOT NULL)
		BEGIN
			; THROW 50001, '@SQLUserName should be null for Windows auth', 1;
		END

		IF (@AuthType = 'SQL' AND @SQLUserName IS NULL)
		BEGIN
			; THROW 50001, 'Invalid @SQLUserName', 1;
		END
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
