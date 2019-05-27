using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace NetworkTest
{
    public partial class Form1 : Form
    {
        Dictionary<string, string> config = new Dictionary<string, string>();
        public Form1()
        {
            InitializeComponent();
            success = new Dictionary<string, string>();
            error = new List<string>();
            timeout = new List<string>();
            CheckForIllegalCrossThreadCalls = false;
            string ss = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsetting.json"));
            config = JsonConvert.DeserializeObject<Dictionary<string, string>>(ss);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            if(adapters.Length == 0)
            {
                textBox1.Text += ("没有找到网络适配器！" + "\r\n");
            }
            else
            {
                foreach (NetworkInterface adapter in adapters)
                {
                    textBox1.Text += ("描述：" + adapter.Description +"\r\n");
                    textBox1.Text += ("标识符：" + adapter.Id + "\r\n");
                    textBox1.Text += ("名称：" + adapter.Name + "\r\n");
                    textBox1.Text += ("类型：" + adapter.NetworkInterfaceType + "\r\n");
                    textBox1.Text += ("速度：" + adapter.Speed * 0.001 * 0.001 + "M" + "\r\n");
                    textBox1.Text += ("操作状态：" + adapter.OperationalStatus + "\r\n");
                    textBox1.Text += ("MAC 地址：" + adapter.GetPhysicalAddress() + "\r\n");

                    // 格式化显示MAC地址                
                    PhysicalAddress pa = adapter.GetPhysicalAddress();//获取适配器的媒体访问（MAC）地址
                    byte[] bytes = pa.GetAddressBytes();//返回当前实例的地址
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        sb.Append(bytes[i].ToString("X2"));//以十六进制格式化
                        if (i != bytes.Length - 1)
                        {
                            sb.Append("-");
                        }
                    }
                    textBox1.Text += ("MAC 地址：" + sb + "\r\n");
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TestConnection();
        }
        public void TestConnection()
        {
            string host = string.Empty;
            if (!config.ContainsKey("domain"))
            {
                host = "61.177.7.1";
            }
            else
            {
                host = config["domain"];
            }
            Ping p1 = new Ping();
            PingReply reply = p1.Send(host);
            if (reply.Status == IPStatus.Success)
            {
                textBox1.Text += (DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + "网络连接成功！" + "\r\n");
            }
            else if (reply.Status == IPStatus.TimedOut)
            {
                textBox1.Text += (DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + "网络连接超时！" + "\r\n");
            }
            else
            {
                textBox1.Text += (DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + "网络连接失败！" + "\r\n");
            }
        }
        public bool flag = true;
        public Dictionary<string,string> success ;
        public List<string> error ;
        public List<string> timeout ;
        private void button3_Click(object sender, EventArgs e)
        {
            if (!button3.Text.Equals("停止"))
            {
                success = new Dictionary<string, string>();
                error = new List<string>();
                timeout = new List<string>();
                Thread th = new Thread(ConnectTestThread);
                flag = true;
                th.Start();
                button3.Text = "停止";
            }
            else
            {
                button3.Text = "重新开始";
                flag = false;
            }
        }
        public long IpChange(string ipmsg)
        {
            char[] separator = new char[] { '.' };
            string[] items = ipmsg.Split(separator);
            long dreamduip = long.Parse(items[0]) << 24
                    | long.Parse(items[1]) << 16
                    | long.Parse(items[2]) << 8
                    | long.Parse(items[3]);
            long sun = IPAddress.HostToNetworkOrder(dreamduip);
            return sun;
        }
        public void CheckMemory()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>
            {
                { "suc", success },
                { "err", error },
                { "timeout", timeout }
            };
            if(JsonConvert.SerializeObject(dic).Length >= 6000)
            {
                FileStream fs = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + "log.log"), FileMode.OpenOrCreate);
                byte[] msg = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dic));
                fs.Write(msg, 0, msg.Length);
                fs.Close();
                success = new Dictionary<string, string>();
                error = new List<string>();
                timeout = new List<string>();
                textBox1.Clear();
            }
        }
        public bool IsConnectInternet(string host,int port)
        {
            try
            {
                TcpClient tcpclient = new TcpClient(host, port);
                if (tcpclient.Connected)
                {
                    tcpclient.Close();
                    return true;
                }
                else
                {
                    tcpclient.Close();
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public void ConnectTestThread()
        {
            int a = 1;
            while (flag)
            {
                string host = string.Empty;
                int port = 0;
                if (!config.ContainsKey("domain"))
                {
                    host = "61.177.7.1";
                }
                else
                {
                    host = config["domain"];
                }
                if (!config.ContainsKey("port"))
                {
                    port = 80 ;
                }
                else
                {
                    port = int.Parse(config["port"]);
                }
                try
                {
                    int timeouttime = int.Parse(textBox2.Text);
                    Thread.Sleep(new TimeSpan(0, 0, 0, timeouttime));
                }
                catch
                {
                    Thread.Sleep(new TimeSpan(0, 0, 0, 30));
                }
                if (a % 100 == 0)
                {
                    CheckMemory();
                    a = 0;
                }
                string time = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
                if (config["type"].Equals("1"))
                {
                    bool iscon = IsConnectInternet(host,port);
                    if (iscon)
                    {
                        textBox1.Text += (time + "网络连接成功！" + "\r\n");
                        textBox1.Focus();
                        textBox1.Select(textBox1.TextLength, 0);
                        textBox1.ScrollToCaret();
                        success.Add(time, "0");
                    }
                    else
                    {
                        textBox1.Text += (time + "网络连接失败！" + "\r\n");
                        textBox1.Focus();
                        textBox1.Select(textBox1.TextLength, 0);
                        textBox1.ScrollToCaret();
                        error.Add(time);
                    }
                }
                else
                {
                    Ping p1 = new Ping();
                    PingReply reply = p1.Send(host);
                    if (reply.Status == IPStatus.Success)
                    {
                        textBox1.Text += (time + "网络连接成功！" + "\r\n");
                        textBox1.Focus();
                        textBox1.Select(textBox1.TextLength, 0);
                        textBox1.ScrollToCaret();
                        success.Add(time, reply.RoundtripTime.ToString());
                    }
                    else if (reply.Status == IPStatus.TimedOut)
                    {
                        textBox1.Text += (time + "网络连接超时！" + "\r\n");
                        textBox1.Focus();
                        textBox1.Select(textBox1.TextLength, 0);
                        textBox1.ScrollToCaret();
                        timeout.Add(time);
                    }
                    else
                    {
                        textBox1.Text += (time + "网络连接失败！" + "\r\n");
                        textBox1.Focus();
                        textBox1.Select(textBox1.TextLength, 0);
                        textBox1.ScrollToCaret();
                        error.Add(time);
                    }
                }
                a++;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Report r = new Report(success, error.ToArray(),timeout.ToArray());
            r.ShowDialog(this);
        }
    }
}
