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

namespace DataFresh
{
	public interface IDataFresh
	{
		/// <summary>
		/// prepare the database to use the dataFresh library
		/// </summary>
		void PrepareDatabaseForDataFresh();

		/// <summary>
		/// refresh the database to a known state
		/// </summary>
		void RefreshTheDatabase();

		/// <summary>
		/// determine if the database has been modified
		/// </summary>
		/// <returns>true if modified</returns>
		bool HasDatabaseBeenModified();

		/// <summary>
		/// remove the dataFresh objects from a database
		/// </summary>
		void RemoveDataFreshFromDatabase();

		/// <summary>
		/// refresh the database ignoring the dataFresh change tracking table.
		/// </summary>
		void RefreshTheEntireDatabase();
	}
}