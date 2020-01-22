namespace MockSite.Common.Core.Models
{
    public class ConsulKvModel
    {
        public string LockIndex { get;  }

        public string Key { get;  }

        public int Flags { get;  }

        public string Value { get;  }

        public int CreateIndex { get;  }

        public int ModifyIndex { get;  }

        public ConsulKvModel(string key, string value, string lockIndex, int createIndex, int modifyIndex, int flags)
        {
            Key = key;
            Value = value;
            LockIndex = lockIndex;
            CreateIndex = createIndex;
            ModifyIndex = modifyIndex;
            Flags = flags;
        }
    }
}