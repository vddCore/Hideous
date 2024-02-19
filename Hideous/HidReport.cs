using System.Text;
using Hideous.DescriptorParser;

namespace Hideous
{
    public abstract class HidReport
    {
        private List<ushort> _usageIds = new();

        public bool UsesId { get; }
        public byte ReportId { get; }

        public int BitsPerField { get; }
        public int FieldCount { get; }

        public int DataLength => (BitsPerField / 8) * FieldCount;
        
        public ushort UsagePage { get; }
        public IReadOnlyList<ushort> UsageIds => _usageIds;

        internal HidReport(byte reportId, int bitsPerField, int fieldCount, ushort usagePage, ushort usageId)
        {
            ReportId = reportId;
            UsesId = true;

            BitsPerField = bitsPerField;
            FieldCount = fieldCount;
            
            UsagePage = usagePage;

            _usageIds.Add(usageId);
        }

        internal HidReport(int bitsPerField, int fieldCount, ushort usagePage, ushort usageId)
        {
            ReportId = 0;
            UsesId = false;

            BitsPerField = bitsPerField;
            FieldCount = fieldCount;
            
            UsagePage = usagePage;

            _usageIds.Add(usageId);
        }

        internal void AddUsagesFromItem(Item item)
        {
            foreach (var usage in item.GetItems(LocalTag.Usage))
            {
                _usageIds.Add((ushort)usage.Value);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("{");

            if (UsesId)
            {
                sb.AppendLine($"    Report ID: 0x{ReportId:X2} ({ReportId})");
            }

            sb.AppendLine($"    Bits per data field: {BitsPerField}");
            sb.AppendLine($"    Data field count: {FieldCount}");
            sb.AppendLine($"    Data length: {DataLength} byte(s)");
            sb.AppendLine($"    Usage page: 0x{UsagePage:X4} ({UsagePage})");
            sb.AppendLine($"    Usage ID(s): [");

            for (var i = 0; i < _usageIds.Count; i++)
            {
                sb.Append($"        0x{_usageIds[i]:X4} ({_usageIds[i]})");

                if (i < _usageIds.Count - 1)
                    sb.Append(",");

                sb.AppendLine();
            }

            sb.AppendLine("    ]");

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}