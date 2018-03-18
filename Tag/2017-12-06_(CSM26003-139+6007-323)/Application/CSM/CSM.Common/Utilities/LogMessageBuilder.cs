using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace CSM.Common.Utilities
{
    public class LogMessageBuilder : Dictionary<string, string>
    {
        protected const string lastLogTimeKey = "__LAST_LOG_TIME_KEY__";
        protected const string totalLogTimeKey = "__TOTAL_LOG_TIME_KEY__";
        private static readonly Hashtable _threadDbContexts = new Hashtable();
        protected string _prefixMsg = string.Empty;
        protected string _suffixMsg = string.Empty;

        protected DateTime FirstLogTime
        {
            get
            {
                if (HttpContext.Current != null) // ใช้ในกรณีเป็น Web App
                {
                    if (!HttpContext.Current.Items.Contains(totalLogTimeKey))
                    {
                        HttpContext.Current.Items[totalLogTimeKey] = DateTime.Now;
                    }

                    return (DateTime) HttpContext.Current.Items[totalLogTimeKey];
                }
                lock (_threadDbContexts.SyncRoot)
                {
                    if (!_threadDbContexts.Contains(totalLogTimeKey))
                    {
                        _threadDbContexts[totalLogTimeKey] = DateTime.Now;
                    }

                    return (DateTime) _threadDbContexts[totalLogTimeKey];
                }
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Items[totalLogTimeKey] = value;
                else
                    _threadDbContexts[totalLogTimeKey] = value;
            }
        }

        protected DateTime LastLogTime
        {
            get
            {
                if (HttpContext.Current != null) // ใช้ในกรณีเป็น Web App
                {
                    if (!HttpContext.Current.Items.Contains(lastLogTimeKey))
                    {
                        HttpContext.Current.Items[lastLogTimeKey] = FirstLogTime = DateTime.Now;
                    }

                    return (DateTime) HttpContext.Current.Items[lastLogTimeKey];
                }
                lock (_threadDbContexts.SyncRoot) // ใช้ในกรณีเป็น Standalone App
                {
                    if (!_threadDbContexts.Contains(lastLogTimeKey))
                    {
                        _threadDbContexts[lastLogTimeKey] = FirstLogTime = DateTime.Now;
                    }

                    return (DateTime) _threadDbContexts[lastLogTimeKey];
                }
            }
            set
            {
                if (HttpContext.Current != null) // ใช้ในกรณีเป็น Web App
                    HttpContext.Current.Items[lastLogTimeKey] = value;
                else // ใช้ในกรณีเป็น Standalone App
                    _threadDbContexts[lastLogTimeKey] = value;
            }
        }

        public string TotalTime
        {
            get { return (LastLogTime - FirstLogTime).TotalMilliseconds.ToString(); }
        }

        protected string ElapseTime
        {
            get
            {
                DateTime now = DateTime.Now;
                double elapseTime = (now - LastLogTime).TotalMilliseconds;
                LastLogTime = now;

                return elapseTime.ToString();
            }
        }

        private bool IsPrimitiveType(Type type)
        {
            return (type == typeof (object) || Type.GetTypeCode(type) != TypeCode.Object);
        }

        public LogMessageBuilder SetPrefixMsg(string msg)
        {
            _prefixMsg = msg;
            return this;
        }

        public LogMessageBuilder SetSuffixMsg(string msg)
        {
            _suffixMsg = msg;
            return this;
        }

        public LogMessageBuilder Add(string key, object value)
        {
            base.Add(key, value.ConvertToString());
            return this;
        }

        public LogMessageBuilder AddObject(object obj, string[] exclude = null)
        {
            return AddObject(obj, null, exclude);
        }

        public LogMessageBuilder AddObject(object obj, object parent, string[] exclude = null)
        {
            if (obj == null)
                return this;

            string parentName = parent != null ? parent.GetType().Name : string.Empty;

            foreach (PropertyInfo prop in obj.GetType().GetProperties())
            {
                try
                {
                    if (exclude != null &&
                        (exclude.Contains(prop.Name) || exclude.Contains(parentName + "::" + prop.Name)))
                        continue;

                    Type type = prop.PropertyType;
                    object value = prop.GetValue(obj, null);

                    if (!IsPrimitiveType(type) && value != null)
                    {
                        AddObject(value, obj, exclude);
                        continue;
                    }

                    if (parent != null)
                        base.Add(parentName + "::" + prop.Name, value != null ? value.ToString() : string.Empty);
                    else
                        base.Add(prop.Name, value != null ? value.ToString() : string.Empty);
                }
                catch (Exception)
                {
                    if (parent != null)
                        base.Add(parentName + "::" + prop.Name, "N/A");
                    else
                        base.Add(prop.Name, "N/A");
                }
            }

            return this;
        }

        public LogMessageBuilder Remove(string key)
        {
            base.Remove(key);
            return this;
        }

        public LogMessageBuilder Clear()
        {
            _prefixMsg = string.Empty;
            _suffixMsg = string.Empty;
            base.Clear();
            return this;
        }

        public string ToInputLogString()
        {
            var builder = new StringBuilder();
            builder.Append("ElapsedTime: " + ElapseTime + ": TotalTime: " + TotalTime + ": I:--START--");

            if (!string.IsNullOrEmpty(_prefixMsg))
                builder.Append(":--" + _prefixMsg + "--");

            foreach (string key in Keys)
            {
                builder.Append(":" + key + "/" + this[key]);
            }

            if (!string.IsNullOrEmpty(_suffixMsg))
                builder.Append(":--" + _suffixMsg + "--");

            return builder.ToString();
        }

        public string ToOutputLogString()
        {
            var builder = new StringBuilder();
            builder.Append("ElapsedTime: " + ElapseTime + ": TotalTime: " + TotalTime + ": O");

            if (!string.IsNullOrEmpty(_prefixMsg))
                builder.Append(":--" + _prefixMsg + "--");

            foreach (string key in Keys)
            {
                builder.Append(":" + key + "/" + this[key]);
            }

            if (!string.IsNullOrEmpty(_suffixMsg))
                builder.Append(":--" + _suffixMsg + "--");

            return builder.ToString();
        }

        public string ToSuccessLogString()
        {
            var builder = new StringBuilder();
            builder.Append("ElapsedTime: " + ElapseTime + ": TotalTime: " + TotalTime + ":O:--SUCCESS--");

            if (!string.IsNullOrEmpty(_prefixMsg))
                builder.Append(":--" + _prefixMsg + "--");

            foreach (string key in Keys)
            {
                builder.Append(":" + key + "/" + this[key]);
            }

            if (!string.IsNullOrEmpty(_suffixMsg))
                builder.Append(":--" + _suffixMsg + "--");

            return builder.ToString();
        }

        public string ToFailLogString()
        {
            var builder = new StringBuilder();
            builder.Append("ElapsedTime: " + ElapseTime + ": TotalTime: " + TotalTime + ":O:--FAIL--");

            if (!string.IsNullOrEmpty(_prefixMsg))
                builder.Append(":--" + _prefixMsg + "--");

            foreach (string key in Keys)
            {
                builder.Append(":" + key + "/" + this[key]);
            }

            if (!string.IsNullOrEmpty(_suffixMsg))
                builder.Append(":--" + _suffixMsg + "--");

            return builder.ToString();
        }
    }
}