using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Security.Permissions;
using System.Text;

namespace Secs4Net
{
    [DebuggerDisplay("<{Format} [{Count}] { (Format==SecsFormat.List) ? string.Empty : ToString() ,nq}>")]
    public sealed class Item : MarshalByRefObject {
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService() {
            ILease lease = (ILease)base.InitializeLifetimeService();
            if (lease.CurrentState == LeaseState.Initial) {
                lease.InitialLeaseTime = TimeSpan.FromSeconds(10);
                lease.RenewOnCallTime = TimeSpan.FromSeconds(10);
            }
            return lease;
        }


        private readonly IEnumerable _values;

        public SecsFormat Format { get; private set; }
        public int Count { get; private set; }

        /// <summary>
        /// List items
        /// </summary>
        public ReadOnlyCollection<Item> Items {
            get {
                if (_list == null) throw new InvalidOperationException("This is not a List.");
                return _list;
            }
        }

        /// <summary>
        /// get value by specific type
        /// </summary>
        public T GetValue<T>() {
            if (_value == null)
                throw new InvalidOperationException("This is a List.");

            if (Format == SecsFormat.ASCII || Format == SecsFormat.JIS8)
                throw new InvalidOperationException("The item is a string");

            if (_value is T)
                return (T)((ICloneable)_value).Clone();

            if (_value is T[])
                return ((T[])_value)[0];

            Type valueType = Nullable.GetUnderlyingType(typeof(T));
            if (valueType != null && _value.GetType().GetElementType() == valueType)
                return ((IEnumerable)_value).Cast<T>().FirstOrDefault();

            throw new SecsException("Access item value with wrong type.");
        }


        public string GetString() => Format != SecsFormat.ASCII && Format != SecsFormat.JIS8
            ? throw new InvalidOperationException("The type is incompatible")
            //: Unsafe.As<string>(_value);
            //: Encoding.ASCII.GetString((byte[])_value);
            : _value.ToString();


        /// <summary>
        /// get value array by specific type
        /// </summary>
        public T[] GetValues<T>() where T : struct
        {
            if (Format == SecsFormat.List)
                throw new InvalidOperationException("The item is list");

            if (Format == SecsFormat.ASCII || Format == SecsFormat.JIS8)
                throw new InvalidOperationException("The item is a string");

            if (_values is T[] arr)
                return arr;

            throw new InvalidOperationException("The type is incompatible");
        }

        public RawData RawData { get { return _rawBytes.Value; } }

        public override string ToString() { return _sml.Value; }

        /// <summary>
        /// if Format is List RawBytes is only header bytes.
        /// otherwise include header and data bytes.
        /// </summary>
        readonly Lazy<RawData> _rawBytes;
        readonly Lazy<string> _sml;
        readonly ReadOnlyCollection<Item> _list;  //  當Format為List時 _list才有值,否則為null
        readonly object _value;      //  當Format不為List時 _value才有值,否則為null;不是string就是Array   

        #region Constructor   构造函数
        /// <summary>
        /// List
        /// </summary>
        Item(ReadOnlyCollection<Item> items) {
            Format = SecsFormat.List;
            Count = items.Count;
            _list = items;
            _sml = EmptySml;
            _rawBytes = Lazy.Create(() => {
                int _;
                return new RawData(Format.EncodeItem(Count, out _));
            });
        }

        /// <summary>
        /// U2,U4,U8
        /// I1,I2,I4,I8
        /// F4,F8
        /// Boolean
        /// </summary>
        Item(SecsFormat format, Array value, Lazy<string> sml) {
            Format = format;
            Count = value.Length;
            _value = value;
            _sml = sml;
            _rawBytes = Lazy.Create(() => {
                Array val = (Array)_value;
                int bytelength = Buffer.ByteLength(val);
                int headerLength;
                byte[] result = Format.EncodeItem(bytelength, out headerLength);
                Buffer.BlockCopy(val, 0, result, headerLength, bytelength);
                result.Reverse(headerLength, headerLength + bytelength, bytelength / val.Length);
                return new RawData(result);
            });
        }

        /// <summary>
        /// A,J
        /// </summary>
        Item(SecsFormat format, string value, Encoding encoder) {
            Format = format;
            Count = value.Length;
            _value = value;
            _sml = Lazy.Create(value);
            _rawBytes = Lazy.Create(() => {
                string str = (string)_value;
                int headerLength;
                byte[] result = Format.EncodeItem(str.Length, out headerLength);
                encoder.GetBytes(str, 0, str.Length, result, headerLength);
                return new RawData(result);
            });
        }

        /// <summary>
        /// Empty Item(none List)
        /// </summary>
        /// <param name="format"></param>
        /// <param name="value"></param>
        Item(SecsFormat format, ICloneable value) {
            Format = format;
            _value = value;
            _rawBytes = Lazy.Create(new RawData(new byte[] { (byte)((byte)Format | 1), 0 }));
            _sml = EmptySml;
        }
        #endregion

        #region Type Casting Operator

        public static explicit operator string(Item item) {
            return item.GetString();
        }

        public static explicit operator byte(Item item) {
            return item.GetValue<byte>();      
        }

        public static explicit operator sbyte(Item item) {
            return item.GetValue<sbyte>();
        }

        public static explicit operator ushort(Item item) {
            return item.GetValue<ushort>();
        }

        public static explicit operator short(Item item) {
            return item.GetValue<short>();
        }

        public static explicit operator uint(Item item) {
            return item.GetValue<uint>();
        }

        public static explicit operator int(Item item) {
            return item.GetValue<int>();
        }

        public static explicit operator ulong(Item item) {
            return item.GetValue<ulong>();
        }

        public static explicit operator long(Item item) {
            return item.GetValue<long>();
        }

        public static explicit operator float(Item item) {
            return item.GetValue<float>();
        }

        public static explicit operator double(Item item) {
            return item.GetValue<double>();
        }

        public static explicit operator bool(Item item) {
            return item.GetValue<bool>();
        }

        public static explicit operator byte?(Item item) {
            return item.GetValue<byte?>();
        }

        public static explicit operator sbyte?(Item item) {
            return item.GetValue<sbyte?>();
        }

        public static explicit operator ushort?(Item item) {
            return item.GetValue<ushort?>();
        }

        public static explicit operator short?(Item item) {
            return item.GetValue<short?>();
        }

        public static explicit operator uint?(Item item) {
            return item.GetValue<uint?>();
        }

        public static explicit operator int?(Item item) {
            return item.GetValue<int?>();
        }

        public static explicit operator ulong?(Item item) {
            return item.GetValue<ulong?>();
        }

        public static explicit operator long?(Item item) {
            return item.GetValue<long?>();
        }

        public static explicit operator float?(Item item) {
            return item.GetValue<float?>();
        }

        public static explicit operator double?(Item item) {
            return item.GetValue<double?>();
        }

        public static explicit operator bool?(Item item) {
            return item.GetValue<bool?>();
        }

        public static explicit operator byte[](Item item) {
            return item.GetValue<byte[]>();
        }

        public static explicit operator sbyte[](Item item) {
            return item.GetValue<sbyte[]>();
        }

        public static explicit operator ushort[](Item item) {
            return item.GetValue<ushort[]>();
        }

        public static explicit operator short[](Item item) {
            return item.GetValue<short[]>();
        }

        public static explicit operator uint[](Item item) {
            return item.GetValue<uint[]>();
        }

        public static explicit operator int[](Item item) {
            return item.GetValue<int[]>();
        }

        public static explicit operator ulong[](Item item) {
            return item.GetValue<ulong[]>();
        }

        public static explicit operator long[](Item item) {
            return item.GetValue<long[]>();
        }

        public static explicit operator float[](Item item) {
            return item.GetValue<float[]>();
        }

        public static explicit operator double[](Item item) {
            return item.GetValue<double[]>();
        }

        public static explicit operator bool[](Item item) {
            return item.GetValue<bool[]>();
        }

        #endregion

        #region Factory Methods  方法
        internal static Item L(IList<Item> items) {
            return new Item(new ReadOnlyCollection<Item>(items));
        }
        public static Item L(IEnumerable<Item> items) {
            return items.Any() ? L(items.ToList()) : L();
        }
        public static Item L(params Item[] items) {
            return L(items.ToList());
        }
        public static Item B(params byte[] value) {
            return new Item(SecsFormat.Binary, value, new Lazy<string>(value.ToHexString));
        }
        public static Item U1(params byte[] value) {
            return new Item(SecsFormat.U1, value, new Lazy<string>(value.ToSmlString));
        }
        public static Item U2(params ushort[] value) {
            return new Item(SecsFormat.U2, value, new Lazy<string>(value.ToSmlString));
        }
        public static Item U4(params uint[] value) {
            return new Item(SecsFormat.U4, value, new Lazy<string>(value.ToSmlString));
        }
        public static Item U8(params ulong[] value) {
            return new Item(SecsFormat.U8, value, new Lazy<string>(value.ToSmlString));
        }
        public static Item I1(params sbyte[] value) {
            return new Item(SecsFormat.I1, value, new Lazy<string>(value.ToSmlString));
        }
        public static Item I2(params short[] value) {
            return new Item(SecsFormat.I2, value, new Lazy<string>(value.ToSmlString));
        }
        public static Item I4(params int[] value) {
            return new Item(SecsFormat.I4, value, new Lazy<string>(value.ToSmlString));
        }
        public static Item I8(params long[] value) {
            return new Item(SecsFormat.I8, value, new Lazy<string>(value.ToSmlString));
        }
        public static Item F4(params float[] value) {
            return new Item(SecsFormat.F4, value, new Lazy<string>(value.ToSmlString));
        }
        public static Item F8(params double[] value) {
            return new Item(SecsFormat.F8, value, new Lazy<string>(value.ToSmlString));
        }
        public static Item Boolean(params bool[] value) {
            return new Item(SecsFormat.Boolean, value, new Lazy<string>(value.ToSmlString));
        }
        public static Item A(string value) {
            return new Item(SecsFormat.ASCII, value, Encoding.ASCII);
        }
        public static Item J(string value) {
            return new Item(SecsFormat.JIS8, value, JIS8Encoding);
        }
        #endregion

        #region Empty Item Factory  空元组方法
        public static Item L() { return Empty_L; }
        public static Item B() { return Empty_Binary; }
        public static Item U1() { return Empty_U1; }
        public static Item U2() { return Empty_U2; }
        public static Item U4() { return Empty_U4; }
        public static Item U8() { return Empty_U8; }
        public static Item I1() { return Empty_I1; }
        public static Item I2() { return Empty_I2; }
        public static Item I4() { return Empty_I4; }
        public static Item I8() { return Empty_I8; }
        public static Item F4() { return Empty_F4; }
        public static Item F8() { return Empty_F8; }
        public static Item Boolean() { return Empty_Boolean; }
        public static Item A() { return Empty_A; }
        public static Item J() { return Empty_J; }
        #endregion

        #region Share Object  共享对象
        internal static readonly Encoding JIS8Encoding = Encoding.GetEncoding(50222);
        internal static readonly Lazy<string> EmptySml = Lazy.Create(string.Empty);
        static readonly Item Empty_L       = new Item(Array.AsReadOnly(new Item[0]));
        static readonly Item Empty_A       = new Item(SecsFormat.ASCII, string.Empty);
        static readonly Item Empty_J       = new Item(SecsFormat.JIS8, string.Empty);
        static readonly Item Empty_Boolean = new Item(SecsFormat.Boolean, new bool[0]);
        static readonly Item Empty_Binary  = new Item(SecsFormat.Binary, new byte[0]);
        static readonly Item Empty_U1      = new Item(SecsFormat.U1, new byte[0]);
        static readonly Item Empty_U2      = new Item(SecsFormat.U2, new ushort[0]);
        static readonly Item Empty_U4      = new Item(SecsFormat.U4, new uint[0]);
        static readonly Item Empty_U8      = new Item(SecsFormat.U8, new ulong[0]);
        static readonly Item Empty_I1      = new Item(SecsFormat.I1, new sbyte[0]);
        static readonly Item Empty_I2      = new Item(SecsFormat.I2, new short[0]);
        static readonly Item Empty_I4      = new Item(SecsFormat.I4, new int[0]);
        static readonly Item Empty_I8      = new Item(SecsFormat.I8, new long[0]);
        static readonly Item Empty_F4      = new Item(SecsFormat.F4, new float[0]);
        static readonly Item Empty_F8      = new Item(SecsFormat.F8, new double[0]);
        #endregion
    }
}