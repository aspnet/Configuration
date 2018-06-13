using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;

namespace Microsoft.Extensions.Configuration
{
    /// <inheritdoc cref="TypeConverter"/>
    public sealed class IpAddressConverter : TypeConverter
    {
        /// <inheritdoc cref="TypeConverter.CanConvertFrom(ITypeDescriptorContext, Type)"/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            return base.CanConvertFrom(context, sourceType);
        }

        /// <inheritdoc cref="TypeConverter.ConvertFrom(ITypeDescriptorContext, CultureInfo, object)"/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var ipString = value as string;
            if (ipString != null) return IPAddress.Parse(ipString);
            return base.ConvertFrom(context, culture, value);
        }
    }
}