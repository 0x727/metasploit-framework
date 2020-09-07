using System;

namespace LumiSoft.Net.MIME
{
	// Token: 0x02000113 RID: 275
	public class MIME_MediaTypes
	{
		// Token: 0x020002C2 RID: 706
		public class Application
		{
			// Token: 0x04000A3F RID: 2623
			public static readonly string octet_stream = "application/octet-stream";

			// Token: 0x04000A40 RID: 2624
			public static readonly string pdf = "application/pdf";

			// Token: 0x04000A41 RID: 2625
			public static readonly string sdp = "application/sdp";

			// Token: 0x04000A42 RID: 2626
			public static readonly string xml = "application/xml";

			// Token: 0x04000A43 RID: 2627
			public static readonly string zip = "application/zip";

			// Token: 0x04000A44 RID: 2628
			public static readonly string x_pkcs7_signature = "application/x-pkcs7-signature";

			// Token: 0x04000A45 RID: 2629
			public static readonly string pkcs7_mime = "application/pkcs7-mime";
		}

		// Token: 0x020002C3 RID: 707
		public class Image
		{
			// Token: 0x04000A46 RID: 2630
			public static readonly string gif = "image/gif";

			// Token: 0x04000A47 RID: 2631
			public static readonly string jpeg = "image/jpeg";

			// Token: 0x04000A48 RID: 2632
			public static readonly string tiff = "image/tiff";
		}

		// Token: 0x020002C4 RID: 708
		public class Text
		{
			// Token: 0x04000A49 RID: 2633
			public static readonly string calendar = "text/calendar";

			// Token: 0x04000A4A RID: 2634
			public static readonly string css = "text/css";

			// Token: 0x04000A4B RID: 2635
			public static readonly string html = "text/html";

			// Token: 0x04000A4C RID: 2636
			public static readonly string plain = "text/plain";

			// Token: 0x04000A4D RID: 2637
			public static readonly string rfc822_headers = "text/rfc822-headers";

			// Token: 0x04000A4E RID: 2638
			public static readonly string richtext = "text/richtext";

			// Token: 0x04000A4F RID: 2639
			public static readonly string xml = "text/xml";
		}

		// Token: 0x020002C5 RID: 709
		public class Multipart
		{
			// Token: 0x04000A50 RID: 2640
			public static readonly string alternative = "multipart/alternative";

			// Token: 0x04000A51 RID: 2641
			public static readonly string digest = "multipart/digest";

			// Token: 0x04000A52 RID: 2642
			public static readonly string encrypted = "multipart/digest";

			// Token: 0x04000A53 RID: 2643
			public static readonly string form_data = "multipart/form-data";

			// Token: 0x04000A54 RID: 2644
			public static readonly string mixed = "multipart/mixed";

			// Token: 0x04000A55 RID: 2645
			public static readonly string parallel = "multipart/parallel";

			// Token: 0x04000A56 RID: 2646
			public static readonly string related = "multipart/related";

			// Token: 0x04000A57 RID: 2647
			public static readonly string report = "multipart/report";

			// Token: 0x04000A58 RID: 2648
			public static readonly string signed = "multipart/signed";

			// Token: 0x04000A59 RID: 2649
			public static readonly string voice_message = "multipart/voice-message";
		}

		// Token: 0x020002C6 RID: 710
		public class Message
		{
			// Token: 0x04000A5A RID: 2650
			public static readonly string rfc822 = "message/rfc822";

			// Token: 0x04000A5B RID: 2651
			public static readonly string disposition_notification = "message/disposition-notification";

			// Token: 0x04000A5C RID: 2652
			public static readonly string delivery_status = "message/delivery-status";
		}
	}
}
