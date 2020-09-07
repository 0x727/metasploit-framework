using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Threading;
using LumiSoft.Net.IO;
using LumiSoft.Net.TCP;

namespace LumiSoft.Net.FTP.Client
{
	// Token: 0x02000236 RID: 566
	public class FTP_Client : TCP_Client
	{
		// Token: 0x0600147A RID: 5242 RVA: 0x0007F9EC File Offset: 0x0007E9EC
		public override void Dispose()
		{
			lock (this)
			{
				base.Dispose();
				this.m_pDataConnectionIP = null;
			}
		}

		// Token: 0x0600147B RID: 5243 RVA: 0x0007FA34 File Offset: 0x0007EA34
		public override void Disconnect()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("FTP client is not connected.");
			}
			try
			{
				base.WriteLine("QUIT");
			}
			catch
			{
			}
			try
			{
				base.Disconnect();
			}
			catch
			{
			}
			this.m_pExtCapabilities = null;
			this.m_pAuthdUserIdentity = null;
			bool flag2 = this.m_pDataConnection != null;
			if (flag2)
			{
				this.m_pDataConnection.Dispose();
				this.m_pDataConnection = null;
			}
		}

		// Token: 0x0600147C RID: 5244 RVA: 0x0007FAEC File Offset: 0x0007EAEC
		public void Reinitialize()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			base.WriteLine("REIN");
			string[] array = this.ReadResponse();
			bool flag2 = !array[0].StartsWith("2");
			if (flag2)
			{
				throw new FTP_ClientException(array[0]);
			}
		}

		// Token: 0x0600147D RID: 5245 RVA: 0x0007FB60 File Offset: 0x0007EB60
		public void Authenticate(string userName, string password)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isAuthenticated = base.IsAuthenticated;
			if (isAuthenticated)
			{
				throw new InvalidOperationException("Session is already authenticated.");
			}
			bool flag2 = string.IsNullOrEmpty(userName);
			if (flag2)
			{
				throw new ArgumentNullException("userName");
			}
			bool flag3 = password == null;
			if (flag3)
			{
				password = "";
			}
			base.WriteLine("USER " + userName);
			string[] array = this.ReadResponse();
			bool flag4 = array[0].StartsWith("331");
			if (!flag4)
			{
				throw new FTP_ClientException(array[0]);
			}
			base.WriteLine("PASS " + password);
			array = this.ReadResponse();
			bool flag5 = !array[0].StartsWith("230");
			if (flag5)
			{
				throw new FTP_ClientException(array[0]);
			}
			this.m_pAuthdUserIdentity = new GenericIdentity(userName, "ftp-user/pass");
		}

		// Token: 0x0600147E RID: 5246 RVA: 0x0007FC68 File Offset: 0x0007EC68
		public void Noop()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			base.WriteLine("NOOP");
			string[] array = this.ReadResponse();
			bool flag2 = !array[0].StartsWith("2");
			if (flag2)
			{
				throw new FTP_ClientException(array[0]);
			}
		}

		// Token: 0x0600147F RID: 5247 RVA: 0x0007FCDC File Offset: 0x0007ECDC
		public void Abort()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			base.WriteLine("ABOR");
			string text = base.ReadLine();
			bool flag2 = !text.StartsWith("2");
			if (flag2)
			{
				throw new FTP_ClientException(text);
			}
		}

		// Token: 0x06001480 RID: 5248 RVA: 0x0007FD4C File Offset: 0x0007ED4C
		public string GetCurrentDir()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			base.WriteLine("PWD");
			string[] array = this.ReadResponse();
			bool flag2 = !array[0].StartsWith("2");
			if (flag2)
			{
				throw new FTP_ClientException(array[0]);
			}
			StringReader stringReader = new StringReader(array[0]);
			stringReader.ReadWord();
			return stringReader.ReadWord();
		}

		// Token: 0x06001481 RID: 5249 RVA: 0x0007FDE0 File Offset: 0x0007EDE0
		public void SetCurrentDir(string path)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = path == null;
			if (flag2)
			{
				throw new ArgumentNullException("path");
			}
			bool flag3 = path == "";
			if (flag3)
			{
				throw new ArgumentException("Argumnet 'path' must be specified.");
			}
			base.WriteLine("CWD " + path);
			string[] array = this.ReadResponse();
			bool flag4 = !array[0].StartsWith("2");
			if (flag4)
			{
				throw new FTP_ClientException(array[0]);
			}
		}

		// Token: 0x06001482 RID: 5250 RVA: 0x0007FE90 File Offset: 0x0007EE90
		public FTP_ListItem[] GetList()
		{
			return this.GetList(null);
		}

		// Token: 0x06001483 RID: 5251 RVA: 0x0007FEAC File Offset: 0x0007EEAC
		public FTP_ListItem[] GetList(string path)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isActive = this.m_pDataConnection.IsActive;
			if (isActive)
			{
				throw new InvalidOperationException("There is already active read/write operation on data connection.");
			}
			List<FTP_ListItem> list = new List<FTP_ListItem>();
			this.SetTransferType(TransferType.Binary);
			bool flag2 = this.m_TransferMode == FTP_TransferMode.Passive;
			if (flag2)
			{
				this.Pasv();
			}
			else
			{
				this.Port();
			}
			bool flag3 = false;
			foreach (string text in this.m_pExtCapabilities)
			{
				bool flag4 = text.ToLower().StartsWith("mlsd");
				if (flag4)
				{
					flag3 = true;
					break;
				}
			}
			bool flag5 = flag3;
			if (flag5)
			{
				bool flag6 = string.IsNullOrEmpty(path);
				if (flag6)
				{
					base.WriteLine("MLSD");
				}
				else
				{
					base.WriteLine("MLSD " + path);
				}
				string[] array = this.ReadResponse();
				bool flag7 = !array[0].StartsWith("1");
				if (flag7)
				{
					throw new FTP_ClientException(array[0]);
				}
				MemoryStream memoryStream = new MemoryStream();
				this.m_pDataConnection.ReadAll(memoryStream);
				array = this.ReadResponse();
				bool flag8 = !array[0].StartsWith("2");
				if (flag8)
				{
					throw new FTP_ClientException(array[0]);
				}
				byte[] buffer = new byte[8000];
				memoryStream.Position = 0L;
				SmartStream smartStream = new SmartStream(memoryStream, true);
				SmartStream.ReadLineAsyncOP readLineAsyncOP;
				for (;;)
				{
					readLineAsyncOP = new SmartStream.ReadLineAsyncOP(buffer, SizeExceededAction.JunkAndThrowException);
					smartStream.ReadLine(readLineAsyncOP, false);
					bool flag9 = readLineAsyncOP.Error != null;
					if (flag9)
					{
						break;
					}
					string lineUtf = readLineAsyncOP.LineUtf8;
					bool flag10 = lineUtf == null;
					if (flag10)
					{
						goto Block_11;
					}
					string[] array2 = lineUtf.Substring(0, lineUtf.LastIndexOf(';')).Split(new char[]
					{
						';'
					});
					string name = lineUtf.Substring(lineUtf.LastIndexOf(';') + 1).Trim();
					string a = "";
					long size = 0L;
					DateTime modified = DateTime.MinValue;
					foreach (string text2 in array2)
					{
						string[] array4 = text2.Split(new char[]
						{
							'='
						});
						bool flag11 = array4[0].ToLower() == "type";
						if (flag11)
						{
							a = array4[1].ToLower();
						}
						else
						{
							bool flag12 = array4[0].ToLower() == "size";
							if (flag12)
							{
								size = Convert.ToInt64(array4[1]);
							}
							else
							{
								bool flag13 = array4[0].ToLower() == "modify";
								if (flag13)
								{
									modified = DateTime.ParseExact(array4[1], "yyyyMMddHHmmss", DateTimeFormatInfo.InvariantInfo);
								}
							}
						}
					}
					bool flag14 = a == "dir";
					if (flag14)
					{
						list.Add(new FTP_ListItem(name, 0L, modified, true));
					}
					else
					{
						bool flag15 = a == "file";
						if (flag15)
						{
							list.Add(new FTP_ListItem(name, size, modified, false));
						}
					}
				}
				throw readLineAsyncOP.Error;
				Block_11:;
			}
			else
			{
				bool flag16 = string.IsNullOrEmpty(path);
				if (flag16)
				{
					base.WriteLine("LIST");
				}
				else
				{
					base.WriteLine("LIST " + path);
				}
				string[] array5 = this.ReadResponse();
				bool flag17 = !array5[0].StartsWith("1");
				if (flag17)
				{
					throw new FTP_ClientException(array5[0]);
				}
				MemoryStream memoryStream2 = new MemoryStream();
				this.m_pDataConnection.ReadAll(memoryStream2);
				array5 = this.ReadResponse();
				bool flag18 = !array5[0].StartsWith("2");
				if (flag18)
				{
					throw new FTP_ClientException(array5[0]);
				}
				memoryStream2.Position = 0L;
				SmartStream smartStream2 = new SmartStream(memoryStream2, true);
				string[] formats = new string[]
				{
					"M-d-yy h:mmtt",
					"MM-dd-yy HH:mm"
				};
				string[] formats2 = new string[]
				{
					"MMM d H:mm",
					"MMM d yyyy"
				};
				SmartStream.ReadLineAsyncOP readLineAsyncOP2 = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
				for (;;)
				{
					smartStream2.ReadLine(readLineAsyncOP2, false);
					bool flag19 = readLineAsyncOP2.Error != null;
					if (flag19)
					{
						break;
					}
					bool flag20 = readLineAsyncOP2.BytesInBuffer == 0;
					if (flag20)
					{
						goto Block_22;
					}
					string lineUtf2 = readLineAsyncOP2.LineUtf8;
					string a2 = "unix";
					bool flag21 = lineUtf2 != null;
					if (flag21)
					{
						StringReader stringReader = new StringReader(lineUtf2);
						DateTime dateTime;
						bool flag22 = DateTime.TryParseExact(stringReader.ReadWord() + " " + stringReader.ReadWord(), formats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out dateTime);
						if (flag22)
						{
							a2 = "win";
						}
					}
					try
					{
						bool flag23 = a2 == "win";
						if (flag23)
						{
							StringReader stringReader2 = new StringReader(lineUtf2);
							DateTime modified2 = DateTime.ParseExact(stringReader2.ReadWord() + " " + stringReader2.ReadWord(), formats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
							stringReader2.ReadToFirstChar();
							bool flag24 = stringReader2.StartsWith("<dir>", false);
							if (flag24)
							{
								stringReader2.ReadSpecifiedLength(5);
								stringReader2.ReadToFirstChar();
								list.Add(new FTP_ListItem(stringReader2.ReadToEnd(), 0L, modified2, true));
							}
							else
							{
								long size2 = Convert.ToInt64(stringReader2.ReadWord());
								stringReader2.ReadToFirstChar();
								list.Add(new FTP_ListItem(stringReader2.ReadToEnd(), size2, modified2, false));
							}
						}
						else
						{
							StringReader stringReader3 = new StringReader(lineUtf2);
							string text3 = stringReader3.ReadWord();
							stringReader3.ReadWord();
							stringReader3.ReadWord();
							stringReader3.ReadWord();
							long size3 = Convert.ToInt64(stringReader3.ReadWord());
							DateTime modified3 = DateTime.ParseExact(string.Concat(new string[]
							{
								stringReader3.ReadWord(),
								" ",
								stringReader3.ReadWord(),
								" ",
								stringReader3.ReadWord()
							}), formats2, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
							stringReader3.ReadToFirstChar();
							string text4 = stringReader3.ReadToEnd();
							bool flag25 = text4 != "." && text4 != "..";
							if (flag25)
							{
								bool flag26 = text3.StartsWith("d");
								if (flag26)
								{
									list.Add(new FTP_ListItem(text4, 0L, modified3, true));
								}
								else
								{
									list.Add(new FTP_ListItem(text4, size3, modified3, false));
								}
							}
						}
					}
					catch
					{
					}
				}
				throw readLineAsyncOP2.Error;
				Block_22:;
			}
			return list.ToArray();
		}

		// Token: 0x06001484 RID: 5252 RVA: 0x000805AC File Offset: 0x0007F5AC
		public void GetFile(string path, string storePath)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isActive = this.m_pDataConnection.IsActive;
			if (isActive)
			{
				throw new InvalidOperationException("There is already active read/write operation on data connection.");
			}
			bool flag2 = path == null;
			if (flag2)
			{
				throw new ArgumentNullException("path");
			}
			bool flag3 = path == "";
			if (flag3)
			{
				throw new ArgumentException("Argument 'path' value must be specified.");
			}
			bool flag4 = storePath == null;
			if (flag4)
			{
				throw new ArgumentNullException("storePath");
			}
			bool flag5 = storePath == "";
			if (flag5)
			{
				throw new ArgumentException("Argument 'storePath' value must be specified.");
			}
			using (FileStream fileStream = File.Create(storePath))
			{
				this.GetFile(path, fileStream);
			}
		}

		// Token: 0x06001485 RID: 5253 RVA: 0x000806A4 File Offset: 0x0007F6A4
		public void GetFile(string path, Stream stream)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isActive = this.m_pDataConnection.IsActive;
			if (isActive)
			{
				throw new InvalidOperationException("There is already active read/write operation on data connection.");
			}
			bool flag2 = path == null;
			if (flag2)
			{
				throw new ArgumentNullException("path");
			}
			bool flag3 = path == "";
			if (flag3)
			{
				throw new ArgumentException("Argument 'path' value must be specified.");
			}
			bool flag4 = stream == null;
			if (flag4)
			{
				throw new ArgumentNullException("stream");
			}
			this.SetTransferType(TransferType.Binary);
			bool flag5 = this.m_TransferMode == FTP_TransferMode.Passive;
			if (flag5)
			{
				this.Pasv();
			}
			else
			{
				this.Port();
			}
			base.WriteLine("RETR " + path);
			string[] array = this.ReadResponse();
			bool flag6 = !array[0].StartsWith("1");
			if (flag6)
			{
				throw new FTP_ClientException(array[0]);
			}
			this.m_pDataConnection.ReadAll(stream);
			array = this.ReadResponse();
			bool flag7 = !array[0].StartsWith("2");
			if (flag7)
			{
				throw new FTP_ClientException(array[0]);
			}
		}

		// Token: 0x06001486 RID: 5254 RVA: 0x000807E4 File Offset: 0x0007F7E4
		public void AppendToFile(string path, Stream stream)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isActive = this.m_pDataConnection.IsActive;
			if (isActive)
			{
				throw new InvalidOperationException("There is already active read/write operation on data connection.");
			}
			bool flag2 = path == null;
			if (flag2)
			{
				throw new ArgumentNullException("path");
			}
			bool flag3 = path == "";
			if (flag3)
			{
				throw new ArgumentException("Argument 'path' value must be specified.");
			}
			bool flag4 = stream == null;
			if (flag4)
			{
				throw new ArgumentNullException("stream");
			}
			this.SetTransferType(TransferType.Binary);
			bool flag5 = this.m_TransferMode == FTP_TransferMode.Passive;
			if (flag5)
			{
				this.Pasv();
			}
			else
			{
				this.Port();
			}
			base.WriteLine("APPE " + path);
			string[] array = this.ReadResponse();
			bool flag6 = !array[0].StartsWith("1");
			if (flag6)
			{
				throw new FTP_ClientException(array[0]);
			}
			this.m_pDataConnection.WriteAll(stream);
			array = this.ReadResponse();
			bool flag7 = !array[0].StartsWith("2");
			if (flag7)
			{
				throw new FTP_ClientException(array[0]);
			}
		}

		// Token: 0x06001487 RID: 5255 RVA: 0x00080924 File Offset: 0x0007F924
		public void StoreFile(string path, string sourcePath)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isActive = this.m_pDataConnection.IsActive;
			if (isActive)
			{
				throw new InvalidOperationException("There is already active read/write operation on data connection.");
			}
			bool flag2 = path == null;
			if (flag2)
			{
				throw new ArgumentNullException("path");
			}
			bool flag3 = path == "";
			if (flag3)
			{
				throw new ArgumentException("Argument 'path' value must be specified.");
			}
			bool flag4 = sourcePath == null;
			if (flag4)
			{
				throw new ArgumentNullException("sourcePath");
			}
			bool flag5 = sourcePath == "";
			if (flag5)
			{
				throw new ArgumentException("Argument 'sourcePath' value must be specified.");
			}
			using (FileStream fileStream = File.OpenRead(sourcePath))
			{
				this.StoreFile(path, fileStream);
			}
		}

		// Token: 0x06001488 RID: 5256 RVA: 0x00080A1C File Offset: 0x0007FA1C
		public void StoreFile(string path, Stream stream)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isActive = this.m_pDataConnection.IsActive;
			if (isActive)
			{
				throw new InvalidOperationException("There is already active read/write operation on data connection.");
			}
			bool flag2 = path == null;
			if (flag2)
			{
				throw new ArgumentNullException("path");
			}
			bool flag3 = path == "";
			if (flag3)
			{
				throw new ArgumentException("Argument 'path' value must be specified.");
			}
			bool flag4 = stream == null;
			if (flag4)
			{
				throw new ArgumentNullException("stream");
			}
			this.SetTransferType(TransferType.Binary);
			bool flag5 = this.m_TransferMode == FTP_TransferMode.Passive;
			if (flag5)
			{
				this.Pasv();
			}
			else
			{
				this.Port();
			}
			base.WriteLine("STOR " + path);
			string[] array = this.ReadResponse();
			bool flag6 = !array[0].StartsWith("1");
			if (flag6)
			{
				throw new FTP_ClientException(array[0]);
			}
			this.m_pDataConnection.WriteAll(stream);
			array = this.ReadResponse();
			bool flag7 = !array[0].StartsWith("2");
			if (flag7)
			{
				throw new FTP_ClientException(array[0]);
			}
		}

		// Token: 0x06001489 RID: 5257 RVA: 0x00080B5C File Offset: 0x0007FB5C
		public void DeleteFile(string path)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = path == null;
			if (flag2)
			{
				throw new ArgumentNullException("path");
			}
			bool flag3 = path == "";
			if (flag3)
			{
				throw new ArgumentException("Argument 'path' value must be specified.");
			}
			base.WriteLine("DELE " + path);
			string text = base.ReadLine();
			bool flag4 = !text.StartsWith("250");
			if (flag4)
			{
				throw new FTP_ClientException(text);
			}
		}

		// Token: 0x0600148A RID: 5258 RVA: 0x00080C08 File Offset: 0x0007FC08
		public void Rename(string fromPath, string toPath)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = fromPath == null;
			if (flag2)
			{
				throw new ArgumentNullException("fromPath");
			}
			bool flag3 = fromPath == "";
			if (flag3)
			{
				throw new ArgumentException("Argument 'fromPath' value must be specified.");
			}
			bool flag4 = toPath == null;
			if (flag4)
			{
				throw new ArgumentNullException("toPath");
			}
			bool flag5 = toPath == "";
			if (flag5)
			{
				throw new ArgumentException("Argument 'toPath' value must be specified.");
			}
			base.WriteLine("RNFR " + fromPath);
			string text = base.ReadLine();
			bool flag6 = !text.StartsWith("350");
			if (flag6)
			{
				throw new FTP_ClientException(text);
			}
			base.WriteLine("RNTO " + toPath);
			text = base.ReadLine();
			bool flag7 = !text.StartsWith("250");
			if (flag7)
			{
				throw new FTP_ClientException(text);
			}
		}

		// Token: 0x0600148B RID: 5259 RVA: 0x00080D1C File Offset: 0x0007FD1C
		public void CreateDirectory(string path)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = path == null;
			if (flag2)
			{
				throw new ArgumentNullException("path");
			}
			bool flag3 = path == "";
			if (flag3)
			{
				throw new ArgumentException("Argument 'path' value must be specified.");
			}
			base.WriteLine("MKD " + path);
			string text = base.ReadLine();
			bool flag4 = !text.StartsWith("257");
			if (flag4)
			{
				throw new FTP_ClientException(text);
			}
		}

		// Token: 0x0600148C RID: 5260 RVA: 0x00080DC8 File Offset: 0x0007FDC8
		public void DeleteDirectory(string path)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = path == null;
			if (flag2)
			{
				throw new ArgumentNullException("path");
			}
			bool flag3 = path == "";
			if (flag3)
			{
				throw new ArgumentException("Argument 'path' value must be specified.");
			}
			base.WriteLine("RMD " + path);
			string text = base.ReadLine();
			bool flag4 = !text.StartsWith("250");
			if (flag4)
			{
				throw new FTP_ClientException(text);
			}
		}

		// Token: 0x0600148D RID: 5261 RVA: 0x00080E74 File Offset: 0x0007FE74
		private void SetTransferType(TransferType type)
		{
			bool flag = type == TransferType.Ascii;
			if (flag)
			{
				base.WriteLine("TYPE A");
			}
			else
			{
				bool flag2 = type == TransferType.Binary;
				if (!flag2)
				{
					throw new ArgumentException("Not supported argument 'type' value '" + type.ToString() + "'.");
				}
				base.WriteLine("TYPE I");
			}
			string[] array = this.ReadResponse();
			bool flag3 = !array[0].StartsWith("2");
			if (flag3)
			{
				throw new FTP_ClientException(array[0]);
			}
		}

		// Token: 0x0600148E RID: 5262 RVA: 0x00080EFC File Offset: 0x0007FEFC
		private void Port()
		{
			string[] array = null;
			foreach (IPAddress ipaddress in Dns.GetHostAddresses(""))
			{
				bool flag = ipaddress.AddressFamily == this.m_pDataConnection.LocalEndPoint.AddressFamily;
				if (flag)
				{
					base.WriteLine(string.Concat(new object[]
					{
						"PORT ",
						ipaddress.ToString().Replace(".", ","),
						",",
						this.m_pDataConnection.LocalEndPoint.Port >> 8,
						",",
						this.m_pDataConnection.LocalEndPoint.Port & 255
					}));
					array = this.ReadResponse();
					bool flag2 = array[0].StartsWith("2");
					if (flag2)
					{
						this.m_pDataConnection.SwitchToActive();
						return;
					}
				}
			}
			throw new FTP_ClientException(array[0]);
		}

		// Token: 0x0600148F RID: 5263 RVA: 0x00081004 File Offset: 0x00080004
		private void Pasv()
		{
			base.WriteLine("PASV");
			string[] array = this.ReadResponse();
			bool flag = !array[0].StartsWith("227");
			if (flag)
			{
				throw new FTP_ClientException(array[0]);
			}
			string[] array2 = array[0].Substring(array[0].IndexOf("(") + 1, array[0].IndexOf(")") - array[0].IndexOf("(") - 1).Split(new char[]
			{
				','
			});
			this.m_pDataConnection.SwitchToPassive(new IPEndPoint(IPAddress.Parse(string.Concat(new string[]
			{
				array2[0],
				".",
				array2[1],
				".",
				array2[2],
				".",
				array2[3]
			})), Convert.ToInt32(array2[4]) << 8 | Convert.ToInt32(array2[5])));
		}

		// Token: 0x06001490 RID: 5264 RVA: 0x000810EC File Offset: 0x000800EC
		private string[] ReadResponse()
		{
			List<string> list = new List<string>();
			string text = base.ReadLine();
			bool flag = text == null;
			if (flag)
			{
				throw new Exception("Remote host disconnected connection unexpectedly.");
			}
			bool flag2 = text.Length >= 4 && text[3] == '-';
			if (flag2)
			{
				string str = text.Substring(0, 3);
				list.Add(text);
				for (;;)
				{
					text = base.ReadLine();
					bool flag3 = text == null;
					if (flag3)
					{
						break;
					}
					bool flag4 = text.StartsWith(str + " ");
					if (flag4)
					{
						goto Block_5;
					}
					list.Add(text);
				}
				throw new Exception("Remote host disconnected connection unexpectedly.");
				Block_5:
				list.Add(text);
			}
			else
			{
				list.Add(text);
			}
			return list.ToArray();
		}

		// Token: 0x06001491 RID: 5265 RVA: 0x000811B8 File Offset: 0x000801B8
		protected override void OnConnected()
		{
			this.m_pDataConnection = new FTP_Client.DataConnection(this);
			string text = base.ReadLine();
			bool flag = text.StartsWith("220");
			if (flag)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(text.Substring(4));
				while (text.StartsWith("220-"))
				{
					text = base.ReadLine();
					stringBuilder.AppendLine(text.Substring(4));
				}
				this.m_GreetingText = stringBuilder.ToString();
				base.WriteLine("FEAT");
				text = base.ReadLine();
				this.m_pExtCapabilities = new List<string>();
				bool flag2 = text.StartsWith("211");
				if (flag2)
				{
					text = base.ReadLine();
					while (text.StartsWith(" "))
					{
						this.m_pExtCapabilities.Add(text.Trim());
						text = base.ReadLine();
					}
				}
				return;
			}
			throw new FTP_ClientException(text);
		}

		// Token: 0x170006AD RID: 1709
		// (get) Token: 0x06001492 RID: 5266 RVA: 0x000812A4 File Offset: 0x000802A4
		// (set) Token: 0x06001493 RID: 5267 RVA: 0x000812D8 File Offset: 0x000802D8
		public FTP_TransferMode TransferMode
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_TransferMode;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				this.m_TransferMode = value;
			}
		}

		// Token: 0x170006AE RID: 1710
		// (get) Token: 0x06001494 RID: 5268 RVA: 0x0008130C File Offset: 0x0008030C
		// (set) Token: 0x06001495 RID: 5269 RVA: 0x00081340 File Offset: 0x00080340
		public IPAddress DataIP
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pDataConnectionIP;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				this.m_pDataConnectionIP = value;
				bool isConnected = this.IsConnected;
				if (isConnected)
				{
					this.m_pDataConnection.CleanUpSocket();
				}
			}
		}

		// Token: 0x170006AF RID: 1711
		// (get) Token: 0x06001496 RID: 5270 RVA: 0x0008138C File Offset: 0x0008038C
		// (set) Token: 0x06001497 RID: 5271 RVA: 0x000813C0 File Offset: 0x000803C0
		public PortRange DataPortRange
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pDataPortRange;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				this.m_pDataPortRange = value;
				bool isConnected = this.IsConnected;
				if (isConnected)
				{
					this.m_pDataConnection.CleanUpSocket();
				}
			}
		}

		// Token: 0x170006B0 RID: 1712
		// (get) Token: 0x06001498 RID: 5272 RVA: 0x0008140C File Offset: 0x0008040C
		public string GreetingText
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = !this.IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("You must connect first.");
				}
				return this.m_GreetingText;
			}
		}

		// Token: 0x170006B1 RID: 1713
		// (get) Token: 0x06001499 RID: 5273 RVA: 0x0008145C File Offset: 0x0008045C
		public string[] ExtenededCapabilities
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = !this.IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("You must connect first.");
				}
				return this.m_pExtCapabilities.ToArray();
			}
		}

		// Token: 0x170006B2 RID: 1714
		// (get) Token: 0x0600149A RID: 5274 RVA: 0x000814B0 File Offset: 0x000804B0
		public override GenericIdentity AuthenticatedUserIdentity
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = !this.IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("You must connect first.");
				}
				return this.m_pAuthdUserIdentity;
			}
		}

		// Token: 0x040007FF RID: 2047
		private FTP_TransferMode m_TransferMode = FTP_TransferMode.Passive;

		// Token: 0x04000800 RID: 2048
		private IPAddress m_pDataConnectionIP = null;

		// Token: 0x04000801 RID: 2049
		private PortRange m_pDataPortRange = null;

		// Token: 0x04000802 RID: 2050
		private string m_GreetingText = "";

		// Token: 0x04000803 RID: 2051
		private List<string> m_pExtCapabilities = null;

		// Token: 0x04000804 RID: 2052
		private GenericIdentity m_pAuthdUserIdentity = null;

		// Token: 0x04000805 RID: 2053
		private FTP_Client.DataConnection m_pDataConnection = null;

		// Token: 0x0200037D RID: 893
		private class DataConnection : IDisposable
		{
			// Token: 0x06001B8B RID: 7051 RVA: 0x000AA14B File Offset: 0x000A914B
			public DataConnection(FTP_Client owner)
			{
				this.m_pOwner = owner;
				this.CreateSocket();
			}

			// Token: 0x06001B8C RID: 7052 RVA: 0x000AA188 File Offset: 0x000A9188
			public void Dispose()
			{
				bool flag = this.m_pSocket != null;
				if (flag)
				{
					this.m_pSocket.Close();
					this.m_pSocket = null;
				}
				this.m_pOwner = null;
			}

			// Token: 0x06001B8D RID: 7053 RVA: 0x000AA1C0 File Offset: 0x000A91C0
			public void SwitchToActive()
			{
				this.m_pSocket.Listen(1);
				this.m_TransferMode = FTP_TransferMode.Active;
				this.m_pOwner.LogAddText("FTP data channel switched to Active mode, listening FTP server connect to '" + this.m_pSocket.LocalEndPoint.ToString() + "'.");
			}

			// Token: 0x06001B8E RID: 7054 RVA: 0x000AA210 File Offset: 0x000A9210
			public void SwitchToPassive(IPEndPoint remoteEP)
			{
				bool flag = remoteEP == null;
				if (flag)
				{
					throw new ArgumentNullException("remoteEP");
				}
				this.m_pOwner.LogAddText("FTP data channel switched to Passive mode, connecting to FTP server '" + remoteEP.ToString() + "'.");
				this.m_pSocket.Connect(remoteEP);
				this.m_TransferMode = FTP_TransferMode.Passive;
				this.m_pOwner.LogAddText(string.Concat(new string[]
				{
					"FTP Passive data channel established, localEP='",
					this.m_pSocket.LocalEndPoint.ToString(),
					"' remoteEP='",
					this.m_pSocket.RemoteEndPoint.ToString(),
					"'."
				}));
			}

			// Token: 0x06001B8F RID: 7055 RVA: 0x000AA2BC File Offset: 0x000A92BC
			public void ReadAll(Stream stream)
			{
				bool flag = stream == null;
				if (flag)
				{
					throw new ArgumentNullException("stream");
				}
				this.m_IsActive = true;
				try
				{
					bool flag2 = this.m_TransferMode == FTP_TransferMode.Active;
					if (flag2)
					{
						using (NetworkStream networkStream = this.WaitFtpServerToConnect(20))
						{
							long num = this.TransferStream(networkStream, stream);
							this.m_pOwner.LogAddRead(num, "Data connection readed " + num + " bytes.");
						}
					}
					else
					{
						bool flag3 = this.m_TransferMode == FTP_TransferMode.Passive;
						if (flag3)
						{
							using (NetworkStream networkStream2 = new NetworkStream(this.m_pSocket, true))
							{
								long num2 = this.TransferStream(networkStream2, stream);
								this.m_pOwner.LogAddRead(num2, "Data connection readed " + num2 + " bytes.");
							}
						}
					}
				}
				finally
				{
					this.m_IsActive = false;
					this.CleanUpSocket();
				}
			}

			// Token: 0x06001B90 RID: 7056 RVA: 0x000AA3D8 File Offset: 0x000A93D8
			public void WriteAll(Stream stream)
			{
				bool flag = stream == null;
				if (flag)
				{
					throw new ArgumentNullException("stream");
				}
				try
				{
					bool flag2 = this.m_TransferMode == FTP_TransferMode.Active;
					if (flag2)
					{
						using (NetworkStream networkStream = this.WaitFtpServerToConnect(20))
						{
							long num = this.TransferStream(stream, networkStream);
							this.m_pOwner.LogAddWrite(num, "Data connection wrote " + num + " bytes.");
						}
					}
					else
					{
						bool flag3 = this.m_TransferMode == FTP_TransferMode.Passive;
						if (flag3)
						{
							using (NetworkStream networkStream2 = new NetworkStream(this.m_pSocket, true))
							{
								long num2 = this.TransferStream(stream, networkStream2);
								this.m_pOwner.LogAddWrite(num2, "Data connection wrote " + num2 + " bytes.");
							}
						}
					}
				}
				finally
				{
					this.m_IsActive = false;
					this.CleanUpSocket();
				}
			}

			// Token: 0x06001B91 RID: 7057 RVA: 0x000AA4EC File Offset: 0x000A94EC
			private NetworkStream WaitFtpServerToConnect(int waitTime)
			{
				NetworkStream result;
				try
				{
					this.m_pOwner.LogAddText("FTP Active data channel waiting FTP server connect to '" + this.m_pSocket.LocalEndPoint.ToString() + "'.");
					DateTime now = DateTime.Now;
					while (!this.m_pSocket.Poll(0, SelectMode.SelectRead))
					{
						Thread.Sleep(50);
						bool flag = now.AddSeconds((double)waitTime) < DateTime.Now;
						if (flag)
						{
							this.m_pOwner.LogAddText("FTP server didn't connect during expected time.");
							throw new IOException("FTP server didn't connect during expected time.");
						}
					}
					Socket socket = this.m_pSocket.Accept();
					this.m_pOwner.LogAddText(string.Concat(new string[]
					{
						"FTP Active data channel established, localEP='",
						socket.LocalEndPoint.ToString(),
						"' remoteEP='",
						socket.RemoteEndPoint.ToString(),
						"'."
					}));
					result = new NetworkStream(socket, true);
				}
				finally
				{
					this.CleanUpSocket();
				}
				return result;
			}

			// Token: 0x06001B92 RID: 7058 RVA: 0x000AA600 File Offset: 0x000A9600
			private void CreateSocket()
			{
				bool flag = this.m_pOwner.LocalEndPoint.Address.AddressFamily == AddressFamily.InterNetwork;
				if (flag)
				{
					this.m_pSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				}
				else
				{
					this.m_pSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
				}
				this.m_pSocket.SendTimeout = this.m_pOwner.Timeout;
				this.m_pSocket.ReceiveTimeout = this.m_pOwner.Timeout;
				bool flag2 = this.m_pOwner.DataPortRange == null;
				int port;
				if (flag2)
				{
					port = 0;
				}
				else
				{
					bool flag3 = this.m_ActivePort == -1 || this.m_ActivePort + 1 > this.m_pOwner.DataPortRange.End;
					if (flag3)
					{
						this.m_ActivePort = this.m_pOwner.DataPortRange.Start;
					}
					else
					{
						this.m_ActivePort++;
					}
					port = this.m_ActivePort;
				}
				bool flag4 = this.m_pOwner.DataIP == null || this.m_pOwner.DataIP == IPAddress.Any;
				if (flag4)
				{
					this.m_pSocket.Bind(new IPEndPoint(this.m_pOwner.LocalEndPoint.Address, port));
				}
				else
				{
					this.m_pSocket.Bind(new IPEndPoint(this.m_pOwner.DataIP, port));
				}
			}

			// Token: 0x06001B93 RID: 7059 RVA: 0x000AA760 File Offset: 0x000A9760
			public void CleanUpSocket()
			{
				bool flag = this.m_pSocket != null;
				if (flag)
				{
					this.m_pSocket.Close();
				}
				this.CreateSocket();
			}

			// Token: 0x06001B94 RID: 7060 RVA: 0x000AA790 File Offset: 0x000A9790
			private long TransferStream(Stream source, Stream target)
			{
				long num = 0L;
				byte[] array = new byte[32000];
				for (;;)
				{
					int num2 = source.Read(array, 0, array.Length);
					bool flag = num2 == 0;
					if (flag)
					{
						break;
					}
					target.Write(array, 0, num2);
					num += (long)num2;
					this.m_LastActivity = DateTime.Now;
				}
				return num;
			}

			// Token: 0x1700088F RID: 2191
			// (get) Token: 0x06001B95 RID: 7061 RVA: 0x000AA7F0 File Offset: 0x000A97F0
			public IPEndPoint LocalEndPoint
			{
				get
				{
					return (IPEndPoint)this.m_pSocket.LocalEndPoint;
				}
			}

			// Token: 0x17000890 RID: 2192
			// (get) Token: 0x06001B96 RID: 7062 RVA: 0x000AA814 File Offset: 0x000A9814
			public DateTime LastActivity
			{
				get
				{
					return this.m_LastActivity;
				}
			}

			// Token: 0x17000891 RID: 2193
			// (get) Token: 0x06001B97 RID: 7063 RVA: 0x000AA82C File Offset: 0x000A982C
			public bool IsActive
			{
				get
				{
					return this.m_IsActive;
				}
			}

			// Token: 0x04000CCB RID: 3275
			private FTP_Client m_pOwner = null;

			// Token: 0x04000CCC RID: 3276
			private Socket m_pSocket = null;

			// Token: 0x04000CCD RID: 3277
			private int m_ActivePort = -1;

			// Token: 0x04000CCE RID: 3278
			private FTP_TransferMode m_TransferMode = FTP_TransferMode.Active;

			// Token: 0x04000CCF RID: 3279
			private DateTime m_LastActivity;

			// Token: 0x04000CD0 RID: 3280
			private bool m_IsActive = false;
		}
	}
}
