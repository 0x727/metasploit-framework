using System;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x020000A0 RID: 160
	public class SIP_ResponseCodes
	{
		// Token: 0x0400023F RID: 575
		public static readonly string x100_Trying = "100 Trying";

		// Token: 0x04000240 RID: 576
		public static readonly string x180_Ringing = "180 Ringing";

		// Token: 0x04000241 RID: 577
		public static readonly string x181_Call_Forwarded = "181 Call Is Being Forwarded";

		// Token: 0x04000242 RID: 578
		public static readonly string x182_Queued = "182 Queued";

		// Token: 0x04000243 RID: 579
		public static readonly string x183_Session_Progress = "183 Session Progress";

		// Token: 0x04000244 RID: 580
		public static readonly string x200_Ok = "200 OK";

		// Token: 0x04000245 RID: 581
		public static readonly string x202_Ok = "202 Accepted";

		// Token: 0x04000246 RID: 582
		public static readonly string x301_Ok = "301 Moved Permanently";

		// Token: 0x04000247 RID: 583
		public static readonly string x302_Ok = "302 Moved Temporarily";

		// Token: 0x04000248 RID: 584
		public static readonly string x400_Bad_Request = "400 Bad Request";

		// Token: 0x04000249 RID: 585
		public static readonly string x401_Unauthorized = "401 Unauthorized";

		// Token: 0x0400024A RID: 586
		public static readonly string x403_Forbidden = "403 Forbidden";

		// Token: 0x0400024B RID: 587
		public static readonly string x404_Not_Found = "404 Not Found";

		// Token: 0x0400024C RID: 588
		public static readonly string x405_Method_Not_Allowed = "405 Method Not Allowed";

		// Token: 0x0400024D RID: 589
		public static readonly string x406_Not_Acceptable = "406 Not Acceptable";

		// Token: 0x0400024E RID: 590
		public static readonly string x407_Proxy_Authentication_Required = "407 Proxy Authentication Required";

		// Token: 0x0400024F RID: 591
		public static readonly string x408_Request_Timeout = "408 Request Timeout";

		// Token: 0x04000250 RID: 592
		public static readonly string x410_Gone = "410 Gone";

		// Token: 0x04000251 RID: 593
		public static readonly string x412_Conditional_Request_Failed = "412 Conditional Request Failed";

		// Token: 0x04000252 RID: 594
		public static readonly string x413_Request_Entity_Too_Large = "413 Request Entity Too Large";

		// Token: 0x04000253 RID: 595
		public static readonly string x414_RequestURI_Too_Long = "414 Request-URI Too Long";

		// Token: 0x04000254 RID: 596
		public static readonly string x415_Unsupported_Media_Type = "415 Unsupported Media Type";

		// Token: 0x04000255 RID: 597
		public static readonly string x416_Unsupported_URI_Scheme = "416 Unsupported URI Scheme";

		// Token: 0x04000256 RID: 598
		public static readonly string x417_Unknown_Resource_Priority = "417 Unknown Resource-Priority";

		// Token: 0x04000257 RID: 599
		public static readonly string x420_Bad_Extension = "420 Bad Extension";

		// Token: 0x04000258 RID: 600
		public static readonly string x421_Extension_Required = "421 Extension Required";

		// Token: 0x04000259 RID: 601
		public static readonly string x422_Session_Interval_Too_Small = "422 Session Interval Too Small";

		// Token: 0x0400025A RID: 602
		public static readonly string x423_Interval_Too_Brief = "423 Interval Too Brief";

		// Token: 0x0400025B RID: 603
		public static readonly string x428_Use_Identity_Header = "428 Use Identity Header";

		// Token: 0x0400025C RID: 604
		public static readonly string x429_Provide_Referrer_Identity = "429 Provide Referrer Identity";

		// Token: 0x0400025D RID: 605
		public static readonly string x436_Bad_Identity_Info = "436 Bad Identity-Info";

		// Token: 0x0400025E RID: 606
		public static readonly string x437_Unsupported_Certificate = "437 Unsupported Certificate";

		// Token: 0x0400025F RID: 607
		public static readonly string x438_Invalid_Identity_Header = "438 Invalid Identity Header";

		// Token: 0x04000260 RID: 608
		public static readonly string x480_Temporarily_Unavailable = "480 Temporarily Unavailable";

		// Token: 0x04000261 RID: 609
		public static readonly string x481_Call_Transaction_Does_Not_Exist = "481 Call/Transaction Does Not Exist";

		// Token: 0x04000262 RID: 610
		public static readonly string x482_Loop_Detected = "482 Loop Detected";

		// Token: 0x04000263 RID: 611
		public static readonly string x483_Too_Many_Hops = "483 Too Many Hops";

		// Token: 0x04000264 RID: 612
		public static readonly string x484_Address_Incomplete = "484 Address Incomplete";

		// Token: 0x04000265 RID: 613
		public static readonly string x485_Ambiguous = "485 Ambiguous";

		// Token: 0x04000266 RID: 614
		public static readonly string x486_Busy_Here = "486 Busy Here";

		// Token: 0x04000267 RID: 615
		public static readonly string x487_Request_Terminated = "487 Request Terminated";

		// Token: 0x04000268 RID: 616
		public static readonly string x488_Not_Acceptable_Here = "488 Not Acceptable Here";

		// Token: 0x04000269 RID: 617
		public static readonly string x489_Bad_Event = "489 Bad Event";

		// Token: 0x0400026A RID: 618
		public static readonly string x491_Request_Pending = "491 Request Pending";

		// Token: 0x0400026B RID: 619
		public static readonly string x493_Undecipherable = "493 Undecipherable";

		// Token: 0x0400026C RID: 620
		public static readonly string x494_Security_Agreement_Required = "494 Security Agreement Required";

		// Token: 0x0400026D RID: 621
		public static readonly string x500_Server_Internal_Error = "500 Server Internal Error";

		// Token: 0x0400026E RID: 622
		public static readonly string x501_Not_Implemented = "501 Not Implemented";

		// Token: 0x0400026F RID: 623
		public static readonly string x502_Bad_Gateway = "502 Bad Gateway";

		// Token: 0x04000270 RID: 624
		public static readonly string x503_Service_Unavailable = "503 Service Unavailable";

		// Token: 0x04000271 RID: 625
		public static readonly string x504_Timeout = "504 Server Time-out";

		// Token: 0x04000272 RID: 626
		public static readonly string x504_Version_Not_Supported = "505 Version Not Supported";

		// Token: 0x04000273 RID: 627
		public static readonly string x513_Message_Too_Large = "513 Message Too Large";

		// Token: 0x04000274 RID: 628
		public static readonly string x580_Precondition_Failure = "580 Precondition Failure";

		// Token: 0x04000275 RID: 629
		public static readonly string x600_Busy_Everywhere = "600 Busy Everywhere";

		// Token: 0x04000276 RID: 630
		public static readonly string x603_Decline = "603 Decline";

		// Token: 0x04000277 RID: 631
		public static readonly string x604_Does_Not_Exist_Anywhere = "604 Does Not Exist Anywhere";

		// Token: 0x04000278 RID: 632
		public static readonly string x606_Not_Acceptable = "606 Not Acceptable";
	}
}
