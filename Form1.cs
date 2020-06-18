using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Gps.Plugin.Common.Helpers;
//using MySql.Data.MySqlClient;
using System.Data.SQLite;

namespace GT808Simulator
{
    public partial class Form1 : Form
    {
        protected static log4net.ILog log = null;
        string connstr = ConfigurationManager.AppSettings["connstr"];// "data source=10.1.97.7;database=gt808Simulator;user id=miracle;password=hl@mic@201905;pooling=false;charset=utf8";//pooling代表是否使用连接池
        //MySqlConnection conn = null;
        private string ip;
        private int port;
        //private string deviceId;
        Random r = new Random();
        Socket tcp;// = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                   //private double initLat = 32.191878;
                   //private double initLon = 119.370384;
        private LatlonBuilder latlonBuilder;
        private int messageID;
        //private string authCodeString = "";
        public Form1()
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));
            log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //conn = new MySqlConnection(connstr);
            InitializeComponent();

            string line = ConfigurationManager.AppSettings["remoteServerPort"];
            int pos = line.IndexOf(':');
            ip = line.Substring(0, pos);
            string strPort = line.Substring(pos + 1);
            int.TryParse(strPort, out port);
            //double minLat = 31.802893;
            //double maxLat = 39.300299;
            //double minLon = 104.941406;
            //double maxLon = 117.861328;
            //20.802893, 50.300299, 73.941406, 130.861328
            //this.latlonBuilder = new LatlonBuilder(initLat, initLon, 20.802893, 50.300299, 73.941406, 130.861328);
            this.latlonBuilder = new LatlonBuilder(Convert.ToDouble(txtLat.Text), Convert.ToDouble(txtLon.Text), 20.802893, 50.300299, 73.941406, 130.861328);

        }

        #region 窗体事件
        private void Form1_Load(object sender, EventArgs e)
        {
            InitDataTable();
            BindRoot();
        }
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("退出当前程序？", "GT808模拟器", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                System.Environment.Exit(0);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                tcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                tcp.Connect(IPAddress.Parse(ip), port);
                if (tcp.Connected)
                {
                    btnConnect.Enabled = false;
                    btnClose.Enabled = true;
                    btnSend.Enabled = true;
                    toolStripStatusLabel1.Text = "已连接：" + this.ip + ":" + this.port;
                    toolStripStatusLabel1.ForeColor = Color.Green;
                    MessageBox.Show("成功连接到服务器：" + this.ip + ":" + this.port, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            //catch (ThreadAbortException)
            //{
            //    return;
            //}
            catch (Exception exp)
            {
                toolStripStatusLabel1.Text = "连接失败：" + this.ip + ":" + this.port;
                MessageBox.Show(exp.Message);
                log.Error(exp);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }
        /// <summary>
        /// 断开服务器连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("确认断开服务器？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    tcp.Disconnect(false);
                    if (!tcp.Connected)
                    {
                        tcp.Shutdown(SocketShutdown.Both);
                        tcp.Close();
                        btnConnect.Enabled = true;
                        btnClose.Enabled = false;
                        btnSend.Enabled = false;
                        toolStripStatusLabel1.Text = "未连接";
                        toolStripStatusLabel1.ForeColor = Color.Red;
                        MessageBox.Show("成功断开服务器：" + this.ip + ":" + this.port, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception exp)
            {
                toolStripStatusLabel1.Text = "未连接";
                MessageBox.Show(exp.Message);
                log.Error(exp);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            DataRow dr = (DataRow)e.Node.Tag;
            try
            {
                if (dr["MessageID"].ToString().Trim() != "")
                {
                    toolStripStatusLabel2.Text = "当前指令：" + dr["FunctionName"].ToString() + "[" + dr["MessageID"].ToString() + "]";
                    this.messageID = Convert.ToInt32(dr["MessageID"].ToString(), 16);
                }
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //   8100-0003-0-10302035743-FFFF00010397"
            //7E810000230-10302035743-00030003003936424431303542324646373442323541374538433233314137334543443846BF7E
            //7E810000230-10302035743-00040004003242343831443441384641463431423938444237313339393635364141363230C47E
            //byte[] b1 = new byte()[1024];


            //7E8100002301234567898700020002004130433443463637303945333439344138323033443839393232434246393537A07E
            //string s1 = "81000014012345678987000C-000100-6264413230323030353235313731333034D3";
            //                   810000230123456789870001-000100-4333434535394635453741463437374238434136364138354331314132363837D97E

            //string s1 = "6264413230323030353235313731333034D3";
            string s1 = "4333434535394635453741463437374238434136364138354331314132363837D9";
            
            byte[] b1 = HexStrTobyte(s1);
            //Buffer.BlockCopy(bytesReceived, HeadPack.PackSize - ClientRegistReturnPack.PackSize + 1, authCodeByte, 0, authCodeByte.Length);

            //byte[] carNumber = System.Text.Encoding.GetEncoding("GBK").GetBytes(txtCarNumber.Text.Trim());
            //MessageIds.
            //MessageIds aaa = (MessageIds)Enum.Parse(typeof(MessageIds), cbFace.Text);
            string e1=Enum.GetName(typeof(MessageIds), 512);
            string e2 = ExtensionMethods.GetDescriptionByName<MessageIds>((MessageIds)512);
            string ss = System.Text.Encoding.Default.GetString(b1);
            //31.802893, 39.300299, 104.941406, 117.861328
            double minLat = 31.802893;
            double maxLat = 39.300299;
            double minLon = 104.941406;
            double maxLon = 117.861328;
            Console.WriteLine(Math.Round(r.NextDouble() * (maxLat - minLat) + minLat, 5)+"--"+ Math.Round(r.NextDouble() * (maxLon - minLon) + minLon, 5));

        }

        private byte[] HexStrTobyte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Trim(), 16);
            return returnBytes;
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            if(!tcp.Connected)
            {
                btnConnect.Enabled = true;
                btnClose.Enabled = false;
                MessageBox.Show("请先连接服务器", "提示",MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (this.messageID == 0)
            {
                MessageBox.Show("请在左侧选择正确的指令", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            switch (this.messageID)
            {
                case (int)MessageIds.PositionReport:
                    {
                        SendPosition();
                        break;
                    }
                case (int)MessageIds.ClientPump:
                    {
                        SendClientPump();
                        break;
                    }
                case (int)MessageIds.ClientRegist:
                    {
                        SendClientRegist();
                        break;
                    }
                case (int)MessageIds.ClientAuth:
                    {
                        if (this.textBoxAuthCode.Text.Trim() == "")
                        {
                            MessageBox.Show("请先填写鉴权码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        SendClientAuth();
                        break;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// 定时发送心跳
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (checkBoxPump.Checked && tcp!=null && tcp.Connected)
            {
                //发送心跳包
                SendClientPump();
            }
        }
        #endregion

        #region 指令处理

        /// <summary>
        /// 终端注册
        /// </summary>
        private void SendClientRegist()
        {
            HeadPack head = new HeadPack() { SeqNO = GetNextSeqNum(), MessageId = (ushort)MessageIds.ClientRegist, BodyProp = (ushort)0 };
            head.SetDeviceId(this.txtDeviceId.Text.Trim());
            ClientRegistPack pack = new ClientRegistPack()
            {
                Province = Convert.ToUInt16("32"),
                City = Convert.ToUInt16("1100"),
                Manufacturer = new byte[5],
                DeviceModel = new byte[20],
                DeviceId = new byte[7],
                CarColor = 2
                //CarNumber = System.Text.Encoding.GetEncoding("GBK").GetBytes(txtCarNumber.Text.Trim())
            };
            //车牌号
            byte[] carNumber = System.Text.Encoding.GetEncoding("GBK").GetBytes(txtCarNumber.Text.Trim());

            byte[] bytesSend = RawFormatter.Instance.Struct2Bytes(pack);
            //加上车牌
            bytesSend = bytesSend.Concat(carNumber).ToArray();

            BodyPropertyHelper.SetMessageLength(ref head.BodyProp, (ushort)bytesSend.Length);

            byte[] headBytes = RawFormatter.Instance.Struct2Bytes(head);
            byte[] fullBytes = headBytes.Concat(bytesSend).ToArray();
            byte checkByte = PackHelper.CalcCheckByte(fullBytes, 0, fullBytes.Length);

            bytesSend = (new byte[] { 0x7e }
            .Concat(PackHelper.EncodeBytes(fullBytes.Concat(new byte[] { checkByte })))
            .Concat(new byte[] { 0x7e })).ToArray();
            //////////////////////////////////////////////////////////////////////////
            //string e1 = Enum.GetName(typeof(MessageIds), Convert.ToInt32(head.MessageId));
            string e2 = ExtensionMethods.GetDescriptionByName<MessageIds>((MessageIds)Convert.ToInt32(head.MessageId));
            /////////////////////////////////////////////////////////////////////////////
            this.dataGridView1.Rows.Add("↑", head.GetDeviceId(), DateTime.Now, head.SeqNO, "0x" + Convert.ToString(head.MessageId, 16).PadLeft(4, '0') + "(" + e2 + ")", 0, bytesSend.ToHexString());

            this.dataGridView1.FirstDisplayedScrollingRowIndex = this.dataGridView1.Rows.Count - 1;
            //this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Selected = true;
            //this.dataGridView1.CurrentCell = this.dataGridView1[0, this.dataGridView1.Rows.Count - 1];

            SendMessage(bytesSend);
        }
        /// <summary>
        /// 
        /// </summary>
        private void SendClientAuth()
        {
            HeadPack head = new HeadPack() { SeqNO = GetNextSeqNum(), MessageId = (ushort)MessageIds.ClientAuth, BodyProp = (ushort)0 };
            head.SetDeviceId(this.txtDeviceId.Text.Trim());
            //ClientAuthPack pack = new ClientAuthPack();

            //byte[] bytesSend = RawFormatter.Instance.Struct2Bytes(pack);
            byte[] bytesSend = System.Text.Encoding.Default.GetBytes(this.textBoxAuthCode.Text.Trim());
            //加上鉴权码
            //bytesSend = bytesSend.Concat(authCode).ToArray();

            BodyPropertyHelper.SetMessageLength(ref head.BodyProp, (ushort)bytesSend.Length);

            byte[] headBytes = RawFormatter.Instance.Struct2Bytes(head);
            byte[] fullBytes = headBytes.Concat(bytesSend).ToArray();
            byte checkByte = PackHelper.CalcCheckByte(fullBytes, 0, fullBytes.Length);

            bytesSend = (new byte[] { 0x7e }
            .Concat(PackHelper.EncodeBytes(fullBytes.Concat(new byte[] { checkByte })))
            .Concat(new byte[] { 0x7e })).ToArray();
            //////////////////////////////////////////////////////////////////////////
            //string e1 = Enum.GetName(typeof(MessageIds), Convert.ToInt32(head.MessageId));
            string e2 = ExtensionMethods.GetDescriptionByName<MessageIds>((MessageIds)Convert.ToInt32(head.MessageId));
            /////////////////////////////////////////////////////////////////////////////
            this.dataGridView1.Rows.Add("↑", head.GetDeviceId(), DateTime.Now, head.SeqNO, "0x" + Convert.ToString(head.MessageId, 16).PadLeft(4, '0') + "(" + e2 + ")", 0, bytesSend.ToHexString());

            this.dataGridView1.FirstDisplayedScrollingRowIndex = this.dataGridView1.Rows.Count - 1;
            //this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Selected = true;
            //this.dataGridView1.CurrentCell = this.dataGridView1[0, this.dataGridView1.Rows.Count - 1];

            SendMessage(bytesSend); ;
        }
        /// <summary>
        /// 终端心跳
        /// </summary>
        private void SendClientPump()
        {
            HeadPack head = new HeadPack() { SeqNO = GetNextSeqNum(), MessageId = (ushort)MessageIds.ClientPump, BodyProp = (ushort)0 };
            head.SetDeviceId(this.txtDeviceId.Text.Trim());
            //ClientPumpPack pack = new ClientPumpPack();
            //byte[] bytesSend = RawFormatter.Instance.Struct2Bytes(pack);
            //BodyPropertyHelper.SetMessageLength(ref head.BodyProp, (ushort)bytesSend.Length);
            BodyPropertyHelper.SetMessageLength(ref head.BodyProp, (ushort)0);

            byte[] headBytes = RawFormatter.Instance.Struct2Bytes(head);
            byte[] fullBytes = headBytes;// headBytes.Concat(bytesSend).ToArray();
            byte checkByte = PackHelper.CalcCheckByte(fullBytes, 0, fullBytes.Length);

            byte[]  bytesSend = (new byte[] { 0x7e }
            .Concat(PackHelper.EncodeBytes(fullBytes.Concat(new byte[] { checkByte })))
            .Concat(new byte[] { 0x7e })).ToArray();
            //////////////////////////////////////////////////////////////////////////
            //string e1 = Enum.GetName(typeof(MessageIds), Convert.ToInt32(head.MessageId));
            string e2 = ExtensionMethods.GetDescriptionByName<MessageIds>((MessageIds)Convert.ToInt32(head.MessageId));
            /////////////////////////////////////////////////////////////////////////////
            this.dataGridView1.Rows.Add("↑", head.GetDeviceId(), DateTime.Now, head.SeqNO, "0x" + Convert.ToString(head.MessageId, 16).PadLeft(4, '0') + "(" + e2 + ")", 0, bytesSend.ToHexString());

            this.dataGridView1.FirstDisplayedScrollingRowIndex = this.dataGridView1.Rows.Count - 1;
            //this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Selected = true;
            //this.dataGridView1.CurrentCell = this.dataGridView1[0, this.dataGridView1.Rows.Count - 1];

            SendMessage(bytesSend); ;
        }
        /// <summary>
        /// 位置和报警
        /// </summary>
        private void SendPosition()
        {
            HeadPack head = new HeadPack() { SeqNO = GetNextSeqNum(), MessageId = (ushort)MessageIds.PositionReport, BodyProp = (ushort)0 };
            head.SetDeviceId(this.txtDeviceId.Text.Trim());

            double lat;
            double lon;
            int speed = Convert.ToInt32(domainUpDown1.Text) + r.Next(10);// 10 + r.Next(90);
            latlonBuilder.GetNextLatlon(speed, out lat, out lon);
            //lat = Convert.ToDouble(txtLat.Text);
            //lon = Convert.ToDouble(txtLon.Text);
            

            #region 报警位设置
            BitValueAlerm bitAlerm = new BitValueAlerm(0);
            bitAlerm.Urgent = checkBox0.Checked;
            bitAlerm.Speeding = checkBox1.Checked;
            bitAlerm.Fatigue = checkBox2.Checked;
            bitAlerm.Forewarning = checkBox3.Checked;
            #endregion

            #region 状态位设置
            BitValueState bitState = new BitValueState(0);
            bitState.ACC = checkBox40.Checked;
            bitState.Position = checkBox41.Checked;
            bitState.IsSouth = checkBox42.Checked;
            bitState.IsWest = checkBox43.Checked;
            bitState.OperationStoped = checkBox44.Checked;
            bitState.CarOilchannelBreaked = checkBox45.Checked;
            bitState.CarCircuitBreaked = checkBox46.Checked;
            bitState.CarDoorLocked = checkBox47.Checked;
            bitState.CarDrive = checkBox48.Checked;//新国标新增的
            #endregion

            # region 附加信息
            //里程
            byte[] additionals = (new byte[] { 0x01 }.Concat(new byte[] { 0x04 }).Concat(Convert.ToInt32(textBoxMileage.Text).intToBytes2())).ToArray();//textBoxMileage
            //油量
            additionals = additionals.Concat(new byte[] { 0x02 }.Concat(new byte[] { 0x02 }).Concat(BitConverter.GetBytes(Convert.ToInt16(textBoxOli.Text)))).ToArray();
            //定位卫星数
            additionals = additionals.Concat(new byte[] { 0x31 }.Concat(new byte[] { 0x01 }).Concat(new byte[] { Convert.ToByte(textBoxGNSS.Text) })).ToArray();

            #endregion

            PositionReportPack pack = new PositionReportPack()
            {
                AlermFlags = BitConverter.ToUInt32(bitAlerm.GetBitValueAlerm(), 0),//0,
                Speed = (ushort)(speed * 10),
                State = BitConverter.ToUInt32(bitState.GetBitValueState(), 0),//0,
                Latitude = Convert.ToUInt32(lat * 1000000),
                Longitude = Convert.ToUInt32(lon * 1000000),
                Altitude = Convert.ToUInt16(txtAltitude.Text),//200,
                Direction = Convert.ToUInt16(txtDirection.Text),//0,
                Time = DateTime.Now.ToString("yyMMddHHmmss")
                //Additionals = Additionals
            };

            byte[] bytesSend = RawFormatter.Instance.Struct2Bytes(pack);
            //位置基础包+附加信息包=完整的0x0200
            bytesSend=bytesSend.Concat(additionals).ToArray();
            //设置头里面body的长度。
            BodyPropertyHelper.SetMessageLength(ref head.BodyProp, (ushort)bytesSend.Length);

            byte[] headBytes = RawFormatter.Instance.Struct2Bytes(head);
            byte[] fullBytes = headBytes.Concat(bytesSend).ToArray();
            byte checkByte = PackHelper.CalcCheckByte(fullBytes, 0, fullBytes.Length);

            bytesSend = (new byte[] { 0x7e }
            .Concat(PackHelper.EncodeBytes(fullBytes.Concat(new byte[] { checkByte })))
            .Concat(new byte[] { 0x7e })).ToArray();
            //////////////////////////////////////////////////////////////////////////
            //string e1 = Enum.GetName(typeof(MessageIds), Convert.ToInt32(head.MessageId));
            string e2 = ExtensionMethods.GetDescriptionByName<MessageIds>((MessageIds)Convert.ToInt32(head.MessageId));
            /////////////////////////////////////////////////////////////////////////////
            this.dataGridView1.Rows.Add("↑", head.GetDeviceId(), DateTime.Now, head.SeqNO, "0x" + Convert.ToString(head.MessageId, 16).PadLeft(4, '0')+"("+e2+")",0, bytesSend.ToHexString());

            this.dataGridView1.FirstDisplayedScrollingRowIndex = this.dataGridView1.Rows.Count - 1;
            //this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Selected = true;
            //this.dataGridView1.CurrentCell = this.dataGridView1[0, this.dataGridView1.Rows.Count - 1];
            //Console.WriteLine("{0} {1}",head.SeqNO, bytesSend.ToHexString());

            SendMessage(bytesSend); ;
        }

        #endregion

        #region 发送和接收

        ushort seqNum = 0;
        ushort GetNextSeqNum()
        {
            lock (this)
            {
                return ++seqNum;
            }
        }
        private byte[] bufferRecv = new byte[1024];

        private bool SendMessage(byte[] bytesSend)
        {
            //等待接收服务端返回值
            var success = false;
            try
            {
                //发送消息
                SendBytes(tcp, bytesSend);
                //控制台打印日志cpu占用太高
                //Console.WriteLine("{0} {1}, LatLon:{2:0.000000},{3:0.000000}", head.GetDeviceId(), DateTime.Now.ToString(), lat, lon);

                success = RecvBytes(tcp);
                return success;
            }
            catch (Exception ex)
            {
                btnConnect.Enabled = true;
                btnClose.Enabled = false;
                btnSend.Enabled = false;
                toolStripStatusLabel1.Text = "未连接";
                toolStripStatusLabel1.ForeColor = Color.Red;
                log.Error(ex.Message, ex);
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return success;
        }
        void SendBytes(Socket tcp, byte[] bytes)
            {
                lock (System.Reflection.MethodBase.GetCurrentMethod())
                {
                    if (bytes.Length != tcp.Send(bytes))
                    {
                        throw new SocketException((int)SocketError.ConnectionReset);
                    }
                }
            }

        private bool RecvBytes(Socket tcp)
        {
            lock (bufferRecv)
            {
                byte[] buffer = bufferRecv;

                int received = tcp.Receive(buffer);

                byte[] originalBytes = new byte[received];
                Buffer.BlockCopy(buffer, 0, originalBytes, 0, received);

                byte[] bytesReceived = PackHelper.DecodeBytes(buffer, 1, received - 2);

                HeadPack headPack = new HeadPack();
                RawFormatter.Instance.Bytes2Struct(headPack, bytesReceived, 0, HeadPack.PackSize);
                byte result = 0;
                string authCode = "";
                if (headPack.MessageId == (ushort)MessageIds.ClientRegistReturn)//注册应答包
                {
                    ClientRegistReturnPack pack = new ClientRegistReturnPack();
                    RawFormatter.Instance.Bytes2Struct(pack, bytesReceived, HeadPack.PackSize, ClientRegistReturnPack.PackSize);
                    result = pack.Result;
                    byte[] authCodeByte = new byte[bytesReceived.Length- HeadPack.PackSize- ClientRegistReturnPack.PackSize-1];
                    Buffer.BlockCopy(bytesReceived, HeadPack.PackSize + ClientRegistReturnPack.PackSize, authCodeByte, 0, authCodeByte.Length);
                    //注册鉴权码
                    authCode = System.Text.Encoding.Default.GetString(authCodeByte);
                    this.textBoxAuthCode.Text = authCode;
                }
                else//服务器通用应答包
                {
                    ServerAnswerPack pack = new ServerAnswerPack();
                    RawFormatter.Instance.Bytes2Struct(pack, bytesReceived, HeadPack.PackSize, ServerAnswerPack.PackSize);
                    result = pack.Result;
                }
                //////////////////////////////////////////////////////////////////////////
                //string e1 = Enum.GetName(typeof(MessageIds), Convert.ToInt32(headPack.MessageId));
                string e2 = "未知消息类型";
                try
                {
                    e2 = ExtensionMethods.GetDescriptionByName<MessageIds>((MessageIds)Convert.ToInt32(headPack.MessageId));
                }
                catch(Exception ex)
                {
                    log.Error(ex.Message);
                    MessageBox.Show("接收到未知消息类型", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                /////////////////////////////////////////////////////////////////////////////
                int index = this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[index].Cells[0].Value = "↓";
                this.dataGridView1.Rows[index].Cells[1].Value = headPack.GetDeviceId();
                this.dataGridView1.Rows[index].Cells[2].Value = DateTime.Now;
                this.dataGridView1.Rows[index].Cells[3].Value = headPack.SeqNO;
                this.dataGridView1.Rows[index].Cells[4].Value = "0x" + Convert.ToString(headPack.MessageId, 16).PadLeft(4, '0') + "(" + e2 + ")";
                this.dataGridView1.Rows[index].Cells[5].Value = result+ ((authCode) != "" ? "(" +authCode+")" : "") ;
                this.dataGridView1.Rows[index].Cells[6].Value = originalBytes.ToHexString();

                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.BackColor = Color.SkyBlue;
                this.dataGridView1.Rows[index].DefaultCellStyle = style;

                this.dataGridView1.FirstDisplayedScrollingRowIndex = this.dataGridView1.Rows.Count - 1;
                //this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Selected = true;
                //this.dataGridView1.CurrentCell = this.dataGridView1[0, this.dataGridView1.Rows.Count - 1];

                //this.dataGridView1.Rows.Add("↓", headPack.GetDeviceId(), DateTime.Now, headPack.SeqNO, "0x" + Convert.ToString(headPack.MessageId, 16).PadLeft(4, '0'), pack.Result);
                //Console.WriteLine("SeqNO:{0} MessageId:{1} Result:{2}", pack.SeqNO, pack.MessageId, pack.Result);
                return result == 0;
            }
        }

        #endregion

        #region 初始化指令树
        private DataTable dt = null;
        //获取所用指令数据 
        private void InitDataTable()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=db.db;Version=3;");
            string sql = "SELECT * FROM `functionlist`";
            //MySqlCommand cmd = new MySqlCommand(sql, conn);
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);

            //MySqlDataAdapter ada = new MySqlDataAdapter(cmd);
            SQLiteDataAdapter ada = new SQLiteDataAdapter(cmd);

            dt = new DataTable();
            ada.Fill(dt);
            conn.Close();
        }

        //绑定根节点 
        private void BindRoot()
        {
            DataRow[] rows = dt.Select("ParentFunctionID=0");
            //取根 
            foreach (DataRow dRow in rows)
            {
                TreeNode rootNode = new TreeNode();
                rootNode.Tag = dRow;
                rootNode.Text = dRow["FunctionName"].ToString();
                treeView1.Nodes.Add(rootNode);
                BindChildAreas(rootNode);
            }
        }
        //递归绑定子区域 
        private void BindChildAreas(TreeNode fNode)
        {
            DataRow dr = (DataRow)fNode.Tag;
            //父节点数据关联的数据行 
            string parentFunctionID = dr["FunctionID"].ToString();
            //父节点ID 
            DataRow[] rows = dt.Select("ParentFunctionID=" + parentFunctionID);
            //子区域 
            if (rows.Length == 0)//递归终止，区域不包含子区域时 
            {
                return;
            }
            foreach (DataRow dRow in rows)
            {
                TreeNode node = new TreeNode();
                node.Tag = dRow;
                node.Text = dRow["FunctionName"].ToString() + "(" + dRow["MessageID"].ToString() + ")";
                //添加子节点 
                fNode.Nodes.Add(node);
                //递归 
                BindChildAreas(node);
            }
        }
        #endregion
    }

    /// <summary>
    /// 经纬度工具类
    /// </summary>
    class LatlonBuilder
    {
        private double lat;
        private double lon;
        private double minLat;
        private double maxLat;
        private double minLon;
        private double maxLon;
        private int direction = 0;
        private Random r = new Random();
        public LatlonBuilder(double lat, double lon, double minLat, double maxlat, double minLon, double maxLon)
        {
            this.lat = lat;
            this.lon = lon;
            this.minLat = minLat;
            this.maxLat = maxlat;
            this.minLon = minLon;
            this.maxLon = maxLon;

            this.direction = r.Next(360);
        }

        public bool GetNextLatlon(int speed, out double lat, out double lon)
        {
            
            direction = (direction + (r.Next(30) - 15)) % 360;
            double angle = Math.PI * this.direction/ 180.0;
            double latAdd = speed / 1000.0 * Math.Sin(angle);
            double lonAdd = speed / 1000.0 * Math.Cos(angle);

            this.lat = lat = this.lat + latAdd;
            this.lon = lon = this.lon + lonAdd;

            if (lat < minLat || lat > maxLat || lon < minLon || lon > maxLon)
            {
                direction = (direction + 180) % 360;
                return GetNextLatlon(speed, out lat, out lon);
            }

            //this.lat = lat = Math.Round(r.NextDouble() * (this.maxLat - this.minLat) + this.minLat, 5); 
            //this.lon = lon = Math.Round(r.NextDouble() * (this.maxLon - this.minLon) + this.minLon, 5);
            return true;
        }
        public double GetRandomNumber(double minimum, double maximum, int Len)   //Len小数点保留位数
        {
            Random random = new Random();
            return Math.Round(random.NextDouble() * (maximum - minimum) + minimum, Len);
        }
    }
}
