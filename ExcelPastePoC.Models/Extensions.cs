using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace ExcelPastePoC.Models
{
    public static class Extensions
    {
        public static T ToModel<T>(this DataRow dr)
            where T : DataModel, new()
        {
            var model = new T();
            foreach (DataColumn col in dr.Table.Columns)
            {
                var colName = col.ColumnName?.Trim();

                var p = model.GetType().GetProperty(colName);

                if (p != null && dr[col] != DBNull.Value)
                {
                    model.SetProperty(dr[col], p);
                }
            }
            return model;
        }

        public static string ToValidFileName(this string fileName)
        {
            fileName = string.Join(
                string.Empty,
                fileName.Split(Path.GetInvalidFileNameChars()));

            var apiInvalidChars = "£$€'".ToCharArray();
            fileName = string.Join(
                string.Empty,
                fileName.Split(apiInvalidChars));

            return fileName;
        }

        public static void SetProperty(this DataModel model, object input, PropertyInfo p)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (p is null)
            {
                throw new ArgumentNullException(nameof(p));
            }
            //var value = Convert.ChangeType(input, p.PropertyType);
            var propType = p.PropertyType;
            var fieldType = input.GetType();
            dynamic value = null;
            if (propType == fieldType)
            {
                if (fieldType == typeof(string))
                {
                    input = input.ToString().Trim();
                }
                value = input;
            }
            else if (propType == typeof(decimal) && fieldType.IsNumeric())
            {
                value = decimal.Parse(input.ToString());
            }
            else if (propType == typeof(decimal) && fieldType == typeof(string))
            {
                var result = decimal.TryParse(input.ToString(), out decimal dec);
                if (result)
                {
                    value = dec;
                }
            }
            else if (propType == typeof(string))
            {
                value = input.ToString().Replace(" 00:00:00", string.Empty);
            }
            else if (propType == typeof(DateTime))
            {
                var @try = input.ToString().TryToDateTime(out DateTime date);
                if (@try)
                {
                    value = date;
                }
                else
                {
                    value = DateTime.MinValue;
                }
            }
            else if (propType == typeof(DateTime?))
            {
                var @try = input.ToString().TryToDateTime(out DateTime date);
                if (@try)
                {
                    value = date;
                }
                else
                {
                    value = (DateTime?)null;
                }
            }
            else if (propType == typeof(bool))
            {
                var @try = bool.TryParse(input.ToString(), out bool boolean);
                if (@try)
                {
                    value = boolean;
                }
            }
            else if (propType == typeof(int))
            {
                var @try = int.TryParse(input.ToString(), out int i);
                if (@try)
                {
                    value = i;
                }
            }
            try
            {
                if (p.CanWrite)
                {
                    p.SetValue(model, value);
                }
                else
                {
                    //TrySetReadOnlyPropertyValue(p, value);
                    var s = p.DeclaringType.GetProperty(p.Name).GetSetMethod(true);
                    if (s != null)
                    {
                        s.Invoke(model, new object[] { value });
                    }
                }
            }
            catch (InvalidCastException ice)
            {
                throw new InvalidOperationException(
                    "Data type mismatch error on input field with type: " +
                    $"[{fieldType}] for model property: [{p.Name}] with" +
                    $"type: [{propType}]."
                    , ice);
            }
        }

        public static bool IsNumeric(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static bool TryToDateTime(this string s, out DateTime result)
        {
            var @try = false;
            result = DateTime.MinValue;
            if (string.IsNullOrEmpty(s))
            {
                return @try;
            }
            string[] formats =
            {
                "dd/MM/yyyy",
                "dddd, dd MMMM yyyy",
                "dddd, dd MMMM yyyy HH:mm:ss",
                "MMMM dd",
                "yyyy MMMM",
                "dd.MM.yy",
                "d.M.yy",
                "dd-MM-yyyy",
                "yyyy-MM-dd",
                "yyyy'-'MM'-'dd",
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK",
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss 'GMT'",
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss",
                "yyyy-MM-dd HH:mm:ss",
                "dd-MM-yyyy HH:mm:ss",
                "HH:mm",
                "HH:mm tt",
                "H:mm tt",
                "HH:mm:ss"
            };
            @try = DateTime.TryParseExact(
                s,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime date);
            if (@try || s == "0000-00-00")
            {
                result = date;
                @try = true;
            }
            return @try;
        }


    }

}
