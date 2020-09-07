using System;

namespace SharpCifs.Smb
{
	// Token: 0x0200007F RID: 127
	public static class NtStatus
	{
		// Token: 0x04000103 RID: 259
		public const int NtStatusOk = 0;

		// Token: 0x04000104 RID: 260
		public const int NtStatusUnsuccessful = -1073741823;

		// Token: 0x04000105 RID: 261
		public const int NtStatusNotImplemented = -1073741822;

		// Token: 0x04000106 RID: 262
		public const int NtStatusInvalidInfoClass = -1073741821;

		// Token: 0x04000107 RID: 263
		public const int NtStatusAccessViolation = -1073741819;

		// Token: 0x04000108 RID: 264
		public const int NtStatusInvalidHandle = -1073741816;

		// Token: 0x04000109 RID: 265
		public const int NtStatusInvalidParameter = -1073741811;

		// Token: 0x0400010A RID: 266
		public const int NtStatusNoSuchDevice = -1073741810;

		// Token: 0x0400010B RID: 267
		public const int NtStatusNoSuchFile = -1073741809;

		// Token: 0x0400010C RID: 268
		public const int NtStatusMoreProcessingRequired = -1073741802;

		// Token: 0x0400010D RID: 269
		public const int NtStatusAccessDenied = -1073741790;

		// Token: 0x0400010E RID: 270
		public const int NtStatusBufferTooSmall = -1073741789;

		// Token: 0x0400010F RID: 271
		public const int NtStatusObjectNameInvalid = -1073741773;

		// Token: 0x04000110 RID: 272
		public const int NtStatusObjectNameNotFound = -1073741772;

		// Token: 0x04000111 RID: 273
		public const int NtStatusObjectNameCollision = -1073741771;

		// Token: 0x04000112 RID: 274
		public const int NtStatusPortDisconnected = -1073741769;

		// Token: 0x04000113 RID: 275
		public const int NtStatusObjectPathInvalid = -1073741767;

		// Token: 0x04000114 RID: 276
		public const int NtStatusObjectPathNotFound = -1073741766;

		// Token: 0x04000115 RID: 277
		public const int NtStatusObjectPathSyntaxBad = -1073741765;

		// Token: 0x04000116 RID: 278
		public const int NtStatusSharingViolation = -1073741757;

		// Token: 0x04000117 RID: 279
		public const int NtStatusDeletePending = -1073741738;

		// Token: 0x04000118 RID: 280
		public const int NtStatusNoLogonServers = -1073741730;

		// Token: 0x04000119 RID: 281
		public const int NtStatusUserExists = -1073741725;

		// Token: 0x0400011A RID: 282
		public const int NtStatusNoSuchUser = -1073741724;

		// Token: 0x0400011B RID: 283
		public const int NtStatusWrongPassword = -1073741718;

		// Token: 0x0400011C RID: 284
		public const int NtStatusLogonFailure = -1073741715;

		// Token: 0x0400011D RID: 285
		public const int NtStatusAccountRestriction = -1073741714;

		// Token: 0x0400011E RID: 286
		public const int NtStatusInvalidLogonHours = -1073741713;

		// Token: 0x0400011F RID: 287
		public const int NtStatusInvalidWorkstation = -1073741712;

		// Token: 0x04000120 RID: 288
		public const int NtStatusPasswordExpired = -1073741711;

		// Token: 0x04000121 RID: 289
		public const int NtStatusAccountDisabled = -1073741710;

		// Token: 0x04000122 RID: 290
		public const int NtStatusNoneMapped = -1073741709;

		// Token: 0x04000123 RID: 291
		public const int NtStatusInvalidSid = -1073741704;

		// Token: 0x04000124 RID: 292
		public const int NtStatusInstanceNotAvailable = -1073741653;

		// Token: 0x04000125 RID: 293
		public const int NtStatusPipeNotAvailable = -1073741652;

		// Token: 0x04000126 RID: 294
		public const int NtStatusInvalidPipeState = -1073741651;

		// Token: 0x04000127 RID: 295
		public const int NtStatusPipeBusy = -1073741650;

		// Token: 0x04000128 RID: 296
		public const int NtStatusPipeDisconnected = -1073741648;

		// Token: 0x04000129 RID: 297
		public const int NtStatusPipeClosing = -1073741647;

		// Token: 0x0400012A RID: 298
		public const int NtStatusPipeListening = -1073741645;

		// Token: 0x0400012B RID: 299
		public const int NtStatusFileIsADirectory = -1073741638;

		// Token: 0x0400012C RID: 300
		public const int NtStatusDuplicateName = -1073741635;

		// Token: 0x0400012D RID: 301
		public const int NtStatusNetworkNameDeleted = -1073741623;

		// Token: 0x0400012E RID: 302
		public const int NtStatusNetworkAccessDenied = -1073741622;

		// Token: 0x0400012F RID: 303
		public const int NtStatusBadNetworkName = -1073741620;

		// Token: 0x04000130 RID: 304
		public const int NtStatusRequestNotAccepted = -1073741616;

		// Token: 0x04000131 RID: 305
		public const int NtStatusCantAccessDomainInfo = -1073741606;

		// Token: 0x04000132 RID: 306
		public const int NtStatusNoSuchDomain = -1073741601;

		// Token: 0x04000133 RID: 307
		public const int NtStatusNotADirectory = -1073741565;

		// Token: 0x04000134 RID: 308
		public const int NtStatusCannotDelete = -1073741535;

		// Token: 0x04000135 RID: 309
		public const int NtStatusInvalidComputerName = -1073741534;

		// Token: 0x04000136 RID: 310
		public const int NtStatusPipeBroken = -1073741493;

		// Token: 0x04000137 RID: 311
		public const int NtStatusNoSuchAlias = -1073741487;

		// Token: 0x04000138 RID: 312
		public const int NtStatusLogonTypeNotGranted = -1073741477;

		// Token: 0x04000139 RID: 313
		public const int NtStatusNoTrustSamAccount = -1073741429;

		// Token: 0x0400013A RID: 314
		public const int NtStatusTrustedDomainFailure = -1073741428;

		// Token: 0x0400013B RID: 315
		public const int NtStatusNologonWorkstationTrustAccount = -1073741415;

		// Token: 0x0400013C RID: 316
		public const int NtStatusPasswordMustChange = -1073741276;

		// Token: 0x0400013D RID: 317
		public const int NtStatusNotFound = -1073741275;

		// Token: 0x0400013E RID: 318
		public const int NtStatusAccountLockedOut = -1073741260;

		// Token: 0x0400013F RID: 319
		public const int NtStatusPathNotCovered = -1073741225;

		// Token: 0x04000140 RID: 320
		public const int NtStatusIoReparseTagNotHandled = -1073741191;

		// Token: 0x04000141 RID: 321
		public static int[] NtStatusCodes = new int[]
		{
			0,
			-1073741823,
			-1073741822,
			-1073741821,
			-1073741819,
			-1073741816,
			-1073741811,
			-1073741810,
			-1073741809,
			-1073741802,
			-1073741790,
			-1073741789,
			-1073741773,
			-1073741772,
			-1073741771,
			-1073741769,
			-1073741767,
			-1073741766,
			-1073741765,
			-1073741757,
			-1073741738,
			-1073741730,
			-1073741725,
			-1073741724,
			-1073741718,
			-1073741715,
			-1073741714,
			-1073741713,
			-1073741712,
			-1073741711,
			-1073741710,
			-1073741709,
			-1073741704,
			-1073741653,
			-1073741652,
			-1073741651,
			-1073741650,
			-1073741648,
			-1073741647,
			-1073741645,
			-1073741638,
			-1073741635,
			-1073741623,
			-1073741622,
			-1073741620,
			-1073741616,
			-1073741606,
			-1073741601,
			-1073741565,
			-1073741535,
			-1073741534,
			-1073741493,
			-1073741487,
			-1073741477,
			-1073741429,
			-1073741428,
			-1073741415,
			-1073741276,
			-1073741275,
			-1073741260,
			-1073741225,
			-1073741191
		};

		// Token: 0x04000142 RID: 322
		public static string[] NtStatusMessages = new string[]
		{
			"The operation completed successfully.",
			"A device attached to the system is not functioning.",
			"Incorrect function.",
			"The parameter is incorrect.",
			"Invalid access to memory location.",
			"The handle is invalid.",
			"The parameter is incorrect.",
			"The system cannot find the file specified.",
			"The system cannot find the file specified.",
			"More data is available.",
			"Access is denied.",
			"The data area passed to a system call is too small.",
			"The filename, directory name, or volume label syntax is incorrect.",
			"The system cannot find the file specified.",
			"Cannot create a file when that file already exists.",
			"The handle is invalid.",
			"The specified path is invalid.",
			"The system cannot find the path specified.",
			"The specified path is invalid.",
			"The process cannot access the file because it is being used by another process.",
			"Access is denied.",
			"There are currently no logon servers available to service the logon request.",
			"The specified user already exists.",
			"The specified user does not exist.",
			"The specified network password is not correct.",
			"Logon failure: unknown user name or bad password.",
			"Logon failure: user account restriction.",
			"Logon failure: account logon time restriction violation.",
			"Logon failure: user not allowed to log on to this computer.",
			"Logon failure: the specified account password has expired.",
			"Logon failure: account currently disabled.",
			"No mapping between account names and security IDs was done.",
			"The security ID structure is invalid.",
			"All pipe instances are busy.",
			"All pipe instances are busy.",
			"The pipe state is invalid.",
			"All pipe instances are busy.",
			"No process is on the other end of the pipe.",
			"The pipe is being closed.",
			"Waiting for a process to open the other end of the pipe.",
			"Access is denied.",
			"A duplicate name exists on the network.",
			"The specified network name is no longer available.",
			"Network access is denied.",
			"The network name cannot be found.",
			"No more connections can be made to this remote computer at this time because there are already as many connections as the computer can accept.",
			"Indicates a Windows NT Server could not be contacted or that objects within the domain are protected such that necessary information could not be retrieved.",
			"The specified domain did not exist.",
			"The directory name is invalid.",
			"Access is denied.",
			"The format of the specified computer name is invalid.",
			"The pipe has been ended.",
			"The specified local group does not exist.",
			"Logon failure: the user has not been granted the requested logon type at this computer.",
			"The SAM database on the Windows NT Server does not have a computer account for this workstation trust relationship.",
			"The trust relationship between the primary domain and the trusted domain failed.",
			"The account used is a Computer Account. Use your global user account or local user account to access this server.",
			"The user must change his password before he logs on the first time.",
			"NT_STATUS_NOT_FOUND",
			"The referenced account is currently locked out and may not be logged on to.",
			"The remote system is not reachable by the transport.",
			"NT_STATUS_IO_REPARSE_TAG_NOT_HANDLED"
		};
	}
}
