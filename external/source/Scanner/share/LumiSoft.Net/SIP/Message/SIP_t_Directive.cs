using System;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000048 RID: 72
	public class SIP_t_Directive : SIP_t_Value
	{
		// Token: 0x06000261 RID: 609 RVA: 0x0000E528 File Offset: 0x0000D528
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000E558 File Offset: 0x0000D558
		public override void Parse(StringReader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			string text = reader.ReadWord();
			bool flag2 = text == null;
			if (flag2)
			{
				throw new SIP_ParseException("'directive' value is missing !");
			}
			bool flag3 = text.ToLower() == "proxy";
			if (flag3)
			{
				this.m_Directive = SIP_t_Directive.DirectiveType.Proxy;
			}
			else
			{
				bool flag4 = text.ToLower() == "redirect";
				if (flag4)
				{
					this.m_Directive = SIP_t_Directive.DirectiveType.Redirect;
				}
				else
				{
					bool flag5 = text.ToLower() == "cancel";
					if (flag5)
					{
						this.m_Directive = SIP_t_Directive.DirectiveType.Cancel;
					}
					else
					{
						bool flag6 = text.ToLower() == "no-cancel";
						if (flag6)
						{
							this.m_Directive = SIP_t_Directive.DirectiveType.NoCancel;
						}
						else
						{
							bool flag7 = text.ToLower() == "fork";
							if (flag7)
							{
								this.m_Directive = SIP_t_Directive.DirectiveType.Fork;
							}
							else
							{
								bool flag8 = text.ToLower() == "no-fork";
								if (flag8)
								{
									this.m_Directive = SIP_t_Directive.DirectiveType.NoFork;
								}
								else
								{
									bool flag9 = text.ToLower() == "recurse";
									if (flag9)
									{
										this.m_Directive = SIP_t_Directive.DirectiveType.Recurse;
									}
									else
									{
										bool flag10 = text.ToLower() == "no-recurse";
										if (flag10)
										{
											this.m_Directive = SIP_t_Directive.DirectiveType.NoRecurse;
										}
										else
										{
											bool flag11 = text.ToLower() == "parallel";
											if (flag11)
											{
												this.m_Directive = SIP_t_Directive.DirectiveType.Parallel;
											}
											else
											{
												bool flag12 = text.ToLower() == "sequential";
												if (flag12)
												{
													this.m_Directive = SIP_t_Directive.DirectiveType.Sequential;
												}
												else
												{
													bool flag13 = text.ToLower() == "queue";
													if (flag13)
													{
														this.m_Directive = SIP_t_Directive.DirectiveType.Queue;
													}
													else
													{
														bool flag14 = text.ToLower() == "no-queue";
														if (!flag14)
														{
															throw new SIP_ParseException("Invalid 'directive' value !");
														}
														this.m_Directive = SIP_t_Directive.DirectiveType.NoQueue;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000E748 File Offset: 0x0000D748
		public override string ToStringValue()
		{
			bool flag = this.m_Directive == SIP_t_Directive.DirectiveType.Proxy;
			string result;
			if (flag)
			{
				result = "proxy";
			}
			else
			{
				bool flag2 = this.m_Directive == SIP_t_Directive.DirectiveType.Redirect;
				if (flag2)
				{
					result = "redirect";
				}
				else
				{
					bool flag3 = this.m_Directive == SIP_t_Directive.DirectiveType.Cancel;
					if (flag3)
					{
						result = "cancel";
					}
					else
					{
						bool flag4 = this.m_Directive == SIP_t_Directive.DirectiveType.NoCancel;
						if (flag4)
						{
							result = "no-cancel";
						}
						else
						{
							bool flag5 = this.m_Directive == SIP_t_Directive.DirectiveType.Fork;
							if (flag5)
							{
								result = "fork";
							}
							else
							{
								bool flag6 = this.m_Directive == SIP_t_Directive.DirectiveType.NoFork;
								if (flag6)
								{
									result = "no-fork";
								}
								else
								{
									bool flag7 = this.m_Directive == SIP_t_Directive.DirectiveType.Recurse;
									if (flag7)
									{
										result = "recurse";
									}
									else
									{
										bool flag8 = this.m_Directive == SIP_t_Directive.DirectiveType.NoRecurse;
										if (flag8)
										{
											result = "no-recurse";
										}
										else
										{
											bool flag9 = this.m_Directive == SIP_t_Directive.DirectiveType.Parallel;
											if (flag9)
											{
												result = "parallel";
											}
											else
											{
												bool flag10 = this.m_Directive == SIP_t_Directive.DirectiveType.Sequential;
												if (flag10)
												{
													result = "sequential";
												}
												else
												{
													bool flag11 = this.m_Directive == SIP_t_Directive.DirectiveType.Queue;
													if (flag11)
													{
														result = "queue";
													}
													else
													{
														bool flag12 = this.m_Directive == SIP_t_Directive.DirectiveType.NoQueue;
														if (!flag12)
														{
															throw new ArgumentException("Invalid property Directive value, this should never happen !");
														}
														result = "no-queue";
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000264 RID: 612 RVA: 0x0000E898 File Offset: 0x0000D898
		// (set) Token: 0x06000265 RID: 613 RVA: 0x0000E8B0 File Offset: 0x0000D8B0
		public SIP_t_Directive.DirectiveType Directive
		{
			get
			{
				return this.m_Directive;
			}
			set
			{
				this.m_Directive = value;
			}
		}

		// Token: 0x04000106 RID: 262
		private SIP_t_Directive.DirectiveType m_Directive = SIP_t_Directive.DirectiveType.Fork;

		// Token: 0x02000284 RID: 644
		public enum DirectiveType
		{
			// Token: 0x04000972 RID: 2418
			Proxy,
			// Token: 0x04000973 RID: 2419
			Redirect,
			// Token: 0x04000974 RID: 2420
			Cancel,
			// Token: 0x04000975 RID: 2421
			NoCancel,
			// Token: 0x04000976 RID: 2422
			Fork,
			// Token: 0x04000977 RID: 2423
			NoFork,
			// Token: 0x04000978 RID: 2424
			Recurse,
			// Token: 0x04000979 RID: 2425
			NoRecurse,
			// Token: 0x0400097A RID: 2426
			Parallel,
			// Token: 0x0400097B RID: 2427
			Sequential,
			// Token: 0x0400097C RID: 2428
			Queue,
			// Token: 0x0400097D RID: 2429
			NoQueue
		}
	}
}
