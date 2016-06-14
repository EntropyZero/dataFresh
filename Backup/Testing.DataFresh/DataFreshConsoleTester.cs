using DataFresh;
using NUnit.Framework;

namespace Testing.DataFresh
{
	[TestFixture]
	public class DataFreshConsoleTester
	{
		[SetUp]
		public void Setup()
		{
			ExecuteDataFreshConsole("PREPARE");
		}

		[TearDown]
		public void TearDown()
		{
			ExecuteDataFreshConsole("REMOVE");
		}

		[Test]
		public void NoArgs()
		{
			string[] args = new string[] {};
			DataFreshConsole console = new DataFreshConsole();
			console.Start(args);
		}

		[Test]
		public void BlankArgs()
		{
			string[] args = new string[] {"", ""};
			DataFreshConsole console = new DataFreshConsole();
			console.Start(args);
		}

		[Test]
		public void BadCommand()
		{
			DataFreshConsole console = ExecuteDataFreshConsole("BADCOMMAND");
			Assert.IsTrue(console.Results.ToString().IndexOf("Command 'BADCOMMAND' was not recognized") > -1);
		}

		[Test]
		public void PrepareCommand()
		{
			DataFreshConsole console = ExecuteDataFreshConsole("PREPARE");
		}

		[Test]
		public void PrepareCommandIgnoreSnapshot()
		{
			DataFreshConsole console = ExecuteDataFreshConsole("PREPARE", "-ignoresnapshot", "1");

		}

		[Test]
		public void RefreshCommand()
		{
			ExecuteDataFreshConsole("REFRESH");
		}

		[Test, ExpectedException(typeof (SqlDataFreshException))]
		public void RefreshWithoutPrepareCommand()
		{
			ExecuteDataFreshConsole("REMOVE");
			ExecuteDataFreshConsole("REFRESH");
		}

		[Test]
		public void CheckResults()
		{
			DataFreshConsole console = ExecuteDataFreshConsole("REFRESH");
			Assert.IsTrue(console.Results.ToString().IndexOf("Entropy Zero") > -1);
		}

		[Test]
		public void PassServerInstanceNameCheckConnectionString()
		{
			string serverInstance = @"localhost\dev";
			DataFreshConsole console = DataFreshConsole.Execute("FOO", "test", "test", serverInstance, "DataFreshSample");
			Assert.AreEqual(5, console.arguments.Keys.Count);
			Assert.AreEqual(serverInstance, console.arguments["s"]);
			string expectedConnectionString = @"user id=test;password=test;Initial Catalog=DataFreshSample;Data Source=localhost\dev;";
			Assert.AreEqual(expectedConnectionString, console.connectionString);
		}

		[Test]
		public void PassServerNameCheckConnectionString()
		{
			string serverInstance = @"localhost";
			DataFreshConsole console = DataFreshConsole.Execute("FOO", "test", "test", serverInstance, "DataFreshSample");
			Assert.AreEqual(5, console.arguments.Keys.Count);
			Assert.AreEqual(serverInstance, console.arguments["s"]);
			string expectedConnectionString = @"user id=test;password=test;Initial Catalog=DataFreshSample;Data Source=localhost;";
			Assert.AreEqual(expectedConnectionString, console.connectionString);
		}
		
		[Test]
		public void CheckSnapshotPath()
		{
			//-sp &quot;${CCNetWorkingDirectory}\Web Harmony Solution\Database\Baseline\BaselineData&quot;"
			//-sp '${CCNetWorkingDirectory}\Web Harmony Solution\Database\Baseline\BaselineData'
			string snapshotPath = @"c:\temp";
			DataFreshConsole console = DataFreshConsole.Execute("FOO", "test", "test", "localhost", "DataFreshSample", "-sp", snapshotPath);
			Assert.AreEqual(6, console.arguments.Keys.Count);
			Assert.AreEqual(snapshotPath, console.arguments["sp"]);
		}

		private static DataFreshConsole ExecuteDataFreshConsole(string command, params string[] options)
		{
			DataFreshConsole console = DataFreshConsole.Execute(command, "test", "test", "localhost", "DataFreshSample", options);
			return console;
		}
	}
}