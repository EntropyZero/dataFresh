dataFresh by EntropyZero Consulting

Entropy's dataFresh is a toolkit that assists test driven development 
projects in restoring their database to a known state before each test within 
a test fixture.  The time consuming effort of having to write tear down 
methods to clean up the database after running your tests are a thing of the 
past.

Our appoach is unlike others as we do not attempt to rip and replace the 
entire database.  Instead we track database modifications to the table 
level and only work with those tables that have been modified.


Preparing your database

Before you can use dataFresh to refresh your data you will need to prepare
your database.  Calling the PrepareDatabaseforDataFresh method will connect
to the specified database and will create the dataFresh elements required 
to track and refresh your data.

Example:

	string connectionString = "<your connection string>";
	SqlDataFresh dataFresh = new SqlDataFresh(connectionString);
	dataFresh.PrepareDatabaseforDataFresh();


Create Snapshot

As part of the call to the PrepareDatabaseforDataFresh method, we create
a snapshot of your database that will be used to refresh the database.
Alternativly you may request that the call to prepare the database skip
the snapshot step, in which case you will want to need to to call the 
CreateSnapshot method manually before you will be able to refresh the 
database.  You may also override the filepath where there snapshot 
resides by setting the SnapshotPath property.

Example:

	string connectionString = "<your connection string>";
	SqlDataFresh dataFresh = new SqlDataFresh(connectionString);
	bool createSnapshot = false;
	dataFresh.PrepareDatabaseforDataFresh(createSnapshot);
	

Specify the SnapshotPath

You may tell dataFresh where you want to save the snapshot files of your 
database by setting the SnapshotPath property of your dataFresh instance.
Please be aware that the location will be referenced by and therefore 
should be accessible to the SQL server.

Example:

	string connectionString = "<your connection string>";
	SqlDataFresh dataFresh = new SqlDataFresh(connectionString);
	bool createSnapshot = false;
	DirectoryInfo snapshotPath = 
		new DirectoryInfo("d:\Sql\Data\Snapshot_DatabaseName");
	dataFresh.PrepareDatabaseforDataFresh(createSnapshot);
	

HasDatabaseBeenModified Method

This method will strangely enough return true if your database has been 
modified since the last refresh command.

Example:

	string connectionString = "<your connection string>";
	SqlDataFresh dataFresh = new SqlDataFresh(connectionString);
	if(dataFresh.HasDatabaseBeenModified)
	{
		Console.Out("Database has been modified!");
		dataFresh.RefreshTheDatabase();
	}


RefreshTheDatabase Method

When called, this method will clear out the modified tables and refresh
the database from the snapshot files.  If you specified an alternate
location for your snapshot files, please be sure the the SnapshotPath
property has been set before calling this command.


RefreshTheEntireDatabase Method

When called, this method will clear out and refresh all tables regardless 
if they have been modified or not. Again, If you specified an alternate
location for your snapshot files, please be sure the the SnapshotPath
property has been set before calling this command.


RemoveDataFreshFromDatabase

When you are ready to move into production, or no longer need the help of 
dataFresh, simply call the RemoveDataFreshFromDatabase method and we will
gladly remove all the dataFresh elements from your database. :)


dataFresh on the Command Line

You can integrate dataFresh into your automated builds by using the command 
line version of dataFresh. Here are some examples of the concepts listed 
above.

DataFreshUtil.exe -s 127.0.0.1 -u <user> -p <pwd> -d DataFreshSample -c PREPARE

DataFreshUtil.exe -s 127.0.0.1 -u <user> -p <pwd> -d DataFreshSample -c REFRESH

DataFreshUtil.exe -s 127.0.0.1 -u <user> -p <pwd> -d DataFreshSample -c REMOVE
