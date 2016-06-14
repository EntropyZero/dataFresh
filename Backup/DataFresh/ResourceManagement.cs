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
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace DataFresh
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public class AssemblyResourceEncrpytion : Attribute
	{
		private bool enryptionEnabled;
		private string enryptionKey = "nopassword";

		public AssemblyResourceEncrpytion()
		{
			enryptionEnabled = true;
		}
		
		public AssemblyResourceEncrpytion(bool enabled)
		{
			enryptionEnabled = enabled;
		}

		public virtual bool Enabled 
		{
			get 
			{
				return enryptionEnabled;
			}
			set
			{
				enryptionEnabled = value;
			}
		}
		
		public virtual string Key 
		{
			get 
			{
				return enryptionKey;
			}		
		}
	}
	
	public class ResourceManagement
	{
		public static bool EncryptionEnabled
		{
			get
			{
				object[] customAttributes = Assembly.GetCallingAssembly().GetCustomAttributes(typeof(AssemblyResourceEncrpytion), false);
				if(customAttributes != null && customAttributes.Length > 0)
				{
					AssemblyResourceEncrpytion enc = (AssemblyResourceEncrpytion) customAttributes[0];
					return enc.Enabled;
				}
				return false;
			}
		}

		public static string EncryptionKey
		{
			get
			{
				object[] customAttributes = Assembly.GetCallingAssembly().GetCustomAttributes(typeof(AssemblyResourceEncrpytion), false);
				if(customAttributes != null && customAttributes.Length > 0)
				{
					AssemblyResourceEncrpytion enc = (AssemblyResourceEncrpytion) customAttributes[0];
					return enc.Key;
				}
				throw new Exception("Encryption key was not set!");
			}
		}
		
		public static string GetDecryptedResource(string name)
		{
			byte[] dec = GetDecryptedResourceBytes(name);
			string decStr = new MemoryStream(dec).ToString();
			return decStr;
		}
		
		public static StreamReader GetDecryptedResourceStream(string name)
		{
			StreamReader reader = new StreamReader(new MemoryStream(GetDecryptedResourceBytes(name)));
			return reader;
		}

		public static byte[] GetDecryptedResourceBytes(string name)
		{
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
			int length = (int)stream.Length;
			byte[] bytes = new byte[length];
			stream.Read(bytes, 0, length);
			if(EncryptionEnabled)
			{
				return Decrypt(bytes, EncryptionKey);
			}
			else
			{
				return bytes;
			}
		}

		public static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
		{
			MemoryStream ms = new MemoryStream();
			Rijndael alg = Rijndael.Create();
			alg.Key = Key;
			alg.IV = IV;
			CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
			cs.Write(cipherData, 0, cipherData.Length);
			cs.Flush();
			cs.Close();
			byte[] decryptedData = ms.ToArray();
			return decryptedData;
		}

		public static byte[] Decrypt(byte[] cipherData, string Password)
		{
			PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
			                                                  new byte[]
			                                                  	{
			                                                  		0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64,
			                                                  		0x65, 0x76
			                                                  	});
			
			return Decrypt(cipherData, pdb.GetBytes(32), pdb.GetBytes(16));
		}
	}
}
