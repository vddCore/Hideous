using HidSharp;
using HidSharp.Reports;

namespace Hideous.Platform
{
    public record DeviceCharacteristics(
        ushort VendorID,
        ushort ProductID,
        ushort? UsagePage = null,
        ushort? UsageId = null,
        int? ReportLength = null)
    {
        public bool SatisfiedBy(HidDevice device)
        {
            var result = device.VendorID == VendorID 
                         && device.ProductID == ProductID;

            var reportDescriptor = device.GetReportDescriptor();
            
            if (result && UsagePage != null)
            {
                result &= HasUsagePage(reportDescriptor, UsagePage.Value);
            }

            if (result && UsageId != null)
            {
                result &= HasUsageId(reportDescriptor, UsageId.Value);
            }
            
            if (result && ReportLength != null)
            {
                result &= HasReportWithLength(reportDescriptor, ReportLength.Value);
            }

            return result;
        }

        private bool HasUsagePage(ReportDescriptor reportDescriptor, ushort usagePage)
        {
            foreach (var report in reportDescriptor.Reports)
            {
                foreach (var usageValue in report.GetAllUsages())
                {
                    var page = (ushort)((usageValue & 0xFFFF0000) >> 16);

                    if (page == usagePage)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        private bool HasUsageId(ReportDescriptor reportDescriptor, ushort usageId)
        {
            foreach (var report in reportDescriptor.Reports)
            {
                foreach (var usageValue in report.GetAllUsages())
                {
                    var id = (ushort)(usageValue & 0xFFFF);

                    if (id == usageId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        private bool HasReportWithLength(ReportDescriptor reportDescriptor, int reportLength)
        {
            foreach (var report in reportDescriptor.Reports)
            {
                if (report.Length == reportLength)
                {
                    return true;
                }
            }

            return false;
        }
    }
}