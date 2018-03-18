using System;

namespace videozid.Utils
{
  [AttributeUsage(AttributeTargets.Property)]
  public class ExcelFormatAttribute : Attribute
  {
    public string ExcelFormat { get; set; } = string.Empty;

    public ExcelFormatAttribute(string format)
    {
      ExcelFormat = format;
    }
  }
}
