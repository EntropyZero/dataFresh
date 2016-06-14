// EntropyZero dataFresh Copyright (C) 2007 EntropyZero Consulting, LLC.
// Please visit us on the web: http://blogs.ent0.com/
//
// This library is free software; you can redistribute it and/or modify 
// it under the terms of the GNU Lesser General Public License as 
// published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU 
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public 
// License along with this library; if not, write to:
// Free Software Foundation, Inc., 
// 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 

using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace DataFresh
{
	public class DataFreshConsole
	{
		private StringBuilder results = new StringBuilder();

		public NameValueCollection arguments = new NameValueCollection();
		public string connectionString = string.Empty;

		public string Results
		{
			get { return results.ToString(); }
		}

		public void Start(string[] args)
		{
			ConsoleWrite("DataFresh provided by Entropy Zero Consulting");

			if (args == null || args.Length < 2)
			{
				WriteUsage();
				return;
			}

			for (int i = 0; i < args.Length; i = i + 2)
			{
				this.arguments.Add(args[i].Replace("-", ""), args[i + 1]);
				//ConsoleWrite(args[i] + ": " +args[i+1]);
			}

			if (!CheckForRequiredArguments(this.arguments))
			{
				return;
			}

			connectionString = string.Format(@"user id={0};password={1};Initial Catalog={2};Data Source={3};",
			                                 this.arguments["u"],
			                                 this.arguments["p"],
			                                 this.arguments["d"],
			                                 this.arguments["s"]);

			SqlDataFresh dataFresh = new SqlDataFresh(connectionString, true);

			string snapshotPath = this.arguments["sp"];

			if (snapshotPath != null)
			{
				snapshotPath = snapshotPath.Replace("\"", "");
				if (!snapshotPath.EndsWith(@"\"))
				{
					snapshotPath += @"\";
				}

				ConsoleWrite("snapshotPath = {0}", snapshotPath);
				string fullPath = Path.GetFullPath(snapshotPath);
				ConsoleWrite("fullPath = {0}", fullPath);
				dataFresh.SnapshotPath = new DirectoryInfo(snapshotPath);
			}

			string command = this.arguments["c"].ToUpper();
			switch (command)
			{
				case "PREPARE":
					bool ignoreSnapshot = false;
					string ignoreSnapshotArgument = arguments["ignoresnapshot"];
					if(ignoreSnapshotArgument != null)
					{
						if(ignoreSnapshotArgument == "1")
						{
							ignoreSnapshot = true;
						}
					}
					dataFresh.PrepareDatabaseforDataFresh(!ignoreSnapshot);
					break;
				case "REFRESH":
					dataFresh.RefreshTheDatabase();
					break;
				case "FORCEREFRESH":
					dataFresh.RefreshTheEntireDatabase();
					break;
				case "REMOVE":
					dataFresh.RemoveDataFreshFromDatabase();
					break;
				case "SNAPSHOT":
					dataFresh.CreateSnapshot();
					break;
				case "FOO":
					//no nothing
					break;
				default:
					ConsoleWrite("Command '{0}' was not recognized", command);
					break;
			}
		}

		private void ConsoleWrite(string message, params object[] args)
		{
			results.AppendFormat(message, args);
			results.AppendFormat(Environment.NewLine);

			Console.Out.WriteLine(message, args);
		}

		private bool CheckForRequiredArguments(NameValueCollection myArgs)
		{
			if (
				myArgs == null ||
					myArgs["c"] == null ||
					myArgs["s"] == null ||
					myArgs["d"] == null ||
					myArgs["u"] == null ||
					myArgs["p"] == null
				)
			{
				ConsoleWrite("Missing required arguments.");
				WriteUsage();
				return false;
			}
			return true;
		}

		private void WriteUsage()
		{
			string usageText = @"
Usage:

  DataFreshUtil.exe 
    -c Command 
    -s Server 
    -d Database 
    -u Username 
    -p Password [options]

Options:

  -sp             specify path on server where snapshot files are located
  -ignoresnapshot 1: ignore snapshot during prepare
                  0: (default) will create snapshot

Commands:

  PREPARE         prepare the database for DataFresh
  REFRESH         refresh the database
  FORCEREFRESH    refresh the database ignoring the change tracking table
  REMOVE          remove the DataFresh elements from the database	
  SNAPSHOT        create a snapshot of your database
";

			ConsoleWrite(usageText);
		}

		public static DataFreshConsole Execute(string command, string username, string password, string server, string database, params string[] options)
		{
			string[] args = new string[]
				{
					"-c", command,
					"-u", username,
					"-p", password,
					"-s", server,
					"-d", database,
				};

			if (options != null && options.Length > 1)
			{
				string argsString = string.Join("|", args);
				string optionsString = string.Join("|", options);
				args = string.Format(argsString + "|" + optionsString).Split('|');
			}

			DataFreshConsole console = new DataFreshConsole();
			console.Start(args);
			return console;
		}
	}
}