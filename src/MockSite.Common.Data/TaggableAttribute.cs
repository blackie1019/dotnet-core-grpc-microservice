#region

using System;

#endregion

namespace MockSite.Common.Data
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TaggableAttribute : Attribute
    {
    }
}