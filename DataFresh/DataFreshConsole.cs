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
using System.Text;

namespace DataFresh
{
	public class DataFreshConsole
	{
		readonly StringBuilder results = new StringBuilder();

		public DataFreshConsole()
		{
			ConnectionString = string.Empty;
			Arguments = new NameValueCollection();
		}

		public string Results => results.ToString();

		public NameValueCollection Arguments { get; }

		public string ConnectionString { get; set; }

		public void Start(string[] args)
		{
			ConsoleWrite("DataFresh provided by Entropy Zero Consulting");

			if (args == null || args.Length < 2)
			{
				WriteUsage();
				return;
			}

			for (var i = 0; i < args.Length; i += 2)
				Arguments.Add(args[i].Replace("-", ""), args[i + 1]);

			if (!CheckForRequiredArguments(Arguments))
			{
				return;
			}

			ConnectionString =
				$@"user id={Arguments["u"]};password={Arguments["p"]};Initial Catalog={Arguments["d"]};Data Source={Arguments["s"]};";

			var dataFresh = new SqlDataFresh(ConnectionString, true);

			var command = Arguments["c"].ToUpper();
			switch (command)
			{
				case "PREPARE":
					var ignoreSnapshotArgument = Arguments["ignoresnapshot"];
					var ignoreSnapshot = (ignoreSnapshotArgument != null && ignoreSnapshotArgument == "1");
					dataFresh.PrepareDatabaseForDataFresh(!ignoreSnapshot);
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

		void ConsoleWrite(string message, params object[] args)
		{
			results.AppendFormat(message, args);
			results.AppendFormat(Environment.NewLine);

			Console.Out.WriteLine(message, args);
		}

		bool CheckForRequiredArguments(NameValueCollection myArgs)
		{
			if (myArgs?["c"] != null
				&& myArgs["s"] != null
				&& myArgs["d"] != null
				&& myArgs["u"] != null
				&& myArgs["p"] != null) return true;

			ConsoleWrite("Missing required arguments.");
			WriteUsage();
			return false;
		}

		void WriteUsage()
		{
			const string usageText = @"
Usage:

  DataFreshUtil.exe 
    -c Command 
    -s Server 
    -d Database 
    -u Username 
    -p Password [options]

Options:

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
			var args = new[]
				{
					"-c", command,
					"-u", username,
					"-p", password,
					"-s", server,
					"-d", database,
				};

			if (options != null && options.Length > 1)
			{
				var argsString = string.Join("|", args);
				var optionsString = string.Join("|", options);
				args = string.Format(argsString + "|" + optionsString).Split('|');
			}

			var console = new DataFreshConsole();
			console.Start(args);
			return console;
		}
	}
}