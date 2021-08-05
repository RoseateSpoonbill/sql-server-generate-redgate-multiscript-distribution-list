/** ==================================================================================================================
Description	:	Return all instances formatted in RedGate Multiscript's Database List format

*** If you are running this from SSMS with "Results to Text" make sure to increase the "Maximum number of characters displayed in each column" or the strings will get cut off ***

Parameters
	-- determine what groups & databases to output
		@Domain				:	What is the domain you want to generate lists for?
		@EnvironmentCode	:	If you only want results for an environment, put its single digit code.  Otherwise don't pass it in
		@DatabaseType		:	If you only want results for a specific DB type.  Otherwise don't pass it in

	-- determines database authentication that will be set up
		@AuthType		:	What kind of authentication will the DB connections use?
								Windows = Windows Authentication
								SQL	= SQL Server Authentication
		@SQLUserName	: If @AuthType = SQL, what user will this be for?

	-- determines what will be returned/output
		@OutputListOfGroupNames			:	Do you want a list of all the Groups that output was generated for, to compare the output against?
		@OutputListOfDatabasesPerGroup	:	Do you want a list of all the Database/Group combos that output was generated for, to compare the output against?
		@TestRun						:	Are you running this for a unit/automation test?

Notes
	Finding the correct value for the @*_SQLEncryptedPassword parameters:
		1. Open Multiscript	
		2. If you do NOT have any lists already set up using DeployUser credentials in each environment:
				a. Create a new throwaway distribution list (i.e. don't conflict with the group names in dbo.View_RedgateMultiScriptDatabaseGroups)
				b. Add to it a database connection using the DeployUser credentials for Test, Stage, Prod
				c. Export the distribution list to a file
				d. Open the file in a text editor 
				e. Copy the value for the "<password encrypted="1">" attribute for each Environment
				f. Pass ^ in for the parameter for that Environment

	If you want to use this SP to generate Lists that use a different SQL Server user name than DeployUser
		1. Run this SP with @AuthType = Windows & @SQLUserName = your user's name
		2. Save the results to a file
		3. Follow the steps for finding the encrypted passwords above, except use the username that you want a new list for when creating those connections
		4. In the file you saved, replace these values with the encrypted email passwords you found: $$Test_SQLEncryptedPassword$$, $$Stage_SQLEncryptedPassword$$, $$Prod_SQLEncryptedPassword$$
		5. Export your existing distribution lists to a file (as a backup)
		6. Import the file into Multiscript (you will have have to delete your existing distribution lists if the names or guids (but not both for the same group) match a group in this list
================================================================================================================== **/

CREATE PROCEDURE dbo.usp_DBConnectionConfig_GetByRedgateMultiscriptFormat

	@Domain								VARCHAR(16)
	, @EnvironmentCode					VARCHAR(1)		= NULL
	, @DatabaseType						VARCHAR(32)		= NULL
	, @AuthType							VARCHAR(32)		= 'Windows'
	, @SQLUserName						NVARCHAR(128)	= NULL
	, @OutputListOfGroupNames			BIT				= 0
	, @OutputListOfDatabasesPerGroup	BIT				= 0
	, @TestRun							BIT				= 0

AS

SET NOCOUNT ON;
 
BEGIN TRY
	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Update Parameter Values
	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		SET @OutputListOfGroupNames			= ISNULL(@OutputListOfGroupNames		, 0);
		SET @OutputListOfDatabasesPerGroup	= ISNULL(@OutputListOfDatabasesPerGroup	, 0);
		SET @TestRun						= ISNULL(@TestRun						, 0);

		SET @SQLUserName = IIF(@AuthType = 'Windows', NULL, ISNULL(@SQLUserName, 'DeployUser'))

		SET @Domain	= '%.' + @Domain;

	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Error Checks
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
			@PerDB				NVARCHAR(MAX)	= ''
			, @GroupGuid		VARCHAR(36)
			, @GroupName		VARCHAR(128)
			, @GroupID			INT
			, @OutputMessage	NVARCHAR(2048);

		SET @PerDB = CONCAT('
        <value version="6" type="database">
          <name>$$DatabaseName$$</name>
          <server>$$Prefx$$.$$Suffix$$,$$Port$$</server>
		  <integratedSecurity>', IIF(@AuthType = 'Windows', 'True</integratedSecurity>', CONCAT('False</integratedSecurity>
          <username>', @SQLUserName, '</username>
          <savePassword>True</savePassword>
          <password encrypted="1">$$SQLEncryptedPassword$$</password>')), '
          <connectionTimeout>15</connectionTimeout>
          <protocol>-1</protocol>
          <packetSize>4096</packetSize>
          <encrypted>False</encrypted>
          <selected>True</selected>
          <cserver>$$Prefx$$</cserver>
          <readonly>False</readonly>
        </value>');

	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Temp Tables
	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		DECLARE @EntireDatabaseList TABLE (
			DatabaseID			INT				NOT NULL	IDENTITY(1,1)
			, EnvironmentCode	VARCHAR(1)		NOT NULL
			, DatabaseType		VARCHAR(32)		NOT NULL
			, DatabaseName		VARCHAR(128)	NOT NULL
			, CNamePrefix		VARCHAR(128)	NOT NULL
			, CNameSuffix		VARCHAR(128)	NOT NULL
			, Node1Prefix		VARCHAR(128)	NOT NULL
			, Node2Prefix		VARCHAR(128)	NULL
			, Node3Prefix		VARCHAR(128)	NULL
			, InstanceSuffix	VARCHAR(128)	NOT NULL
			, PortNumber		VARCHAR(128)	NOT NULL
			);

		DECLARE @GroupsToCreate TABLE (
			GroupID				INT				NOT NULL
			, EnvironmentCode	VARCHAR(1)		NOT NULL
			, DatabaseType		VARCHAR(32)		NOT NULL
			, AuthType			VARCHAR(32)		NOT NULL
			, SQLUserName		NVARCHAR(128)	NULL
			, NodeType			VARCHAR(32)		NOT NULL
			, GroupGuid			VARCHAR(36)		NULL
			, GroupName			VARCHAR(128)	NULL
			);

		DECLARE @Output	TABLE (
			TempID			INT				NOT NULL	IDENTITY(1,1)
			, Src			VARCHAR(64)		NOT NULL
			, GroupID		INT				NULL
			, DatabaseID	INT				NULL
			, XMLCode		NVARCHAR(MAX)	NOT NULL
			);

	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Databases
	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		INSERT INTO @EntireDatabaseList (
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
			)
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
		FROM dbo.View_DBConnectionConfigForRedgateMultiScript
		WHERE InstanceSuffix LIKE @Domain;

	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Groups to Loop over
	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		INSERT INTO @GroupsToCreate (
			GroupID
			, EnvironmentCode
			, DatabaseType
			, AuthType
			, SQLUserName
			, NodeType
			, GroupGuid
			, GroupName
			)
		EXEC dbo.usp_RedGateMultiScriptDatabaseGroups_Get
			@Domain				= @Domain
			, @EnvironmentCode	= @EnvironmentCode
			, @DatabaseType		= @DatabaseType
			, @AuthType			= @AuthType
			, @SQLUserName		= @SQLUserName;

	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Databases to Loop over per Group
	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		DECLARE @DatabasesToCreatePerGroup TABLE (
			GroupID				INT				NOT NULL
			, DatabaseID		INT				NOT NULL
			, EnvironmentCode	VARCHAR(1)		NOT NULL
			, DatabaseType		VARCHAR(32)		NOT NULL
			, DatabaseName		VARCHAR(128)	NOT NULL
			, Node1Prefix		VARCHAR(128)	NOT NULL
			, Node2Prefix		VARCHAR(128)	NULL
			, Node3Prefix		VARCHAR(128)	NULL
			, Suffix			VARCHAR(128)	NOT NULL
			, PortNumber		VARCHAR(128)	NOT NULL
			, DatabaseOutput	NVARCHAR(MAX)	NULL
			);

		INSERT INTO @DatabasesToCreatePerGroup (
			GroupID
			, DatabaseID
			, EnvironmentCode
			, DatabaseType
			, DatabaseName
			, Node1Prefix
			, Node2Prefix
			, Node3Prefix
			, Suffix
			, PortNumber
			)
		SELECT 
			G.GroupID
			, D.DatabaseID
			, D.EnvironmentCode
			, D.DatabaseType
			, D.DatabaseName
			, IIF(G.NodeType = 'CNAME', D.CNamePrefix, D.Node1Prefix) AS Node1Prefix
			, CASE
				WHEN G.NodeType = 'CNAME' THEN NULL
				WHEN D.Node2Prefix IS NOT NULL AND D.Node2Prefix NOT IN ('', D.Node1Prefix) THEN D.Node2Prefix
			END AS Node2Prefix
			, CASE
				WHEN G.NodeType = 'CNAME' THEN NULL
				WHEN D.Node3Prefix IS NOT NULL AND D.Node3Prefix NOT IN ('', D.Node1Prefix, D.Node2Prefix) THEN D.Node3Prefix
			END Node3Prefix
			, IIF(G.NodeType = 'CNAME', D.CNameSuffix, D.InstanceSuffix) AS Suffix
			, D.PortNumber
		FROM @GroupsToCreate AS G
		INNER JOIN @EntireDatabaseList AS D
			ON G.EnvironmentCode = D.EnvironmentCode
				AND G.DatabaseType = D.DatabaseType;

		UPDATE @DatabasesToCreatePerGroup
		SET DatabaseOutput = 
			REPLACE(
				REPLACE(REPLACE(REPLACE(
					CONCAT(
						REPLACE(@PerDB, '$$Prefx$$', Node1Prefix)
						, IIF(Node2Prefix IS NULL, '', REPLACE(@PerDB, '$$Prefx$$', Node2Prefix))
						, IIF(Node3Prefix IS NULL, '', REPLACE(@PerDB, '$$Prefx$$', Node3Prefix))
						)
				, '$$DatabaseName$$', DatabaseName), '$$Suffix$$', Suffix), '$$Port$$', PortNumber)
				, '$$SQLEncryptedPassword$$'
				, IIF(@AuthType = 'Windows', '', CONCAT('$$', EnvironmentCode, '_SQLEncryptedPassword$$'))
				);

	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Set Start of File
	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		INSERT INTO @Output (
			Src
			, XMLCode
		)
		SELECT
			'FileStart'
			, 
'<?xml version="1.0" encoding="utf-16" standalone="yes"?>
<!--
SQL Multi Script
-->
<databaseListsFile version="1" type="databaseListsFile">
  <databaseLists type="List_databaseList" version="1">';

	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Loop through groups
	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		DECLARE GroupCursor CURSOR LOCAL FAST_FORWARD FOR

			SELECT
				GroupID
				, GroupGuid
				, GroupName
			FROM @GroupsToCreate
			ORDER BY
				GroupName
				, EnvironmentCode
				, DatabaseType
				, NodeType DESC;

		OPEN GroupCursor;
		FETCH NEXT FROM GroupCursor INTO @GroupID, @GroupGuid, @GroupName;

		WHILE ( @@FETCH_STATUS = 0 )
		BEGIN
			INSERT INTO @Output (
				Src
				, GroupID
				, XMLCode
				)
			SELECT
				'GroupStart'
				, @GroupID
				, CONCAT(
'    <value version="2" type="databaseList">
      <name>', @GroupName, '</name>
      <databases type="BindingList_database" version="1">');

			INSERT INTO @Output (
				Src
				, GroupID
				, DatabaseID
				, XMLCode
				)
			SELECT
				'Database'
				, @GroupID
				, DatabaseID
				, DatabaseOutput
			FROM @DatabasesToCreatePerGroup
			WHERE GroupID = @GroupID
			ORDER BY 
				DatabaseName
				, Node1Prefix;

			INSERT INTO @Output (
				Src
				, GroupID
				, XMLCode
			)
			SELECT
				'GroupEnd'
				, @GroupID
				, CONCAT(
'      </databases>
      <guid>', @GroupGuid, '</guid>
    </value>');

			FETCH NEXT FROM GroupCursor INTO @GroupID, @GroupGuid, @GroupName;
		END;

		CLOSE GroupCursor;
		DEALLOCATE GroupCursor;

	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Set Start and End of File
	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		INSERT INTO @Output (
			Src
			, XMLCode
		)
		SELECT
			'FileEnd'
			, 
'  </databaseLists>
</databaseListsFile>';

	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Validation
	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		IF EXISTS (
			SELECT 1
			FROM @GroupsToCreate AS G
			FULL OUTER JOIN (
				SELECT GroupID
				FROM @Output
				WHERE GroupID IS NOT NULL
				GROUP BY GroupID
				) AS O
				ON G.GroupID = O.GroupID
			WHERE G.GroupID IS NULL
				OR O.GroupID IS NULL
			)
		BEGIN
			SELECT 
				G.GroupID
				, G.EnvironmentCode
				, G.DatabaseType
				, G.AuthType
				, G.NodeType
				, G.GroupGuid
				, G.GroupName
				, O.GroupID
			FROM @GroupsToCreate AS G
			FULL OUTER JOIN (
				SELECT GroupID
				FROM @Output
				WHERE GroupID IS NOT NULL
				GROUP BY GroupID
				) AS O
				ON G.GroupID = O.GroupID
			WHERE G.GroupID IS NULL
				OR O.GroupID IS NULL;

			THROW 50001, '@Output does not contain all groups from @GroupsToCreate', 1;
		END

		IF EXISTS (
			SELECT 1
			FROM @GroupsToCreate AS G
			INNER JOIN (
				SELECT GroupID, COUNT(DISTINCT DatabaseID) AS NumDBs
				FROM @Output
				WHERE GroupID IS NOT NULL
				GROUP BY GroupID
				) AS O
				ON G.GroupID = O.GroupID
			WHERE O.NumDBs = 0
			)
		BEGIN
			SELECT 
				G.GroupID
				, G.EnvironmentCode
				, G.DatabaseType
				, G.AuthType
				, G.NodeType
				, G.GroupGuid
				, G.GroupName
				, O.NumDBs
			FROM @GroupsToCreate AS G
			INNER JOIN (
				SELECT GroupID, COUNT(DISTINCT DatabaseID) AS NumDBs
				FROM @Output
				WHERE GroupID IS NOT NULL
				GROUP BY GroupID
				) AS O
				ON G.GroupID = O.GroupID
			WHERE O.NumDBs = 0;

			THROW 50001, '@Output does not contain any databases for some groups', 1;
		END

		IF EXISTS (
			SELECT 1
			FROM @EntireDatabaseList AS D
			FULL OUTER JOIN (
				SELECT DatabaseID
				FROM @Output
				WHERE DatabaseID IS NOT NULL
				GROUP BY DatabaseID
				) AS O
				ON D.DatabaseID = O.DatabaseID
			WHERE D.DatabaseID IS NULL
				OR O.DatabaseID IS NULL
			)
		BEGIN
			SELECT 
				D.DatabaseID
				, D.EnvironmentCode
				, D.DatabaseType
				, D.DatabaseName
				, D.CNamePrefix
				, D.CNameSuffix
				, D.Node1Prefix
				, D.Node2Prefix
				, D.Node3Prefix
				, D.InstanceSuffix
				, D.PortNumber
				, O.DatabaseID
			FROM @EntireDatabaseList AS D
			FULL OUTER JOIN (
				SELECT DatabaseID
				FROM @Output
				WHERE DatabaseID IS NOT NULL
				GROUP BY DatabaseID
				) AS O
				ON D.DatabaseID = O.DatabaseID
			WHERE D.DatabaseID IS NULL
				OR O.DatabaseID IS NULL;

			THROW 50001, '@Output does not contain all databases from @EntireDatabaseList', 1;
		END

	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Results
	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		IF (@TestRun = 0)
		BEGIN
			SELECT 
				XMLCode
			FROM @Output
			ORDER BY TempID;
		END
		ELSE
		BEGIN
			SELECT 
				O.TempID
				, O.Src
				, O.GroupID
				, O.DatabaseID
				, O.XMLCode
				, G.EnvironmentCode AS G_EnvironmentCode
				, G.DatabaseType AS G_DatabaseType
				, G.AuthType
				, G.NodeType
				, G.GroupGuid
				, G.GroupName
				, D.EnvironmentCode AS D_EnvironmentCode
				, D.DatabaseType AS D_DatabaseType
				, D.DatabaseName
				, D.CNamePrefix
				, D.CNameSuffix
				, D.Node1Prefix
				, D.Node2Prefix
				, D.Node3Prefix
				, D.InstanceSuffix
				, D.PortNumber
			FROM @Output AS O
			LEFT JOIN @GroupsToCreate AS G
				ON O.GroupID = G.GroupID
			LEFT JOIN @EntireDatabaseList AS D
				ON O.DatabaseID = D.DatabaseID
			ORDER BY TempID;
		END

		IF (@AuthType <> 'WindowsAuth')
		BEGIN
			SET @OutputMessage = 'Remember to replace the "$$?_SQLEncryptedPassword$$" values in the XMLCode in the file with the correct encrypted passwords for the user and environment';
			RAISERROR(@OutputMessage, 10, 1);
		END

	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Output info to validate against
	--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		IF (@OutputListOfGroupNames = 1)
		BEGIN
			SELECT 
				G.GroupName
				, G.EnvironmentCode AS EnvironmentCode
				, G.DatabaseType	AS DatabaseType
				, G.AuthType
				, G.NodeType
				, G.GroupGuid
			FROM @Output AS O
			LEFT JOIN @GroupsToCreate AS G
				ON O.GroupID = G.GroupID
			WHERE O.GroupID IS NOT NULL
				AND O.DatabaseID IS NULL
			GROUP BY
				G.GroupName
				, G.EnvironmentCode
				, G.DatabaseType
				, G.AuthType
				, G.NodeType
				, G.GroupGuid
			ORDER BY 
				MIN(O.TempID);
		END

		IF (@OutputListOfDatabasesPerGroup = 1)
		BEGIN
			SELECT 
				G.GroupName
				, D.EnvironmentCode
				, D.DatabaseType
				, D.DatabaseName
				, G.AuthType
				, G.NodeType
				, D.Node1Prefix
				, D.Node2Prefix
				, D.Node3Prefix
			FROM @Output AS O
			LEFT JOIN @DatabasesToCreatePerGroup AS D
				ON O.GroupID = D.GroupID
					AND O.DatabaseID = D.DatabaseID
			LEFT JOIN @GroupsToCreate AS G
				ON O.GroupID = G.GroupID
			WHERE D.DatabaseID IS NOT NULL
			ORDER BY O.TempID;
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
