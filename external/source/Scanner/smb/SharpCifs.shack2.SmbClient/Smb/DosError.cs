﻿using System;

namespace SharpCifs.Smb
{
	// Token: 0x02000073 RID: 115
	public static class DosError
	{
		// Token: 0x040000D1 RID: 209
		public static int[][] DosErrorCodes = new int[][]
		{
			new int[2],
			new int[]
			{
				65537,
				-1073741822
			},
			new int[]
			{
				65538,
				-1073741822
			},
			new int[]
			{
				131073,
				-1073741809
			},
			new int[]
			{
				131074,
				-1073741718
			},
			new int[]
			{
				196609,
				-1073741766
			},
			new int[]
			{
				196610,
				-1073741621
			},
			new int[]
			{
				262146,
				-1073741622
			},
			new int[]
			{
				327681,
				-1073741790
			},
			new int[]
			{
				327682,
				-1073741811
			},
			new int[]
			{
				393217,
				-1073741816
			},
			new int[]
			{
				393218,
				-1073741620
			},
			new int[]
			{
				524289,
				-1073741670
			},
			new int[]
			{
				1245187,
				-1073741662
			},
			new int[]
			{
				1376259,
				-1073741805
			},
			new int[]
			{
				2031617,
				-1073741823
			},
			new int[]
			{
				2031619,
				-1073741823
			},
			new int[]
			{
				2097153,
				-1073741757
			},
			new int[]
			{
				2097155,
				-1073741757
			},
			new int[]
			{
				2162691,
				-1073741740
			},
			new int[]
			{
				2555907,
				-1073741697
			},
			new int[]
			{
				3407873,
				-1073741635
			},
			new int[]
			{
				4390913,
				-1073741620
			},
			new int[]
			{
				4653057,
				-1073741616
			},
			new int[]
			{
				5242881,
				-1073741771
			},
			new int[]
			{
				5701633,
				-1073741821
			},
			new int[]
			{
				5898242,
				-1073741618
			},
			new int[]
			{
				5963778,
				-1073741811
			},
			new int[]
			{
				7143425,
				-1073741493
			},
			new int[]
			{
				8060929,
				-1073741773
			},
			new int[]
			{
				9502721,
				-1073741567
			},
			new int[]
			{
				11993089,
				-1073741771
			},
			new int[]
			{
				15138817,
				-1073741653
			},
			new int[]
			{
				15204353,
				-1073741647
			},
			new int[]
			{
				15269889,
				-1073741648
			},
			new int[]
			{
				15335425,
				-1073741802
			},
			new int[]
			{
				146735106,
				-1073741421
			},
			new int[]
			{
				146800642,
				-1073741712
			},
			new int[]
			{
				146866178,
				-1073741713
			},
			new int[]
			{
				146931714,
				-1073741711
			}
		};

		// Token: 0x040000D2 RID: 210
		public static string[] DosErrorMessages = new string[]
		{
			"The operation completed successfully.",
			"Incorrect function.",
			"Incorrect function.",
			"The system cannot find the file specified.",
			"Bad password.",
			"The system cannot find the path specified.",
			"reserved",
			"The client does not have the necessary access rights to perform the requested function.",
			"Access is denied.",
			"The TID specified was invalid.",
			"The handle is invalid.",
			"The network name cannot be found.",
			"Not enough storage is available to process this command.",
			"The media is write protected.",
			"The device is not ready.",
			"A device attached to the system is not functioning.",
			"A device attached to the system is not functioning.",
			"The process cannot access the file because it is being used by another process.",
			"The process cannot access the file because it is being used by another process.",
			"The process cannot access the file because another process has locked a portion of the file.",
			"The disk is full.",
			"A duplicate name exists on the network.",
			"The network name cannot be found.",
			"ERRnomoreconn.",
			"The file exists.",
			"The parameter is incorrect.",
			"Too many Uids active on this session.",
			"The Uid is not known as a valid user identifier on this session.",
			"The pipe has been ended.",
			"The filename, directory name, or volume label syntax is incorrect.",
			"The directory is not empty.",
			"Cannot create a file when that file already exists.",
			"All pipe instances are busy.",
			"The pipe is being closed.",
			"No process is on the other end of the pipe.",
			"More data is available.",
			"This user account has expired.",
			"The user is not allowed to log on from this workstation.",
			"The user is not allowed to log on at this time.",
			"The password of this user has expired."
		};
	}
}
