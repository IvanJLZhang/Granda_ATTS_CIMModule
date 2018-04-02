using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Secs4Net.Properties;

namespace Secs4Net
{
    public class SecsGem : IDisposable
    {

        #region ConnectChangeHandler 

        /// <summary>
        /// HSMS connection state changed event
        /// </summary>
        public event EventHandler<TEventArgs<ConnectionState>> ConnectionChanged;

        /// <summary>
        /// HSMS primary message received evect
        /// </summary>
        public event EventHandler<TEventArgs<SecsMessage>> PrimaryMessageRecived;
        #endregion

        #region 属性

        /// <summary>
        /// Connection state
        /// </summary>
        public ConnectionState State { get; private set; }

        /// <summary>
        /// Device Id
        /// </summary>
        public short DeviceId { get; set; }

        /// <summary>
        /// LinkTest Timer Interval
        /// </summary>
        public int LinkTestInterval { get; set; }

        /// <summary>
        /// T3 Timer Interval
        /// </summary>
        public int T3 { get; set; }

        /// <summary>
        /// T5 Timer Interval
        /// </summary>
        public int T5 { get; set; }

        /// <summary>
        ///T6 Timer Interval
        /// </summary>
        public int T6 { get; set; }

        /// <summary>
        /// T7 Timer Interval
        /// </summary>
        public int T7 { get; set; }

        /// <summary>
        /// T8 Timer Interval
        /// </summary>
        public int T8 { get; set; }

        /// <summary>
        /// get or set linking test timer enable or not 
        /// </summary>
        public bool LinkTestEnable
        {
            get { return _timerLinkTest.Enabled; }
            set
            {
                _timerLinkTest.Interval = LinkTestInterval;
                _timerLinkTest.Enabled = value;
            }
        }

        private const int RECEIVE_BUFFER_SIZE = 0x4000;

        #endregion

        #region 字段
        readonly bool _isActive;
        readonly IPAddress _ip;
        readonly int _port;
        Socket _socket;

        readonly SecsDecoder _secsDecoder;
        readonly ConcurrentDictionary<int, SecsAsyncResult> _replyExpectedMsgs = new ConcurrentDictionary<int, SecsAsyncResult>();
        readonly Action<SecsMessage, Action<SecsMessage>> PrimaryMessageHandler;
        SecsTracer _tracer;
        readonly System.Timers.Timer _timer7 = new System.Timers.Timer();	// between socket connected and received Select.req timer
        readonly System.Timers.Timer _timer8 = new System.Timers.Timer();
        readonly System.Timers.Timer _timerLinkTest = new System.Timers.Timer();

        readonly Action StartImpl;
        readonly Action StopImpl;

        readonly byte[] _recvBuffer;
        static SecsMessage ControlMessage = new SecsMessage(0, 0, 0, string.Empty, true);
        static ArraySegment<byte> ControlMessageLengthBytes = new ArraySegment<byte>(new byte[] { 0, 0, 0, 10 });
        static readonly SecsTracer DefaultTracer = new SecsTracer();
        //static int systemByte = new Random(Guid.NewGuid().GetHashCode()).Next();
        //public static Func<int> NewSystemByte = () => Interlocked.Increment(ref systemByte);

        public static int NewSystemByte()
        {
            int systemByte = new Random(Guid.NewGuid().GetHashCode()).Next();
            return Interlocked.Increment(ref systemByte);
        }
        #endregion

        #region Socket Start Process 网络连接启动
        public SecsGem()
        {

        }
        /// <summary>
        /// 使用指定的IP地址，端口号，通信模式，处理主消息的委托方法，SecsMessage消息调试对象，内部调用的sorket实例的缓冲区大小，初始化SecsGem类的新实例
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口号</param>
        /// <param name="isActive">活动模式</param>
        /// <param name="receiveBufferSize">构造函数内部的Socket接收区缓存大小</param>
        public SecsGem(IPAddress ip, int port, bool isActive)
        {
            if (ip == null)
                throw new ArgumentNullException("ip");

            _ip = ip;
            _port = port;
            _isActive = isActive;
            _recvBuffer = new byte[RECEIVE_BUFFER_SIZE];
            _secsDecoder = new SecsDecoder(HandleControlMessage, HandleDataMessage);
            // PrimaryMessageHandler = primaryMsgHandler ?? ((primary, reply) => reply(null));
            T3 = 45000;
            T5 = 10000;
            T6 = 5000;
            T7 = 10000;
            T8 = 5000;
            LinkTestInterval = 60000;
            _tracer = new SecsTracer();

            #region Timer Action
            // Timer时间绑定委托
            _timer7.Elapsed += delegate
            {
                _tracer.TraceError("T7 Timeout");
                this.CommunicationStateChanging(ConnectionState.Retry);
            };

            _timer8.Elapsed += delegate
            {
                _tracer.TraceError("T8 Timeout");
                this.CommunicationStateChanging(ConnectionState.Retry);
            };

            _timerLinkTest.Elapsed += delegate
            {
                if (this.State == ConnectionState.Selected)
                    this.SendControlMessage(MessageType.Linktest_req, NewSystemByte());
            };
            #endregion
            // 是否为Active状态
            if (_isActive)
            {
                #region Active Impl
                var timer5 = new System.Timers.Timer();
                timer5.Elapsed += delegate
                {
                    timer5.Enabled = false;
                    _tracer.TraceError("T5 Timeout");
                    this.CommunicationStateChanging(ConnectionState.Retry);
                };

                // StartImpl事务绑定委托事件
                StartImpl = delegate
                {
                    this.CommunicationStateChanging(ConnectionState.Connecting);
                    try
                    {
                        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socket.Connect(_ip, _port);
                        this.CommunicationStateChanging(ConnectionState.Connected);
                        this._socket = socket;
                        // 发送Select请求
                        this.SendControlMessage(MessageType.Select_req, NewSystemByte());
                        this._socket.BeginReceive(_recvBuffer, 0, _recvBuffer.Length, SocketFlags.None, ReceiveComplete, null);
                    }
                    catch (Exception ex)
                    {
                        if (_isDisposed) return;
                        _tracer.TraceError(ex.Message);
                        _tracer.TraceInfo("Start T5 Timer");
                        timer5.Interval = T5;
                        timer5.Enabled = true;
                    }
                };

                // StopImpl事务绑定委托事件
                StopImpl = delegate
                {
                    timer5.Stop();
                    if (_isDisposed) timer5.Dispose();
                };
                #endregion
                StartImpl.BeginInvoke(null, null);
            }
            else
            {

                // 不是active状态
                #region Passive Impl
                var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(_ip, _port));
                server.Listen(0);

                // StartImpl事务绑定委托事件
                StartImpl = delegate
                {
                    this.CommunicationStateChanging(ConnectionState.Connecting);
                    server.BeginAccept(iar =>
                    {
                        try
                        {
                            this._socket = server.EndAccept(iar);
                            this.CommunicationStateChanging(ConnectionState.Connected);
                            this._socket.BeginReceive(_recvBuffer, 0, _recvBuffer.Length, SocketFlags.None, ReceiveComplete, null);
                        }
                        catch (Exception ex)
                        {
                            _tracer.TraceError("System Exception:" + ex.Message);
                            this.CommunicationStateChanging(ConnectionState.Retry);
                        }
                    }, null);
                };

                StopImpl = delegate
                {
                    if (_isDisposed && server != null)
                        server.Close();
                };
                #endregion
                StartImpl();
            }
        }

        public SecsGem(IPAddress ip, int port, bool isActive, Action<SecsMessage, Action<SecsMessage>> primaryMsgHandler, SecsTracer tracer, int receiveBufferSize)
        {
            if (ip == null)
                throw new ArgumentNullException("ip");

            _ip = ip;
            _port = port;
            _isActive = isActive;
            _recvBuffer = new byte[receiveBufferSize < 0x4000 ? 0x4000 : receiveBufferSize];
            _secsDecoder = new SecsDecoder(HandleControlMessage, HandleDataMessage);
            _tracer = tracer ?? DefaultTracer;
            PrimaryMessageHandler = primaryMsgHandler ?? ((primary, reply) => reply(null));
            T3 = 45000;
            T5 = 10000;
            T6 = 5000;
            T7 = 10000;
            T8 = 5000;
            LinkTestInterval = 60000;

            int systemByte = new Random(Guid.NewGuid().GetHashCode()).Next();
            //NewSystemByte = () => Interlocked.Increment(ref systemByte);

            #region Timer Action
            // Timer时间绑定委托
            _timer7.Elapsed += delegate
            {
                _tracer.TraceError("T7 Timeout");
                this.CommunicationStateChanging(ConnectionState.Retry);
            };

            _timer8.Elapsed += delegate
            {
                _tracer.TraceError("T8 Timeout");
                this.CommunicationStateChanging(ConnectionState.Retry);
            };

            _timerLinkTest.Elapsed += delegate
            {
                if (this.State == ConnectionState.Selected)
                    this.SendControlMessage(MessageType.Linktest_req, NewSystemByte());
            };
            #endregion
            // 是否为Active状态
            if (_isActive)
            {
                #region Active Impl
                var timer5 = new System.Timers.Timer();
                timer5.Elapsed += delegate
                {
                    timer5.Enabled = false;
                    _tracer.TraceError("T5 Timeout");
                    this.CommunicationStateChanging(ConnectionState.Retry);
                };

                // StartImpl事务绑定委托事件
                StartImpl = delegate
                {
                    this.CommunicationStateChanging(ConnectionState.Connecting);
                    try
                    {
                        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socket.Connect(_ip, _port);
                        this.CommunicationStateChanging(ConnectionState.Connected);
                        this._socket = socket;
                        // 发送Select请求
                        this.SendControlMessage(MessageType.Select_req, NewSystemByte());
                        this._socket.BeginReceive(_recvBuffer, 0, _recvBuffer.Length, SocketFlags.None, ReceiveComplete, null);
                    }
                    catch (Exception ex)
                    {
                        if (_isDisposed) return;
                        _tracer.TraceError(ex.Message);
                        _tracer.TraceInfo("Start T5 Timer");
                        timer5.Interval = T5;
                        timer5.Enabled = true;
                    }
                };

                // StopImpl事务绑定委托事件
                StopImpl = delegate
                {
                    timer5.Stop();
                    if (_isDisposed) timer5.Dispose();
                };
                #endregion
                StartImpl.BeginInvoke(null, null);
            }
            else
            {

                // 不是active状态
                #region Passive Impl
                var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(_ip, _port));
                server.Listen(0);

                // StartImpl事务绑定委托事件
                StartImpl = delegate
                {
                    this.CommunicationStateChanging(ConnectionState.Connecting);
                    server.BeginAccept(iar =>
                    {
                        try
                        {
                            this._socket = server.EndAccept(iar);
                            this.CommunicationStateChanging(ConnectionState.Connected);
                            this._socket.BeginReceive(_recvBuffer, 0, _recvBuffer.Length, SocketFlags.None, ReceiveComplete, null);
                        }
                        catch (Exception ex)
                        {
                            _tracer.TraceError("System Exception:" + ex.Message);
                            this.CommunicationStateChanging(ConnectionState.Retry);
                        }
                    }, null);
                };

                StopImpl = delegate
                {
                    if (_isDisposed && server != null)
                        server.Close();
                };
                #endregion
                StartImpl();
            }
        }

        //public SecsGem(IPAddress ip, int port, bool isActive) : this(ip, port, isActive, 0) { }
        #endregion

        #region Socket Receive Process  接收程序
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="iar">异步操作状态参数</param>
        void ReceiveComplete(IAsyncResult iar)
        {
            try
            {
                //返回接受到的字节长度
                int count = _socket.EndReceive(iar);

                _timer8.Enabled = false;

                if (count == 0)
                {
                    _tracer.TraceError("Received 0 byte data. Close the socket.");
                    this.CommunicationStateChanging(ConnectionState.Retry);
                    return;
                }

                //判断接收到的数据是否为不完整
                if (_secsDecoder.Decode(_recvBuffer, 0, count))
                {
                    _tracer.TraceInfo("Start T8 Timer");
                    _timer8.Interval = T8;
                    _timer8.Enabled = true;
                }

                _socket.BeginReceive(_recvBuffer, 0, _recvBuffer.Length, SocketFlags.None, ReceiveComplete, null);
            }
            catch (NullReferenceException)
            {
            }
            catch (SocketException ex)
            {
                _tracer.TraceError("RecieveComplete socket error:" + ex.Message);
                this.CommunicationStateChanging(ConnectionState.Retry);
            }
            catch (Exception ex)
            {
                _tracer.TraceError(ex.ToString());
                this.CommunicationStateChanging(ConnectionState.Retry);
            }
        }

        /// <summary>
        /// 处理控制消息
        /// </summary>
        /// <param name="header">头字节数据</param>
        void HandleControlMessage(Header header)
        {
            int systembyte = header.SystemBytes;
            //如果取余数为零，则该信息不需要响应
            if ((byte)header.MessageType % 2 == 0)
            {
                SecsAsyncResult ar = null;
                // 如果在字典中找到需要的系统字节
                if (_replyExpectedMsgs.TryGetValue(systembyte, out ar))
                {
                    ar.EndProcess(ControlMessage, false);
                }
                else
                {
                    _tracer.TraceWarning("Received Unexpected Control Message: " + header.MessageType);
                    return;
                }
            }
            _tracer.TraceInfo("Receive Control message: " + header.MessageType);
            switch (header.MessageType)
            {
                case MessageType.Select_req:
                    this.SendControlMessage(MessageType.Select_rsp, systembyte);
                    this.CommunicationStateChanging(ConnectionState.Selected);
                    break;
                case MessageType.Select_rsp:
                    switch (header.F)
                    {
                        case 0:
                            this.CommunicationStateChanging(ConnectionState.Selected);
                            break;
                        case 1:
                            _tracer.TraceError("Communication Already Active.");
                            break;
                        case 2:
                            _tracer.TraceError("Connection Not Ready.");
                            break;
                        case 3:
                            _tracer.TraceError("Connection Exhaust.");
                            break;
                        default:
                            _tracer.TraceError("Connection Status Is Unknown.");
                            break;
                    }
                    break;
                case MessageType.Linktest_req:
                    this.SendControlMessage(MessageType.Linktest_rsp, systembyte);
                    break;
                case MessageType.Seperate_req:
                    this.CommunicationStateChanging(ConnectionState.Retry);
                    break;
                case MessageType.Deselect_req:
                    this.SendControlMessage(MessageType.Deselect_rsp, systembyte);
                    break;
                case MessageType.Deselect_rsp:
                    switch (header.F)
                    {

                    }
                    break;
                case MessageType.Reject_req:
                    break;
            }
        }

        /// <summary>
        /// 处理数据消息
        /// </summary>
        /// <param name="header">头字节数据</param>
        /// <param name="msg">消息主体</param>
        void HandleDataMessage(Header header, SecsMessage msg)
        {
            int systembyte = header.SystemBytes;

            // 如果接受到的设备ID不等于现有的设备ID，且Stream不为9，Function不为1
            if (header.DeviceId != msg.DeviceID && msg.S != 9 && msg.F != 1)
            {
                _tracer.TraceMessageIn(msg, systembyte);
                _tracer.TraceWarning("Received Unrecognized Device Id Message");
                try
                {
                    this.SendDataMessage(new SecsMessage(msg.DeviceID, 9, 1, "Unknown", false, -1, Item.B(header.Bytes)), null, null);
                }
                catch (Exception ex)
                {
                    _tracer.TraceError("Send S9F1 Error:" + ex.Message);
                }
                return;
            }

            // 如果期待响应
            if (msg.F % 2 != 0)
            {
                if (msg.S != 9)
                {
                    // 触发message receive事件
                    PrimaryMessageRecived?.Invoke(this, new TEventArgs<SecsMessage>(msg));

                    //Primary message
                    _tracer.TraceMessageIn(msg, systembyte);
                    //PrimaryMessageHandler(msg, secondary =>
                    //{
                    //    // 如果不需要返回消息，且不处于选择状态则返回
                    //    if (!header.ReplyExpected || State != ConnectionState.Selected)
                    //        return;

                    //    secondary = secondary ?? new SecsMessage(msg.DeviceID, 9, 7, "Unknown", false, -1, Item.B(header.Bytes));
                    //    try
                    //    {
                    //        this.SendDataMessage(secondary, null, null);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        _tracer.TraceError("Reply Secondary Message Error:" + ex.Message);
                    //    }
                    //});
                    return;
                }
                // Error message
                var headerBytes = (byte[])msg.SecsItem;
                systembyte = BitConverter.ToInt32(new byte[] { headerBytes[9], headerBytes[8], headerBytes[7], headerBytes[6] }, 0);
            }

            // Secondary message
            SecsAsyncResult ar = null;
            if (_replyExpectedMsgs.TryGetValue(systembyte, out ar))
                ar.EndProcess(msg, false);
            _tracer.TraceMessageIn(msg, systembyte);
        }
        #endregion

        #region Socket Send Process  发送程序

        /// <summary>
        /// 发送控制消息
        /// </summary>
        /// <param name="msgType">消息指令类型</param>
        /// <param name="systembyte">系统字节</param>
        void SendControlMessage(MessageType msgType, int systembyte)
        {
            // 如果socket为空，或socket不为连接状态，则返回
            if (this._socket == null || !this._socket.Connected)
                return;

            // 如果指令需求响应，且不为分离请求
            if ((byte)msgType % 2 == 1 && msgType != MessageType.Seperate_req)
            {
                var ar = new SecsAsyncResult(ControlMessage, systembyte, null, null);
                _replyExpectedMsgs[systembyte] = ar;

                ThreadPool.RegisterWaitForSingleObject(ar.AsyncWaitHandle,
                    (state, timeout) =>
                    {
                        SecsAsyncResult ars;
                        // 如果字典移除，且超时
                        if (_replyExpectedMsgs.TryRemove((int)state, out ars) && timeout)
                        {
                            // 控制事务T6超时
                            _tracer.TraceError("T6 Timeout");
                            this.CommunicationStateChanging(ConnectionState.Retry);
                        }
                    }, systembyte, T6, true);
            }

            var header = new Header(new byte[10])
            {
                MessageType = msgType,
                SystemBytes = systembyte
            };
            header.Bytes[0] = 0xFF;
            header.Bytes[1] = 0xFF;
            _socket.Send(new List<ArraySegment<byte>>(2){
                ControlMessageLengthBytes,
                new ArraySegment<byte>(header.Bytes)
            });
            _tracer.TraceInfo("Sent Control Message: " + header.MessageType);
        }

        /// <summary>
        /// 发送数据消息
        /// </summary>
        /// <param name="msg">主体消息内容</param>
        /// <param name="systembyte">系统字节</param>
        /// <param name="callback">异步操作结果</param>
        /// <param name="syncState">异步状态</param>
        /// <returns>异步返回操作状态对象</returns>
        SecsAsyncResult SendDataMessage(SecsMessage msg, AsyncCallback callback, object syncState)
        {
            // 处于为选择状态时不能发送数据消息
            if (this.State != ConnectionState.Selected)
                throw new SecsException("Device is not selected");
            int systembyte = msg.SystenBytes;
            var header = new Header(new byte[10])
            {
                S = msg.S,
                F = msg.F,
                ReplyExpected = msg.ReplyExpected,
                DeviceId = msg.DeviceID,
                SystemBytes = msg.SystenBytes
            };
            var buffer = new EncodedBuffer(header.Bytes, msg.RawDatas);

            SecsAsyncResult ar = new SecsAsyncResult(msg, systembyte, callback, syncState); ;
            if (msg.ReplyExpected)
            {
                ar = new SecsAsyncResult(msg, systembyte, callback, syncState);
                _replyExpectedMsgs[systembyte] = ar;

                ThreadPool.RegisterWaitForSingleObject(ar.AsyncWaitHandle,
                   (state, timeout) =>
                   {
                       SecsAsyncResult ars;
                       if (_replyExpectedMsgs.TryRemove((int)state, out ars) && timeout)
                       {
                           _tracer.TraceError(string.Format("T3 Timeout[id=0x{0:X8}]", ars.SystemByte));
                           ars.EndProcess(null, timeout);
                       }
                   }, systembyte, T3, true);
            }

            SocketError error;
            _socket.Send(buffer, SocketFlags.None, out error);
            if (error != SocketError.Success)
            {
                var errorMsg = "Socket send error :" + new SocketException((int)error).Message;
                _tracer.TraceError(errorMsg);
                this.CommunicationStateChanging(ConnectionState.Retry);
                throw new SecsException(errorMsg);
            }

            _tracer.TraceMessageOut(msg, systembyte);
            return ar;
        }
        #endregion

        #region Internal State Transition  状态转换

        /// <summary>
        /// 改变状态
        /// </summary>
        /// <param name="newState"></param>
        void CommunicationStateChanging(ConnectionState newState)
        {
            State = newState;
            if (ConnectionChanged != null)
                ConnectionChanged(this, new TEventArgs<ConnectionState>(State));

            switch (State)
            {
                case ConnectionState.Selected:
                    _timer7.Enabled = false;
                    _tracer.TraceInfo("Stop T7 Timer");
                    break;
                case ConnectionState.Connected:
                    _tracer.TraceInfo("Start T7 Timer");
                    _timer7.Interval = T7;
                    _timer7.Enabled = true;
                    break;
                case ConnectionState.Retry:
                    if (_isDisposed)
                        return;
                    Reset();
                    Thread.Sleep(2000);
                    StartImpl();
                    break;
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        void Reset()
        {
            this._timer7.Stop();
            this._timer8.Stop();
            this._timerLinkTest.Stop();
            this._secsDecoder.Reset();
            if (_socket != null)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                _socket = null;
            }
            this._replyExpectedMsgs.Clear();
            StopImpl();
        }
        #endregion

        #region Public API  公共接口
        /// <summary>
        /// 发送SECS消息到设备
        /// </summary>
        /// <param name="msg">SecsMessage对象</param>
        /// <returns>如果msg.ReplyExpected为true，则设备的回复msg;否则为null.</returns>
        public SecsMessage Send(SecsMessage msg)
        {
            return this.EndSend(this.BeginSend(msg, null, null));
        }

        /// <summary>
        /// 将SECS消息异步发送至设备
        /// </summary>
        /// <param name="msg">SecsMessage对象</param>
        /// <param name="callback">设备的回复消息处理程序回调.</param>
        /// <param name="state">同步状态对象.</param>
        /// <returns>如果msg.ReplyExpected为true，则引用异步发送的IAsyncResult，否则为空.</returns>
        public IAsyncResult BeginSend(SecsMessage msg, AsyncCallback callback, object state)
        {
            return this.SendDataMessage(msg, callback, state);
        }

        /// <summary>
        /// 结束异步发送
        /// </summary>
        /// <param name="asyncResult">引用异步发送的IAsyncResult</param>
        /// <returns>如果参数 <paramref name="asyncResult"/>是引用异步发送的IAsyncResult，则设备的回复消息，否则为空.</returns>
        public SecsMessage EndSend(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
                throw new ArgumentNullException("asyncResult");
            var ar = asyncResult as SecsAsyncResult;
            if (ar == null)
                throw new ArgumentException("參數asyncResult不是BeginSend所取得的", "asyncResult");
            ar.AsyncWaitHandle.WaitOne();
            return ar.Secondary;
        }

        volatile bool _isDisposed;

        /// <summary>
        /// 清除连接信息，并将所有设置重置
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                ConnectionChanged = null;
                if (State == ConnectionState.Selected)
                    this.SendControlMessage(MessageType.Seperate_req, NewSystemByte());
                Reset();
                _timer7.Dispose();
                _timer8.Dispose();
                _timerLinkTest.Dispose();
            }
        }

        public string DeviceAddress
        {
            get
            {
                return _isActive ? _ip.ToString() :
                    this._socket == null ? "N/A" : ((IPEndPoint)_socket.RemoteEndPoint).Address.ToString();
            }
        }
        #endregion

        #region Async Impl  异步
        sealed class SecsAsyncResult : IAsyncResult
        {
            readonly ManualResetEvent _ev = new ManualResetEvent(false);
            readonly SecsMessage Primary;
            readonly AsyncCallback _callback;
            public readonly int SystemByte;

            SecsMessage _secondary;
            bool _timeout;

            internal SecsAsyncResult(SecsMessage primaryMsg, int systembyte, AsyncCallback callback, object state)
            {
                Primary = primaryMsg;
                AsyncState = state;
                SystemByte = systembyte;
                _callback = callback;
            }

            internal void EndProcess(SecsMessage replyMsg, bool timeout)
            {
                if (replyMsg != null)
                {
                    _secondary = replyMsg;
                    _secondary.Name = Primary.Name;
                }
                _timeout = timeout;
                IsCompleted = !timeout;
                _ev.Set();
                if (_callback != null)
                    _callback(this);
            }

            internal SecsMessage Secondary
            {
                get
                {
                    if (_timeout) throw new SecsException(Primary, Resources.T3Timeout);
                    if (_secondary == null) return null;
                    if (_secondary.F == 0) throw new SecsException(Primary, Resources.SxF0);
                    if (_secondary.S == 9)
                    {
                        switch (_secondary.F)
                        {
                            case 1: throw new SecsException(Primary, Resources.S9F1);
                            case 3: throw new SecsException(Primary, Resources.S9F3);
                            case 5: throw new SecsException(Primary, Resources.S9F5);
                            case 7: throw new SecsException(Primary, Resources.S9F7);
                            case 9: throw new SecsException(Primary, Resources.S9F9);
                            case 11: throw new SecsException(Primary, Resources.S9F11);
                            case 13: throw new SecsException(Primary, Resources.S9F13);
                            default: throw new SecsException(Primary, Resources.S9Fy);
                        }
                    }
                    return _secondary;
                }
            }

            #region IAsyncResult Members

            public object AsyncState { get; private set; }

            public WaitHandle AsyncWaitHandle { get { return _ev; } }

            public bool CompletedSynchronously { get { return false; } }

            public bool IsCompleted { get; private set; }

            #endregion
        }
        #endregion

        #region SECS Decoder  解码
        sealed class SecsDecoder
        {
            delegate int Decoder(byte[] data, int length, ref int index, out int need);
            #region share
            uint _messageLength;// total byte length
            Header _msgHeader; // message header
            SecsMessage _msg;
            readonly Stack<List<Item>> _stack = new Stack<List<Item>>(); // List Item stack
            SecsFormat _format;
            byte _lengthBits;
            readonly byte[] _itemLengthBytes = new byte[4];
            int _itemLength;
            #endregion

            /// <summary>
            /// 解码通道
            /// </summary>
            readonly Decoder[] decoders;
            readonly Action<Header, SecsMessage> DataMsgHandler;
            readonly Action<Header> ControlMsgHandler;

            // 实例化Aciton委托DataMsgHandler，ControlMsgHandler，实例化委托Decoder
            internal SecsDecoder(Action<Header> controlMsgHandler, Action<Header, SecsMessage> msgHandler)
            {
                DataMsgHandler = msgHandler;
                ControlMsgHandler = controlMsgHandler;

                decoders = new Decoder[5];

                #region decoders[0]: get total message length 4 bytes
                decoders[0] = (byte[] data, int length, ref int index, out int need) =>
                {
                    if (!CheckAvailable(length, index, 4, out need)) return 0;

                    Array.Reverse(data, index, 4);
                    _messageLength = BitConverter.ToUInt32(data, index);
                    Trace.WriteLine("Get Message Length =" + _messageLength);
                    index += 4;

                    return 1;
                };
                #endregion
                #region decoders[1]: get message header 10 bytes
                decoders[1] = (byte[] data, int length, ref int index, out int need) =>
                {
                    if (!CheckAvailable(length, index, 10, out need)) return 1;

                    _msgHeader = new Header(new byte[10]);
                    Array.Copy(data, index, _msgHeader.Bytes, 0, 10);
                    index += 10;
                    _messageLength -= 10;
                    if (_messageLength == 0)
                    {
                        if (_msgHeader.MessageType == MessageType.Data_Message)
                        {
                            _msg = new SecsMessage(_msgHeader.DeviceId, _msgHeader.S, _msgHeader.F, string.Empty, _msgHeader.ReplyExpected, _msgHeader.SystemBytes, null);
                            ProcessMessage();
                        }
                        else
                        {
                            ControlMsgHandler(_msgHeader);
                            _messageLength = 0;
                        }
                        return 0;
                    }
                    else if (length - index >= _messageLength)
                    {
                        _msg = new SecsMessage(_msgHeader.DeviceId, _msgHeader.S, _msgHeader.F, string.Empty, _msgHeader.ReplyExpected, _msgHeader.SystemBytes, data, ref index);
                        ProcessMessage();
                        return 0; //completeWith message received
                    }
                    return 2;
                };
                #endregion
                #region decoders[2]: get _format + lengthBits(2bit) 1 byte
                decoders[2] = (byte[] data, int length, ref int index, out int need) =>
                {
                    if (!CheckAvailable(length, index, 1, out need)) return 2;

                    _format = (SecsFormat)(data[index] & 0xFC);
                    _lengthBits = (byte)(data[index] & 3);
                    index++;
                    _messageLength--;
                    return 3;
                };
                #endregion
                #region decoders[3]: get _itemLength _lengthBits bytes
                decoders[3] = (byte[] data, int length, ref int index, out int need) =>
                {
                    if (!CheckAvailable(length, index, _lengthBits, out need)) return 3;

                    Array.Copy(data, index, _itemLengthBytes, 0, _lengthBits);
                    Array.Reverse(_itemLengthBytes, 0, _lengthBits);

                    _itemLength = BitConverter.ToInt32(_itemLengthBytes, 0);
                    Array.Clear(_itemLengthBytes, 0, 4);

                    index += _lengthBits;
                    _messageLength -= _lengthBits;
                    return 4;
                };
                #endregion
                #region decoders[4]: get item value
                decoders[4] = (byte[] data, int length, ref int index, out int need) =>
                {
                    need = 0;
                    Item item = null;
                    if (_format == SecsFormat.List)
                    {
                        if (_itemLength == 0)
                            item = Item.L();
                        else
                        {
                            _stack.Push(new List<Item>(_itemLength));
                            return 2;
                        }
                    }
                    else
                    {
                        if (!CheckAvailable(length, index, _itemLength, out need)) return 4;

                        item = _itemLength == 0 ? _format.BytesDecode() : _format.BytesDecode(data, index, _itemLength);
                        index += _itemLength;
                        _messageLength -= (uint)_itemLength;
                    }
                    if (_stack.Count > 0)
                    {
                        var list = _stack.Peek();
                        list.Add(item);
                        while (list.Count == list.Capacity)
                        {
                            item = Item.L(_stack.Pop());
                            if (_stack.Count > 0)
                            {
                                list = _stack.Peek();
                                list.Add(item);
                            }
                            else
                            {
                                _msg = new SecsMessage(_msgHeader.DeviceId, _msgHeader.S, _msgHeader.F, string.Empty, _msgHeader.ReplyExpected, _msgHeader.SystemBytes, item);
                                ProcessMessage();
                                return 0;
                            }
                        }
                    }
                    return 2;
                };
                #endregion
            }

            void ProcessMessage()
            {
                DataMsgHandler(_msgHeader, _msg);
                _msg = null;
                _messageLength = 0;
            }

            static bool CheckAvailable(int length, int index, int requireCount, out int need)
            {
                need = requireCount - (length - index);
                return need <= 0;
            }

            public void Reset()
            {
                _msg = null;
                _stack.Clear();
                _currentStep = 0;
                _remainBytes = new ArraySegment<byte>();
                _messageLength = 0;
            }

            /// <summary>
            /// Offset: next fill index
            /// Cout : next fill count
            /// </summary>
            ArraySegment<byte> _remainBytes;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="bytes">位元組</param>
            /// <param name="index">有效位元的起始索引</param>
            /// <param name="length">有效位元長度</param>
            /// <returns>如果輸入的位元組經解碼後尚有不足則回傳true,否則回傳false</returns>
            public bool Decode(byte[] bytes, int index, int length)
            {
                if (_remainBytes.Count == 0)
                {
                    int need = Decode(bytes, length, ref index);
                    int remainLength = length - index;
                    if (remainLength > 0)
                    {
                        var temp = new byte[remainLength + need];
                        Array.Copy(bytes, index, temp, 0, remainLength);
                        _remainBytes = new ArraySegment<byte>(temp, remainLength, need);
                        Trace.WriteLine("Remain Lenght: " + _remainBytes.Offset + ", Need:" + _remainBytes.Count);
                    }
                    else
                    {
                        _remainBytes = new ArraySegment<byte>();
                    }
                }
                else if (length - index >= _remainBytes.Count)
                {
                    Array.Copy(bytes, index, _remainBytes.Array, _remainBytes.Offset, _remainBytes.Count);
                    index = _remainBytes.Count;
                    byte[] temp = _remainBytes.Array;
                    _remainBytes = new ArraySegment<byte>();
                    if (Decode(temp, 0, temp.Length))
                        Decode(bytes, index, length);
                }
                else
                {
                    int remainLength = length - index;
                    Array.Copy(bytes, index, _remainBytes.Array, _remainBytes.Offset, remainLength);
                    _remainBytes = new ArraySegment<byte>(_remainBytes.Array, _remainBytes.Offset + remainLength, _remainBytes.Count - remainLength);
                    Trace.WriteLine("Remain Lenght: " + _remainBytes.Offset + ", Need:" + _remainBytes.Count);
                }
                return _messageLength > 0;
            }

            int _currentStep;
            /// <summary>
            /// 將位元組通過decode pipeline處理
            /// </summary>
            /// <param name="bytes">位元組</param>
            /// <param name="length">有效位元的起始索引</param>
            /// <param name="index">位元組的起始索引</param>
            /// <returns>回傳_currentStep不足的byte數</returns>
            int Decode(byte[] bytes, int length, ref int index)
            {
                int need;
                int nexStep = _currentStep;
                do
                {
                    _currentStep = nexStep;
                    nexStep = decoders[_currentStep](bytes, length, ref index, out need);
                } while (nexStep != _currentStep);
                return need;
            }
        }
        #endregion

        #region Message Header Struct  消息头结构体
        /// <summary>
        /// 
        /// </summary>
        public struct Header
        {
            internal readonly byte[] Bytes;
            internal Header(byte[] headerbytes)
            {
                Bytes = headerbytes;
            }

            public short DeviceId
            {
                get
                {
                    return BitConverter.ToInt16(new[] { Bytes[1], Bytes[0] }, 0);
                }
                set
                {
                    byte[] values = BitConverter.GetBytes(value);
                    Bytes[0] = values[1];
                    Bytes[1] = values[0];
                }
            }
            public bool ReplyExpected
            {
                get { return (Bytes[2] & 0x80) == 0x80; }
                set { Bytes[2] = (byte)(S | (value ? 0x80 : 0)); }
            }
            public byte S
            {
                get { return (byte)(Bytes[2] & 0x7F); }
                set { Bytes[2] = (byte)(value | (ReplyExpected ? 0x80 : 0)); }
            }
            public byte F
            {
                get { return Bytes[3]; }
                set { Bytes[3] = value; }
            }
            public MessageType MessageType
            {
                get { return (MessageType)Bytes[5]; }
                set { Bytes[5] = (byte)value; }
            }
            public int SystemBytes
            {
                get
                {
                    return BitConverter.ToInt32(new[] {
                        Bytes[9],
                        Bytes[8],
                        Bytes[7],
                        Bytes[6]
                    }, 0);
                }
                set
                {
                    byte[] values = BitConverter.GetBytes(value);
                    Bytes[6] = values[3];
                    Bytes[7] = values[2];
                    Bytes[8] = values[1];
                    Bytes[9] = values[0];
                }
            }
        }
        #endregion

        #region EncodedByteList Wrapper just need IList<T>.Count and Indexer
        sealed class EncodedBuffer : IList<ArraySegment<byte>>
        {
            readonly IList<RawData> _data;// raw data include first message length 4 byte
            readonly byte[] _header;

            internal EncodedBuffer(byte[] header, IList<RawData> msgRawDatas)
            {
                _header = header;
                _data = msgRawDatas;
            }

            #region IList<ArraySegment<byte>> Members
            int IList<ArraySegment<byte>>.IndexOf(ArraySegment<byte> item) { return -1; }
            void IList<ArraySegment<byte>>.Insert(int index, ArraySegment<byte> item) { }
            void IList<ArraySegment<byte>>.RemoveAt(int index) { }
            ArraySegment<byte> IList<ArraySegment<byte>>.this[int index]
            {
                get { return new ArraySegment<byte>(index == 1 ? _header : _data[index].Bytes); }
                set { }
            }
            #endregion
            #region ICollection<ArraySegment<byte>> Members
            void ICollection<ArraySegment<byte>>.Add(ArraySegment<byte> item) { }
            void ICollection<ArraySegment<byte>>.Clear() { }
            bool ICollection<ArraySegment<byte>>.Contains(ArraySegment<byte> item) { return false; }
            void ICollection<ArraySegment<byte>>.CopyTo(ArraySegment<byte>[] array, int arrayIndex) { }
            int ICollection<ArraySegment<byte>>.Count { get { return _data.Count; } }
            bool ICollection<ArraySegment<byte>>.IsReadOnly { get { return true; } }
            bool ICollection<ArraySegment<byte>>.Remove(ArraySegment<byte> item) { return false; }
            #endregion
            #region IEnumerable<ArraySegment<byte>> Members
            public IEnumerator<ArraySegment<byte>> GetEnumerator()
            {
                for (int i = 0, length = _data.Count; i < length; i++)
                    yield return new ArraySegment<byte>(i == 1 ? _header : _data[i].Bytes);
            }
            #endregion
            #region IEnumerable Members
            IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
            #endregion
        }
        #endregion
    }
}