using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

using SNETCracker.Model;
using Amib.Threading;

namespace Tools
{
    public class ThreadTool
    {
        private HashSet<string> list_username = new HashSet<string>();
        private HashSet<string> list_password = new HashSet<string>();
        private HashSet<string> list_target = new HashSet<string>();
        private HashSet<string> list_import_target = new HashSet<string>();
        private HashSet<string> list_ip_break = new HashSet<string>();
        private HashSet<string> list_ip_user_break = new HashSet<string>();
        private HashSet<string> list_success_username = new HashSet<string>();


        public int retryCount = 0;
        public int maxThread = 50;
        public int timeOut = 5;
        public int logLevel = 2;

        public Boolean crackerOneCount = true;//只检查一个账户
        public int successCount = 0;
        //public long creackerSumCount = 0;
        public long allCrackCount = 0;
        //private Boolean notAutoSelectDic = true;
        //public string[] servicesName = { };
        //public string[] servicesPort = { };

        //public long scanPortsSumCount = 0;

        //private Dictionary<string, ServiceModel> services = new Dictionary<string, ServiceModel>();//服务列表
        //private Dictionary<string, HashSet<string>> dics = new Dictionary<string, HashSet<string>>();//字典列表
        public ConcurrentBag<string> list_cracker = new ConcurrentBag<string>();

        public void LogWarning(string strLog)
        {
            if (logLevel < 3) return;
            Console.WriteLine("Warning: " + strLog);
        }

        public void LogInfo(string strLog)
        {
            if (logLevel < 2) return;
            Console.WriteLine("Info: " + strLog);
        }

        public void LogError(string strLog)
        {
            if (logLevel < 1) return;
            Console.WriteLine("Error: " + strLog);
        }

        private void crackerService(string crakerstring, string username, string password)
        {
            try
            {
                string[] crakers = crakerstring.Split(':');
                string ip = crakers[0];
                int port = int.Parse(crakers[1]);
                string serviceName = crakers[2];

                //跳过无法检查的IP列表，提高效率
                //多线程安全
                lock (list_ip_break)
                {
                    if (list_ip_break.Contains(ip + port))
                    {
                        LogWarning(ip + "-" + port + "跳过检查!");
                        Interlocked.Increment(ref allCrackCount);
                        return;
                    }
                }
                //多线程安全
                lock (list_ip_user_break)
                {
                    //跳过已经检查的列表，提高效率
                    if (list_ip_user_break.Contains(ip + port + username))
                    {
                        LogWarning(ip + "-" + port + "-" + username + "跳过检查!");
                        Interlocked.Increment(ref allCrackCount);
                        return;
                    }
                }


                if (true)
                {
                    Object[] pramars = { ip, port, username, password, timeOut, retryCount };

                    int count = 0;
                    Server server = new Server();

                    while (count <= this.retryCount)
                    {
                        count++;
                        try
                        {
                            //跳过检查，多线程安全
                            bool cconce = false;
                            lock (list_success_username)
                            {
                                cconce = list_success_username.Contains(ip + serviceName + port);
                            }
                            if (this.crackerOneCount && cconce)
                            {
                                break;
                            }
                            Stopwatch sw = new Stopwatch();
                            sw.Start();

                            {
                                CrackService cs = null;
                                if (cs == null)
                                {
                                    Type type = Type.GetType("SNETCracker.Model.Crack" + serviceName);
                                    if (type != null)
                                    {
                                        cs = (CrackService)Activator.CreateInstance(type);

                                    }
                                }
                                server = cs.crack(ip, port, username, password, timeOut);

                            }

                            sw.Stop();
                            server.userTime = sw.ElapsedMilliseconds;

                        }
                        catch (IPBreakException ie)
                        {
                            string breakip = ie.Message;
                            lock (list_ip_break)
                            {
                                if (!list_ip_break.Contains(breakip))
                                {
                                    list_ip_break.Add(breakip);
                                }
                            }
                        }
                        catch (IPUserBreakException ie)
                        {
                            lock (list_ip_user_break)
                            {
                                string breakipuser = ie.Message;
                                if (!list_ip_user_break.Contains(breakipuser))
                                {
                                    list_ip_user_break.Add(breakipuser);
                                }
                            }
                        }
                        catch (TimeoutException te)
                        {
                            continue;
                        }
                        catch (Exception e)
                        {
                            string logInfo = "检查" + ip + ":" + port.ToString() + ":" + serviceName + "登录发生异常！" + e.Message;
                            LogWarning(logInfo);
                            //FileTool.log(logInfo + e.StackTrace);
                        }
                        break;
                    }
                    if (server.isSuccess)
                    {
                        bool success = false;
                        lock (list_success_username)
                        {
                            success = list_success_username.Contains(ip + serviceName + port + username);
                        }
                        if (!success)
                        {
                            if (this.crackerOneCount)
                            {
                                //多线程安全
                                lock (list_success_username)
                                {
                                    success = list_success_username.Contains(ip + serviceName + port);
                                }
                            }
                            if (!success)
                            {
                                //多线程安全
                                lock (list_success_username)
                                {
                                    list_success_username.Add(ip + serviceName + port);
                                    list_success_username.Add(ip + serviceName + port + username);
                                }
                                Interlocked.Increment(ref successCount);
                                //addItemToListView(successCount, ip, serviceName, port, username, password, server.banner, server.userTime);
                                String sinfo = ip + "-----" + serviceName + "----" + username + "----" + password + "----" + server.banner + "----成功！";
                                LogInfo(sinfo);
                                //FileTool.AppendLogToFile(Directory.GetCurrentDirectory() + "/logs/success-" + DateTime.Now.ToString("yyyy-MM-dd") + ".log", sinfo);

                            }

                        }

                    }
                    else
                    {
                        LogWarning(ip + "-----" + serviceName + "----" + username + "----" + password + "失败！");
                    }

                }

            }
            catch (Exception e)
            {
                LogError(e.Message + e.StackTrace);
            }
            Interlocked.Increment(ref allCrackCount);
        }

        private Boolean initTargetAndDic(string strServiceName, string txt_target, string txt_username, string txt_password)
        {
            List<Interval> lstIPInterval = HostTool.GetHosts(txt_target);

            this.list_target.Clear();
            foreach (Interval interval in lstIPInterval)
            {

                if (interval.st != interval.ed)
                {
                    for (long l = interval.st; l <= interval.ed; l++)
                    {
                        this.list_target.Add(HostTool.LongToIP(l));
                    }
                }
                else
                {
                    this.list_target.Add(HostTool.LongToIP(interval.st));
                }
            }

            //if ("".Equals(txt_target))
            //{
            //    LogError("请设置需要检查的目标的IP地址或域名！");
            //    return false;
            //}
            ////else if (this.services_list.CheckedItems.Count <= 0)
            ////{
            ////    MessageBox.Show("请选择需要检查服务！");
            ////    return false;
            ////}
            //else
            //{
            //    if (!"".Equals(txt_target))
            //    {

            //        bool isTrue = Regex.IsMatch(txt_target, "^([\\w\\-\\.]{1,100}[a-zA-Z]{1,8})$|^(\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3})$");
            //        if (isTrue)
            //        {
            //            this.list_target.Clear();
            //            this.list_target.Add(txt_target);
            //        }
            //        else
            //        {
            //            isTrue = Regex.IsMatch(txt_target, "^\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\-\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}$");

            //            if (isTrue)
            //            {
            //                this.list_target.Clear();
            //                string[] ips = txt_target.Split('-');
            //                if (ips.Length == 2)
            //                {
            //                    string startip = ips[0];
            //                    string endip = ips[1];
            //                    string[] startips = startip.Split('.');
            //                    string[] endips = endip.Split('.');
            //                    if (startips.Length == 4 && endips.Length == 4)
            //                    {
            //                        int startips_3 = int.Parse(startips[2]);
            //                        int endips_3 = int.Parse(endips[2]);
            //                        int startips_4 = int.Parse(startips[3]);
            //                        int endips_4 = int.Parse(endips[3]);

            //                        if (endips_3 >= startips_3 && endips_3 <= 255 && endips_4 <= 255)
            //                        {

            //                            for (int i = startips_3; i <= endips_3; i++)
            //                            {

            //                                if (startips_3 == endips_3)
            //                                {
            //                                    if (startips_4 <= endips_4)
            //                                    {
            //                                        for (int j = startips_4; j <= endips_4; j++)
            //                                        {
            //                                            string ip = startips[0] + "." + startips[1] + "." + startips[2] + "." + j;
            //                                            this.list_target.Add(ip);
            //                                        }
            //                                    }

            //                                }
            //                                else
            //                                {
            //                                    int index_start = 0;
            //                                    int index_end = 255;
            //                                    if (i == startips_3)
            //                                    {
            //                                        index_start = startips_4;
            //                                    }
            //                                    if (i == endips_3)
            //                                    {
            //                                        index_end = endips_4;
            //                                    }

            //                                    for (int j = index_start; j <= index_end; j++)
            //                                    {
            //                                        string ip = startips[0] + "." + startips[1] + "." + i + "." + j;
            //                                        this.list_target.Add(ip);
            //                                    }

            //                                }
            //                            }

            //                        }
            //                    }

            //                }


            //            }
            //            else
            //            {
            //                this.list_target = this.list_import_target;
            //            }
            //        }
            //        if (this.list_target.Count <= 0)
            //        {
            //            LogError("目标格式错误！\r\n格式示例：\r\n192.168.1.1\r\nwww.baidu.com\r\n192.168.1.1-192.168.200.1\r\n192.168.1.1-192.168.1.200");
            //            return false;
            //        }
            //    }

            //}
            //加载自定义字典字典
            //if (notAutoSelectDic)
            {
                if (txt_username.EndsWith(".txt"))
                {
                    this.list_username = FileTool.readFileToList(txt_username);
                }
                else
                {
                    if (txt_username.Length > 0)
                    {
                        this.list_username.Clear();
                        this.list_username.Add(txt_username);
                    }
                }

                if (txt_password.EndsWith(".txt"))
                {
                    this.list_password = FileTool.readFileToList(txt_password);
                }
                else
                {
                    if (txt_password.Length > 0)
                    {
                        this.list_password.Clear();
                        this.list_password.Add(txt_password);
                    }
                }

                if (this.list_username.Count <= 0)
                {
                    LogError("请设置需要检查的用户名！");
                    return false;
                }
                else if (this.list_password.Count <= 0)
                {
                    LogError("请设置检查的密码！");
                    return false;
                }
            }
            //else
            //{
            //    //如果默认字典没有数据则加载默认字典

            //    if (dics.Count <= 0)
            //    {
            //        LogInfo("根据选择检查的服务自动加载字典......");

            //        //foreach (string serviceName in this.services_list.CheckedItems)
            //        string serviceName = strServiceName;
            //        {
            //            ServiceModel sm = this.services[serviceName];
            //            // ......
            //            //sm.ListUserName = FileTool.readFileToList(Directory.GetCurrentDirectory() + sm.DicUserNamePath);
            //            //sm.ListPassword = FileTool.readFileToList(Directory.GetCurrentDirectory() + sm.DicPasswordPath);
            //            if (sm.ListUserName.Count <= 0)
            //            {
            //                LogWarning("加载" + serviceName + "用户名字典未发现数据！");
            //            }
            //            else if (sm.ListPassword.Count <= 0)
            //            {
            //                LogWarning("加载" + serviceName + "密码字典未发现数据！");
            //            }
            //            else
            //            {
            //                LogWarning("加载" + serviceName + "字典成功，用户名" + sm.ListUserName.Count + "个，密码" + sm.ListPassword.Count + "个！");
            //            }
            //        }
            //        LogInfo("根据选择检查的服务自动加载字典完成！");
            //    }
            //}
            return true;

        }

        private void initStatusCount()
        {
            successCount = 0;
            allCrackCount = 0;
        }

        //Thread crackerThread = null;
        public bool cracker(string strServiceName, string strTarget, int nPort, string strUsernameOrFileName, string strPasswordOrFileName)
        {
            SmartThreadPool stp = null;
            if (!initTargetAndDic(strServiceName, strTarget, strUsernameOrFileName, strPasswordOrFileName))
            {
                LogError("Failed initTargetAndDic!");
                return false;
            }

            //初始化检查次数
            initStatusCount();
            //清空检查列表
            this.list_cracker = new ConcurrentBag<string>();
            //清空跳过列表
            this.list_ip_break.Clear();
            this.list_ip_user_break.Clear();

            stp = new SmartThreadPool();
           
            if (this.list_target.Count < maxThread)
                stp.MaxThreads = this.list_target.Count;
            else
                stp.MaxThreads = maxThread;

            //creackerSumCount = 0;
            //scanPortsSumCount = 0;

            foreach (string target in this.list_target)
            {
                this.list_cracker.Add(target + ":" + nPort.ToString() + ":" + strServiceName);
            }

            //foreach (string serviceName in this.services_list.CheckedItems)
            string serviceName = strServiceName;
            {
                HashSet<string> clist_username = null;
                HashSet<string> clist_password = null;
                //if (this.notAutoSelectDic == false)
                //{
                //    clist_username = this.services[serviceName].ListUserName;
                //    clist_password = this.services[serviceName].ListPassword;
                //}
                //else
                {
                    clist_username = this.list_username;
                    clist_password = this.list_password;
                }

                foreach (string user in clist_username)
                {
                    //替换变量密码
                    string username = user;// + this.txt_username_ext.Text;
                    HashSet<string> list_current_password = new HashSet<string>();

                    foreach (string cpass in clist_password)
                    {
                        string newpass = cpass.Replace("%user%", user);
                        if (!list_current_password.Contains(newpass))
                        {
                            list_current_password.Add(newpass);

                        }
                        else
                        {
                            //重复密码
                            //creackerSumCount -= this.list_cracker.Count * clist_username.Count;
                        }

                    }
                    foreach (string pass in list_current_password)
                    {
                        foreach (string cracker in this.list_cracker)
                        {
                            if (cracker.EndsWith(serviceName))
                            {
                                stp.QueueWorkItem<string, string, string>(crackerService, cracker, username, pass);
                            }
                        }
                    }
                }
            }
            stp.WaitForIdle();
            stp.Shutdown();
            LogInfo("检查完成！");
            return true;
        }
    }
}
