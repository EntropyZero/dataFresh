using System;
using DataFresh;
using NUnit.Framework;
using System.IO;

namespace Testing.DataFresh
{
	[TestFixture]
	public class DataFreshConsoleTester
	{
		readonly string userId;
		readonly string password;
		readonly string server;

		public DataFreshConsoleTester()
		{
			var doc = new System.Xml.XmlDocument();
			doc.Load(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location) ?? string.Empty, @"..\..\..\TestConnectionStrings.xml"));
			userId = doc.SelectSingleNode("/connectionStrings/properties/userId")?.InnerText;
			password = doc.SelectSingleNode("/connectionStrings/properties/password")?.InnerText;
			server = doc.SelectSingleNode("/connectionStrings/properties/server")?.InnerText;
		}

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
			var args = new string[] { };
			var console = new DataFreshConsole();
			Assert.DoesNotThrow(() => console.Start(args));
		}

		[Test]
		public void BlankArgs()
		{
			var args = new[] { "", "" };
			var console = new DataFreshConsole();
			Assert.DoesNotThrow(() => console.Start(args));
		}

		[Test]
		public void BadCommand()
		{
			var console = ExecuteDataFreshConsole("BADCOMMAND");
			StringAssert.Contains("Command 'BADCOMMAND' was not recognized", console.Results);
		}

		[Test]
		public void PrepareCommand()
		{
			Assert.DoesNotThrow(() => ExecuteDataFreshConsole("PREPARE"));
		}

		[Test]
		public void PrepareCommandIgnoreSnapshot()
		{
			Assert.DoesNotThrow(() => ExecuteDataFreshConsole("PREPARE", "-ignoresnapshot", "1"));
		}

		[Test]
		public void RefreshCommand()
		{
			Assert.DoesNotThrow(() => ExecuteDataFreshConsole("REFRESH"));
		}

		[Test]
		public void RefreshWithoutPrepareCommand()
		{
			ExecuteDataFreshConsole("REMOVE");
			Assert.Throws<SqlDataFreshException>(() => ExecuteDataFreshConsole("REFRESH"));
		}

		[Test]
		public void CheckResults()
		{
			var console = ExecuteDataFreshConsole("REFRESH");
			Assert.IsTrue(console.Results.IndexOf("Entropy Zero", StringComparison.Ordinal) > -1);
		}

		[Test]
		public void PassServerInstanceNameCheckConnectionString()
		{
			const string serverInstance = @"localhost\dev";
			var console = DataFreshConsole.Execute("FOO", "test", "test", serverInstance, "DataFreshSample");
			Assert.AreEqual(5, console.Arguments.Keys.Count);
			Assert.AreEqual(serverInstance, console.Arguments["s"]);
			const string expectedConnectionString = @"user id=test;password=test;Initial Catalog=DataFreshSample;Data Source=localhost\dev;";
			Assert.AreEqual(expectedConnectionString, console.ConnectionString);
		}

		[Test]
		public void PassServerNameCheckConnectionString()
		{
			const string serverInstance = @"localhost";
			var console = DataFreshConsole.Execute("FOO", "test", "test", serverInstance, "DataFreshSample");
			Assert.AreEqual(5, console.Arguments.Keys.Count);
			Assert.AreEqual(serverInstance, console.Arguments["s"]);
			const string expectedConnectionString = @"user id=test;password=test;Initial Catalog=DataFreshSample;Data Source=localhost;";
			Assert.AreEqual(expectedConnectionString, console.ConnectionString);
		}

		[Test]
		public void CheckSnapshotPath()
		{
			//-sp &quot;${CCNetWorkingDirectory}\Web Harmony Solution\Database\Baseline\BaselineData&quot;"
			//-sp '${CCNetWorkingDirectory}\Web Harmony Solution\Database\Baseline\BaselineData'
			const string snapshotPath = @"c:\temp";
			var console = DataFreshConsole.Execute("FOO", "test", "test", "localhost", "DataFreshSample", "-sp", snapshotPath);
			Assert.AreEqual(6, console.Arguments.Keys.Count);
			Assert.AreEqual(snapshotPath, console.Arguments["sp"]);
		}

		DataFreshConsole ExecuteDataFreshConsole(string command, params string[] options)
		{
			var console = DataFreshConsole.Execute(command, userId, password, server, "DataFreshSample", options);
			return console;
		}
	}
}