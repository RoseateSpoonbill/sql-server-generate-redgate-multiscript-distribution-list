# Generate RedGate Multiscript Distribution List
This code will allow you to generate RedGate Multiscript Distribution Lists dynamically based on a list of databases in a view

## Getting Started

### Updating the files to be accurate for your system
1. Replace all of the VALUES lines in dbo.View_DBConnectionConfig with the correct values for your system
1. Alter the VALUES lines in dbo.dbo.View_RedgateMultiScriptDatabaseGroups to have the correct values for the distribution lists you want to create
	1. You can keep the current GroupGuid values, as long as they remain unique
	1. AdditionalAvailableGroupGuids.txt has more guids already generated from MultiScript
	1. There are comments at the top of this view indicating what to do if you need more GroupGuid values than the view currently has
1. Update the allowed environment values in
	1. dbo.View_Environment
	1. Constants.cs (Environment_Codes, Environment_ShortNames)
1. Update the database types in
	1. Constants.cs (Database_Types)
1. Update the allowed domain value lists/comments/whatever in
	1. dbo.usp_RedGateMultiScriptDatabaseGroups_ErrorChecks (@Domain)
	1. Constants.cs (MultiScript_DomainList_Valid)
1. Replace "DeployUser" in all scripts with your sa or service account user's name
	1. dbo.View_RedgateMultiScriptDatabaseGroups
	1. dbo.usp_RedGateMultiScriptDatabaseGroups_Get
	1. dbo.usp_DBConnectionConfig_GetByRedgateMultiscriptFormat
	1. Constants.cs (MultiScript_SqlUserNamesList)
	1. View_RedgateMultiScriptDatabaseGroups.cs (sqlUserList)
	1. usp_DBConnectionConfig_GetByRedgateMultiscriptFormat.cs (domainList_Valid)
1. Remember to update 
	1. The comments in all of the objects that indicate the allowed values
	1. The default values of any parameters, variables, etc... so they are accurate for your system

### Testing
1. Deploy the SQL objects to your desired database
1. Copy the appsettings.test.json.templatefile
	1. Rename it to appsettings.test.json
	1. Update it so it is connecting to the correct database


## Solution Structure
1. MSSQL.Extension - Contains supplimental helper methods to assist with testing
1. MSSQL.Models - Contains the models used for CRUD operations in tests (these models are tightly coupled with Database tables)
1. *.Tests - Contains all unit tests for this DB Type
